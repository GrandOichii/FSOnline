-- status: not tested

function _Create()
    return FS.B.Item()
        .Static:Common(
            FS.C.StateMod.TakeNoCombatDamageOnRollsForPlayer({ 1, 6 })
        )
    :Build()
end