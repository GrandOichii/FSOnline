-- status: not tested
-- TODO should be first player

function _Create()
    return FS.B.Character()
        :Basic()
        :StartingItem('sleight-of-hand-b')
    :Build()
end