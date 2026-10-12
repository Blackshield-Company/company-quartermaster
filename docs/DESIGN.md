# Design notes (v0.1.0, early beta)

## Records (64, FormIDs 0x800-0x83F, ESL-flagged, Skyrim.esm only, no overrides)

| EditorID | Type | Purpose |
|---|---|---|
| `CQM_QuartermasterQuest` | QUST (Start Game Enabled, Run Once) | Controller (`CQM_QuartermasterScript`). Alias 0 = player (`CQM_PlayerAliasScript`: grant on init, catch-up on `OnLocationChange` and `OnPlayerLoadGame`). Alias 1 = `CQM_TravelAlias` (optional, protected, package `DefaultFollowPlayer`). Alias 2 = `CQM_CampAlias` (optional, protected, package `DefaultSandboxCurrentLocation256`). In `SEQ/CompanyQuartermaster.seq`. |
| `CQM_FollowerQuest` | QUST | Five optional reference aliases, nearest living player teammates in the loaded area (same shape as `WBN_RallyQuest`). Restarted when the follower menu opens, stopped when it closes. Stage 10 log carries the `<Alias=Follower1>`…`<Alias=Follower5>` tags the menus use. Not start-game-enabled. |
| `CQM_Brynja` | NPC_ | Template `LCharBanditMeleeNordF` (traits, stats, model, inventory, attack data, keywords). Own name, vendor faction only. Unique, protected. Not in `PotentialFollowerFaction` or `CurrentFollowerFaction`. No packages of her own. Does not exist until `PlaceActorAtMe`. |
| `CQM_VendorFaction` | FACT | Vendor flag, hours 0-24, radius 50000, buy/sell list `CQM_BuyList` only, merchant container `CQM_MerchantChestRef`. |
| `CQM_StorageCell` | CELL | New interior. Not a vanilla cell. Holds the merchant chest, eight stash chests and a camp marker, all persistent. |
| `CQM_MerchantChest` / `CQM_MerchantChestRef` | CONT / REFR | Respawning chest. Stock: bread, cheese, venison, cure disease, minor and standard health and stamina potions, iron and steel arrows, leather strips, iron ingots, charcoal, leather. |
| `CQM_StashChest` / `CQM_StashRef01`-`08` | CONT / REFR | Empty, no respawn. Assigned in script to a follower on first use. |
| `CQM_CampMarkerRef` | REFR | `XMarker`. Moved to her when she camps. The camp package is current-location sandbox, because a `PlaceActorAtMe` actor has no useful editor location. |
| `CQM_BuyList`, `CQM_JunkTier1/3/6/12/25` | FLST | Buy list is the supply forms. Junk lists are vanilla misc clutter only. |
| `CQM_PowerQuartermaster`, `CQM_PowerEffect` | SPEL (lesser power) + MGEF (script) | "Company Quartermaster". |
| `CQM_AbRepaired`, `CQM_AbEmberRepaired` | SPEL (ability) | +40 armor / +10% damage, and +60 / +15%. Constant, removed by script. |
| `CQM_Eff*` | MGEF | Value modifiers for those abilities. |
| `CQM_Msg*` | MESG | Hire, hired menus, sell confirm, follower list (`<Alias=…>`), per-follower service, repair (ember button conditioned on `CQM_EmberPresent`), dismiss. |
| `CQM_*` | GLOB (10) | Settings and a few state flags. |

Vanilla records used (none edited): `LocTypeInn` (01CB87), `DefaultFollowPlayer` (04B037), `DefaultSandboxCurrentLocation256` (0956B8), `Gold001`, `XMarker`, the Nord bandit template, and the food, potion, ammo and clutter forms in `Vanilla.cs`. FormIDs were looked up with `Mutagen.Bethesda.FormKeys.SkyrimSE`.

## Why inns

Blackshield Company has no placed hall. Its plugin is a ledger, a letter and a sellsword called through the vanilla follower system. Hiring Brynja at an inn is the whole "where" of this mod. Outside an inn, and while she is not hired, the power says so and does nothing else.

## Not a follower

She is never added to `PotentialFollowerFaction` or `CurrentFollowerFaction`, and the script calls `SetPlayerTeammate(false)`. Travel is an alias package (`DefaultFollowPlayer`), not `DialogueFollower.SetFollower`. Sellsword Roster and the Blackshield sellsword keep the one vanilla follower slot. Either of them can still be a teammate and show up in her follower list.

## No SKSE, no polling

`GetItemCount(FormList)` and `RemoveItem(FormList, count)` are vanilla. Junk is only what the lists name. The power opens menus. Catch-up is `OnLocationChange` and `OnPlayerLoadGame`. Refit expiry is one `RegisterForSingleUpdateGameTime` on the earliest slot. There is no `RegisterForUpdate` loop.

## Ember Smithing

`Form emb = Game.GetFormFromFile(0x803, "EmberSmithing.esp") ; soft: EMB_EmberForgedIronSword`, guarded with `If emb`. That sets `CQM_EmberPresent`. The ember button on `CQM_MsgRepair` has `GetGlobalValue == 1`, so it is hidden when the plugin is absent. Ember repair spends vanilla charcoal and fire salts, not records from that mod. No master.

## Stashes and dismiss

Eight persistent chests in `CQM_StorageCell`. An `Actor[]` on the controller (the quest never stops) remembers who owns which chest. Dead or `None` slots are reused; the chest is not cleared. Dismiss clears both aliases, then `Disable` and `Delete`. The chests stay. Rehiring does not spawn a new set.
