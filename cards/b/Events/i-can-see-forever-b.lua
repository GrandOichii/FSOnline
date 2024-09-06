-- status: not tested

function _Create()
    return FS.B.Event('Look at the top 6 cards of the loot deck, you may put them back in any order, then loot 1.')
        .Effect:Common(
            FS.C.Effect.ReorderTop(6, FS.DeckIDs.LOOT)
        )
        .Effect:Common(
            FS.C.Effect.Loot(1)
        )
    :Build()
end