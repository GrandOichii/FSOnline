-- status: implemented

function _Create()
    return FS.B.Item()
        .Static:Common(
            'You take no combat damage on attack rolls of 1.',
            FS.C.StateMod.TakeNoCombatDamageOnRollsForPlayer({ 1 })
        )
    :Build()
end