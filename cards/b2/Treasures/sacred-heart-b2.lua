-- status: not tested
-- TODO too low-level

function _Create()
    return FS.B.Item()
        .Static:Raw(
            FS.ModLayers.ROLL_REPLACEMENT_EFFECTS,
            'When you would roll a 1, you may change the result to a 6.',
            function (me)
                local player = me.Owner
                local replacement = 6
                player.State.RollReplacementEffects:Add(function (rollStackEffect)
                    local roll = rollStackEffect.Value
                    if roll ~= 1 then
                        return true
                    end
                    local accept = FS.C.Choose.YesNo(me.Owner.Idx, 'You rolled a '..roll..'. Set to '..replacement..'?')
                    if not accept then
                        return true
                    end
                    SetRollValue(rollStackEffect, 6)

                    -- TODO? should this stop the chain
                    return true
                end)
            end
        )
    :Build()
end