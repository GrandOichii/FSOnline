-- status: implemented

function _Create()
    return FS.B.Character()
        :Basic()
        .Static:Raw(
            FS.ModLayers.PLAYER_ATTACK,
            'While you control 1 or 2 Souls, you have +1{attack}. If you control 3+ souls, you have +2{attack} instead.',
            function (me)
                local mod = 0
                local count = GetSoulCount(me.Owner.Idx)
                if count == 0 then
                    return
                end
                mod = 1
                if count >= 3 then
                    mod = 2
                end
                me.Owner.Stats.State.Attack = me.Owner.Stats.State.Attack + mod
            end
        )
        :StartingItem('soulbond-r')
    :Build()
end