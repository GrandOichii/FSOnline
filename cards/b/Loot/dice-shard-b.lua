function _Create()
    return FS.B.Loot('Choose a dice roll. Its controller rerolls it.')
        .Target:StackEffect(
            function (player)
                return FS.F.StackEffects():Rolls():Do()
            end
        )
        .Effect:Common(
            FS.C.Effect.RerollTargetRoll(0)
        )
    :Build()
end