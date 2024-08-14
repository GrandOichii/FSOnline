-- status: implemented

function _Create()
    -- Gain 10{cent}
    return FS.B.Loot()
        .Effect:Common(
            FS.C.Effect.GainCoins(10)
        )
    :Build()
end