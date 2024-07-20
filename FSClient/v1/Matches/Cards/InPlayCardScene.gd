extends Control
class_name InPlayCardScene

@onready var Card = %Card
@onready var Counter = %Counter

var _last = null

func _ready():
	pass # Replace with function body.
	
func set_controller(controller: MatchController):
	Card.set_controller(controller)
	
func rotate_to(degrees: float):
	create_tween().tween_property(Card, 'rotation', degrees, .1)
	
func load_counters():
	Counter.text = ''
	if 'Generic' in _last.Counters.keys():
		Counter.text = str(_last.Counters['Generic'])

func load_snapshot(snapshot: Variant):
	_last = snapshot
	Card.load_snapshot(snapshot)
	load_counters()
	
	var rot = 0
	if snapshot.Tapped:
		rot = PI / 2
	rotate_to(rot)

func get_ipid():
	return _last.IPID
