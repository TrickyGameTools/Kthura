// self
#include "../headers/UserInterface.hpp"

// C++
#include <iostream>

// Units
#include <TQSE.hpp>
#include <TQSG.hpp>


namespace KthuraEdit {
	bool UI::_initialized{ false };

	void UI::Start() {
		_initialized = true;
		std::cout << "Staring User Interface\n";
	}
	bool UI::Run() {
		return false; // True code comes later!
	}
	void UI::Done() {
		_initialized = false;
		std::cout << "Closing User Interface\n";
	}
}