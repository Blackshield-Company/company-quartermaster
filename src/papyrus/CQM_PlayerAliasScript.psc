Scriptname CQM_PlayerAliasScript extends ReferenceAlias
{Company Quartermaster - grants the power, and in travel mode brings Brynja
along on a location change or a load (fast travel).}

CQM_QuartermasterScript Property Quartermaster Auto

Event OnInit()
	Quartermaster.EnsurePower()
EndEvent

Event OnPlayerLoadGame()
	Quartermaster.GameLoaded()
EndEvent

Event OnLocationChange(Location akOldLoc, Location akNewLoc)
	Quartermaster.CatchUp()
EndEvent
