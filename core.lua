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
    COIN_GAIN_AMOUNT = 1
}

-- common
FS.C = {}

-- common effects
FS.C.Effect = {}

function FS.C.Effect.PutGenericCountersOnMe(amount)
    return function (stackEffect)
        assert(IsAbilityActivation(stackEffect), 'Provided a non-ability-activation stack effect for FS.C.Effect.PutGenericCountersOnMe')

        local card = stackEffect.Card
        PutGenericCounters(card.IPID, amount)
    end
end

function FS.C.Effect.GainCoins(amount)
    return function (stackEffect)
        AddCoins(stackEffect.OwnerIdx, amount)
        return true
    end
end

function FS.C.Effect.AddLootPlay(amount)
    return function (stackEffect)
        AddLootPlay(stackEffect.OwnerIdx, amount)
        return true
    end
end

function FS.C.Effect.RerollTargetRoll(target_idx)
    return function (stackEffect)
        local effect = GetStackEffect(stackEffect.Targets[target_idx].Value)

        assert(IsRollStackEffect(effect), 'Provided a non-roll target stack effect for FS.C.Effect.ModifyTargetRoll')

        RerollDice(effect)
    end
end

function FS.C.Effect.RechargeTarget(target_idx)
    return function (stackEffect)
        local ipid = stackEffect.Targets[target_idx].Value

        Recharge(ipid)
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

function FS.C.Effect.GainTreasure(amount)
    return function (stackEffect)
        GainTreasure(stackEffect.OwnerIdx, amount)
        return true
    end
end

function FS.C.Effect.Loot(amount)
    return function (stackEffect)
        LootCards(stackEffect.OwnerIdx, amount, stackEffect)
        return true
    end
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
    result.labels = {}
    result.stateModifiers = {}
    result.lootCosts = {}
    result.lootChecks = {}

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

        -- activate abilities
        card.ActivatedAbilities = result.activatedAbilities

        -- labels
        card.Labels = result.labels

        -- state modifiers
        card.StateModifiers = result.stateModifiers

        -- additional loot costs
        card.LootCosts = result.lootCosts

        -- addditional loot checks
        card.LootChecks = result.lootChecks

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

    function result.Target:StackEffect(filterFunc, hint)
        hint = hint or 'Choose stack effect'
        -- TODO
        result.lootChecks[#result.lootChecks+1] = function (player)
            return #filterFunc(player) > 0
        end

        result.lootCosts[#result.lootCosts+1] = function (stackEffect)
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

        result.lootChecks[#result.lootChecks+1] = function (player)
            return #filterFunc(player) > 0
        end

        result.lootCosts[#result.lootCosts+1] = function (stackEffect)
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
        return self
    end

    -- add label to card
    function result:Label(label)
        result.labels[#result.labels+1] = label
        return self
    end

    result.State = {}
    function result.State:Raw(layer, func)
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

function FS.B.ActivatedAbility(costText, effectText)
    local result = {}

    result.costs = {}
    result.effectGroups = {}

    result.Cost = {}
    result.Effect = {}

    -- build activated ability
    function result:Build()
        -- TODO repeated code
        local effects = {}
        for _, group in ipairs(self.effectGroups) do
            effects[#effects+1] = function (stackEffect)
                for _, e in ipairs(group) do
                    if not e(stackEffect) then
                        return
                    end
                end
            end
        end

        return {
            EffectText = effectText,
            CostText = costText,
            Check = function (me, player)
                for _, cost in ipairs(result.costs) do
                    if not cost.Check(me, player) then
                        return false
                    end
                end
                return true
            end,
            Cost = function (me, player, stackEffect)
                for _, cost in ipairs(result.costs) do
                    if not cost.Pay(me, player, stackEffect) then
                        return false
                    end
                end
                return true
            end,
            Effects = effects
        }
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

    function result.Target:Player(filterFunc, hint)
        hint = hint or 'Choose a player'
        result.costs[#result.costs+1] = {
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


    -- function result.Target:Item(filterFunc, hint)
    --     hint = hint or 'Choose an Item'

    --     result.lootChecks[#result.lootChecks+1] = function (player)
    --         return #filterFunc(player) > 0
    --     end

    --     result.lootCosts[#result.lootCosts+1] = function (stackEffect)
    --         local player = GetPlayer(stackEffect.OwnerIdx)
    --         local options = filterFunc(player)
    --         local ipids = {}
    --         for _, item in ipairs(options) do
    --             ipids[#ipids+1] = item.IPID
    --         end


    --         -- TODO add optional
    --         local ipid = ChooseItem(player.Idx, ipids, hint)
    --         AddTarget(stackEffect, FS.TargetTypes.ITEM, ipid)

    --         return true
    --     end

    --     return result
    -- end
    function result.Target:Item(filterFunc, hint)
        hint = hint or 'Choose an item'
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

FS.C.Choose = {}

function FS.C.Choose.YesNo(playerIdx, hint)
    return PromptString(playerIdx, {'Yes', 'No'}, hint) == 'Yes'
end

-- filters
FS.F = {}

function FS.F.Players()
    local result = {}

    result.filters = {}

    function result:Do()
        local res = {}
        local players = GetPlayers()
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
