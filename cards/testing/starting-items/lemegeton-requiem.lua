-- status: implemented
-- TODO too low-level

function _Create()
    return FS.B.Item()
        :ActivatedAbility(
            FS.B.ActivatedAbility('{T}', 'Gain +1 treasure. The next time your turn ends, destroy a non-eternal item you control.')
                .Cost:Common(
                    FS.C.Cost.Tap()
                )
                .Effect:Common(
                    FS.C.Effect.GainTreasure(1)
                )
                .Effect:Custom(
                    function (stackEffect)
                        local ownerIdx = stackEffect.OwnerIdx
                        AtEndOfTurn(ownerIdx, function ()
                            local ipids = FS.C.IPIDs(
                                FS.F.Items()
                                    :ControlledBy(ownerIdx)
                                    :Do()
                            )
                            if #ipids == 0 then
                                return
                            end

                            local ipid = ChooseItem(ownerIdx, ipids, 'Choose an item to destroy')
                            DestroyItem(ipid)
                            
                            return
                        end)
                    end
                )
            :Build()
        )
        :Label(FS.Labels.Eternal)
    :Build()
end