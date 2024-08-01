-- status: implemented

function _Create()
    return FS.B.Item()
        :ActivatedAbility(
            FS.B.ActivatedAbility('{T}', 'Destroy an item you control. If you do, put a counter on this.')
                .Cost:Common(
                    FS.C.Cost.Tap()
                )
                .Target:Item(
                    function (me, player)
                        return FS.F.Items():ControlledBy(player.Idx):Destructable():Do()
                    end,
                    'Choose an Item to sacrifice'
                )
                .Effect:Custom(function (stackEffect)
                    -- TODO too low-level
                    local ipid = stackEffect.Targets[0].Value
                    if not IsPresent(ipid) then
                        return false
                    end
                    local accept = FS.C.Choose.YesNo(stackEffect.OwnerIdx, 'Destroy '..GetInPlayCard(ipid).LogName..'?')
                    if not accept then
                        return false
                    end
                    DestroyItem(ipid)
                    return FS.C.Effect.PutGenericCountersOnMe(1)(stackEffect)
                end)
            :Build()
        )
        .Static:Common(
            FS.C.StateMod.ModPlayerAttack(function (me, player)
                return GetCountersCount(me.IPID) / 2
            end)
        )
        :Label(FS.Labels.Eternal)
    :Build()
end