using System.Text.Json;
using HuntHelperEvolved.Ipc;

namespace Umbra.HuntHelperEvolved;

// The UI adapter translates only a missing V2 IPC gate into this exception.
// A faulty or malformed V2 provider must not silently display V1's current world.
public sealed class WorldTrainStatusUnavailableException(Exception? innerException = null)
    : Exception("The world-specific train-status IPC gate is unavailable.", innerException);

// Keep IPC failures and reloads outside the widget's drawing lifecycle.
public sealed class TrainConnection(Func<string> read, Func<bool> toggle, Func<DateTime>? utcNow = null,
    Func<uint, string>? readWorld = null)
{
    public TrainStatusSnapshot? Snapshot { get; private set; }
    public string UnavailableReason { get; private set; } = "Enable Hunt Helper Evolved with train-status support.";

    public void Refresh(uint worldId = 0)
    {
        Snapshot = null;
        UnavailableReason = "Enable Hunt Helper Evolved with train-status support.";
        try
        {
            string response;
            var expectedVersion = TrainStatusContract.WorldSnapshotVersion;
            try
            {
                response = readWorld is null ? throw new WorldTrainStatusUnavailableException() : readWorld(worldId);
            }
            catch (WorldTrainStatusUnavailableException)
            {
                if (worldId != 0)
                {
                    UnavailableReason = "Update Hunt Helper Evolved to use a fixed world.";
                    return;
                }
                expectedVersion = TrainStatusContract.Version;
                response = read();
            }
            var value = JsonSerializer.Deserialize<TrainStatusSnapshot>(response);
            if (value == null || value.Version != expectedVersion)
            {
                UnavailableReason = "Update Hunt Helper Evolved and this Umbra companion to compatible versions.";
                return;
            }
            var age = (utcNow?.Invoke() ?? DateTime.UtcNow) - value.UpdatedAtUtc;
            if (age > TimeSpan.FromSeconds(5) || age < TimeSpan.FromSeconds(-5))
            {
                UnavailableReason = "Waiting for a fresh Hunt Helper Evolved train status.";
                return;
            }
            if (!Valid(value))
            {
                UnavailableReason = "Hunt Helper Evolved returned an invalid train status.";
                return;
            }
            if (worldId != 0 && value.LoggedIn && value.WorldId != worldId)
            {
                UnavailableReason = "Waiting for Hunt Helper Evolved to return the selected world's train status.";
                return;
            }
            Snapshot = value;
            if (!value.LoggedIn && worldId != 0)
                UnavailableReason = "Waiting for the selected world's train status. Log in or wait for your area transfer to finish.";
        }
        catch (Exception)
        {
            // A missing or reloading provider clears old progress and retries on the next poll.
        }
    }

    public bool Toggle(uint worldId = 0)
    {
        Refresh(worldId);
        if (Snapshot is not { LoggedIn: true }) return false;
        try
        {
            if (toggle()) return true;
        }
        catch (Exception) { }
        Snapshot = null;
        UnavailableReason = "Could not toggle the train popout. Waiting for Hunt Helper Evolved.";
        return false;
    }

    private static bool Valid(TrainStatusSnapshot value)
    {
        if (value.Expansions == null || value.WorldName == null) return false;
        if (!value.LoggedIn) return value.WorldId == 0 && value.Expansions.Length == 0;
        var expectedExpansions = value.Version == TrainStatusContract.WorldSnapshotVersion
            ? TrainPresentation.AllExpansionCodes : ["DT", "EW", "ShB"];
        if (value.WorldId == 0 || string.IsNullOrWhiteSpace(value.WorldName)
            || value.Expansions.Length != expectedExpansions.Count) return false;
        var expansions = new HashSet<string>(StringComparer.Ordinal);
        foreach (var row in value.Expansions)
        {
            if (row == null || !expectedExpansions.Contains(row.Expansion) || !expansions.Add(row.Expansion)
                || row.Total <= 0 || row.Total > 1000 || row.Recorded < 0 || row.Recorded > 1000
                || row.KnownProgressCount < 0 || row.KnownProgressCount > row.Total) return false;
            if (row.KnownProgressCount == 0)
            {
                if (row.Progress != null) return false;
            }
            else if (row.Progress is not { } progress || !double.IsFinite(progress) || progress < 0 || progress > 1)
                return false;
            if (row.CompletedProgressCount is { } completed)
            {
                if (completed < 0 || completed > row.KnownProgressCount) return false;
                // Leave missing counts compatible with earlier HHE previews;
                // the presentation keeps their bar unknown and requests an update.
                if (row.KnownProgressCount > 0 && (row.Progress!.Value * row.KnownProgressCount < completed - 1e-9
                    || completed == row.KnownProgressCount && row.Progress.Value != 1)) return false;
            }
            if (value.WorldOffline && row.KnownProgressCount != 0) return false;
        }
        return true;
    }
}
