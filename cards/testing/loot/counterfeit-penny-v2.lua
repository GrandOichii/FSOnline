-- status: implemented

function _Create()
    return FS.B.Loot()
        :Trinket()
        .State:Raw(
            FS.ModLayers.COIN_GAIN_AMOUNT,
            function (me)
                local pState = me.Owner.State
                pState.CoinGainModifiers:Add(function (player, amount)
                    return amount + 1
                end)
            end
        )
    :Build()
end