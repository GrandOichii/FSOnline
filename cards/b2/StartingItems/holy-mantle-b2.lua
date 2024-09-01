-- status: implemented, requires a lot of testing
-- !FIXME encountered weird scenario: put blood donation's effect on the stack during my turn, in response opponent used holy mantle to prevent my death. after life loss resolution, my death was placed on the stack, however it showed that I was suddenly not the current player

function _Create()
    return FS.B.Item()
        :ActivatedAbility(
            FS.B.ActivatedAbility('{T}', 'Choose a player. The next time that player would die this turn, prevent it. If it\'s their turn, cancel everything that hasn\'t resolved and end it.')
                .Cost:Common(
                    FS.C.Cost:Tap()
                )
                .Target:Player()
                .Effect:Common(
                    FS.C.Effect.MantleTargetPlayer(0)
                )
            :Build()
        )
        :TriggeredAbility(
            FS.B.TriggeredAbility('At the end of your turn, recharge this.')
                .On:ControllerTurnEnd()
                .Effect:Common(
                    FS.C.Effect.RechargeMe()
                )
            :Build()
        )
        :Label(FS.Labels.Eternal)
    :Build()
end