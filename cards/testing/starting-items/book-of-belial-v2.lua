-- status: implemented

function _Create()
    return FS.B.Item()
        :ActivatedAbility(
            FS.B.ActivatedAbility('{T}', 'Add or subtract 1 from a roll.')
                .Cost:Common(
                    FS.C.Cost.Tap()
                )
                .Target:StackEffect(
                    function (me, player)
                        return FS.F.StackEffect():Rolls():Do()
                    end
                )
                .Effect:Common(
                    FS.C.Effect.ModifyTargetRoll(0, {
                        {
                            option = '+1',
                            mod = 1
                        },
                        {
                            option = '-1',
                            mod = -1
                        }
                    })
                )
                :Build()
        )
        -- TODO add trigger
        :Label(FS.Labels.Eternal)
    :Build()
end