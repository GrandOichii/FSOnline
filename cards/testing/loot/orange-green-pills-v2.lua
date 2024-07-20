-- status: implemented

function _Create()
    -- Roll-
    -- 1-2: Gain 4{cent}.
    -- 3-4: Gain 7{cent}.
    -- 5-6: Lose 4{cent}.

    return FS.B.Loot()
        .Effect:Roll(function (stackEffect)
            local roll = stackEffect.Rolls[0]

            local amount = 4
            if roll > 2 then
                amount = 7
            end
            if roll > 4 then
                LoseCoins(stackEffect.OwnerIdx, 4)
                return
            end

            FS.C.Effect.GainCoins(amount, function (effect)
                return FS.F.Players():Do(effect.OwnerIdx)
            end)(stackEffect)
        end)
    :Build()
end