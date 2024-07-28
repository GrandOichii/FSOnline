-- status: implemented
-- TODO too low-level

function _Create()
    return FS.B.Loot()
        -- Roll-
        -- 1-2: You gain +1{attack} till end of turn.
        -- 3-4: You gain +1{health} till end of turn.
        -- 5-6: Take 1 damage.",
        .Effect:Roll(function (stackEffect)
            local roll = stackEffect.Rolls[0]

            if roll == 1 or roll == 2 then
                FS.C.Effect.TillEndOfTurn(
                    FS.ModLayers.PLAYER_ATTACK,
                    function ()
                        local player = GetPlayer(stackEffect.OwnerIdx)
                        player.State.Stats.Attack = player.State.Stats.Attack + 1
                    end
                )(stackEffect)
                return
            end

            if roll == 3 or roll == 4 then
                FS.C.Effect.TillEndOfTurn(
                    FS.ModLayers.PLAYER_MAX_HEALTH,
                    function ()
                        local player = GetPlayer(stackEffect.OwnerIdx)
                        player.State.Stats.Health = player.State.Stats.Health + 1
                    end
                )(stackEffect)
                return
            end

            FS.C.Effect.DamageToPlayer(1)(stackEffect)
        end)
        -- .Effect:Roll(
        --     FS.C.Effect.SwitchRoll(0, {
        --         {
        --             [1] = FS.C.Effect.TillEndOfTurn(FS.C.)
        --         }
        --     })
        -- )
    :Build()
end