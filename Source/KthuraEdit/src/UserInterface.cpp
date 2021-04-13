// Lic:
// Kthura Map Editor (C++)
// User Interface
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
// Version: 21.04.13
// EndLic

// self
#include "../headers/UserInterface.hpp"
#include "../headers/UI_Map.hpp"
#include "../headers/UI_Layer.hpp"
#include "../headers/UI_SeveralStrings.hpp"
#include "../headers/UI_BlockMap.hpp"
#include "../headers/UI_TagObject.hpp"
#include "../headers/UI_TagOverview.hpp"

// C++
#include <iostream>

// Units
#include <QuickString.hpp>

// Kthura
#include <Kthura_Core.hpp>
#include <Kthura_SDL_Driver.hpp>

// TQSG
#include <TQSE.hpp>
#include <TQSG.hpp>

#undef QUICK_QUIT

using namespace TrickyUnits;
using namespace june19;

namespace KthuraEdit {

	static void Panic4Kthura(std::string s) { UI::Crash(s); }

	bool UI::_initialized{ false };
	TQSG_AutoImage UI::Mouse{ nullptr };
	UI* UI::_Current{ nullptr };
	std::map<std::string, UI> UI::Stage{};

	UI::UI(std::string name) {
		_Name = name;
	}
	UI::UI() { }

	void UI::Crash(std::string m) {
		std::cout << "ERROR! " << m << std::endl;
		SDL_ShowSimpleMessageBox(SDL_MESSAGEBOX_ERROR, "Kthura - Fatal Error", m.c_str(), TQSG_Window());
		Done();
		exit(1);
	}

	void UI::AddStage(std::string st) {
		st = Upper(st);
		if (Stage.count(st)) Crash("Dupe stage: " + st);
		Stage[st] = UI{st};
		Stage[st].MainGadget = CreateGroup(0, 0, TQSG_ScreenWidth(), TQSG_ScreenHeight() - 36,WorkScreen());
	}

	UI* UI::GetStage(std::string st) {
		st = Upper(st);
		if (!Stage.count(st)) Crash("Non-existent stage: " + st);
		return &Stage[st];
	}

	UI* UI::CurrentStage() {
		return _Current;
	}

	void UI::GoToStage(std::string st) {
		st = Upper(st);
		if (!Stage.count(st)) Crash("Non-existent stage: " + st);
		_Current = &Stage[st];
		for (auto& si : Stage) {
			si.second.MainGadget->Visible = si.first == st;
		}
	}

	static bool go_on{ true };
	static void dquit(j19gadget* g, j19action a) { go_on = false; }

	void UI::Start() {

		// Set Kthura Panic
		NSKthura::Kthura::Panic = Panic4Kthura;
		
		// JCR6
		auto J{ Config::GetJCR() };
		if (!J.Succesful) Crash("Failed to load " + Config::MyAssets() + "\n\n" + J.error);
		jcr6::JCRPanic = Panic4Kthura; // config is the same so why bother!

		// Project + Map
		Config::LoadProject();

		// Interface load
		_initialized = true;
		std::cout << "Staring User Interface\n";
		if (Config::FWindowed()) {
			if (!TQSG_Init("Kthura Map Editor - " + Config::Project + " - " + Config::MapFile, 1800, 1000)) Crash("Init Graphics failed");
		} else {
			if (!TQSG_Init("Kthura Map Editor - " + Config::Project + " - " + Config::MapFile, 0, 0, true)) Crash("Init Graphics failed");
		}
		/*
		TQSG_Cls(); TQSG_Flip();
		int
			W{ TQSG_DesktopWidth() },
			H{ TQSG_DesktopHeight() };
		std::cout << "Desktop size: " << W << "x" << H << std::endl;
		SDL_SetWindowSize(TQSG_Window(), floor(W / .99), floor(H / .75));
		//*/

		// Mouse
		TQSE_Init();
		HideMouse();
		jcr6::JT_Dir* JD = Config::JCR();		
		Mouse = TQSG_LoadAutoImage(JD, std::string("MousePointer.png"));

		// Kthura
		NSKthura::Kthura_Draw_SDL_Driver::Init();
		

		// Main screen config
		AdeptStatus();
		j19gadget::SetDefaultFont(JD, "Fonts/DOSFont.jfbf");
		auto Scr = WorkScreen();
		Scr->BB = 255;
		Scr->BG = 0;
		Scr->BR = 180;
		Scr->BA = 255;
		Scr->FR = 0;
		Scr->FG = 255;
		Scr->FB = 255;
		Scr->FA = 255;
		auto
			MenuFile = Scr->AddMenu("File"),
			GridMenu = Scr->AddMenu("Grid"),
			LayerMenu = Scr->AddMenu("Layers"),
			ScrollMenu = Scr->AddMenu("Scroll"),
			OptionsMenu = Scr->AddMenu("Options"),
			DebugMenu = Scr->AddMenu("Debug");
		MenuFile->AddItem("Save", MenuSave, SDLK_s);
		MenuFile->AddStrike();
		MenuFile->AddItem("Edit Meta Data", GoMeta, SDLK_m);
		MenuFile->AddStrike();
		MenuFile->AddItem("Quit", dquit, SDLK_q);
		//std::cout << "GridMenu=" << (unsigned long long)GridMenu << "\t"<<GridMenu->Caption<<std::endl;
		GridMenu->AddItem("Toggle Grid", ToggleUseGrid, SDLK_g);
		GridMenu->AddItem("Toggle GridShow", ToggleShowGrid, SDLK_d);
		LayerMenu->AddItem("New Layer", NewLayer, SDLK_n);
		LayerMenu->AddItem("Rename Layer", ChangeLayer, SDLK_KP_5);
		ScrollMenu->AddItem("Down", ScrollDn, SDLK_DOWN);
		ScrollMenu->AddItem("Left", ScrollLe, SDLK_LEFT);
		ScrollMenu->AddItem("Up", ScrollUp, SDLK_UP);
		ScrollMenu->AddItem("Right", ScrollRi, SDLK_RIGHT);
		DebugMenu->AddItem("View BlockMap", ShowBlockMap, SDLK_b);
		DebugMenu->AddItem("Tag Overview",ShowTags,SDLK_z);
		// Stages
		UI_MapStart(); // Must be last
	}
	bool UI::Run() {
		auto CS{ CurrentStage() };
		//auto go_on{ true };
		go_on = true;
		TQSG_Cls();
		if (CS->PreJune) CS->PreJune();
		TQSE_Poll();
		Screen()->Draw();
		if (CS->PostJune) CS->PostJune();
		if (TQSE_Quit()) go_on = false;
#ifdef QUICK_QUIT
		if (TQSE_KeyHit(SDLK_ESCAPE)) go_on = false;
#endif
		TQSG_ACol(255, 255, 255, 255);
		Mouse->Draw(TQSE_MouseX(), TQSE_MouseY());
		TQSG_Flip(20);
		return go_on; // True code comes later!
	}
	void UI::Done() {
		if (!_initialized) return;
		_initialized = false;
		std::cout << "Closing User Interface\n";
		MenuSave(nullptr, j19action::Unknown);
	}
}