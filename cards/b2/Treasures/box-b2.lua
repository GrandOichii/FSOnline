-- status: implemented
-- TODO too low-level

function _Create()
    return FS.B.Item()
        :ActivatedAbility(
            FS.B.ActivatedAbility('{T}', 'Destroy this. If you do, you may play any number of additional loot cards till end of turn.')
                .Cost:Common(
                    FS.C.Cost.Tap()
                )
                .Effect:Custom(function (stackEffect)
                    local owner = GetPlayer(stackEffect.OwnerIdx)
                    local destroyed = FS.C.Cost.DestroyMe().Pay(stackEffect.Card, owner, stackEffect)
                    if not destroyed then
                        return
                    end

                    TillEndOfTurn(
                        FS.ModLayers.MOD_MAX_LOOT_PLAYS,
                        function ()
                            owner.State.UnlimitedLootPlays = true
                        end
                    )
                end)
            :Build()
        )
    :Build()
end

