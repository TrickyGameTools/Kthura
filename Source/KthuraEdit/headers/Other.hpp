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