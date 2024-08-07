extends Control
class_name StackEffectScene

@export_group('Colors')
@export var default_color: Color
@export var choose_color: Color
@export var choose_hover_color: Color

@onready var Type = %Type
@onready var SID = %SID
@onready var Details = %Details
@onready var Bg = %Bg

var _type_map = {
	'roll': 'Roll',
	'loot_play': 'Loot card',
	'ability_activation': 'Ability activation',
	'ability_trigger': 'Triggered ability',
	'purchase_declaration': 'Purchase declaration',
	'attack_declaration': 'Attack declaration',
	'damage': 'Damage',
	'player_death': 'Player death',
	'attack': 'Attack',
	'card_death': 'Monster Death',
	'reward': 'Reward',
	'event': 'Event',
}

var _type_action_map = {
	'roll': func (snapshot):
		Details.text = 'Rolled: [color=red]' + str(snapshot.Value) + '[/color]',
	'loot_play': func (snapshot):
		Details.text = snapshot.Card.Names[0] + '\n\n' + snapshot.Card.Text,
	'ability_activation': func (snapshot):
		Details.text = snapshot.EffectText,
	'ability_trigger': func (snapshot):
		Details.text = snapshot.EffectText,
	'damage': func(snapshot):
		Details.text = str(snapshot.Amount),
	'event': func(snapshot):
		# TODO not card text, effect text
		Details.text = snapshot.Card.Text
}

@onready var _bg_color = default_color

var _controller: MatchController
var _sid: String

func load_snapshot(snapshot: Variant):
	_sid = snapshot.SID
	Type.text = snapshot.Type
	set_type(snapshot.Type)
	set_sid(snapshot.SID)
	Details.text = ''
	if snapshot.Type in _type_action_map:
		_type_action_map[snapshot.Type].call(snapshot)
	Details.text += '\n'
	for target in snapshot.Targets:
		if target.Type == 0:
			Details.text += '[Player] '
		if target.Type == 1:
			Details.text += '[Stack effect] '
		if target.Type == 2:
			Details.text += '[Card] '
		Details.text += target.Value

	
func set_type(type: String):
	Type.text = _type_map[type]
	
func set_sid(sid: String):
	SID.text = '[' + sid + ']'

func set_controller(controller: MatchController):
	_controller = controller
	_controller.Update.connect(on_update)
	
func can_choose():
	return _controller.can_choose_stack_effect(_sid)
	
func set_bg_color(color: Color):
	Bg.get('theme_override_styles/panel').bg_color = color
	
func interact():
	if _controller == null: return
	if _controller.last_update == null:
		return
	if can_choose():
		_controller.choose_stack_effect(_sid)
		return

# signal connections

func on_update(update: Variant):
	_bg_color = default_color
	if can_choose():
		_bg_color = choose_color
	set_bg_color(_bg_color)

func _on_gui_input(e):
	if e.is_action_pressed('interact'):
		interact()


func _on_mouse_entered():
	if _controller == null: return
	if _controller.last_update == null:
		return
	if can_choose():
		set_bg_color(choose_hover_color)
		return

func _on_mouse_exited():
	if _controller == null: return
	if _controller.last_update == null:
		return
	set_bg_color(_bg_color)
