// Lic:
// Kthura Map Editor (C++)
// Configuration (header)
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
// Version: 21.04.03
// EndLic
#pragma once

#define CONFIGDEBUG

#include <string>
#include <map>

#include <jcr6_core.hpp>
#include <GINIE.hpp>
#include <Platform.hpp>



namespace KthuraEdit {

	typedef struct Success {
		bool Succesful{ true };
		std::string error{ "" };
	} Success;

	class Config {
	private:
		static std::string _MyExe;
		static std::string _MyExeDir;
		static jcr6::JT_Dir _JCR;
		static TrickyUnits::GINIE GlobalConfig;
		static TrickyUnits::GINIE ProjectConfig;
	public:
		static std::string Project;
		static std::string MapFile;
		static std::string JCRPrefix;

		static std::map<std::string, std::string> ArgConfig;

		static std::string MyExe();
		static std::string MyExeDir();
		static std::string MyAssets();
		static std::string WorkSpace();
		static std::string ProjectFile();
		static std::string FullMapFile();
		static jcr6::JT_Dir* JCR();

		static Success GetJCR();

		static void ParseArgs(int aantal_arg, char** arg);
		static void LoadProject();
	};

}