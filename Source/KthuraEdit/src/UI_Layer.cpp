#include "../headers/MapData.hpp"
#include "../headers/UserInterface.hpp"
#include "../headers/UI_Layer.hpp"
#include "../headers/UI_Map.hpp"
#include <june19.hpp>

using namespace june19;
using namespace TrickyUnits;

namespace KthuraEdit {

	static j19gadget* LCaption{ nullptr };
	static j19gadget* LTextFld{ nullptr };

	static j19gadget* LOk{ nullptr };
	static j19gadget* LCancel{ nullptr };

	static std::string oldlay{ "" };

	static void ActCancel(j19gadget* g, j19action e) { UI::GoToStage("Map"); }
	static void ActOkay(j19gadget* g, j19action e) {
		LTextFld->Text = Upper(LTextFld->Text);
		LOk->Enabled = LTextFld->Text != "" && (!WorkMap.Layers.count(LTextFld->Text));
		if (!LOk->Enabled) return;
		if (g == LOk || e == j19action::Enter) {
			if (oldlay == "") {
				// New Layer
				WorkMap.NewLayer(LTextFld->Text);
				UI::GoToStage("Map");
				RenewLayers();
			} else {
				// Rename Layer

			}
		}
	}

	static void Init() {
		UI::AddStage("Layer");
		auto mn = UI::GetStage("Layer")->MainGadget;
		LCaption = CreateLabel("", 10, 100, mn->W() - 20, 20,mn);
		LCaption->SetForeground(0, 255, 255);
		LTextFld = CreateTextfield(10, 150, mn->W() - 20, mn);
		LTextFld->SetForeground(255, 180, 0);
		LTextFld->SetBackground(25, 18, 0, 255);
		LTextFld->CBAction = ActOkay;
		LOk = CreateButton("Okay", 10, 200, mn);
		LOk->SetForeground(0, 255, 0, 255);
		LOk->SetBackground(0, 25, 0, 255);
		LOk->CBAction = ActOkay;
		LCancel = CreateButton("Cancel", 100, 200, mn);
		LCancel->SetForeground(255, 0, 0, 255);
		LCancel->SetBackground(25, 0, 0, 255);
		LCancel->CBAction = ActCancel;
	}


	static void UI_MLay(std::string ol="") {
		if (!LCaption) Init();
		if (ol == "") {
			LCaption->Caption = "Please enter the name for the new layer:";
			LTextFld->Text = "";
		} else {
			LCaption->Caption = std::string("Please enter a new name for layer '" + ol + "':");
			LTextFld->Text = ol;
		}
		oldlay = ol;
		ActOkay(nullptr, j19action::Unknown);
		UI::GoToStage("Layer");
	}

	void NewLayer(j19gadget* g, j19action a) { UI_MLay(); }
	void ChangeLayer(june19::j19gadget* g, june19::j19action a) { UI_MLay(CurrentLayer); }

}