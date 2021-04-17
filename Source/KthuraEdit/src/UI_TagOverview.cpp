// Lic:
// Kthura Map Editor
// Tag Overview
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
// Version: 21.04.17
// EndLic
// C++
#include <map>
#include <vector>


// June 19
#include <june19.hpp>

// This app
#include "../headers/UserInterface.hpp"
#include "../headers/MapData.hpp"

using namespace std; // Yeah, I'm lazy. Don't tell me!
using namespace june19;

namespace KthuraEdit {

	static j19gadget
		* MP{ nullptr },
		* ListKind{ nullptr },
		* ListTag{ nullptr },
		* Okay{ nullptr };
	static map<string, vector<string>> TagMap;

	static void ActOkay(j19gadget* g, j19action a) {
		UI::GoToStage("Map");
	}

	static void SelectKind(j19gadget* g, j19action a) {
		if (g->ItemText()=="") {
			ListTag->ClearItems();
			return;
		}
		ListTag->ClearItems();
		for (auto T : TagMap[g->ItemText()]) ListTag->AddItem(T);
	}

	void ShowTags(j19gadget*g,j19action a) {
		if (!MP) {
			cout << "Creating UI for Tag overview!\n";
			UI::AddStage("TagOverview");
			MP = UI::GetStage("TagOverview")->MainGadget;
			ListKind = CreateListBox(0, 0, 250, MP->H() - 80, MP);
			ListKind->SetForeground(0, 180, 255, 255);
			ListKind->SetBackground(0, 10, 25, 255);
			ListTag = CreateListBox(250,0, MP->W() - 250, MP->H() - 80, MP);
			ListTag->SetForeground(255, 180, 0, 255);
			ListTag->SetBackground(25, 10, 0, 255);
			Okay = CreateButton("Go back to the editor", 0, MP->H() - 75, MP);
			Okay->SetForeground(255, 180, 0, 255);
			Okay->SetBackground(250, 10, 0, 255);
			Okay->CBAction = ActOkay;
			ListKind->CBAction = SelectKind;

		}
		ListKind->ClearItems();
		ListTag->ClearItems();
		for (auto o : WorkMap.Layer(CurrentLayer)->Objects) {
			if (o->Tag() != "")
				TagMap[o->Kind()].push_back(o->Tag());
		}
		for (auto k : TagMap) ListKind->AddItem(k.first);
		UI::GoToStage("TagOverview");
	}

}