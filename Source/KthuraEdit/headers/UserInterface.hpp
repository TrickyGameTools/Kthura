// Lic:
// Kthura Map Editor (C++)
// User Interface (header)
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
// C++
#include <map>
#include <string>

// June19
#include <june19.hpp>

// TQSG
#include <TQSG.hpp>

// Kthura Editor
#include "../headers/Config.hpp"



namespace KthuraEdit {

	typedef void(*UIV)();

	class UI;

	class UI {
	private:
		static bool _initialized;
		static TrickyUnits::TQSG_AutoImage Mouse;
		static std::map<std::string, UI> Stage;
		std::string _Name{};
		UI(std::string name);
		static UI* _Current;
	public:
		UI();

		static void Crash(std::string m);

		june19::j19gadget* MainGadget{ nullptr };
		UIV PreJune{ nullptr };
		UIV PostJune{ nullptr };

		static void AddStage(std::string st);
		static UI* GetStage(std::string st);
		static UI* CurrentStage();
		static void GoToStage(std::string st);

		// Notify
		static void Start();
		static bool Run();
		static void Done();
	};
}