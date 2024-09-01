extends Node

@export var file_drop_enabled = true
@export var base_loot_amount: int = 1

@export var card_source: CardSource

@export_group('Numerical values')
@export var simple_values: Array[ConfigValue]

@onready var SimpleValuesContainer = %SimpleValuesContainer

func _ready():
	append_simple_values()
	get_tree().root.files_dropped.connect(_on_files_dropped)
	%Loot.base_value = base_loot_amount
	
func append_simple_values():
	for value in simple_values:
		value.append_control_nodes(SimpleValuesContainer)
		
func build() -> Dictionary:
	var result = {}
	
	# values
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
	%Rooms.modify(result)
	%Monsters.modify(result)
	%Events.modify(result)
	%Curses.modify(result)
	
	# starting items
	var si = []
	for card in card_source.of_type('StartingItem'):
		si.append(card.Key)
	result['StartingItems'] = si
	
	# loot cards
	%Loot.modify(result)
	
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
	%Rooms.load(config)
	%Monsters.load(config)
	%Events.load(config)
	%Curses.load(config)
	
	# loot cards
	%Loot.load(config)

func _on_files_dropped(files):
	if len(files) != 1:
		return
		
	var file = files[0]
	var format = file.substr(len(file) - 4)
	
	if format == 'json':
		load_json_file(file)
		return
		
	# TODO confirm
