-- status: implemented

function _Create()
    return FS.B.Item()
        :ActivatedAbility(
            FS.B.ActivatedAbility('{T}', 'Deal 1 damage to a player.')
                .Cost:Common(
                    FS.C.Cost.Tap()
                )
                .Target:Player(
                    function (me, player)
                        return FS.F.Players():Do()
                    end
                )
                .Effect:Common(
                    FS.C.Effect.DamageToTargetPlayer(0, 1)
                )
            :Build()
        )
    :Build()
end