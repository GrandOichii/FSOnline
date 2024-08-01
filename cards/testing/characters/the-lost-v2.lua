-- status: implemented

function _Create()
    return FS.B.Character()
        :Basic()
        -- TODO add soul value
        :TriggeredAbility(
            FS.B.TriggeredAbility('At the end of your turn, recharge up to 1 Character.')
                .On:TurnEnd()
                .Target:Character()
                .Effect:Common(
                    FS.C.Effect.RechargeTarget(0, true)
                )
            :Build()
        )
    :Build()
end