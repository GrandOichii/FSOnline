-- status: not tested

function _Create()
    return FS.B.Loot('Roll-\n1: Gain 1{cent}.\n2: Take 2 damage.\n3: Loot 3.\n4: Lose 4{cent}. 5: Gain 5{cent}.\n6: Gain +1 Treasure.')
        .Effect:Roll(FS.C.Effect.SwitchRoll(0,
        {
            [1] = FS.C.Effect.GainCoins(1),
            [2] = FS.C.Effect.DamageToPlayer(2),
            [3] = FS.C.Effect.Loot(3),
            [4] = FS.C.Effect.LoseCoins(4),
            [5] = FS.C.Effect.GainCoins(5),
            [6] = FS.C.Effect.GainTreasure(1),
        }))
    :Build()
end