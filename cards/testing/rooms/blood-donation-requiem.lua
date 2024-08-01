-- status: implemented

function _Create()
    return FS.B.Room()
        :ActivatedAbility(
            FS.B.ActivatedAbility('Pay 1{health}', 'Gain 3{cent}')
                .Cost:Common(
                    FS.C.Cost.PayHealth(1)
                )
                .Effect:Common(
                    FS.C.Effect.GainCoins(3)
                )
            :Build()
        )
    :Build()
end