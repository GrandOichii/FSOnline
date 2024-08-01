-- If another player would pay the death penalty, you choose what item they would destroy and you gain any loot cards and {cent} they would lose.
-- status: implemented

function _Create()
    return FS.B.Card()
        .Static:Raw(
            FS.ModLayers.DEATH_PENALTY_REPLACEMENT_EFFECTS,
            function (me)
                local players = FS.F.Players():Except(me.Owner.Idx):Do()
                for _, player in ipairs(players) do
                    player.State.DeathPenaltyReplacementEffects:Add(function (player_, deathSource)
                        -- TODO too low level

                        -- item destruction
                        local itemAmount = player_:GetDeathPenaltyDestroyedItemsAmount()
                        for i = 1, itemAmount do
                            local items = FS.F.Items():ControlledBy(player_.Idx):Destructable():Do()
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
                        local lootAmount = player_:GetDeathPenaltyLootDiscardAmount()
                        FS.C.GiveLootCards(me.Owner.Idx, player_.Idx, lootAmount)

                        -- losing coins
                        StealCoins(me.Owner.Idx, player_.Idx, player_:GetDeathPenaltyCoinLoss())

                        -- tapping all items + characters
                        TapCard(player_.Character.IPID)
                        local items = FS.F.Items():ControlledBy(player_.Idx):Do()
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
