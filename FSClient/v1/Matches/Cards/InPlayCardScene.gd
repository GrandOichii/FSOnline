extends Control
class_name InPlayCardScene

@export var show_stats = true

@onready var Card = %Card
@onready var Stats = %Stats
@onready var Counter = %Counter

var _last = null

func _ready():
	adjust_minimum_size()
	
func adjust_minimum_size():
	#custom_minimum_size = Vector2(Card.custom_minimum_size.y, Card.custom_minimum_size.y) * Card.scale
	%Wrapper.custom_minimum_size = Vector2(Card.custom_minimum_size.y, Card.custom_minimum_size.y) * Card.scale
	
func set_controller(controller: MatchController):
	Card.set_controller(controller)
	
func rotate_to(degrees: float):
	create_tween().tween_property(Card, 'rotation', degrees, .1)
	
func load_counters():
	Counter.text = ''
	if 'Generic' in _last.Counters.keys():
		Counter.text = str(_last.Counters['Generic'])
		
func load_stats():
	Stats.hide()
	if _last.Stats == null: return
	
	Stats.show()
	Stats.load_data(_last.Stats)

func load_snapshot(snapshot: Variant):
	_last = snapshot
	Card.load_snapshot(snapshot)
	load_counters()
	load_stats()
	
	var rot = 0
	if snapshot.Tapped:
		rot = PI / 2
	rotate_to(rot)

func get_ipid():
	return _last.IPID
