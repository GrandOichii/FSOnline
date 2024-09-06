-- status: not tested

function _Create()
    return FS.B.Loot('A player gains +1{attack} and +1{health} till the end of turn.')
        .Target:Player()
        .Effect:Common(
            FS.C.Effect.ModTargetAttackTEOT(0, 1),
            FS.C.Effect.ModTargetHealthTEOT(0, 1)
        )
    :Build()
end