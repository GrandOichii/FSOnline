extends Control
class_name TreasureSlotScene

@onready var Cost = %Cost
@onready var Card = %Card

# Called when the node enters the scene tree for the first time.
func _ready():
	%CardWrapper.custom_minimum_size = %Card.custom_minimum_size * %Card.scale
	Cost.text = ''

func set_controller(controller: MatchController):
	Card.set_controller(controller)
	
func load_snapshot(slot: Variant):
	# TODO show overlay if slot.Card is null
	Card.load_snapshot(slot.Card)
	Card.Behavior.set_shop_idx(slot.Idx)
