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
    local result = {}

    function result.Effect(stackEffect)
        AddCoins(stackEffect.OwnerIdx, amount)
        return true
    end

    return result
end

function FS.C.Effect.GainTreasure(amount)
    local result = {}

    function result.Effect(stackEffect)
        -- TODO implement in ScriptMaster
        GainTreasure(stackEffect.OwnerIdx, amount)
        return true
    end

    return result
end

function FS.C.Effect.Loot(amount)
    local result = {}

    function result.Effect(stackEffect)
        -- TODO implement in ScriptMaster
        LootCards(stackEffect.OwnerIdx, amount, stackEffect)
        return true
    end

    return result
end

function FS.C.Effect.Discard(amount)
    local result = {}

    function result.Effect(stackEffect)
        -- TODO

        return true
    end

    return result
end

-- common costs (for activated abilities)
FS.C.Cost = {}

function FS.C.Cost.Tap()
    local result = {}

    function result:Pay(me)
        -- TODO tap card
        return true
    end

    function result:Check(me)
        -- TODO check if card is tapped
        return true
    end

    return result
end

-- builders
FS.B = {}

-- loot card builder
function FS.B.Loot()
    local result = {}

    result.effectGroups = {}

    result.Effect = {}

    -- add common effect(s) (these are considered "and" effects - if one fails, the rest fill not be executed)
    function result.Effect:Common(...)
        local commons = {...}
        assert(#commons > 0, 'provided 0 common effect functions in result.Effect:Common function')

        local group = {}
        for _, common in ipairs(commons) do
            -- effects
            group[#group+1] = common.Effect
            -- costs
            -- TODO
            -- targets
            -- TODO
        end

        result.effectGroups[#result.effectGroups+1] = group

        return result
    end

    function result.Effect:Custom(...)
        -- TODO

        return self
    end

    function result:Build()
        local card = {}

        local e = {
            effect = function (stackEffect)
                local execGroup = function (group)
                    for _, e in ipairs(group) do
                        if not e(stackEffect) then
                            return
                        end
                    end
                end
                for _, group in ipairs(self.effectGroups) do
                    execGroup(group)
                end
            end
        }
        card.Effects = {e}

        return card
    end

    return result
end

-- character builder
function FS.B.Character()
    local result = {}

    function result:Build()
        -- TODO construct character card and return
    end

    function result:Basic()
        -- TODO add basic tap ability to play an additional loot card
        return self
    end

    return result
end

-- item builder
function FS.B.Item()
    local result = {}

    -- build item card
    function result:Build()
        -- TODO construct item object and return
    end

    -- add activated ability
    function result:ActivatedAbility(aa)
        -- TODO add activated ability to result
        return self
    end

    -- add label to card
    function result:Label(label)
        -- TODO add label to result
        return self
    end

    return result
end

function FS.B.ActivatedAbility(effectText)
    -- TODO use effectText
    local result = {}

    -- build activated ability
    function result:Build()
        -- TODO construct activated ability and return
    end

    -- add common cost(s)
    function result.Cost:Common(...)
        local costs = {...}
        -- TODO add costs

        return self
    end

    -- add common effect(s)
    function result.Effect:Common(...)
        local effects = ...
        -- TODO add effects

        return self
    end

    return result
end