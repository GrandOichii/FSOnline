extends Resource
class_name ConfigValue

@export var name: String
@export var labeled: String

func append_control_nodes(parent: Control):
	var label = Label.new()
	label.text = labeled + ' '
	
	parent.add_child(label)
	
	append_main_control_node(parent)
	
func append_main_control_node(parent: Node):
	# TODO throw error
	pass
