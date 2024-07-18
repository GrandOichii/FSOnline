-- status: implemented

function _Create()
    -- Gain +1 treasure.
    return FS.B.Loot()
        .Effect:Common(
            FS.C.Effect.GainTreasure(3)
        )
    :Build()
end