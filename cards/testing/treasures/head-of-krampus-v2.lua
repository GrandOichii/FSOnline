-- status: implemented

function _Create()
    return FS.B.Item()
        :ActivatedAbility(
            FS.B.ActivatedAbility('{T}', 'Roll-\n1-3: Deal 1 damage to each player.\n4-6: Deal 1 damage to each monster.')
                .Cost:Common(
                    FS.C.Cost:Tap()
                )
                .Effect:Roll(
                    FS.C.Effect.SwitchRoll(0, {
                        [1] = FS.C.Effect.DamageToPlayer(1, FS.C.AllPlayers),
                        [2] = FS.C.Effect.DamageToPlayer(1, FS.C.AllPlayers),
                        [3] = FS.C.Effect.DamageToPlayer(1, FS.C.AllPlayers),
                        [4] = FS.C.Effect.DamageToMonster(1, FS.C.AllMonsters),
                        [5] = FS.C.Effect.DamageToMonster(1, FS.C.AllMonsters),
                        [6] = FS.C.Effect.DamageToMonster(1, FS.C.AllMonsters),
                    })
                )
            :Build()
        )
    :Build()
end