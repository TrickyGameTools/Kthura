#include <june19.hpp>
#include "../headers/UserInterface.hpp"
#include "../headers/UI_TexSelect.hpp"

using namespace june19;
using namespace TrickyUnits;

namespace KthuraEdit {

	static const int ListMargin = 200;

	static j19gadget
		* Preview{ nullptr },
		* TexList{ nullptr },
		* UsedTexList{ nullptr },
		* Okay{ nullptr },
		* Cancel{ nullptr },
		* ButtonChainType{ nullptr },
		* ButtonTexData{ nullptr },
		* ButtonUsedOnly{ nullptr };


	static void CreateSelector() {
		UI::AddStage("PicTex");
		auto prnt = UI::GetStage("PicText")->MainGadget;
		auto col = prnt->W() - ListMargin;
		Preview = CreatePicture(TQSG_ScreenWidth() / 2, 0, TQSG_ScreenWidth() / 2, prnt->H(), prnt);
		TexList = CreateListBox(0, 0, prnt->W()-ListMargin, prnt->H(), prnt);
		TexList->SetBackground(0, 0, 0, 0);
		TexList->SetForeground(0, 255, 255, 255);
		UsedTexList = CreateListBox(0, 0, col, prnt->H(), prnt);
		UsedTexList->SetBackground(0, 0, 0, 0);
		UsedTexList->SetForeground(255, 180, 0, 255);
		UsedTexList->Visible = false;
		int sety{ prnt->H() -30};
		Cancel = CreateButton("Cancel", col, sety, prnt);
		Cancel->SetForeground(255, 0, 0, 255);
		Cancel->SetBackground(0, 255, 0, 255);
		sety -= 30;
		Okay = CreateButton("Okay", col, sety, prnt);
		Okay->SetForeground(0, 255, 0, 255);
		Okay->SetBackground(0, 25, 0, 255);
		sety -= 20;
		ButtonUsedOnly = CreateCheckBox("Used Only", col, sety, ListMargin, 25, prnt);
		ButtonUsedOnly->SetForeground(255, 180, 0, 255);
		ButtonTexData->checked = false;
		sety -= 20;
		ButtonTexData = CreateCheckBox("AutoTexData", col, sety, ListMargin, 25, prnt);
		ButtonTexData->SetForeground(255, 180, 0, 255);
		ButtonTexData->checked = true;
		sety -= 20;
		ButtonChainType = CreateCheckBox("AutoObjKind",col,sety,ListMargin,25,prnt);
		ButtonChainType->SetForeground(0, 180, 255, 0);
		ButtonChainType->checked = true;
	}

	void GoToTex(june19::j19gadget* g, june19::j19action a) {
	}
}