// Lic:
// Kthura
// Other (header)
// 
// 
// 
// (c) Jeroen P. Broks, 2022
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
// Version: 22.01.06
// EndLic
#pragma once
#include <string>
#include <june19_core.hpp>


namespace KthuraEdit {
	typedef void (*PO_AreaEffect)(int x, int y, int w, int h, std::string what);
	typedef void (*PO_SpotEffect)(int x, int y, std::string what);

	void AreaEffect(int x, int y, int w, int h, std::string what);
	void SpotEffect(int x, int y, std::string what);

	void RegisterAreaEffect(std::string what, PO_AreaEffect fun);
	void RegisterSpotEffect(std::string what, PO_SpotEffect fun);

	void RegArea2Box(june19::j19gadget* g);
	void RegSpot2Box(june19::j19gadget* g);
}