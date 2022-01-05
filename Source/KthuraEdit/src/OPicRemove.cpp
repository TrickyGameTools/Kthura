#include "../headers/OPicRemove.hpp"
#include "../headers/MapData.hpp"
#include <TQSE.hpp>
#include <TQSG.hpp>
#include <TrickyMath.hpp>
#include <iostream>
#include <stdio.h>
#include <Kthura_Draw.hpp>


using namespace std;
using namespace TrickyUnits;
using namespace NSKthura;


namespace KthuraEdit {

	static size_t AttemptRemove() {
		vector<KthuraShObject> pics;
		auto Lay{ WorkMap.Layer(CurrentLayer) };
		unsigned int PCount{ 0 };
		cout << "Scanning for Pics\n";
		for (auto o : Lay->Objects) {
			if (o->EKind() == KthuraKind::Pic) pics.push_back(o);
		}
		if (pics.size()) {
			bool newones;
			map<int,KthuraShObject> Collected;
			Collected[pics[0]->ID()] = pics[0];
			cout << "Scanning " << pics.size() << " Pic-objects with object #" << pics[0]->ID() << " as base\n";
			do {
				newones = false;
				for (auto oc : pics) {
					for (auto oni : Collected) {
						auto on{ oni.second };
						if (on->ID() != oc->ID() && (!Collected.count(oc->ID()))) {
							//std::cout << "Check: #" << oc->ID() << ";\n\tTextureCheck: " << (on->Texture() == oc->Texture()) << ";\n\tY: " << (on->Y() == oc->Y()) << "\n\tDominance:" << (on->Dominance() == oc->Dominance()) << endl;
							if (
								on->Texture() == oc->Texture() &&
								on->Y() == oc->Y() &&
								on->Dominance() == oc->Dominance() &&
								on->Alpha255() == oc->Alpha255() &&
								on->R() == oc->R() &&
								on->G() == oc->G() &&
								on->B() == oc->B() &&
								oc->Tag()=="" &&
								(on->X()== oc->X()-KthuraDraw::DrawDriver->ObjectWidth(oc) || on->X()==oc->X()+ KthuraDraw::DrawDriver->ObjectWidth(oc))
								) {
								Collected[oc->ID()] = oc;
								//cout << "Added Pic #" << oc->ID() << "\n";
								newones = true;
								PCount++;
							}
						}
					}
				}
			} while (newones);
			int
				y{ pics[0]->Y() },
				h{ KthuraDraw::DrawDriver->ObjectHeight(pics[0]) },
				x{ pics[0]->X() },
				w{ 0 },
				ex{ pics[0]->X() };
			cout << "Calculating Tiled Area\n";
			for (auto o : Collected) {
				x = QMIN(x, o.second->X());
				ex = QMAX(x, o.second->X());
			}
			w = (ex - x) + KthuraDraw::DrawDriver->ObjectWidth(pics[0]);
			printf("Creating TiledArea(%05d,%05d)   %05dx%05d\n", x, y, w, h);
			auto nobj{ Lay->RNewObject("TiledArea") };
			nobj->X(x);
			nobj->Y(y);
			nobj->W(w);
			nobj->H(h);
			nobj->Texture(pics[0]->Texture());
			nobj->R(pics[0]->R());
			nobj->G(pics[0]->G());
			nobj->B(pics[0]->B());
			nobj->Alpha255(pics[0]->Alpha255());
			nobj->Visible(pics[0]->Visible());
			nobj->Impassible(false);
			nobj->ForcePassible(false);
			nobj->Dominance(pics[0]->Dominance());
			cout << "Removing " << PCount+1 << " no longer needed Pic-Object(s)\n";
			for (auto o : Collected) { Lay->Kill(o.first);/* cout << "Killed object #" << o.first << "\n"; */}
		}
		return pics.size();
	}

	void RemovePicObjects(june19::j19gadget* g, june19::j19action a) {
		if (!TQSE_Yes("Pic remove!\n\nThis feature will try to bundle pics into tiled objects.\nPlease note there is no guarantee this will be safe so backing up before doing this is recommended.\nPlus this may take time, and in that time it may look like the system is frozen. This is normal and no reason to be alarmed.\n\n\nDo you wat to continue?"))
			return;
		int r{ -1 };
		int timeout{ 10000 };
		int hue{ 0 };
		auto before{ WorkMap.Layer(CurrentLayer)->Objects.size() };
		do {
			timeout--;
			if (!timeout) { TQSE_Notify("Timeout!"); return; }
			r = AttemptRemove();
			TQSG_Cls();
			hue = (hue + 1) % 360;
			TQSG_ColorHSV(hue, 100, 100);
			june19::j19gadget::GetDefaultFont()->Draw(to_string(r) + " pictures",5,5);
			june19::j19gadget::GetDefaultFont()->Draw("Timeout: " + to_string(timeout), 5, 50);
			TQSG_Flip();
			TQSE_Poll();
		} while (r);
		auto after{ WorkMap.Layer(CurrentLayer)->Objects.size() };
		std::cout << "Reduced " << before << " objects to " << after << " objects, saving " << (before - after) << " objects. Ratio: " << floor(((double)after / before) * 100) << "%\n";
	}
}