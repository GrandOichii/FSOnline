extends Control
class_name HandCardScene

@onready var Card = %Card
@onready var Overlay = %Overlay

var player_idx: int
var hand_idx: int

func _ready():
	pass # Replace with function body.
	
func set_controller(controller: MatchController):
	Card.set_controller(controller)

func set_player_idx(idx: int):
	player_idx = idx
	
func set_hand_idx(idx: int):
	hand_idx = idx
	Card.Behavior.set_hand_idx(idx)
	
func load_snapshot(snapshot: Variant):
	var player = snapshot.Players[player_idx]
	Overlay.show()
	
	if hand_idx >= len(player.VisibleHandCards):
		Card.Behavior.set_visible(false)
		return
	
	# card is visible
	Overlay.hide()
	Card.Behavior.set_visible(true)
	Card.load_snapshot(player.VisibleHandCards[hand_idx])
	
