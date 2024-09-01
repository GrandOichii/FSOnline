-- status: implemented
-- TODO too low level

function _Create()
    return FS.B.Item()
        :ActivatedAbility(
            FS.B.ActivatedAbility('{T}', 'Choose a player, then roll- that player gains {cent} equal to the result.')
                .Cost:Common(
                    FS.C.Cost.Tap()
                )
                .Target:Player(
                    function (me, player)
                        return FS.F.Players():Do()
                    end
                )
                .Effect:Roll(
                    function (stackEffect)
                        local roll = stackEffect.Rolls[0]
                        FS.C.Effect.GainCoins(roll)(stackEffect)
                    end
                )
            :Build()
        )
        :Label(FS.Labels.Eternal)
    :Build()
end