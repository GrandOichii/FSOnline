-- status: implemented

function _Create()
    return FS.B.Monster()
        :Reward(
            FS.B.Reward('Loot 1')
                .Effect:Common(
                    FS.C.Effect.Loot(1, FS.F.CurrentPlayers)
                )
            :Build()
        )
    :Build()
end