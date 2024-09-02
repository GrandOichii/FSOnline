-- status: implemented

function _Create()
    return FS.B.Loot()
        :Trinket()
        .Static:Raw(
            FS.ModLayers.COIN_GAIN_AMOUNT,
            'If you would gain any number of {cent}, gain that much +1{cent} instead.',
            function (me)
                local pState = me.Owner.State
                pState.CoinGainModifiers:Add(function (player, amount)
                    return amount + 1
                end)
            end
        )
    :Build()
end