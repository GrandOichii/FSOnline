extends Control
class_name PlayerScene

@export var item_scene: PackedScene

@export_group('Hand')
@export var hand_card_scene: PackedScene

@onready var Info = %Info
@onready var Hand = %Hand
@onready var Items = %Items
@onready var Character = %Character
@onready var Bg = %Bg

var player_idx: int
var _controller: MatchController

func _ready():
	# clear hand
	while Hand.get_child_count() > 0:
		Hand.remove_child(Hand.get_child(0))
		
	# clear items
	while Items.get_child_count() > 0:
		Items.remove_child(Items.get_child(0))
		
func set_controller(controller: MatchController):
	# info
	Info.set_controller(controller)
	
	# character
	Character.set_controller(controller)
	
	# hand
	_controller = controller
	for card: HandCardScene in Hand.get_children():
		card.set_controller(controller)
		
	# items
	for item: InPlayCardScene in Items.get_children():
		item.set_controller(controller)

func set_player_idx(idx: int):
	player_idx = idx

func load_snapshot(snapshot: Variant):
	Info.load_snapshot(snapshot, player_idx)
	var color: Color = Bg.get('theme_override_styles/panel').bg_color
	color.a = 0
	if player_idx == snapshot.CurPlayerIdx:
		color.a = .1
	Bg.get('theme_override_styles/panel').bg_color = color
	load_hand(snapshot)

	var player = snapshot.Players[player_idx]
	Character.load_snapshot(player.Character)

	load_items(snapshot)

func load_items(snapshot: Variant):
	var items = snapshot.Players[player_idx].Items
	var ipids = []
	var index = {}
	for item in items: 
		ipids.append(item.IPID)
		index[item.IPID] = item

	for item in Items.get_children():
		var ipid = item.get_ipid()
		if not ipid in ipids:
			item.queue_free()
			continue
		item.load_snapshot(index[ipid])
		(ipids as Array).erase(ipid)

	for ipid in ipids:
		var child = item_scene.instantiate()
		Items.add_child(child)

		var item = child as InPlayCardScene
		item.load_snapshot(index[ipid])
		item.set_controller(_controller)
	
func load_hand(snapshot: Variant):
	var count = snapshot.Players[player_idx].HandSize
	while Hand.get_child_count() > count:
		Hand.remove_child(Hand.get_child(0))
		
	while Hand.get_child_count() < count:
		var child = hand_card_scene.instantiate()
		Hand.add_child(child)
		
		var hand_card = child as HandCardScene
		hand_card.set_controller(_controller)
		hand_card.set_player_idx(player_idx)
		
	var hi = 0
	for hand_card in Hand.get_children():
		hand_card.set_hand_idx(hi)
		hi += 1
		hand_card.load_snapshot(snapshot)
