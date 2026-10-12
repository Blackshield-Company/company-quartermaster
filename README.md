# Company Quartermaster

Stores for a company that has no hall. Company Quartermaster hires Brynja Tally-Hand at an inn. She sells supplies, buys clutter in bulk, refits your followers' gear, and keeps a stash for each of them. She is not a follower.

**Status: v0.1.0, early beta.** It passes every automated check in the build (Papyrus compile, plugin validation, byte-exact round trip, SEQ check, FOMOD schema, cross-mod compatibility check) but has not been playtested in game yet. Expect rough edges and please report what you find. See [docs/TESTING.md](docs/TESTING.md).

- Game: Skyrim Special Edition / Anniversary Edition 1.6.x (1.5.97 also works). Runs on Windows, and on Linux through Proton.
- Requirements: just the base game. No SKSE, SkyUI, DLC or Creation Club content.
- Plugin: `CompanyQuartermaster.esp`, ESL-flagged, with Skyrim.esm as its only master. **It edits no vanilla records.** The only cell it adds is its own holding cell for chests. Brynja does not exist until you hire her.
- Works with the rest of the series ([Blackshield Company](https://github.com/Blackshield-Company/blackshield-company), [Bounties of Oblivion](https://github.com/Blackshield-Company/bounties-of-oblivion), [Contract Hunter](https://github.com/Blackshield-Company/contract-hunter), [Steel Oath](https://github.com/Blackshield-Company/steel-oath), [Ember Smithing](https://github.com/Blackshield-Company/ember-smithing), [The Gray-Mane Debt](https://github.com/Blackshield-Company/the-gray-mane-debt), [Blackshield Codex](https://github.com/Blackshield-Company/blackshield-codex), [War Banners](https://github.com/Blackshield-Company/war-banners), [Roadwarden](https://github.com/Blackshield-Company/roadwarden), [Sellsword Roster](https://github.com/Blackshield-Company/sellsword-roster), [The Turncoat Contract](https://github.com/Blackshield-Company/the-turncoat-contract), [The Ledger of Ash](https://github.com/Blackshield-Company/the-ledger-of-ash), [Oathbreaker's Price](https://github.com/Blackshield-Company/oathbreakers-price), [Champion's Reforge](https://github.com/Blackshield-Company/champions-reforge), [Barrow-Iron Pact](https://github.com/Blackshield-Company/barrow-iron-pact)). It has its own prefix (`CQM_`), records, globals and files, and it is checked by loading all sixteen plugins together (see [tools/modkit](tools/modkit/README.md)).

## Install

Use the FOMOD. It installs `CompanyQuartermaster.esp`, the scripts, and `SEQ/CompanyQuartermaster.seq`. Papyrus sources are optional. Enable the plugin. It is ESL-flagged, so it does not take a full load-order slot. Load order does not matter: it has no master but Skyrim.esm and it overrides nothing.

## How it works

1. At the start you learn the lesser power **Company Quartermaster**.
2. **Not hired:** the power works only inside an inn (`LocTypeInn` on your current location). The Blackshield Company has no physical base in [Blackshield Company](https://github.com/Blackshield-Company/blackshield-company) (that mod is a ledger and a letter, not a hall), so hiring is at inns only. Hire her for `CQM_HireCost` gold (500). She appears beside you and starts travelling with you. She is placed with `PlaceActorAtMe`. She is not in `PotentialFollowerFaction` or `CurrentFollowerFaction`, she is not a teammate, and she does not take a follower slot.
3. **Hired:** Trade supplies / Sell all junk / Follower services / Travel with the company or Make camp here / Dismiss / Cancel.
4. **Trade supplies** opens her barter menu. She must be within `CQM_ServiceRadius` (1500). Otherwise: "Brynja is too far away."
5. **Sell all junk** pays a flat bulk rate for clutter on her lists. Confirm the gold total before anything is removed.
6. **Follower services** lists up to five nearest living teammates in the loaded area. Repair gear, or open that follower's stash.
7. **Travel / camp** toggles. Travel: she follows (`DefaultFollowPlayer`). Camp: she sandboxes where you left her (`DefaultSandboxCurrentLocation256`). In travel mode, a location change or a load moves her next to you if she is in another cell or farther than 3000 units. That covers fast travel. Camp mode leaves her where she is.
8. **Dismiss** asks first, then disables and deletes her. Stashes keep their items. Hire her again at an inn and the same stashes are still there.

She is protected (only you can kill her). If she dies, the next use of the power clears her so you can hire her again at an inn.

### Supplies

Her merchant chest, in this mod's own interior cell, stocks bread, cheese, venison, a cure disease potion, minor and standard health and stamina potions, iron and steel arrows, leather strips, iron ingots, charcoal and leather. The chest respawns. She buys only that list, not general goods. Skyrim.esm has no ingestible named salted meat, so the meat on the counter is venison.

### Sell all junk

No SKSE, so she does not walk your inventory. She buys only the misc clutter on five FormLists, grouped by a bulk tier. Pay is `count * tier * CQM_SellRate` (default 0.6), shown as a whole number of gold before she takes anything. Quest items, gems, ingots and anything not on those lists are never touched.

| Tier | Gold each | What |
|---|---|---|
| 1 | 1 | Tankards, wooden plates and bowls, ladles, baskets, buckets, brooms |
| 3 | 3 | Kettles, spigots, forks, knives, quills, inkwells, flagons, plates |
| 6 | 6 | Glazed cups, goblets, plates and bowls, lanterns, cast-iron pots |
| 12 | 12 | Silver goblets, plates, bowls, platters and jugs, a dwarven bowl |
| 25 | 25 | Silver candlesticks, dwarven tableware |

There is no takeable gold candlestick in the base game (the golden urn is a Thieves Guild quest item and is not on the list). Silver candlesticks are the high tier.

### Follower services

Repair is a field refit. Vanilla has no durability. For 2 leather strips, 1 iron ingot and `CQM_RepairGold` (50), the follower gets +40 armor and +10% weapon damage for `CQM_RepairDays` (3) in-game days. One game-time timer tracks the earliest expiry.

If [Ember Smithing](https://github.com/Blackshield-Company/ember-smithing) is installed, the repair menu also offers **Ember repair**: 2 charcoal and 1 fire salts (both vanilla), for +60 armor and +15% weapon damage. The option is hidden when that mod is absent. The check is a soft `GetFormFromFile` on `EMB_EmberForgedIronSword`. This mod does not master Ember Smithing.

Each follower gets a stash the first time you open one, up to 8. A slot whose actor is dead or gone is reused; the chest is not emptied. When all 8 are held by living actors, she reports that the stash slots are full. Open with the chest itself (`Activate`).

## Settings (console)

```
set CQM_HireCost to 500           ; gold to hire her
set CQM_ServiceRadius to 1500     ; units; trade and sell-all need her this close
set CQM_SellRate to 0.6           ; fraction of the tier total paid for clutter
set CQM_RepairDays to 3           ; in-game days a refit lasts
set CQM_RepairGold to 50          ; gold for a normal refit (ember repair takes no gold)
set CQM_CatchupDistance to 3000   ; travel mode: move her to you past this distance
```

State, also console-adjustable, but the script overwrites these: `CQM_PowerGranted`, `CQM_Hired`, `CQM_Traveling`, `CQM_EmberPresent` (1 when Ember Smithing is loaded).

## Compatibility

No shared records with the other fifteen mods. She does not use the vanilla follower quest, so she does not fight [Sellsword Roster](https://github.com/Blackshield-Company/sellsword-roster) or the [Blackshield Company](https://github.com/Blackshield-Company/blackshield-company) sellsword for the one follower slot. Those sellswords are teammates, so they show up in her follower list and can take a refit or a stash. Ember Smithing is optional and is not a master. War Banners has its own nearest-teammate quest; this one is separate and only runs while the menu is open.

## Known issues (early beta)

- Not playtested in game.
- No new dialogue. Her looks and voice come from a vanilla Nord bandit template. Hiring and every menu go through the power, not through talking to her.
- She does not fight. She is not a teammate.
- Catch-up runs on a location change and on load, not while you walk. The follow package keeps her close; fast travel is what the catch-up is for.
- Sell-all only covers the lists above. Other clutter stays in your pack.
- A reused stash still holds whatever the last owner stored.
- Camp is a short sandbox, not a tent. The holding cell is not a place you visit.
- Trade prices follow your speechcraft. Sell-all does not.

## Building

`./build.sh` on Linux builds the release zip from this repo. Papyrus is compiled with the open-source [papyrus-compiler](https://github.com/russo-2025/papyrus-compiler), the plugin is generated from C# with [Mutagen](https://github.com/Mutagen-Modding/Mutagen), and everything is validated. You need .NET SDK 9+, python3, curl and, optionally, xmllint. See [tools/modkit/README.md](tools/modkit/README.md).

The sixteen-mod compatibility report is in [tools/modkit/compat-report.md](tools/modkit/compat-report.md).

## Layout

```
src/plugin/CompanyQuartermaster.Generator/   C# generator (NPC, cell, chests, vendor, power, messages, quests, settings)
src/plugin/formids.json                      pinned FormIDs
src/papyrus/                                 controller, power and player alias scripts
fomod/                                       FOMOD installer config
tools/modkit/                                shared build tooling (Mutagen helpers, validator, Papyrus, FOMOD, compat check)
docs/                                        design notes, testing checklist, Nexus page
```

## Credits

- Built with [Mutagen](https://github.com/Mutagen-Modding/Mutagen) and [russo-2025/papyrus-compiler](https://github.com/russo-2025/papyrus-compiler).
- Brynja's looks and voice are a vanilla Skyrim.esm template. Chest and marker meshes are vanilla. The mod ships no Bethesda assets.

## Support

Donations are optional. Nothing in the mod is gated on them.

- GitHub Sponsors: https://github.com/sponsors/synthalorian
- Ko-fi: https://ko-fi.com/synthalorian

## License

MIT (code and text in this repository). See [LICENSE](LICENSE).
