extends Control
class_name StackEffectScene

@onready var Type = %Type
@onready var SID = %SID

var _type_map = {
	'roll': 'Roll',
	'loot_play': 'Loot card',
	'ability_activation': 'Ability activation'
}

func load_snapshot(snapshot: Variant):
	Type.text = snapshot.Type
	set_type(snapshot.Type)
	set_sid(snapshot.SID)
	
func set_type(type: String):
	Type.text = _type_map[type]
	
func set_sid(sid: String):
	SID.text = '[' + sid + ']'

func set_controller(controller: MatchController):
	# TODO
	pass
