extends Node

@export var file_drop_enabled = true
@export var cards_path: String

@export var card_source: CardSource

@export_group('Numerical values')
@export var simple_values: Array[ConfigValue]

@onready var SimpleValuesContainer = %SimpleValuesContainer

func _ready():
	append_simple_values()
	get_tree().root.files_dropped.connect(_on_files_dropped)
	
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
	var loot = {}
	for card in card_source.of_type('Loot'):
		loot[card.Key] = 10
	result['LootCards'] = loot
	
	return result
	
func load_json_file(path):
	var file = FileAccess.open(path, FileAccess.READ)
	var text = file.get_as_text()
	var config = JSON.parse_string(text)
	
	# values
	for child in %SimpleValuesContainer.get_children():
		if child is Label: continue
		
		var name = child.get_meta('value_name')
		# !FIXME horrible
		if child is SpinBox:
			child.value = config[name]
			continue
		if child is CheckBox:
			child.button_pressed = config[name]
			continue
	
	# card lists
	%Characters.load(config)
	%BonusSouls.load(config)
	%Treasures.load(config)
	
	# loot cards
	# TODO

func _on_files_dropped(files):
	if len(files) != 1:
		return
		
	var file = files[0]
	var format = file.substr(len(file) - 4)
	
	if format == 'json':
		load_json_file(file)
		return
		
	# TODO warn player
