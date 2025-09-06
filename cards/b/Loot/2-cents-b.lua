function _Create()
    return FS.B.Loot('Gain 2{cent}.')
        .Effect:Common(
            FS.C.Effect.GainCoins(2)
        )
    :Build()
end