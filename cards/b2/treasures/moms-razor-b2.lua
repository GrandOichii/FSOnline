-- status: implemented
-- TODO too low-level

function _Create()
    return FS.B.Item()
        :TriggeredAbility(
            FS.B.TriggeredAbility('Each time a player rolls a {roll:6}, you may deal 1 damage to them.')
                .On:Roll(function (me, player, args)
                    return args.Value == 6
                end)
                .Effect:Custom(
                    function (stackEffect, args)
                        local accept = FS.C.Choose.YesNo(stackEffect.OwnerIdx, 'Deal 1 damage to '..args.Player.LogName..'?')
                        if not accept then
                            return false
                        end
                        DealDamageToPlayer(args.Player.Idx, 1, stackEffect)
                    end
                )
            :Build()
        )
    :Build()
end