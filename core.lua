-- core object
FS = {}

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

function FS.C.Effect.GainTreasure(amount)
    return function (stackEffect)
        -- TODO implement in ScriptMaster
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

function FS.C.Effect.Discard(amount)
    return function (stackEffect)
        -- TODO

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

function FS.C.Cost.DestroyMe()
    local result = {}

    function result.Pay(me, player, stackEffect)
        -- TODO ask player for permission
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