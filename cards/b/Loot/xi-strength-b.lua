function _Create()
    return FS.B.Loot('A player gains +1{attack} till the end of turn and may attack an additional time.')
        .Target:Player()
        .Effect:Common(
            FS.C.Effect.ModTargetPlayerAttackTEOT(0, 1),
            FS.C.Effect.AddAOToTarget(0, 1)
        )
    :Build()
end