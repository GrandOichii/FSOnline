-- status: implemented
-- TODO too low-level

function _Create()
    return FS.B.Item()
        -- Shop items you purchase cost 5{cent} less.
        .Static:Raw(
            FS.ModLayers.PURCHASE_COST,
            function (me)
                me.Owner.State.PurchaseCostModifiers:Add(function (slot, cost)
                    if slot >= 0 then
                        return cost - 5
                    end
                    return cost
                end)
            end
        )
        :Label(FS.Labels.Guppys)
    :Build()
end