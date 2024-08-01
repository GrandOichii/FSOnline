-- status: implemented

function _Create()
    return FS.B.Card()
            :TriggeredAbility(
                FS.B.TriggeredAbility('Each time you die, choose another player. That player dies.')
                    .On:PlayerDeath(function (me, player, args)
                        return player.Idx == args.Player.Idx
                    end)
                    .Target:Player(function (me, player)
                        return FS.F.Players():Except(player.Idx):Do()
                    end)
                    .Effect:Common(
                        FS.C.Effect.KillTargetPlayer(0)
                    )
                :Build()
            )
            -- You don\'t lose {cent} or discard loot cards when paying the death penalty.
            .Static:Raw(
                FS.ModLayers.DEATH_PENALTY_MODIFIERS,
                function (me)
                    -- TODO too low level
                    local owner = me.Owner
                    local f = function (amount)
                        return 0
                    end
                    owner.State.DeathPenaltyCoinLoseAmountModifiers:Add(f)
                    owner.State.DeathPenaltyLootDiscardAmountModifiers:Add(f)
                end
            )
        :Label(FS.Labels.Eternal)
    :Build()
end