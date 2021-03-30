// C++
#include <iostream>

// Kthura
#include "../headers/Config.hpp"


int main(int aantal_arg, char** arg) {
	using namespace std;
	using namespace KthuraEdit;
	cout << "Kthura Map Editor - Build " <<  __TIMESTAMP__ << "\n";
	cout << "Coded by: Tricky\n";
	cout << "(c) 2021 Jeroen P. Broks - Released under the terms of the GPL3\n\n";
	Config::ParseArgs( aantal_arg, arg);
}