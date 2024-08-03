extends InPlayCardBehavior
class_name MonsterBehavior

@export var can_attack_color: Color
@export var can_attack_hover_color: Color

var _idx: int = -1

func set_slot_idx(idx: int):
	_idx = idx

func can_attack():
	if Controller == null:
		return false
	if _last == null:
		return false
	return Controller.can_attack(_idx)

func determine_bg_color():
	super.determine_bg_color()
	if can_attack():
		_bg_color = can_attack_color
		return

func mouse_enter():
	super.mouse_enter()
	if can_attack():
		set_bg_color(can_attack_hover_color)
		return

func click():
	super.click()
	
	if can_attack():
		Controller.attack(_idx)
		return
