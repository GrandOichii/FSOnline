-- status: implemented

function _Create()
    return FS.B.Item()
        :ActivatedAbility(
            FS.B.ActivatedAbility('{T}', 'Choose a dice roll. its controller rerolls it.')
                .Cost:Common(
                    FS.C.Cost.Tap()
                )
                .Target:StackEffect(
                    function (me, player)
                        return FS.F.StackEffects():Rolls():Do()
                    end
                )
                .Effect:Common(
                    FS.C.Effect.RerollTargetRoll(0)
                )
                :Build()
        )
        -- TODO add trigger
        :Label(FS.Labels.Eternal)
    :Build()
end