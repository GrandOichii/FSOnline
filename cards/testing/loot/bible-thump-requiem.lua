-- status: not tested

function _Create()
    return FS.B.Loot()
        .Effect:Common(
            FS.C.Effect.Loot(2)
        )
        .Effect:Custom(function (stackEffect)
            local amount = #FS.F.Players()
                :DiedThisTurn()
                :Do()
            LootCards(stackEffect.OwnerIdx, amount, stackEffect)
        end)
    :Build()
end