-- status: not tested

function _Create()
    return FS.B.Item()
        :ActivatedAbility(
            FS.B.ActivatedAbility('{T}', 'Subtract up to 2 from any roll.')
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
                            option = 'Subtract 1',
                            modFunc = function (roll)
                                return roll - 1
                            end
                        },
                        {
                            option = 'Subtract 2',
                            modFunc = function (roll)
                                return roll - 2
                            end
                        }
                    })
                )
                :Build()
        )
    :Build()
end