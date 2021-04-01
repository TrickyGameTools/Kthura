// self
#include "../headers/UserInterface.hpp"

// C++
#include <iostream>


namespace KthuraEdit {
	void UI::Start() {
		std::cout << "Staring User Interface\n";
	}
	bool UI::Run() {
		return false; // True code comes later!
	}
	void UI::Done() {
		std::cout << "Closing User Interface\n";
	}
}