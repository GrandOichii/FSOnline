-- status: implemented

function _Create()
    return FS.B.Monster()
        :Reward(
            FS.B.Reward('Reward text here')
                .Effect:Common(
                    FS.C.Effect.Loot(2, FS.F.CurrentPlayers)
                )
            :Build()
        )
    :Build()
end