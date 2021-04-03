#include <june19.hpp>
#include "../headers/UserInterface.hpp"
#include "../headers/UI_TexSelect.hpp"

using namespace june19;
using namespace TrickyUnits;

namespace KthuraEdit {
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
		CreatePicture(TQSG_ScreenWidth() - 300, 0, 300, prnt->H(), prnt);
	}

	void GoToTex(june19::j19gadget* g, june19::j19action a) {
	}
}