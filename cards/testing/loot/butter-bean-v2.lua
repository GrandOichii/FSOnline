-- status: implemented
-- TODO a little bit too low-level

function _Create()
    return FS.B.Loot()
        .Target:StackEffect(function (me, player)
            return FS.F.StackEffects():Custom(function (se)
                return IsAbilityActivation(se) or IsLootStackEffect(se)
            end):Do()
        end)
        .Effect:Common(
            FS.C.Effect.CancelTargetStackEffect(0)
        )
    :Build()
end