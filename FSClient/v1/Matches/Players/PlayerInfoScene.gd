extends Control
class_name PlayerInfoScene

@export var current_player_color: Color

@onready var Bg: PanelContainer = %Bg
@onready var PlayerName = %PlayerName
@onready var Coins = %Coins
@onready var LootPlays = %LootPlays
@onready var AttackOpportunities = %AttackOpportunities
@onready var PurchaseOpportunities = %PurchaseOpportunities
@onready var Health = %Health
@onready var Attack = %Attack

@onready var _default_bg_color: Color = Bg.get('theme_override_styles/panel').bg_color

# Called when the node enters the scene tree for the first time.
func _ready():
	pass # Replace with function body.

func set_player_name(new_name: String):
	PlayerName.text = new_name
	
func set_coins(coins: int):
	Coins.text = str(coins) + '$'
	
func set_loot_plays(loot_plays: int):
	LootPlays.text = str(loot_plays) + ' LP'
	
func set_attack_opportunities(ao: int):
	AttackOpportunities.text = str(ao) + ' Ao'
	
func set_purchase_opportunities(po: int):
	PurchaseOpportunities.text = str(po) + ' Ao'

func set_health(health: int):
	Health.text = str(health)

func set_attack(attack: int):
	Attack.text = str(attack)
	
func load_snapshot(snapshot: Variant, player_idx: int):
	var player = snapshot.Players[player_idx]
	set_player_name(player.Name + ' [' + str(player_idx) + ']')
	set_coins(player.Coins)
	set_loot_plays(player.LootPlays)
	
	set_bg_color(_default_bg_color)
	if snapshot.CurPlayerIdx == player_idx:
		set_bg_color(current_player_color)
	
func set_bg_color(color: Color):
	Bg.get('theme_override_styles/panel').bg_color = color
