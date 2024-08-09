-- status: not tested

function _Create()
    return FS.B.Curse()
        :TriggeredAbility(
            FS.B.TriggeredAbility('At the end of your turn, lose 4{cent}.')
                .On:ControllerTurnEnd()
                .Effect:Common(
                    FS.C.Effect.LoseCoins(4)
                )
            :Build()
        )        
    :Build()
end