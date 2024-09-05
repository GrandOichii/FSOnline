-- status: implementeed

function _Create()
    return FS.B.Monster()
        :TriggeredAbility(
            FS.B.TriggeredAbility('When any player rolls a {roll:5}, they discard a loot card.')
                .On:RollOfValue(5)
                .Effect:Common(
                    FS.C.Effect.Discard(1, FS.C.RollOwner)
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