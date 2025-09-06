function _Create()
    return FS.B.Loot('A player gains +2{health} till the end of turn.')
        .Target:Player()
        .Effect:Common(
            FS.C.Effect.ModTargetHealthTEOT(0, 2)
        )
    :Build()
end