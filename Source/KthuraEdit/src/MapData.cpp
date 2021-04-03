
#include "../headers/MapData.hpp"
namespace KthuraEdit {


	// Yes, I know! Globals are evil! I don't care!
	NSKthura::Kthura WorkMap{};
	int ScrollX{ 0 };
	int ScrollY{ 0 };
	std::string CurrentLayer{ "" };

	int GridX() { return WorkMap.Layer(CurrentLayer)->GridX; }
	int GridY() { return WorkMap.Layer(CurrentLayer)->GridY; }

}
