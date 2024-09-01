-- status: not tested
-- TODO too low-level

function _Create()
    return FS.B.Monster()
        .Static:Raw(
            FS.ModLayers.MONSTER_EVASION,
            function (me)
                if me.Stats:GetCurrentHealth() ~= 1 then
                    return
                end
                me.Stats.State.Attack = me.Stats.State.Attack + 1
            end
        )
        :Reward(
            FS.B.Reward('5{cent}')
                .Effect:Common(
                    FS.C.Effect.GainCoins(5)
                )
            :Build()
        )
    :Build()
end