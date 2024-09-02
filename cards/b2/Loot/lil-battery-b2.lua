-- status: implemented

function _Create()
    return FS.B.Loot('Recharge an Item.')
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