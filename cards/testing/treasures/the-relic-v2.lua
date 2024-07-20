function _Create()
    return FS.B.Item()
        :TriggeredAbility(
            FS.B.TriggeredAbility('Each time a player rolls a {roll:1}, gain 3{cent}.')
                .On:Roll(function (me, player, args)
                    return args.Value == 1
                end)
                .Effect:Common(
                    FS.C.Effect.Loot(1)
                )
            :Build()
        )
    :Build()
end