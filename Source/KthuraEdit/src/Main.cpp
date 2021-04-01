// C++
#include <iostream>

// Kthura Editor
#include "../headers/Config.hpp"
#include "../headers/UserInterface.hpp"

// Units
#include <QuickString.hpp>

// JCR6
#include <jcr6_core.hpp>
#include <jcr6_zlib.hpp>
#include <jcr6_jxsda.hpp>



void InitJCR() {
	using namespace jcr6;
	std::cout << "Initizing JCR6.\n";
	init_JCR6();
	init_zlib();
	init_jxsda();
}

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
	cout << "    Map: " << Config::MapFile << endl<<endl;
	
	InitJCR();

	UI::Start();
	do {} while (UI::Run());
	UI::Done();
	

	return 0;
}