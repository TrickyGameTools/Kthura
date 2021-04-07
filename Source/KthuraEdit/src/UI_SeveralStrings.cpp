#include <june19.hpp>
#include <QuickString.hpp>
#include "../headers/UserInterface.hpp"
#include "../headers/UI_SeveralStrings.hpp"



using namespace std;
using namespace june19;
using namespace TrickyUnits;

static j19gadget* MS{ nullptr };

namespace KthuraEdit {

	class DPage {
	private:
		int _y{ 30 };
	public:
		std::string Caption{ "" };
		j19gadget* Group{ nullptr };
		j19gadget* LabelCaption{ nullptr };
		map<string, j19gadget*> TextFields;
		DPage(string CPTN) {
			Caption = CPTN;
			Group = CreateGroup(MS->DrawX(), MS->DrawY(), MS->W(), MS->Y(), MS);
			LabelCaption = CreateLabel(CPTN, 0, 0, Group->W(), Group->H(), Group, 2);
			LabelCaption->SetForeground(255, 0, 0, 255);
		}

		void Add(string Fld) {
			auto w{ floor(Group->W() / 3) };
			auto l = CreateLabel(Fld, 0, _y, w, 25, Group);
			l->SetForeground(0, 180, 255);
			TextFields[Fld] = CreateTextfield(w, _y, w * 2, Group);
			TextFields[Fld]->SetForeground(255, 180, 0);
			TextFields[Fld]->SetBackground(25, 17, 0, 255);
			_y += 25;
		}

		int y() { return _y; }
	};
	map<string, shared_ptr<DPage>> Pages;

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
}
