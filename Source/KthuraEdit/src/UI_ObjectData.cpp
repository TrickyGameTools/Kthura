// Lic:
// Kthura Map Editor
// Edit Object Data
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
// Version: 21.08.22
// EndLic

#include <string>
#include <map>
#include <TQSE.hpp>
#include "../headers/UI_ObjectData.hpp"
#include "../headers/MapData.hpp"
#include "../headers/UserInterface.hpp"

using namespace std;
using namespace june19;
using namespace TrickyUnits;

namespace KthuraEdit {

	static const int _fw = 200;
	static const int _fh = 25;

	typedef struct truecombi {
		string Tag{ "" };
		j19gadget* Label{ nullptr };
		j19gadget* Field{ nullptr };
	} truecombi;

	typedef  shared_ptr<truecombi> combi;

	static j19gadget* ODScreen{ nullptr };
	static map<j19gadget*,combi> Fld;
	static j19gadget* BackButton{ nullptr };

	static void Modify(j19gadget* G, j19action A) { ModifyObject->MetaData(Fld[G]->Tag, G->Text); }

	static void BackButtonAction(j19gadget* G, j19action A) {
		TQSE_Flush();
		UI::GoToStage("Map");
	}

	static void BackButtonDraw(j19gadget* G, j19action A) { G->Y(ODScreen->Y() - G->H()); }



	static void SetUpOD() {
		static bool HasStage{ false };
		if (!HasStage) UI::AddStage("OBJData");
		auto Screen{ UI::GetStage("OBJData") };
		ODScreen = Screen->MainGadget;
		Fld.clear();
		int y{ 0 };
		for (auto f : ModifyObject->MetaDataFields()) {
			combi Entry{ make_shared<truecombi>() };
			Entry->Tag = f;
			Entry->Label = CreateLabel(f, 2, y, _fw, _fh, Screen->MainGadget);
			Entry->Field = CreateTextfield(_fw, y, Screen->MainGadget->H(), Screen->MainGadget);
			Entry->Label->FR = 0;
			Entry->Label->FG = 180;
			Entry->Label->FB = 255;
			Entry->Field->FR = 255;
			Entry->Field->FG = 180;
			Entry->Field->FB = 0;
			Entry->Field->BR = 25;
			Entry->Field->BG = 18;
			Entry->Field->BB = 0;
			Fld[Entry->Field] = Entry;
			Entry->Field->CBAction = Modify;
			Entry->Field->Text = ModifyObject->MetaData(f);
			y += _fh;
		}
		if (!BackButton) {
			BackButton = CreateButton("Back to editor", 2, 0, ODScreen);
			BackButton->CBDraw = BackButtonDraw;
			BackButton->CBAction = BackButtonAction;
			BackButton->FR = 255;
			BackButton->BR = 25;
			BackButton->FG = 0;
			BackButton->BG = 0;
			BackButton->FB = 0;
			BackButton->BB = 0;
		}
		UI::GoToStage("OBJData");
	}




	void GoToObjectData(june19::j19gadget* g, june19::j19action a) {
		if (!ModifyObject) return;
		SetUpOD();

	}
}