-- status: implemented

function _Create()
    -- Gain 3{cent}.
    return FS.B.Loot()
        .Effect:Common(
            FS.C.Effect.GainCoins(3)
        )
    :Build()
end