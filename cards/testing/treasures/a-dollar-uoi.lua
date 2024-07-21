-- status: implemented
-- TODO too low level

function _Create()
    return FS.B.Item()
        :ActivatedAbility(
            FS.B.ActivatedAbility('Destroy this', 'Steal all shop items.')
                .Cost:Common(
                    FS.C.Cost:DestroyMe()
                )
                .Effect:Custom(function (stackEffect)
                    local items = FS.F.Items():InShop():Do()
                    for _, item in ipairs(items) do
                        StealItem(stackEffect.OwnerIdx, item.IPID)
                    end
                end)
            :Build()
        )
    :Build()
end
