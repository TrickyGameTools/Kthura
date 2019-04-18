

--[[

Base Script.
This script contains just a few quick functions all project 
scripts written in Lua should contain. Since I do not want to write
everything in the underlying APIs.

]]

-- Locals appear to be faster than globals
local table = table
local os    = os
local math  = math

function NOTHING() end -- You'd be amazed how handy this will be!

function ASH(value)
	if type(value)=="nil" then return nil
	elseif type(value)=="boolean" then
		if value then return "true" else return "false" end
	elseif type(value)=="string" then return value
	elseif type(value)=="number" then return ""..value -- Guarantees a string
	else return "<<"..type(value)..">>" end
end

-- Using os.exit() is not desirable
function os.exit()
    error("os.exit() has been disabled for security reasons!")
end


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

function reveach(a) -- each in reversed order!
   assert(type(a)=="table","HEY! 'reveach' requires a table. Not a "..type(a).."!");
   local acopy={}
   for i,c in ipairs(a) do
	acopy[i]=c
   end
   local idx=#acopy+1
   return function()
      idx=idx-1
      if (idx<=0) then return nil end
      return acopy[idx]
   end
end

function spairs(a,func)
   assert(type(a)=="table","HEY! 'spairs' requires a table. Not a "..type(a).."!");
   local keys = {}
   local acopy = {}
   for k,v in pairs(a) do
      keys[#keys+1]=k
      acopy[k]=v
   end
   if (func) then
      assert(type(func)=="function","HEY! I need a function for sorting. Not a "..type(func).."!")
      table.sort(keys,func)
   else
      table.sort(keys)
   end   
   local idx=0
   return function()
      idx=idx+1
      if keys[idx]==nil then return nil,nil end
      return keys[idx],acopy[keys[idx]]
   end
end   

function GenerateKey(prefix)
   assert(type(prefix)=="string", "GenerateKey("..ASH(prefix).."): I only accept strings for a prefix and a "..type(prefix))
   local ret = Kthura:GenKey(prefix)
   assert(ret and ret~="","GenerateKey("..ASH(prefix).."): Failed!")
end   

function Use(file)
	assert(type(file)=="string", "I expected a string for a file name! Not a "..type(file).."!")
	print("Using "..file)
	local script = Kthura:GetScriptToUse(file)
	return load(script,file)
end

function EditorBuild()
	return Kthura.Build
end

function SelectedLayer()
	return Kthura.LayerName
end

function Remap()
	Kthura:Remap()
end


-- Destroy import function for safety reasons
import = function() end
print("  = Lua script loaded succesfully")
