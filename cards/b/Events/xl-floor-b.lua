-- status: not tested

function _Create()
    return FS.B.Event('Expand the number of active monsters by 1.\nYou may attack an additional time this turn.')
        .Effect:Common(
            FS.C.Effect.ExpandMonsterSlots(1)
        )
        .Effect:Common(
            FS.C.Effect.AddAO(1, FS.C.CurrentPlayers)
        )
    :Build()
end