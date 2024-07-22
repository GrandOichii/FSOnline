-- status: implemented

function _Create()
    return FS.B.Loot()
        :Trinket()
        :TriggeredAbility(
            FS.B.TriggeredAbility('When this enters play, expand shop slots by 2.')
                .On:EnterPlay(
                    function (me, player, args)
                        return args.Card.IPID == me.IPID
                    end
                )
                .Effect:Common(
                    FS.C.Effect.ExpandShotSlots(2)
                )
            :Build()
        )
        :TriggeredAbility(
            FS.B.TriggeredAbility('Each time another player purchases a shop item, gain 2{cent} and loot 1.')
                .On:Purchase(
                    function (me, player, args)
                        return me.Owner.Idx ~= args.Player.Idx
                    end
                )
                .Effect:Common(
                    FS.C.Effect.GainCoins(2),
                    FS.C.Effect.Loot(1)
                )
            :Build()
        )
        -- TODO add second triggered ability
    :Build()
end