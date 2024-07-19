extends InPlayCardBehavior
class_name OwnedCardBehavior

@export_group('Colors')
@export var DefaultColor: Color
@export var PickColor: Color
@export var PickHoverColor: Color
@export var CanActivateColor: Color
@export var CanActivateHoverColor: Color

var _bg_color = DefaultColor
var _last: Variant = null

func load_snapshot(card: Variant):
	_last = card

func can_activate():
	if Controller == null:
		return false
	if _last == null:
		return false
	return Controller.can_activate(_last)

func determine_bg_color():
	_bg_color = DefaultColor
	if can_activate():
		_bg_color = CanActivateColor
		return

func mouse_enter():
	super.mouse_enter()
	if can_activate():
		set_bg_color(CanActivateHoverColor)
		return

func mouse_leave():
	set_bg_color(_bg_color)

func click():
	super.click()
	
	if can_activate():
		Controller.activate(_last)
		return

func OnUpdate(update: Variant):
	super.OnUpdate(update)
	_bg_color = DefaultColor
	determine_bg_color()
	set_bg_color(_bg_color)
