// Lic:
// Kthura
// Ohter
// 
// 
// 
// (c) Jeroen P. Broks, 2022
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
// Version: 22.01.06
// EndLic
#include <iostream>
#include <string>
#include <map>
#include "../headers/Other.hpp"
#include "../headers/MapData.hpp"

namespace KthuraEdit {
	using namespace std;
	typedef struct { PO_AreaEffect f; } SAE;
	typedef struct { PO_SpotEffect f; } SPE;
	static map<string, SAE> Reg_AreaEffect{};
	static map<string, SPE> Reg_SpotEffect{};

	static void Script_AreaEffect(int x, int y, int w, int h, std::string what) {}
	static void Script_SpotEffect(int x, int y, std::string what) {}

#pragma region inbuilt routines that should replace scripts
	// Area
	void AE_Relabel(int x, int y, int w, int h, std::string wh); // Header! Makes and extra hpp file unneeded
	// Spot
	static void SE_Pivot(int x, int y, std::string wh) { auto O{ WorkMap.Layer(CurrentLayer)->RNewObject("Pivot") }; O->X(x); O->Y(y); O->Dominance(20); } // No extra code needed here!
	void SE_Exit(int x, int y, std::string wh);
#pragma endregion

	static void O_Init(bool force=false){
		static bool before = false;
		if (!(before || force)) {
			RegisterAreaEffect("Relabel", AE_Relabel);
			RegisterSpotEffect("Exit", SE_Exit);
			RegisterSpotEffect("Pivot", SE_Pivot);
		}
		before = true;
	}

	void AreaEffect(int x, int y, int w, int h, std::string what) {
		O_Init();
		if (what[0] == '$')
			Script_AreaEffect(x, y, w, h, what);
		else {
			if (Reg_AreaEffect.count(what))
				Reg_AreaEffect[what].f(x, y, w, h, what);
			else
				cout << "\x7No function attached to situation '" << what <<  "' for area effect (" << x << "," << y << ") " << w << "x" << h << endl;
		}
		
	}
	void SpotEffect(int x, int y, std::string what) {
		if (what[0] == '$')
			Script_SpotEffect(x, y, what);
		else {
			if (Reg_SpotEffect.count(what))
				Reg_SpotEffect[what].f(x, y, what);
			else
				cout << "\x7No function attached to situation '" << what << "' for area effect (" << x << "," << y << ")\n";
		}
	}

	void RegisterAreaEffect(string what, PO_AreaEffect fun) { Reg_AreaEffect[what].f = fun; }
	void RegisterSpotEffect(string what, PO_SpotEffect fun) { Reg_SpotEffect[what].f = fun; }


	void RegArea2Box(june19::j19gadget* g) { O_Init(); for (auto s : Reg_AreaEffect) g->AddItem(s.first); }
	void RegSpot2Box(june19::j19gadget* g) { O_Init(); for (auto s : Reg_SpotEffect) g->AddItem(s.first); }
}