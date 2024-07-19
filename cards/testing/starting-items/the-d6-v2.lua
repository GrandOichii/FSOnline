-- status: not tested

function _Create()
    return FS.B.Item()
        :ActivatedAbility(
            FS.B.ActivatedAbility('{T}', 'Loot 2, then discard 1 Loot card.')
                .Cost:Common(
                    FS.C.Cost.Tap()
                )
                .Target:StackEffect(
                    function (me, player)
                        return FS.F.StackEffect():Rolls():Do()
                    end
                )
                -- .Effect:Common(
                --     FS.C.Effect.RerollTargetRoll(0)
                -- )
                .Effect:Custom(
                    function (stackEffect)
                        -- TODO assert stack effect type
                        local effect = GetStackEffect(stackEffect.Targets[0].Value)
                        RerollDice(effect)
                    end
                )
                :Build()
        )
        -- TODO add trigger
        :Label(FS.Labels.Eternal)
    :Build()
end