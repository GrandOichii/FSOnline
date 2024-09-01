-- status: implemented

function _Create()
    return FS.B.Item()
        :ActivatedAbility(
            FS.B.ActivatedAbility('Destroy 2 items you control', 'Steal a non-eternal item from a player.')
                .Target:Item(
                    function (me, player)
                        return FS.F.Items()
                            :ControlledByPlayer()
                            :NonEternal()
                            :Do()
                    end
                )
                .Cost:Common(
                    FS.C.Cost.SacrificeItems(2)
                )
                .Effect:Common(
                    FS.C.Effect.StealTargetItem(0)
                )
            :Build()
        )
    :Build()
end