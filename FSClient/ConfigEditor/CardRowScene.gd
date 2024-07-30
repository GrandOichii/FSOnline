extends HBoxContainer
class_name CardRowScene

var _source: CardSource

func _ready():
	%NotFound.hide()

func set_source(source: CardSource):
	_source = source

func load(key: String):
	%Key.text = key
	%NotFound.visible = not _source.has_key(key)

func _on_delete_button_pressed():
	queue_free()

func _on_key_text_changed(new_text):
	%NotFound.visible = not _source.has_key(new_text)
