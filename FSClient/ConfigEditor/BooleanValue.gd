extends ConfigValue
class_name BooleanValue

@export var default_value: bool

func append_main_control_node(parent: Node):
	var check = CheckBox.new()
	check.button_pressed = default_value
	check.set_meta('value_name', name)
	
	parent.add_child(check)
