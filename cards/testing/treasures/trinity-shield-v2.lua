-- status: implemented
-- TODO too low-level

function _Create()
    return FS.B.Item()
        -- Each other player plays with their hand revealed.",
        .Static:Raw(
            FS.ModLayers.LOOT_PLAY_RESTRICTIONS,
            function (me)
                local players = FS.F.Players():Except(me.Owner.Idx):Do()
                for _, player in ipairs(players) do
                    for i = 0, player.Hand.Count - 1 do
                        local card = player.Hand[i].State
                        card.PlayRestrictions:Add(function (card_, player_)
                            return GetCurPlayerIdx() == player_.Idx
                        end)
                    end
                end
            end
        )
        .Static:Raw(
            FS.ModLayers.ITEM_ACTIVATION_RESTRICTIONS,
            function (me)
                local items = FS.F.Items()
                    :ControlledByPlayer()
                    :NotControlledBy(me.Owner.Idx)
                    :Do()
                for _, item in ipairs(items) do
                    local abilities = item.State.ActivatedAbilities
                    for i = 0, abilities.Count - 1 do
                        abilities[i].AdditionalChecks:Add(function (card, player)
                            return GetCurPlayerIdx() == player.Idx
                        end)
                    end
                end
            end
        )
    :Build()
end