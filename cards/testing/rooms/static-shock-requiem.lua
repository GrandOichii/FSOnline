-- status: implemented
-- TODO too low-level

function _Create()
    return FS.B.Item()
        :TriggeredAbility(
            FS.B.TriggeredAbility('Each time a player activates an item, they take 1 damage.')
                .On:ItemActivation()
                .Effect:Custom(
                    function (stackEffect, args)
                        print(args.Player.Idx)
                        DealDamageToPlayer(args.Player.Idx, 1, stackEffect)
                    end
                )
            :Build()
        )
    :Build()
end