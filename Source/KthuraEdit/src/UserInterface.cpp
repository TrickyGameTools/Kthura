// self
#include "../headers/UserInterface.hpp"

// C++
#include <iostream>

// Units
#include <TQSE.hpp>
#include <TQSG.hpp>

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
		int
			W{ TQSG_DesktopWidth() },
			H{ TQSG_DesktopHeight() };
		std::cout << "Desktop size: " << W << "x" << H << std::endl;
		TQSG_Init("Kthura Map Editor - " + Config::Project + " - " + Config::MapFile, floor(W / .99), floor(H / .80));

	}
	bool UI::Run() {
		return false; // True code comes later!
	}
	void UI::Done() {
		if (!_initialized) return;
		_initialized = false;
		std::cout << "Closing User Interface\n";
	}
}