// Lic:
// Kthura Map Editor
// Convert
// 
// 
// 
// (c) Jeroen P. Broks, 2021, 2022
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
// Version: 22.05.13
// EndLic
#include <iostream>
#include <TQSE.hpp>
#include "../headers/Convert.hpp"
#include "../headers/MapData.hpp"
#include "../headers/Config.hpp"

#define TexDef WorkMap.Options.Value("JEROEN_CONVERT_BLITZ_JPBF", Tex)
#define TexDefD(D) WorkMap.Options.Value("JEROEN_CONVERT_BLITZ_JPBF", Tex,D)

namespace KthuraEdit {
	void PNG2JPBF(june19::j19gadget* g, june19::j19action a) {
		using namespace std;
		using namespace TrickyUnits;
		auto TJCR = Config::Textures();
		cout << "PNG to JPBF request!\n";
		for (auto L : WorkMap.Layers) {
			cout << "- Searching layer: " << L.first << "\n";
			for (auto O : L.second->Objects) {
				auto Tex{ O->Texture() };
				auto Png{ Upper(ExtractExt(Tex)) == "PNG" };
				auto Stp{ StripExt(Tex) };
				auto Frm{ Stp + ".frames" };
				auto Bnd{ Stp + ".jpbf" };
				//cout << "  = Tex: " << Tex << "; Frame: " << Frm << " (" << TJCR->EntryExists(Frm) << "); Bundle: " << Bnd << "(" << TJCR->DirectoryExists(Bnd) << ")" << endl;
				if (Png && TJCR->DirectoryExists(Bnd) && TJCR->EntryExists(Frm)) {
					cout << "  = PNG found '" << Tex << "'; has frames: " << Frm << "; and there's a bundle too: " << Bnd << "\n";
					if (TexDef == "") {
						if (TQSE_Yes(Tex + " is an animated tex in Blitz Standard. No longer supported and could lead to errors.\nJPBF equivalent has been found! Convert it?"))
							TexDefD("YES");
						else
							TexDefD("NO");
					}
					if (Upper(TexDef) == "YES") {
						O->Texture(Bnd);
						cout << "  = Converted to bundle: " << Bnd << endl;
					}
				}
			}

		}
	}


	static void UnOrigin() {
		std::cout<< "UnOrigin Request done!\n";
		int 
			mx = 0, 
			my = 0;
		for (auto o : WorkMap.Layer(CurrentLayer)->Objects) {
			if (mx > o->X()) mx = o->X();
			if (my > o->Y()) my = o->Y();
		}
		if (mx == 0 && my == 0) { std::cout << "Nothing underorigin, so let's get outta here!\n"; return; }
		//DBG.Log($"UnderOrigin Objects found. {Math.Abs(mx)} x-dist UnderOrigin, and {Math.Abs(my)} y=dist UnderOrigin. Let's fix that!");
		for (auto o : WorkMap.Layer(CurrentLayer)->Objects) {
			o->X(o->X() - mx);
			o->Y(o->Y() - my);
			// Please note, since mx and my always contain a negative number, you get --, which will always generate a + in mathematics.
		}
	}

	void OptimizeToOrigin(june19::j19gadget *g,june19::j19action a) {
		UnOrigin();
		std:: cout << "OptimizeOrigin Request done!\n";
		int 
			mx = -1, 
			my = -1;
		for(auto o : WorkMap.Layer(CurrentLayer)->Objects) {
			if (mx > o->X() || mx < 0) mx = o->X();
			if (my > o->Y() || my < 0) my = o->Y();
		}
		if (mx < 0) mx = 0;
		if (my < 0) my = 0;
		if (mx == 0 && my == 0) {std::cout << "Nothing wrong, so let's get outta here!\n"; return; }
		//DBG.Log($"Origin WhiteSpace found. {Math.Abs(mx)} x-dist from Origin, and {Math.Abs(my)} y=dist from Origin. Let's fix that!");
		for (auto o : WorkMap.Layer(CurrentLayer)->Objects) {
			o->X(o->X() - mx);
			o->Y(o->Y() - my);
		}
	}

	void RemoveRottenObjects(june19::j19gadget* g, june19::j19action a) {
		using namespace std;
		using namespace NSKthura;
		vector<int> Victims;
		ModifyObject = nullptr;
		for (auto o : WorkMap.Layer(CurrentLayer)->Objects) {
			switch (o->EKind()) {
			case KthuraKind::Rect:
			case KthuraKind::TiledArea:
			case KthuraKind::StretchedArea:
			case KthuraKind::Zone:
				if (o->W() <= 0 || o->H() <= 0) {
					Victims.push_back(o->ID());
					cout << "Object #" << o->ID() << " (" << o->Kind() << ") is \"Rotten\" and will be killed\n";
				}
				break;
			case KthuraKind::Exit:
				if (o->MetaData("Wind") == "") {
					if (o->MetaData("___DontAskAboutWindAgain") != "TRUE") {
						cout << "Exit #" << o->ID() << " has no wind!\n";
						if (TrickyUnits::TQSE_Yes("Exit ("+o->Tag()+") found without Wind!\nAssume North ? ")) {
							o->MetaData("Wind","North");
						} else {
							o->MetaData("___DontAskAboutWindAgain","TRUE");
						}
					}
				}
			}
		}
		// KILL!
		for (auto Victim : Victims) {
			WorkMap.Layer(CurrentLayer)->Kill(Victim);
			cout << "Killed: " << Victim << endl;
		}
	}

}