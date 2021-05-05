// Lic:
// Kthura Map Editor
// Convert
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
// Version: 21.05.05
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
}