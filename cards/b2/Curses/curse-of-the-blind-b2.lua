-- status: implemented

function _Create()
    return FS.B.Curse()
        .Static:Common(
            'Monsters have +1{evasion} on your turn.',
            FS.C.StateMod.ModMonsterEvasion(
                function (me, monster)
                    if me.Owner == nil then
                        return 0
                    end
                    if me.Owner.Idx ~= GetCurPlayerIdx() then
                        return 0
                    end
                    return 1
                end,
                FS.C.AllMonsters
            )
        )
    :Build()
end
