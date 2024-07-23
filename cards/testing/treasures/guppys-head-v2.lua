-- status: implemented
-- TODO too low level

function _Create()
    return FS.B.Item()
        :ActivatedAbility(
            FS.B.ActivatedAbility('{T}', 'Choose a player. That player gives you a loot card.')
                .Cost:Common(
                    FS.C.Cost.Tap()
                )
                .Target:Player(
                    function (me, player)
                        return FS.F.Players():Do()
                    end
                )
                .Effect:Custom(
                    function (stackEffect)
                        -- TODO
                        local player_idx = tonumber(stackEffect.Targets[0].Value)
                        local hand = GetPlayer(player_idx).Hand
                        local indicies = {}
                        for i = 0, hand.Count - 1 do
                            indicies[#indicies+1] = i
                        end
                        if #indicies == 0 then
                            return false
                        end
                        local choice = ChooseCardInHand(player_idx, indicies, 'Choose a card to give to '..GetPlayer(stackEffect.OwnerIdx).Name)
                        local card = GetPlayer(player_idx).Hand[choice].Card
                        RemoveFromHand(player_idx, choice)
                        AddToHand(stackEffect.OwnerIdx, card)
                    end
                )
            :Build()
        )
    :Build()
end