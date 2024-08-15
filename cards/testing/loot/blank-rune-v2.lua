-- status: implemented

function _Create()
    local ff = function (stackEffect)
        return FS.F.Players():Do(stackEffect.OwnerIdx)
    end

    return FS.B.Loot('Roll-\n1: Each player gains 1{cent}.\n2: Each player loots 2.\n3: Each player takes 3 damage.\n4: Each player gains 4{cent}.\n5: Each player loots 5.\n6: Each player gains 6{cent}.')
        .Effect:Roll(FS.C.Effect.SwitchRoll(0,
        {
            [1] = FS.C.Effect.GainCoins(1, ff),
            [2] = FS.C.Effect.Loot(2, ff),
            [3] = FS.C.Effect.DamageToPlayer(3, ff),
            [4] = FS.C.Effect.GainCoins(4, ff),
            [5] = FS.C.Effect.Loot(5, ff),
            [6] = FS.C.Effect.GainCoins(6, ff),
        }))
    :Build()
end