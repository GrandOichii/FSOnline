function _Create()
    return FS.B.Loot('Gain 1{cent}.')
        .Effect:Common(
            FS.C.Effect.GainCoins(1)
        )
    :Build()
end