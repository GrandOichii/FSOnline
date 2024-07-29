extends Control
class_name CardListScene

@export var deck_name: String
@export var card_type: String
@export var card_source: CardSource

func _ready():
	%DeckName.text = deck_name

func _on_add_key_button_pressed():
	var cards = card_source.of_type(card_type)
	for card in cards:
		print(card.Key)
