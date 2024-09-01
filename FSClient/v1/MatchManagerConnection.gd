extends Control

@export var default_character: String = 'isaac-b2'

@onready var ConnectPanel = %ConnectPanel
@onready var Match = %Match
@onready var Config = %Config
@onready var Address = %Address
@onready var MatchId = %MatchId
@onready var PlayerName = %PlayerName
@onready var Character = %Character
@onready var Password = %Password

func _ready():
	%StartMatchButton.hide()
	%Character.text = default_character
	Match.hide()
	Match.process_mode = Node.PROCESS_MODE_DISABLED
	_on_create_check_toggled(%CreateCheck.button_pressed)

func _on_start_button_pressed():
	# TODO add connect
	
	Match.player_name = PlayerName.text
	Match.character_key = Character.text
	Match.ws_password = Password.text
	Match.create_params = build_create_params()
	Match.process_mode = Node.PROCESS_MODE_INHERIT
	
	if %MatchId.text == '':
		# create
		Match.Connection.Create(Address.text)
		%StartMatchButton.show()
	else:
		# connect
		Match.Connection.Connect(Address.text, %MatchId.text)
	
func build_create_params() -> String:
	var result = {}
	
	result['Config'] = Config.build()
	result['Password'] = Password.text
	
	var bots = []
	for i in %BotCount.value:
		bots.append({
			'Type': 0,
			'Name': 'Bot' + str(i)
		})
	result['Bots'] = bots

	print(str(result))
	
	return str(result)

func _on_config_button_pressed():
	Config.visible = !Config.visible

func _on_match_match_info_received(Variant):
	ConnectPanel.hide()
	Match.show()

func _on_start_match_button_pressed():
	Match.Connection.Write('start')

func _on_ws_connection_message_received(message: String):
	if message.begins_with('id:'):
		var id = message.substr(3)
		%StartMatchButton.text = 'Start match ' + id
		DisplayServer.clipboard_set(id)

func _on_create_check_toggled(toggled_on):
	%StartButton.text = 'Connect'
	if toggled_on:
		%StartButton.text = 'Create'
	
	var nodes = get_tree().get_nodes_in_group('connect_control')
	for node: Control in nodes:
		node.visible = not toggled_on
	nodes = get_tree().get_nodes_in_group('create_control')
	for node: Control in nodes:
		node.visible = toggled_on

func _on_copy_config_button_pressed():
	var c = Config.build()
	var text = str(c)
	DisplayServer.clipboard_set(text)
