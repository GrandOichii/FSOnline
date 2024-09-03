-- status: not tested

function _Create()
    -- Effect text here
    return FS.B.Loot()
        .Target:Player(
            function (me, player)
                return FS.F.Players():Killable():Do()
            end
        )
        .Effect:Common(
            FS.C.Effect.RechargeItemsOfTargetPlayer(0)
        )
    :Build()
end