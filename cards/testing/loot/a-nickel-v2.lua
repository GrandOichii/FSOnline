-- status: implemented

function _Create()
    -- Gain 5{cent}
    return FS.B.Loot()
        .Effect:Common(
            FS.C.Effect.GainCoins(5)
        )
    :Build()
end