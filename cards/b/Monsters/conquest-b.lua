-- TODO when created a 4-player game with ~20 Conquests the game crashed after a bot gained 4 souls

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
            FS.B.Reward('6{cent}')
                .Effect:Common(
                    FS.C.Effect.GainCoins(6)
                )
            :Build()
        )
    :Build()
end