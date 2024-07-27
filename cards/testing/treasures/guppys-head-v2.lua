-- status: implemented
-- TODO too low level

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
                .Effect:Custom(
                    function (stackEffect)
                        FS.C.GiveLootCards(stackEffect.OwnerIdx, tonumber(stackEffect.Targets[0].Value), 1)
                    end
                )
            :Build()
        )
    :Build()
end