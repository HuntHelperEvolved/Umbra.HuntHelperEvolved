# Hunt Helper Evolved for Umbra

An Umbra toolbar widget showing a hunt train for your current world or a fixed world:

`DT:0/12, EW:0/12, ShB:0/12`

> **0.2.0.0:** Choose a current or fixed world and any combination of expansions
> for each widget. All new settings require **HHE 0.6.0.1** or newer.

- Left-click toggles HHE's train popout: open on the first click, close on the next.
- Right-click cycles **All → each enabled expansion → All**. Each widget remembers
  its own selection. With one expansion enabled, the view stays on that expansion.
- The first number counts A-rank marks in the train list, excluding killed or
  sniped marks even when retained. Custom flags and marks from other worlds are
  excluded.
- The second number is the full expansion roster, adjusted for known zone
  instances. HW through DT each have twelve marks with one instance per zone,
  or thirty-six with three. ARR has seventeen before instance adjustments.
- The progress bar averages the displayed expansions' known A-rank spawn-window
  progress **below 100%**. Marks already at 100% are excluded until every mark in
  the displayed roster is at 100%, when the bar fills completely. For example,
  one mark at 100% and one at 40% gives a 40% bar. Unknown timers are excluded
  from the average and prevent full completion; hover shows their count.
  All mode weights each remaining known mark equally. Individual progress uses
  the same calculation as HHE's A-rank timers: elapsed respawn-window time,
  not spawn probability.
- Missing HHE, logout and stale snapshots clear the previous display. Shared
  sync disconnection leaves HHE's retained local data available and is noted on
  hover. A world reported offline has unknown spawn progress.

## Configure each widget

Open a widget's settings and select **Train**:

- **World:** Follow **Current world**, or choose a fixed world from the list.
  Fixed widgets show the world name by default and keep showing that
  world's available HHE data when you travel. Selecting a world does not travel
  there or obtain data HHE has not received; unknown timers stay unknown.
- **Show world name:** Turn this off to hide the fixed world's name from the
  button and save space. The world remains visible in the tooltip. Enabled by
  default, and saved separately for each widget.
- **Enabled expansions:** Choose any combination of **A Realm Reborn (ARR)**,
  **Heavensward (HW)**, **Stormblood (SB)**, **Shadowbringers (ShB)**,
  **Endwalker (EW)** and **Dawntrail (DT)**. Existing defaults stay DT/EW/ShB;
  enable the older expansions here to include them. ARR has 17 possible marks
  before instance adjustments; the other expansions have 12 each.
- **Displayed view:** Choose **All enabled expansions** or an enabled expansion.
  All combines only that widget's selections, including its progress bar. A
  disabled view returns to All; disabling every expansion shows a settings hint.

Add multiple copies for different worlds or expansion groups. Their settings
and right-click selection are saved separately. Left-click opens the same HHE
train popout and does not change the character's world.

## Install directly from GitHub

Requires **Umbra 3.1.18.0**, **Dalamud API 15** and **Hunt Helper Evolved 0.6.0.1
or newer** for all settings. With HHE 0.6.0.0, current-world DT/EW/ShB views still
work; fixed worlds and older expansions show an update hint. For published builds,
enable **Get plugin testing builds** and follow the
[main HHE installation instructions](https://github.com/HuntHelperEvolved/HuntHelperEvolved#install).
Existing HHE users can update normally in `/xlplugins`; no repository change is needed.

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

The default view shows DT/EW/ShB. Umbra's widget settings also let you customize
the icon, text and progress bar.

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
for the 27 September 2026 build, headless-test and package checks. Companion
rendering, settings persistence and clicks still require in-game validation.
