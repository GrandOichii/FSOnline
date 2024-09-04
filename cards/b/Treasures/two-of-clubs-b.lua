-- status: implemented, requires testing
-- TODO too low-level

function _Create()
    return FS.B.Item()
        :ActivatedAbility(
            FS.B.ActivatedAbility('{T}', 'Double the number of loot cards a player would draw, till the end of turn.')
                .Cost:Common(
                    FS.C.Cost.Tap()
                )
                .Target:Player()
                .Effect:Custom(
                    function (stackEffect)
                        TillEndOfTurn(
                            FS.ModLayers.LOOT_AMOUNT,
                            function ()
                                local idx = tonumber(stackEffect.Targets[0].Value)
                                local pState = GetPlayer(idx).State
                                pState.LootAmountModifiers:Add(function (player, amount, reason)
                                    return amount * 2
                                end)
                            end
                        )
                    end
                )
            :Build()
        )
    :Build()
end