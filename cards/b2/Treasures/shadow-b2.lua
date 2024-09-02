-- status: implemented

function _Create()
    return FS.B.Card()
        .Static:Raw(
            FS.ModLayers.DEATH_PENALTY_REPLACEMENT_EFFECTS,
            'If another player would pay the death penalty, you choose what item they would destroy and you gain any loot cards and {cent} they would lose.',
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
