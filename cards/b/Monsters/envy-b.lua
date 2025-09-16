function _Create()
    return FS.B.Monster()
        :TriggeredAbility(
            FS.B.TriggeredAbility('When this dies, the active player must make an additonal attack.')
                .On:ThisDies()
                .Effect:Common(
                    FS.C.Effect.MustMakeAnAdditionalAttack(FS.C.CurrentPlayers)
                )
            :Build()
        )
        :Reward(
            FS.B.Reward('1{cent}')
                .Effect:Common(
                    FS.C.Effect.GainCoins(1)
                )
            :Build()
        )
    :Build()
end