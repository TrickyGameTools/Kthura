// Lic:
// Kthura Map Editor
// String Config Dialog (header)
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
// Version: 21.04.10
// EndLic

#pragma once
#include <string>
#include <june19_core.hpp>

namespace KthuraEdit {
	void New_StringPage(std::string Tag,std::string Caption,std::initializer_list<std::string> Fields);
	void New_Labels();
	void StringPage(std::string Tag);

	// Callbacks
	void GoMeta(june19::j19gadget* g, june19::j19action a);
	void GoLabel(june19::j19gadget* g, june19::j19action a);
	
}