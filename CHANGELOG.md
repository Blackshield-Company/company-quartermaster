# Changelog

All notable changes to this project are documented here. Versions follow [Semantic Versioning](https://semver.org/).

## [0.1.0] - 2026-10-12

Early beta. First public release. It passes every automated check in the build (Papyrus compile, plugin validation, round trip, SEQ check, FOMOD schema, cross-mod compatibility check) but has not been playtested in game yet. Please report problems on the Nexus page or on GitHub.

### Added
- Brynja Tally-Hand, a named unique quartermaster. She does not exist until hired, is not a follower, and does not take a follower slot.
- The lesser power "Company Quartermaster", granted at the start by a Start Game Enabled quest. Hire her at an inn (the Blackshield Company has no hall). Once hired: trade supplies, sell listed clutter in bulk, follower refits and stashes, travel or camp, dismiss.
- A vendor faction and a merchant chest in a new interior cell owned by this mod. No vanilla cell is edited.
- Bulk clutter sale from five FormLists of vanilla junk, paid at a flat rate. Quest items, gems and ingots are not on those lists.
- Field refit (+40 armor, +10% weapon damage) for a follower, and a stronger ember refit when Ember Smithing is installed (soft check, no master).
- Eight per-follower stashes that keep their contents after she is dismissed.
- Travel and camp packages. In travel mode she is moved next to the player on a location change or a load if she is in another cell or farther than 3000 units.
- Settings as console globals.
- `SEQ/CompanyQuartermaster.seq` for the Start Game Enabled controller quest.
- FOMOD installer with optional Papyrus sources.
