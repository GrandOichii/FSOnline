-- status: implemented
-- TODO this works differently than v2

function _Create()
    return FS.B.Item()
        .Static:Raw(
            FS.ModLayers.ROLL_REPLACEMENT_EFFECTS,
            'When anyone rolls a {roll:1}, you may force that player to reroll it.',
            function (me)
                local players = GetPlayers()
                for _, player in ipairs(players) do
                    player.State.RollReplacementEffects:Add(function (rollStackEffect)
                        local roll = rollStackEffect.Value
                        if roll ~= 1 then
                            return true
                        end
                        local accept = FS.C.Choose.YesNo(me.Owner.Idx, 'Player '..player.Name..' rolled a '..roll..'. Force them to reroll?')
                        if not accept then
                            return true
                        end
                        RerollDice(rollStackEffect)
                        return false
                    end)
                end
            end
        )
    :Build()
end