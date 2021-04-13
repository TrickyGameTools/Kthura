// Lic:
// Kthura Map Editor
// Blockmap Viewer
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
// Version: 21.04.13
// EndLic
#include <june19.hpp>
#include <Kthura_Core.hpp>
#include <TQSG.hpp>
#include "../headers/MapData.hpp"
#include "../headers/UserInterface.hpp"

using namespace TrickyUnits;
using namespace std; // fuck the "official" rules :-P
using namespace june19;
using namespace NSKthura;

namespace KthuraEdit {

	static UI* MyStage{ nullptr };
	static j19gadget
		*BMP{ nullptr },
		*Okay{ nullptr };
	
	static shared_ptr<KthuraLayer> MyLayer{ nullptr };

	static int
		bw{ 0 },
		bh{ 0 },
		tw{ 0 },
		th{ 0 };

	static void ToonBlock(){
		for (int y = 0; y <= MyLayer->BlockMapHeight(); ++y) for (int x = 0; x <= MyLayer->BlockMapWidth(); ++x) {
			if (MyLayer->Blocked(x, y)) { TQSG_ACol(180, 255, 0, 255); } else { TQSG_ACol(18, 25, 0, 255); }
			TQSG_Rect(BMP->DrawX() + (x * bw), (y * bh) + BMP->DrawY(), tw, th);
		}
	}

	static void DoOkay(j19gadget* g, j19action a) { UI::GoToStage("Map"); }

	void ShowBlockMap() {
		if (!BMP) {
			UI::AddStage("BlockMap");
			MyStage = UI::GetStage("BlockMap");
			BMP = MyStage->MainGadget;
			MyStage->PreJune = ToonBlock;
			Okay = CreateButton("Okay", 5, BMP->H() - 40, BMP);
			Okay->SetForeground(0, 255, 0, 255);
			Okay->SetBackground(0, 25, 0, 255);
			Okay->CBAction = DoOkay;
		}
		MyLayer = WorkMap.Layers[CurrentLayer];
		MyLayer->BuildBlockMap(); // Make sure this is up-to-date!
		bw = (BMP->W()-10) / (MyLayer->BlockMapWidth()+1);
		bh = (BMP->H()-80) / (MyLayer->BlockMapHeight()+1);
		tw = max(1, bw - 1);
		th = max(1, bh - 1);
		UI::GoToStage("BlockMap");
	}

	void ShowBlockMap(june19::j19gadget* g, june19::j19action a) { ShowBlockMap(); }
}