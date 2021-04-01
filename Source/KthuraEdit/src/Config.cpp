// Myself
#include "../headers/Config.hpp"

// Tricky's units
#include <QuickString.hpp>


// C++
#include <iostream>

using namespace TrickyUnits;

#define CLIERR(err) std::cout << "\x7\x1b[31mCLI ERROR>\x1b[0m\t"<<err<<endl

#ifdef CONFIGDEBUG
#define ConfChat(abs) std::cout << "DENUG>\t" << abs<<endl
#else
#define ConfChat(abs)
#endif


namespace KthuraEdit {
	// privates
	jcr6::JT_Dir Config::_JCR;
	std::string Config::_MyExe{ "" };
	std::string Config::_MyExeDir{ "" };

	// public
	std::string Config::Project{ "" };
	std::string Config::MapFile{ "" };
	std::string Config::StartOnLayer{ "" };

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

	jcr6::JT_Dir* Config::JCR() {
		return &_JCR;
	}

	Success Config::GetJCR() {
		using namespace jcr6;
		std::cout << "Analysing: " << MyAssets() << std::endl;
		_JCR = Dir(MyAssets());
		auto err{ Get_JCR_Error_Message() };
		if (err != "")
			return Success{ false,err };
		return Success{ true,"" };
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
					StartOnLayer = arg[i];
					break;
				default:
					CLIERR("Unknown argument #" << i << " (" << pcnt << ")");
					break;
				}
				pcnt++;
			}

		}
	}
}