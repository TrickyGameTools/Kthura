// Lic:
// Kthura Map Editor (C++)
// Layer alteration
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
// Version: 21.04.05
// EndLic
#include "../headers/MapData.hpp"
#include "../headers/UserInterface.hpp"
#include "../headers/UI_Layer.hpp"
#include "../headers/UI_Map.hpp"
#include <june19.hpp>

using namespace june19;
using namespace TrickyUnits;

namespace KthuraEdit {

	static j19gadget* LCaption{ nullptr };
	static j19gadget* LTextFld{ nullptr };

	static j19gadget* LOk{ nullptr };
	static j19gadget* LCancel{ nullptr };

	static std::string oldlay{ "" };

	static void ActCancel(j19gadget* g, j19action e) { UI::GoToStage("Map"); }
	static void ActOkay(j19gadget* g, j19action e) {
		LTextFld->Text = Upper(LTextFld->Text);
		LOk->Enabled = LTextFld->Text != "" && (!WorkMap.Layers.count(LTextFld->Text));
		if (!LOk->Enabled) return;
		if (g == LOk || e == j19action::Enter) {
			if (oldlay == "") {
				// New Layer
				WorkMap.NewLayer(LTextFld->Text);
				UI::GoToStage("Map");
				RenewLayers();
			} else {
				// Rename Layer
				WorkMap.RenameLayer(CurrentLayer, LTextFld->Text);
				CurrentLayer = LTextFld->Text;
				UI::GoToStage("Map");
				RenewLayers();
			}
		}
	}

	static void Init() {
		UI::AddStage("Layer");
		auto mn = UI::GetStage("Layer")->MainGadget;
		LCaption = CreateLabel("", 10, 100, mn->W() - 20, 20,mn);
		LCaption->SetForeground(0, 255, 255);
		LTextFld = CreateTextfield(10, 150, mn->W() - 20, mn);
		LTextFld->SetForeground(255, 180, 0);
		LTextFld->SetBackground(25, 18, 0, 255);
		LTextFld->CBAction = ActOkay;
		LOk = CreateButton("Okay", 10, 200, mn);
		LOk->SetForeground(0, 255, 0, 255);
		LOk->SetBackground(0, 25, 0, 255);
		LOk->CBAction = ActOkay;
		LCancel = CreateButton("Cancel", 100, 200, mn);
		LCancel->SetForeground(255, 0, 0, 255);
		LCancel->SetBackground(25, 0, 0, 255);
		LCancel->CBAction = ActCancel;
	}


	static void UI_MLay(std::string ol="") {
		if (!LCaption) Init();
		if (ol == "") {
			LCaption->Caption = "Please enter the name for the new layer:";
			LTextFld->Text = "";
		} else {
			LCaption->Caption = std::string("Please enter a new name for layer '" + ol + "':");
			LTextFld->Text = ol;
		}
		oldlay = ol;
		ActOkay(nullptr, j19action::Unknown);
		UI::GoToStage("Layer");
	}

	void NewLayer(j19gadget* g, j19action a) { UI_MLay(); }
	void ChangeLayer(june19::j19gadget* g, june19::j19action a) { UI_MLay(CurrentLayer); }

}