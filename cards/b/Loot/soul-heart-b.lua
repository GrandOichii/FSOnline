-- status: implemented

function _Create()
    return FS.B.Loot('Choose a player. Prevent the next 1 damage they would take this turn.')
        .Target:Player(
            function (me, player)
                return FS.F.Players():Do()
            end
        )
        .Effect:Common(
            FS.C.Effect.PreventNextDamageToTargetPlayer(0, 1)
        )
    :Build()
end