extends CardBehavior
class_name InPlayCardBehavior

@export_group('Colors')
@export var default_color: Color
@export var choose_color: Color
@export var choose_hover_color: Color

var _last: Variant = null
var _bg_color = default_color

func load_snapshot(card: Variant):
	_last = card

func can_choose():
	return Controller.can_choose_in_play(_last.IPID)

func determine_bg_color():
	_bg_color = default_color
	if can_choose():
		_bg_color = choose_color
		return

func mouse_leave():
	set_bg_color(_bg_color)

func OnUpdate(update: Variant):
	super.OnUpdate(update)
	_bg_color = default_color
	determine_bg_color()
	set_bg_color(_bg_color)

func mouse_enter():
	super.mouse_enter()
	if can_choose():
		set_bg_color(choose_hover_color)
		return

func click():
	super.click()
	
	if can_choose():
		Controller.choose_in_play(_last.IPID)
		return
