#pragma once
#include "../headers/Config.hpp"


namespace KthuraEdit {
	class UI {
	private:
		static bool _initialized;
	public:

		// Notify
		static void Start();
		static bool Run();
		static void Done();
	};
}