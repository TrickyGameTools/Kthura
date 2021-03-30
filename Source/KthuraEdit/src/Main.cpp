// C++
#include <iostream>

// Kthura Editor
#include "../headers/Config.hpp"

// Units
#include <QuickString.hpp>


int main(int aantal_arg, char** arg) {
	using namespace std;
	using namespace KthuraEdit;
	using namespace TrickyUnits;
	cout << "Kthura Map Editor - Build " <<  __TIMESTAMP__ << "\n";
	cout << "Coded by: Tricky\n";
	cout << "(c) 2021 Jeroen P. Broks - Released under the terms of the GPL3\n\n";
	Config::ParseArgs( aantal_arg, arg);
	if (Config::Project == "" || Config::MapFile == "") {
		cout << "Usage: " << StripAll(arg[0])<<" [switces] <Project> <Map>"<<endl;
		cout << endl << "You can also use the Kthura Map Editor with the help of the Kthura Launcher" << endl;
		return 0;
	}
	cout << "Project: " << Config::Project << endl;
	cout << "    Map: " << Config::MapFile << endl;

	return 0;
}