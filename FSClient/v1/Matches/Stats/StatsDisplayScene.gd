extends Control
class_name StatsDisplayScene

func _ready():
	pass # Replace with function body.

func load_data(data: Variant):
	%Health.text = str(data.Health)
	%Attack.text = str(data.Attack)
	%Preventable.visible = data.PreventableDamage > 0
	%Preventable.text = '+' + str(data.PreventableDamage)

	%EvasionContainer.hide()
	if data.Evasion == null or data.Evasion < 0: return
	
	%EvasionContainer.show()
	%Evasion.text = str(data.Evasion)
