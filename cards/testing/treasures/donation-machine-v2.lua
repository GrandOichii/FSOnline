-- status: implemented

function _Create()
    return FS.B.Item()
        :ActivatedAbility(
            FS.B.ActivatedAbility('Give another non-eternal item you control to another player', 'Gain 8{cent}')
                .Cost:Common(
                    FS.C.Cost.DonateItems(
                        1,
                        function (me, player)
                            print(me, player)
                            return FS.F.Items()
                                :Except(me.IPID)
                                :NonEternal()
                                :ControlledBy(player.Idx)
                                :Do()
                        end,
                        function (me, player)
                            return FS.F.Players()
                                :Except(player.Idx)
                                :Do()
                        end
                    )
                )
                .Effect:Common(
                    FS.C.Effect.GainCoins(8)
                )
            :Build()
        )
    :Build()
end