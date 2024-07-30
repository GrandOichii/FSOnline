extends Control
class_name CardListScene

@export var list_name: String
@export var list_label: String
@export var card_type: String
@export var card_source: CardSource

@export var card_row_scene: PackedScene

func _ready():
	%ListName.text = list_label

	# fill default
	var cards = card_source.of_type(card_type)
	for card in cards:
		add_key(card.Key)
	set_key_filter('')

func set_key_filter(filter: String):
	%PossibleKeys.clear()
	var cards = card_source.of_type(card_type)
	for card in cards:
		if filter == '' or filter.to_lower() in card.LowerKey():
			add_possible_key(card.Key) 

func add_possible_key(key: String):
	%PossibleKeys.add_item(key)
	
func add_key(key: String):
	var child = card_row_scene.instantiate()
	%Keys.add_child(child)
	
	var row = child as CardRowScene
	row.set_source(card_source)
	row.load(key)

func _on_add_key_button_pressed():
	var key = %NewKeyEdit.text
	%NewKeyEdit.text = ''
	add_key(key)

func _on_new_key_edit_text_submitted(new_text):
	_on_add_key_button_pressed()

func _on_new_key_edit_text_changed(new_text):
	set_key_filter(new_text)

func _on_possible_keys_item_activated(index):
	add_key(%PossibleKeys.get_item_text(index))
