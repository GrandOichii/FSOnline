extends Control
class_name MonsterSlotScene

@onready var Card = %Card

# TODO mostly repeated code

func _ready():
	%CardWrapper.custom_minimum_size = %Card.custom_minimum_size * %Card.scale

func set_controller(controller: MatchController):
	Card.set_controller(controller)
	
func load_snapshot(slot: Variant):
	# TODO show overlay if slot.Card is null
	Card.load_snapshot(slot.Card)
	Card.Card.Behavior.set_slot_idx(slot.Idx)
