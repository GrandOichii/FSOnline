-- status: implemented
-- TODO too low-level

function _Create()
    return FS.B.Item()
        -- Shop items you purchase cost 5{cent} less.
        .Static:Common(
            FS.C.StateMod.ShopItemsCostsNLess(5)
        )
    :Build()
end