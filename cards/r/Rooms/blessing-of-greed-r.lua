
function _Create()
    return FS.B.Room()
        :TriggeredAbility(
            FS.B.TriggeredAbility('At the start of each turn, the active player gains 3{cent}.')
                .On:TurnStart()
                .Effect:Common(
                    FS.C.Effect.GainCoins(
                        3,
                        function (stackEffect, args)
                            return FS.F.Players():Idx(args.playerIdx):Do()
                        end
                    )
                )
            :Build()
        )
    :Build()
end