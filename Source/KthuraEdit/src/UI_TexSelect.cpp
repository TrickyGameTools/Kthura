#include <june19.hpp>
#include "../headers/UserInterface.hpp"
#include "../headers/UI_TexSelect.hpp"
#include "../headers/UI_Map.hpp"
#include "../headers/MapData.hpp"

using namespace june19;
using namespace TrickyUnits;

namespace KthuraEdit {

	static const int ListMargin = 200;
	std::string WorkTabName;

	static j19gadget
		* Preview{ nullptr },
		* TexList{ nullptr },
		* UsedTexList{ nullptr },
		* Okay{ nullptr },
		* Cancel{ nullptr },
		* ButtonChainType{ nullptr },
		* ButtonTexData{ nullptr },
		* ButtonUsedOnly{ nullptr };

	static void ActCancel(j19gadget* e, j19action a) { UI::GoToStage("Map"); }
	static void ActButUsedOnly(j19gadget* e, j19action a) { UsedTexList->Visible = e->checked; TexList->Visible = !e->checked; }
	static void EnaOkay(j19gadget* e, j19action a) { Okay->Enabled = (TexList->Visible && TexList->SelectedItem() >= 0) || (UsedTexList->Visible && UsedTexList->SelectedItem() >= 0); }
	static void DClTexList(j19gadget* e, j19action a) {
		std::cout << "Texture selection" << e->ItemText()<<"; Action: "<<(int)a<<"\n";
		if (a == j19action::DoubleClick) {
			std::string tab{ "" };
			// TODO: Go to correct tab *if* applicable
			SetTex(e->ItemText(),tab);
			UI::GoToStage("Map");
		}
	}
	static void ActOkay(j19gadget* g, j19action a) {
		if (UsedTexList)
			DClTexList(UsedTexList, j19action::DoubleClick);
		else
			DClTexList(TexList, j19action::DoubleClick);
	}


	static void CreateSelector() {
		std::cout << "Creating Selector Screen\n";
		UI::AddStage("PickTex");
		auto prnt = UI::GetStage("PickTex")->MainGadget;
		auto col = prnt->W() - ListMargin;
		Preview = CreatePicture(TQSG_ScreenWidth() / 2, 0, TQSG_ScreenWidth() / 2, prnt->H(), prnt);
		TexList = CreateListBox(0, 0, prnt->W()-ListMargin, prnt->H(), prnt);
		TexList->SetBackground(0, 10, 10, 50);
		TexList->SetForeground(0, 255, 255, 255);
		TexList->CBAction = DClTexList;
		UsedTexList = CreateListBox(0, 0, col, prnt->H(), prnt);
		UsedTexList->SetBackground(10, 5, 0, 50);
		UsedTexList->SetForeground(255, 180, 0, 255);
		UsedTexList->Visible = false;
		UsedTexList->CBAction = DClTexList;
		int sety{ prnt->H() -30};
		Cancel = CreateButton("Cancel", col, sety, prnt);
		Cancel->SetForeground(255, 0, 0, 255);
		Cancel->SetBackground(25, 0, 0, 255);
		Cancel->CBAction = ActCancel;
		sety -= 30;
		Okay = CreateButton("Okay", col, sety, prnt);
		Okay->SetForeground(0, 255, 0, 255);
		Okay->SetBackground(0, 25, 0, 255);
		Okay->CBDraw = EnaOkay;
		Okay->CBAction = ActOkay;
		sety -= 20;
		ButtonUsedOnly = CreateCheckBox("Used Only", col, sety, ListMargin, 25, prnt);
		ButtonUsedOnly->SetForeground(255, 180, 0, 255);
		ButtonUsedOnly->checked = false;
		ButtonUsedOnly->CBDraw = ActButUsedOnly;
		sety -= 20;
		ButtonTexData = CreateCheckBox("AutoTxDta", col, sety, ListMargin, 25, prnt);
		ButtonTexData->SetForeground(0, 180, 255, 255);
		ButtonTexData->checked = true;
		sety -= 20;
		ButtonChainType = CreateCheckBox("AutoObKnd",col,sety,ListMargin,25,prnt);
		ButtonChainType->SetForeground(0, 180, 255, 0);
		ButtonChainType->checked = true;
	}

	void ScanTex() {
		auto tdir{ Config::Textures() };
		TexList->ClearItems();
		UsedTexList->ClearItems();
		WorkMap.TexDir = tdir;
		for (auto& e : tdir->Entries()) {
			TexList->AddItem(e.second.Entry());
			// TODO: Scan for used (since the map editor itself is not yet operative this cannot easily be debugged)
		}
	}

	void GoToTex(june19::j19gadget* g, june19::j19action a) {
		if (!Preview) CreateSelector();
		WorkTabName = g->HData;
		std::cout << "Go to Texture Selector (" << WorkTabName << ")\n";
		ScanTex();
		UI::GoToStage("PickTex");
	}
}