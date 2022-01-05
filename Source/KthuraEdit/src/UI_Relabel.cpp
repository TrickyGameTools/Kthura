// Lic:
// Kthura
// Relabel Interface
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
	static void ActOkay(j19gadget* g, j19action a) { WorkMap.Layer(CurrentLayer)->Relabel(x, y, w, h, nLabel, Overwrite); UI::GoToStage("Map"); }
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
			FLabel->FG = 180;
			FLabel->BG = 18;
			FLabel->BB = 25;
			LDelOld = CreateLabel("Overwrite:", 10, 80, 200, 20, MP);
			FDelOld = CreateCheckBox("Yes", 200, 80, 40, 40, MP);
			FDelOld->FR = 0;
			FDelOld->FG = 180;
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
		FArea->Caption = SArea;

		x = _x;
		y = _y;
		w = _w;
		h = _h;
		UI::GoToStage("RELABEL");
	}
}