-- status: implemented

function _Create()
    return FS.B.Item()
        :TriggeredAbility(
            FS.B.TriggeredAbility('Each time a player rolls a {roll:2}, you may deactivate an item.')
                .On:Roll(function (me, player, args)
                    return args.Value == 2
                end)
                .Target:Item(
                    function (me, player)
                        return FS.F.Items():Deactivatable():Do()
                    end
                )
                .Effect:Custom(
                    function (stackEffect)
                        -- TODO too low-level
                        local ipid = stackEffect.Targets[0].Value
                        local item = GetInPlayCard(ipid)
                        local accept = FS.C.Choose.YesNo(stackEffect.OwnerIdx, 'Deactivate '..item.LogName..'?')
                        if not accept then
                            return false
                        end

                        TapCard(ipid)
                        return true
                    end
                )
            :Build()
        )
    :Build()
end