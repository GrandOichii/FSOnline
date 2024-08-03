-- status: implemented
-- TODO too low-level

function _Create()
    return FS.B.Monster()
        :TriggeredAbility(
            FS.B.TriggeredAbility('Each time this takes damage, it gains +1{attack} till end of turn.')
                .On:MonsterDamaged(function (me, player, args)
                    return me.IPID == args.Card.IPID
                end)
                .Effect:Custom(
                    function (stackEffect)
                        local me = stackEffect.Card
                        TillEndOfTurn(
                            FS.ModLayers.MONSTER_ATTACK,
                            function ()
                                me.Stats.State.Attack = me.Stats.State.Attack + 1
                            end
                        )
                    end
                )
            :Build()
        )
        :Reward(
            FS.B.Reward('6{cent}')
                .Effect:Common(
                    FS.C.Effect.GainCoins(8, FS.F.CurrentPlayers)
                )
            :Build()
        )
    :Build()
end