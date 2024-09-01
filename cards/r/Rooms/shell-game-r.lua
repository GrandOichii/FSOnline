-- status: implemented
-- TODO too low-level

function _Create()
    return FS.B.Room()
        :ActivatedAbility(
            FS.B.ActivatedAbility('Pay 3{cent}', 'roll-\n3: Gain 1{cent}.\n4: Loot 1.\n5: Gain 5{cent}.\n6: Gain +1 treasure. Put this into discard.')
                .Cost:Common(
                    FS.C.Cost.PayCoins(3)
                )
                .Effect:Roll(
                    FS.C.Effect.SwitchRoll(0, {
                        [3] = FS.C.Effect.GainCoins(1),
                        [4] = FS.C.Effect.Loot(1),
                        [5] = FS.C.Effect.GainCoins(5),
                        [6] = function (stackEffect)
                            FS.C.Effect.GainTreasure(1)(stackEffect)
                            TryDiscardFromPlay(stackEffect.Card.IPID)
                        end
                    })
                )
            :Build()
        )
    :Build()
end
