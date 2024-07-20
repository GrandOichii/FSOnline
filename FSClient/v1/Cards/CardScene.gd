extends Control
class_name CardScene

@export var images: CardImages
@export var Behavior: CardBehavior

@onready var Bg = %Bg
@onready var Art = %Art

var _prev_key = ''

func _ready():
	if Behavior != null:
		Behavior.set_card(self)
		
func set_controller(controller: MatchController):
	if Behavior != null:
		Behavior.set_controller(controller)
	
func load_snapshot(snapshot: Variant):
	set_key(snapshot.Key)
	if Behavior != null:
		Behavior.load_snapshot(snapshot)

func set_key(key: String):
	if _prev_key == key:
		return
	_prev_key = key
	var tex = images.get_image_for(_prev_key)
	Art.texture = tex
	
func set_bg_color(color: Color):
	Bg.get('theme_override_styles/panel').bg_color = color

# signal connections

func _on_mouse_entered():
	if Behavior != null:
		Behavior.mouse_enter()
	
func _on_mouse_exited():
	if Behavior != null:
		Behavior.mouse_leave()

func _on_gui_input(e):
	if e.is_action_pressed('interact'):
		if Behavior != null:
			Behavior.click()
