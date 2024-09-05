-- status: not tested

function _Create()
    return FS.B.Monster()
        :TriggeredAbility(
            FS.B.TriggeredAbility('When any player rolls a {roll:4}, all active monsters gain +1{attack} till the end of turn.')
                .On:RollOfValue(4)
                .Effect:Common(
                    FS.C.Effect.ModMonsterAttackTEOT(1, FS.C.AllMonsters)
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