-- status: not tested

function _Create()
    return FS.B.Event('Expand the number of items in the shop by 2.\nYou may attack an additional time this turn.')
        .Effect:Common(
            FS.C.Effect.ExpandShotSlots(2)
        )
        .Effect:Common(
            FS.C.Effect.AddAO(1, FS.C.CurrentPlayers)
        )
    :Build()
end