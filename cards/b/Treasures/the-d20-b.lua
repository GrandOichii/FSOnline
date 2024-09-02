function _Create()
    return FS.B.Item()
        :ActivatedAbility(
            FS.B.ActivatedAbility('{T}', 'Destroy any item in play and replace it with the top card of the treasure deck.')
                .Cost:Common(
                    FS.C.Cost.Tap()
                )
                .Target:Item(
                    function (me, player)
                        return FS.F.Items():Rerollable():Do()
                    end
                )
                .Effect:Common(
                    FS.C.Effect.RerollTargetItem(0)
                )
            :Build()
        )
    :Build()
end