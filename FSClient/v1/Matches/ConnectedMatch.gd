extends Control
class_name ConnectedMatch

signal UpdateReceived(Variant)
signal MatchInfoReceived(Variant)

@export var start_fullscreen: bool = true
@export var player_name: String = 'FSClient'
@export var character_key: String = 'guppy-v2'

@export_group('Connection')
@export var address = '127.0.0.1'
@export var port = 9090

@onready var Connection = %Connection
@onready var Controller = %Controller
@onready var Action = %Action
@onready var Request = %Request
@onready var Hint = %Hint
@onready var Options = %Options
@onready var Match = %Match

@onready var ChooseStringWindow = %ChooseStringWindow
@onready var ChooseStringText = %ChooseStringText
@onready var ChooseStringButtons = %ChooseStringButtons

var _update: Variant
var _match_info: Variant

var _auto_response = null

func _ready():
	ChooseStringWindow.hide()
	
	if start_fullscreen:
		DisplayServer.window_set_mode(DisplayServer.WINDOW_MODE_FULLSCREEN)
	
	print('connecting...')
	Connection.Connect(address, int(port))
	Match.set_controller(Controller)

func process_match_info(match_info: Variant):
	_match_info = match_info
	Controller.set_match_info(match_info)
	Match.load_match_info(match_info)
	print(match_info)

func process_update(update: Variant):
	Match.load_snapshot(update.Match)

	Controller.set_last_update(update)

	Hint.text = update.Hint
	Request.text = update.Request
	var text = ''
	for key in update.Args:
		text += key + ': ' + str(update.Args[key]) + '\n'
	Options.text = text
	
	if update.Request == 'ChooseString':
		setup_pick_string(update)
		return
	
func _input(e):
	if e.is_action_pressed('yield_until_empty_stack'):
		_auto_response = func(data):
			if len(data.Match.Stack.Effects) == 0:
				_auto_response = null
				return
			if Controller.can_perform_action():
				if not Controller.can_pass():
					_auto_response = null
					return
				send_pass()
		_auto_response.call(_update)
		return
	if e.is_action_pressed('yield_until_turn'):
		_auto_response = func(data):
			if Controller.can_perform_action():
				if not Controller.can_pass():
					_auto_response = null
					return
				if data.Match.CurPlayerIdx == _match_info.PlayerIdx:
					_auto_response = null
					return
				send_pass()
		if Controller.can_pass():
			send_pass()
		return
	if e.is_action_pressed('pass'):
		send_pass()
		
func send_pass():
	if Controller.can_pass():
		Connection.Write('pass')
	
func send_declare_purchase():
	Connection.Write('dp')

func setup_pick_string(update: Variant):
	ChooseStringText.text = update.Hint
	ChooseStringWindow.show()
	while (ChooseStringButtons.get_child_count() > 0):
		ChooseStringButtons.remove_child(ChooseStringButtons.get_child(0))
	for option in update.Args.values():
		var b = Button.new()
		ChooseStringButtons.add_child(b)
		b.text = option
		var action = func():
			Connection.Write(option)
			ChooseStringWindow.hide()
		b.pressed.connect(action)

# signal connections

func _on_connection_connected():
	Connection.Write(player_name)
	Connection.Write(character_key)

func _on_connection_message_received(message):
	var json = JSON.new()
	var error = json.parse(message)
	if error != OK:
		return
	var data = json.data
	if 'Request' in data:
		_update = data
		process_update(data)
		UpdateReceived.emit(data)
		if _auto_response != null:
			_auto_response.call(data)
		return
	process_match_info(data)
	MatchInfoReceived.emit(data)

func _on_send_button_pressed():
	Connection.Write(Action.text)
	Action.text = ''

func _on_controller_response(msg: String):
	Connection.Write(msg)

func _on_pass_button_pressed():
	send_pass()

func _on_purchase_button_pressed():
	send_declare_purchase()
