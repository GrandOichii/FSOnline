extends VBoxContainer
class_name BoardScene

# TODO utilize
@export var bottom_priority = false
@export var player_scene: PackedScene

@onready var Top = %Top
@onready var Bottom = %Bottom
@onready var DecksRow = %DecksRow

var _controller: MatchController

func _ready():
	while Top.get_child_count() > 0:
		Top.remove_child(Top.get_child(0))
	while Bottom.get_child_count() > 0:
		Bottom.remove_child(Bottom.get_child(0))
		
func set_controller(controller: MatchController):
	_controller = controller
	for player: PlayerScene in Top.get_children():
		player.set_controller(controller)
	for player: PlayerScene in Bottom.get_children():
		player.set_controller(controller)
	DecksRow.set_controller(controller)

func load_match_info(match_info: Variant):
	for i in match_info.PlayerCount:
		var child = player_scene.instantiate()
		var parent = Top
		if player_count() % 2 == 1:
			parent = Bottom
		parent.add_child(child)
		
		var player = child as PlayerScene
		player.set_controller(_controller)
		
	var i = 0
	if match_info.PlayerIdx != -1:
		i = 1
	for player: PlayerScene in Top.get_children():
		var p_idx = i
		player.set_player_idx(p_idx)
		i = (i + 1) % int(match_info.PlayerCount)
	for player: PlayerScene in Bottom.get_children():
		var p_idx = i
		player.set_player_idx(p_idx)
		i = (i + 1) % int(match_info.PlayerCount)
	pass
		
func load_snapshot(snapshot: Variant):
	for player: PlayerScene in Top.get_children():
		player.load_snapshot(snapshot)
	for player: PlayerScene in Bottom.get_children():
		player.load_snapshot(snapshot)
	DecksRow.load_snapshot(snapshot)
	
func player_count() -> int:
	return Top.get_child_count() + Bottom.get_child_count()
	
