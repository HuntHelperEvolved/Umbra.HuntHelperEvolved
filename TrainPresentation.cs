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
        var progress = Summarize(rows);
        var lines = new List<string>
        {
            $"Hunt Helper Evolved · {snapshot.WorldName}",
            "Train list / total possible A-rank marks",
            "Listed count excludes killed/sniped marks and flags.",
            "Totals include known zone instances.",
            "",
        };
        foreach (var row in rows)
        {
            var expansion = Summarize([row]);
            lines.Add($"{row.Expansion}: {row.Recorded}/{row.Total} · remaining spawn progress {Percent(expansion)}");
        }
        if (rows.Any(row => row.Recorded > row.Total))
            lines.Add("The train retains marks beyond the current instance roster.");
        lines.Add($"Remaining-mark spawn progress: {Percent(progress)}");
        if (progress.Completed is null)
            lines.Add("Update HHE to a build with remaining-mark progress support.");
        else if (progress.AllComplete)
            lines.Add($"All {progress.Total} marks are at 100%.");
        else
        {
            lines.Add($"{progress.Completed}/{progress.Total} marks at 100%, excluded from the average.");
            if (progress.Remaining > 0) lines.Add($"Averaging {progress.Remaining} known marks below 100%.");
        }
        if (progress.Known < progress.Total)
            lines.Add($"{progress.Total - progress.Known} unknown {(progress.Total - progress.Known == 1 ? "mark" : "marks")} excluded from the average; full completion is not confirmed.");
        lines.Add("Progress measures elapsed respawn-window time, not spawn probability.");
        if (snapshot.WorldOffline) lines.Add("World offline: spawn progress unavailable.");
        else if (!snapshot.SyncConnected) lines.Add("Shared sync disconnected; using HHE's retained local data.");
        lines.Add("");
        lines.Add(controls);
        // Never round a nearly-complete window to a full bar. Only the roster
        // completion count can confirm that every selected mark is at 100%.
        var bar = progress.AllComplete ? 10000 : Math.Min(9999, (int)Math.Round((progress.Average ?? 0) * 10000));
        return new(text, string.Join("\n", lines), bar);
    }

    private readonly record struct SpawnProgress(int Total, int Known, int? Completed, double? Average)
    {
        public bool AllComplete => Completed == Total;
        public int Remaining => Known - (Completed ?? Known);
    }

    private static SpawnProgress Summarize(TrainExpansionStatus[] rows)
    {
        var total = rows.Sum(row => row.Total);
        var known = rows.Sum(row => row.KnownProgressCount);
        if (rows.Any(row => row.CompletedProgressCount is null)) return new(total, known, null, null);
        var completed = rows.Sum(row => row.CompletedProgressCount!.Value);
        var remaining = known - completed;
        if (completed == total) return new(total, known, completed, 1);
        // Subtract each fully progressed mark's contribution from the existing
        // v1 inclusive sum, preserving compatibility with the first companion.
        var sum = rows.Sum(row => (row.Progress ?? 0) * row.KnownProgressCount - row.CompletedProgressCount!.Value);
        return new(total, known, completed, remaining > 0 ? Math.Clamp(sum / remaining, 0, 1) : null);
    }

    private static string Percent(SpawnProgress progress) => progress.AllComplete ? "100%"
        : progress.Average is { } average ? Math.Min(99.9, average * 100).ToString("0.#", CultureInfo.InvariantCulture) + "%"
        : "unknown";
}
