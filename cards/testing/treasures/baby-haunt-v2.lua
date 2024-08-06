-- status: implemented

function _Create()
    return FS.B.Card()
        .Static:Raw(
            FS.ModLayers.MONSTER_EVASION,
            function (me)
                if me.Owner.Idx ~= GetCurPlayerIdx() then
                    return
                end
                local monsters = FS.F.Monsters():Do()
                for _, m in ipairs(monsters) do
                    m.Stats.State.Evasion = m.Stats.State.Evasion + 1
                end
            end
        )
        :TriggeredAbility(
            FS.B.TriggeredAbility('When you die, before paying penalties, give this to another player.')
                .On:PlayerDeathBeforePenalties(function (me, player, args)
                    return player.Idx == args.Player.Idx
                end)
                -- .On:PlayerDeathBeforePenalties()
                .Target:Player(function (me, player)
                    return FS.F.Players():Except(player.Idx):Do()
                end)
                .Effect:Common(
                    FS.C.Effect.GiveMeToTargetPlayer(0)
                )
            :Build()
        )
        -- TODO add triggered ability
    :Build()
end