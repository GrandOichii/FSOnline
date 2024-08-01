-- status: implemented

function _Create()
    return FS.B.Loot()
        .Target:Player()
        .Effect:Common(
            FS.C.Effect.MantleTargetPlayer(0)
        )
    :Build()
end