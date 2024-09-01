-- status: implemented

function _Create()
    return FS.B:BonusSoul()
        :Check(function (player)
            return player.Hand.Count > 10
        end)
    :Build()
end