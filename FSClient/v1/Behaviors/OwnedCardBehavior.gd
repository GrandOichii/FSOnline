extends InPlayCardBehavior
class_name OwnedCardBehavior

@export_group('Colors')
@export var can_activate_color: Color
@export var can_activate_hover_color: Color

func can_activate():
	if Controller == null:
		return false
	if _last == null:
		return false
	return Controller.can_activate(_last)

func determine_bg_color():
	super.determine_bg_color()
	if can_activate():
		_bg_color = can_activate_color
		return

func mouse_enter():
	super.mouse_enter()
	if can_activate():
		set_bg_color(can_activate_hover_color)
		return

func click():
	super.click()
	
	if can_activate():
		Controller.activate(_last)
		return
