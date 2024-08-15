extends Control
class_name RoomSlotScene

@onready var Card = %Card

# TODO mostly repeated code

func _ready():
	var p = %Card.custom_minimum_size.y
	%CardWrapper.custom_minimum_size = Vector2(p, p) * %Card.scale

func set_controller(controller: MatchController):
	Card.set_controller(controller)
	
func load_snapshot(slot: Variant):
	# TODO show overlay if slot.Card is null
	if slot.Card == null:
		%Card.visible = false
		return
	%Card.visible = true
	Card.load_snapshot(slot.Card)
	#Card.Behavior.set_shop_idx(slot.Idx)
