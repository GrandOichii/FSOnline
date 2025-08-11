-- status: implemented

-- TODO this works differently in b

function _Create()
    return FS.B.Item()
        .Static:Common(
            'Shop items you purchase cost 5{cent} less.',
            FS.C.StateMod.ShopItemsCostsNLess(5)
        )
    :Build()
end