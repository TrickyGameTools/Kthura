#include <june19.hpp>
#include <QuickString.hpp>
#include "../headers/UserInterface.hpp"
#include "../headers/UI_SeveralStrings.hpp"



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

	void New_Labels() {
		if (!MS) Init();
		if (!Pages.count("Labels")) { // Please note, due to normally always making everything full caps, this cannot cause any conflicts.
			Pages["Labels"] = make_shared<DPage>("Object Labels");
			for (unsigned int i = 1; Pages["Labels"]->y() < MS->W() - 40; i++) { // I know this for-expression is beyond odd, but I know what I'm doing (I hope)
				char Label[15]; //123456789012
				sprintf_s(Label, "Label %02d: ",i);
				Pages["Labels"]->Add(Label);
			}
		}
	}
	static void TrueStringPage(string Tag) {
		for (auto f : Pages) 
			f.second->Group->Visible = f.first == Tag;
		UI::GoToStage("StringDataPage");
	}

	void StringPage(std::string Tag) { TrueStringPage(Upper(Tag)); }
	
	void GoMeta(june19::j19gadget* g, june19::j19action a) {
		static bool donebefore{ false };
		New_StringPage("METADATA", "Map Meta Data", {});
		if (!donebefore) {
			std::cout << "Adding Meta data items\n";
			for (auto f : Config::PrjMapMeta()) {
				std::cout << "Meta: Added field " << f << "\n";
				Pages["METADATA"]->Add(f);
			}
			donebefore = true;
		}
		TQSE_Flush();
		StringPage("METADATA");
	}
}
