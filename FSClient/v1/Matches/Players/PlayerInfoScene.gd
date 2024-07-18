extends Control
class_name PlayerInfoScene

@onready var Bg = %Bg
@onready var PlayerName = %PlayerName
@onready var Coins = %Coins
@onready var LootPlays = %LootPlays
@onready var AttackOpportunities = %AttackOpportunities
@onready var PurchaseOpportunities = %PurchaseOpportunities
@onready var Health = %Health
@onready var Attack = %Attack

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
