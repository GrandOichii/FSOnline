-- status: implemented
-- TODO too low-level

function _Create()
    return FS.B.Item()
        .Static:Raw(
            FS.ModLayers.ROLL_RESULT_MODIFIERS,
            'You have +1 to attack rolls.',
            function (me)
                me.Owner.State.RollResultModifiers:Add(
                    function (roll, stackEffect)
                        if not stackEffect.IsAttackRoll then
                            return roll
                        end
                        return roll + 1
                    end
                )
            end
        )
    :Build()
end