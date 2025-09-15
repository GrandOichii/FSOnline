function _Create()
    return FS.B.Monster()
        :TriggeredAbility(
            FS.B.TriggeredAbility('When this dies, you may attack the monster deck an additional time.')
                .On:ThisDies()
                .Effect:Common(
                    FS.C.Effect.AddTopOfDeckAO(1, FS.C.CurrentPlayers)
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