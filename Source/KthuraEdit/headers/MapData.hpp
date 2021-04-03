#pragma once

#include <string>
#include <Kthura.hpp>

namespace KthuraEdit{

	extern NSKthura::Kthura WorkMap;
	extern int ScrollX;
	extern int ScrollY;
	extern std::string CurrentLayer;


	int GridX();
	int GridY();
}
