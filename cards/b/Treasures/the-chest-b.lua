-- status: implemented
-- TODO too low level
-- TODO works differently in base game

function _Create()
    return FS.B.Item()
        .Static:Raw(
            FS.ModLayers.ITEM_DESTRUCTION_REPLACEMENT_EFFECTS,
            'If this Item is destroyed, it becomes a Soul for the player who owned it.',
            function (me)
                me.State.DestructionReplacementEffects:Add(function (card)
                    if not IsOwned(card) then
                        return false
                    end

                    RemoveFromPlay(card.IPID)
                    AddSoulCard(card.Owner.Idx, card.Card)
                    return true
                end)
            end
        )
    :Build()
end