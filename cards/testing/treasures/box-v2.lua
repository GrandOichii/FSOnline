
function _Create()
    return FS.B.Item()
        :ActivatedAbility(
            -- TODO is "destroy this" considered part of the cost?
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

                    -- TODO set LootPlays to -1
                    owner.LootPlays = -1
                end)
            :Build()
        )
    :Build()
end

