using System.Text.Json;
using HuntHelperEvolved.Ipc;

namespace Umbra.HuntHelperEvolved;

// Keep IPC failures and reloads outside the widget's drawing lifecycle.
public sealed class TrainConnection(Func<string> read, Func<bool> toggle, Func<DateTime>? utcNow = null)
{
    public TrainStatusSnapshot? Snapshot { get; private set; }
    public string UnavailableReason { get; private set; } = "Enable Hunt Helper Evolved with train-status support.";

    public void Refresh()
    {
        Snapshot = null;
        UnavailableReason = "Enable Hunt Helper Evolved with train-status support.";
        try
        {
            var value = JsonSerializer.Deserialize<TrainStatusSnapshot>(read());
            if (value == null || value.Version != TrainStatusContract.Version)
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
            Snapshot = value;
        }
        catch (Exception)
        {
            // A missing or reloading provider clears old progress and retries on the next poll.
        }
    }

    public bool Toggle()
    {
        Refresh();
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
        if (value.WorldId == 0 || value.Expansions.Length != 3) return false;
        var expansions = new HashSet<string>(StringComparer.Ordinal);
        foreach (var row in value.Expansions)
        {
            if (row == null || row.Expansion is not ("DT" or "EW" or "ShB") || !expansions.Add(row.Expansion)
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
