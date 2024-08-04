-- status: implemented

function _Create()
    return FS.B.Monster()
        :Reward(
            FS.B.Reward('3{cent}')
                .Effect:Common(
                    FS.C.Effect.GainCoins(3, FS.F.CurrentPlayers)
                )
            :Build()
        )
    :Build()
end