-- status: implemented
-- TODO too low-level

function _Create()
    return FS.B.Item()
        :ActivatedAbility(
            FS.B.ActivatedAbility('{T}', 'Pay 1{health}. If you do, choose a player. Prevent the next instance of up to 2 damage they would take this turn.')
                .Cost:Common(
                    FS.C.Cost.Tap()
                )
                .Target:Player(
                    function (me, player)
                        return FS.F.Players():Do()
                    end
                )
                .Effect:Custom(
                    function (stackEffect)
                        local healthCost = 1
                        local accept = FS.C.Choose.YesNo(stackEffect.OwnerIdx, 'Pay '..healthCost..'{health}?')
                        if not accept then
                            return false
                        end

                        LoseLife(stackEffect.OwnerIdx, healthCost)
                        FS.C.Effect.PreventNextDamageToTargetPlayer(0, 2)(stackEffect)
                    end
                )
            :Build()
        )
        :Label(FS.Labels.Guppys)
    :Build()
end