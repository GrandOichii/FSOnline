-- status: implemented

function _Create()
    return FS.B.Loot()
        .Target:Player(
            function (me, player)
                return FS.F.Players():Alive():Do()
            end
        )
        .Effect:Common(
            FS.C.Effect.KillTargetPlayer(0)
        )
    :Build()
end