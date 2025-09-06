function _Create()
    return FS.B.Monster()
        :TriggeredAbility(
            FS.B.TriggeredAbility('When this dies, the active player kills a player.')
                .On:ThisDies()
                .Target:Player(
                    function (me, player)
                        return FS.F.Players():Killable():Do()
                    end
                )
                .Effect:Common(
                    FS.C.Effect.KillTargetPlayer(0)
                )
            :Build()
        )
        :Reward(
            FS.B.Reward('+1 Treasure')
                .Effect:Common(
                    FS.C.Effect.GainTreasure(1)
                )
            :Build()
        )
    :Build()
end