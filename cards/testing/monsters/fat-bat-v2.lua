-- status: implemented

function _Create()
    return FS.B.Monster()
        :Reward(
            FS.B.Reward('+1 Treasure')
                .Effect:Common(
                    FS.C.Effect.GainTreasure(1, FS.F.CurrentPlayers)
                )
            :Build()
        )
    :Build()
end