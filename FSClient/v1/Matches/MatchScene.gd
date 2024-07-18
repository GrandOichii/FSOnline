extends Control
class_name MatchScene

@onready var Board = %Board

var _board_index = []

func _ready():
	for board in %Boards.get_children():
		_board_index.append(board)
		
func load_match_info(match_info: Variant):
	Board.load_match_info(match_info)
	
func load_snapshot(snapshot: Variant):
	Board.load_snapshot(snapshot)
	# TODO
	
func set_controller(controller: MatchController):
	Board.set_controller(controller)

# signal connections

func _on_option_button_item_selected(index):
	for i in len(_board_index):
		var board = _board_index[i]
		board.hide()
		if i == index:
			board.show()
