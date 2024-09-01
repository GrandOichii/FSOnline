-- status: not tested
-- TODO too low-level

function _Create()
    return FS.B.Item()
        :TriggeredAbility(
            FS.B.TriggeredAbility('If you have 0{cent} at the end of your turn, gain 6{cent}.')
                .On:ControllerTurnEnd()
                .Check:Custom(
                    function (me, player)
                        return player.Coins == 0
                    end
                )
                .Effect:Common(
                    FS.C.Effect.RechargeMe()
                )
            :Build()
        )
    :Build()
end