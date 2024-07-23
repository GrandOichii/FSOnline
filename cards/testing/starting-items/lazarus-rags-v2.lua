function _Create()
    return FS.B.Item()
        :TriggeredAbility(
            FS.B.TriggeredAbility('Each time you die, after paying penalties, gain +1 Treasure.')
                .On:PlayerDeath(function (me, player, args)
                    return player.Idx == args.Player.Idx
                end)
                .Effect:Common(
                    FS.C.Effect.GainTreasure(1)
                )
            :Build()
        )
        :Label(FS.Labels.Eternal)
    :Build()
end