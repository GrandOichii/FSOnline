extends CardBehavior
class_name HandCardBehavior

@export_group('Colors')
@export var DefaultColor: Color
@export var PlayableColor: Color
@export var HoverColor: Color

# TODO utilize
@export var PickColor: Color
@export var PickHoverColor: Color

var _bg_color: Color = DefaultColor
var _last: Variant
var _hand_idx: int
var _visible: bool

func set_hand_idx(hand_idx: int):
	_hand_idx = hand_idx

func set_visible(visible: bool):
	_visible = visible

func load_snapshot(card: Variant):
	_last = card

func can_play() -> bool:
	if not _visible:
		return false
	return Controller.can_play(_last)
	
#func can_pick() -> bool:
	#return Controller.can_pick_card_in_hand(_hand_idx)
	
func mouse_enter():
	if can_play():
		set_bg_color(HoverColor)
		return
	#if can_pick():
		#set_bg_color(DiscardHoverColor)
		#return
	
func mouse_leave():
	set_bg_color(_bg_color)
	
func click():
	if can_play():
		Controller.play(_last)
		return
	#if can_pick():
		#Controller.pick_card_in_hand(_hand_idx)

func OnUpdate(update: Variant):
	super.OnUpdate(update)
	_bg_color = DefaultColor
	if can_play():
		_bg_color = PlayableColor
	#if can_pick():
		#_bg_color = DiscardColor
	set_bg_color(_bg_color)
