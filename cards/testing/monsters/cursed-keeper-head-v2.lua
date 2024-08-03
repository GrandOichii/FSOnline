-- status: partially implemented

function _Create()
    return FS.B.Monster()
        -- TODO add trigger
        :Reward(
            FS.B.Reward('Roll- gain x{cent}')
                .Effect:Roll(
                    function (stackEffect)
                        local roll = stackEffect.Rolls[0]
                        FS.C.Effect.GainCoins(roll, FS.C.CurrentPlayers)(stackEffect)
                    end
                )
            :Build()
        )
    :Build()
end