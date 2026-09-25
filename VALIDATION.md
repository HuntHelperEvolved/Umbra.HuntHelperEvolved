# Release validation

Companion release: **0.1.0.2**, targeting Umbra **3.1.18.0**, Dalamud **API 15**,
and HHE **0.6.0.0** with remaining-mark train-status data.

The release build uses the installed host assemblies and excludes them from
the output package. Headless companion tests cover labels, expansion selection,
remaining-mark spawn progress, stale/invalid snapshots, missing providers,
logout, repeated toggle requests and recovery. Progress checks cover completed
marks within one expansion and across expansions, weighted remaining means,
unknown roster entries, old providers missing completion counts, and values
immediately below 100%. Only a fully completed selected roster can fill the bar;
integer and tooltip rounding cannot report completion early.

All **52 companion checks** and **52 focused HHE train-status checks** pass for
this release. The latter includes provider, contract and IPC lifecycle cases.

The latest HHE 0.6 preview excludes killed/sniped marks from the listed count,
including retained rows. Roster totals and spawn-progress coverage are unchanged.
Provider checks cover all three expansions, restored marks, inconsistent sniped
flags, fully killed trains and retained dead rows supplying instance/timer data.
Companion 0.1.0.2 updates the tooltip and documentation; the filtering comes from
HHE and also works with companion 0.1.0.1.

HHE supplies an optional completion count while retaining the original v1 mean
and known-count semantics. Provider checks cover live marks, expired windows,
the exact 100% boundary, instances, offline/maintenance state and compatibility
with the original JSON contract.

Publication checks verify:

- A source allowlist excluding private tests, build output, local configuration
  and credentials, with public GitHub noreply commit identity.
- Agreement between the vendored MIT IPC contract and the matching HHE source.
- A package containing one companion DLL, its dependency metadata, documentation
  and licences, without debug symbols or host/game DLLs.
- ZIP integrity, safe extraction paths and a SHA-256 checksum.
- A non-draft, non-prerelease GitHub release discoverable through
  `/releases/latest`, with one installable ZIP asset and a matching public download.

No live FFXIV client is available for validation. Actual installation inside
Umbra, themed rendering and mouse interaction remain in-game checks. HHE 0.6 is
currently a local preview; the public HHE 0.5.0.22 lacks the necessary IPC.
