// Lua
#include "../headers/QuickLuaInclude.hpp"

// Kthura
#include "../headers/QuickLuaInclude.hpp"
#include "../headers/Script.hpp"
#include "../headers/UserInterface.hpp"
#include "../headers/Config.hpp"

// Stuff
#include <jcr6_core.hpp>

using namespace jcr6;
using namespace std;



namespace KthuraEdit {

#pragma region Local variables
static lua_State* LState{ nullptr };
static JT_Dir LDir;
#pragma endregion


#pragma region Patch it all up
	static void JCRScriptPatch(string f){
		cout << "Adding script library directory: " << f << endl;
		auto jd = Dir(f);
		LDir.PatchDir(jd);
	}

	static void JCRScriptPatchList(vector<string> *Lst){
		for (auto &d : *Lst) JCRScriptPatch(d);
	}

	static int Script_Paniek(lua_State* L) {
		std::string Trace = "";
		// /* DEBUG
		cout << lua_gettop(L) << "\n";
		for (int i = 1; i <= lua_gettop(L); i++) {
			cout << "Arg #" << i << "\t";
			switch (lua_type(L, i)) {
			case LUA_TSTRING:
				cout << "String \"" << luaL_checkstring(L, i);
				Trace += luaL_checkstring(L, i); Trace += "\n";
				break;
			case LUA_TNUMBER:
				cout << "Number " << luaL_checknumber(L, i);
			case LUA_TFUNCTION:
				cout << "Function";
			default:
				cout << "Unknown: " << lua_type(L, i);
				break;
			}
			cout << "\n";
		}
		// */
		// Normally this should not happen, but just in case!
		// The "Lua Panic!" prefix is to make sure I know this happened.
		//auto err = luaL_checkstring(L, 1);
		std::string Paniek = "Lua Panic!\n\n";
		//Paniek += err;
		UI::Crash(Paniek+"\n\nLua Dump:\n" + Trace);
		return 0;
	}
#pragma endregion

#pragma region Init & Done
	void InitScript() {
		JCRScriptPatchList(&Config::ScriptLibPath());
		cout << "Initiating scripting engine\n";
		LState = luaL_newstate();
		if (LState == NULL) {
			std::cout << "\x1b[31mLua Error\x1b[0m " << "Cannot create state: not enough memory\n";
			UI::Crash("Lua could not create a new state: Not enough memory");
			return; // Now "Crash" should already end all stuff, but ya never know, so to make sure!
		}
		cout << "= Setting up default libraries\n";
		luaL_openlibs(LState);
		cout << "= Setting up panic\n";
		lua_atpanic (LState, Script_Paniek);
		cout << "= Loading Neil\n";
		auto Neil{ Config::JCR()->String("Script/Neil.lua") };
		luaL_loadstring(LState, Neil.c_str());
		lua_call(LState, 0, 0);

	}

	void DoneScript() {
		lua_close(LState);
	}
#pragma endregion
}