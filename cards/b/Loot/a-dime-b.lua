function _Create()
    return FS.B.Loot('Gain 10{cent}.')
        .Effect:Common(
            FS.C.Effect.GainCoins(10)
        )
    :Build()
end