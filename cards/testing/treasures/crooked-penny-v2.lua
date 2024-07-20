-- status: implemented
-- TODO too low level

function _Create()
    return FS.B.Item()
        :ActivatedAbility(
            FS.B.ActivatedAbility('{T}', 'Choose a player, then roll-\n1-3: double the amount of {cent} they have.\n4-6: that player loses all {cent}.')
                .Cost:Common(
                    FS.C.Cost.Tap()
                )
                .Target:Player(
                    function (me, player)
                        return FS.F.Players():Do()
                    end
                )
                .Effect:Roll(
                    function (stackEffect)
                        local pIdx = tonumber(stackEffect.Targets[0].Value)
                        local roll = stackEffect.Rolls[0]
                        local amount = GetPlayer(pIdx).Coins
                        if roll < 4 then
                            -- TODO? is "doubling" considered "gaining"
                            AddCoins(pIdx, amount)
                            return
                        end
                        LoseCoins(pIdx, amount)
                    end
                )
            :Build()
        )
    :Build()
end