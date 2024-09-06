-- status: not tested

function _Create()
    return FS.B.Item()
        :ActivatedAbility(
            FS.B.ActivatedAbility('Pay 5{cent}', 'Deal 1 damage to a player or monster')
                .Cost:Common(
                    FS.C.Cost.PayCoins(5)
                )
                .Target:MonsterOrPlayer(
                    
                )
                .Effect:Common(
                    FS.C.Effect.DamageToTarget(0, 1)
                )
            :Build()
        )
    :Build()
end