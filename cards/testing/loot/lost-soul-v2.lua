-- status: implemented

function _Create()
    return FS.B.Loot()
        :Trinket()
        :TriggeredAbility(
            FS.B.TriggeredAbility('When this enters play, it becomes a soul.')
                .On:EnterPlay(
                    function (me, player, args)
                        return args.Card.IPID == me.IPID
                    end
                )
                .Effect:Custom(
                    function (stackEffect)
                        local me = stackEffect.Card
                        if not IsPresent(me.IPID) then
                            return false
                        end
                        
                        RemoveFromPlay(me.IPID)
                        AddSoulCard(me.Owner.Idx, me.Card)
                        return true
                    end
                )
            :Build()
        )
    :Build()
end