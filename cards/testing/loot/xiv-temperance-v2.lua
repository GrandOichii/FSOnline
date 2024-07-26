-- Choose one-\nTake 1 Damage and gain 4{cent}.\nTake 2 Damage and gain 8{cent}.

function _Create()
    return FS.B.Loot()
        :Choose(
            FS.C.Choose.Effect(
                {
                    label = 'Take 1 Damage and gain 4{cent}.',
                    effects = {
                        FS.C.Effect.DamageToPlayer(1),
                        FS.C.Effect.GainCoins(4)
                    }
                },
                {
                    label = 'Take 2 Damage and gain 8{cent}.',
                    effects = {
                        FS.C.Effect.DamageToPlayer(2),
                        FS.C.Effect.GainCoins(8)
                    }
                }
            )
        )
    :Build()
end
