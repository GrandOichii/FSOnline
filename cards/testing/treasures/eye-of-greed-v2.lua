function _Create()
    return FS.B.Item()
        :TriggeredAbility(
            FS.B.TriggeredAbility('Each time a player rolls a {roll:5}, loot 1.')
                .On:Roll(function (me, player, args)
                    return args.Value == 5
                end)
                .Effect:Common(
                    FS.C.Effect.GainCoins(3)
                )
            :Build()
        )
    :Build()
end