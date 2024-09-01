-- status: not tested

function _Create()
    return FS.B.Event('Roll-\n1: Take 3 damage.\n2-3: Discard 2 loot cards.\n4-5: Gain 7{cent}.\n6: Gain +1 Treasure.')
        .Effect:Roll(
            FS.C.Effect.SwitchRoll(0, {
                [1] = FS.C.Effect.DamageToPlayer(3),
                [2] = FS.C.Effect.Discard(2),
                [3] = FS.C.Effect.Discard(2),
                [4] = FS.C.Effect.GainCoins(7),
                [5] = FS.C.Effect.GainCoins(7),
                [6] = FS.C.Effect.GainTreasure(1),
            })
        )
    :Build()
end