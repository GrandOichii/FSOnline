-- status: implemented

function _Create()
    return FS.B.Item()
        :TriggeredAbility(
            FS.B.TriggeredAbility('Each time you take damage, loot 1.')
                .On:ControllerDamaged()
                .Effect:Common(
                    FS.C.Effect.Loot(1)
                )
            :Build()
        )
    :Build()
end