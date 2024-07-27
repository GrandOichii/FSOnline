-- status: implemented

function _Create()
    return FS.B.Item()
        :TriggeredAbility(
            FS.B.TriggeredAbility('The first time you take damage each turn, you may recharge an item')
                :Limit(1)
                .On:PlayedDamaged(function (me, player, args)
                    return player.Idx == args.Player.Idx
                end)
                .Target:Item(
                    function (me, player, args)
                        return FS.F.Items()
                            :ControlledBy(player.Idx)
                            :Rechargeable()
                            :Do()
                    end
                )
                .Effect:Common(
                    FS.C.Effect.RechargeTarget(0, true)
                )
            :Build()
        )
    :Build()
end