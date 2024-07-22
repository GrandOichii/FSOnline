function _Create()
    return FS.B:BonusSoul()
        :Check(function (player)
            if GetCurPlayerIdx() ~= player.Idx then
                return false
            end

            local rolls = player.RollHistory
            local count = 0
            for i = 0, rolls.Count - 1 do
                if rolls[i] == 1 then
                    count = count + 1
                end
            end
            return count >= 3
        end)
    :Build()
end