
// NOTE!
// As this source file only has 1 function used in another source, 
// and that other source is also the only source needing it, 
// a .hpp file was unneeded as that one file already has the header by itself.
// That's why UI_Relabel.hpp is non-existent

#include <string>
#include <june19.hpp>

#include "../headers/UserInterface.hpp"
#include "../headers/MapData.hpp"

using namespace june19;

namespace KthuraEdit {
	static int
		x{ 0 }, y{ 0 }, w{ 0 }, h{ 0 };
	std::string
		nLabel{ "" };
	bool
		Overwrite{ false };
	



	inline j19gadget* PG() { return UI::GetStage("RELABEL")->MainGadget; }

	static j19gadget
		* Okay{ nullptr },
		* Cancel{ nullptr };

#pragma region Callbacks
	static void CorrectCancel(j19gadget* g, j19action a) { Cancel->X(Okay->W() + 103); }
	static void EditLabel(j19gadget* g,j19action a) { nLabel = g->Text; }
	static void EditOver(j19gadget* g, j19action a) { Overwrite = g->checked; }
	static void ActCancel(j19gadget* g, j19action a) { UI::GoToStage("Map"); }
	static void ActOkay(j19gadget* g, j19action a) {}
#pragma endregion

	void AE_Relabel(int _x, int _y, int _w, int _h, std::string wh) {
		static j19gadget
			* MP{ nullptr },
			* Caption{ nullptr },
			//* FX{ nullptr },
			//* FY{ nullptr },
			//* FW{ nullptr },
			//* FH{ nullptr },
			* FArea{ nullptr },
			* FLabel{ nullptr },
			* FDelOld{ nullptr },
			* LLabel{ nullptr },
			* LArea{ nullptr },
			* LDelOld{ nullptr };
		if (!Caption) {
			UI::AddStage("RELABEL");
			MP = PG();
			Caption = CreateLabel("Relabel area",10,10,MP->W(),20,MP);
			Caption->FG = 180;
			Caption->FB = 0;
			LArea = CreateLabel("Area:",10,30,200,20,MP);
			FArea = CreateLabel("?", 200, 30, MP->W() - 210, 20, MP);
			FArea->FR = 0;
			FArea->FB = 180;
			LLabel = CreateLabel("Label:", 10, 50, 200, 20, MP);
			FLabel = CreateTextfield(200, 50, MP->W() - 210, MP);
			FLabel->FR = 0;
			FLabel->FB = 180;
			LDelOld = CreateLabel("Overwrite:", 10, 80, 200, 20, MP);
			FDelOld = CreateCheckBox("", MP->W() - 210, 80, 40, 40, MP);
			FLabel->CBAction = EditLabel;
			FDelOld->CBAction = EditOver;
			Okay = CreateButton("Okay", 100, 300, PG());
			Okay->SetForeground(0, 255, 0, 255);
			Okay->SetBackground(0, 25, 0, 255);
			//Okay->CBDraw = AllowOkay;
			Okay->CBAction = ActOkay;

			Cancel = CreateButton("Cancel", 0, 300, PG());
			Cancel->SetForeground(255, 0, 0, 255);
			Cancel->SetBackground(25, 0, 0, 255);
			Cancel->CBAction = ActCancel;
			Cancel->CBDraw = CorrectCancel;

		}
		char SArea[300]{ "" }; sprintf_s(SArea, "(%04d,%04d); %04dx%04d", _x, _y, _w, _h);
		Caption->Caption = SArea;

		x = _x;
		y = _y;
		w = _w;
		h = _h;
	}
}