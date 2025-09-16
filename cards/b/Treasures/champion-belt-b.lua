function _Create()
    return FS.B.Item()
        -- TODO add attack modification effect
        .Static:Common(
            'You may attack an additional time on your turn.',
            FS.C.StateMod.ModAO()
        )
    :Build()
end