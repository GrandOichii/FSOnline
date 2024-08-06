extends Control

@export var list_name: String
@export var list_label: String
@export var base_value: int = 1
@export var card_type: String
@export var card_source: CardSource

@export var numeric_card_row_scene: PackedScene

func _ready():
	%ListName.text = list_label

	# fill default
	var cards = card_source.of_type(card_type)
	for card in cards:
		add_key(card.Key, base_value)
	set_key_filter('')

func set_key_filter(filter: String):
	%PossibleKeys.clear()
	var cards = card_source.of_type(card_type)
	for card in cards:
		if filter == '' or filter.to_lower() in card.LowerKey():
			add_possible_key(card.Key) 

func add_possible_key(key: String):
	%PossibleKeys.add_item(key)
	
func add_key(key: String, amount: int):
	var child = numeric_card_row_scene.instantiate()
	%Keys.add_child(child)
	
	var row = child as NumericCardRowScene
	row.set_source(card_source)
	row.load(key, amount)
	
func modify(d: Dictionary):
	var dict = {}
	# TODO duplicates
	for row: NumericCardRowScene in %Keys.get_children():
		dict[row.key] = row.get_amount()
	d[list_name] = dict
	
func load(config: Dictionary):
	while %Keys.get_child_count() > 0:
		%Keys.remove_child(%Keys.get_child(0))
	
	for key in config[list_name]:
		add_key(key, config[list_name][key])

func _on_add_key_button_pressed():
	var key = %NewKeyEdit.text
	%NewKeyEdit.text = ''
	add_key(key, 1)

func _on_new_key_edit_text_submitted(new_text):
	_on_add_key_button_pressed()

func _on_new_key_edit_text_changed(new_text):
	set_key_filter(new_text)

func _on_possible_keys_item_activated(index):
	var key = %PossibleKeys.get_item_text(index)
	for child: NumericCardRowScene in %Keys.get_children():
		if child.key == key:
			child.add_amount(1)
			return
	add_key(key, 1)

func _on_filter_text_changed(new_text):
	for child: NumericCardRowScene in %Keys.get_children():
		child.visible = new_text == '' or new_text.to_lower() in child.key.to_lower()
