-- status: implemented

function _Create()
    return FS.B.Character()
        :Basic()
        .Static:Raw(
            FS.ModLayers.PLAYER_ATTACK,
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
                me.Owner.State.Stats.Attack = me.Owner.State.Stats.Attack + mod
            end
        )
    :Build()
end