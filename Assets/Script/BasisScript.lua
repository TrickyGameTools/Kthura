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
   for _,cnt in ipairs(params) do
       content = content .. cnt
   end
   Kthura:KthuraPrint(content)
end
local print=print -- locals are faster than globals

function each(a)
   assert(type(a)=="table","HEY! 'each' requires a table. Not a "..type(a).."!");
   local acopy={}
   for i,c in ipairs(a) do
	acopy[i]=c
   end
   local idx=0
   return function()
      idx=idx+1
      return acopy[idx]
   end
end


--[[CONTENT]]


-- Destroy import function for safety reasons
import = function() end
print("  = Lua script loaded succesfully")
