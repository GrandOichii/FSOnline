function _Create()
    return FS.B.Item()
        .Static:Common(
            '+1{health}',
            FS.C.StateMod.ModPlayerHealth(function (me, player)
                return 1
            end)
        )
    :Build()
end