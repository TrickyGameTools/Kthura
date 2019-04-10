// Lic:
// Kthura for C#
// Lua Script API
// 
// 
// 
// (c) Jeroen P. Broks, 
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
// Version: 19.04.10
// EndLic

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KthuraEdit.Stages;
using NSKthura;

namespace KthuraEdit
{

    // This class will not really been used by the Kthura editor itself
    // But merely serve in order to 
    class Lua_API {

        // Should Replace Lua's own print command
        public void KthuraPrint(string content) => DBG.Log(content);

        // When creating new CSpots, the "ME" object should contain the Kthura object in question.
        public KthuraObject ME;
    }
}

