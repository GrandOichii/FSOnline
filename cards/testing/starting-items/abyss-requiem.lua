-- status: implemented
-- TODO too low-level

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
        .Static:Raw(
            FS.ModLayers.PLAYER_ATTACK,
            function (me)
                local amount = GetCountersCount(me.IPID)
                me.Owner.Stats.State.Attack = me.Owner.Stats.State.Attack + amount / 2
            end
        )
        :Label(FS.Labels.Eternal)
    :Build()
end