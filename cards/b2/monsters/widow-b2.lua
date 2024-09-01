-- status: implemented

function _Create()
    return FS.B.Monster()
        :Reward(
            FS.B.Reward('Loot 2')
                .Effect:Common(
                    FS.C.Effect.Loot(2)
                )
            :Build()
        )
    :Build()
end