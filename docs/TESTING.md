# Testing checklist (v0.2.3, early beta)

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
6. **Dismiss** (no HQ set)
   - [ ] Confirm dismiss: she is gone. Cancel: she stays. Stashes are not emptied.
7. **Sellsword Roster and Blackshield Company** (optional)
   - [ ] Hire a roster sellsword or the Blackshield sellsword while Brynja is hired. Both stay. She does not replace them.
   - [ ] That sellsword appears in Follower services.
8. **Company HQ**
   - [ ] Hired, travelling or camped: the menu has Company HQ before Dismiss. Cancel still does nothing.
   - [ ] Set company HQ here, outdoors or in a shop that is not an inn or a house you own: "Company HQ can only be set at an inn or a house you own." Send her to HQ and Clear HQ are absent until one is set.
   - [ ] At an inn, or in a house you own (Breezehome): Set company HQ here. Note: "Company HQ set here."
   - [ ] Send her to HQ. She leaves you and waits there. She is still hired. Trade from across the city: "Brynja is too far away." Trade beside her still opens the barter menu.
   - [ ] While she is at HQ, the menu says she is at company HQ, and the travel button is Travel with the company (not Make camp here). That brings her back to you.
   - [ ] Make camp here, while she is travelling, still leaves her where she stands. It does not send her to HQ.
   - [ ] Clear HQ while she is at HQ: she camps where she stands. Clear HQ while she travels: she keeps travelling, and Send her to HQ is gone until you set HQ again.
   - [ ] Dismiss with HQ set: Send her to HQ (she waits, still hired), Release her from service (she is gone, stashes remain, HQ stays set), Cancel (nothing).
   - [ ] After release, hire again at that HQ (the inn or the house), not only at an inn. Without an HQ, hire is still inns only. Outside both: the not-an-inn notice.
   - [ ] Save while she is at HQ, leave the cell, load: she is back in the HQ cell. Fast travel away, then enter another location: if she has left the HQ cell, she returns. No constant script lag (no update loop).
9. **Upgrade a 0.1.0 save**
   - [ ] Load a save where she is already hired (travel or camp) from 0.1.0. She is still hired. Stashes still hold their items. Travel and camp still work. The hired menu has the new Company HQ button; the old buttons do the same things.
   - [ ] `help CQM_QuartermasterQuest 4` is still the 800 quest. She was not dropped because of a new alias on that quest.
   - [ ] Set an HQ, send her, save, load. She is still at HQ. Dismiss to HQ or release, then hire again at that place.

When reporting a bug, include what you did and the Papyrus log lines containing `CQM_` (enable Papyrus logging in `Skyrim.ini`: `[Papyrus] bEnableLogging=1`, `bEnableTrace=1`).
