-- status: implemented

function _Create()
    return FS.B:BonusSoul()
        :Check(function (player)
            return #FS.F.Items()
                :ControlledByPlayer()
                :ControlledBy(player.Idx)
                :Labeled(FS.Labels.Guppys)
                :Do() >= 2
        end)
    :Build()
end