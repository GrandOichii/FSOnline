extends Node
class_name MatchController

signal Update(update: Variant)
signal Response(msg: String)

var match_info: Variant = null
var last_update: Variant = null

#func _input(e):
	#if e.is_action_pressed('fight'):
		## TODO add checks
		#send('f')

func set_last_update(update: Variant):
	last_update = update
	Update.emit(update)

func set_match_info(info: Variant):
	match_info = info

func can_play(card: Variant) -> bool:
	if last_update.Request != 'PromptAction':
		return false
	return ('p ' + card.ID) in last_update.Args.values()

func play(card: Variant):
	send('p ' + card.ID)
	
#func can_activate(in_play_card: Variant) -> bool:
	#if last_update.Request != 'PromptAction':
		#return false
	#var values = last_update.Args.values()
	#for v in values:
		#if v.begins_with('a ' + str(in_play_card.ID) + ' '):
			#return true
	#return false
	#
#func activate(in_play_card: Variant):
	## !FIXME only activates the first ability of card
	#send('a ' + str(in_play_card.ID) + ' 0')
	
#func can_pick_card_in_hand(hand_idx: int) -> bool:
	#if last_update.Request != 'PickCardInHand':
		#return false
	#return str(hand_idx) in last_update.Args.values()
	#
#func pick_card_in_hand(hand_idx: int):
	#send(str(hand_idx))

func send(msg: String):
	Response.emit(msg)
