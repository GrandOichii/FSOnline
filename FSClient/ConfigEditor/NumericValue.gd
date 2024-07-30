extends ConfigValue
class_name NumericValue

@export var default_value: int
@export var min: int = 0
@export var max: int = 100

func append_main_control_node(parent: Node):
	var spin = SpinBox.new()
	spin.value = default_value
	spin.max_value = max
	spin.min_value = min
	spin.set_meta('value_name', name)
	
	parent.add_child(spin)
