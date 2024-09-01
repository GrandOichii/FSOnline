-- status: not tested
-- TODO too low-level

function _Create()
    return FS.B.Room()
        :ActivatedAbility(
            FS.B.ActivatedAbility('Pay 3{cent}', 'roll-\n2: Gain 1{cent}\n3: Gain 2{cent}\n4: Gain 5{cent}\n5: Gain 7{cent}\n6: Gain 15{cent}. Put this into discard.')
                .Cost:Common(
                    FS.C.Cost.PayCoins(3)
                )
                .Effect:Roll(
                    FS.C.Effect.SwitchRoll(0, {
                        [2] = FS.C.Effect.GainCoins(1),
                        [3] = FS.C.Effect.GainCoins(2),
                        [4] = FS.C.Effect.GainCoins(5),
                        [5] = FS.C.Effect.GainCoins(7),
                        [6] = function (stackEffect)
                            FS.C.Effect.GainCoins(15)(stackEffect)
                            TryDiscardFromPlay(stackEffect.Card.IPID)
                        end
                    })
                )
            :Build()
        )
    :Build()
end