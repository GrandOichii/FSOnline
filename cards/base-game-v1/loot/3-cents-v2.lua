-- status: not tested

function _Create()
    return FS.B.Loot()
        .Effect:Common(
            FS.C.GainCoins(3)
        )
        :Build()
end