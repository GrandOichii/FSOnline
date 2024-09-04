-- status: not tested

function _Create()
    return FS.B.Monster()
        :TriggeredAbility(
            FS.B.TriggeredAbility('When this dies, force a player to discard 2 loot cards.')
                .On:ThisDies()
                .Target:Player()
                .Effect:Common(
                    FS.C.Effect.TargetPlayerDiscards(0, 2)
                )
            :Build()
        )
        :Reward(
            FS.B.Reward('Loot 2')
                .Effect:Common(
                    FS.C.Effect.Loot(2)
                )
            :Build()
        )
    :Build()
end