-- status: implemented

function _Create()
    return FS.B.Monster()
        :Reward(
            FS.B.Reward('4{cent}')
                .Effect:Common(
                    FS.C.Effect.GainCoins(4, FS.F.CurrentPlayers)
                )
            :Build()
        )
    :Build()
end