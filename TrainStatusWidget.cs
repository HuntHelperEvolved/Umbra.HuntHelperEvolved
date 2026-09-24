using Dalamud.Interface;
using Dalamud.Plugin.Ipc;
using HuntHelperEvolved.Ipc;
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
    private ICallGateSubscriber<bool>? toggleGate;
    private TrainConnection? connection;
    private long refreshAt;

    protected override IEnumerable<IWidgetConfigVariable> GetConfigVariables() =>
    [
        ..base.GetConfigVariables(),
        new SelectWidgetConfigVariable("TrainExpansion", "Expansions", "Right-click cycles through All, Dawntrail, Endwalker and Shadowbringers.", "all",
            new() { ["all"] = "All", ["DT"] = "Dawntrail", ["EW"] = "Endwalker", ["ShB"] = "Shadowbringers" }),
    ];

    protected override void OnLoad()
    {
        snapshotGate = Framework.DalamudPlugin.GetIpcSubscriber<string>(TrainStatusContract.SnapshotGate);
        toggleGate = Framework.DalamudPlugin.GetIpcSubscriber<bool>(TrainStatusContract.ToggleGate);
        connection = new(() => snapshotGate.InvokeFunc(), () => toggleGate.InvokeFunc());
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
        if (Environment.TickCount64 >= refreshAt)
        {
            refreshAt = Environment.TickCount64 + 500;
            connection?.Refresh();
        }
        var display = TrainPresentation.Create(connection?.Snapshot, GetConfigValue<string>("TrainExpansion"),
            connection?.UnavailableReason ?? "Waiting for Hunt Helper Evolved.");
        SetText(display.Text);
        SetTooltip(display.Tooltip);
        SetProgressBarValue(display.Progress);
    }

    private void ToggleTrain(Node _) => connection?.Toggle();

    private void CycleExpansion(Node _)
    {
        SetConfigValue("TrainExpansion", TrainPresentation.NextMode(GetConfigValue<string>("TrainExpansion")));
    }

    protected override void OnUnload()
    {
        Node.OnClick -= ToggleTrain;
        Node.OnRightClick -= CycleExpansion;
        connection = null;
        snapshotGate = null;
        toggleGate = null;
    }
}
