-- status: implemented

function _Create()
    return FS.B.Room()
        .Static:Common(
            FS.C.StateMod.ShopItemsCostsNLess(
                5,
                function (me)
                    return FS.F.Players():Current():Do()
                end
            )
        )
    :Build()
end