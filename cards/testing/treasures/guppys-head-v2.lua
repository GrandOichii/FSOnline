-- status: implemented

function _Create()
    return FS.B.Item()
        :ActivatedAbility(
            FS.B.ActivatedAbility('{T}', 'Choose a player. That player gives you a loot card.')
                .Cost:Common(
                    FS.C.Cost.Tap()
                )
                .Target:Player(
                    function (me, player)
                        return FS.F.Players():Do()
                    end
                )
                .Effect:Common(
                    FS.C.Effect.TargetPlayerGivesLootCards(1)
                )
            :Build()
        )
    :Build()
end