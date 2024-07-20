-- status: implemented

function _Create()
    -- Recharge an Item
    return FS.B.Loot()
        .Target:Item(
            function (player)
                return FS.F.Items():Rechargeable():Do()
            end
        )
        .Effect:Common(
            FS.C.Effect.RechargeTarget(0)
        )
    :Build()
end