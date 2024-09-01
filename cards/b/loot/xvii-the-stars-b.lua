-- status: implemented

function _Create()
    return FS.B.Loot('Gain +1 treasure.')
        .Effect:Common(
            FS.C.Effect.GainTreasure(1)
        )
    :Build()
end