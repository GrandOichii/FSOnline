-- status: not tested

function _Create()
    return FS.B.Room()
        .Static:Common(
            'Players have +1{attack}.',
            FS.C.StateMod.ModPlayerAttack(
                function (me, player)
                    return 1
                end,
                FS.C.AllPlayers
            )
        )
    :Build()
end