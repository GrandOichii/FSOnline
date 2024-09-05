-- status: not tested

function _Create()
    return FS.B.Item()
        :TriggeredAbility(
            FS.B.TriggeredAbility('At the end of your turn, look at the top 4 cards of the treasure deck. You may put them back in any order.')
                .On:ControllerTurnEnd()
                .Effect:Common(
                    FS.C.Effect.ReorderTop(4, FS.DeckIDs.TREASURE)
                )
            :Build()
        )
    :Build()
end