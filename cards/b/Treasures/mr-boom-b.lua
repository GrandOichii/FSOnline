function _Create()
    return FS.B.Card()
        :ActivatedAbility(
            FS.B.ActivatedAbility('{T}', 'Deal 1 damage to a monster.')
                .Cost:Common(
                    FS.C.Cost.Tap()
                )
                .Target:Monster(
                    function (me, player)
                        return FS.F.Monsters():Do()
                    end
                )
                .Effect:Common(
                    FS.C.Effect.DamageToTargetMonster(0, 1)
                )

            :Build()
        )
    :Build()
end