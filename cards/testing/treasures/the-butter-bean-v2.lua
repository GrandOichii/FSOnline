function _Create()
    return FS.B.Item()
        :ActivatedAbility(
            FS.B.ActivatedAbility('{T}', 'Roll-\n6: Recharge this.')
                .Cost:Common(
                    FS.C.Cost.Tap()
                )
                .Effect:Roll(function (stackEffect)
                    local roll = stackEffect.Rolls[0]

                    if roll ~= 6 then
                        return
                    end

                    Recharge(stackEffect.Card.IPID)
                end)
            :Build()
        )
    :Build()
end