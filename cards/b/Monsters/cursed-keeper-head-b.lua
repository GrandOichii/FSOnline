-- status: implemented

function _Create()
    return FS.B.Monster()
        :TriggeredAbility(
            FS.B.TriggeredAbility('Each time a player rolls a {roll:1}, they lose 2{cent}.')
                .On:RollOfValue(1)
                .Effect:Common(
                    FS.C.Effect.LoseCoins(2, FS.C.RollOwner)
                )
            :Build()
        )
        :Reward(
            FS.B.Reward('Roll- gain x{cent}')
                .Effect:Roll(
                    function (stackEffect)
                        local roll = stackEffect.Rolls[0]
                        return FS.C.Effect.GainCoins(roll)(stackEffect)
                    end
                )
            :Build()
        )
    :Build()
end