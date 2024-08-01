-- status: implemented

function _Create()
    return FS.B.Item()
        :TriggeredAbility(
            FS.B.TriggeredAbility('At the start of your turn, roll-\n1-2: Gain 3{cent}.\n3-4: Loot 1.\n5-6: Take 1 damage.')
                .On:ControllerTurnStart()
                .Effect:Roll(
                    FS.C.Effect.SwitchRoll(0, {
                        [1] = FS.C.Effect.GainCoins(3),
                        [2] = FS.C.Effect.GainCoins(3),
                        [3] = FS.C.Effect.Loot(1),
                        [4] = FS.C.Effect.Loot(1),
                        [5] = FS.C.Effect.DamageToPlayer(1),
                        [6] = FS.C.Effect.DamageToPlayer(1),
                    })
                )
            :Build()
        )
    :Build()
end