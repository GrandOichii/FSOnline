-- status: implemented

function _Create()
    -- Gain 2{cent}.
    return FS.B.Loot()
        .Effect:Common(
            FS.C.Effect.GainCoins(2)
        )
    :Build()
end