-- status: implemented

function _Create()
    return FS.B.Item()
        -- +1{health}.
        .Static:Common(
            FS.C.StateMod.ModPlayerHealth(function (me, player)
                return 1
            end)
        )
    :Build()
end