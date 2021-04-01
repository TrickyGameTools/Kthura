#pragma once

#define CONFIGDEBUG

#include <string>
#include <map>

#include <jcr6_core.hpp>



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
	public:
		static std::string Project;
		static std::string MapFile;
		static std::string StartOnLayer;

		static std::map<std::string, std::string> ArgConfig;

		static std::string MyExe();
		static std::string MyExeDir();
		static std::string MyAssets();
		static jcr6::JT_Dir* JCR();

		static Success GetJCR();

		static void ParseArgs(int aantal_arg, char** arg);
	};

}