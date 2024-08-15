-- status: implemented

function _Create()
    return FS.B.Item()
        :ActivatedAbility(
            FS.B.ActivatedAbility('{T}', 'Change the result of a dice roll to a 1 or 6.')
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
                            option = 'Set to 1',
                            modFunc = function (roll)
                                return 1
                            end
                        },
                        {
                            option = 'Set to 6',
                            modFunc = function (roll)
                                return 6
                            end
                        }
                    })
                )
                :Build()
        )
    :Build()
end