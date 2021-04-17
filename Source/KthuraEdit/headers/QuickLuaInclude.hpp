// Lic:
// Kthura Map Editor
// Just a quick file that includes all Lua headers in C++ style
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
// Version: 21.04.17
// EndLic
#pragma once

#pragma once

// This include not only has to include all Lua stuff
// It also must take care of all unwanted conflicts within C and C++

// I must owe my thanks in this question
// https://stackoverflow.com/questions/22813642/lua-compile-error-with-visual-2010-external-symbol-struct-lua-state-cdecl
// to https://stackoverflow.com/users/451007/010110110101 for explaining this.




extern "C" {
#include <lua.h>
#include <lauxlib.h>
#include <lualib.h>
}