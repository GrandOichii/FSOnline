-- status: implemented
-- TODO too low-level

function _Create()
    return FS.B.Item()
        -- !FIXME this does nothing when it enters play, only at the start of the next turn
        -- You may play an additional loot card on your turn.
        .Static:Raw(
            FS.ModLayers.MOD_MAX_LOOT_PLAYS,
            function (me)
                me.Owner.State.LootPlaysForTurn = me.Owner.State.LootPlaysForTurn + 1
            end
        )
    :Build()
end