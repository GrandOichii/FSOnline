extends InPlayCardBehavior
class_name ShotItemBehavior

@export_group('Colors')
@export var purchase_color: Color
@export var purchase_hover_color: Color

var _idx: int

func set_shop_idx(idx: int):
	_idx = idx

func can_purchase():
	if Controller == null:
		return false
	if _last == null:
		return false
	return Controller.can_purchase(_idx)

func determine_bg_color():
	super.determine_bg_color()
	if can_purchase():
		_bg_color = purchase_color
		return

func mouse_enter():
	super.mouse_enter()
	if can_purchase():
		set_bg_color(purchase_hover_color)
		return

func click():
	super.click()
	
	if can_purchase():
		Controller.purchase(_idx)
		return
