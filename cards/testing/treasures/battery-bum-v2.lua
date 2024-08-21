-- status: not tested

function _Create()
    return FS.B.Item()
        :ActivatedAbility(
            FS.B.ActivatedAbility('Pay 4{cent}', 'Recharge an Item')
                .Cost:Common(
                    FS.C.Cost.PayCoins(4)
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
                )
            :Build()
        )
    :Build()
end