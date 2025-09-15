extends Control
class_name PlayerInfoScene


@export_group('Colors')
@export var default_color: Color
@export var choose_color: Color
@export var choose_hover_color: Color

@onready var Bg: PanelContainer = %Bg
@onready var PlayerName = %PlayerName
@onready var Coins = %Coins
@onready var LootPlays = %LootPlays
@onready var AttackOpportunities = %AttackOpportunities
@onready var PurchaseOpportunities = %PurchaseOpportunities
@onready var Health = %Health
@onready var Attack = %Attack
@onready var Souls = %Souls

@onready var _bg_color: Color = default_color

var _controller: MatchController
var _player_idx: int

func set_controller(controller: MatchController):
	_controller = controller
	_controller.Update.connect(on_update)

func set_player_name(new_name: String):
	PlayerName.text = new_name
	
func set_coins(coins: int):
	Coins.text = str(coins) + '$'
	
func set_loot_plays(loot_plays: int):
	LootPlays.text = str(loot_plays) + ' LP'
	
func set_attack_opportunities(ao: int):
	AttackOpportunities.text = str(ao) + ' Ao'
	
func set_purchase_opportunities(po: int):
	PurchaseOpportunities.text = str(po) + ' Po'

func set_soul_count(soul_count: int):
	Souls.text = str(soul_count)
	
	
func load_snapshot(snapshot: Variant, player_idx: int):
	var player = snapshot.Players[player_idx]
	_player_idx = player.Idx
	set_player_name(player.Name + ' [' + str(player_idx) + ']')
	set_coins(player.Coins)
	set_loot_plays(player.LootPlays)
	set_purchase_opportunities(player.PurchaseOpportunities)
	set_attack_opportunities(player.AttackOpportunities)
	set_soul_count(player.SoulCount)

	# Stats
	%Stats.load_data(player.Stats)
	
func set_bg_color(color: Color):
	Bg.get('theme_override_styles/panel').bg_color = color

func can_choose():
	return _controller.can_choose_player(_player_idx)

func interact():
	if _controller == null: return
	if _controller.last_update == null:
		return
	if can_choose():
		_controller.choose_player(_player_idx)
		return

# signal connections

func on_update(update: Variant):
	_bg_color = default_color
	if can_choose():
		_bg_color = choose_color
	set_bg_color(_bg_color)

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

func _on_gui_input(e):
	if e.is_action_pressed('interact'):
		interact()
