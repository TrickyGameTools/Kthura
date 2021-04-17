// Lic:
// Kthura Map Editor
// Scripting Engine
// 
// 
// 
// (c) Jeroen P. Broks, 2021
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
// 
// Please note that some references to data like pictures or audio, do not automatically
// fall under this licenses. Mostly this is noted in the respective files.
// 
// Version: 21.04.17
// EndLic
// Lua
#include "../headers/QuickLuaInclude.hpp"

// Kthura
#include "../headers/QuickLuaInclude.hpp"
#include "../headers/Script.hpp"
#include "../headers/UserInterface.hpp"
#include "../headers/Config.hpp"

// Stuff
#include <jcr6_core.hpp>
#include <QuickString.hpp>
#include <QuickStream.hpp>

using namespace jcr6;
using namespace std;
using namespace TrickyUnits;



namespace KthuraEdit {

#pragma region Local variables
	enum class ScriptKind { Unknown, Auto, Neil, Lua };
	static ScriptKind LKind{ ScriptKind::Unknown };
	static lua_State* LState{ nullptr };
	static string LFile{ "<???>" };
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

#pragma region Execution
	static void ExeString(std::string source, std::string Chunk="",ScriptKind Kind=ScriptKind::Auto) {
		std::string work = "--[[Kthura Script Load String]]\n";
		if (Kind == ScriptKind::Auto) Kind = LKind;
		if (Chunk == "") Chunk = LFile;
		switch (Kind) {
		case ScriptKind::Lua:
			work += "local success,workfunc = xpcall(load,Panic,\"" + bsdec(source) + "\",\"" + Chunk + "\")\n";
			break;
		case ScriptKind::Neil:
			work += "local success,workfunc = xpcall(Neil.Load,Panic,\"" + bsdec(source) + "\",\"" + Chunk + "\")\n";
			break;
		default:
			UI::Crash("State Type has not been recognized: " + to_string((int)LKind));
		}
		work += "\n\nlocal e\n";
		work += "success,e=xpcall(workfunc,Panic)\n";
		// cout << "<LOADED SCRIPT>\n" << work << "</LOADED SCRIPT>\n";
		luaL_loadstring(LState, work.c_str());
		lua_call(LState, 0, 0);
	}
#pragma endregion

#pragma region APIs (KSA = Kthura Script API)
	static int KSA_Test(lua_State* L) { cout << "Testing! Testing! One! Two! Three!\n"; return 0; }
	static int KSA_Crash(lua_State* L) { UI::Crash(luaL_checkstring(L, 1)); }
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
		cout << "= Setting up APIs\n";
		lua_register(LState, "KTH_TEST", KSA_Test);
		lua_register(LState, "KTH_CRASH", KSA_Crash);
		cout << "= Loading Neil\n";
		auto Neil{ "--[[NEIL]]\t\tlocal function LoadNeil()\t\t"+Config::JCR()->String("Script/Neil.lua")+"\n\nend\nNeil = LoadNeil()" };
		luaL_loadstring(LState, Neil.c_str());
		lua_call(LState, 0, 0);
		cout << "= Compiling Panic Escape\n";
		auto PanicScript{
			"--[[ Panic ]]\n"
			"function Panic(errormessage)\n"
			"\tKth.Crash(errormessage..\"\\n\\n\"..debug.traceback())\n"
			"end\n"
			//"print(type(Panic)..' Panic')\n" // debug
		};
		//cout << "<Panic>\n" << PanicScript << "\n</Panic>\n";
		luaL_loadstring(LState, PanicScript);
		lua_call(LState, 0, 0);
		cout << "= Compiling base script\n";
		auto BaseScript{ Config::JCR()->String("Script/BasisScript.Neil") };
		ExeString(BaseScript, "Base Script", ScriptKind::Neil);
		// cout << FileExists(Config::NeilScript()) << ". " << Config::NeilScript() << endl;
		if (FileExists(Config::NeilScript())) {
			LFile = Config::NeilScript();
			LKind = ScriptKind::Neil;
			cout << "= Loading Neil Script: " << LFile << endl;
			auto scr{ LoadString(Config::NeilScript()) };
			cout << "= Compiling" << endl;
			ExeString(scr);
		} else if (FileExists(Config::LuaScript())) {
			LFile = Config::NeilScript();
			LKind = ScriptKind::Lua;
			cout << "= Loading Lua Script: " << LFile << endl;
			auto scr{ LoadString(Config::NeilScript()) };
			cout << "= Compiling" << endl;
			ExeString(scr);
		} else {
			ExeString("Init\nCout(\"= No script available!\\n\")\nEnd", "CORE SCRIPT", ScriptKind::Neil); // Seems a bit awkward, but this way I could at least if Neil scripting works at all.
		}

	}

	void DoneScript() {
		lua_close(LState);
	}
#pragma endregion
}