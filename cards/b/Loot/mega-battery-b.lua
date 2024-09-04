-- status: implemented

function _Create()
    return FS.B.Loot('Choose a player. Recharge each item they control.')
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