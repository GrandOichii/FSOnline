-- status: implemented

function _Create()
    return FS.B.Item()
        -- Damage you would take is reduced to 1.
        .Static:Common(
            FS.C.StateMod.TakeNoCombatDamageOnRollsForPlayer({ 1 })
        )
    :Build()
end