extends CardSource
class_name FileCardSource

@export var card_paths: Array[String] = []

func _ready():
	for path in card_paths:
		%CardMaster.Load(path)

func of_type(type: String):
	return %CardMaster.GetOfType(type)

func has_key(key: String):
	return %CardMaster.HasKey(key)
