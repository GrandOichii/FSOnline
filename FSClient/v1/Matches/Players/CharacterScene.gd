extends Control
class_name CharacterScene

@onready var Card = %Card

func _ready():
	pass # Replace with function body.

func load_snapshot(snapshot: Variant):
	Card.load_snapshot(snapshot)
	
func set_controller(controller: MatchController):
	Card.set_controller(controller)
