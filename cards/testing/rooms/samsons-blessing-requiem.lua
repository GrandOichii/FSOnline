-- status: not tested

function _Create()
    return FS.B.Room()
    -- Players have +1{attack}.
        .Static:Common(
            FS.C.StateMod.ModPlayerAttack(
                function (me, player)
                    return 1
                end,
                FS.C.AllPlayers
            )
        )
    :Build()
end