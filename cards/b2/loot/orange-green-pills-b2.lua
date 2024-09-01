-- status: implemented
-- TODO change to SwitchRoll

function _Create()
    return FS.B.Loot('Roll-\n1-2: Gain 4{cent}.\n3-4: Gain 7{cent}.\n5-6: Lose 4{cent}.')
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