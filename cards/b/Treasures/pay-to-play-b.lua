function _Create()
    return FS.B.Item()
        :ActivatedAbility(
            FS.B.ActivatedAbility('Pay 10{cent}', 'Steal a non-eternal item a player controls.')
                .Cost:Common(
                    FS.C.Cost.PayCoins(10)
                )
                .Target:Item(
                    function (me, player)
                        return FS.F.Items()
                            :NonEternal()
                            :ControlledByPlayer()
                            :Do()
                    end
                )
                .Effect:Common(
                    FS.C.Effect.StealTargetItem(0)
                )
            :Build()
        )
    :Build()
end