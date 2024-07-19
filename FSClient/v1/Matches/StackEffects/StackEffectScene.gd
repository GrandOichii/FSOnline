extends Control
class_name StackEffectScene

@onready var Type = %Type
@onready var SID = %SID

func load_snapshot(snapshot: Variant):
	Type.text = snapshot.Type
	set_sid(snapshot.SID)
	
func set_sid(sid: String):
	SID.text = '[' + sid + ']'

func set_controller(controller: MatchController):
	# TODO
	pass
