// Lic:
// Kthura Editor
// Map User Interface (header)
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
#pragma once
#include "UserInterface.hpp"

namespace KthuraEdit {

	extern UI* UI_MapEdit;
	void AdeptStatus();
	void UI_MapStart();
	void RenewLayers();
	void SetTex(std::string Tex,std::string Tab="");
	void ToggleShowGrid(june19::j19gadget* g,june19::j19action a);
	void ToggleUseGrid(june19::j19gadget* g, june19::j19action a);
	void MenuSave(june19::j19gadget* g, june19::j19action a);

	void ScrollUp(june19::j19gadget* g, june19::j19action a);
	void ScrollDn(june19::j19gadget* g, june19::j19action a);
	void ScrollLe(june19::j19gadget* g, june19::j19action a);
	void ScrollRi(june19::j19gadget* g, june19::j19action a);

	void SetLabels(std::string l, std::string tab="");
	std::string GetTabLabels(std::string t);

}