# Hunt Helper Evolved for Umbra

An Umbra toolbar widget showing the current world's hunt train:

`DT:0/12, EW:0/12, ShB:0/12`

- Left-click toggles HHE's train popout: open on the first click, close on the next.
- Right-click cycles **All → DT → EW → ShB → All**. Each widget remembers its selection.
- The first number counts A-rank marks retained in the train list, including dead
  or sniped marks. Custom flags and marks from other worlds are excluded.
- The second number is the full expansion roster, adjusted for known zone
  instances. Twelve is the total when all six zones have one instance; three
  instances in every zone give thirty-six.
- The progress bar averages the displayed expansions' known A-rank spawn-window
  progress, using the same calculation as HHE's A-rank timers. Confirmed living
  marks contribute 100%. Unknown timers are excluded; hover shows their count.
  All mode weights each known mark equally, rather than each expansion equally.
  This measures elapsed respawn-window time, not spawn probability.
- Missing HHE, logout and stale snapshots clear the previous display. Shared
  sync disconnection leaves HHE's retained local data available and is noted on
  hover. A world reported offline has unknown spawn progress.

## Install directly from GitHub

Requires **Umbra 3.1.18.0**, **Dalamud API 15** and **Hunt Helper Evolved 0.6.0.0
or newer** with train-status IPC.

> **HHE 0.6 requirement:** HHE 0.6 is currently a local preview. Its published
> installer still serves 0.5.0.22, which does not provide this integration. The
> companion can be installed now, but needs the matching HHE 0.6 preview or the
> eventual 0.6 release to display data.

1. Install and enable **Umbra** and a compatible **Hunt Helper Evolved** build in
   Dalamud.
2. Open **Umbra → Settings → Plugins**. Enable custom plugins if prompted.
3. Under **Install from repository**, enter:

   | Field | Value |
   | --- | --- |
   | Author / owner | `HuntHelperEvolved` |
   | Repository | `Umbra.HuntHelperEvolved` |

4. Add the repository and confirm the **Hunt Helper Evolved for Umbra** release.
5. Restart Umbra when prompted. In the toolbar configuration, choose **Add
   Widget** and select **Hunt Helper Evolved**.

Enter those two field values, rather than a URL or `repo.json`. Umbra downloads
and installs the companion directly from this repository's latest release.
Future companion releases can be discovered by Umbra's repository updater.

If you previously installed the companion using **Install from file**, remove
that companion entry from Umbra's Plugins list before adding this repository,
so only one copy registers the widget. Keep the main HHE plugin enabled.

The main HHE plugin uses its separate
[Dalamud installation instructions](https://github.com/HuntHelperEvolved/HuntHelperEvolved#install).

The default view shows all three expansions. Umbra's widget settings also let
you choose the expansion directly and customize the icon, text and progress bar.

## Build and package

With .NET 10 and the installed host assemblies:

```sh
dotnet build Umbra.HuntHelperEvolved.csproj -c Release -p:UmbraLibPath="PATH_TO_UMBRA" -p:DalamudLibPath="PATH_TO_DALAMUD"
python3 tools/package_release.py
```

Standard Linux and Windows launcher paths for the supported host version are
detected automatically. The ZIP and SHA-256 checksum are written to `artifacts/`.
Publish one companion ZIP and its checksum as a normal, non-prerelease GitHub
release. Umbra reads `/releases/latest` and considers DLL/ZIP assets, so do not
attach host assemblies or the main HHE package to this repository's release.
This is an Umbra extension, not a separate Dalamud plugin. It makes no network
requests and does not bundle HHE, Umbra or game assemblies.

The extension uses AGPL-3.0-or-later. The standalone JSON contract in `Contract/`
is copied from HHE under its MIT licence in `CONTRACT-LICENSE.txt`.
See [SOURCES.md](SOURCES.md) for API references and [VALIDATION.md](VALIDATION.md)
for release checks. In-game rendering and click validation remain outstanding.
