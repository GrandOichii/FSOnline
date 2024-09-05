-- status: implemented

function _Create()
    return FS.B.Item()
        :TriggeredAbility(
            FS.B.TriggeredAbility('Each time a player activates an item, they take 1 damage.')
                .On:ItemActivation()
                .Effect:Common(
                    FS.C.Effect.DamageToPlayer(1, FS.C.ActivationOwner)
                )
            :Build()
        )
    :Build()
end