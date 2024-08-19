-- status: implemented

function _Create()
    return FS.B.Monster()
        :TriggeredAbility(
            FS.B.TriggeredAbility('Each time a player rolls a {roll:1}, they gain 1{cent}.')
                .On:Roll(
                    function (me, player, args)
                        return args.Value == 1
                    end
                )
                .Effect:Common(
                    FS.C.Effect.GainCoins(1, FS.C.RollOwner)
                )
            :Build()
        )
        :Reward(
            FS.B.Reward('1{cent}')
                .Effect:Common(
                    FS.C.Effect.GainCoins(1)
                )
            :Build()
        )
    :Build()
end