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