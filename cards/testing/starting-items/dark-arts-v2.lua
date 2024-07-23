function _Create()
    return FS.B.Item()
        :TriggeredAbility(
            FS.B.TriggeredAbility('Each time a player rolls a {roll:6}, gain 2{cent}.')
                .On:Roll(function (me, player, args)
                    return args.Value == 6
                end)
                .Effect:Common(
                    FS.C.Effect.GainCoins(2)
                )
            :Build()
        )
        :TriggeredAbility(
            FS.B.TriggeredAbility('Each time another player dies, loot 1')
                .On:PlayerDeath(function (me, player, args)
                    return player.Idx ~= args.Player.Idx
                end)
                .Effect:Common(
                    FS.C.Effect.Loot(1)
                )
            :Build()
        )
        :Label(FS.Labels.Eternal)
    :Build()
end