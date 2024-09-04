-- status: not tested

function _Create()
    return FS.B.Monster()
        :TriggeredAbility(
            FS.B.TriggeredAbility('When this dies, expand the number of active monsters by 1.')
                .On:ThisDies()
                .Effect:Common(
                    FS.C.Effect.ExpandMonsterSlots(1)
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