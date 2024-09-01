-- status: implemented

function _Create()
    return FS.B.Item()
        :TriggeredAbility(
            FS.B.TriggeredAbility('At the start of your turn, you may put any number of shop items into discard.')
                .On:ControllerTurnStart()
                .Effect:Custom(function (stackEffect)
                    for _, slot in ipairs(GetShopSlots()) do
                        local card = slot.Card
                        if card ~= nil then
                            local accept = FS.C.Choose.YesNo(stackEffect.OwnerIdx, 'Discard '..card.LogName..'?')
                            if accept then
                                DiscardFromPlay(card.IPID)
                            end
                        end
                    end
                end)
            :Build()
        )
    :Build()
end