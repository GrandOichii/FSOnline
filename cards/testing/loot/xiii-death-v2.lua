-- status: implemented

function _Create()
    return FS.B.Loot()
        .Target:Player(
            function (player)
                return FS.F.Players():Do()
            end
        )
        .Effect:Common(
            FS.C.Effect.KillTargetPlayer(0)
        )
    :Build()
end