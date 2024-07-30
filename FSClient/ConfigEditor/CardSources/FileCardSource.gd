extends CardSource
class_name FileCardSource

@export var cards_path: String = ''

func _ready():
	%CardMaster.Load(cards_path)

func of_type(type: String):
	return %CardMaster.GetOfType(type)

func has_key(key: String):
	return %CardMaster.HasKey(key)
