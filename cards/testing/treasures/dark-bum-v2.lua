-- status: implemented
-- TODO too low-level

function _Create()
    return FS.B.Item()
        :TriggeredAbility(
            FS.B.TriggeredAbility('At the start of your turn, roll-\n1-2: Gain 3{cent}.\n3-4: Loot 1.\n5-6: Take 1 damage.')
                .On:TurnStart()
                .Effect:Roll(function (stackEffect)
                    local roll = stackEffect.Rolls[0]
                    if roll == 1 or roll == 2 then
                        FS.C.Effect.GainCoins(3)(stackEffect)
                        return
                    end

                    if roll == 3 or roll == 4 then
                        FS.C.Effect.Loot(1)(stackEffect)
                        return
                    end

                    DealDamageToPlayer(stackEffect.OwnerIdx, 1, stackEffect)
                end)
            :Build()
        )
        :Label(FS.Labels.Eternal)
    :Build()
end