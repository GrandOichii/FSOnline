-- status: implemented
-- TODO too low-level

function _Create()
    return FS.B.Item()
        :ActivatedAbility(
            FS.B.ActivatedAbility('{T}', 'Each other player may choose to gain 1{cent}. Gain 1{cent} + 1{cent} for each player who did.')
                .Cost:Common(
                    FS.C.Cost.Tap()
                )
                .Effect:Common(
                    function (stackEffect)
                        local ownerIdx = stackEffect.OwnerIdx
                        local players = FS.F.Players()
                            :Except(ownerIdx)
                            :Do(ownerIdx)
                        local amount = 1
                        for _, player in ipairs(players) do
                            local accept = FS.C.Choose.YesNo(player.Idx, 'Gain 1{cent}?')
                            if accept then
                                AddCoins(player.Idx, 1)
                                amount = amount + 1
                            end
                        end
                        AddCoins(ownerIdx, amount)
                    end
                )
            :Build()
        )
        -- Shop items you purchase cost 3{cent} less.
        .Static:Raw(
            FS.ModLayers.PURCHASE_COST,
            function (me)
                me.Owner.State.PurchaseCostModifiers:Add(function (slot, cost)
                    if slot >= 0 then
                        return cost - 3
                    end
                    return cost
                end)
            end
        )
        :Label(FS.Labels.Eternal)
    :Build()
end