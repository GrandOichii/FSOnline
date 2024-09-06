-- status: implemented

function _Create()
    return FS.B.Loot('')
        :TriggeredAbility(
            FS.B.TriggeredAbility('At the start of your turn, look at the top card of the monster deck, you may put it on the bottom.')
                .On:ControllerTurnStart()
                .Effect:Common(
                    FS.C.Effect.Scry(1, FS.DeckIDs.MONSTER)
                )
            :Build()
        )
        :Trinket()
    :Build()
end