extends Control
class_name DecksRowScene

@export var treasure_slot_scene: PackedScene

@onready var TreasureSlots = %TreasureSlots

var _controller: MatchController

func _ready():
	while TreasureSlots.get_child_count() > 0:
		TreasureSlots.remove_child(TreasureSlots.get_child(0))

func load_snapshot(snapshot: Variant):
	load_shop_items(snapshot)

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

func set_controller(controller: MatchController):
	_controller = controller
	
	# treasures
	for slot: TreasureSlotScene in TreasureSlots.get_children():
		slot.set_controller(controller)
