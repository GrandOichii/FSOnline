-- status: implemented

function _Create()
    return FS.B.Loot()
        -- Reroll an item
        .Target:Item(
            function (me, player)
                return FS.F.Items():Rerollable():Do()
            end,
            'Choose an Item to reroll'
        )
        .Effect:Common(
            FS.C.Effect.RerollTargetItem(0)
        )
    :Build()
end