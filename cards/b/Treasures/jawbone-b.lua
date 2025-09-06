function _Create()
    return FS.B.Item()
        :ActivatedAbility(
            FS.B.ActivatedAbility('{T}', 'Steal 3{cent} from a player.')
                .Cost:Common(
                    FS.C.Cost.Tap()
                )
                .Target:Player(
                    function (me, player)
                        return FS.F.Players():CoinsGte(3):Do()
                    end
                )
                .Effect:Common(
                    FS.C.Effect.StealCoinsFromTarget(0, 3)
                )
            :Build()
        )
    :Build()
end