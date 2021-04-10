// Lic:
// Kthura Map Editor
// String Config Dialog
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
// Version: 21.04.09
// EndLic

#include <june19.hpp>
#include <QuickString.hpp>
#include "../headers/MapData.hpp"
#include "../headers/UserInterface.hpp"
#include "../headers/UI_SeveralStrings.hpp"
#include "../headers/UI_Map.hpp"



using namespace std;
using namespace june19;
using namespace TrickyUnits;

static j19gadget* MS{ nullptr };

namespace KthuraEdit {

	class DPage;
	map<string, shared_ptr<DPage>> Pages;

	static void CorrectOkay(j19gadget* g, j19action a) { g->Y(g->GetParent()->H() - g->H()); g->Enabled = g->CBAction != nullptr; }
	static void CorrectCancel(j19gadget* g, j19action a);
	static void ActCancel(j19gadget* g, j19action a) { UI::GoToStage("Map"); }

	

	class DPage {
	private:
		int _y{ 30 };
	public:
		std::string Caption{ "" };
		j19gadget* Group{ nullptr };
		j19gadget* LabelCaption{ nullptr };
		j19gadget* Okay{ nullptr };
		j19gadget* Cancel{ nullptr };
		map<string, j19gadget*> TextFields;
		DPage(string CPTN) {
			Caption = CPTN;
			Group = CreateGroup(MS->DrawX(), MS->DrawY(), MS->W(), MS->Y(), MS);
			LabelCaption = CreateLabel(CPTN, 0, 0, Group->W(), Group->H(), Group, 2);
			LabelCaption->SetForeground(255, 0, 0, 255);
			Okay = CreateButton("Okay", 0, 0, MS);
			Okay->CBDraw = CorrectOkay;
			Okay->SetForeground(0, 255, 0);
			Okay->SetBackground(0, 25, 0, 255);
			Cancel = CreateButton("Cancel", 0, 0, MS);
			Cancel->CBDraw = CorrectCancel;
			Cancel->CBAction = ActCancel;
			Cancel->SetForeground(255, 0, 0);
			Cancel->SetBackground(25, 0, 0, 255);

		}

		void Add(string Fld) {
			auto w{ floor(Group->W() / 4) };
			auto l = CreateLabel(Fld, 0, _y, w, 25, Group);
			l->SetForeground(0, 180, 255);
			TextFields[Fld] = CreateTextfield(w, _y, w * 3, Group);
			TextFields[Fld]->SetForeground(255, 180, 0);
			TextFields[Fld]->SetBackground(25, 17, 0, 255);
			_y += 25;
		}

		int y() { return _y; }
	};

	void CorrectCancel(j19gadget* g, j19action a) {
		static std::map<j19gadget*, j19gadget*> gOk{};
		if (!gOk.count(g)) {
			cout << "Tying a cancel button\n";
			for (auto k : Pages) {
				if (k.second->Cancel == g) gOk[g] = k.second->Okay;
			}
		}
		if (!gOk.count(g)) return;
		g->X(gOk[g]->X() + gOk[g]->W() + 2);
		g->Y(gOk[g]->Y());
	}

	static void Init() {
		UI::AddStage("StringDataPage");
		MS = UI::GetStage("StringDataPage")->MainGadget;

	}

	void New_StringPage(string Tag,std::string Caption, std::initializer_list<std::string> Fields) {
		if (!MS) Init();
		Tag = Upper(Tag);
		if (!Pages.count(Tag)) {
			Pages[Tag] = make_shared<DPage>(Caption);
			for (auto f : Fields) Pages[Tag]->Add(f);
		}
	}

	static string LG(int i) {
		char Label[15]; //123456789012
		sprintf_s(Label, "Label %02d: ", i);
		return string(Label);
	}

	static void Act_Labels(j19gadget* g, j19action a) {
		string labelstr{ "" };
		auto PG = Pages["Labels"];
		for (int i = 1; PG->TextFields.count(LG(i)); i++) {
			auto V{ PG->TextFields[LG(i)]->Text };
			if (V != "") {
				if (labelstr != "") labelstr += ",";
				labelstr += V;
			}
		}
		if (g->HData == "Modify") {
			//UI::Crash("Modify not yet implemented");
			ModifyObject->Labels(labelstr);
		} else {
			SetLabels(labelstr,g->HData);
		}
		UI::GoToStage("Map");
	}

	void New_Labels() {
		if (!MS) Init();
		if (!Pages.count("Labels")) { // Please note, due to normally always making everything full caps, this cannot cause any conflicts.
			Pages["Labels"] = make_shared<DPage>("Object Labels");
			for (unsigned int i = 1; Pages["Labels"]->y() < MS->H() - (MS->DrawY()+80); i++) { // I know this for-expression is beyond odd, but I know what I'm doing (I hope)
				char Label[15]; //123456789012
				sprintf_s(Label, "Label %02d: ",i);
				Pages["Labels"]->Add(Label);
			}
		}
		Pages["Labels"]->Okay->CBAction = Act_Labels;
	}
	static void TrueStringPage(string Tag) {
		for (auto f : Pages) 
			f.second->Group->Visible = f.first == Tag;
		UI::GoToStage("StringDataPage");
	}

	void StringPage(std::string Tag) { TrueStringPage(Upper(Tag)); }

	static void OkMeta(june19::j19gadget* g, june19::j19action a) {
		for (auto f : Config::PrjMapMeta()) {
			WorkMap.MetaData[f] = Pages["METADATA"]->TextFields[f]->Text;
		}
		UI::GoToStage("Map");
	}
	
	void GoMeta(june19::j19gadget* g, june19::j19action a) {
		static bool donebefore{ false };
		New_StringPage("METADATA", "Map Meta Data", {});
		if (!donebefore) {
			std::cout << "Adding Meta data items\n";
			for (auto f : Config::PrjMapMeta()) {
				std::cout << "Meta: Added field " << f << "\n";
				Pages["METADATA"]->Add(f);
			}
			Pages["METADATA"]->Okay->CBAction = OkMeta;
			donebefore = true;
		}
		for (auto f : Config::PrjMapMeta())
			Pages["METADATA"]->TextFields[f]->Text = WorkMap.MetaData[f];
		TQSE_Flush();
		StringPage("METADATA");
	}

	void GoLabel(june19::j19gadget* g, june19::j19action a) {
		New_Labels();
		auto PG = Pages["Labels"];
		for (int i = 1; PG->TextFields.count(LG(i)); i++)
			PG->TextFields[LG(i)]->Text = "";
		Pages["Labels"]->Okay->HData = g->HData;
		if (g->HData == "Modify") {
			//UI::Crash("Modify not yet implemented");
			string labelstr{ "" };
			auto labelsplit = Split(ModifyObject->Labels(), ',');
			for (int i = 0; i < labelsplit.size(); i++) {
				if (PG->TextFields.count(LG(i + 1))) {
					PG->TextFields[LG(i + 1)]->Text = labelsplit[i];
				} else {
					auto V{ PG->TextFields[LG(i)]->Text };
					if (V != "") {
						if (labelstr != "") labelstr += ",";
						labelstr += V;
					}
				}
				//SetLabels(labelstr, g->HData);
			}
			TrueStringPage("Labels");
		} else {			
			string labelstr{ "" };
			auto labelsplit = Split(GetTabLabels(g->HData), ',');
			for (int i = 0; i < labelsplit.size(); i++) {
				if (PG->TextFields.count(LG(i+1))) {
					PG->TextFields[LG(i+1)]->Text = labelsplit[i];
				} else {
					auto V{ PG->TextFields[LG(i)]->Text };
					if (V != "") {
						if (labelstr != "") labelstr += ",";
						labelstr += V;
					}
				}
				SetLabels(labelstr, g->HData);
			}
			TrueStringPage("Labels");
		}
	}
}