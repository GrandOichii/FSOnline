-- status: not tested

function _Create()
    return FS.B.Monster()
        :TriggeredAbility(
            FS.B.TriggeredAbility('When this dies, roll:\n1-3: All players take 1 damage.\n4-6: All players take 2 damage.')
                .On:ThisDies()
                .Effect:Roll(
                    FS.C.Effect.SwitchRoll(0, {
                        [1] = FS.C.Effect.DamageToPlayer(1, FS.C.AllPlayers),
                        [2] = FS.C.Effect.DamageToPlayer(1, FS.C.AllPlayers),
                        [3] = FS.C.Effect.DamageToPlayer(1, FS.C.AllPlayers),
                        [4] = FS.C.Effect.DamageToPlayer(2, FS.C.AllPlayers),
                        [5] = FS.C.Effect.DamageToPlayer(2, FS.C.AllPlayers),
                        [6] = FS.C.Effect.DamageToPlayer(2, FS.C.AllPlayers),
                    })
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