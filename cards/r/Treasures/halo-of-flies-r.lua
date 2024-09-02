-- status: not tested

function _Create()
    return FS.B.Item()
        .Static:Common(
            'You take no combat damage on attack rolls of 1 or 6.',
            FS.C.StateMod.TakeNoCombatDamageOnRollsForPlayer({ 1, 6 })
        )
    :Build()
end