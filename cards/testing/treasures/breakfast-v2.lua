-- status: implemented

function _Create()
    return FS.B.Item()
        -- +1{health}.
        .Static:Raw(
            FS.ModLayers.PLAYER_MAX_HEALTH,
            function (me)
                me.Owner.State.Stats.Health = me.Owner.State.Stats.Health + 1
            end
        )
    :Build()
end