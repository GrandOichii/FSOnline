-- status: not tested

function _Create()
    return FS.B.Item()
        .Effect:Common(
            FS.C.Effect.Scry(1)
        )
    :Build()
end