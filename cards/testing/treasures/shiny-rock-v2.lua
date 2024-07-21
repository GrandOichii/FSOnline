function _Create()
    return FS.B.Item()
        :TriggeredAbility(
            FS.B.TriggeredAbility('Each time a player rolls a {roll:1}, gain 3{cent}.')
                .On:ItemActivation(function (me, player, args)
                    return args.Item.Owner.Idx == me.Owner.Idx
                end)
                .Effect:Common(
                    FS.C.Effect.GainCoins(1)
                )
            :Build()
        )
    :Build()
end