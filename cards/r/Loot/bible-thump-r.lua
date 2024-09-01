-- status: not tested

function _Create()
    return FS.B.Loot('Loot 2, then loot 2 for each player that died this turn.')
        .Effect:Common(
            FS.C.Effect.Loot(2)
        )
        .Effect:Custom(function (stackEffect)
            local amount = #FS.F.Players()
                :DiedThisTurn()
                :Do()
            LootCards(stackEffect.OwnerIdx, amount * 2, stackEffect)
        end)
    :Build()
end