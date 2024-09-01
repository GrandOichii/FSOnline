-- status: not tested

function _Create()
    return FS.B.Item()
        :TriggeredAbility(
            FS.B.TriggeredAbility('When anyone rolls a {roll:2}, you may recharge any item.')
                .On:Roll(function (me, player, args)
                    return args.Value == 2
                end)
                .Target:Item(
                    function (me, player)
                        return FS.F.Items()
                            :Rechargeable()
                            :Do()
                    end
                )
                .Effect:Common(
                    FS.C.Effect.RechargeTarget(0)
                )
            :Build()
        )
    :Build()
end