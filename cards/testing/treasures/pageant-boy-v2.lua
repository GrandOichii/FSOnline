
function _Create()
    return FS.B.Item()
        :ActivatedAbility(
            FS.B.ActivatedAbility('Destroy this', 'Roll-\nGain x{cent}, where x is 4 times the result.')
                .Cost:Common(
                    FS.C.Cost:DestroyMe()
                )
                .Effect:Roll(function (stackEffect)
                    local roll = stackEffect.Rolls[0]

                    FS.C.Effect.GainCoins(4 * roll)(stackEffect)
                end)
            :Build()
        )
    :Build()
end
