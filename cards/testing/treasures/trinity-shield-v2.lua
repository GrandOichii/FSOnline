-- status: partially implemented
-- TODO too low-level


function _Create()
    return FS.B.Item()
        -- Each other player plays with their hand revealed.",
        .Static:Raw(
            FS.ModLayers.LOOT_PLAY_RESTRICTIONS,
            function (me)
                local players = GetPlayers()
                for _, player in ipairs(players) do
                    if player.Idx ~= me.Owner.Idx then
                        for i = 0, player.Hand.Count - 1 do
                            local card = player.Hand[i].State
                            card.PlayRestrictions:Add(function (card_, player_)
                                return GetCurPlayerIdx() == player_.Idx
                            end)
                        end
                    end
                end
            end
        )
    :Build()
end