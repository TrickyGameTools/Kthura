

--[[

Base Script.
This script contains just a few quick functions all project 
scripts written in Lua should contain. Since I do not want to write
everything in the underlying APIs.

]]



-- Sorry :P
local sprintf = string.format

-- A few internal things
function NOTHING() end -- You'd be amazed how handy this will be!

function ASH(value)
	if type(value)=="nil" then return "nil"
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

-- Just some quicker functions
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

-- API link functions
function GenerateKey(prefix)
   assert(type(prefix)=="string", "GenerateKey("..ASH(prefix).."): I only accept strings for a prefix and a "..type(prefix))
   local ret = Kthura:GenKey(prefix)
   assert(ret and ret~="","GenerateKey("..ASH(prefix).."): Failed!")
   return ret
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

function Marker(radius,x,y)
	assert(type(radius)=="number","radius must be a number!")
	assert(type(x)=="number","x must be a number!")
	assert(type(y)=="number","y must be a number!")	
	assert(Kthura:Marker(radius,x,y),("Marker(%d,%d,%d): Marker expects a radius which is an integer number from 4 till 500 which can be divided by 4"):format(radius,x,y))
end

function IsByte(b)
	return type(b)=="number" and Kthura:IsByte(b)
end

function Color(r,g,b)
	local me = sprintf("Color(%d,%d,%d): ",r,g,b)
	assert(IsByte(r),me.."Color red out of range!")
	assert(IsByte(g),me.."Color green out of range!")
	assert(IsByte(b),me.."Color blue out of range!")
	Kthura:Color(r,g,b)
end

function Ask(question,sort)
	assert(Kthura.CallBackStage=="INIT","The Ask() function may ONLY be called during the 'INIT' call back stage")
	assert(type(question)=="string","Ask requires a string for a parameter, not a "..type(question))
   Kthura:Ask(question)
   Kthura:Debug("Object script asks about: "..question)
	if (sort) then Kthura:AskSort() end
end

function HasTag(tag)
   return Kthura:HasTag(tag)
end

function KillMe()
	Kthura:KillMe()
end

-- Pivot callback
Pivot_Init = NOTHING
Pivot_Retag = NOTHING
Pivot_Remove = NOTHING

function Pivot_Create(ME,qdata)
   local gkey = GenerateKey(sprintf("Pivot_%s_",SelectedLayer()))
   Kthura:Debug("Key generated "..gkey)
   ME.Tag = gkey
   print("Pivot created: "..ME.Tag)
end

function Pivot_Show(ME)
    Color(0xff,0xff,0xff)
    Marker(8,ME.x,ME.y)
end

-- Exit callback
Exit_Retag=NOTHING
Exit_Remove=NOTHING
function Exit_Init()
   Ask("Wind")
   Ask("Tag",true)
end

function Exit_Create(ME,qdata)
   if qdata.Tag==nil or qdata.Tag=="" or HasTag(qdata.Tag) then
      print("No tag available or duplicate tag. Generating new one!")
      local gkey = GenerateKey(sprintf("Exit_%s_",SelectedLayer()))
      Kthura:Debug("Key generated "..gkey)
      qdata.Tag=gkey
   end   
   ME.Tag=qdata.Tag;
   ME.MetaData["Wind"]=qdata.Wind --ME.Wind
   print("Exit created: "..ME.Tag)
end

function Exit_Show(ME)
   Color(0xb4,0xff,0x00)
   Marker(8,ME.x,ME.y)
end


-- Destroy import function for safety reasons
import = function() end
print("  = Lua script loaded succesfully")

