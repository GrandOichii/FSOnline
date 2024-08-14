-- status: implemented

function _Create()
    -- Gain 4{cent}.
    return FS.B.Loot()
        .Effect:Common(
            FS.C.Effect.GainCoins(4)
        )
    :Build()
end