-- status: implemented

function _Create()
    return FS.B.Room()
        .Static:Common(
            'Players have +1{health}.',
            FS.C.StateMod.ModPlayerHealth(
                function (me, player)
                    return 1
                end,
                function (me)
                    return FS.F.Players():Do()
                end
            )
        )
    :Build()
end