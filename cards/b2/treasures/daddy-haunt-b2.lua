-- status: not tested
-- TODO too low-level

function _Create()
    return FS.B.Item()
        -- If you would take any amount of damage, take that much damage +1 instead.
        .Static:Raw(
            FS.ModLayers.DAMAGE_RECEIVED_MODIFICATORS,
            function (me)
                me.Owner.Stats.State.ReceivedDamageModifiers:Add(
                    function (amount, sourceEffect)
                        return amount + 1
                    end
                )
            end
        )
        :Haunt()
    :Build()
end