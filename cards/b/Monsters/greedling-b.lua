-- status: not tested

function _Create()
    return FS.B.Monster()
        :TriggeredAbility(
            FS.B.TriggeredAbility('When this dies, force a player to lose 7{cent}.')
                .On:ThisDies()
                .Target:Player()
                .Effect:Common(
                    FS.C.Effect.LoseCoins(7)
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