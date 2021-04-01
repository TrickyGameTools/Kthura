// self
#include "../headers/UserInterface.hpp"

// C++
#include <iostream>

// Units
#include <TQSE.hpp>
#include <TQSG.hpp>

#define QUICK_QUIT

using namespace TrickyUnits;

namespace KthuraEdit {
	bool UI::_initialized{ false };

	void UI::Crash(std::string m) {
		std::cout << "ERROR! " << m << std::endl;
		SDL_ShowSimpleMessageBox(SDL_MESSAGEBOX_ERROR, "Kthura - Fatal Error", m.c_str(), TQSG_Window());
		Done();
		exit(1);
	}

	void UI::Start() {
		// JCR6
		auto J{ Config::GetJCR() };
		if (!J.Succesful) Crash("Failed to load " + Config::MyAssets() + "\n\n" + J.error);

		// Interface load
		_initialized = true;
		std::cout << "Staring User Interface\n";
		if (!TQSG_Init("Kthura Map Editor - " + Config::Project + " - " + Config::MapFile,0,0,true)) Crash("Init Kthura failed");
		/*
		TQSG_Cls(); TQSG_Flip();
		int
			W{ TQSG_DesktopWidth() },
			H{ TQSG_DesktopHeight() };
		std::cout << "Desktop size: " << W << "x" << H << std::endl;
		SDL_SetWindowSize(TQSG_Window(), floor(W / .99), floor(H / .75));
		//*/
		TQSE_Init();
	}
	bool UI::Run() {
		auto go_on{ true };
		TQSG_Cls();
		TQSE_Poll();
		if (TQSE_Quit()) go_on = false;
#ifdef QUICK_QUIT
		if (TQSE_KeyHit(SDLK_ESCAPE)) go_on = false;
#endif
		TQSG_Flip(20);
		return go_on; // True code comes later!
	}
	void UI::Done() {
		if (!_initialized) return;
		_initialized = false;
		std::cout << "Closing User Interface\n";
	}
}