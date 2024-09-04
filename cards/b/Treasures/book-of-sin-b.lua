-- status: not tested

function _Create()
    return FS.B.Item()
        :ActivatedAbility(
            FS.B.ActivatedAbility('{T}', 'Roll:\n1-2: gain 1{cent}.\n3-4: Loot 1.\n5-6: Gain +1{health} till the end of turn.')
                .Cost:Common(
                    FS.C.Cost.Tap()
                )
                .Effect:Roll(
                    FS.C.Effect.SwitchRoll(0, {
                        [1] = FS.C.Effect.GainCoins(1),
                        [2] = FS.C.Effect.GainCoins(1),
                        [3] = FS.C.Effect.Loot(1),
                        [4] = FS.C.Effect.Loot(1),
                        [5] = FS.C.Effect.ModPlayerHealthTEOT(1),
                        [6] = FS.C.Effect.ModPlayerHealthTEOT(1),
                    })
                )
            :Build()
        )
    :Build()
end