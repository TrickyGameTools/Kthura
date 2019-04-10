--[[

Base Script.
This script contains just a few quick functions all project 
scripts written in Lua should contain. Since I do not want to write
everything in the underlying APIs.

]]


-- Replacing Lua's own "print" command
local LUATRUEPRINT=print
function print(a,b,c,d,e,f)
   params={a,b,c,d,e,f}
   content=""
   for a in ipairs(params) do
       content = content .. a
   end
   KthuraAPI.KthuraPrint(content)
end
local print=print -- locals are faster than globals





-- Destroy import function for safety reasons
import = function() end
print("Lua script loaded succesfully")
