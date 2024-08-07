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
	
func can_perform_action() -> bool:
	return last_update.Request == 'PromptAction'
	
func can_pass() -> bool:
	if last_update.Request != 'PromptAction':
		return false
	return 'pass' in last_update.Args.values()
	
func can_activate(in_play_card: Variant) -> bool:
	if last_update.Request != 'PromptAction':
		return false
	var values = last_update.Args.values()
	for v in values:
		if v.begins_with('a ' + str(in_play_card.IPID) + ' '):
			return true
	return false
	
func activate(in_play_card: Variant):
	# !FIXME only activates the first ability of card
	send('a ' + str(in_play_card.IPID) + ' 0')
	
func can_choose_player(player_idx: int) -> bool:
	if last_update.Request == 'ChoosePlayer':
		for v in last_update.Args.values():
			if v == player_idx:
				return true
		return false
		
	if last_update.Request == 'ChooseMonsterOrPlayer':
		return ('p-' + str(player_idx)) in last_update.Args.values()
		
	return false

func choose_player(player_idx: int):
	if last_update.Request == 'ChoosePlayer':
		send(str(player_idx))
		return
	if last_update.Request == 'ChooseMonsterOrPlayer':
		send('p-' + str(player_idx))
		return
	
func can_choose_stack_effect(sid: String) -> bool:
	if last_update.Request != 'ChooseStackEffect':
		return false
	return sid in last_update.Args.values()

func choose_stack_effect(sid: String):
	send(str(sid))

func can_choose_card_in_hand(hand_idx: int) -> bool:
	if last_update.Request != 'ChooseCardInHand':
		return false
	for v in last_update.Args.values():
		if v == hand_idx:
			return true
	return false
	
func choose_card_in_hand(hand_idx: int):
	send(str(hand_idx))

func can_choose_in_play(ipid: String) -> bool:
	# TODO sketchy
	if last_update.Request == 'ChooseItem':
		return ipid in last_update.Args.values()
	if last_update.Request == 'ChooseMonsterOrPlayer':
		return ('m-' + ipid) in last_update.Args.values()
	
	return false

func choose_in_play(ipid: String):
	if last_update.Request == 'ChooseItem':
		send(ipid)
		return
	if last_update.Request == 'ChooseMonsterOrPlayer':
		send('m-' + ipid)
		return
	
func can_purchase(idx: int):
	if last_update.Request != 'ChooseItemToPurchase':
		return false
	for v in last_update.Args.values():
		if v == idx:
			return true
	return false
	
func purchase(idx: int):
	send(str(idx))
	
func can_attack(idx: int):
	if last_update.Request != 'ChooseMonsterToAttack':
		return false
	for v in last_update.Args.values():
		if v == idx:
			return true
	return false
	
func attack(idx: int):
	send(str(idx))

func send(msg: String):
	Response.emit(msg)
