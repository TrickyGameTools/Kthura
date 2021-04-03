// Lic:
// Kthura Editor
// Map User Interface
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
// Version: 21.04.02
// EndLic

#define UI_AltTab

#define DuoDataLabel(cap,t1,t2)	{\
	auto temp = DataLabel(cap, CreateGroup(0, 0, 0, 0, Tab));\
	t1 = CreateTextfield(0, 0, 80, temp);	\
	t2 = CreateTextfield(100, 0, 80, temp); \
	t1->FR = 255; t1->FG = 180; t1->FB = 0; t1->BG = 25; t1->BG = 18; t1->BB = 0; \
	t2->FR = 255; t2->FG = 180; t2->FB = 0; t2->BG = 25; t2->BG = 18; t2->BB = 0; \
}


// C/C++
#include <math.h>


// Tricky's Units
#include <QuickString.hpp>
#include <TrickySTOI.hpp>

// Editor
#include "..\headers\Config.hpp"
#include "..\headers\UserInterface.hpp"
#include "..\headers\UI_Map.hpp"

using namespace june19;
using namespace TrickyUnits;

namespace KthuraEdit {

	class TTab {
	public:
		static int y;
#ifdef UI_AltTab
		j19gadget* RadioToMe{ nullptr };
#endif
		j19gadget* Tab{ nullptr };
		j19gadget* ValKind{ nullptr };
		j19gadget* ValTex{ nullptr };
		j19gadget* ValX{ nullptr };
		j19gadget* ValY{ nullptr };
		j19gadget* InsertX{ nullptr };
		j19gadget* InsertY{ nullptr };
		j19gadget* ValW{ nullptr };
		j19gadget* ValH{ nullptr };
		j19gadget* ValDom{ nullptr };
		j19gadget* ValAlpha{ nullptr };
		j19gadget* ValRotDeg{ nullptr };
		j19gadget* ValRotRad{ nullptr };
		j19gadget* ValImpassible{ nullptr };
		j19gadget* ValForcePassible{ nullptr };
		j19gadget* ValColR{ nullptr };
		j19gadget* ValColG{ nullptr };
		j19gadget* ValColB{ nullptr };
		j19gadget* ValAnimSpeed{ nullptr };
		j19gadget* ValFrame{ nullptr };
		j19gadget* ValScaleX{ nullptr };
		j19gadget* ValScaleY{ nullptr };
		j19gadget* ValVisible{ nullptr };
		j19gadget* ValTag{ nullptr };
		j19gadget* ValLabels{ nullptr };
	};
	int TTab::y{ 0 };

	static bool GridMode{ true };
	UI* UI_MapEdit{ nullptr };
	static j19gadget* LayPanel{ nullptr };
	static j19gadget* LayList{ nullptr };
	static j19gadget* DataPanel{ nullptr };
	static j19gadget* DataTab{ nullptr };



	static std::map<std::string, TTab> TabMap;

	static int FH() { return UI_MapEdit->MainGadget->Font()->TextHeight("ABC"); }

	static void CreateOther(){}

	static j19gadget* DataLabel( std::string cap, j19gadget* val,bool reset=false) {
		static int dly = 0;	
		static int dlym = 30;
		if (reset) dly = 0;
		val->X(200);
		val->Y(dly * dlym);
		val->W(200);
		val->H(20);
		val->FR = 255; val->FG = 180; val->FB = 0; val->BG = 25; val->BG = 18; val->BB = 0;
		auto cl = june19::CreateLabel(cap, 0, dly * dlym, 200, 20, val->GetParent());
		cl->FR = 180; cl->FG = 0; cl->FB = 255; cl->BG = 18; cl->BG = 0; cl->BB = 25;
		dly++;
		return val;
	}
#ifdef UI_AltTab
	static void RadioTab(j19gadget* source, j19action action) {
		for (auto TB : TabMap) {
			TB.second.Tab->Visible = TB.first == source->Caption;
		}
	}
#endif

	static void Deg2Rad(j19gadget* source, j19action action) {
		int i = ToInt(source->Text);
		double d = i * (M_PI / 180);
		for (auto m : TabMap) {
			if (m.second.ValRotDeg == source) m.second.ValRotRad->Text = left(std::to_string(d),4);
		}
	}

	static void DataNewTab(std::string caption) {
		auto TB = &TabMap[caption];
#ifdef UI_AltTab
		auto Tab = CreatePanel(0, 0, DataTab->W(), 100, DataTab);
		TB->RadioToMe = CreateRadioButton(caption, 0, TTab::y, DataPanel->W(), 20, DataPanel);
		TB->RadioToMe->CBAction = RadioTab;
		if (TTab::y == 0) TB->RadioToMe->checked = true;
		TTab::y += 20;
		DataTab->Y(TTab::y);
		DataTab->H(DataPanel->H() - TTab::y);		
#else
		auto Tab=AddTab(DataTab, TrickyUnits::left(caption,5));
#endif
		std::cout << "Creating Data Tab: " << caption << "\n";
		TB->Tab = Tab;
		Tab->FB = 255; Tab->FG = 0; Tab->FR = 180;
		Tab->BB = 25; Tab->BG = 0; Tab->BR = 18;
		if (caption == "Other") { CreateOther(); return; }		
		auto KV{ caption }; if (caption == "Modify") KV = "";
		TB->ValKind = DataLabel("Kind", CreateLabel(KV, 0, 0, 0, 0, Tab),true);
		TB->ValTex = DataLabel("Texture", CreateButton("...", 0, 0, Tab));
		TB->ValTex->Enabled = (caption != "Rect" && caption != "Zone");
		DuoDataLabel("Coords", TB->ValX, TB->ValY);
		TB->ValX->Enabled = false; TB->ValY->Enabled = false;
		DuoDataLabel("Insert", TB->InsertX, TB->InsertY);
		TB->InsertX->Enabled = (caption == "TiledArea");
		TB->InsertY->Enabled = (caption == "TiledArea");
		DuoDataLabel("Size", TB->ValW, TB->ValH);
		TB->ValW->Enabled = false;
		TB->ValH->Enabled = false;
		TB->ValDom = DataLabel("Dominance", CreateTextfield(0, 0, 0, Tab));
		TB->ValDom->Text = "20";
		TB->ValAlpha = DataLabel("Alpha", CreateTextfield(0, 0, 0, Tab));
		TB->ValAlpha->Text = "255";
		TB->ValAlpha->Enabled = (caption != "Zone");
		DuoDataLabel("Rotate", TB->ValRotDeg, TB->ValRotRad);
		TB->ValRotDeg->Enabled = caption == "Obstacle"; TB->ValRotRad->Enabled = false;
		TB->ValRotDeg->CBAction = Deg2Rad;
		TB->ValImpassible = DataLabel("Impassible", CreateCheckBox("", 0, 0, 0, 0, Tab));
		TB->ValForcePassible = DataLabel("F Passible", CreateCheckBox("", 0, 0, 0, 0, Tab));
		TB->ValImpassible->Enabled = (caption != "Obstacle");
		TB->ValForcePassible->Enabled = (caption != "Obstacle");
		TB->ValColR = DataLabel("Color.R", CreateTextfield(0, 0, 0, Tab)); TB->ValColR->Text = "255";
		TB->ValColG = DataLabel("Color.G", CreateTextfield(0, 0, 0, Tab)); TB->ValColG->Text = "255";
		TB->ValColB = DataLabel("Color.B", CreateTextfield(0, 0, 0, Tab)); TB->ValColB->Text = "255";
		TB->ValFrame = DataLabel("AnimFrame", CreateTextfield(0, 0, 0, 0, Tab)); TB->ValFrame->Text = "0";
		TB->ValAnimSpeed = DataLabel("AnimSpeed", CreateTextfield(0, 0, 0, 0, Tab)); TB->ValAnimSpeed->Text = "-1";
		TB->ValFrame->Enabled = (caption != "Rect" && caption != "Zone");
		TB->ValAnimSpeed->Enabled = TB->ValFrame->Enabled;
		TB->ValScaleX = DataLabel("Scale.X", CreateTextfield(0, 0, 0, Tab, "1000"));
		TB->ValScaleY = DataLabel("Scale.Y", CreateTextfield(0, 0, 0, Tab, "1000"));
		TB->ValScaleX->Enabled = (caption == "Obstacle");
		TB->ValScaleY->Enabled = (caption == "Obstacle");
		TB->ValVisible = DataLabel("Visible", CreateCheckBox("", 0, 0, 0, 0, Tab)); TB->ValVisible->checked = true;
		TB->ValTag = DataLabel("Tag", CreateButton("...", 0, 0, Tab)); TB->ValTag->Enabled = false;
		TB->ValLabels = DataLabel("Labels", CreateButton("0", 0, 0, Tab));
	}

	static void DrawMap() {

	}
	

	void AdeptStatus() {
		auto st{ Config::Project };
		st += "\t\t" + Config::MapFile + "\t\t";
		if (GridMode) { st += "Grid mode"; }
		st += "\t";
		june19::j19gadget::StatusText(st);
	}
	
	void UI_MapStart() {
		// Start
		UI::AddStage("Map");
		UI::GoToStage("Map");
		UI_MapEdit = UI::GetStage("Map");
		UI_MapEdit->PreJune = DrawMap;
		auto MG = UI_MapEdit->MainGadget;
		// Layers
		LayPanel = CreatePanel(0, 0, 125, MG->H(), MG);
		LayPanel->BB = 255; LayPanel->BG = 0; LayPanel->BR = 180;
		LayList = CreateListBox(1, 0, LayPanel->W()-2, LayPanel->H() - 120,LayPanel);
		LayList->BR = 0;
		LayList->BG = 25;
		LayList->BB = 25;
		LayList->FR = 0;
		LayList->FG = 255;
		LayList->FB = 255;
		auto KthPic = CreatePicture(0, LayPanel->H() - 120, LayPanel->W(), 120, LayPanel, Pic_FullStretch);
		KthPic->Image(*Config::JCR(), "Kthura.png"); 
		// Work Panel
		DataPanel = CreatePanel(TQSG_ScreenWidth() - 400, 0, 400, MG->H(), MG);
		DataPanel->BB = 255; DataPanel->BG = 0; DataPanel->BR = 180;
#ifdef UI_AltTab
		DataTab = CreateGroup(0, 0, DataPanel->W(), DataPanel->H(), DataPanel);
#else
		DataTab = CreateTabber(0, 0, DataPanel->W(), DataPanel->H(), DataPanel);
		DataTab->FB = 255; DataTab->FG = 0; DataTab->FR = 180;
		DataTab->BB = 25; DataTab->BG = 0; DataTab->BR = 18;
#endif
		auto Tabs = { "TiledArea","Obstacle","StrecthedArea","Rect","Zone" ,"Other","Modify"};
		for (auto T : Tabs) DataNewTab(T);
#ifdef UI_AltTab
		for (auto T : TabMap) T.second.Tab->H(DataTab->H());
		RadioTab(TabMap["TiledArea"].RadioToMe, j19action::Check);
#endif
	}
}