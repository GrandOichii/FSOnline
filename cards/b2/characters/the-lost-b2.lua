-- status: implemented

function _Create()
    return FS.B.Character()
        :Basic()
        .Static:Common(
            FS.C.StateMod.AddSouls(1)
        )
        :TriggeredAbility(
            FS.B.TriggeredAbility('At the end of your turn, recharge up to 1 Character.')
                .On:ControllerTurnEnd()
                .Target:Character()
                .Effect:Common(
                    FS.C.Effect.RechargeTarget(0, true)
                )
            :Build()
        )
        :StartingItem('holy-mantle-b2')
    :Build()
end