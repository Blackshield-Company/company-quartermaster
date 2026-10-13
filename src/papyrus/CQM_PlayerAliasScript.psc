Scriptname CQM_PlayerAliasScript extends ReferenceAlias
{Company Quartermaster - grants the power, and on a location change or a load
brings Brynja along in travel mode, or back to the HQ cell in HQ mode.}

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
