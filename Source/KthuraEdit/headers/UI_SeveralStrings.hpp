#pragma once
#include <string>
#include <june19_core.hpp>
namespace KthuraEdit {
	void New_StringPage(std::string Tag,std::string Caption,std::initializer_list<std::string> Fields);
	void New_Labels();
	void StringPage(std::string Tag);

	// Callbacks
	void GoMeta(june19::j19gadget* g, june19::j19action a);
	
}