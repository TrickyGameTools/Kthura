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
		Preview = CreatePicture(TQSG_ScreenWidth() / 2, 0, TQSG_ScreenWidth() / 2, prnt->H(), prnt);
		TexList = CreateListBox(0, 0, prnt->W()-ListMargin, prnt->H(), prnt);
		TexList->SetBackground(0, 0, 0, 0);
		TexList->SetForeground(0, 255, 255, 255);
		UsedTexList = CreateListBox(0, 0, prnt->W() - ListMargin, prnt->H(), prnt);
		UsedTexList->SetBackground(0, 0, 0, 0);
		UsedTexList->SetForeground(255, 180, 0, 255);

	}

	void GoToTex(june19::j19gadget* g, june19::j19action a) {
	}
}