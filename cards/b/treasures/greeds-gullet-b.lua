-- status: not tested

function _Create()
    return FS.B.Item()
        :TriggeredAbility(
            FS.B.TriggeredAbility('Each time you die, before paying penalties, gain 8{cent}.')
                .On:ControllerDeathBeforePenalties()
                .Effect:Common(
                    FS.C.Effect.GainCoins(8)
                )
            :Build()
        )
        :Label(FS.Labels.Eternal)
    :Build()
end