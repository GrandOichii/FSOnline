-- status: implemented

function _Create()
    return FS.B.Loot('Each player gains +1 Treasure, then you gain +1 Treasure.')
        .Effect:Common(
            FS.C.Effect.GainTreasure(1, function (stackEffect)
                return FS.F.Players():Do(stackEffect.OwnerIdx)
            end)
        )
        .Effect:Common(
            FS.C.Effect.GainTreasure(1)
        )
    :Build()
end