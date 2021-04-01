#pragma once
// June19
#include <june19.hpp>

// Kthura Editor
#include "../headers/Config.hpp"



namespace KthuraEdit {
	class UI {
	private:
		static bool _initialized;
	public:

		static void Crash(std::string m);

		// Notify
		static void Start();
		static bool Run();
		static void Done();
	};
}