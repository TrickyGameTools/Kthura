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
#include "..\headers\Config.hpp"
#include "..\headers\UserInterface.hpp"
#include "..\headers\UI_Map.hpp"
namespace KthuraEdit {

	static bool GridMode{ true };

	void AdeptStatus() {
		auto st{ Config::Project };
		st += "\t\t" + Config::MapFile + "\t\t";
		if (GridMode) { st += "Grid mode"; }
		st += "\t";
		june19::j19gadget::StatusText(st);
	}
}