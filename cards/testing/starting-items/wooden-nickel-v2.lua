function _Create()
    return FS.B.Item()
        :ActivatedAbility(
            FS.B.ActivatedAbility('{T}', 'Choose a player, then roll- that player gains {cent} equal to the result.')
                .Target:Player(
                    function (me, player)
                        return FS.F.Player():Do()
                    end
                )
                .Effect:Roll(
                    function (stackEffect)
                        local roll = stackEffect.Rolls[0]
                        local pIdx = tonumber(stackEffect.Targets[0])
                        AddCoins(pIdx, roll)
                    end
                )
            :Build()
        )
    :Build()
end