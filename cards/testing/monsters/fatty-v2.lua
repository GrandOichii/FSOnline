-- status: implemented

function _Create()
    return FS.B.Monster()
        :Reward(
            FS.B.Reward('1{cent}')
                .Effect:Common(
                    FS.C.Effect.Loot(1, FS.F.CurrentPlayers)
                )
            :Build()
        )
    :Build()
end