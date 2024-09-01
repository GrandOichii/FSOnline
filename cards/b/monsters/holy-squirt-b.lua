-- status: implemented

function _Create()
    return FS.B.Monster()
        :TriggeredAbility(
            FS.B.TriggeredAbility('Each time a player rolls a {roll:5}, they loot 1.')
                .On:Roll(
                    function (me, player, args)
                        return args.Value == 5
                    end
                )
                .Effect:Common(
                    FS.C.Effect.Loot(1, FS.C.RollOwner)
                )
            :Build()
        )
        :Reward(
            FS.B.Reward('Loot 2')
                .Effect:Common(
                    FS.C.Effect.Loot(2)
                )
            :Build()
        )
    :Build()
end