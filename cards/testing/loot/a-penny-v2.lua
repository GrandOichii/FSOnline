-- status: implemented

function _Create()
    -- Gain 1{cent}.
    return FS.B.Loot()
        .Effect:Common(
            FS.C.Effect.GainCoins(1)
        )
    :Build()
end