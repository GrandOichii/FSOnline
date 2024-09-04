-- status: not tested

function _Create()
    return FS.B.Monster()
        :TriggeredAbility(
            FS.B.TriggeredAbility('When this dies, expand the number of items in the shop by 1.')
                .On:ThisDies()
                .Effect:Common(
                    FS.C.Effect.ExpandShotSlots(1)
                )
            :Build()
        )
        :Reward(
            FS.B.Reward('7{cent}')
                .Effect:Common(
                    FS.C.Effect.GainCoins(7)
                )
            :Build()
        )
    :Build()
end