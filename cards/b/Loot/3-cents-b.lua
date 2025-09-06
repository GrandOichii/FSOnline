function _Create()
    return FS.B.Loot('Gain 3{cent}.')
        .Effect:Common(
            FS.C.Effect.GainCoins(3)
        )
    :Build()
end