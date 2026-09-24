# Release validation

First public companion release: **0.1.0.0**, targeting Umbra **3.1.18.0**,
Dalamud **API 15**, and HHE **0.6.0.0** with train-status IPC.

The release build uses the installed host assemblies and excludes them from
the output package. Headless companion tests cover labels, expansion selection,
weighted known spawn progress, stale/invalid snapshots, missing providers,
logout, repeated toggle requests and recovery. All **40 companion checks** and
**12 focused HHE IPC checks** passed for the toggle integration. The preceding
full HHE suite passed 535 cases. Publication adds repository metadata and
installation documentation; widget behavior is unchanged.

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
