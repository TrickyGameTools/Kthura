// Lic:
// Kthura Map Editor
// Scripting Engine
// 
// 
// 
// (c) Jeroen P. Broks, 2021, 2022
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
// Version: 22.05.13
// EndLic

#define Debug_Script

// C++
#include <string>

// Lua
#include "../headers/QuickLuaInclude.hpp"

// Kthura
#include <Kthura.hpp>

// Kthura Editor
#include "../headers//MapData.hpp"
#include "../headers/QuickLuaInclude.hpp"
#include "../headers/Script.hpp"
#include "../headers/UserInterface.hpp"
#include "../headers/Config.hpp"

#include "../headers/UI_Map.hpp"

// Stuff
#include <jcr6_core.hpp>
#include <QuickString.hpp>
#include <QuickStream.hpp>

using namespace jcr6;
using namespace std;
using namespace TrickyUnits;
using namespace NSKthura;

#ifdef Debug_Script
#define Chat(a) std::cout << "Script Debug> "<<a<<std::endl;
#else
#define Chat(a)
#endif

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
			cout << "= Neil translation\n";
			work += "local success,workfunc = xpcall(Neil.Load,KTH_CRASH,\"" + bsdec(source) + "\",\"" + Chunk + "\")\n";
			break;
		default:
			UI::Crash("State Type has not been recognized: " + to_string((int)LKind));
		}
		work += "\n\nlocal e\n";
		work += "success,e=xpcall(workfunc,Panic)\n";
		work += "print(' = Success: '..tostring(success))\n";
		work += "if not(success) then print('ERROR:',e) KTH_CRASH(e) end\n";
		// cout << "<LOADED SCRIPT>\n" << work << "</LOADED SCRIPT>\n";
		luaL_loadstring(LState, work.c_str());
		lua_call(LState, 0, 0);
	}
#pragma endregion

#pragma region APIs (KSA = Kthura Script API)
#define KSA_GetObject()\
	KthuraObject* O{ nullptr };\
	if (lua_isnumber(L, 1)) {\
		auto I = luaL_checkinteger(L, 1);\
		auto IM{ WorkMap.Layer(CurrentLayer)->GetIDPMap() };\
		if (!IM->count(I)) { UI::Crash("No object with ID number " + to_string(I)); return 0; }\
		O = (*IM)[I];\
	} else if (lua_isstring(L, 1)) {\
		auto T{ luaL_checkstring(L,1) };\
		if (!WorkMap.Layer(CurrentLayer)->HasTag(T)) { UI::Crash(string("No object with Tag ") + T); return 0; }\
		O = WorkMap.Layer(CurrentLayer)->TagMap(T);\
	} else {\
		UI::Crash("Illegal function call!");\
		return 0;\
	}

	static int KSA_Test(lua_State* L) { cout << "Testing! Testing! One! Two! Three!\n"; return 0; }
	static int KSA_Crash(lua_State* L) { UI::Crash(luaL_checkstring(L, 1)); }
	static int KSA_Color(lua_State* L) { TQSG_Color(luaL_optinteger(L, 1, 255), luaL_optinteger(L, 2, 255), luaL_optinteger(L, 3, 255)); return 0; }
	static int KSA_ColorHSV(lua_State* L) { TQSG_ColorHSV(luaL_optinteger(L, 1, 0), luaL_optnumber(L, 2, 1), luaL_optnumber(L, 3, 1)); return 0; }
	static int KSA_ObjMarker(lua_State* L) {
		KthuraObject* O{ nullptr };
		if (lua_isnumber(L, 1)) {
			auto I = luaL_checkinteger(L, 1);
			auto IM{ WorkMap.Layer(CurrentLayer)->GetIDMap() };
			if (!IM.count(I)) { UI::Crash("No object with ID number " + to_string(I)); return 0; }
			O = IM[I];
		} else if (lua_isstring(L, 1)) {
			auto T{ luaL_checkstring(L,1) };
			if (!WorkMap.Layer(CurrentLayer)->HasTag(T)) { UI::Crash(string("No object with Tag ") + T); return 0; }
			O = WorkMap.Layer(CurrentLayer)->TagMap(T);
		} else {
			UI::Crash("Illegal function call!");
			return 0;
		}
		int size = luaL_optinteger(L, 2, 8);
		AutoDrawMarker(O->X(), O->Y(),size);
		return 0;
	}

	static int KSA_ObjSet(lua_State* L) {
		KSA_GetObject();
		auto Fld{ Upper(luaL_checkstring(L,2)) };
		if (Fld == "KIND") O->Kind(luaL_checkstring(L, 3),true);
		else if (Fld == "TEXTURE" || Fld == "TEX" || Fld == "TEXTUREFILE")  O->Texture(luaL_checkstring(L, 3));
		else if (Fld == "DOMINANCE") O->Dominance(luaL_checkinteger(L, 3));
		else if (Fld == "R") O->R(luaL_checkinteger(L, 3) % 256);
		else if (Fld == "G") O->G(luaL_checkinteger(L, 3) % 256);
		else if (Fld == "B") O->B(luaL_checkinteger(L, 3) % 256);
		else if (Fld == "ALPHA" || Fld=="ALPHA255") O->Alpha255(luaL_checkinteger(L, 3) % 256);
		else if (Fld == "ALPHA1000") O->Alpha1000(luaL_checkinteger(L, 3) % 1001);
		else if (Fld == "TAG") {
			auto T{ luaL_checkstring(L,3) };
			if (string(T) != "" && O->GetParent()->HasTag(T) && O!=O->GetParent()->TagMap(T)) UI::Crash("Script error dupe tag definition: " + string(T));
			O->Tag(T);
			O->GetParent()->RemapTags();
			Chat("Object #" << O->ID() << " tagged as: " << T);
		}
		else UI::Crash("Unknown object field: " + Fld);
	}

	static int KSA_CreateObject(lua_State* L) {
		auto Kind{ luaL_checkstring(L,1) };
		auto NObj{ WorkMap.Layer(CurrentLayer)->RNewObject(Kind) };
		switch (NObj->EKind()) {
		case KthuraKind::Custom:
		case KthuraKind::CustomItem:
		case KthuraKind::Exit:
		case KthuraKind::Obstacle:
			NObj->Texture(luaL_checkstring(L, 4));
				// FALLTHROUGH!!!
		case KthuraKind::Pivot:
			{
				auto
					x{ luaL_checkinteger(L,2) },
					y{ luaL_checkinteger(L,3) };
				NObj->X(x);
				NObj->Y(y);
			} break;
		case KthuraKind::TiledArea:
		case KthuraKind::StretchedArea:
			NObj->Texture(luaL_checkstring(L, 6));
			// FALLTHROUGH!
		case KthuraKind::Rect:
		case KthuraKind::Zone: {
			auto
				x{ luaL_checkinteger(L,2) },
				y{ luaL_checkinteger(L,3) },
				w{ luaL_checkinteger(L,4) },
				h{ luaL_checkinteger(L,5) };
			NObj->X(x);
			NObj->Y(y);
			NObj->W(w);
			NObj->H(h);
		} break;
		default:
			UI::Crash("Kind " + string(Kind) + " cannot be used in this version of the Kthura Map Editor");
			break;
		}
		Chat("Object '" << Kind << "' created. #" << NObj->ID());
		lua_pushinteger(L,NObj->ID());
		return 1;
	}

	static int KSA_HasTag(lua_State* L) {
		lua_pushboolean(L, WorkMap.Layer(CurrentLayer)->HasTag(luaL_checkstring(L, 1)));
		return 1;
	}
#pragma endregion

#pragma region CallBacks	
	void DrawCustomItem(KthuraObject* o, int x, int y, int sx, int sy) {
		auto wgroup{ TReplace(o->Kind(), "$", "CSPOT_") };
		CallBack(wgroup + ".Draw",{"$KthuraObject(" + to_string(o->ID()) + ")"});
	}
#pragma endregion

#pragma region Init & Done
	void InitScript() {
		auto SLP{ Config::ScriptLibPath() };
		JCRScriptPatchList(&SLP);
		KthuraDraw::DrawCSpot = DrawCustomItem;
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
		lua_register(LState, "KTH_COLOR", KSA_Color);
		lua_register(LState, "KTH_COLORHSV", KSA_ColorHSV);
		lua_register(LState, "KTH_OBJMARKER", KSA_ObjMarker);
		lua_register(LState, "KTH_OBJSET", KSA_ObjSet);
		lua_register(LState, "KTH_CREATEOBJ", KSA_CreateObject);
		lua_register(LState, "KTH_HASTAG", KSA_HasTag);
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
			cout << "No scripts found '" << Config::NeilScript() << "' / " << Config::NeilScript() << "\n";
			ExeString("Init\nCout(\"= No script available!\\n\")\nEnd", "CORE SCRIPT", ScriptKind::Neil); // Seems a bit awkward, but this way I could at least if Neil scripting works at all.
		}

	}

	void DoneScript() {
		lua_close(LState);
	}

	void RawCall(std::string function, std::string parameters, int retvalues=0) {
	std:string work = "--[[RawCall]]\nif type(" + function + ")~='function' then\n\tKth.Crash(\"Callback error:\\n" + function + " is not a function but a \"..type(" + function + ")..\"\\n\\n \")\nelse\n\tlocal s,e=xpcall(" + function + ", Panic, " + parameters + ")\nreturn s\nend";
		//cout << "<RAWCALL>\n" << work << "\n</RAWCALL>\n";
		if (!LState) {
			UI::Crash("RawCall to NULL state\n\nRawCall(\"" + function + "\",\"" + parameters + "\"," + std::to_string(retvalues) + "):\n\n<Work>\n" + work + "</work>");
			return;
		}
		//cout << "<RawCall>\n" << work << "\n</RawCall>\n";
		luaL_loadstring(LState, work.c_str());
		lua_call(LState, 0, 0, retvalues);
	}

	void CallBack(std::string f, std::vector<std::string> p) {
		std::string Para{ "" };
		for(auto PR:p) { 
			if (Para != "") Para += ", ";
			if (prefixed(PR, "$"))
				Para += right(PR, PR.size() - 1);
			else
				Para += "\"" + PR + "\"";
		}
		Chat("Calling " << f << " with parameters "<< Para);
		switch (LKind) {
		case ScriptKind::Lua:
			RawCall(f,Para);
			break;
		case ScriptKind::Neil:
			RawCall("Neil.Globals." + f, Para);
			break;
		default:
			UI::Crash("Unknown Script kind: " + std::to_string((int)LKind));
		}
	}
#pragma endregion
}