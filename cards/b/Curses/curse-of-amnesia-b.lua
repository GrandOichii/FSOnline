-- status: not tested

function _Create()
    return FS.B.Curse()
        :TriggeredAbility(
            FS.B.TriggeredAbility('At the end of your turn, discard 2 loot.')
                .On:ControllerTurnEnd()
                .Effect:Common(
                    FS.C.Effect.Discard(2)
                )
            :Build()
        )        
    :Build()
end