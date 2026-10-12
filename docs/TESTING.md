# Testing checklist (v0.1.0, early beta)

Automated (every `./build.sh`):

- [x] Papyrus compiles.
- [x] Plugin validation: ESL range, Skyrim.esm only, no overrides, every form link resolves, script properties match their declarations, alias tags in the follower quest log, SEQ contents, byte-exact round trip.
- [x] FOMOD schema.
- [x] Cross-mod check (`tools/modkit/compat-series.sh`) with all sixteen plugins of the series. Report: `tools/modkit/compat-report.md`.

In game (not done yet; please help). Names to look for: the power "Company Quartermaster", the NPC "Brynja Tally-Hand". Plugin FormIDs load as FE xxx 8xx: the quest is `CQM_QuartermasterQuest` (800) and the power is `CQM_PowerQuartermaster` (828).

1. **Start**
   - [ ] On a new game, you learn "Company Quartermaster" and see the intro note. She is not in the world.
2. **Hire**
   - [ ] Outside an inn: "The company has no hall. Hire Brynja Tally-Hand at an inn."
   - [ ] Inside an inn (Bannered Mare, Sleeping Giant, Bee and Barb): hire for 500 gold. Not enough gold: the notice shows 500.
   - [ ] She appears beside you and follows. She is not a follower: the follower count stays 0, and you can still hire Lydia or a roster sellsword.
   - [ ] `help "Tally-Hand" 4` finds her only after the hire.
3. **Trade and sell**
   - [ ] Trade supplies opens a barter menu. Stock includes bread, cheese, venison, potions, arrows, leather strips, iron ingots, charcoal, leather.
   - [ ] Walk away (more than about 20 m) and try trade or sell-all: "Brynja is too far away."
   - [ ] With tankards and a silver candlestick, sell-all shows a gold total and, only after you confirm, removes those items and pays. Gems, ingots and quest items stay.
4. **Followers**
   - [ ] With a teammate nearby, Follower services lists them by name (the alias text, not "Follower 1") and offers Repair gear / Open stash.
   - [ ] Repair takes 2 leather strips, 1 iron ingot and 50 gold. The follower's armor goes up. After 3 in-game days (`set timescale` and wait) the buff is gone.
   - [ ] With Ember Smithing installed, Ember repair is on the menu and spends 2 charcoal and 1 fire salts. Without it, that button is absent and `CQM_EmberPresent` is 0.
   - [ ] Open stash. Store something. Dismiss her, hire her again, open the same follower's stash: the item is still there.
   - [ ] Eight different living followers (or the same eight if you can fill the slots) and a ninth: "stash slots full." A dead owner's slot can be reused.
5. **Travel and camp**
   - [ ] Make camp here: she stops following and wanders a few steps. Fast travel: she stays.
   - [ ] Travel with the company: she follows. Fast travel: she is beside you on arrival. Same after a save and load.
6. **Dismiss**
   - [ ] Confirm dismiss: she is gone. Cancel: she stays. Stashes are not emptied.
7. **Sellsword Roster and Blackshield Company** (optional)
   - [ ] Hire a roster sellsword or the Blackshield sellsword while Brynja is hired. Both stay. She does not replace them.
   - [ ] That sellsword appears in Follower services.

When reporting a bug, include what you did and the Papyrus log lines containing `CQM_` (enable Papyrus logging in `Skyrim.ini`: `[Papyrus] bEnableLogging=1`, `bEnableTrace=1`).
