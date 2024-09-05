-- status: not tested

function _Create()
    return FS.B.Loot('Toll:\n1-2: All players take 1 damage.\n3-4: All monsterss take 1 damage.\n5-6: All players take 2 damage.')
        .Effect:Roll(
            FS.C.Effect.SwitchRoll(0, {
                [1] = FS.C.Effect.DamageToPlayer(1, FS.C.AllPlayers),
                [2] = FS.C.Effect.DamageToPlayer(1, FS.C.AllPlayers),
                [3] = FS.C.Effect.DamageToMonster(1, FS.C.AllPlayers),
                [4] = FS.C.Effect.DamageToMonster(1, FS.C.AllPlayers),
                [5] = FS.C.Effect.DamageToPlayer(2, FS.C.AllPlayers),
                [6] = FS.C.Effect.DamageToPlayer(2, FS.C.AllPlayers),
            })
        )
    :Build()
end