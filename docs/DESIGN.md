# Design notes (v0.2.2, early beta)

## Records (72, FormIDs 0x800-0x847, ESL-flagged, Skyrim.esm only, no overrides)

0.1.0 used 0x800-0x83F. 0.2.0 only adds records from 0x840. No existing EditorID or FormID changed. No alias was added to `CQM_QuartermasterQuest` (aliases added to a quest that is already running in old saves do not fill reliably).

| EditorID | Type | Purpose |
|---|---|---|
| `CQM_QuartermasterQuest` | QUST (Start Game Enabled, Run Once) | Controller (`CQM_QuartermasterScript`). Alias 0 = player (`CQM_PlayerAliasScript`: grant on init, catch-up on `OnLocationChange` and `OnPlayerLoadGame`). Alias 1 = `CQM_TravelAlias` (optional, protected, package `DefaultFollowPlayer`). Alias 2 = `CQM_CampAlias` (optional, protected, package `DefaultSandboxCurrentLocation256`). In `SEQ/CompanyQuartermaster.seq`. |
| `CQM_FollowerQuest` | QUST | Five optional reference aliases, nearest living player teammates in the loaded area (same shape as `WBN_RallyQuest`). Restarted when the follower menu opens, stopped when it closes. Stage 10 log carries the `<Alias=Follower1>`…`<Alias=Follower5>` tags the menus use. Not start-game-enabled. |
| `CQM_HQQuest` | QUST | Not start-game-enabled, not run-once, priority 30, QuestFormVersion 65. Alias 0 = `CQM_HQAlias` (optional, protected, package `DefaultSandboxCurrentLocation1024`). The controller starts it with `Start()` when the alias is needed. Not in the SEQ file. |
| `CQM_Brynja` | NPC_ | Template `LCharBanditMeleeNordF` (traits, stats, model, inventory, attack data, keywords). Own name, vendor faction only. Unique, protected. Not in `PotentialFollowerFaction` or `CurrentFollowerFaction`. No packages of her own. Does not exist until `PlaceActorAtMe`. |
| `CQM_VendorFaction` | FACT | Vendor flag, hours 0-24, radius 50000, buy/sell list `CQM_BuyList` only, merchant container `CQM_MerchantChestRef`. |
| `CQM_StorageCell` | CELL | New interior. Not a vanilla cell. Holds the merchant chest, eight stash chests, a camp marker and an HQ marker, all persistent. |
| `CQM_MerchantChest` / `CQM_MerchantChestRef` | CONT / REFR | Respawning chest. Stock: bread, cheese, venison, cure disease, minor and standard health and stamina potions, iron and steel arrows, leather strips, iron ingots, charcoal, leather. |
| `CQM_StashChest` / `CQM_StashRef01`-`08` | CONT / REFR | Empty, no respawn. Assigned in script to a follower on first use. |
| `CQM_CampMarkerRef` | REFR | `XMarker`. Moved to her when she camps. The camp package is current-location sandbox, because a `PlaceActorAtMe` actor has no useful editor location. |
| `CQM_HQMarkerRef` | REFR | `XMarker`. Moved to the player when company HQ is set. She is moved to it in HQ mode, and again on load or a location change if she is not in its cell. |
| `CQM_BuyList`, `CQM_JunkTier1/3/6/12/25` | FLST | Buy list is the supply forms. Junk lists are vanilla misc clutter only. |
| `CQM_PowerQuartermaster`, `CQM_PowerEffect` | SPEL (lesser power) + MGEF (script) | "Company Quartermaster". |
| `CQM_AbRepaired`, `CQM_AbEmberRepaired` | SPEL (ability) | +40 armor / +10% damage, and +60 / +15%. Constant, removed by script. |
| `CQM_Eff*` | MGEF | Value modifiers for those abilities. |
| `CQM_Msg*` | MESG | Hire, hired menus (travel, camp, HQ), sell confirm, follower list (`<Alias=…>`), per-follower service, repair (ember button conditioned on `CQM_EmberPresent`), HQ (Send and Clear conditioned on `CQM_HQSet`), dismiss, dismiss-with-HQ. |
| `CQM_*` | GLOB (12) | Settings and a few state flags, including `CQM_HQSet` and `CQM_HQMode`. |

Vanilla records used (none edited): `LocTypeInn` (01CB87), `LocTypePlayerHouse` (0FC1A3), `DefaultFollowPlayer` (04B037), `DefaultSandboxCurrentLocation256` (0956B8), `DefaultSandboxCurrentLocation1024` (0BFB6B), `Gold001`, `XMarker`, the Nord bandit template, and the food, potion, ammo and clutter forms in `Vanilla.cs`. FormIDs were looked up with `Mutagen.Bethesda.FormKeys.SkyrimSE`.

## Why inns

Blackshield Company has no placed hall. Its plugin is a ledger, a letter and a sellsword called through the vanilla follower system. The first hire is at an inn. Company HQ is not a new hall: it is an inn or a player house (`LocTypePlayerHouse`) the player marks. After that, hire is allowed at that location as well as at any inn. Outside those places, and while she is not hired, the power says so and does nothing else.

## Company HQ

`CQM_HQQuest` holds the only new alias. The controller quest's aliases are unchanged (player 0, travel 1, camp 2).

Set HQ only if the player's current location has `LocTypeInn` or `LocTypePlayerHouse`. Otherwise `CQM_MsgHQNotHere`. The marker moves to the player, `CQM_HQSet` becomes 1, and the location is stored in a script variable on the controller (that quest never stops). If she is already in HQ mode, she moves to the new marker.

Send-to-HQ clears the travel and camp aliases, `ForceRefTo` on `CQM_HQAlias`, `MoveTo` the marker, `CQM_HQMode` 1, `CQM_Traveling` 0, `EvaluatePackage`. She stays hired. Services still use `NearBrynja` / `CQM_ServiceRadius`.

Clear HQ sets `CQM_HQSet` to 0. If she is in HQ mode she camps where she stands (camp marker moved to her, HQ alias cleared, camp alias forced). Travel with the company from HQ mode clears the HQ alias as well. Make camp here is unchanged.

Dismiss with `CQM_HQSet == 1` uses `CQM_MsgDismissHQ`: send her to HQ (same as above) or release her. Release is the old dismiss: clear travel, camp and HQ aliases, clear `CQM_HQMode`, disable and delete. `CQM_HQSet` is left alone, so the place can hire her again. With no HQ, `CQM_MsgDismiss` is unchanged.

`CQM_MsgHQ` conditions the Send and Clear buttons on `CQM_HQSet == 1`. Conditioned-out buttons keep their index in `Show()`, so the script's indexes stay fixed.

Catch-up for HQ mode runs from `GameLoaded` (already called on `OnPlayerLoadGame`) and from `CatchUp` (location change). If she is in HQ mode and not in the marker's parent cell, she is moved to the marker. No `RegisterForUpdate`.

## Not a follower

She is never added to `PotentialFollowerFaction` or `CurrentFollowerFaction`, and the script calls `SetPlayerTeammate(false)`. Travel is an alias package (`DefaultFollowPlayer`), not `DialogueFollower.SetFollower`. Sellsword Roster and the Blackshield sellsword keep the one vanilla follower slot. Either of them can still be a teammate and show up in her follower list.

## No SKSE, no polling

`GetItemCount(FormList)` and `RemoveItem(FormList, count)` are vanilla. Junk is only what the lists name. The power opens menus. Catch-up is `OnLocationChange` and `OnPlayerLoadGame` (travel distance, and the HQ cell). Refit expiry is one `RegisterForSingleUpdateGameTime` on the earliest slot. There is no `RegisterForUpdate` loop.

## Ember Smithing

`Form emb = Game.GetFormFromFile(0x803, "EmberSmithing.esp") ; soft: EMB_EmberForgedIronSword`, guarded with `If emb`. That sets `CQM_EmberPresent`. The ember button on `CQM_MsgRepair` has `GetGlobalValue == 1`, so it is hidden when the plugin is absent. Ember repair spends vanilla charcoal and fire salts, not records from that mod. No master.

## Stashes and dismiss

Eight persistent chests in `CQM_StorageCell`. An `Actor[]` on the controller (the quest never stops) remembers who owns which chest. Dead or `None` slots are reused; the chest is not cleared. Dismiss clears the travel, camp and HQ aliases, then `Disable` and `Delete`. The chests stay. Rehiring does not spawn a new set. `CQM_HQSet` and the remembered location also stay, so she can be hired again at that HQ.
