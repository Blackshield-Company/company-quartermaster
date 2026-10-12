Scriptname CQM_QuartermasterScript extends Quest
{Company Quartermaster - hire Brynja Tally-Hand at an inn. She sells supplies,
buys listed clutter in bulk, refits follower gear, and keeps a stash per
follower. She is not a teammate and never takes a follower slot.
No RegisterForUpdate loop: menus come from the lesser power, catch-up from
OnLocationChange / OnPlayerLoadGame, refit expiry from one game-time timer.}

GlobalVariable Property CQM_PowerGranted Auto
GlobalVariable Property CQM_HireCost Auto
GlobalVariable Property CQM_ServiceRadius Auto
GlobalVariable Property CQM_SellRate Auto
GlobalVariable Property CQM_RepairDays Auto
GlobalVariable Property CQM_RepairGold Auto
GlobalVariable Property CQM_CatchupDistance Auto
GlobalVariable Property CQM_EmberPresent Auto
GlobalVariable Property CQM_Hired Auto
GlobalVariable Property CQM_Traveling Auto

ActorBase Property CQM_Brynja Auto
Spell Property CQM_PowerQuartermaster Auto
Spell Property CQM_AbRepaired Auto
Spell Property CQM_AbEmberRepaired Auto
Keyword Property LocTypeInn Auto

MiscObject Property Gold001 Auto
MiscObject Property LeatherStrips Auto
MiscObject Property IngotIron Auto
MiscObject Property Charcoal Auto
Ingredient Property FireSalts Auto

FormList[] Property JunkLists Auto
Int[] Property TierValues Auto
ObjectReference[] Property Stashes Auto
ObjectReference Property CQM_CampMarker Auto

ReferenceAlias Property CQM_TravelAlias Auto
ReferenceAlias Property CQM_CampAlias Auto
Quest Property CQM_FollowerQuest Auto
ReferenceAlias[] Property Followers Auto

Message Property CQM_MsgIntro Auto
Message Property CQM_MsgNotInn Auto
Message Property CQM_MsgHire Auto
Message Property CQM_MsgNoGold Auto
Message Property CQM_MsgHiredTravel Auto
Message Property CQM_MsgHiredCamp Auto
Message Property CQM_MsgTooFar Auto
Message Property CQM_MsgSellConfirm Auto
Message Property CQM_MsgSellNone Auto
Message Property CQM_MsgFollowers Auto
Message Property CQM_MsgNobody Auto
Message[] Property ServiceMenus Auto
Message Property CQM_MsgRepair Auto
Message Property CQM_MsgRepairMats Auto
Message Property CQM_MsgEmberMats Auto
Message Property CQM_MsgRepairFull Auto
Message Property CQM_MsgStashFull Auto
Message Property CQM_MsgDismiss Auto
Message Property CQM_MsgFallen Auto

Int Property STASH_COUNT = 8 AutoReadOnly
Int Property FOLLOWER_COUNT = 5 AutoReadOnly

Actor[] stashOwners
Actor[] repairActors
Float[] repairUntil
Int[] repairKind

Event OnInit()
	EnsureArrays()
	CheckEmber()
	EnsurePower()
EndEvent

Event OnUpdateGameTime()
	ProcessRepairs()
	ScheduleRepair()
EndEvent

Function GameLoaded()
	EnsureArrays()
	CheckEmber()
	EnsurePower()
	SyncState()
	CatchUp()
	ScheduleRepair()
EndFunction

Function EnsureArrays()
	If stashOwners == None || stashOwners.Length < STASH_COUNT
		stashOwners = new Actor[8]
	EndIf
	If repairActors == None || repairActors.Length < FOLLOWER_COUNT
		repairActors = new Actor[5]
		repairUntil = new Float[5]
		repairKind = new Int[5]
	EndIf
EndFunction

; Presence only. Ember repair spends vanilla charcoal and fire salts.
Function CheckEmber()
	Form emb = Game.GetFormFromFile(0x803, "EmberSmithing.esp") ; soft: EMB_EmberForgedIronSword
	If emb
		CQM_EmberPresent.SetValueInt(1)
	Else
		CQM_EmberPresent.SetValueInt(0)
	EndIf
EndFunction

Function EnsurePower()
	Actor player = Game.GetPlayer()
	If !player
		Return
	EndIf
	If !player.HasSpell(CQM_PowerQuartermaster)
		player.AddSpell(CQM_PowerQuartermaster, False)
	EndIf
	If CQM_PowerGranted.GetValueInt() == 0
		CQM_PowerGranted.SetValueInt(1)
		CQM_MsgIntro.Show()
		Debug.Trace("[CompanyQuartermaster] CQM_ power granted")
	EndIf
EndFunction

Actor Function Brynja()
	Actor a = CQM_TravelAlias.GetActorReference()
	If a
		Return a
	EndIf
	Return CQM_CampAlias.GetActorReference()
EndFunction

Bool Function HiredAlive()
	Actor b = Brynja()
	If !b || b.IsDead() || b.IsDisabled()
		Return False
	EndIf
	Return True
EndFunction

Function SyncState()
	If HiredAlive()
		CQM_Hired.SetValueInt(1)
		If CQM_TravelAlias.GetActorReference()
			CQM_Traveling.SetValueInt(1)
		Else
			CQM_Traveling.SetValueInt(0)
		EndIf
	Else
		CQM_Hired.SetValueInt(0)
		CQM_Traveling.SetValueInt(0)
	EndIf
EndFunction

Bool Function InInn()
	Actor player = Game.GetPlayer()
	Location loc = player.GetCurrentLocation()
	If !loc
		Return False
	EndIf
	Return loc.HasKeyword(LocTypeInn)
EndFunction

Bool Function NearBrynja()
	Actor b = Brynja()
	Actor player = Game.GetPlayer()
	If !b || b.IsDisabled()
		Return False
	EndIf
	If b.GetParentCell() != player.GetParentCell()
		Return False
	EndIf
	Return b.GetDistance(player) <= CQM_ServiceRadius.GetValue()
EndFunction

; Called by the lesser power.
Function UsePower()
	EnsureArrays()
	Actor b = Brynja()
	If b && b.IsDead()
		CQM_MsgFallen.Show()
		DismissBody(b)
		b = None
	EndIf
	If !HiredAlive()
		OfferHire()
		Return
	EndIf
	OpenHiredMenu()
EndFunction

Function OfferHire()
	If !InInn()
		CQM_MsgNotInn.Show()
		Return
	EndIf
	Int cost = CQM_HireCost.GetValueInt()
	If cost < 0
		cost = 0
	EndIf
	If CQM_MsgHire.Show(cost as Float) != 0
		Return
	EndIf
	Actor player = Game.GetPlayer()
	If player.GetItemCount(Gold001) < cost
		CQM_MsgNoGold.Show(cost as Float)
		Return
	EndIf
	If cost > 0
		player.RemoveItem(Gold001, cost, True)
	EndIf
	Hire(player)
EndFunction

Function Hire(Actor player)
	Actor b = player.PlaceActorAtMe(CQM_Brynja)
	CQM_CampAlias.Clear()
	CQM_TravelAlias.ForceRefTo(b)
	Float ang = player.GetAngleZ()
	b.MoveTo(player, -100.0 * Math.Sin(ang), -100.0 * Math.Cos(ang), 0.0)
	b.Enable()
	b.SetPlayerTeammate(False, False)
	b.SetRelationshipRank(player, 3)
	b.EvaluatePackage()
	CQM_Hired.SetValueInt(1)
	CQM_Traveling.SetValueInt(1)
	Debug.Notification("Brynja Tally-Hand signs on.")
	Debug.Trace("[CompanyQuartermaster] CQM_ hired")
EndFunction

Function OpenHiredMenu()
	Int choice = -1
	If CQM_TravelAlias.GetActorReference()
		choice = CQM_MsgHiredTravel.Show()
	Else
		choice = CQM_MsgHiredCamp.Show()
	EndIf
	If choice == 0
		Trade()
	ElseIf choice == 1
		SellJunk()
	ElseIf choice == 2
		FollowerServices()
	ElseIf choice == 3
		ToggleTravel()
	ElseIf choice == 4
		AskDismiss()
	EndIf
EndFunction

Function Trade()
	If !NearBrynja()
		CQM_MsgTooFar.Show()
		Return
	EndIf
	Actor b = Brynja()
	b.ShowBarterMenu()
EndFunction

Function SellJunk()
	If !NearBrynja()
		CQM_MsgTooFar.Show()
		Return
	EndIf
	Actor player = Game.GetPlayer()
	Int n = JunkLists.Length
	Int[] counts = new Int[8]
	If n > 8
		n = 8
	EndIf
	Float gross = 0.0
	Int i = 0
	While i < n
		Int c = player.GetItemCount(JunkLists[i])
		counts[i] = c
		gross += c * TierValues[i]
		i += 1
	EndWhile
	If gross <= 0.0
		CQM_MsgSellNone.Show()
		Return
	EndIf
	Float rate = CQM_SellRate.GetValue()
	If rate < 0.0
		rate = 0.0
	EndIf
	Int pay = Math.Floor(gross * rate + 0.5)
	If pay < 1
		pay = 1
	EndIf
	If CQM_MsgSellConfirm.Show(pay as Float) != 0
		Return
	EndIf
	i = 0
	While i < n
		If counts[i] > 0
			player.RemoveItem(JunkLists[i], counts[i], True)
		EndIf
		i += 1
	EndWhile
	player.AddItem(Gold001, pay, True)
	Debug.Notification("Brynja pays " + pay + " gold for the clutter.")
	Debug.Trace("[CompanyQuartermaster] CQM_ sold clutter for " + pay)
EndFunction

Function FollowerServices()
	CQM_FollowerQuest.Stop()
	CQM_FollowerQuest.Start()
	Int choice = CQM_MsgFollowers.Show()
	If choice < 0 || choice >= FOLLOWER_COUNT
		CQM_FollowerQuest.Stop()
		Return
	EndIf
	Actor who = Followers[choice].GetActorReference()
	If !who || who.IsDead()
		CQM_FollowerQuest.Stop()
		CQM_MsgNobody.Show()
		Return
	EndIf
	Int act = ServiceMenus[choice].Show()
	CQM_FollowerQuest.Stop()
	If act == 0
		RepairMenu(who)
	ElseIf act == 1
		OpenStash(who)
	EndIf
EndFunction

Function RepairMenu(Actor who)
	Float days = CQM_RepairDays.GetValue()
	If days < 0.25
		days = 0.25
	EndIf
	Int gold = CQM_RepairGold.GetValueInt()
	If gold < 0
		gold = 0
	EndIf
	Int choice = CQM_MsgRepair.Show(days, gold as Float)
	If choice == 0
		DoRepair(who, False, days, gold)
	ElseIf choice == 1
		DoRepair(who, True, days, gold)
	EndIf
EndFunction

Function DoRepair(Actor who, Bool ember, Float days, Int gold)
	Actor player = Game.GetPlayer()
	If ember
		If player.GetItemCount(Charcoal) < 2 || player.GetItemCount(FireSalts) < 1
			CQM_MsgEmberMats.Show()
			Return
		EndIf
	Else
		If player.GetItemCount(LeatherStrips) < 2 || player.GetItemCount(IngotIron) < 1 || player.GetItemCount(Gold001) < gold
			CQM_MsgRepairMats.Show(gold as Float)
			Return
		EndIf
	EndIf
	Int slot = RepairSlot(who)
	If slot < 0
		CQM_MsgRepairFull.Show()
		Return
	EndIf
	If ember
		player.RemoveItem(Charcoal, 2, True)
		player.RemoveItem(FireSalts, 1, True)
	Else
		player.RemoveItem(LeatherStrips, 2, True)
		player.RemoveItem(IngotIron, 1, True)
		If gold > 0
			player.RemoveItem(Gold001, gold, True)
		EndIf
	EndIf
	ClearRepair(slot)
	If ember
		who.RemoveSpell(CQM_AbRepaired)
		who.AddSpell(CQM_AbEmberRepaired, False)
		repairKind[slot] = 2
		Debug.Notification("Ember refit applied.")
	Else
		who.RemoveSpell(CQM_AbEmberRepaired)
		who.AddSpell(CQM_AbRepaired, False)
		repairKind[slot] = 1
		Debug.Notification("Field refit applied.")
	EndIf
	repairActors[slot] = who
	repairUntil[slot] = Utility.GetCurrentGameTime() + days
	ScheduleRepair()
	Debug.Trace("[CompanyQuartermaster] CQM_ repair slot " + slot + " ember=" + ember)
EndFunction

Int Function RepairSlot(Actor who)
	Int free = -1
	Int i = 0
	While i < FOLLOWER_COUNT
		If repairActors[i] == who
			Return i
		EndIf
		If free < 0
			If repairActors[i] == None || repairUntil[i] <= 0.0
				free = i
			ElseIf repairActors[i].IsDead()
				free = i
			EndIf
		EndIf
		i += 1
	EndWhile
	Return free
EndFunction

Function ProcessRepairs()
	Float now = Utility.GetCurrentGameTime()
	Int i = 0
	While i < FOLLOWER_COUNT
		If repairUntil[i] > 0.0 && now >= repairUntil[i]
			ClearRepair(i)
			Debug.Notification("A field refit has worn off.")
		EndIf
		i += 1
	EndWhile
EndFunction

Function ClearRepair(Int i)
	Actor a = repairActors[i]
	If a
		a.RemoveSpell(CQM_AbRepaired)
		a.RemoveSpell(CQM_AbEmberRepaired)
	EndIf
	repairActors[i] = None
	repairUntil[i] = 0.0
	repairKind[i] = 0
EndFunction

Function ScheduleRepair()
	UnregisterForUpdateGameTime()
	Float now = Utility.GetCurrentGameTime()
	Float earliest = 0.0
	Int i = 0
	While i < FOLLOWER_COUNT
		If repairUntil[i] > now
			If earliest <= 0.0 || repairUntil[i] < earliest
				earliest = repairUntil[i]
			EndIf
		ElseIf repairUntil[i] > 0.0
			ClearRepair(i)
		EndIf
		i += 1
	EndWhile
	If earliest > now
		Float hours = (earliest - now) * 24.0
		If hours < 0.1
			hours = 0.1
		EndIf
		RegisterForSingleUpdateGameTime(hours)
	EndIf
EndFunction

Function OpenStash(Actor who)
	Int slot = StashSlot(who)
	If slot < 0
		CQM_MsgStashFull.Show()
		Return
	EndIf
	If stashOwners[slot] != who
		stashOwners[slot] = who
		Debug.Trace("[CompanyQuartermaster] CQM_ stash " + slot + " assigned")
	EndIf
	Stashes[slot].Activate(Game.GetPlayer())
EndFunction

Int Function StashSlot(Actor who)
	Int free = -1
	Int dead = -1
	Int i = 0
	While i < STASH_COUNT
		Actor s = stashOwners[i]
		If s == who
			Return i
		EndIf
		If free < 0 && s == None
			free = i
		EndIf
		If dead < 0 && s != None && s.IsDead()
			dead = i
		EndIf
		i += 1
	EndWhile
	If free >= 0
		Return free
	EndIf
	Return dead
EndFunction

Function ToggleTravel()
	Actor b = Brynja()
	If !b
		Return
	EndIf
	Actor player = Game.GetPlayer()
	If CQM_TravelAlias.GetActorReference()
		CQM_CampMarker.MoveTo(b)
		CQM_TravelAlias.Clear()
		CQM_CampAlias.ForceRefTo(b)
		CQM_Traveling.SetValueInt(0)
		b.EvaluatePackage()
		Debug.Notification("Brynja makes camp.")
		Debug.Trace("[CompanyQuartermaster] CQM_ camp")
	Else
		CQM_CampAlias.Clear()
		CQM_TravelAlias.ForceRefTo(b)
		Float ang = player.GetAngleZ()
		b.MoveTo(player, -100.0 * Math.Sin(ang), -100.0 * Math.Cos(ang), 0.0)
		CQM_Traveling.SetValueInt(1)
		b.EvaluatePackage()
		Debug.Notification("Brynja travels with the company.")
		Debug.Trace("[CompanyQuartermaster] CQM_ travel")
	EndIf
EndFunction

; Fast travel and load. Camp mode does not move her.
Function CatchUp()
	Actor b = CQM_TravelAlias.GetActorReference()
	If !b || b.IsDead() || b.IsDisabled()
		Return
	EndIf
	Actor player = Game.GetPlayer()
	Float limit = CQM_CatchupDistance.GetValue()
	If limit < 256.0
		limit = 256.0
	EndIf
	Bool otherCell = b.GetParentCell() != player.GetParentCell()
	Bool far = False
	If !otherCell
		far = b.GetDistance(player) > limit
	EndIf
	If otherCell || far
		Float ang = player.GetAngleZ()
		b.MoveTo(player, -100.0 * Math.Sin(ang), -100.0 * Math.Cos(ang), 0.0)
		b.EvaluatePackage()
		Debug.Trace("[CompanyQuartermaster] CQM_ catch-up")
	EndIf
EndFunction

Function AskDismiss()
	If CQM_MsgDismiss.Show() != 0
		Return
	EndIf
	Actor b = Brynja()
	DismissBody(b)
	Debug.Notification("Brynja Tally-Hand takes her leave.")
	Debug.Trace("[CompanyQuartermaster] CQM_ dismissed")
EndFunction

Function DismissBody(Actor b)
	CQM_TravelAlias.Clear()
	CQM_CampAlias.Clear()
	If b
		b.SetPlayerTeammate(False, False)
		b.Disable()
		b.Delete()
	EndIf
	CQM_Hired.SetValueInt(0)
	CQM_Traveling.SetValueInt(0)
EndFunction
