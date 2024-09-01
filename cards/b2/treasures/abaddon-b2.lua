-- status: not tested

function _Create()
    return FS.B.Item()
        -- +3{attack}
        .Static:Common(
            FS.C.StateMod.ModPlayerAttack(function (me, player)
                return 3
            end)
        )
        :TriggeredAbility(
            FS.B.TriggeredAbility('Each time you take damage, die!')
                .On:ControllerDamaged()
                .Effect:Common(
                    FS.C.Effect.KillOwner()
                )
            :Build()
        )
    :Build()
end