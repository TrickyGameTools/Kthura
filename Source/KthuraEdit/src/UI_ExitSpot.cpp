// Lic:
// Kthura Map Editor
// Exit Spot Placement
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
#include <string>
#include <june19.hpp>
#include "../headers/MapData.hpp"
#include "../headers/UserInterface.hpp"

// This source has no header file. Only Other.cpp required the header
// So the header is in there.

namespace KthuraEdit {
	using namespace std;
	using namespace june19;

	static int
		x{ 0 },
		y{ 0 };
	static string
		Tag{ "" },
		Wind{ "" };
	static j19gadget
		* Okay{ nullptr },
		* LErr{ nullptr };

	

	static void ExT(j19gadget* g, j19action a) { Tag = g->Text; }
	static void ExW(j19gadget* g, j19action a) { Wind = g->Text; }
	static void CorrectCancel(j19gadget* g, j19action a) { g->X(Okay->W() + 103); }
	static void AllowOkay(j19gadget* g, j19action a) { g->Enabled = Tag.size() && (!WorkMap.Layer(CurrentLayer)->HasTag(Tag)); LErr->Visible = !g->Enabled; }
	static void ActCancel(j19gadget* g, j19action a) { UI::GoToStage("Map"); }

	static void ActOkay(j19gadget* g, j19action a) {
		auto
			O{ WorkMap.Layer(CurrentLayer)->RNewObject("Exit") };
		O->X(x);
		O->Y(y);
		O->MetaData("Wind", Wind);
		O->Tag(Tag);
		O->Dominance(20);
		UI::GoToStage("Map");
	}



	void SE_Exit(int _x, int _y, std::string wh){
		static j19gadget
			* Caption{ nullptr },
			* LTag{ nullptr },
			* FTag{ nullptr },
			* LWind{ nullptr },
			* FWind{ nullptr },
			* Cancel{ nullptr },
			* MF{ nullptr };
		static unsigned int count{ 0 };
		char DefTag[20]{ "" };
		auto Lay{ WorkMap.Layer(CurrentLayer) };
		if (!Caption) {
			UI::AddStage("EXIT");
			MF = UI::GetStage("EXIT")->MainGadget;
			Caption = CreateLabel("Place exit spot",0,0,200,40,MF); Caption->FB = 0; Caption->FG = 180;
			LTag = CreateLabel("Tag", 0, 40, 200, 20, MF); LTag->FR = 180; LTag->FG = 0; LTag->FB = 255;
			FTag = CreateTextfield(200, 40, MF->W()-220, MF); FTag->FR = 0; FTag->FG = 180; FTag->BG = 18; FTag->BB = 25;
			LWind = CreateLabel("Wind", 0, 80, 200, 20, MF); LWind->FR = 180; LWind->FG = 0; LTag->FB = 255;
			FWind = CreateTextfield(200, 80, 300, MF); FWind->FR = 0; FWind->FG = 180; FWind->BG = 18; FWind->BB = 25;
			LErr = CreateLabel("That tag is already in use", 200, 120, 200, 20, MF, 2); LErr->FG = 0; LErr->FB = 0;
			FTag->CBAction = ExT;
			FWind->CBAction = ExW;

			Okay = CreateButton("Okay", 100, 300, MF);
			Okay->SetForeground(0, 255, 0, 255);
			Okay->SetBackground(0, 25, 0, 255);
			Okay->CBDraw = AllowOkay;
			Okay->CBAction = ActOkay;

			Cancel = CreateButton("Cancel", 0, 300, MF);
			Cancel->SetForeground(255, 0, 0, 255);
			Cancel->SetBackground(25, 0, 0, 255);
			Cancel->CBAction = ActCancel;
			Cancel->CBDraw = CorrectCancel;

		}
		do { sprintf_s(DefTag, "Exit_%08X", ++count); } while (Lay->HasTag(DefTag)); Tag = DefTag; FTag->Text = DefTag;
		FWind->Text = "North"; Wind = "North";
		x = _x;
		y = _y;
		UI::GoToStage("EXIT");
	}
}