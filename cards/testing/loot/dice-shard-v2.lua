-- status: implemented

function _Create()
    -- Choose a dice roll. Its controller rerolls it.
    return FS.B.Loot()
        .Target:StackEffect(
            function (player)
                return FS.F.StackEffect():Rolls():Do()
            end
        )
        .Effect:Common(
            FS.C.Effect.RerollTargetRoll(0)
        )
    :Build()
end