extends Control
class_name NumericCardRowScene

var key: String
var _source: CardSource

func _ready():
	%NotFound.hide()

func set_source(source: CardSource):
	_source = source

func load(new_key: String, amount: int):
	key = new_key
	%Key.text = key
	%Amount.value = amount
	%NotFound.visible = not _source.has_key(key)
	
func add_amount(amount: int):
	%Amount.value += amount
	
func get_amount() -> int:
	return %Amount.value

func _on_key_text_changed(new_text):
	%NotFound.visible = not _source.has_key(new_text)

func _on_amount_value_changed(value):
	if value == 0:
		queue_free()
