using System.Globalization;
using HuntHelperEvolved.Ipc;

namespace Umbra.HuntHelperEvolved;

public sealed record TrainDisplay(string Text, string Tooltip, int Progress);

public static class TrainPresentation
{
    private static readonly string[] Expansions = ["DT", "EW", "ShB"];
    public static string NormalizeMode(string? mode) => mode is "DT" or "EW" or "ShB" ? mode : "all";
    public static string NextMode(string? mode) => NormalizeMode(mode) switch
    {
        "all" => "DT", "DT" => "EW", "EW" => "ShB", _ => "all",
    };

    public static TrainDisplay Create(TrainStatusSnapshot? snapshot, string? mode, string unavailableReason)
    {
        const string controls = "Left-click: toggle train popout\nRight-click: All → DT → EW → ShB";
        if (snapshot == null) return new("HHE unavailable", unavailableReason + "\n\n" + controls, 0);
        if (!snapshot.LoggedIn) return new("HHE: waiting", "Log in or wait for your area transfer to finish.\n\n" + controls, 0);

        mode = NormalizeMode(mode);
        var rows = Expansions.Where(expansion => mode == "all" || expansion == mode)
            .Select(expansion => snapshot.Expansions.Single(row => row.Expansion == expansion)).ToArray();
        var text = string.Join(", ", rows.Select(row => $"{row.Expansion}:{row.Recorded}/{row.Total}"));
        var known = rows.Sum(row => row.KnownProgressCount);
        var total = rows.Sum(row => row.Total);
        var average = known == 0 ? 0 : rows.Sum(row => (row.Progress ?? 0) * row.KnownProgressCount) / known;
        var lines = new List<string>
        {
            $"Hunt Helper Evolved · {snapshot.WorldName}",
            "Train list / total possible A-rank marks",
            "Includes retained dead and sniped marks; excludes flags.",
            "Totals include known zone instances.",
            "",
        };
        foreach (var row in rows)
        {
            var progress = row.Progress is { } fraction ? Percent(fraction) : "unknown";
            lines.Add($"{row.Expansion}: {row.Recorded}/{row.Total} · spawn progress {progress}");
        }
        if (rows.Any(row => row.Recorded > row.Total))
            lines.Add("The train retains marks beyond the current instance roster.");
        lines.Add(known == 0 ? "Average spawn progress: unknown" : $"Average spawn progress: {Percent(average)} ({known}/{total} marks known)");
        if (known < total) lines.Add($"{total - known} unknown marks excluded from the average.");
        lines.Add("Progress measures elapsed respawn-window time, not spawn probability.");
        if (snapshot.WorldOffline) lines.Add("World offline: spawn progress unavailable.");
        else if (!snapshot.SyncConnected) lines.Add("Shared sync disconnected; using HHE's retained local data.");
        lines.Add("");
        lines.Add(controls);
        return new(text, string.Join("\n", lines), (int)Math.Round(Math.Clamp(average, 0, 1) * 10000));
    }

    private static string Percent(double value) => (value * 100).ToString("0.#", CultureInfo.InvariantCulture) + "%";
}
