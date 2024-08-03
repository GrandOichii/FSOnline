-- status: implemented

function _Create()
    return FS.B.Monster()
        :TriggeredAbility(
            FS.B.TriggeredAbility('When this dies, it deals 1 damage to each player.')
                .On:ThisDies()
                .Effect:Common(
                    FS.C.Effect.DamageToPlayer(
                        1,
                        function (stackEffect, args)
                            return FS.F.Players():Do(GetCurPlayerIdx())
                        end
                    )
                )
            :Build()
        )
        :Reward(
            FS.B.Reward('4{cent}')
                .Effect:Common(
                    FS.C.Effect.GainCoins(4, FS.F.CurrentPlayers)
                )
            :Build()
        )
    :Build()
end