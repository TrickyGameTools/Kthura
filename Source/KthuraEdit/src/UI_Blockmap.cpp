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