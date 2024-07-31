-- status: implemented
-- TODO too low-level

function _Create()
    return FS.B.Item()
        :TriggeredAbility(
            FS.B.TriggeredAbility('Each time you take damage, choose one-\nGain 2{cent}.\nLoot 1, then discard a loot card.\nGain +2{attack} till end of turn.')
                .On:PlayerDamaged(function (me, player, args)
                    return player.Idx == args.Player.Idx
                end)
                :Choose(
                    FS.C.Choose.Effect(
                        {
                            label = 'Gain 2{cent}.',
                            effects = {
                                FS.C.Effect.GainCoins(2)
                            }
                        },
                        {
                            label = 'Loot 1, then discard a loot card',
                            effects = {
                                function (stackEffect)
                                    FS.C.Effect.Loot(1)(stackEffect)
                                    FS.C.Effect.Discard(1)(stackEffect)
                                    return true
                                end
                            }
                        },
                        {
                            label = 'Gain +2{attack} till end of turn.',
                            effects = {
                                function (stackEffect)
                                    FS.C.Effect.TillEndOfTurn(
                                        FS.ModLayers.PLAYER_ATTACK,
                                        function ()
                                            local player = GetPlayer(stackEffect.OwnerIdx)
                                            player.Stats.State.Attack = player.Stats.State.Attack + 2
                                        end
                                    )(stackEffect)
                                end
                            }
                        }
                    )
                )
            :Build()
        )
        :Label(FS.Labels.Eternal)
    :Build()
end
