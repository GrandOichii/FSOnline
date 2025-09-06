function _Create()
    return FS.B.Monster()
        :Reward(
            FS.B.Reward('7{cent}')
                .Effect:Common(
                    FS.C.Effect.GainCoins(7)
                )
            :Build()
        )
    :Build()
end