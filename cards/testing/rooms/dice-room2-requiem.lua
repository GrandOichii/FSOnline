-- status: implemented

function _Create()
    return FS.B.Room()
        :TriggeredAbility(
            FS.B.TriggeredAbility('When this enters play, each player discards their hands and loots 3.')
                .On:MeEnteringPlay()
                .Effect:Common(
                    FS.C.Effect.Wheel(
                        3,
                        function (stackEffect, args)
                            -- TODO? does player order matter here
                            return FS.F.Players():Do()
                        end
                    )
                )
            :Build()
        )
        :TriggeredAbility(
            FS.B.TriggeredAbility('At the end of the turn, put this into discard.')
                .On:TurnEnd()
                .Effect:Common(
                    FS.C.Effect.DiscardMeFromPlay()
                )
            :Build()
        )
    :Build()
end