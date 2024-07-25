-- status: not tested

function _Create()
    -- Roll-
    -- 1: Gain 1{cent}.
    -- 2: Take 2 damage.
    -- 3: Loot 3.
    -- 4: Lose 4{cent}. 5: Gain 5{cent}.
    -- 6: Gain +1 Treasure.

    return FS.B.Loot()
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