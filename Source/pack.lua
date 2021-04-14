JLS.Output("FATSTORAGE:zlib")
JLS.SetJCR6OutputFile("../Releases/KthuraEdit.jcr")

-- Assets
Add("../Assets","",{Storage="zlib"})


-- Please note that this line is dependent on where you have the Neil repository
Add("/Projects/Applications/Lua/Neil/Neil.lua","Script/Neil.lua",{Storage="zlib"})
