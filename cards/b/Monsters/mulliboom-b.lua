-- status: not tested

function _Create()
    return FS.B.Monster()
        :TriggeredAbility(
            FS.B.TriggeredAbility('When this dies, deal 3 damage to any player.')
                .On:ThisDies()
                .Target:Player(
                    function (me, player)
                        return FS.F.Players():Do()
                    end
                )
                .Effect:Common(
                    FS.C.Effect.DamageToTargetPlayer(0, 3)
                )
            :Build()
        )
        :Reward(
            FS.B.Reward('6{cent}')
                .Effect:Common(
                    FS.C.Effect.GainCoins(6)
                )
            :Build()
        )
    :Build()
end