-- status: not tested

function _Create()
    return FS.B.Monster()
        :TriggeredAbility(
            FS.B.TriggeredAbility('When this dies, recharge all of your active items.')
                .On:ThisDies()
                .Effect:Common(
                    FS.C.Effect.RechargeAllItems()
                )
            :Build()
        )
        :Reward(
            FS.B.Reward('Loot 1')
                .Effect:Common(
                    FS.C.Effect.Loot(1)
                )
            :Build()
        )
    :Build()
end