#pragma once

#define CONFIGDEBUG

#include <string>
#include <map>



namespace KthuraEdit {

	class Config {
	private:
		static std::string _MyExe;
		static std::string _MyExeDir;
	public:
		static std::string Project;
		static std::string MapFile;
		static std::string StartOnLayer;

		static std::map<std::string, std::string> ArgConfig;

		static std::string MyExe();
		static std::string MyExeDir();

		static void ParseArgs(int aantal_arg, char** arg);
	};

}