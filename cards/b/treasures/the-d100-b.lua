-- status: not tested

function _Create()
    return FS.B.Item()
        :ActivatedAbility(
            FS.B.ActivatedAbility('{T}', 'Roll:\n1: Loot 1.\n2: Loot 2.\n\n3: Gain 3{cent}.\n4: gain 4{cent}.\n5: gain +1{health} till the end of turn.\n6: gain +1{attack} till the end of turn.')
                .Cost:Common(
                    FS.C.Cost.Tap()
                )
                .Effect:Roll(
                    FS.C.Effect.SwitchRoll(0, {
                        [1] = FS.C.Effect.Loot(1),
                        [2] = FS.C.Effect.Loot(2),
                        [3] = FS.C.Effect.GainCoins(3),
                        [4] = FS.C.Effect.GainCoins(4),
                        [5] = FS.C.Effect.ModPlayerAttackTEOT(1),
                        [6] = FS.C.Effect.ModPlayerHealthTEOT(1),
                    })
                )
            :Build()
        )
    :Build()
end