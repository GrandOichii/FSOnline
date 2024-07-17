-- core object
FS = {}

-- card labels
FS.Labels = {
    Eternal = 'Eternal'
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

    function result.Pay(me, player)
        TapCard(me.IPID)
        return true
    end

    function result.Check(me, player)
        return not me.Tapped
    end

    return result
end

-- builders
FS.B = {}

function FS.B.Card()
    local result = {}

    -- TODO add labels

    result.effectGroups = {}
    result.activatedAbilities = {}

    function result:PreBuild()
        return {
            Effects = {},
            ActivatedAbilities = {},
        }
    end

    return result
end

-- loot card builder
function FS.B.Loot()
    local result = FS.B.Card()

    result.Effect = {}

    -- add common effect(s) (these are considered "and" effects - if one fails, the rest fill not be executed)
    function result.Effect:Common(...)
        local commons = {...}
        assert(#commons > 0, 'provided 0 common effect functions in result.Effect:Common function (for Loot)')

        result.effectGroups[#result.effectGroups+1] = commons

        return result
    end

    function result.Effect:Custom(...)
        -- TODO

        return self
    end

    function result:Build()
        local card = result:PreBuild()

        -- effects
        for _, group in ipairs(self.effectGroups) do
            card.Effects[#card.Effects+1] = function (stackEffect)
                for _, e in ipairs(group) do
                    if not e(stackEffect) then
                        return
                    end
                end
            end
        end

        return card
    end

    return result
end

-- character builder
function FS.B.Character()
    local result = FS.B.Item()

    function result:Basic()
        result:ActivatedAbility(
            FS.B.ActivatedAbility('{T}: Play an additional Loot card this turn.')
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

    -- build item card
    function result:Build()
        local item = result:PreBuild()

        item.ActivatedAbilities = result.activatedAbilities

        return item
    end

    -- add activated ability
    function result:ActivatedAbility(aa)
        result.activatedAbilities[#result.activatedAbilities+1] = aa
        return self
    end

    -- -- add label to card
    -- function result:Label(label)
    --     -- TODO add label to result
    --     return self
    -- end

    return result
end

function FS.B.ActivatedAbility(effectText)
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
            Text = effectText,
            Check = function (me, player)
                for _, cost in ipairs(result.costs) do
                    if not cost.Check(me, player) then
                        return false
                    end
                end
                return true
            end,
            Cost = function (me, player)
                for _, cost in ipairs(result.costs) do
                    if not cost.Pay(me, player) then
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
    -- add common effect(s)
    function result.Effect:Common(...)
        local commons = {...}
        assert(#commons > 0, 'provided 0 common effect functions in result.Effect:Common function (for ActivatedAbility)')

        result.effectGroups[#result.effectGroups+1] = commons

        return result
    end

    return result
end