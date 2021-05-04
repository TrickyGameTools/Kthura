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