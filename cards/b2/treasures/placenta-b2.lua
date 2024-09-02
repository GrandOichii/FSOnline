-- status: not tested

function _Create()
    return FS.B.Item()
        .Static:Common(
            '+1{health}',
            FS.C.StateMod.ModPlayerHealth(function (me, player)
                return 1
            end)
        )
        .Static:Common(
            'You take no combat damage on attack rolls of 4.',
            FS.C.StateMod.TakeNoCombatDamageOnRollsForPlayer({ 4 })
        )
    :Build()
end