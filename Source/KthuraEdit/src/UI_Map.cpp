// Lic:
// Kthura Editor
// Map User Interface
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
// Version: 21.04.02
// EndLic
#include "..\headers\Config.hpp"
#include "..\headers\UserInterface.hpp"
#include "..\headers\UI_Map.hpp"

using namespace june19;
using namespace TrickyUnits;

namespace KthuraEdit {

	static bool GridMode{ true };
	UI* UI_MapEdit{ nullptr };
	static j19gadget* LayPanel{ nullptr };
	static j19gadget* LayList{ nullptr };

	static int FH() { return UI_MapEdit->MainGadget->Font()->TextHeight("ABC"); }
	

	void AdeptStatus() {
		auto st{ Config::Project };
		st += "\t\t" + Config::MapFile + "\t\t";
		if (GridMode) { st += "Grid mode"; }
		st += "\t";
		june19::j19gadget::StatusText(st);
	}
	
	void UI_MapStart() {
		// Start
		UI::AddStage("Map");
		UI::GoToStage("Map");
		UI_MapEdit = UI::GetStage("Map");
		auto MG = UI_MapEdit->MainGadget;
		LayPanel = CreatePanel(0, 0, 125, MG->H(), MG);
		LayPanel->BB = 255; LayPanel->BG = 0; LayPanel->BR = 180;
		LayList = CreateListBox(1, 0, LayPanel->W()-2, LayPanel->H() - 120,LayPanel);
		LayList->BR = 0;
		LayList->BG = 25;
		LayList->BB = 25;
		LayList->FR = 0;
		LayList->FG = 255;
		LayList->FB = 255;
		auto KthPic = CreatePicture(0, LayPanel->H() - 120, LayPanel->W(), 120, LayPanel, Pic_FullStretch);
		KthPic->Image(*Config::JCR(), "Kthura.png");

	}
}