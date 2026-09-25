using Dalamud.Interface;
using Dalamud.Plugin.Ipc;
using Dalamud.Plugin.Ipc.Exceptions;
using Dalamud.Plugin.Services;
using HuntHelperEvolved.Ipc;
using Lumina.Excel.Sheets;
using System.Globalization;
using Umbra.Common;
using Umbra.Widgets;
using Una.Drawing;

namespace Umbra.HuntHelperEvolved;

[ToolbarWidget("HuntHelperEvolved.Train", "Hunt Helper Evolved", "Train counts and average A-rank spawn progress from Hunt Helper Evolved.", ["hunt", "train", "spawn", "hhe"])]
public sealed class TrainStatusWidget(WidgetInfo info, string? guid = null, Dictionary<string, object>? configValues = null)
    : StandardToolbarWidget(info, guid, configValues)
{
    protected override StandardWidgetFeatures Features => StandardWidgetFeatures.Text |
        StandardWidgetFeatures.Icon | StandardWidgetFeatures.ProgressBar;
    protected override int DefaultMaxTextWidth => 360;
    private ICallGateSubscriber<string>? snapshotGate;
    private ICallGateSubscriber<uint, string>? worldSnapshotGate;
    private ICallGateSubscriber<bool>? toggleGate;
    private TrainConnection? connection;
    private long refreshAt;
    private uint requestedWorld = uint.MaxValue;

    protected override IEnumerable<IWidgetConfigVariable> GetConfigVariables() =>
    [
        ..base.GetConfigVariables(),
        new SelectWidgetConfigVariable("TrainWorld", "World", "Follow your current world, or keep this widget on one world. Uses the train and timer data available to HHE.", "0", WorldOptions())
            { Category = "Train" },
        new BooleanWidgetConfigVariable("ShowWorldName", "Show world name", "Show the fixed world's name on the button. The tooltip always includes it.", true)
            { Category = "Train" },
        new SelectWidgetConfigVariable("TrainExpansion", "Displayed view", "All combines the expansions enabled below. Right-click cycles only through enabled expansions.", "all",
            new() { ["all"] = "All enabled expansions", ["DT"] = "Dawntrail", ["EW"] = "Endwalker", ["ShB"] = "Shadowbringers",
                ["SB"] = "Stormblood", ["HW"] = "Heavensward", ["ARR"] = "A Realm Reborn" }) { Category = "Train" },
        ExpansionOption("DT", "Dawntrail", true),
        ExpansionOption("EW", "Endwalker", true),
        ExpansionOption("ShB", "Shadowbringers", true),
        ExpansionOption("SB", "Stormblood", false),
        ExpansionOption("HW", "Heavensward", false),
        ExpansionOption("ARR", "A Realm Reborn", false),
    ];

    private static BooleanWidgetConfigVariable ExpansionOption(string code, string name, bool enabled) =>
        new("Show" + code, name, "Include in All and right-click cycling for this widget.", enabled)
            { Category = "Train", Group = "Enabled expansions" };

    private static Dictionary<string, string> WorldOptions()
    {
        var options = new Dictionary<string, string> { ["0"] = "Current world" };
        foreach (var world in Framework.Service<IDataManager>().GetExcelSheet<World>()
                     .Where(w => w.IsPublic && !string.IsNullOrWhiteSpace(w.Name.ExtractText())).OrderBy(w => w.Name.ExtractText()))
            options[world.RowId.ToString(CultureInfo.InvariantCulture)] = $"{world.Name.ExtractText()} ({world.DataCenter.Value.Name.ExtractText()})";
        return options;
    }

    private uint SelectedWorld => uint.TryParse(GetConfigValue<string>("TrainWorld"), NumberStyles.None,
        CultureInfo.InvariantCulture, out var world) ? world : uint.MaxValue;

    private string[] EnabledExpansions() => TrainPresentation.AllExpansionCodes
        .Where(code => GetConfigValue<bool>("Show" + code)).ToArray();

    protected override void OnLoad()
    {
        snapshotGate = Framework.DalamudPlugin.GetIpcSubscriber<string>(TrainStatusContract.SnapshotGate);
        worldSnapshotGate = Framework.DalamudPlugin.GetIpcSubscriber<uint, string>(TrainStatusContract.WorldSnapshotGate);
        toggleGate = Framework.DalamudPlugin.GetIpcSubscriber<bool>(TrainStatusContract.ToggleGate);
        connection = new(() => snapshotGate.InvokeFunc(), () => toggleGate.InvokeFunc(), readWorld: ReadWorld);
        Node.OnClick += ToggleTrain;
        Node.OnRightClick += CycleExpansion;
        SetFontAwesomeIcon(FontAwesomeIcon.Train);
        SetText("Hunt Helper Evolved");
        SetProgressBarConstraint(0, 10000);
        SetProgressBarValue(0);
        refreshAt = 0;
    }

    protected override void OnDraw()
    {
        var world = SelectedWorld;
        var enabled = EnabledExpansions();
        var mode = TrainPresentation.NormalizeMode(GetConfigValue<string>("TrainExpansion"), enabled);
        if (mode != GetConfigValue<string>("TrainExpansion")) SetConfigValue("TrainExpansion", mode);
        if (world != requestedWorld || Environment.TickCount64 >= refreshAt)
        {
            requestedWorld = world;
            refreshAt = Environment.TickCount64 + 500;
            connection?.Refresh(world);
        }
        var display = TrainPresentation.Create(connection?.Snapshot, mode,
            connection?.UnavailableReason ?? "Waiting for Hunt Helper Evolved.", enabled,
            showWorld: world != 0, showWorldName: GetConfigValue<bool>("ShowWorldName"));
        SetText(display.Text);
        SetTooltip(display.Tooltip);
        SetProgressBarValue(display.Progress);
    }

    private string ReadWorld(uint world)
    {
        if (worldSnapshotGate is not { HasFunction: true }) throw new WorldTrainStatusUnavailableException();
        try { return worldSnapshotGate.InvokeFunc(world); }
        catch (IpcNotReadyError) { throw new WorldTrainStatusUnavailableException(); }
    }

    private void ToggleTrain(Node _) => connection?.Toggle(SelectedWorld);

    private void CycleExpansion(Node _)
    {
        SetConfigValue("TrainExpansion", TrainPresentation.NextMode(GetConfigValue<string>("TrainExpansion"), EnabledExpansions()));
    }

    protected override void OnUnload()
    {
        Node.OnClick -= ToggleTrain;
        Node.OnRightClick -= CycleExpansion;
        connection = null;
        snapshotGate = null;
        worldSnapshotGate = null;
        toggleGate = null;
    }
}
