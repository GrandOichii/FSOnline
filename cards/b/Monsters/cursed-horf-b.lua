-- status: not tested

function _Create()
    return FS.B.Monster()
        :TriggeredAbility(
            FS.B.TriggeredAbility('When any player rolls a {roll:2}, they take 2 damage.')
                .On:RollOfValue(5)
                .Effect:Common(
                    FS.C.Effect.DamageToPlayer(2, FS.C.RollOwner)
                )
            :Build()
        )
        :Reward(
            FS.B.Reward('3{cent}')
                .Effect:Common(
                    FS.C.Effect.GainCoins(3)
                )
            :Build()
        )
    :Build()
end