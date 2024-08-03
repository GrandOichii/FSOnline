-- status: implemented

function _Create()
    return FS.B.Monster()
        :TriggeredAbility(
            FS.B.TriggeredAbility('Each time a player rolls a {roll:1}, they lose 2{cent}.')
                .On:Roll(
                    function (me, player, args)
                        return args.Value == 1
                    end
                )
                .Effect:Common(
                    FS.C.Effect.LoseCoins(2, FS.C.RollOwner)
                )
            :Build()
        )
        :Reward(
            FS.B.Reward('Roll- gain x{cent}')
                .Effect:Roll(
                    function (stackEffect)
                        local roll = stackEffect.Rolls[0]
                        FS.C.Effect.GainCoins(roll, FS.C.CurrentPlayers)(stackEffect)
                    end
                )
            :Build()
        )
    :Build()
end