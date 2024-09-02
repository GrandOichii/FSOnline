-- status: implemented

function _Create()
    return FS.B.Loot('Kill a player.')
        .Target:Player(
            function (me, player)
                return FS.F.Players():Killable():Do()
            end
        )
        .Effect:Common(
            FS.C.Effect.KillTargetPlayer(0)
        )
    :Build()
end