extends Node

@export var file_drop_enabled = true
@export var cards_path: String

@export var CardSource: CardSource

@export_group('Numerical values')
@export var simple_values: Array[ConfigValue]

@onready var SimpleValuesContainer = %SimpleValuesContainer

func _ready():
	append_simple_values()
	
func append_simple_values():
	for value in simple_values:
		value.append_control_nodes(SimpleValuesContainer)
