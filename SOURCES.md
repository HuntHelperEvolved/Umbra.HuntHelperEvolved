# API references

Target: installed Umbra **3.1.18.0**, Dalamud **API 15**, .NET 10.

- [Umbra custom-plugin sample](https://github.com/una-xiv/Umbra.SamplePlugin)
  describes extension discovery and the widget attribute/constructor.
- [StandardToolbarWidget](https://github.com/una-xiv/umbra/blob/cbd00524952aa1c0386a47126f05721191f80033/Umbra/src/Toolbar/Widgets/System/Types/StandardToolbarWidget.cs)
  supplies the native text, icon, tooltip and themed progress bar.
- [ToolbarWidget](https://github.com/una-xiv/umbra/blob/cbd00524952aa1c0386a47126f05721191f80033/Umbra/src/Toolbar/Widgets/System/Types/ToolbarWidget.cs)
  provides configuration persistence and load/unload hooks.

Normal left/right clicks use Una.Drawing's `Node.OnClick` and
`Node.OnRightClick`. Host references are never packaged with this extension.
The project follows the existing Umbra.RelicAtlas companion's build layout.

HHE 0.6 publishes `HuntHelperEvolved.GetTrainStatusV1` as JSON
and `HuntHelperEvolved.ToggleTrainPopout` as a boolean function. The independent
versioned contract is vendored unchanged in `Contract/TrainStatusContract.cs`;
no HHE assembly is loaded by the companion.

Companion 0.1.0.1 uses the optional `completedProgressCount` field to exclude
100% marks from the inclusive v1 mean. HHE retains the original mean/count for
older consumers. If the field is missing, the companion retains counts and the
popout toggle, leaves progress unknown and asks for an HHE update.

Repository installation follows Umbra's
[PluginFetcher](https://github.com/una-xiv/umbra/blob/cbd00524952aa1c0386a47126f05721191f80033/Umbra/src/Plugins/Repository/PluginFetcher.cs)
and [repository dialog](https://github.com/una-xiv/umbra/blob/cbd00524952aa1c0386a47126f05721191f80033/Umbra/src/Windows/Library/Settings/Components/RepositoryInstallerNode.cs):
use owner `HuntHelperEvolved` and repository `Umbra.HuntHelperEvolved`. The fetcher
reads GitHub's latest normal release and downloads DLL/ZIP assets. This project
publishes one companion ZIP and a non-installable checksum sidecar.
