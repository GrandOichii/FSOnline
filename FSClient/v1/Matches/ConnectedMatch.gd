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

var _update: Variant

func _ready():
	if start_fullscreen:
		DisplayServer.window_set_mode(DisplayServer.WINDOW_MODE_FULLSCREEN)
	
	print('connecting...')
	Connection.Connect(address, int(port))

func process_match_info(match_info: Variant):
	Controller.set_match_info(match_info)
	Match.load_match_info(match_info)
	print(match_info)

func process_update(update: Variant):
	#Controller.set_last_update(update)
#
	Hint.text = update.Hint
	Request.text = update.Request
	var text = ''
	for key in update.Args:
		text += key + ': ' + update.Args[key] + '\n'
	Options.text = text
	#
	#if update.Request == 'PromptLandscapePlacement':
		#var landscapes = []
		#for key in update.Args:
			#for i in int(update.Args[key]):
				#landscapes += [key]
		#Connection.Write('|'.join(landscapes))
		#return
	#if update.Request == 'PickOption':
		#setup_pick_string(update)
		#return
	#
	#update_hand(update)

# signal connections

func _on_connection_connected():
	Connection.Write(player_name)
	Connection.Write(character_key)

func _on_connection_message_received(message):
	var json = JSON.new()
	var error = json.parse(message)
	if error != OK:
		print("JSON Parse Error: ", json.get_error_message(), " in ", message, " at line ", json.get_error_line())
		return
	var data = json.data
	if 'Request' in data:
		_update = data
		process_update(data)
		UpdateReceived.emit(data)
		return
	process_match_info(data)
	MatchInfoReceived.emit(data)

func _on_send_button_pressed():
	Connection.Write(Action.text)
	Action.text = ''
