-- Destroy this: Roll-
-- 1: You die.
-- 2: Loot 3.
-- 3: Gain 9{cent}.
-- 4: Gain +1 treasure.
-- 5: Gain +2 treasure.
-- 6: This becomes a soul. gain it.",

function _Create()
    return FS.B.Item()
        :ActivatedAbility(
            FS.B.ActivatedAbility('Destroy this', 'Roll-\n1: You die.\n2: Loot 3.\n3: Gain 9{cent}.\n4: Gain +1 treasure.\n5: Gain +2 treasure.\n6: This becomes a soul. gain it.')
                .Cost:Common(
                    FS.C.Cost.DestroyMe()
                )
                .Effect:Roll(
                    FS.C.Effect.SwitchRoll(0, {
                        [1] = FS.C.Effect.KillOwner(),
                        [2] = FS.C.Effect.Loot(3),
                        [3] = FS.C.Effect.GainCoins(9),
                        [4] = FS.C.Effect.GainTreasure(1),
                        [5] = FS.C.Effect.GainTreasure(2),
                        [6] = FS.C.Effect.RemoveAndBecomeASoul(),
                    })
                )
            :Build()
        )
    :Build()
end