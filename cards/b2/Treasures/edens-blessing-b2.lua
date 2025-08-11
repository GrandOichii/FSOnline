-- status: not tested
-- TODO too low-level

function _Create()
    return FS.B.Item()
        :TriggeredAbility(
            FS.B.TriggeredAbility('At the end of your turn, if you have 0{cent}, gain 6{cent}.')
                .On:ControllerTurnEnd()
                .Check:Custom(
                    function (me, player)
                        return player.Coins == 0
                    end
                )
                .Effect:Common(
                    FS.C.Effect.GainCoins(6)
                )
            :Build()
        )
    :Build()
end