-- status: implemented
-- Each time another player dies, you choose what item they destroy.\nEach time another player dies, you gain any loot or {cent} they lose.
-- TODO? does this work differently

function _Create()
    return FS.B.Card()
        .Static:Raw(
            FS.ModLayers.DEATH_PENALTY_REPLACEMENT_EFFECTS,
            function (me)
                local players = FS.F.Players():Except(me.Owner.Idx):Do()
                for _, player in ipairs(players) do
                    player.State.DeathPenaltyReplacementEffects:Add(function (effected, deathSource)
                        -- TODO too low level

                        -- item destruction
                        local itemAmount = effected:GetDeathPenaltyDestroyedItemsAmount()
                        for i = 1, itemAmount do
                            local items = FS.F.Items():ControlledBy(effected.Idx):Destructable():Do()
                            if #items == 0 then
                                break
                            end

                            local ipid = FS.C.Choose.Item(
                                me.Owner.Idx,
                                items,
                                'Choose an item to destroy'
                            )
                            DestroyItem(ipid)
                        end

                        -- discarding loot
                        local lootAmount = effected:GetDeathPenaltyLootDiscardAmount()
                        FS.C.GiveLootCards(me.Owner.Idx, effected.Idx, lootAmount)

                        -- losing coins
                        StealCoins(me.Owner.Idx, effected.Idx, effected:GetDeathPenaltyCoinLoss())

                        -- tapping all items + characters
                        TapCard(effected.Character.IPID)
                        local items = FS.F.Items():ControlledBy(effected.Idx):Do()
                        for _, item in ipairs(items) do
                            TapCard(item.IPID)
                        end

                        return true
                    end)
                end
            end
        )
    :Build()
end
