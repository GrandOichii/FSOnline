-- status: implemented

function _Create()
    return FS.B.Monster()
        :Reward(
            FS.B.Reward('+2 Treasure')
                .Effect:Common(
                    FS.C.Effect.GainTreasure(2)
                )
            :Build()
        )
    :Build()
end