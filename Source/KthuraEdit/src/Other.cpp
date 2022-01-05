#include <iostream>
#include <string>
#include <map>
#include "../headers/Other.hpp"

namespace KthuraEdit {
	using namespace std;
	typedef struct { PO_AreaEffect f; } SAE;
	typedef struct { PO_SpotEffect f; } SPE;
	static map<string, SAE> Reg_AreaEffect{};
	static map<string, SPE> Reg_SpotEffect{};

	static void Script_AreaEffect(int x, int y, int w, int h, std::string what){}
	static void Script_SpotEffect(int x,int y,std::string what){}

#pragma region inbuilt routines that should replace scripts
	void AE_Relabel(int x, int y, int w, int h, std::string wh); // Header! Makes and extra hpp file unneeded	
#pragma endregion

	static void O_Init(bool force=false){
		static bool before = false;
		if (!(before || force)) {
			RegisterAreaEffect("Relabel", AE_Relabel);
		}
		before = true;
	}

	void AreaEffect(int x, int y, int w, int h, std::string what) {
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
