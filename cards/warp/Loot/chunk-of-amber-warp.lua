-- status: implemented

function _Create()
    return FS.B.Loot('Roll- Gain x{cent}, where x is 2 times the result.')
        .Effect:Roll(function (stackEffect)
            local roll = stackEffect.Rolls[0]

            FS.C.Effect.GainCoins(2 * roll)(stackEffect)
        end)
    :Build()
end