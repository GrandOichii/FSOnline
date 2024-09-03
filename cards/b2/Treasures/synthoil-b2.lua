-- status: implemented

function _Create()
    return FS.B.Item()
        .Static:Common(
            '+1 to all your attack rolls.',
            FS.C.StateMod.PlusToAttackRolls(1)
        )
    :Build()
end