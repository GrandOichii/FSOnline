-- status: implemented

function _Create()
    return FS.B.Monster()
        :Reward(
            FS.B.Reward('6{cent}')
                .Effect:Common(
                    FS.C.Effect.GainCoins(6, FS.F.CurrentPlayers)
                )
            :Build()
        )
    :Build()
end