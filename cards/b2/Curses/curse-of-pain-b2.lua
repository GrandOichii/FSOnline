-- status: implemented

function _Create()
    return FS.B.Curse()
        :TriggeredAbility(
            FS.B.TriggeredAbility('At the start of your turn, take 1 damage.')
                .On:ControllerTurnStart()
                .Effect:Common(
                    FS.C.Effect.DamageToPlayer(1)
                )
            :Build()
        )
    :Build()
end