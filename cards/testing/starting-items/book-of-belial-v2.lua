-- status: implemented

function _Create()
    return FS.B.Item()
        :ActivatedAbility(
            FS.B.ActivatedAbility('{T}', 'Add or subtract 1 from a roll.')
                .Cost:Common(
                    FS.C.Cost.Tap()
                )
                .Target:StackEffect(
                    function (me, player)
                        return FS.F.StackEffects():Rolls():Do()
                    end
                )
                .Effect:Common(
                    FS.C.Effect.ModifyTargetRoll(0, {
                        {
                            option = 'Add 1',
                            modFunc = function (roll)
                                return roll + 1
                            end
                        },
                        {
                            option = 'Subtract 1',
                            modFunc = function (roll)
                                return roll - 1
                            end
                        }
                    })
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