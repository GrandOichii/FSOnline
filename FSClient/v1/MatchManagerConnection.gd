extends Control

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
	Match.hide()
	Match.process_mode = Node.PROCESS_MODE_DISABLED

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
	# TODO
	result['Bots'] = [
		{
			'Type': 0,
			'Name': 'Bot1'
		}
	]
	
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
