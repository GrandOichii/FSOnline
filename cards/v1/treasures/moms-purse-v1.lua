-- status: implemented
-- TODO too low-level

function _Create()
    return FS.B.Loot()
        .Static:Raw(
            FS.ModLayers.LOOT_AMOUNT,
            function (me)
                local pState = me.Owner.State
                pState.LootAmountModifiers:Add(function (player, amount, reason)
                    if reason.type ~= 'loot_phase' then
                        return amount
                    end
                    return amount + 1
                end)
            end
        )
    :Build()
end