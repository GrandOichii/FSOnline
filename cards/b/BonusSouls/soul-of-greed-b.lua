-- status: implemented

function _Create()
    return FS.B:BonusSoul()
        :Check(function (player)
            return player.Coins >= 25
        end)
    :Build()
end