-- status: implemented

function _Create()
    return FS.B.Room()
        .Static:Common(
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