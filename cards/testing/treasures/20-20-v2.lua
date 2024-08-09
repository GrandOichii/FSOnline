-- status: implemented

function _Create()
    return FS.B.Item()
        :ActivatedAbility(
            FS.B.ActivatedAbility('{T}', 'Add up to 2 to an attack roll.')
                .Cost:Common(
                    FS.C.Cost.Tap()
                )
                .Target:StackEffect(
                    function (me, player)
                        return FS.F.StackEffects():AttackRolls():Do()
                    end
                )
                .Effect:Common(
                    FS.C.Effect.ModifyTargetRoll(0, {
                        {
                            option = 'Add 1',
                            modFunc = function (roll)
                                return roll + 1
                            end
                        },
                        {
                            option = 'Add 2',
                            modFunc = function (roll)
                                return roll + 2
                            end
                        }
                    })
                )
            :Build()
        )
    :Build()
end