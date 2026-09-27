# Release preparation validation — 27 September 2026

Companion **0.2.0.0**, targeting Umbra **3.1.18.0**, Dalamud **API 15** and
the matching HHE **0.6.0.1** build. This update was approved for release on
27 September 2026.

Fresh checks for companion commit `aa801415a2c33367298ef9fb9cfb0cac33f23ec8`
completed on 27 September 2026:

- The Release build succeeded with **0 warnings and 0 errors**, using installed
  Umbra **3.1.18.0** and Dalamud **API 15** host assemblies.
- All **80 companion checks passed**, with **0 failures and 0 skips**. The first
  sandboxed invocation was unable to open the test runner's local communication
  socket; rerunning with the required permission completed successfully.
- The vendored MIT train-status contract matches the corresponding HHE source
  byte for byte. The built assembly reports version **0.2.0.0**.

These results concern the companion; the main HHE release is validated
separately. Headless companion checks cover:

- Per-widget expansion subsets, six-expansion labels, All and right-click
  cycling, a single enabled expansion and no enabled expansions.
- Current/fixed world requests, world mismatch rejection, changing selections,
  missing or reloading providers, stale/invalid data, logout and popout toggling.
- V1 compatibility for older HHE/companions, and explicit update hints when
  a selected world or expansion requires V2.
- Display of six-expansion snapshots, ARR roster totals, selected-world
  validation, retained killed/sniped counts and offline-world responses.
- Remaining-mark progress, weighted expansion averages, unknown timers and
  exact completion boundaries. Killed/sniped marks affect only the listed count.
- Missing or faulty providers, snapshot freshness, failure recovery and
  selection changes that clear data from the previous world.

Packaging uses `python3 tools/package_release.py`. Its checks verify ZIP
integrity, SHA-256 checksums, absence of local path markers and an allowlist
containing only the companion DLL, dependency metadata, documentation and
licences. Host/game assemblies, private tests and debug symbols are excluded.
The package and checksum are approved for release. Public-download and GitHub
discovery checks are performed after publication.

No companion UI automation or in-game companion validation was performed during
this preparation. The user's HHE map inspection does not validate this widget.
In-game validation remains:

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

Private tests are intentionally excluded from the public source tree and ZIP.
Approval does not change the in-game validation limits listed above.
