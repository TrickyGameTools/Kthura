// Lic:
// Kthura Map Editor (C++)
// Configuration
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
// Version: 21.04.07
// EndLic

// Myself
#include "../headers/Config.hpp"
#include "../headers/UserInterface.hpp"
#include "../headers/MapData.hpp"

// Tricky's units
#include <Dirry.hpp>
#include <QuickStream.hpp>
#include <QuickString.hpp>


// C++
#include <iostream>

using namespace TrickyUnits;

#define CLIERR(err) std::cout << "\x7\x1b[31mCLI ERROR>\x1b[0m\t"<<err<<endl

#ifdef CONFIGDEBUG
#define ConfChat(abs) std::cout << "DEBUG>\t" << abs<<endl
#else
#define ConfChat(abs)
#endif


namespace KthuraEdit {
	// privates
	GINIE Config::MapConfig;
	GINIE Config::ProjectConfig;
	GINIE Config::GlobalConfig;
	jcr6::JT_Dir Config::_JCR;
	jcr6::JT_Dir Config::_TexJCR;
	std::string Config::_MyExe{ "" };
	std::string Config::_MyExeDir{ "" };

	// public
	std::string Config::Project{ "" };
	std::string Config::MapFile{ "" };
	std::string Config::JCRPrefix{ "" };

	std::map<std::string, std::string> Config::ArgConfig{};
	
	std::string Config::MyExe() {
		return _MyExe;
	}

	std::string Config::MyExeDir() {
		return _MyExeDir;
	}

	std::string Config::MyAssets() {
		return StripExt(_MyExe) + ".jcr";
	}

	std::string Config::WorkSpace() {
		return GlobalConfig.Value(Platform(), "Workspace");		
	}

	std::string Config::ProjectFile() {
		string ret{ WorkSpace() };
		ret = TReplace(ret, '\\', '/');
		if (right(ret, 1) != "/") ret += "/";
		ret += Project+"/"+Project + ".Project.ini";
		return ret;
	}

	std::string Config::ProjectDir() {
		return ExtractDir(ProjectFile());
	}

	std::string Config::FullMapFile() {
		return ProjectConfig.Value("Paths." + Platform(), "Maps") + "/" + MapFile;
	}

	jcr6::JT_Dir* Config::Textures() {
		bool Merge{ Upper(ProjectConfig.Value("Paths." + Platform(),"TexMerge")) == "YES" };
		if (Merge) {
			UI::Crash("Merge mode not yet available"); // This will be there! I am too dependent on this one myself.
		} else {
			_TexJCR = jcr6::Dir(ProjectConfig.Value("Paths." + Platform(), "TexDir"));			
		}
		return &_TexJCR;
	}

	jcr6::JT_Dir* Config::JCR() {
		return &_JCR;
	}

	std::vector<string> Config::PrjMapMeta() {		
		return ProjectConfig.List("Map", "GeneralData");
	}

	Success Config::GetJCR() {
		using namespace jcr6;
		std::cout << "Analysing: " << MyAssets() << std::endl;
		_JCR = Dir(MyAssets());
		auto err{ Get_JCR_Error_Message() };
		if (err != "" && err!="Ok")
			return Success{ false,err };
		return Success{ true,"" };
	}

	bool Config::FWindowed() {
		return Upper(ArgConfig["FORCEWINDOWED"])=="YES";
	}

	void Config::ParseArgs(int aantal_arg, char** arg) {
		using namespace std;
		_MyExe = arg[0]; 
		_MyExeDir = ExtractDir(_MyExe);
		int pcnt{ 0 };
		bool sw{ false };
		string lstsw{ "" };
		for (int i = 1; i < aantal_arg; ++i) {
			if (sw) {
				if (ArgConfig.count(lstsw)) {
					CLIERR("Duplicate switch (" << lstsw << ")");
				} else {
					ArgConfig[lstsw] = arg[i];
					sw = false;
					ConfChat("= Value: " << arg[i]);
				}
			} else if (prefixed(arg[i], "-")) {
				lstsw = Upper(right(arg[i], strlen(arg[i]) - 1));
				ConfChat("= Found switch: " << lstsw);
				sw = true;
			} else {
				switch(pcnt) {
				case 0:
					Project = arg[i];
					break;
				case 1:
					MapFile = arg[i];
					break;
				case 2:
					JCRPrefix = arg[i];
					break;
				default:
					CLIERR("Unknown argument #" << i << " (" << pcnt << ")");
					break;
				}
				pcnt++;
			}

		}
	}

	void Config::LoadProject() {
		auto gcfgfile = Dirry("$AppSupport$/KthuraMapEditor.Config.Ini");
		cout << "Loading global config: " << gcfgfile << endl;
		if (!FileExists(gcfgfile)) UI::Crash("Global Config not found!");
		GlobalConfig.FromFile(gcfgfile);
		cout << "Loading Project: " << ProjectFile()<<endl;
		if (!FileExists(ProjectFile())) UI::Crash("Project file not found!");
		ProjectConfig.FromFile(ProjectFile());
		cout << "Searching for map: " << Config::FullMapFile() << endl;
		if (!FileExists(Config::FullMapFile())) {
			cout << "= Creating new map\n";
			WorkMap.NewLayer("__BASE");
		} else {
			cout << "= Loading map\n";
			WorkMap.Load(Config::FullMapFile(), Config::JCRPrefix);
		}
		cout << "Texture configure\n";
		WorkMap.TexDir = Textures();
		auto TSD{ ProjectDir() + "/Tex Settings" };
		auto TSF{ TSD + "/" + MapFile + ".ini" };
		if (!DirectoryExists(TSD)) { MakeDir(TSD); cout << "= Creating " << TSD << endl; }
		cout << "= Loading config:" << TSF<<" (creating new if non-existent)\n";
		MapConfig.FromFile(TSF, true);
		MapConfig.AutoSave = TSF;
		CurrentLayer = MapConfig.Value("Layer", "Current");
		for (auto lscan : WorkMap.Layers) {
			if (CurrentLayer == "") { CurrentLayer = lscan.first; MapConfig.Value("Layer", "Current", CurrentLayer); }
			cout << "= Layer \"" << lscan.first << "\"; Objects: " << lscan.second->Objects.size();
			if (CurrentLayer == lscan.first) cout << "; Current Layer";
			cout << endl;
		}
	}
}