-- status: not tested

function _Create()
    return FS.B.Loot('Look at the top 5 cards of the monster deck, put 4 on the bottom of the deck, and one back on top.')
        .Effect:Common(
            FS.C.Effect.LootAndPlaceOnTopRestToBottom(5, 1, FS.DeckIDs.MONSTER)
        )
    :Build()
end