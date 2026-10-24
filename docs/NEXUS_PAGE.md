# Nexus Mods page: Company Quartermaster

Early beta: v0.2.3 passes every automated build check but nobody has playtested it in game yet. The page says this at the top. The exact upload-form values are in [NEXUS_FIELDS.md](NEXUS_FIELDS.md).

## Page settings

- **Game:** Skyrim Special Edition
- **Mod name:** Company Quartermaster
- **Version:** 0.2.3 (early beta)
- **Category:** Gameplay
- **Suggested tags:** Gameplay, Economy, Followers, Lore-Friendly, ESL, No SKSE, Scripted
- **Requirements:** none. Ember Smithing is optional and is not a master.
- **Permissions (suggested):** the source is on GitHub under MIT, so allow modification and conversions with credit, but not re-uploads of the archive itself.
- **File to upload:** `CompanyQuartermaster-0.2.3.zip` (FOMOD) as the main file, named "Company Quartermaster".

## Description (paste into the BBCode editor)

```bbcode
[center][size=6][b]COMPANY QUARTERMASTER[/b][/size]
[size=3][i]No hall. An inn, a ledger, and a woman who counts.[/i][/size]

[size=3][color=#e0a030][b]EARLY BETA (v0.2.3)[/b]: this version passes every automated build check but hasn't been playtested in game yet. Please report bugs in the Bugs tab or on GitHub, ideally with your Papyrus log. Saves from 0.1.0, 0.2.0, 0.2.1 and 0.2.2 keep working.[/color][/size][/center]

[line]

[size=4][b]Summary[/b][/size]
Hire Brynja Tally-Hand at an inn. She sells supplies, buys clutter in bulk, refits your followers' gear, and keeps a stash for each of them. She travels with you, waits in camp, or waits at a company HQ you set at an inn or a house you own. She is not a follower and does not take a follower slot.

The Blackshield Company has no hall of its own, so the hire is at an inn. Base game only. No SKSE. ESL-flagged. Edits no vanilla records.

[size=4][b]What she does[/b][/size]
[list]
[*][b]Trade supplies[/b]. Bread, cheese, venison, potions, arrows, leather, iron, charcoal. She buys only that list.
[*][b]Sell all junk[/b]. Tankards, plates, goblets, silver candlesticks and the like, at a flat bulk rate (60% by default). You confirm the gold first. Quest items, gems and ingots are never touched.
[*][b]Follower services[/b]. A field refit (+40 armor, +10% weapon damage for 3 days) or a personal stash. Eight stashes, and they keep their contents if you dismiss her.
[*][b]Travel or camp[/b]. She follows, or she waits where you left her. Fast travel brings her along only while she is travelling with you.
[*][b]Company HQ[/b]. Set it at an inn or a house you own. Send her there to wait, and hire her there again. If you leave that cell she is sent back on a load or a location change. Dismiss can send her to HQ instead of releasing her.
[/list]

If Ember Smithing is installed, the refit menu also offers an ember refit (+60 armor, +15% damage) for charcoal and fire salts. Without that mod the option is hidden. This mod does not require it.

[size=4][b]Changing her look[/b][/size]
Brynja borrows Uthgerd the Unbroken's face through a Use Traits template, so the game's own FaceGen is used. In SSEEdit open CompanyQuartermaster.esp, find NPC CQM_Brynja (xx000820) and point Template at another vanilla female NPC (or save that into your own override plugin), then hire her again. A custom face needs FaceGen exported in the Creation Kit (Ctrl+F4) or it renders dark grey.

[size=4][b]Requirements[/b][/size]
[list]
[*]Skyrim Special Edition or Anniversary Edition (1.5.97 or 1.6.x).
[*]No SKSE, no SkyUI, no DLC, no Creation Club.
[/list]

[size=4][b]Install[/b][/size]
FOMOD. Enable CompanyQuartermaster.esp. It is ESL-flagged and overrides nothing.

[size=4][b]Settings[/b][/size]
[code]
set CQM_HireCost to 500
set CQM_ServiceRadius to 1500
set CQM_SellRate to 0.6
set CQM_RepairDays to 3
set CQM_RepairGold to 50
set CQM_CatchupDistance to 3000
[/code]

[size=4][b]The series[/b][/size]
Part of the Blackshield series. She does not take the follower slot used by Sellsword Roster or the Blackshield Company sellsword. Those companions can still use her refit and her stashes.

[url=https://github.com/Blackshield-Company/company-quartermaster]Source and issues[/url].

Donations are optional. Nothing is gated.
[url=https://github.com/sponsors/synthalorian]GitHub Sponsors[/url] · [url=https://ko-fi.com/synthalorian]Ko-fi[/url]
```
