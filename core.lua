-- core object
FS = {}

FS.TargetTypes = {
    PLAYER = 0,
    STACK_EFFECT = 1,
    ITEM = 2,
}

-- card labels
FS.Labels = {
    Eternal = 'Eternal',
    Trinket = 'Trinket'
}

-- modification layers
FS.ModLayers = {
    COIN_GAIN_AMOUNT = 1,
    ROLL_REPLACEMENT_EFFECTS = 2,
    LOOT_AMOUNT = 3,
}

-- triggers
FS.Triggers = {
    ROLL = 'roll'
}

-- common
FS.C = {}

-- common effects
FS.C.Effect = {}

function FS.C.Effect._ApplyToPlayer(effect, filterFunc)
    filterFunc = filterFunc or function (stackEffect)
        return FS.F.Players():Idx(stackEffect.OwnerIdx):Do()
    end
    return function (stackEffect)
        -- TODO just stops mid resolving?
        local players = filterFunc(stackEffect)
        for _, player in ipairs(players) do
            if not effect(player, stackEffect) then
                return false
            end
        end
        return true
    end
end

function FS.C.Effect.PutGenericCountersOnMe(amount)
    return function (stackEffect)
        assert(IsAbilityActivation(stackEffect), 'Provided a non-ability-activation stack effect for FS.C.Effect.PutGenericCountersOnMe')

        local card = stackEffect.Card
        PutGenericCounters(card.IPID, amount)
    end
end

function FS.C.Effect.GainCoins(amount, filterFunc)
    return FS.C.Effect._ApplyToPlayer(function (player, stackEffect)
        AddCoins(player.Idx, amount)
        return true
    end, filterFunc)
end

function FS.C.Effect.AddLootPlay(amount, filterFunc)
    return FS.C.Effect._ApplyToPlayer(function (player, stackEffect)
        AddLootPlay(player.Idx, amount)
        return true
    end, filterFunc)
end

function FS.C.Effect.RerollTargetRoll(target_idx)
    return function (stackEffect)
        local effect = GetStackEffect(stackEffect.Targets[target_idx].Value)

        assert(IsRollStackEffect(effect), 'Provided a non-roll target stack effect for FS.C.Effect.ModifyTargetRoll')

        RerollDice(effect)
    end
end

function FS.C.Effect.SetTargetRoll(target_idx, value)
    return function (stackEffect)
        local effect = GetStackEffect(stackEffect.Targets[target_idx].Value)

        assert(IsRollStackEffect(effect), 'Provided a non-roll target stack effect for FS.C.Effect.ModifyTargetRoll')

        SetRollValue(effect, value)
    end
end

function FS.C.Effect.RechargeTarget(target_idx)
    return function (stackEffect)
        local ipid = stackEffect.Targets[target_idx].Value

        Recharge(ipid)
    end
end

function FS.C.Effect.RechargeMe()
    return function (stackEffect)
        assert(IsAbilityActivation(stackEffect) or IsTrigger(stackEffect), 'Provided a non-ability stack effect for FS.C.Effect.RechargeMe')

        local card = stackEffect.Card
        Recharge(card.IPID)
    end
end

function FS.C.Effect.RerollTargetItem(target_idx)
    return function (stackEffect)
        local ipid = stackEffect.Targets[target_idx].Value

        -- TODO fizzle if no targets left
        if GetItemOrDefault(ipid) == nil then
            return
        end


        RerollItem(ipid)
    end
end

function FS.C.Effect.StealTargetItem(target_idx)
    return function (stackEffect)
        local ipid = stackEffect.Targets[target_idx].Value

        -- TODO fizzle if no targets left
        if GetItemOrDefault(ipid) == nil then
            return
        end

        StealItem(stackEffect.OwnerIdx, ipid)
    end
end

function FS.C.Effect.ModifyTargetRoll(target_idx, options, hint)
    hint = hint or 'Choose roll modifier'
    return function (stackEffect)
        local rollStackEffect = GetStackEffect(stackEffect.Targets[target_idx].Value)

        assert(IsRollStackEffect(rollStackEffect), 'Provided a non-roll target stack effect for FS.C.Effect.ModifyTargetRoll')

        local rolled = rollStackEffect.Value
        local optionsIndex = {}
        local optionsT = {}
        for _, pair in ipairs(options) do
            if rolled + pair.mod >= 1 and rolled + pair.mod <= 6 then
                optionsT[#optionsT+1] = pair.option
                optionsIndex[pair.option] = pair.mod
            end
        end

        assert(#optionsT > 0, 'Provided invalid options table for FS.C.Effect.ModifyTargetRoll (optionsT table is empty)')

        local choice = optionsT[1]
        if #optionsT > 1 then
            choice = PromptString(stackEffect.OwnerIdx, optionsT, hint)
        end

        local mod = optionsIndex[choice]
        ModRoll(rollStackEffect, mod)
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

-- TODO change hint to hintFunc
function FS.C.Effect.Discard(amount, hint)
    hint = hint or 'Choose a card to discard'
    return function (stackEffect)
        -- TODO could be better
        for di = 0, amount - 1 do
            local player_idx = stackEffect.OwnerIdx
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
    end
end

-- common costs (for activated abilities)
FS.C.Cost = {}

function FS.C.Cost.Tap()
    local result = {}

    function result.Pay(me, player, stackEffect)
        TapCard(me.IPID)
        return true
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
        -- TODO check if can be destroyed (is eternal)
        return true
    end

    return result
end

-- TODO replacement effects affect how much is required to pay
function FS.C.Cost.PayCoins(amount)
    local result = {}

    function result.Pay(me, player, stackEffect)
        -- TODO ask permission to pay
        PayCoins(player.Idx, amount)
        return true
    end

    function result.Check(me, player)
        return player.Coins >= amount
    end

    return result
end

-- builders
FS.B = {}

function FS.B.Card()
    local result = {}

    result.effectGroups = {}
    result.activatedAbilities = {}
    result.triggeredAbilities = {}
    result.labels = {}
    result.stateModifiers = {}

    result.lootCosts = {}
    result.lootChecks = {}
    result.lootTargets = {}
    result.lootFizzleChecks = {}

    function result:Build()
        local card = {}

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

        -- labels
        card.Labels = result.labels

        -- state modifiers
        card.StateModifiers = result.stateModifiers

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

    result.Effect = {}

    -- add common effect(s) (these are considered "and" effects - if one fails, the rest fill not be executed)
    function result.Effect:Common(...)
        local commons = {...}
        assert(#commons > 0, 'provided 0 common effect functions in result.Effect:Common function (for Loot)')

        result.effectGroups[#result.effectGroups+1] = commons

        return result
    end

    function result.Effect:Roll(effect)
        result.lootCosts[#result.lootCosts+1] = function (stackEffect)
            Roll(stackEffect)
            return true
        end

        result.effectGroups[#result.effectGroups+1] = {effect}
        return result
    end

    result.Target = {}

    function result.Target._AddFizzleCheck(filterFunc, extractFunc)
        local targetIdx = #result.lootTargets

        result.lootFizzleChecks[#result.lootFizzleChecks+1] = function (stackEffect)
            local target = stackEffect.Targets[targetIdx].Value
            -- TODO kinda sketchy
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
        end)
        result.Target._AddLootCheck(filterFunc)

        result.lootTargets[#result.lootTargets+1] = function (stackEffect)
            local player = GetPlayer(stackEffect.OwnerIdx)
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
        end)
        result.Target._AddLootCheck(filterFunc)

        result.lootTargets[#result.lootTargets+1] = function (stackEffect)
            local player = GetPlayer(stackEffect.OwnerIdx)
            local options = filterFunc(player)
            local ipids = {}
            for _, item in ipairs(options) do
                ipids[#ipids+1] = item.IPID
            end


            -- TODO add optional
            local ipid = ChooseItem(player.Idx, ipids, hint)
            AddTarget(stackEffect, FS.TargetTypes.ITEM, ipid)

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

    -- add label to card
    function result:Label(label)
        result.labels[#result.labels+1] = label
        return self
    end

    result.Static = {}
    function result.Static:Raw(layer, func)
        if result.stateModifiers[layer] == nil then
            result.stateModifiers[layer] = {}
        end
        local t = result.stateModifiers[layer]
        t[#t+1] = func
        return result
    end

    return result
end

-- loot card builder
function FS.B.Loot()
    local result = FS.B.Card()

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

    return result
end

-- item builder
function FS.B.Item()
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

    function result.Target._AddFizzleCheck(filterFunc, extractFunc)
        local targetIdx = #result.targets

        result.fizzleChecks[#result.fizzleChecks+1] = function (stackEffect)
            local target = stackEffect.Targets[targetIdx].Value
            local options = filterFunc(stackEffect.Card, GetPlayer(stackEffect.OwnerIdx))
            for _, o in ipairs(options) do
                if extractFunc(o) == target then
                    return true
                end
            end
            return false
        end

        return result
    end

    function result.Target:Player(filterFunc, hint)
        hint = hint or 'Choose a player'
        result.Target._AddFizzleCheck(filterFunc, function (player)
            return tostring(player.Idx)
        end)

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
        end)

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
                AddTarget(stackEffect, FS.TargetTypes.ITEM, ipid)
                return true
            end
        }
        return result
    end

    function result.Target:StackEffect(filterFunc, hint)
        hint = hint or 'Choose a stack effect'
        result.Target._AddFizzleCheck(filterFunc, function (effect)
            return effect.SID
        end)

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

    -- TODO repeated code
    -- add common effect(s)
    function result.Effect:Common(...)
        local commons = {...}
        assert(#commons > 0, 'provided 0 common effect functions in result.Effect:Common function (for ActivatedAbility)')

        result.effectGroups[#result.effectGroups+1] = commons

        return result
    end

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

    result.On = {}

    result.builders[#result.builders+1] = function (ability)
        ability.Trigger = result.trigger
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

    -- TODO add player filter
    function result.On:TurnEnd()
        result.trigger = 'turn_end'

        result.costs[#result.costs+1] = {
            Check = function (me, player, args)
                return args.playerIdx == player.Idx
            end,
            Pay = function (me, player, stackEffect, args)
                return true
            end
        }

        return result
    end

    return result
end

FS.C.Choose = {}

function FS.C.Choose.YesNo(playerIdx, hint)
    return PromptString(playerIdx, {'Yes', 'No'}, hint) == 'Yes'
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

    function result:Idx(player_idx)
        result.filters[#result.filters+1] = function (player)
            return player.Idx == player_idx
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
        result.filters[#result.filters+1] = function (stackEffect)
            return IsRollStackEffect(stackEffect)
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
        local stackEffects = GetItems()
        local filter = function (item)
            for _, f in ipairs(result.filters) do
                if not f(item) then
                    return false
                end
            end
            return true
        end
        for _, item in ipairs(stackEffects) do
            if filter(item) then
                res[#res+1] = item
            end
        end
        return res
    end

    function result:Rechargeable()
        result.filters[#result.filters+1] = function (item)
            -- TODO some effect prohibit this
            return item.Tapped
        end
        return result
    end

    function result:NonEternal()
        result.filters[#result.filters+1] = function (item)
            return not item:HasLabel(FS.Labels.Eternal)
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

    function result:Except(ipid)
        result.filters[#result.filters+1] = function (item)
            return item.IPID ~= ipid
        end
        return result
    end

    return result
end
