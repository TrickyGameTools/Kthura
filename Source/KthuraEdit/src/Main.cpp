// Lic:
// Kthura Map Editor (C++)
// Main
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
// Version: 21.04.03
// EndLic
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

// SDL2 (must even though not used here be there, due to main replacement in SDL2)
#include <SDL.h>
#include <SDL_main.h>



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