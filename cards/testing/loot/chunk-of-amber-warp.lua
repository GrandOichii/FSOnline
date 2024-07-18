-- status: implemented

function _Create()
    -- Roll-
    -- Gain x{cent}, where x is 2 times the result.

    return FS.B.Loot()
        .Effect:Roll(function (stackEffect)
            local roll = stackEffect.Rolls[0]

            FS.C.Effect.GainCoins(2 * roll)(stackEffect)
        end)
    :Build()
end