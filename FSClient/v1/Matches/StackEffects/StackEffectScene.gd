extends Control
class_name StackEffectScene

@onready var Type = %Type
@onready var SID = %SID
@onready var Details = %Details

var _type_map = {
	'roll': 'Roll',
	'loot_play': 'Loot card',
	'ability_activation': 'Ability activation'
}

var _type_action_map = {
	'roll': func (snapshot):
		Details.text = 'Rolled: [color=red]' + str(snapshot.Value) + '[/color]'
}

func load_snapshot(snapshot: Variant):
	Type.text = snapshot.Type
	set_type(snapshot.Type)
	set_sid(snapshot.SID)
	Details.text = ''
	if snapshot.Type in _type_action_map:
		_type_action_map[snapshot.Type].call(snapshot)
	
func set_type(type: String):
	Type.text = _type_map[type]
	
func set_sid(sid: String):
	SID.text = '[' + sid + ']'

func set_controller(controller: MatchController):
	# TODO
	pass
