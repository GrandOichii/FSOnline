function _Create()
    return FS.B.Monster()
        :Reward(
            FS.B.Reward('5{cent}')
                .Effect:Common(
                    FS.C.Effect.GainCoins(5)
                )
            :Build()
        )
    :Build()
end