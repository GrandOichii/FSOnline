function _Create()
    return FS.B.Item()
        .Static:Raw(
            FS.ModLayers.ITEM_DESTRUCTION_REPLACEMENT_EFFECTS,
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