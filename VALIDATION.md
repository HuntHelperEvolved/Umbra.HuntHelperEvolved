# Staged validation

Companion **0.2.0.0**, targeting Umbra **3.1.18.0**, Dalamud **API 15** and
the matching HHE **0.6.0.1** staged build. This update is prepared locally;
the public companion remains 0.1.0.2 and public HHE remains 0.6.0.0.

All **80 companion checks** and **568 HHE checks** pass, including **69 focused
HHE train-status checks**. Both Release builds use the installed host assemblies.
Headless checks cover:

- Per-widget expansion subsets, six-expansion labels, All and right-click
  cycling, a single enabled expansion and no enabled expansions.
- Current/fixed world requests, world mismatch rejection, changing selections,
  missing or reloading providers, stale/invalid data, logout and popout toggling.
- V1 compatibility for older HHE/companions, and explicit update hints when
  a selected world or expansion requires V2.
- Six-expansion roster totals, ARR's 17 marks and differing spawn windows,
  world/instance isolation, retained killed/sniped rows and offline worlds.
- Remaining-mark progress, weighted expansion averages, unknown timers and
  exact completion boundaries. Killed/sniped marks affect only the listed count.
- Framework-only capture, bounded world requests, expiry, failure recovery and
  timestamps from real captures rather than IPC reads.

Package checks verify the matching MIT IPC contract, ZIP integrity, safe paths,
SHA-256 checksums and an allowlist containing only the companion DLL, dependency
metadata, documentation and licences. Host/game assemblies, private tests and
debug symbols are excluded.

No live FFXIV client was available. In-game validation remains:

1. Add two widget copies with different worlds and expansion selections; reload
   Umbra and confirm their settings remain separate.
2. Enable ARR/HW/SB and check labels, roster totals, tooltips and progress. Try
   All, right-click cycling, one enabled expansion and none.
3. Travel while one widget follows Current world and another stays fixed;
   confirm the fixed-world name and data remain selected. Unavailable timer data
   should stay unknown. Check logout, login and plugin reload recovery.
4. Confirm left-click opens and closes the shared HHE train popout and inspect
   settings layout, text truncation and themed rendering. Turn off Show world
   name on one fixed-world widget; only its button label should shorten, with
   the world still shown in the tooltip and counts/progress unchanged.

GitHub discovery and public-download checks apply when this update is published;
no new release, tag or installer-feed change is part of local staging.
