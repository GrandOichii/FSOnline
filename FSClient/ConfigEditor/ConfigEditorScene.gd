extends Node

@export var file_drop_enabled = true
@export var cards_path: String

@export var card_source: CardSource

@export_group('Numerical values')
@export var simple_values: Array[ConfigValue]

@onready var SimpleValuesContainer = %SimpleValuesContainer

func _ready():
	append_simple_values()
	
	var d = build()
	var text = %JSON.Encode(d)
	print(text)
	
func append_simple_values():
	for value in simple_values:
		value.append_control_nodes(SimpleValuesContainer)
		
func build() -> Dictionary:
	var result = {}
	
	# values
	# TODO
	for child in %SimpleValuesContainer.get_children():
		if child is Label: continue
		
		var name = child.get_meta('value_name')
		# !FIXME horrible
		if child is SpinBox:
			result[name] = child.value
			continue
		if child is CheckBox:
			result[name] = child.button_pressed
			
			continue
	
	# card lists
	%Characters.modify(result)
	%BonusSouls.modify(result)
	%Treasures.modify(result)
	
	# starting items
	var si = []
	for card in card_source.of_type('StartingItem'):
		si.append(card.Key)
	result['StartingItems'] = si
	# TODO
	
	# loot cards
	# TODO
	
	return result
