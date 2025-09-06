function _Create()
    return FS.B.Loot('Gain 4{cent}.')
        .Effect:Common(
            FS.C.Effect.GainCoins(4)
        )
    :Build()
end