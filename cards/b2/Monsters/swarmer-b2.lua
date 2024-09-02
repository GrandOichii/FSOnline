-- status: implemented

function _Create()
    return FS.B.Monster()
        :TriggeredAbility(
            FS.B.TriggeredAbility('When this dies, expand monster slots by 2.')
                .On:ThisDies()
                .Effect:Common(
                    FS.C.Effect.ExpandMonsterSlots(2)
                )
            :Build()
        )
        :Reward(
            FS.B.Reward('4{cent}')
                .Effect:Common(
                    FS.C.Effect.GainCoins(4)
                )
            :Build()
        )
    :Build()
end