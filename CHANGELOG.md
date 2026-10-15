# Changelog

All notable changes to this project are documented here. Versions follow [Semantic Versioning](https://semver.org/).

## [0.2.2] - 2026-10-15

### Fixed
- Brynja's arms, midriff and legs rendered black or dark brown while her face looked normal. Since 0.2.1 she is no longer templated on a bandit, and her NPC record had no body skin tone (QNAM, "Texture Lighting"), so the engine tinted her body with black. She now has a pale Nord skin tone that matches her face. No FormIDs changed. Saves from 0.2.1 and earlier keep working, and the fix applies the next time her actor loads.

## [0.2.1] - 2026-10-13

### Fixed
- Brynja looked like a different female bandit each hire because her NPC used the leveled bandit list LCharBanditMeleeNordF as a template with the Traits flag; she is now a unique non-templated Nord woman with fixed vanilla head parts, hair, iron-and-hide outfit and iron sword.

Saves from 0.2.0 and 0.1.0 keep working; no FormIDs changed. A Brynja already hired in an old save keeps her old look until dismissed and hired again.

## [0.2.0] - 2026-10-12

### Added
- Company HQ. Set it at an inn or a house you own, then send Brynja there to wait. You can hire her there again. On a load or a location change she returns to that cell if she has left it.

### Changed
- Dismiss, when a company HQ is set, asks whether to send her to HQ or release her from service.

Saves from 0.1.0 keep working. Existing FormIDs are unchanged. This version only adds new records.

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
