extends CardSource
class_name FileCardSource

@export var cards_path: String = ''

@onready var CardMaster = %CardMaster

func _ready():
	CardMaster.Load(cards_path)

func of_type(type: String):
	return CardMaster.GetOfType(type)
