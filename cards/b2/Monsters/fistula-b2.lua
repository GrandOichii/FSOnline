-- status: implemented
-- TODO too low-level

function _Create()
    return FS.B.Monster()
        :TriggeredAbility(
            FS.B.TriggeredAbility('Each time this takes damage, it gains +1{evasion} till end of turn.')
                .On:MonsterDamaged(function (me, player, args)
                    return me.IPID == args.Card.IPID
                end)
                .Effect:Common(
                    FS.C.Effect.ModMonsterEvasionTEOT(1)
                )
            :Build()
        )
        :Reward(
            FS.B.Reward('6{cent}')
                .Effect:Common(
                    FS.C.Effect.GainCoins(8)
                )
            :Build()
        )
    :Build()
end