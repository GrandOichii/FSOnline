-- status: not tested

function _Create()
    return FS.B.Item()
        :ActivatedAbility(
            FS.B.ActivatedAbility('Pay 3{cent}', 'Play an additional loot card this turn.')
                .Cost:Common(
                    FS.C.Cost.PayCoins(3)
                )
                .Effect:Common(
                    FS.C.Effect.AddLootPlay(1)
                )
            :Build()
        )
        :ActivatedAbility(
            FS.B.ActivatedAbility('Pay 4{cent}', 'Loot 1.')
                .Cost:Common(
                    FS.C.Cost.PayCoins(4)
                )
                .Effect:Common(
                    FS.C.Effect.Loot(1)
                )
            :Build()
        )
        :ActivatedAbility(
            FS.B.ActivatedAbility('Pay 6{cent}', 'Deal 1 damage to a monster or player.')
                .Cost:Common(
                    FS.C.Cost.PayCoins(6)
                )
                .Target:MonsterOrPlayer()
                .Effect:Common(
                    FS.C.Effect.DamageToTarget(0, 1)
                )
            :Build()
        )
        :Label(FS.Labels.Eternal)
    :Build()
end