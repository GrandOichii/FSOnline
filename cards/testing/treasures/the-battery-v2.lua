function _Create()
    return FS.B.Item()
        :ActivatedAbility(
            FS.B.ActivatedAbility('{T}', 'Recharge another item.')
                .Cost:Common(
                    FS.C.Cost:Tap()
                )
                .Target:Item(
                    function (me, player)
                        return FS.F.Items():Except(me.IPID):Do()
                    end
                )
                .Effect:Common(
                    FS.C.Effect.RechargeTarget(0)
                )
            :Build()
        )
    :Build()
end