-- status: implemented
-- TODO too low-level

function _Create()
    return FS.B.Item()
        .Static:Raw(
            FS.ModLayers.HAND_CARD_VISIBILITY,
            'Each other player plays with their hand revealed.',
            function (me)
                local players = GetPlayers()
                local indicies = {}
                for _, player in ipairs(players) do
                    indicies[#indicies+1] = player.Idx
                end
                for _, player in ipairs(players) do
                    if player.Idx ~= me.Owner.Idx then
                        for i = 0, player.Hand.Count - 1 do
                            local card = player.Hand[i].State
                            for _, idx in ipairs(indicies) do
                                card.VisibleTo:Add(idx)
                            end
                        end
                    end
                end
            end
        )
        :Label(FS.Labels.Guppys)
    :Build()
end