-- status: implemented

function _Create()
    return FS.B.Item()
        :ActivatedAbility(
            FS.B.ActivatedAbility('{T}', 'Choose a dice roll. its controller rerolls it.')
                .Cost:Common(
                    FS.C.Cost.Tap()
                )
                .Target:StackEffect(
                    function (me, player)
                        return FS.F.StackEffects():Rolls():Do()
                    end
                )
                .Effect:Common(
                    FS.C.Effect.RerollTargetRoll(0)
                )
                :Build()
        )
        :TriggeredAbility(
            FS.B.TriggeredAbility('At the end of your turn, recharge this.')
                .On:TurnEnd(function (me, player, args)
                    return player.Idx == args.playerIdx
                end)
                .Effect:Common(
                    FS.C.Effect.RechargeMe()
                )
            :Build()
        )
        :Label(FS.Labels.Eternal)
    :Build()
end