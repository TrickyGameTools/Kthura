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
// Version: 21.04.06
// EndLic


#pragma region Macros
#define UI_AltTab

#define DuoDataLabel(cap,t1,t2)	{\
	auto temp = DataLabel(cap, CreateGroup(0, 0, 0, 0, Tab));\
	t1 = CreateTextfield(0, 0, 80, temp);	\
	t2 = CreateTextfield(100, 0, 80, temp); \
	t1->FR = 255; t1->FG = 180; t1->FB = 0; t1->BG = 25; t1->BG = 18; t1->BB = 0; \
	t2->FR = 255; t2->FG = 180; t2->FB = 0; t2->BG = 25; t2->BG = 18; t2->BB = 0; \
}
#pragma endregion

#pragma region includes
// C/C++
#include <math.h>


// Tricky's Units
#include <QuickString.hpp>
#include <TrickySTOI.hpp>

// Kthura
#include <Kthura_Save.hpp>

// Editor
#include "../headers/Config.hpp"
#include "../headers/UserInterface.hpp"
#include "../headers/UI_Map.hpp"
#include "../headers/UI_Layer.hpp"
#include "../headers/UI_TexSelect.hpp"
#include "../headers/MapData.hpp"
#pragma endregion

#pragma region Pure evil (using namespaces)
using namespace june19;
using namespace TrickyUnits;
using namespace NSKthura;
#pragma endregion


namespace KthuraEdit {

#pragma region Work tabs

	enum class TabNum {
		NONE,
		TiledArea,
		StrechedArea,
		Rect,
		Zone,
		Obstacles,
		Other,
		Modify
	};

	class TTab {
	public:
		static int y;
		TabNum cID; // Used for casing!
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
		std::string Tex{ "" };
		std::string Lab{ "" };
	};
	int TTab::y{ 0 };
	TabNum CurrentTabID{ TabNum::NONE };
	std::string CurrentTab{ "" };
	std::string Labels{ "" };
	
#pragma endregion

#pragma region Variables needed
	static bool GridMode{ true };
	static bool ShowGrid{ true };
	static std::string ChosenTex{ "" };
	UI* UI_MapEdit{ nullptr };
	static j19gadget* LayPanel{ nullptr };
	static j19gadget* LayList{ nullptr };
	static j19gadget* DataPanel{ nullptr };
	static j19gadget* DataTab{ nullptr };
	static j19gadget* MapGroup{ nullptr }; // Just needed as reference


	static std::map<std::string, TTab> TabMap;
#pragma endregion

#pragma region A few forwarding headers
	static void DrawMap();
#pragma endregion


#pragma region User Interface itself
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
			if (TB.first == source->Caption) {
				TB.second.Tab->Visible = true;
				CurrentTabID = TB.second.cID;
				CurrentTab = TB.first;
				ChosenTex = TB.second.Tex;
			} else { TB.second.Tab->Visible = false; }

		}
	}
#endif

	static void LayerSelected(j19gadget* source, j19action) {
		if (source->ItemText() == "") return;
		CurrentLayer = source->ItemText();
	}

	static void Deg2Rad(j19gadget* source, j19action action) {
		int i = ToInt(source->Text);
		double d = i * (M_PI / 180);
		for (auto m : TabMap) {
			if (m.second.ValRotDeg == source) m.second.ValRotRad->Text = left(std::to_string(d),4);
		}
	}

	static void ToggleImp(j19gadget* source, j19action) {
		auto frc = (&TabMap[source->GetParent()->HData])->ValForcePassible;
		frc->Enabled = !source->checked;
		// TODO: Change object state if in modify mode
	}
	static void ToggleFrc(j19gadget* source, j19action) {
		auto imp = (&TabMap[source->GetParent()->HData])->ValImpassible;
		imp->Enabled = !source->checked;
		// TODO: Change object state if in modify mode
	}


	static void DataNewTab(std::string caption,TabNum ncID) {
		auto TB = &TabMap[caption];
#ifdef UI_AltTab
		auto Tab = CreatePanel(0, 0, DataTab->W(), 100, DataTab);
		TB->cID = ncID;
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
		Tab->HData = caption;
		Tab->FB = 255; Tab->FG = 0; Tab->FR = 180;
		Tab->BB = 25; Tab->BG = 0; Tab->BR = 18;
		if (caption == "Other") { CreateOther(); return; }		
		auto KV{ caption }; if (caption == "Modify") KV = "";
		TB->ValKind = DataLabel("Kind", CreateLabel(KV, 0, 0, 0, 0, Tab),true);
		TB->ValTex = DataLabel("Texture", CreateButton("...", 0, 0, Tab));
		TB->ValTex->CBAction = GoToTex;
		TB->ValTex->Enabled = (caption != "Rect" && caption != "Zone");
		TB->ValTex->HData = caption; // The texture selector will use this to identify the tab used.
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
		TB->ValImpassible->CBAction = ToggleImp;
		TB->ValForcePassible = DataLabel("F Passible", CreateCheckBox("", 0, 0, 0, 0, Tab));		
		TB->ValForcePassible->CBAction = ToggleFrc;
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

	

	void AdeptStatus() {
		auto st{ Config::Project };
		st += "\t\t" + Config::MapFile + "\t\t";
		if (GridMode) { st += "Grid mode"; }
		st += "\t";
		june19::j19gadget::StatusText(st);
	}
	typedef struct TBI { std::string s; TabNum i; } TBI;

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
		LayList->CBAction = LayerSelected;
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
		//auto Tabs = std::map<std::string, TabNum>{ {"TiledArea",TabNum::TiledArea},{"Obstacle",TabNum::Obstacles},{"StrecthedArea",TabNum::StrechedArea},{"Rect",TabNum::Rect},{"Zone",TabNum::Zone} ,{"Other",TabNum::Other},{"Modify",TabNum::Modify} };
		auto Tabs={ TBI{"TiledArea",TabNum::TiledArea},TBI{"Obstacle",TabNum::Obstacles},TBI{"StretchedArea",TabNum::StrechedArea},TBI{"Rect",TabNum::Rect},TBI{"Zone",TabNum::Zone} ,TBI{"Other",TabNum::Other},TBI{"Modify",TabNum::Modify} };
		for (auto T : Tabs) DataNewTab(T.s,T.i);
#ifdef UI_AltTab
		for (auto T : TabMap) T.second.Tab->H(DataTab->H());
		RadioTab(TabMap["TiledArea"].RadioToMe, j19action::Check);
#endif
		MapGroup = CreateGroup(LayPanel->W(), LayPanel->DrawY(), TQSG_ScreenWidth() - (LayPanel->W() + DataPanel->W()), LayPanel->H(),MG);
		RenewLayers();
	}

	void RenewLayers() {
		using namespace std;
		LayList->ClearItems();
		for (auto lay : WorkMap.Layers) {
			LayList->AddItem(lay.first);
			//cout << "Layer: " << lay.first << endl;
		}
		for (unsigned long long i = 0; i <= LayList->NumItems(); i++) if (LayList->ItemText(i) == CurrentLayer) LayList->SelectItem(i);
	}

	void SetTex(std::string Tex,std::string tab) { 
		ChosenTex = Tex; 
		if (tab != "") {
			for (auto& tb : TabMap) tb.second.RadioToMe->checked = tab == tb.first;
			RadioTab(TabMap[tab].RadioToMe, j19action::Check);
		}
		TabMap[CurrentTab].Tex = Tex;
	}

	void ToggleShowGrid(j19gadget* g, j19action a) { ShowGrid = !ShowGrid; }
	void ToggleUseGrid(june19::j19gadget* g, june19::j19action a) { GridMode = !GridMode; AdeptStatus(); }

#pragma endregion


#pragma region Actual Editor
	SDL_Rect Placement{ -1,-1,-1,-1 };
	SDL_Rect TruePlacement{ -1,-1,-1,-1 };
	bool ml;

	static bool UpdatePlacement(){
		static bool
			oml{ false };
		int
			x{ TQSE_MouseX()-MapGroup->DrawX() },
			y{ TQSE_MouseY()-MapGroup->DrawY() };
		if (GridMode) {
			x = floor(x / WorkMap.Layers[CurrentLayer]->GridX) * WorkMap.Layers[CurrentLayer]->GridX;
			y = floor(y / WorkMap.Layers[CurrentLayer]->GridY) * WorkMap.Layers[CurrentLayer]->GridY;
		}
		if ((!ml) && oml) { oml = false; return true; }
		if (ml) {
			TruePlacement.w = x - TruePlacement.x;
			TruePlacement.h = y - TruePlacement.y;
			Placement = TruePlacement;
			if (TruePlacement.w < 0) { Placement.x = TruePlacement.x + TruePlacement.w; Placement.w = abs(TruePlacement.w); }
			if (TruePlacement.h < 0) { Placement.y = TruePlacement.y + TruePlacement.h; Placement.h = abs(TruePlacement.h); }
		} else {
			TruePlacement.x = x;
			TruePlacement.y = y;
			Placement = TruePlacement;
		}
		oml = ml;
		//TQSG_ACol(255, 180, 0, 255);
		//std::string dbgp{ "Placement (" + std::to_string(Placement.x) + "," + std::to_string(Placement.y) + ")  " + std::to_string(Placement.w) + "x" + std::to_string(Placement.h)+" ["+std::to_string((int)CurrentTabID)+"]" };
		
		//if (ml) dbgp += " D";
		//MapGroup->Font()->Draw(dbgp.c_str(), MapGroup->DrawX(), MapGroup->DrawY());
		return false;
	}

	static void WorkArea(std::string Tab, bool place) {
		if (ml) {
			TQSG_ACol(100, 100, 100, 100);
			TQSG_Rect(Placement.x + MapGroup->DrawX(), Placement.y + MapGroup->DrawY(), Placement.w, Placement.h);
		}
		if (place) {
			if (Placement.w && Placement.h) {
				if ((Tab == "StrechedArea" || Tab == "TiledArea") && ChosenTex == "") return;
				std::cout << "Creating area based object (" << Tab << ")\n";
				if (!TabMap.count(Tab)) UI::Crash("No Kthura Edit Tab called '" + Tab + "'"); 
				auto GTab{ &TabMap[Tab] };
				auto O{ WorkMap.Layer(CurrentLayer)->RNewObject(Tab) };
				O->Texture(ChosenTex);
				O->X(Placement.x);
				O->Y(Placement.y);
				O->W(Placement.w);
				O->H(Placement.h);
				O->R(ToInt(GTab->ValColR->Text));
				O->G(ToInt(GTab->ValColG->Text));
				O->B(ToInt(GTab->ValColB->Text));
				O->Alpha255(ToInt(GTab->ValAlpha->Text));
				O->Visible(GTab->ValVisible->checked);
				O->Impassible(GTab->ValImpassible->checked);
				O->ForcePassible(GTab->ValForcePassible->checked);
				//O->ScaleX(ToInt(GTab->ValScaleX->Text));
				//O->ScaleY(ToInt(GTab->ValScaleY->Text));
				O->ScaleX(1000);
				O->ScaleY(1000);
				O->Dominance(ToInt(GTab->ValDom->Text));
				O->Labels(Labels);
			}
		}
	}

	static void Obstacle() {
		int
			x = Placement.x,
			y = Placement.y;
		if (GridMode) {
			x += floor(GridX() / 2);
			y += GridY();
		}
		int
			dx = x + MapGroup->DrawX(),
			dy = y + MapGroup->DrawY();
		TQSG_ACol(127, 127, 127, 127);
		TQSG_Line(dx - 10, dy, dx + 10, dy);
		TQSG_Line(dx, dy - 10, dx, dy + 10);
		if (ml && TQSE_MouseHit(1) && ChosenTex!="") {
			auto GTab{ &TabMap["Obstacle"] };
			auto O{ WorkMap.Layer(CurrentLayer)->RNewObject("Obstacle") };
			O->Texture(ChosenTex);
			O->X(x);
			O->Y(y);
			O->W(0);
			O->H(0);
			O->R(ToInt(GTab->ValColR->Text));
			O->G(ToInt(GTab->ValColG->Text));
			O->B(ToInt(GTab->ValColB->Text));
			O->Alpha255(ToInt(GTab->ValAlpha->Text));
			O->Visible(GTab->ValVisible->checked);
			O->Impassible(GTab->ValImpassible->checked);
			O->ForcePassible(GTab->ValForcePassible->checked);
			O->ScaleX(ToInt(GTab->ValScaleX->Text));
			O->ScaleY(ToInt(GTab->ValScaleY->Text));
			//O->ScaleX(1000);
			//O->ScaleY(1000);
			O->Dominance(ToInt(GTab->ValDom->Text));
			O->Labels(Labels);
			O->RotationDegrees(ToInt(GTab->ValRotDeg->Text));
		}
	}


	void DrawMap() {
		auto place{ UpdatePlacement() };
		if (TQSE_MouseDown(1) && 
			TQSE_MouseX()>MapGroup->DrawX() && 
			TQSE_MouseX()<DataPanel->DrawX() &&			
			TQSE_MouseY() > MapGroup->DrawY() &&
			TQSE_MouseY() < MapGroup->DrawY() + MapGroup->H()) ml = true; else if (TQSE_MouseReleased(1)) ml = false;
		if (ShowGrid) {
			TQSG_Color(255, 0, 0);
			if (ScrollX < 0) TQSG_Rect(MapGroup->DrawX(), MapGroup->DrawY(), abs(ScrollX), MapGroup->H());
			if (ScrollY < 0) TQSG_Rect(MapGroup->DrawX(), MapGroup->DrawY(), MapGroup->W(), abs(ScrollY));
			TQSG_Color(80, 80, 80);
			for (int x = ScrollX % GridX(); x <= TQSG_ScreenWidth(); x += GridX()) TQSG_Line(LayPanel->W() + x, 0, LayPanel->W() + x, TQSG_ScreenHeight());
			for (int y = ScrollY % GridY(); y <= TQSG_ScreenHeight(); y += GridY())TQSG_Line(0, LayPanel->DrawY() + y, TQSG_ScreenWidth(), LayPanel->DrawY() + y);
		}
		KthuraDraw::DrawMap(WorkMap.Layers[CurrentLayer].get(), ScrollX, ScrollY, MapGroup->DrawX(), MapGroup->DrawY());

		switch (CurrentTabID) {
		case TabNum::NONE:
			CurrentTabID = TabNum::TiledArea;
		case TabNum::TiledArea:
			WorkArea("TiledArea", place);
			break;
		case TabNum::StrechedArea:
			WorkArea("StretchedArea", place);
			break;
		case TabNum::Rect:
			WorkArea("Rect", place);
			break;
		case TabNum::Zone:
			WorkArea("Zone", place);
			break;
		case TabNum::Obstacles:
			Obstacle();
			break;
		}
		TQSG_ACol(255, 180, 0, 200);
		MapGroup->Font()->Draw(ChosenTex, MapGroup->DrawX() + MapGroup->W(), MapGroup->DrawY(), 1, 0);
		//MapGroup->Font()->Draw(ChosenTex, MapGroup->DrawX() + 5, MapGroup->DrawY() + 5, 0, 0);
	}

	void ScrollUp(june19::j19gadget* g, june19::j19action a) { ScrollY -= 16; }
	void ScrollDn(june19::j19gadget* g, june19::j19action a) { ScrollY += 16; }
	void ScrollLe(june19::j19gadget* g, june19::j19action a) { ScrollX -= 16; }
	void ScrollRi(june19::j19gadget* g, june19::j19action a) { ScrollX += 16; }

	void MenuSave(june19::j19gadget* g, june19::j19action a) {
		std::cout << "Saving: " << Config::FullMapFile() << '\n';
		KthuraSave(&WorkMap, Config::FullMapFile(),"");
	}

#pragma endregion
}