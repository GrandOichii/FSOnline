-- status: implemented

function _Create()
    return FS.B.Room()
        .Static:Common(
            'Monsters have +1{evasion}.',
            FS.C.StateMod.ModMonsterEvasion(
                function (me, monster)
                    return 1
                end,
                FS.C.AllMonsters
            )
        )
    :Build()
end