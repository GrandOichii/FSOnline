-- status: implemented
-- TODO too low-level

function _Create()
    return FS.B.Item()
        -- Damage you would take is reduced to 1.
        .Static:Raw(
            FS.ModLayers.DAMAGE_RECEIVED_MODIFICATORS,
            function (me)
                me.Owner.Stats.State.ReceivedDamageModifiers:Add(
                    function (amount, sourceEffect)
                        return 1
                    end
                )
            end
        )
    :Build()
end