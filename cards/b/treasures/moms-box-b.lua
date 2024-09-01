-- status: not tested
-- TODO too low-level


function _Create()
    return FS.B.Item()
        :TriggeredAbility(
            FS.B.TriggeredAbility('When anyone rolls a {roll:4}, you may loot 1 and then discard a loot card.')
                .On:RollOfValue(4)
                .Effect:Custom(
                    function (stackEffect)
                        local accept = FS.C.Choose.YesNo(stackEffect.OwnerIdx, 'Loot 1 and discard a loot card?')
                        if not accept then
                            return false
                        end

                        if not FS.C.Effect.Loot(1)(stackEffect) then
                            return false
                        end

                        return FS.C.Effect.Discard(1)(stackEffect)
                    end
                )
            :Build()
        )
    :Build()
end