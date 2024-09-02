-- status: not tested

function _Create()
    return FS.B.Item()
        :ActivatedAbility(
            FS.B.ActivatedAbility('{T}', 'Gain 1{cent}.')
                .Cost:Common(
                    FS.C.Cost.Tap()
                )
                .Effect:Common(
                    FS.C.Effect.GainCoins(1)
                )
            :Build()
        )
        :TriggeredAbility(
            FS.B.TriggeredAbility('When any player rolls a {roll:1}, you may recharge this.')
                .On:RollOfValue()
                .Effect:Common(
                    FS.C.Effect.RechargeMe(true)
                )
            :Build()
        )
    :Build()
end