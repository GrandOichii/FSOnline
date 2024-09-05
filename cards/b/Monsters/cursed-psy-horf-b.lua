-- status: not tested

function _Create()
    return FS.B.Monster()
        :TriggeredAbility(
            FS.B.TriggeredAbility('When any player activates an item, they take 1 damage.')
                .On:ItemActivation()
                .Effect:Common(
                    FS.C.Effect.DamageToPlayer(1, FS.C.ActivationOwner)
                )
            :Build()
        )
        :Reward(
            FS.B.Reward('Loot 2')
                .Effect:Common(
                    FS.C.Effect.Loot(2)
                )
            :Build()
        )
    :Build()
end