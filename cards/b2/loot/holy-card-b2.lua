-- status: implemented

function _Create()
    return FS.B.Loot('Choose a player. The next time that player would die this turn, prevent death. If it\'s their turn, cancel everything that hasn\'t resolved and end it.')
        .Target:Player()
        .Effect:Common(
            FS.C.Effect.MantleTargetPlayer(0)
        )
    :Build()
end