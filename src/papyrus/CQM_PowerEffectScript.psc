Scriptname CQM_PowerEffectScript extends ActiveMagicEffect
{Company Quartermaster - the lesser power opens Brynja's menu.}

CQM_QuartermasterScript Property Quartermaster Auto

Event OnEffectStart(Actor akTarget, Actor akCaster)
	If akCaster == Game.GetPlayer()
		Quartermaster.UsePower()
	EndIf
EndEvent
