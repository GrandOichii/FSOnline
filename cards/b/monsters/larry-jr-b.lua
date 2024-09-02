-- status: implemented
-- TODO too low-level

function _Create()
    return FS.B.Monster()
        .Static:Raw(
            FS.ModLayers.MONSTER_EVASION,
            'While this is at 2{health} or less, it has +1{evasion}.',
            function (me)
                if me.Stats:GetCurrentHealth() > 2 then
                    return
                end
                me.Stats.State.Evasion = me.Stats.State.Evasion + 1
            end
        )
        :Reward(
            FS.B.Reward('6{cent}')
                .Effect:Common(
                    FS.C.Effect.GainCoins(6)
                )
            :Build()
        )
    :Build()
end