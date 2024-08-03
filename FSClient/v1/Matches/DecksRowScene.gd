extends Control
class_name DecksRowScene

@export var treasure_slot_scene: PackedScene
@export var monster_slot_scene: PackedScene
@export var room_slot_scene: PackedScene

@onready var MonsterSlots = %MonsterSlots
@onready var TreasureSlots = %TreasureSlots
@onready var RoomSlots = %RoomSlots

var _controller: MatchController

func _ready():
	while TreasureSlots.get_child_count() > 0:
		TreasureSlots.remove_child(TreasureSlots.get_child(0))
	while RoomSlots.get_child_count() > 0:
		RoomSlots.remove_child(RoomSlots.get_child(0))
	while MonsterSlots.get_child_count() > 0:
		MonsterSlots.remove_child(MonsterSlots.get_child(0))

func load_snapshot(snapshot: Variant):
	load_shop_items(snapshot)
	load_rooms(snapshot)
	load_monsters(snapshot)
	
func load_rooms(snapshot: Variant):
	var count = len(snapshot.RoomSlots)
	while RoomSlots.get_child_count() > count:
		RoomSlots.remove_child(RoomSlots.get_child(0))
	while RoomSlots.get_child_count() < count:
		var child = room_slot_scene.instantiate()
		RoomSlots.add_child(child)
		
		var slot = child as RoomSlotScene
		slot.set_controller(_controller)
		
	var ri = 0
	for slot: RoomSlotScene in RoomSlots.get_children():
		slot.load_snapshot(snapshot.RoomSlots[ri])
		ri += 1

func load_shop_items(snapshot: Variant):
	var count = len(snapshot.ShopSlots)
	while TreasureSlots.get_child_count() > count:
		TreasureSlots.remove_child(TreasureSlots.get_child(0))
	while TreasureSlots.get_child_count() < count:
		var child = treasure_slot_scene.instantiate()
		TreasureSlots.add_child(child)
		
		var slot = child as TreasureSlotScene
		slot.set_controller(_controller)
		
	var si = 0
	for slot: TreasureSlotScene in TreasureSlots.get_children():
		slot.load_snapshot(snapshot.ShopSlots[si])
		si += 1
		

func load_monsters(snapshot: Variant):
	var count = len(snapshot.MonsterSlots)
	while MonsterSlots.get_child_count() > count:
		MonsterSlots.remove_child(MonsterSlots.get_child(0))
	while MonsterSlots.get_child_count() < count:
		var child = monster_slot_scene.instantiate()
		MonsterSlots.add_child(child)
		
		var slot = child as MonsterSlotScene
		slot.set_controller(_controller)
		
	var mi = 0
	for slot: MonsterSlotScene in MonsterSlots.get_children():
		slot.load_snapshot(snapshot.MonsterSlots[mi])
		mi += 1

func set_controller(controller: MatchController):
	_controller = controller
	
	# treasures
	for slot: TreasureSlotScene in TreasureSlots.get_children():
		slot.set_controller(controller)
