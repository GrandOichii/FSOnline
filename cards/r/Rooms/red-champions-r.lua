-- status: implemented

function _Create()
    return FS.B.Room()
        .Static:Common(
            FS.C.StateMod.ModMonsterHealth(
                function (me, monster)
                    return 1
                end,
                FS.C.AllMonsters
            )
        )
    :Build()
end