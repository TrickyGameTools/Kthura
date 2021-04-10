// Lic:
// Kthura Map Editor
// Tag Edit
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
// Version: 21.04.10
// EndLic
#include <june19.hpp>
#include "../headers/UserInterface.hpp"
#include "../headers/MapData.hpp"
#include "../headers/UI_TagObject.hpp"

using namespace june19;
using namespace TrickyUnits;

namespace KthuraEdit {


	inline j19gadget* PG() { return UI::GetStage("TAGEDIT")->MainGadget; }

	j19gadget
		* Caption,
		* TextField,
		* Taken,
		* Okay,
		* Cancel;

	static void Cycle(j19gadget* p, j19action a) {
		static int
			c{ 0 },
			r{ 0 },
			g{ 0 },
			b{ 0 };
		c = (c + 1) % 255;
		if (c == 0) {
			r = (r + 1) % 255;
			if (r == 0){
				g = (g + 1) % 255;
				if (g == 0) b = (b + 1) % 255;
			}
		}
		p->SetBackground(r, b, b, 255);
	}

	static void CorrectCancel(j19gadget* g, j19action a) { Cancel->X( Okay->W() + 103); }
	static void AllowOkay(j19gadget* g, j19action a) {
		Okay->Enabled = ModifyObject && (TextField->Text == "" || (!WorkMap.Layer(CurrentLayer)->HasTag(TextField->Text)));
		Taken->Visible = (!Okay->Enabled) && ModifyObject->Tag() != Upper(TextField->Text);
	}
	static void ActCancel(j19gadget* g, j19action a) { UI::GoToStage("Map"); }
	static void ActOkay(j19gadget* g, j19action a) { ModifyObject->Tag(TextField->Text); WorkMap.Remap(); UI::GoToStage("Map"); }

	void New_TagObject() {
		if (Caption) return;
		UI::AddStage("TAGEDIT");
		PG()->CBDraw = Cycle;

		Caption = CreateLabel("Please enter a tag for this object:", 0, 100, PG()->W(), 30, PG(), 2);
		Caption->SetBackground(0, 0, 0, 127);
		Caption->SetForeground(0, 180, 255);

		Taken = CreateLabel("That tag is already taken", 0, 140, PG()->W(), 30, PG(), 2);
		Taken->SetBackground(0, 0, 0, 127);
		Taken->SetForeground(255, 0, 0);

		TextField = CreateTextfield(10, 200, PG()->W() - 20, PG());
		TextField->SetForeground(255, 180, 0);
		TextField->SetBackground(25, 18, 0, 255);

		Okay = CreateButton("Okay", 100, 300, PG());
		Okay->SetForeground(0, 255, 0, 255);
		Okay->SetBackground(0, 25, 0, 255);
		Okay->CBDraw = AllowOkay;
		Okay->CBAction = ActOkay;

		Cancel = CreateButton("Cancel", 0, 300, PG());
		Cancel->SetForeground(255, 0, 0, 255);
		Cancel->SetBackground(25, 0, 0, 255);
		Cancel->CBAction = ActCancel;
		Cancel->CBDraw = CorrectCancel;

	}
	

	void Go_TagObject(june19::j19gadget* g, june19::j19action a) {
		if (!ModifyObject) return;
		New_TagObject();
		TextField->Text = ModifyObject->Tag();
		UI::GoToStage("TAGEDIT");
	}
}