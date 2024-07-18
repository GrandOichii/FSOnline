extends VBoxContainer
class_name BoardScene

@export var bottom_priority = false
@export var player_scene: PackedScene

@onready var Top = %Top
@onready var Bottom = %Bottom

func _ready():
	pass # Replace with function body.

func load_match_info(match_info: Variant):
	var pCount = match_info.PlayerCount
	print(pCount)
