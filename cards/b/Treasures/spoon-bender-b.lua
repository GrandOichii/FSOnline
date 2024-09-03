-- status: not tested

function _Create()
    return FS.B.Item()
        :ActivatedAbility(
            FS.B.ActivatedAbility('{T}', 'Add 1 to any dice roll.')
                .Cost:Common(
                    FS.C.Cost.Tap()
                )
                .Target:StackEffect(
                    function (me, player)
                        return FS.F.StackEffects():Rolls():Do()
                    end
                )
                .Effect:Common(
                    FS.C.Effect.ModifyTargetRoll(0, {
                        {
                            option = 'Add 1',
                            modFunc = function (roll)
                                return roll + 1
                            end
                        }
                    })
                )
                :Build()
        )
        :Label(FS.Labels.Eternal)
    :Build()
end