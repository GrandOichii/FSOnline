-- status: not tested
-- TODO too low-level

function _Create()
    return FS.B.Character()
        :Basic()
        :OnStart(function (player)
            local cards = RemoveTopCards(FS.DeckIDs.TREASURE, 3)
            local extract = function (card)
                return card.LogName
            end
            -- TODO change to actual card prompt
            local choices = {}
            for _, card in ipairs(cards) do
                choices[#choices+1] = extract(card)
            end
            local choice = PromptString(player.Idx, choices, 'Choose your starting item')
            for _, card in ipairs(cards) do
                if extract(card) == choice then
                    -- TODO change
                    -- TODO grant eternal label
                    PutOnTop(FS.DeckIDs.TREASURE, card)
                    GainTreasure(player.Idx, 1)
                else
                    PutToBottom(FS.DeckIDs.TREASURE, card)
                end
            end
        end)
        :Build()
end