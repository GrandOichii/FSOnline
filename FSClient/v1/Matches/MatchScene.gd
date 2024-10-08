extends Control
class_name MatchScene

@export var stack_effect_scene: PackedScene

@onready var Board = %Board
@onready var Stack = %Stack

var _board_index = []
var _controller: MatchController

func _ready():
	for board in %Boards.get_children():
		_board_index.append(board)

func _input(e: InputEvent):
	if e.is_action_pressed('toggle_fullscreen'):
		_toggle_fullscreen()
		
func _toggle_fullscreen():
	var mode = DisplayServer.window_get_mode()
	var targetMode = DisplayServer.WINDOW_MODE_FULLSCREEN
	if mode == DisplayServer.WINDOW_MODE_FULLSCREEN:
		targetMode = DisplayServer.WINDOW_MODE_WINDOWED
	DisplayServer.window_set_mode(targetMode)
	
		
func load_match_info(match_info: Variant):
	Board.load_match_info(match_info)
	
func load_snapshot(snapshot: Variant):
	Board.load_snapshot(snapshot)
	load_stack(snapshot)
	
func load_stack(snapshot: Variant):
	var stack = snapshot.Stack
	var ssize = len(stack.Effects)
	while Stack.get_child_count() > ssize:
		Stack.remove_child(Stack.get_child(0))
	while Stack.get_child_count() < ssize:
		var child = stack_effect_scene.instantiate()
		Stack.add_child(child)
		
		var stack_effect = child as StackEffectScene
		stack_effect.set_controller(_controller)
		
	var e_idx = 0
	for stack_effect: StackEffectScene in Stack.get_children():
		stack_effect.load_snapshot(stack.Effects[e_idx])
		e_idx += 1
	
func set_controller(controller: MatchController):
	_controller = controller
	Board.set_controller(controller)
	
	for stack_effect: StackEffectScene in Stack.get_children():
		stack_effect.set_controller(controller)

# signal connections

func _on_option_button_item_selected(index):
	for i in len(_board_index):
		var board = _board_index[i]
		board.hide()
		if i == index:
			board.show()
