-- status: implemented

function _Create()
    return FS.B.Card()
        .Static:Raw(
            FS.ModLayers.MONSTER_EVASION,
            'Monsters have +1{evasion} on your turn.',
            function (me)
                if me.Owner.Idx ~= GetCurPlayerIdx() then
                    return
                end
                local monsters = FS.F.Monsters():Do()
                for _, m in ipairs(monsters) do
                    m.Stats.State.Evasion = m.Stats.State.Evasion + 1
                end
            end
        )
        :Haunt()
        -- TODO add triggered ability
    :Build()
end