-- status: implemented

function _Create()
    return FS.B.Loot('Gain 5{cent}.')
        .Effect:Common(
            FS.C.Effect.GainCoins(5)
        )
    :Build()
end