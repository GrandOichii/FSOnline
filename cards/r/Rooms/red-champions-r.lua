-- status: implemented

function _Create()
    return FS.B.Room()
        .Static:Common(
            'Monsters have +1{health}.',
            FS.C.StateMod.ModMonsterHealth(
                function (me, monster)
                    return 1
                end,
                FS.C.AllMonsters
            )
        )
    :Build()
end