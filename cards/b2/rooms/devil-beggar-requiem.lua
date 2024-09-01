-- status: implemented

function _Create()
    return FS.B.Room()
        :ActivatedAbility(
            FS.B.ActivatedAbility('Pay 1{health}', 'Roll-\n1-2: Loot 2.\n3-4: Loot 1.\n5-6: Take 1 damage.')
                .Cost:Common(
                    FS.C.Cost.PayHealth(1)
                )
                .Effect:Roll(
                    FS.C.Effect.SwitchRoll(0, {
                        [1] = FS.C.Effect.Loot(2),
                        [2] = FS.C.Effect.Loot(2),
                        [3] = FS.C.Effect.Loot(1),
                        [4] = FS.C.Effect.Loot(1),
                        [5] = FS.C.Effect.Loot(1),
                        [6] = FS.C.Effect.DamageToPlayer(1),
                    })
                )
            :Build()
        )
    :Build()
end