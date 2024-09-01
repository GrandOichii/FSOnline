function _Create()
    return FS.B.Item()
        :ActivatedAbility(
            FS.B.ActivatedAbility('{T}', 'Roll-\n1-2: Loot 1.\n3-4: Gain 4{cent}.\n5-6: Nothing.')
                .Cost:Common(
                    FS.C.Cost.Tap()
                )
                .Effect:Roll(function (stackEffect)
                    local roll = stackEffect.Rolls[0]

                    if roll == 1 or roll == 2 then
                        FS.C.Effect.Loot(1)(stackEffect)
                        return
                    end
                    if roll == 3 or roll == 4 then
                        FS.C.Effect.GainCoins(4)(stackEffect)
                        return
                    end
                end)
            :Build()
        )
    :Build()
end