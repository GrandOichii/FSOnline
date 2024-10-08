-- core object
FS = {}

FS.DeckIDs = {
    LOOT = 0,
    TREASURE = 1,
    MONSTER = 2,
    ROOM = 3
}

FS.TargetTypes = {
    PLAYER = 0,
    STACK_EFFECT = 1,
    IN_PLAY_CARD = 2,
}

-- card labels
FS.Labels = {
    Eternal = 'Eternal',
    Trinket = 'Trinket',
    Guppys = 'Guppy\'s'
}

-- modification layers
FS.ModLayers = {
    COIN_GAIN_AMOUNT = 1,
    ROLL_REPLACEMENT_EFFECTS = 2,
    LOOT_AMOUNT = 3,
    HAND_CARD_VISIBILITY = 4,
    LOOT_PLAY_RESTRICTIONS = 5,
    ITEM_ACTIVATION_RESTRICTIONS = 6,
    PURCHASE_COST = 7,
    LAST = 8,
    ITEM_DESTRUCTION_REPLACEMENT_EFFECTS = 9,
    MOD_MAX_LOOT_PLAYS = 10,
    PLAYER_MAX_HEALTH = 11,
    PLAYER_ATTACK = 12,
    DEATH_PENALTY_MODIFIERS = 13,
    DEATH_PENALTY_REPLACEMENT_EFFECTS = 14,
    MONSTER_HEALTH = 15,
    MONSTER_EVASION = 16,
    MONSTER_ATTACK = 17,
    DAMAGE_RECEIVED_MODIFICATORS = 18,
    ROLL_RESULT_MODIFIERS = 19,
    PLAYER_SOUL_COUNT = 20,
}

-- triggers
FS.Triggers = {
    ROLL = 'roll',
    ITEM_ACTIVATION = 'item_activation',
    ITEM_ENTER = 'item_enter',
    SOUL_ENTER = 'soul_enter',
    PURCHASE = 'purchase',
    PLAYER_DEATH_BEFORE_PENALTIES = 'player_death_before_penalties',
    PLAYER_DEATH = 'player_death',
    PLAYED_DAMAGED = 'player_damaged',
    CARD_DAMAGED = 'card_damaged',
    CARD_DEATH = 'card_death',
}

-- common
FS.C = {}

function FS.C.GiveLootCards(toIdx, fromIdx, amount)
    for i = 1, amount do
        local player_idx = tonumber(fromIdx)
        local hand = GetPlayer(player_idx).Hand
        local indicies = {}
        for hi = 0, hand.Count - 1 do
            indicies[#indicies+1] = hi
        end
        if #indicies == 0 then
            return false
        end
        local choice = ChooseCardInHand(player_idx, indicies, 'Choose a card to give to '..GetPlayer(toIdx).Name)
        local card = GetPlayer(player_idx).Hand[choice].Card
        RemoveFromHand(player_idx, choice)
        AddToHand(toIdx, card)
    end
end

function FS.C.IPIDs(items)
    local result = {}
    for _, item in ipairs(items) do
        result[#result+1] = item.IPID
    end
    return result
end

function FS.C.PlayerIndicies(players)
    local result = {}
    for _, player in ipairs(players) do
        result[#result+1] = player.Idx
    end
    return result
end

-- common effects
FS.C.Effect = {}

function FS.C.Effect._ApplyToPlayer(effect, filterFunc)
    filterFunc = filterFunc or function (stackEffect, args)
        return FS.F.Players():Idx(stackEffect.OwnerIdx):Do()
    end
    return function (stackEffect, args)
        -- TODO? just stops mid resolving?
        local players = filterFunc(stackEffect, args)
        for _, player in ipairs(players) do
            if not effect(player, stackEffect) then
                return false
            end
        end
        return true
    end
end

function FS.C.Effect.SwitchRoll(rollIdx, index)
    return function (stackEffect)
        assert(stackEffect.Rolls.Count > rollIdx, 'Tried to get roll idx ' .. rollIdx .. ' from stack effect ' .. tostring(stackEffect) .. ', which only has ' .. tostring(stackEffect.Rolls.Count) .. ' rolls')
        local roll = stackEffect.Rolls[rollIdx]
        local action = index[roll]
        -- assert(action ~= nil, 'Didn\'t provide scenario for roll value '..roll..' for FS.C.Effect.SwitchRoll')
        if action == nil then
            return false
        end
        action(stackEffect)
        return true
    end
end

function FS.C.Effect.ExpandShotSlots(amount)
    return function (stackEffect)
        ExpandShotSlots(amount)
        return true
    end
end

function FS.C.Effect.ExpandMonsterSlots(amount)
    return function (stackEffect)
        ExpandMonsterSlots(amount)
        return true
    end
end

function FS.C.Effect.PutGenericCountersOnMe(amount)
    return function (stackEffect)
        assert(IsAbilityStackEffect(stackEffect), 'Provided a non-ability-activation stack effect for FS.C.Effect.PutGenericCountersOnMe')

        local card = stackEffect.Card
        PutGenericCounters(card.IPID, amount)
        return true
    end
end

function FS.C.Effect.GainCoins(amount, filterFunc)
    return FS.C.Effect._ApplyToPlayer(function (player, stackEffect)
        AddCoins(player.Idx, amount)
        return true
    end, filterFunc)
end

function FS.C.Effect.PreventNextDamageToPlayer(amount, filterFunc)
    return FS.C.Effect._ApplyToPlayer(function (player, stackEffect)
        AddGenericDamagePreventors(player.Idx, amount)
        return true
    end, filterFunc)
end

function FS.C.Effect.PreventNextDamageToTarget(target_idx, amount)
    return function (stackEffect)
        local target = stackEffect.Targets[target_idx]
        local type = TargetTypeToInt(target.Type)
        if type == FS.TargetTypes.PLAYER then
            AddGenericDamagePreventors(tonumber(target.Value), amount)
            return true
        end

        if type == FS.TargetTypes.IN_PLAY_CARD then
        AddGenericDamagePreventorsToMonster(target.Value, amount)
            return true
        end

        error('Invalid damage prevention target: '..tostring(type))
    end
end

function FS.C.Effect.LoseCoins(amount, filterFunc)
    return FS.C.Effect._ApplyToPlayer(function (player, stackEffect)
        LoseCoins(player.Idx, amount)
        return true
    end, filterFunc)
end

function FS.C.Effect.AddLootPlay(amount, filterFunc)
    return FS.C.Effect._ApplyToPlayer(function (player, stackEffect)
        TillEndOfTurn(
            FS.ModLayers.MOD_MAX_LOOT_PLAYS,
            function ()
                player.State.LootPlaysForTurn = player.State.LootPlaysForTurn + amount
            end
        )
        return true
    end, filterFunc)
end

function FS.C.Effect.Wheel(lootAfter, filterFunc)
    return FS.C.Effect._ApplyToPlayer(function (player, stackEffect)
        while player.Hand.Count > 0 do
            DiscardFromHand(player.Idx, 0)
        end
        LootCards(player.Idx, lootAfter, stackEffect)
        return true
    end, filterFunc)
end

function FS.C.Effect.DiscardMeFromPlay()
    return function (stackEffect)
        return TryDiscardFromPlay(stackEffect.Card.IPID)
    end
end

function FS.C.Effect.RerollTargetRoll(target_idx)
    return function (stackEffect)
        local effect = GetStackEffect(stackEffect.Targets[target_idx].Value)

        assert(IsRollStackEffect(effect), 'Provided a non-roll target stack effect for FS.C.Effect.ModifyTargetRoll')

        RerollDice(effect)
        return true
    end
end

function FS.C.Effect.KillTargetPlayer(target_idx)
    return function (stackEffect)
        local idx = tonumber(stackEffect.Targets[target_idx].Value)
        KillPlayer(idx, stackEffect)
        return true
    end
end

function FS.C.Effect.TargetPlayerDiscards(target_idx, amount, hint)
    hint = hint or 'Choose a card to discard'
    return function (stackEffect)
        local idx = tonumber(stackEffect.Targets[target_idx].Value)
        for di = 0, amount - 1 do
            local hand = GetPlayer(idx).Hand
            local indicies = {}
            for i = 0, hand.Count - 1 do
                indicies[#indicies+1] = i
            end
            if #indicies == 0 then
                return false
            end
            local choice = ChooseCardInHand(idx, indicies, hint)
            DiscardFromHand(idx, choice)
            SoftReloadState()
        end
        return true
    end
end

function FS.C.Effect.RechargeItemsOfTargetPlayer(target_idx)
    return function (stackEffect)
        local idx = tonumber(stackEffect.Targets[target_idx].Value)
        local items = FS.F.Items()
            :ControlledBy(idx)
            :Rechargeable()
            :Do()

        for _, item in ipairs(items) do
            Recharge(item.IPID)
        end
        return true
    end
end

function FS.C.Effect.KillTargetMonster(target_idx)
    return function (stackEffect)
        local ipid = stackEffect.Targets[target_idx].Value
        KillMonster(ipid, stackEffect)
        return true
    end
end

function FS.C.Effect.PreventNextDamageToTargetPlayer(target_idx, amount)
    return function (stackEffect)
        local idx = tonumber(stackEffect.Targets[target_idx].Value)
        AddGenericDamagePreventors(idx, amount)
        return true
    end
end

function FS.C.Effect.CancelTargetStackEffect(target_idx)
    return function (stackEffect)
        local sid = stackEffect.Targets[target_idx].Value
        CancelEffect(sid)
        return true
    end
end

function FS.C.Effect.DamageToTarget(target_idx, amount)
    return function (stackEffect)
        local target = stackEffect.Targets[target_idx]
        local type = TargetTypeToInt(target.Type)
        if type == FS.TargetTypes.PLAYER then
            return FS.C.Effect.DamageToTargetPlayer(target_idx, amount)(stackEffect)
        end
        
        if type == FS.TargetTypes.IN_PLAY_CARD then
            return FS.C.Effect.DamageToTargetMonster(target_idx, amount)(stackEffect)
        end

        error('Invalid damage target: '..tostring(type))
    end
end

function FS.C.Effect.KillTarget(target_idx)
    return function (stackEffect)
        local target = stackEffect.Targets[target_idx]
        local type = TargetTypeToInt(target.Type)
        if type == FS.TargetTypes.PLAYER then
            return FS.C.Effect.KillTargetPlayer(target_idx)
        end
        
        if type == FS.TargetTypes.IN_PLAY_CARD then
            return FS.C.Effect.KillTargetMonster(target_idx)
        end

        error('Invalid damage target: '..tostring(type))
    end
end

function FS.C.Effect.DamageToTargetPlayer(target_idx, amount)
    return function (stackEffect)
        local toIdx = tonumber(stackEffect.Targets[target_idx].Value)
        DealDamageToPlayer(toIdx, amount, stackEffect)
        return true
    end
end

function FS.C.Effect.GiveMeToTargetPlayer(target_idx, amount)
    return function (stackEffect)
        local idx = tonumber(stackEffect.Targets[target_idx].Value)
        StealItem(idx, stackEffect.Card.IPID)
        return true
    end
end

function FS.C.Effect.DamageToTargetMonster(target_idx, amount)
    return function (stackEffect)
        local ipid = stackEffect.Targets[target_idx].Value
        DealDamageToCard(ipid, amount, stackEffect)
        return true
    end
end

function FS.C.Effect.SetTargetRoll(target_idx, value)
    return function (stackEffect)
        local effect = GetStackEffect(stackEffect.Targets[target_idx].Value)

        assert(IsRollStackEffect(effect), 'Provided a non-roll target stack effect for FS.C.Effect.ModifyTargetRoll')

        SetRollValue(effect, value)
        return true
    end
end

function FS.C.Effect.RechargeTarget(target_idx, optional)
    return function (stackEffect)
        local ipid = stackEffect.Targets[target_idx].Value
        optional = optional or false
        if optional then
            local accept = FS.C.Choose.YesNo(stackEffect.OwnerIdx, 'Recharge '..GetInPlayCard(ipid).LogName..'?')
            if not accept then
                -- TODO? return true
                return false
            end
        end

        Recharge(ipid)
        return true
    end
end

function FS.C.Effect.DeactivateTarget(target_idx, optional)
    optional = optional or false
    return function (stackEffect)
        local ipid = stackEffect.Targets[target_idx].Value

        if optional then
            local item = GetInPlayCard(ipid)
            local accept = FS.C.Choose.YesNo(stackEffect.OwnerIdx, 'Deactivate '..item.LogName..'?')
            if not accept then
                return false
            end
        end

        return TapCard(ipid)
    end
end

function FS.C.Effect.RechargeMe(optional)
    optional = optional or false
    return function (stackEffect)
        assert(IsAbilityActivation(stackEffect) or IsTrigger(stackEffect), 'Provided a non-ability stack effect for FS.C.Effect.RechargeMe')

        local card = stackEffect.Card
        if optional then
            local accept = FS.C.Choose.YesNo(stackEffect.OwnerIdx, 'Recharge '..card.LogName..'?')
            if not accept then
                return true
                -- TODO? return false
            end
        end
        Recharge(card.IPID)
        return true
    end
end

function FS.C.Effect.RechargeAllItems()
    return function (stackEffect)
        local items = FS.F.Items()
            :ControlledBy(stackEffect.OwnerIdx)
            :Rechargeable()
            :Do()
        for _, item in ipairs(items) do
            Recharge(item.IPID)
        end
        return true
    end
end

function FS.C.Effect.RerollTargetItem(target_idx)
    return function (stackEffect)
        local ipid = stackEffect.Targets[target_idx].Value
        RerollItem(ipid)
        return true
    end
end

function FS.C.Effect.StealTargetItem(target_idx)
    return function (stackEffect)
        local ipid = stackEffect.Targets[target_idx].Value

        StealItem(stackEffect.OwnerIdx, ipid)
        return true
    end
end

function FS.C.Effect.StealCoinsFromTarget(target_idx, amount)
    return function (stackEffect)
        local idx = tonumber(stackEffect.Targets[target_idx].Value)

        StealCoins(stackEffect.OwnerIdx, idx, amount)
        return true
    end
end

function FS.C.Effect.ModifyTargetRoll(target_idx, options, hint)
    hint = hint or 'Choose what to do with roll'
    return function (stackEffect)
        local rollStackEffect = GetStackEffect(stackEffect.Targets[target_idx].Value)

        assert(IsRollStackEffect(rollStackEffect), 'Provided a non-roll target stack effect for FS.C.Effect.ModifyTargetRoll')

        local rolled = rollStackEffect.Value
        local optionsIndex = {}
        local optionsT = {}
        for _, pair in ipairs(options) do
            local potential = pair.modFunc(rolled)
            -- if potential >= 1 and potential <= 6 then
                optionsT[#optionsT+1] = pair.option
                optionsIndex[pair.option] = pair.modFunc
            -- end
        end

        assert(#optionsT > 0, 'Provided invalid options table for FS.C.Effect.ModifyTargetRoll (optionsT table is empty)')

        local choice = optionsT[1]
        if #optionsT > 1 then
            choice = PromptString(stackEffect.OwnerIdx, optionsT, hint)
        end

        local modFunc = optionsIndex[choice]
        SetRollValue(rollStackEffect, modFunc(rolled))
        return true
    end
end

function FS.C.Effect.GainTreasure(amount, filterFunc)
    return FS.C.Effect._ApplyToPlayer(function (player, stackEffect)
        GainTreasure(player.Idx, amount)
        return true
    end, filterFunc)
end

function FS.C.Effect.Loot(amount, filterFunc)
    return FS.C.Effect._ApplyToPlayer(function (player, stackEffect)
        LootCards(player.Idx, amount, stackEffect)
        return true
    end, filterFunc)
end

function FS.C.Effect.DamageToPlayer(amount, filterFunc)
    return FS.C.Effect._ApplyToPlayer(function (player, stackEffect)
        DealDamageToPlayer(player.Idx, amount, stackEffect)
        return true
    end, filterFunc)
end

function FS.C.Effect.DamageToMonster(amount, filterFunc)
    return function (stackEffect, args)
        local monsters = filterFunc(stackEffect, args)
        for _, m in ipairs(monsters) do
            DealDamageToCard(m.IPID, amount, stackEffect)
        end
        return true
    end
end

function FS.C.Effect.KillOwner()
    return function (stackEffect)
        KillPlayer(stackEffect.OwnerIdx, stackEffect)
    end
end

function FS.C.Effect.RemoveAndBecomeASoul()
    return function (stackEffect)
        local me = stackEffect.Card
        RemoveFromEverywhere(me.Card.ID)
        AddSoulCard(me.Owner.Idx, me.Card)
    end
end

-- direction: 1 for left, -1 for right
function FS.C.Effect.HandShift(direction)
    return function (stackEffect)
        local hands = {}
        local players = GetPlayers()

        -- record player's hands
        for _, player in ipairs(players) do
            local hand = {}
            for hi = 0, player.Hand.Count - 1 do
                hand[#hand+1] = player.Hand[0].Card
                RemoveFromHand(player.Idx, 0)
            end
            hands[player.Idx + 1] = hand
        end

        -- give the cards
        for idx, hand in ipairs(hands) do
            local targetIdx = math.fmod(idx - 1 + direction + #players, #players)
            for _, card in ipairs(hand) do
                AddToHand(targetIdx, card)
            end
        end

        return true
    end
end

-- TODO change hint to hintFunc
function FS.C.Effect.Discard(amount, filterFunc, hint)
    hint = hint or 'Choose a card to discard'
    
    return FS.C.Effect._ApplyToPlayer(function (player, stackEffect)
        -- TODO could be better
        for di = 0, amount - 1 do
            local player_idx = player.Idx
            local hand = GetPlayer(player_idx).Hand
            local indicies = {}
            for i = 0, hand.Count - 1 do
                indicies[#indicies+1] = i
            end
            if #indicies == 0 then
                return false
            end
            local choice = ChooseCardInHand(player_idx, indicies, hint)
            DiscardFromHand(player_idx, choice)
            SoftReloadState()
        end
        return true
    end, filterFunc)
end

function FS.C.Effect.TillEndOfTurnRaw(layer, effect)
    return function (stackEffect)
        TillEndOfTurn(layer, effect)
    end
end

function FS.C.Effect.StealShopItem(shopItemFilterFunc)
    shopItemFilterFunc = shopItemFilterFunc or function (stackEffect)
        return FS.F.Items():InShop():Do()
    end

    return function (stackEffect)
        local items = shopItemFilterFunc(stackEffect)
        for _, item in ipairs(items) do
            StealItem(stackEffect.OwnerIdx, item.IPID)
        end
        return true
    end
end

function FS.C.Effect.TargetPlayerGivesLootCards(target_idx, amount)
    return function (stackEffect)
        return FS.C.GiveLootCards(stackEffect.OwnerIdx, tonumber(stackEffect.Targets[target_idx].Value), amount)
    end
end

function FS.C.Effect.StealRandomLootCardsFromTarget(target_idx, amount)
    return function (stackEffect)
        local idx = tonumber(stackEffect.Targets[target_idx].Value)
        for i = 1, amount do
            local player_idx = tonumber(idx)
            local hand = GetPlayer(player_idx).Hand
            local cardIdx = RandInt(0, hand.Count)
            local card = GetPlayer(player_idx).Hand[cardIdx].Card
            RemoveFromHand(player_idx, cardIdx)
            AddToHand(stackEffect.OwnerIdx, card)
        end
        return true
    end
end

function FS.C.Effect.MantleTargetPlayer(target_idx)
    return function (stackEffect)
        -- The next time that player would die this turn, prevent it.
        -- If it\'s their turn, cancel everything that hasn\'t resolved and end it.

        local pidx = tonumber(stackEffect.Targets[target_idx].Value)
        AddDeathPreventor(pidx, function (deathSource)
            if GetCurPlayerIdx() == pidx then
                EndTheTurn()
                CancelEverything()
                -- TODO cancel the attack
            end

            return true
        end)

        return true
    end
end

function FS.C.Effect.ModPlayerAttackTEOT(mod)
    return function (stackEffect)
        TillEndOfTurn(
            FS.ModLayers.PLAYER_ATTACK,
            function ()
                local player = GetPlayer(tonumber(stackEffect.OwnerIdx))
                player.Stats.State.Attack = player.Stats.State.Attack + mod
            end
        )
        return true
    end
end

function FS.C.Effect.ModTargetPlayerAttackTEOT(target_idx, mod)
    return function (stackEffect)
        TillEndOfTurn(
            FS.ModLayers.PLAYER_ATTACK,
            function ()
                local idx = tonumber(stackEffect.Targets[target_idx].Value)
                local player = GetPlayer(idx)
                player.Stats.State.Attack = player.Stats.State.Attack + mod
            end
        )
        return true
    end
end

function FS.C.Effect.ModTargetHealthTEOT(target_idx, mod)
    return function (stackEffect)
        TillEndOfTurn(
            FS.ModLayers.PLAYER_MAX_HEALTH,
            function ()
                local idx = tonumber(stackEffect.Targets[target_idx].Value)
                local player = GetPlayer(idx)
                player.Stats.State.Health = player.Stats.State.Health + mod
            end
        )
        return true
    end
end

function FS.C.Effect.AddAO(amount, filterFunc)
    return FS.C.Effect._ApplyToPlayer(function (player, stackEffect)
        AddAttackOpportunities(player.Idx, amount)
        return true
    end, filterFunc)
end

function FS.C.Effect.AddAOToTarget(target_idx, amount)
    return function (stackEffect)
        local idx = tonumber(stackEffect.Targets[target_idx].Value)
        AddAttackOpportunities(idx, amount)
        return true
    end
end

function FS.C.Effect.ModPlayerHealthTEOT(mod)
    return function (stackEffect)
        TillEndOfTurn(
            FS.ModLayers.PLAYER_ATTACK,
            function ()
                local player = GetPlayer(tonumber(stackEffect.OwnerIdx))
                player.Stats.State.Health = player.Stats.State.Health + mod
            end
        )
        return true
    end
end

function FS.C.Effect.ModTargetAttackTEOT(target_idx, mod)
    return function (stackEffect)

        local target = stackEffect.Targets[target_idx]
        local type = TargetTypeToInt(target.Type)

        if type == FS.TargetTypes.PLAYER then
            TillEndOfTurn(
                FS.ModLayers.PLAYER_ATTACK,
                function ()
                    local player = GetPlayer(tonumber(target.Value))
                    player.Stats.State.Attack = player.Stats.State.Attack + mod
                end
            )
            return true
        end
        if type == FS.TargetTypes.IN_PLAY_CARD then
            TillEndOfTurn(
                FS.ModLayers.MONSTER_ATTACK,
                function ()
                    local monster = GetInPlayCardOrDefault(target.Value)
                    if monster == nil then
                        return
                    end
                    monster.Stats.State.Attack = monster.Stats.State.Attack + mod
                end
            )
            return true
        end

        error('Invalid attack mod target: '..tostring(type))
    end
end

function FS.C.Effect.ModTargetMonsterEvasionTEOT(target_idx, mod)
    return function (stackEffect)
        local target = stackEffect.Targets[target_idx]
        TillEndOfTurn(
            FS.ModLayers.MONSTER_EVASION,
            function ()
                local monster = GetInPlayCardOrDefault(target.Value)
                if monster == nil then
                    return
                end
                monster.Stats.State.Evasion = monster.Stats.State.Evasion + mod
            end
        )
        return true
    end
end

function FS.C.Effect.ModMonsterAttackTEOT(mod, monsterFilter)
    monsterFilter = monsterFilter or function (stackEffect)
        return FS.F.Monsters():IPID(stackEffect.Card.IPID):Do()
    end
    return function (stackEffect)
        TillEndOfTurn(
            FS.ModLayers.MONSTER_ATTACK,
            function ()
                local monsters = monsterFilter(stackEffect)
                for _, monster in ipairs(monsters) do
                    monster.Stats.State.Attack = monster.Stats.State.Attack + mod
                end
            end
        )
    end
end

function FS.C.Effect.ModMonsterEvasionTEOT(mod, monsterFilter)
    monsterFilter = monsterFilter or function (stackEffect)
        return FS.F.Monsters():IPID(stackEffect.Card.IPID):Do()
    end
    return function (stackEffect)
        TillEndOfTurn(
            FS.ModLayers.MONSTER_EVASION,
            function ()
                local monsters = monsterFilter(stackEffect)
                for _, monster in ipairs(monsters) do
                    monster.Stats.State.Evasion = monster.Stats.State.Evasion + mod
                end
            end
        )
    end
end

function FS.C.Effect.ReorderTop(amount, deckID)
    return function (stackEffect)
        local dID = deckID
        if dID == nil then
            local deckIDs = GetDeckIDs()
            dID = ChooseDeck(stackEffect.OwnerIdx, deckIDs, 'Choose a deck')
        end
        local cards = RemoveTopCards(dID, amount)
        
        local extract = function (card)
            return card.LogName
        end
        local remove = function (s)
            local result = nil
            local newCards = {}
            for _, card in ipairs(cards) do
                if extract(card) == s then
                    result = card
                else
                    newCards[#newCards+1] = card
                end
            end
            cards = newCards
            return result
        end
        local getChoices = function ()
            local result = {}
            for _, card in ipairs(cards) do
                result[#result+1] = extract(card)
            end
            return result
        end
        while #cards > 0 do
            -- TODO change to card prompt
            local choice = PromptString(stackEffect.OwnerIdx, getChoices(), 'Choose a card to put on top')
            local card = remove(choice)
            PutOnTop(dID, card)
        end
        return true
    end
end

function FS.C.Effect.Scry(amount, deckID)
    assert(amount == 1, 'No support for FS.C.Effect.Scry for more or less than 1 card :(')
    return function (stackEffect)
        local dID = deckID
        if dID == nil then
            local deckIDs = GetDeckIDs()
            dID = ChooseDeck(stackEffect.OwnerIdx, deckIDs, 'Choose a deck')
        end
        local cards = RemoveTopCards(dID, amount)
        if #cards == 0 then
            return true
        end
        local card = cards[1]
        local accept = FS.C.Choose.YesNo(stackEffect.OwnerIdx, 'Leave '..card.LogName..' on top?')
        if accept then
            PutOnTop(dID, card)
            return true
        end
        PutToBottom(dID, card)
        return true
    end
end

function FS.C.Effect.LootAndPlaceOnTopRestToBottom(amount, toTop, deckID)
    assert(amount > toTop, 'Provided invalid arguments for LootAndPlaceOnTopRestToBottom: amount is less than toTop')

    return function (stackEffect)
        local dID = deckID
        if dID == nil then
            local deckIDs = GetDeckIDs()
            dID = ChooseDeck(stackEffect.OwnerIdx, deckIDs, 'Choose a deck')
        end
        local cards = RemoveTopCards(dID, amount)

        local extract = function (card)
            return card.LogName
        end
        local remove = function (s)
            local result = nil
            local newCards = {}
            for _, card in ipairs(cards) do
                if extract(card) == s then
                    result = card
                else
                    newCards[#newCards+1] = card
                end
            end
            cards = newCards
            return result
        end
        local getChoices = function ()
            local result = {}
            for _, card in ipairs(cards) do
                result[#result+1] = extract(card)
            end
            return result
        end
        while toTop > 0 do
            -- TODO change to card prompt
            local choice = PromptString(stackEffect.OwnerIdx, getChoices(), 'Choose a card to put on top')
            local card = remove(choice)
            PutOnTop(dID, card)
            toTop = toTop - 1
        end
        -- TODO order
        for _, card in ipairs(cards) do
            PutToBottom(dID, card)
        end
        return true
    end
end

function FS.C.Effect.RechargeCharacter(optional)
    optional = optional or false
    return function (stackEffect)
        local card = GetPlayer(stackEffect.OwnerIdx).Character
        if optional then
            local accept = FS.C.Choose.YesNo(stackEffect.OwnerIdx, 'Recharge '..card.LogName..'?')
            if not accept then
                return true
                -- TODO? return false
            end
        end
        Recharge(card.IPID)
        return true
    end
end

-- common costs (for activated abilities)
FS.C.Cost = {}

function FS.C.Cost.Tap()
    local result = {}

    function result.Pay(me, player, stackEffect)
        return TapCard(me.IPID)
    end

    function result.Check(me, player)
        return not me.Tapped
    end

    return result
end

function FS.C.Cost.RemoveCounters(amount)
    local result = {}

    function result.Pay(me, player, stackEffect)
        RemoveCounters(me.IPID, amount)
        return true
    end

    function result.Check(me, player)
        return GetCountersCount(me.IPID) >= amount
    end

    return result
end

function FS.C.Cost.DiscardLoot(amount, hint)
    local result = {}

    function result.Pay(me, player, stackEffect)
        FS.C.Effect.Discard(amount, hint)(stackEffect)
        return true
    end

    function result.Check(me, player)
        return player.Hand.Count >= amount
    end

    return result
end

function FS.C.Cost.DestroyMe()
    local result = {}

    function result.Pay(me, player, stackEffect)
        if not FS.C.Choose.YesNo(player.Idx, 'Destroy '..me:GetFormattedName()..'?') then
            return false
        end
        DestroyItem(me.IPID)
        return true
    end

    function result.Check(me, player)
        return not me:HasLabel(FS.Labels.Eternal)
    end

    return result
end

function FS.C.Cost.SacrificeItems(amount, itemFilterFunc)
    itemFilterFunc = itemFilterFunc or function (me, player)
        return FS.F.Items():ControlledBy(player.Idx):Destructable():Do()
    end

    local result = {}

    function result.Pay(me, player, stackEffect)
        for i = 1, amount do
            local items = itemFilterFunc(me, player)
            local ipids = FS.C.IPIDs(items)
            local ipid = ChooseItem(player.Idx, ipids, 'Choose an item to sacrifice ('..(amount-i+1)..' left)')
            DestroyItem(ipid)
        end
        return true
    end

    function result.Check(me, player)
        return #itemFilterFunc(me, player) >= amount
    end

    return result
end

function FS.C.Cost.DonateItems(amount, itemFilterFunc, playerFilterFunc)
    local result = {}

    function result.Pay(me, player, stackEffect)

        for i = 1, amount do
            local items = itemFilterFunc(me, player)
            local ipids = FS.C.IPIDs(items)
            local ipid = ChooseItem(player.Idx, ipids, 'Choose an item to donate ('..(amount-i+1)..' left)')
            local item = GetInPlayCard(ipid)

            local players = playerFilterFunc(me, player)
            local indicies = FS.C.PlayerIndicies(players)
            local idx = ChoosePlayer(player.Idx, indicies, 'Choose a player to donate '..item.LogName..' to')
            StealItem(idx, ipid)
        end
        return true
    end

    function result.Check(me, player)
        return #itemFilterFunc(me, player) >= amount
    end

    return result
end

-- TODO replacement effects affect how much is required to pay
function FS.C.Cost.PayCoins(amount)
    local result = {}

    function result.Pay(me, player, stackEffect)
        if GetConfig().PromptWhenPayingCoins then
            local accept = FS.C.Choose.YesNo(player.Idx, 'Pay '..amount..'{cent}?')
            if not accept then
                return false
            end
        end
        PayCoins(player.Idx, amount)
        return true
    end

    function result.Check(me, player)
        return player.Coins >= amount
    end

    return result
end

function FS.C.Cost.PayHealth(amount)
    local result = {}

    function result.Pay(me, player, stackEffect)
        if GetConfig().PromptWhenPayingLife then
            local accept = FS.C.Choose.YesNo(player.Idx, 'Pay '..amount..'{health}?')
            if not accept then
                return false
            end
        end
        LoseHealth(player.Idx, amount, stackEffect)
        return true
    end

    function result.Check(me, player)
        return player.Stats.State.Health - player.Stats.Damage >= amount
    end

    return result
end

FS.C.StateMod = {}

function FS.C.StateMod.ModMaxLootPlays(amount)
    local result = {}
    
    result.Layer = FS.ModLayers.MOD_MAX_LOOT_PLAYS
    function result.Mod(me)
        if me.Owner.Idx ~= GetCurPlayerIdx() then
            return
        end
        me.Owner.State.LootPlaysForTurn = me.Owner.State.LootPlaysForTurn + amount
    end

    return result
end

function FS.C.StateMod.AddSouls(amount, playerFilterFunc)
    playerFilterFunc = playerFilterFunc or function (me)
        return FS.F.Players():Idx(me.Owner.Idx):Do()
    end

    local result = {}
    result.Layer = FS.ModLayers.PLAYER_SOUL_COUNT
    function result.Mod(me)
        local players = playerFilterFunc(me)
        for _, player in ipairs(players) do
            player.State.AdditionalSoulCount = player.State.AdditionalSoulCount + 1
        end
    end

    return result
end

function FS.C.StateMod.ModPlayerAttack(modF, playerFilterFunc)
    playerFilterFunc = playerFilterFunc or function (me)
        return FS.F.Players():Idx(me.Owner.Idx):Do()
    end

    local result = {}
    result.Layer = FS.ModLayers.PLAYER_ATTACK
    function result.Mod(me)
        local players = playerFilterFunc(me)
        for _, player in ipairs(players) do
            player.Stats.State.Attack = player.Stats.State.Attack + modF(me, player)
        end
    end

    return result
end

function FS.C.StateMod.PlusToAttackRolls(modF, playerFilterFunc)
    local result = {}
    result.Layer = FS.ModLayers.ROLL_RESULT_MODIFIERS
    function result.Mod(me)
        me.Owner.State.RollResultModifiers:Add(
            function (roll, stackEffect)
                if not stackEffect.IsAttackRoll then
                    return roll
                end
                return roll + 1
            end
        )
    end

    return result
end

function FS.C.StateMod.ModMonsterEvasion(modF, monsterFilterFunc)
    monsterFilterFunc = monsterFilterFunc or FS.C.AllMonsters

    local result = {}
    result.Layer = FS.ModLayers.MONSTER_EVASION
    function result.Mod(me)
        local monsters = monsterFilterFunc(me)
        for _, monster in ipairs(monsters) do
            monster.Stats.State.Evasion = monster.Stats.State.Evasion + modF(me, monster)
        end
    end

    return result
end

function FS.C.StateMod.ModMonsterAttack(modF, monsterFilterFunc)
    monsterFilterFunc = monsterFilterFunc or FS.C.AllMonsters

    local result = {}
    result.Layer = FS.ModLayers.MONSTER_ATTACK
    function result.Mod(me)
        local monsters = monsterFilterFunc(me)
        for _, monster in ipairs(monsters) do
            monster.Stats.State.Attack = monster.Stats.State.Attack + modF(me, monster)
        end
    end

    return result
end

function FS.C.StateMod.ModMonsterHealth(modF, monsterFilterFunc)
    monsterFilterFunc = monsterFilterFunc or FS.C.AllMonsters

    local result = {}
    result.Layer = FS.ModLayers.MONSTER_HEALTH
    function result.Mod(me)
        local monsters = monsterFilterFunc(me)
        for _, monster in ipairs(monsters) do
            monster.Stats.State.Health = monster.Stats.State.Health + modF(me, monster)
        end
    end

    return result
end

function FS.C.StateMod.ModPlayerHealth(modF, playerFilterFunc)
    playerFilterFunc = playerFilterFunc or function (me)
        return FS.F.Players():Idx(me.Owner.Idx):Do()
    end

    local result = {}
    result.Layer = FS.ModLayers.PLAYER_MAX_HEALTH
    function result.Mod(me)
        local players = playerFilterFunc(me)
        for _, player in ipairs(players) do
            player.Stats.State.Health = player.Stats.State.Health + modF(me, player)
        end
    end

    return result
end

function FS.C.StateMod.ShopItemsCostsNLess(amount, playerFilterFunc)
    playerFilterFunc = playerFilterFunc or function (me)
        return FS.F.Players():Idx(me.Owner.Idx):Do()
    end

    local result = {}
    result.Layer = FS.ModLayers.PURCHASE_COST
    function result.Mod(me)
        local players = playerFilterFunc(me)
        for _, player in ipairs(players) do
            player.State.PurchaseCostModifiers:Add(function (slot, cost)
                if slot >= 0 then
                    return cost - amount
                end
                return cost
            end)
        end
    end
    return result
end

function FS.C.StateMod.TakeNoCombatDamageOnRollsForPlayer(rolls)
    local result = {}
    result.Layer = FS.ModLayers.DAMAGE_RECEIVED_MODIFICATORS

    function result.Mod(me)
        me.Owner.Stats.State.ReceivedDamageModifiers:Add(
            function (amount, sourceEffect)
                for _, roll in ipairs(rolls) do
                    if roll == sourceEffect.Roll then
                        return 0
                    end
                end
                return amount
            end
        )
    end

    return result
end

function FS.C.StateMod.TakeNoCombatDamageOnRollsForMonster(rolls)
    local result = {}
    result.Layer = FS.ModLayers.DAMAGE_RECEIVED_MODIFICATORS

    function result.Mod(me)
        me.Stats.State.ReceivedDamageModifiers:Add(
            function (amount, sourceEffect)
                print('AMOPGUS')
                for _, roll in ipairs(rolls) do
                    if roll == sourceEffect.Roll then
                        return 0
                    end
                end
                return amount
            end
        )
    end

    return result
end

-- builders
FS.B = {}

function FS.B.Card()
    local result = {}

    result.startingItemKeys = {}
    result.matchStartEffects = {}

    result.effectGroups = {}
    result.activatedAbilities = {}
    result.triggeredAbilities = {}
    result.rewards = {}
    result.labels = {}
    result.stateModifiers = {}

    result.lootText = ''
    result.lootCosts = {}
    result.lootChecks = {}
    result.lootTargets = {}
    result.lootFizzleChecks = {}

    function result:Build()
        local card = {}

        -- starting items
        card.StartingItemKeys = result.startingItemKeys

        -- match start effects
        card.MatchStartEffects = result.matchStartEffects

        -- effects
        card.Effects = {}
        for _, group in ipairs(self.effectGroups) do
            card.Effects[#card.Effects+1] = function (stackEffect)
                for _, e in ipairs(group) do
                    if not e(stackEffect) then
                        return
                    end
                end
            end
        end

        -- activated abilities
        card.ActivatedAbilities = result.activatedAbilities

        -- triggered abilities
        card.TriggeredAbilities = result.triggeredAbilities

        -- rewards
        card.Rewards = result.rewards

        -- labels
        card.Labels = result.labels

        -- state modifiers
        card.StateModifiers = result.stateModifiers

        -- loot text
        card.LootText = result.lootText or '' -- TODO remove "or ''"

        -- additional loot costs
        card.LootCosts = {}
        for _, target in ipairs(result.lootTargets) do
            card.LootCosts[#card.LootCosts+1] = target
        end
        for _, cost in ipairs(result.lootCosts) do
            card.LootCosts[#card.LootCosts+1] = cost
        end

        -- addditional loot checks
        card.LootChecks = result.lootChecks

        card.FizzleCheck = function (stackEffect)
            for _, check in ipairs(result.lootFizzleChecks) do
                if not check(stackEffect) then
                    return false
                end
            end
            return true
        end

        return card
    end

    -- choose loot effect
    function result:Choose(choices)
        result.lootCosts[#result.lootCosts+1] = choices.Pay
        result.effectGroups[#result.effectGroups+1] = {choices.Effect}

        return result
    end

    result.Effect = {}

    -- add common effect(s) (these are considered "and" effects - if one fails, the rest fill not be executed)
    function result.Effect:Common(...)
        local commons = {...}
        assert(#commons > 0, 'provided 0 common effect functions in result.Effect:Common function (for Loot)')

        result.effectGroups[#result.effectGroups+1] = commons

        return result
    end

    function result.Effect:Custom(...)
        local commons = {...}
        assert(#commons > 0, 'provided 0 custom effect functions in result.Effect:Custom function (for Loot)')

        result.effectGroups[#result.effectGroups+1] = commons

        return result
    end

    function result.Effect:Roll(effect)
        result.lootCosts[#result.lootCosts+1] = function (me, player, stackEffect)
            Roll(stackEffect)
            return true
        end

        result.effectGroups[#result.effectGroups+1] = {effect}
        return result
    end

    result.Target = {}

    function result.Target._AddFizzleCheck(filterFunc, extractFunc, checkType)
        local targetIdx = #result.lootTargets

        result.lootFizzleChecks[#result.lootFizzleChecks+1] = function (stackEffect)
            local target = stackEffect.Targets[targetIdx].Value

            -- * this is here for "choose a monster or player"
            if TargetTypeToInt(stackEffect.Targets[targetIdx].Type) ~= checkType then
                return true
            end

            local options = filterFunc(stackEffect.Card, GetPlayer(stackEffect.OwnerIdx))
            for _, o in ipairs(options) do
                if extractFunc(o) == target then
                    return true
                end
            end
            return false
        end
    end

    function result.Target._AddLootCheck(filterFunc)
        result.lootChecks[#result.lootChecks+1] = function (player)
            return #filterFunc(player) > 0
        end
    end

    function result.Target:StackEffect(filterFunc, hint)
        hint = hint or 'Choose stack effect'

        result.Target._AddFizzleCheck(filterFunc, function (effect)
            return effect.SID
        end, FS.TargetTypes.STACK_EFFECT)
        result.Target._AddLootCheck(filterFunc)

        result.lootTargets[#result.lootTargets+1] = function (me, player, stackEffect)
            local options = filterFunc(player)
            local indicies = {}
            for _, e in ipairs(options) do
                indicies[#indicies+1] = e.SID
            end
            -- TODO add optional
            local target = ChooseStackEffect(player.Idx, indicies, hint)
            AddTarget(stackEffect, FS.TargetTypes.STACK_EFFECT, target)

            return true
        end

        return result
    end

    function result.Target:Item(filterFunc, hint)
        hint = hint or 'Choose an Item'

        result.Target._AddFizzleCheck(filterFunc, function (item)
            return item.IPID
        end, FS.TargetTypes.IN_PLAY_CARD)
        result.Target._AddLootCheck(filterFunc)

        result.lootTargets[#result.lootTargets+1] = function (me, player, stackEffect)
            local options = filterFunc(player)
            local ipids = FS.C.IPIDs(options)

            -- TODO add optional
            local ipid = ChooseItem(player.Idx, ipids, hint)
            AddTarget(stackEffect, FS.TargetTypes.IN_PLAY_CARD, ipid)

            return true
        end

        return result
    end

    function result.Target:Player(filterFunc, hint)
        hint = hint or 'Choose a player'
        filterFunc = filterFunc or function (player)
            return FS.F.Players():Do()
        end

        result.Target._AddFizzleCheck(filterFunc, function (player)
            return tostring(player.Idx)
        end, FS.TargetTypes.PLAYER)
        result.Target._AddLootCheck(filterFunc)

        result.lootTargets[#result.lootTargets+1] = function (me, player, stackEffect)
            local options = filterFunc(player)
            local indicies = {}
            for _, p in ipairs(options) do
                indicies[#indicies+1] = p.Idx
            end

            -- TODO add optional
            local idx = ChoosePlayer(player.Idx, indicies, hint)
            AddTarget(stackEffect, FS.TargetTypes.PLAYER, tostring(idx))

            return true
        end

        return result
    end
    
    function result.Target:MonsterOrPlayer(monsterFilterFunc, playerFilterFunc, hint)
        hint = hint or 'Choose a Monster or player'

        playerFilterFunc = playerFilterFunc or function (player)
            return FS.F.Players():Do()
        end
        result.Target._AddFizzleCheck(playerFilterFunc, function (player)
            return tostring(player.Idx)
        end, FS.TargetTypes.PLAYER)
        result.Target._AddLootCheck(playerFilterFunc)

        monsterFilterFunc = monsterFilterFunc or function (monster)
            return FS.F.Monsters():Do()
        end
        result.Target._AddFizzleCheck(monsterFilterFunc, function (monster)
            return monster.IPID
        end, FS.TargetTypes.IN_PLAY_CARD)
        result.Target._AddLootCheck(monsterFilterFunc)

        result.lootTargets[#result.lootTargets+1] = function (me, player, stackEffect)
            local pOptions = playerFilterFunc(player)
            local indicies = {}
            for _, p in ipairs(pOptions) do
                indicies[#indicies+1] = p.Idx
            end
            
            local mOptions = monsterFilterFunc(player)
            local ipids = {}
            for _, p in ipairs(mOptions) do
                ipids[#ipids+1] = p.IPID
            end

            -- TODO add optional
            local choice = ChooseMonsterOrPlayer(player.Idx, ipids, indicies, hint)

            AddTarget(stackEffect, choice.type, tostring(choice.value))

            return true
        end

        return result
    end

    -- add activated ability
    function result:ActivatedAbility(aa)
        result.activatedAbilities[#result.activatedAbilities+1] = aa
        return result
    end

    -- add triggered ability
    function result:TriggeredAbility(ta)
        result.triggeredAbilities[#result.triggeredAbilities+1] = ta
        return result
    end

    function result:Reward(reward)
        result.rewards[#result.rewards+1] = reward
        return result
    end

    -- add label to card
    function result:Label(label)
        result.labels[#result.labels+1] = label
        return self
    end

    result.Static = {}

    function result.Static:Raw(layer, text, func)
        if result.stateModifiers[layer] == nil then
            result.stateModifiers[layer] = {}
        end
        local t = result.stateModifiers[layer]
        t[#t+1] = {
            func = func,
            text = text
        }
        return result
    end

    function result.Static:Common(text, commonMod)
        return result.Static:Raw(commonMod.Layer, text, commonMod.Mod)
    end

    function result:Haunt()
        return result:TriggeredAbility(
            FS.B.TriggeredAbility('When you die, before paying penalties, give this to another player.')
                .On:ControllerDeathBeforePenalties()
                .Target:Player(function (me, player)
                    return FS.F.Players():Except(player.Idx):Do()
                end)
                .Effect:Common(
                    FS.C.Effect.GiveMeToTargetPlayer(0)
                )
            :Build()
        )
    end

    return result
end

-- loot card builder
function FS.B.Loot(effectText)
    local result = FS.B.Card()
    result.lootText = effectText

    function result:Trinket()
        self:Label(FS.Labels.Trinket)

        self.effectGroups[#self.effectGroups+1] = {function (lootStackEffect)
            lootStackEffect.GoesToDiscard = false
            local card = lootStackEffect.Card

            CreateOwnedItem(card, lootStackEffect.OwnerIdx)
        end}

        return self
    end

    return result
end

-- event card builder
function FS.B.Event(effectText)
    local result = FS.B.Card()
    result.lootText = effectText

    return result
end

-- curse card builder
function FS.B.Curse()
    local result = FS.B.Card()

    result.Target:Player()
    result.Effect:Custom(
        function (stackEffect)
            local removed = TryDiscardFromPlay(stackEffect.Card.IPID)
            if not removed then
                return false
            end
            CreateCurseCard(stackEffect.Card.Card, tonumber(stackEffect.Targets[0].Value))
            return true
        end
    )
    -- result:TriggeredAbility(
    --     FS.B.TriggeredAbility('When this enters play, give this to a player.')
    --         .On:MeEnteringPlay()
    --         .Target:Player()
    --         .Effect:Custom(
    --             function (stackEffect)
    --                 local removed = TryDiscardFromPlay(stackEffect.Card.IPID)
    --                 if not removed then
    --                     return false
    --                 end
    --                 CreateOwnedItem(stackEffect.Card.Card, tonumber(stackEffect.Targets[0].Value))
    --                 return true
    --             end
    --         )
    --     :Build()
    -- )

    result:TriggeredAbility(
        FS.B.TriggeredAbility('When you die, put this into discard.')
            .On:ControllerDeath()
            .Effect:Common(
                FS.C.Effect.DiscardMeFromPlay()
            )
        :Build()
    )

    return result
end

-- character builder
function FS.B.Character()
    local result = FS.B.Item()

    function result:Basic()
        result:ActivatedAbility(
            FS.B.ActivatedAbility('{T}', 'Play an additional Loot card this turn.')
                .Cost:Common(
                    FS.C.Cost.Tap()
                )
                .Effect:Common(
                    FS.C.Effect.AddLootPlay(1)
                )
            :Build()
        )
        return result
    end

    function result:StartingItem(key)
        result.startingItemKeys[#result.startingItemKeys+1] = key
        return result
    end

    function result:OnStart(func)
        result.matchStartEffects[#result.matchStartEffects+1] = func
        return result
    end

    return result
end

-- item builder
function FS.B.Item()
    local result = FS.B.Card()

    return result
end

-- bouns soul builder
function FS.B.BonusSoul()
    local result = FS.B.Card()

    function result:Check(predicate)
        result.Static:Raw(
            FS.ModLayers.LAST,
            '',
            function (me)
                local players = FS.F.Players():Custom(predicate):Do()
                local indicies = FS.C.PlayerIndicies(players)
                if #indicies == 0 then
                    return
                end
                local idx = indicies[1]
                if #indicies > 1 then
                    local choice = ChoosePlayer(GetCurPlayerIdx(), indicies, 'Choose a player to gain '..me.LogName)
                    idx = choice
                end

                RemoveFromBonusSouls(me.ID)
                AddSoulCard(idx, me)
            end
        )
        return result
    end

    return result
end

function FS.B.Room()
    local result = FS.B.Card()

    return result
end

function FS.B.Monster()
    local result = FS.B.Card()

    return result
end

function FS.B._Ability(effectText)
    local result = {}

    result.costs = {}
    result.targets = {}
    result.effectGroups = {}
    result.fizzleChecks = {}

    result.Cost = {}
    result.Effect = {}

    result.builders = {}

    result.builders[#result.builders+1] = function (ability)
        -- TODO repeated code

        local effects = {}

        for _, group in ipairs(result.effectGroups) do
            effects[#effects+1] = function (stackEffect, args)
                for _, e in ipairs(group) do
                    if not e(stackEffect, args) then
                        return
                    end
                end
            end
        end

        ability.EffectText = effectText

        ability.Check = function (me, player, args)
            for _, target in ipairs(result.targets) do
                if not target.Check(me, player, args) then
                    return false
                end
            end
            for _, cost in ipairs(result.costs) do
                if not cost.Check(me, player, args) then
                    return false
                end
            end
            return true
        end

        ability.Cost = function (me, player, stackEffect, args)
            for _, target in ipairs(result.targets) do
                if not target.Pay(me, player, stackEffect, args) then
                    return false
                end
            end
            for _, cost in ipairs(result.costs) do
                if not cost.Pay(me, player, stackEffect, args) then
                    return false
                end
            end
            return true
        end

        ability.Effects = effects

        ability.FizzleCheck = function (stackEffect)
            for _, check in ipairs(result.fizzleChecks) do
                if not check(stackEffect) then
                    return false
                end
            end
            return true
        end
    end

    -- build activated ability
    function result:Build()
        local ability = {}
        for _, b in ipairs(result.builders) do
            b(ability)
        end
        return ability
    end

    result.Check = {}

    function result.Check:Custom(check)
        result.costs[#result.costs+1] = {
            Check = function (me, player)
                return check(me, player)
            end,
            Pay = function (me, player, stackEffect)
                return true
            end
        }

        return result
    end

    -- add common cost(s)
    function result.Cost:Common(...)
        local costs = {...}

        for _, cost in ipairs(costs) do
            result.costs[#result.costs+1] = cost
        end

        return result
    end

    -- TODO repeated code
    function result.Effect:Roll(effect)
        result.costs[#result.costs+1] = {
            Check = function (me, player)
                return true
            end,
            Pay = function (me, player, stackEffect)
                Roll(stackEffect)
                return true
            end
        }

        return result.Effect:Custom(effect)
    end

    function result.Effect:Custom(effect)
        result.effectGroups[#result.effectGroups+1] = {effect}
        return result
    end

    result.Target = {}

    function result.Target._AddFizzleCheck(filterFunc, extractFunc, checkType)
        local targetIdx = #result.targets

        result.fizzleChecks[#result.fizzleChecks+1] = function (stackEffect)
            if TargetTypeToInt(stackEffect.Targets[targetIdx].Type) ~= checkType then
                return true
            end
            local target = stackEffect.Targets[targetIdx].Value

            local options = filterFunc(stackEffect.Card, GetPlayerOrCurrent(stackEffect.OwnerIdx))
            for _, o in ipairs(options) do
                if extractFunc(o) == target then
                    return true
                end
            end
            return false
        end

        return result
    end

    function result.Target:MonsterOrPlayer(monsterFilterFunc, playerFilterFunc, hint)
        playerFilterFunc = playerFilterFunc or function (player)
            return FS.F.Players():Do()
        end
        monsterFilterFunc = monsterFilterFunc or function (monster)
            return FS.F.Monsters():Do()
        end
        hint = hint or 'Choose a monster or player'

        result.Target._AddFizzleCheck(playerFilterFunc, function (player)
            return tostring(player.Idx)
        end, FS.TargetTypes.PLAYER)
        result.Target._AddFizzleCheck(monsterFilterFunc, function (player)
            return player.IPID
        end, FS.TargetTypes.IN_PLAY_CARD)

        result.targets[#result.targets+1] = {
            Check = function (me, player)
                return (#playerFilterFunc(me, player) + #monsterFilterFunc(me, player)) > 0
            end,
            Pay = function (me, player, stackEffect)
                local pOptions = playerFilterFunc(player)
                local indicies = {}
                for _, p in ipairs(pOptions) do
                    indicies[#indicies+1] = p.Idx
                end
                
                local mOptions = monsterFilterFunc(player)
                local ipids = {}
                for _, p in ipairs(mOptions) do
                    ipids[#ipids+1] = p.IPID
                end

                -- TODO add optional
                local choice = ChooseMonsterOrPlayer(player.Idx, ipids, indicies, hint)

                AddTarget(stackEffect, choice.type, tostring(choice.value))

                return true
            end
        }
        return result
    end

    function result.Target:Player(filterFunc, hint)
        filterFunc = filterFunc or function (me, player)
            return FS.F.Players():Do()
        end
        hint = hint or 'Choose a player'

        result.Target._AddFizzleCheck(filterFunc, function (player)
            return tostring(player.Idx)
        end, FS.TargetTypes.PLAYER)

        result.targets[#result.targets+1] = {
            Check = function (me, player)
                return #filterFunc(me, player) > 0
            end,
            Pay = function (me, player, stackEffect)
                local options = filterFunc(me, player)
                local indicies = {}
                for _, p in ipairs(options) do
                    indicies[#indicies+1] = p.Idx
                end
                -- TODO add optional
                local target = ChoosePlayer(player.Idx, indicies, hint)
                AddTarget(stackEffect, FS.TargetTypes.PLAYER, tostring(target))

                return true
            end
        }
        return result
    end

    function result.Target:Item(filterFunc, hint)
        hint = hint or 'Choose an item'
        result.Target._AddFizzleCheck(filterFunc, function (item)
            return item.IPID
        end, FS.TargetTypes.IN_PLAY_CARD)

        result.costs[#result.costs+1] = {
            Check = function (me, player)
                return #filterFunc(me, player) > 0
            end,
            Pay = function (me, player, stackEffect)
                local options = filterFunc(me, player)
                local ipids = {}
                for _, item in ipairs(options) do
                    ipids[#ipids+1] = item.IPID
                end
                local ipid = ChooseItem(player.Idx, ipids, hint)
                AddTarget(stackEffect, FS.TargetTypes.IN_PLAY_CARD, ipid)
                return true
            end
        }
        return result
    end

    function result.Target:StackEffect(filterFunc, hint)
        hint = hint or 'Choose a stack effect'
        result.Target._AddFizzleCheck(filterFunc, function (effect)
            return effect.SID
        end, FS.TargetTypes.STACK_EFFECT)

        result.costs[#result.costs+1] = {
            Check = function (me, player)
                return #filterFunc(me, player) > 0
            end,
            Pay = function (me, player, stackEffect)
                local options = filterFunc(me, player)
                local indicies = {}
                for _, e in ipairs(options) do
                    indicies[#indicies+1] = e.SID
                end
                -- TODO add optional
                local target = ChooseStackEffect(player.Idx, indicies, hint)
                AddTarget(stackEffect, FS.TargetTypes.STACK_EFFECT, target)

                return true
            end
        }
        return result
    end

    function result.Target:Character(filterFunc, hint)
        filterFunc = filterFunc or function (me, player)
            return FS.F.Characters():Do()
        end
        hint = hint or 'Choose a Character'
        result.Target._AddFizzleCheck(filterFunc, function (character)
            return character.IPID
        end, FS.TargetTypes.IN_PLAY_CARD)

        result.costs[#result.costs+1] = {
            Check = function (me, player)
                return #filterFunc(me, player) > 0
            end,
            Pay = function (me, player, stackEffect)
                local options = filterFunc(me, player)
                local indicies = {}
                for _, e in ipairs(options) do
                    indicies[#indicies+1] = e.IPID
                end
                -- TODO add optional
                local ipid = ChooseInPlayCard(player.Idx, indicies, hint)
                AddTarget(stackEffect, FS.TargetTypes.IN_PLAY_CARD, ipid)

                return true
            end
        }
        return result
    end

    function result.Target:Monster(filterFunc, hint)
        filterFunc = filterFunc or function (me, player)
            return FS.F.Monsters():Do()
        end
        hint = hint or 'Choose a Monster'
        result.Target._AddFizzleCheck(filterFunc, function (monster)
            return monster.IPID
        end, FS.TargetTypes.IN_PLAY_CARD)

        result.costs[#result.costs+1] = {
            Check = function (me, player)
                return #filterFunc(me, player) > 0
            end,
            Pay = function (me, player, stackEffect)
                local options = filterFunc(me, player)
                local indicies = {}
                for _, e in ipairs(options) do
                    indicies[#indicies+1] = e.IPID
                end
                -- TODO add optional
                local ipid = ChooseInPlayCard(player.Idx, indicies, hint)
                AddTarget(stackEffect, FS.TargetTypes.IN_PLAY_CARD, ipid)

                return true
            end
        }
        return result
    end

    -- TODO repeated code
    -- add common effect(s)
    function result.Effect:Common(...)
        local commons = {...}
        assert(#commons > 0, 'provided 0 common effect functions in result.Effect:Common function (for ActivatedAbility)')

        result.effectGroups[#result.effectGroups+1] = commons

        return result
    end

    function result:Choose(choices)
        result.costs[#result.costs+1] = {
            Pay = function (me, player, stackEffect)
                return choices.Pay(me, player, stackEffect)
            end,
            Check = function (me, player)
                return true
            end
        }
        result.effectGroups[#result.effectGroups+1] = {choices.Effect}

        return result
    end

    return result
end

function FS.B.Reward(effectText)
    local result = FS.B._Ability(effectText)

    return result
end

function FS.B.ActivatedAbility(costText, effectText)
    local result = FS.B._Ability(effectText)

    result.builders[#result.builders+1] = function (ability)
        ability.CostText = costText
    end

    return result
end

function FS.B.TriggeredAbility(effectText)
    local result = FS.B._Ability(effectText)

    result.trigger = {}
    result.triggerLimit = -1

    result.On = {}

    result.builders[#result.builders+1] = function (ability)
        ability.Trigger = result.trigger
        ability.TriggerLimit = result.triggerLimit
    end

    function result:Limit(limit)
        result.triggerLimit = limit
        return result
    end

    function result.On:MonsterDies(check)
        result.trigger = FS.Triggers.CARD_DEATH
        check = check or function (me, player, args)
            return true
        end

        result.costs[#result.costs+1] = {
            Check = function (me, player, args)
                return check(me, player, args)
            end,
            Pay = function (me, player, stackEffect, args)
                return true
            end
        }

        return result
    end

    function result.On:ThisDies()
        return result.On:MonsterDies(function (me, player, args)
            return me.IPID == args.Card.IPID
        end)
    end

    function result.On:MonsterDamaged(check)
        result.trigger = FS.Triggers.CARD_DAMAGED

        result.costs[#result.costs+1] = {
            Check = function (me, player, args)
                return check(me, player, args)
            end,
            Pay = function (me, player, stackEffect, args)
                return true
            end
        }

        return result
    end

    function result.On:PlayerDamaged(check)
        result.trigger = FS.Triggers.PLAYED_DAMAGED

        result.costs[#result.costs+1] = {
            Check = function (me, player, args)
                return check(me, player, args)
            end,
            Pay = function (me, player, stackEffect, args)
                return true
            end
        }

        return result
    end

    function result.On:ControllerDamaged()
        return result.On:PlayerDamaged(function (me, player, args)
            return player.Idx == args.Player.Idx
        end)
    end

    function result.On:PlayerDeathBeforePenalties(check)
        check = check or function (me, player, args)
            return true
        end
        result.trigger = FS.Triggers.PLAYER_DEATH_BEFORE_PENALTIES

        result.costs[#result.costs+1] = {
            Check = function (me, player, args)
                return check(me, player, args)
            end,
            Pay = function (me, player, stackEffect, args)
                return true
            end
        }

        return result
    end

    function result.On:ControllerDeathBeforePenalties()
        return result.On:PlayerDeathBeforePenalties(function (me, player, args)
            return player.Idx == args.Player.Idx
        end)
    end

    function result.On:PlayerDeath(check)
        result.trigger = FS.Triggers.PLAYER_DEATH

        result.costs[#result.costs+1] = {
            Check = function (me, player, args)
                return check(me, player, args)
            end,
            Pay = function (me, player, stackEffect, args)
                return true
            end
        }

        return result
    end
    
    function result.On:ControllerDeath()
        return result.On:PlayerDeath(function (me, player, args)
            return player.Idx == args.Player.Idx
        end)
    end

    function result.On:Purchase(check)
        result.trigger = FS.Triggers.PURCHASE

        result.costs[#result.costs+1] = {
            Check = function (me, player, args)
                return check(me, player, args)
            end,
            Pay = function (me, player, stackEffect, args)
                return true
            end
        }

        return result
    end

    function result.On:Roll(check)
        result.trigger = FS.Triggers.ROLL

        result.costs[#result.costs+1] = {
            Check = function (me, player, args)
                return check(me, player, args)
            end,
            Pay = function (me, player, stackEffect, args)
                return true
            end
        }

        return result
    end

    function result.On:RollOfValue(value)
        return result.On:Roll(function (me, player, args)
            return args.Value == value
        end)
    end

    function result.On:SoulEnter(check)
        result.trigger = FS.Triggers.SOUL_ENTER

        result.costs[#result.costs+1] = {
            Check = function (me, player, args)
                return check(me, player, args)
            end,
            Pay = function (me, player, stackEffect, args)
                return true
            end
        }

        return result
    end

    -- TODO add filter func for which items are considered
    function result.On:EnterPlay(check)
        result.trigger = FS.Triggers.ITEM_ENTER

        result.costs[#result.costs+1] = {
            Check = function (me, player, args)
                return check(me, player, args)
            end,
            Pay = function (me, player, stackEffect, args)
                return true
            end
        }

        return result
    end

    function result.On:MeEnteringPlay()
        return result.On:EnterPlay(function (me, player, args)
            return me.IPID == args.Card.IPID
        end)
    end

    -- TODO add filter func for which items are activated
    function result.On:ItemActivation(check)
        check = check or function (me, player, args)
            return true
        end
        result.trigger = FS.Triggers.ITEM_ACTIVATION

        result.costs[#result.costs+1] = {
            Check = function (me, player, args)
                return args.Item:IsItem() and check(me, player, args)
            end,
            Pay = function (me, player, stackEffect, args)
                return true
            end
        }

        return result
    end

    function result.On:TurnEnd(check)
        check = check or function (me, player, args)
            return true
        end

        result.trigger = 'turn_end'

        result.costs[#result.costs+1] = {
            Check = function (me, player, args)
                return check(me, player, args)
            end,
            Pay = function (me, player, stackEffect, args)
                return true
            end
        }

        return result
    end

    function result.On:ControllerTurnEnd()
        return result.On:TurnEnd(function (me, player, args)
           return player.Idx == args.playerIdx
        end)
    end

    function result.On:TurnStart(check)
        check = check or function (me, player, args)
            return true
        end
        result.trigger = 'turn_start'

        result.costs[#result.costs+1] = {
            Check = function (me, player, args)
                return check(me, player, args)
            end,
            Pay = function (me, player, stackEffect, args)
                return true
            end
        }

        return result
    end

    function result.On:ControllerTurnStart()
        return result.On:TurnStart(function (me, player, args)
            return player.Idx == args.playerIdx
        end)
    end

    return result
end

FS.C.Choose = {}

function FS.C.Choose.Item(playerIdx, items, hint)
    local ipids = {}
    for _, item in ipairs(items) do
        ipids[#ipids+1] = item.IPID
    end

    local ipid = ChooseItem(playerIdx, ipids, hint)
    return ipid
end

function FS.C.Choose.YesNo(playerIdx, hint)
    return PromptString(playerIdx, {'Yes', 'No'}, hint) == 'Yes'
end

function FS.C.Choose.Effect(...)
    local choices = {...}
    local labels = {}
    local effects = {}
    for _, choice in ipairs(choices) do
        labels[#labels+1] = choice.label
        effects[#effects+1] = choice.effects
    end

    local result = {}

    function result.Pay(me, player, stackEffect)
        -- TODO more descriptive hint
        local choice = PromptString(player.Idx, labels, 'Choose')
        for idx, label in ipairs(labels) do
            if label == choice then
                stackEffect:AddChoice(idx)
                return true
            end
        end
        error('Invalid choice: '..choice)
        return true
    end

    function result.Effect(stackEffect)
        local choice = stackEffect:PopChoice()
        for _, e in ipairs(effects[choice]) do
            local executed = e(stackEffect)
            if not executed then
                break
            end
        end
    end

    return result
end

-- filters
FS.F = {}

function FS.F.Players()
    local result = {}

    result.filters = {}

    function result:Do(first)
        first = first or 0
        local res = {}
        local players = GetPlayersInTurnOrder(first)
        local filter = function (player)
            for _, f in ipairs(result.filters) do
                if not f(player) then
                    return false
                end
            end
            return true
        end
        for _, player in ipairs(players) do
            if filter(player) then
                res[#res+1] = player
            end
        end
        return res
    end

    function result:Current()
        return result:Custom(function (player)
            return player.Idx == GetCurPlayerIdx()
        end)
    end

    function result:Idx(player_idx)
        return result:Custom(function (player)
            return player.Idx == player_idx
        end)
    end

    function result:DiedThisTurn()
        return result:Custom(function (player)
            return player.IsDead
        end)
    end

    function result:Except(player_idx)
        return result:Custom(function (player)
            return player.Idx ~= player_idx
        end)
    end

    function result:CoinsGte(amount)
        return result:Custom(function (player)
            return player.Coins >= amount
        end)
    end

    function result:Alive()
        return result:Custom(function (player)
            return not player.IsDead
        end)
    end
    
    function result:Killable()
        return result:Alive()
    end

    function result:Custom(predicate)
        result.filters[#result.filters+1] = function (player)
            return predicate(player)
        end
        return result
    end

    return result
end

function FS.F.StackEffects()
    local result = {}

    result.filters = {}

    function result:Do()
        local res = {}
        local stackEffects = GetStackEffects()
        local filter = function (stackEffect)
            for _, f in ipairs(result.filters) do
                if not f(stackEffect) then
                    return false
                end
            end
            return true
        end
        for _, stackEffect in ipairs(stackEffects) do
            if filter(stackEffect) then
                res[#res+1] = stackEffect
            end
        end
        return res
    end

    function result:Rolls()
        return result:Custom(function (stackEffect)
            return IsRollStackEffect(stackEffect)
        end)
    end

    function result:AttackRolls()
        return result:Custom(function (stackEffect)
            return IsRollStackEffect(stackEffect) and stackEffect.IsAttackRoll
        end)
    end

    function result:NonAttackRolls()
        return result:Custom(function (stackEffect)
            return IsRollStackEffect(stackEffect) and not stackEffect.IsAttackRoll
        end)
    end

    function result:Custom(predicate)
        result.filters[#result.filters+1] = function (stackEffect)
            return predicate(stackEffect)
        end
        return result
    end

    return result
end

function FS.F.Items()
    local result = {}

    result.filters = {}

    function result:Do()
        local res = {}
        local items = GetItems()
        local filter = function (item)
            for _, f in ipairs(result.filters) do
                if not f(item) then
                    return false
                end
            end
            return true
        end
        for _, item in ipairs(items) do
            if filter(item) then
                res[#res+1] = item
            end
        end
        return res
    end

    function result:InShop()
        result.filters[#result.filters+1] = function (item)
            return IsShopItem(item)
        end
        return result
    end

    function result:Rechargeable()
        result.filters[#result.filters+1] = function (item)
            -- TODO some effect prohibit this
            return item.Tapped
        end
        return result
    end

    function result:Deactivatable()
        result.filters[#result.filters+1] = function (item)
            return not item.Tapped
        end
        return result
    end

    function result:NonEternal()
        result.filters[#result.filters+1] = function (item)
            return not item:HasLabel(FS.Labels.Eternal)
        end
        return result
    end

    function result:Labeled(label)
        result.filters[#result.filters+1] = function (item)
            return item:HasLabel(label)
        end
        return result
    end

    function result:ControlledByPlayer()
        result.filters[#result.filters+1] = function (item)
            return IsOwned(item)
        end

        return result
    end

    function result:Rerollable()
        return result:NonEternal()
    end

    function result:Destructable()
        return result:NonEternal()
    end

    function result:Except(ipid)
        result.filters[#result.filters+1] = function (item)
            return item.IPID ~= ipid
        end
        return result
    end

    function result:NotControlledBy(idx)
        result.filters[#result.filters+1] = function (item)
            if item.Owner == nil then
                return false
            end
            return item.Owner.Idx ~= idx
        end
        return result
    end

    function result:ControlledBy(idx)
        result.filters[#result.filters+1] = function (item)
            -- TODO assert that is owned item
            if item.Owner == nil then
                return false
            end
            return item.Owner.Idx == idx
        end
        return result
    end

    return result
end

function FS.F.Characters()
    local result = {}

    result.filters = {}

    function result:Do()
        local res = {}
        local characters = {}
        for _, player in ipairs(GetPlayers()) do
            characters[#characters+1] = player.Character
        end
        local filter = function (item)
            for _, f in ipairs(result.filters) do
                if not f(item) then
                    return false
                end
            end
            return true
        end
        for _, item in ipairs(characters) do
            if filter(item) then
                res[#res+1] = item
            end
        end
        return res
    end

    return result
end

function FS.F.Monsters()
    local result = {}

    result.filters = {}

    function result:Do()
        local res = {}
        local monsters = GetMonsters()
        local filter = function (item)
            for _, f in ipairs(result.filters) do
                if not f(item) then
                    return false
                end
            end
            return true
        end
        for _, item in ipairs(monsters) do
            if filter(item) then
                res[#res+1] = item
            end
        end
        return res
    end

    function result:Custom(predicate)
        result.filters[#result.filters+1] = function (monster)
            return predicate(monster)
        end
        return result
    end

    function result:IPID(ipid)
        return result:Custom(function (monster)
            return monster.IPID == ipid
        end)
    end

    return result
end

FS.C.CurrentPlayers = function (...)
    return FS.F.Players():Current():Do()
end

FS.C.RollOwner = function (stackEffect, args)    
    return FS.F.Players():Idx(args.Player.Idx):Do()
end

FS.C.AllPlayers = function (...)
    return FS.F.Players():Do()
end

FS.C.ActivationOwner = function (stackEffect, args)
    return FS.F.Players():Idx(args.Player.Idx):Do()
end

FS.C.AllMonsters = function (...)
    return FS.F.Monsters():Do()
end