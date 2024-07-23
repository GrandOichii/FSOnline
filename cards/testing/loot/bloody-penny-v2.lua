-- status: implemented
-- TODO too low-level

function _Create()
    return FS.B.Loot()
        :Trinket()
        :TriggeredAbility(
            FS.B.TriggeredAbility('Each time a player dies, before paying penalties. Loot 1.')
                .On:PlayerDeathBeforePenalties(function (me, player, args)
                    return true
                end)
                .Effect:Common(
                    FS.C.Effect.Loot(1)
                )
            :Build()
        )
    :Build()
end
