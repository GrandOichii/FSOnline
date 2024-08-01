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
                .Effect:Common(
                    FS.C.Effect.DeactivateTarget(0, true)
                )
            :Build()
        )
    :Build()
end