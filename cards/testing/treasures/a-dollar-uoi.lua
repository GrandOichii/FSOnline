-- status: implemented

function _Create()
    return FS.B.Item()
        :ActivatedAbility(
            FS.B.ActivatedAbility('Destroy this', 'Steal all shop items.')
                .Cost:Common(
                    FS.C.Cost:DestroyMe()
                )
                .Effect:Common(
                    FS.C.Effect.StealShopItem()
                )
            :Build()
        )
    :Build()
end
