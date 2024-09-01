function _Create()
    return FS.B.Item()
        :ActivatedAbility(
            FS.B.ActivatedAbility('{T}', 'Reroll an item.')
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