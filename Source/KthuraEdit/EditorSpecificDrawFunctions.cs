// Lic:
// Kthura in C#
// Functions merely for working with the Kthura Editor
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
// Version: 19.04.18
// EndLic




using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrickyUnits;
using NSKthura;

namespace KthuraEdit
{

    /// <summary>
    /// The KthuraDraw classes has some stuff it won't draw by itself, simply they should always be invisible, however in some particular cases it is desireable that they are seen, like in this editor, and hence this class to set that all in order. ;)
    /// </summary>
    class EditorSpecificDrawFunctions  {

        static void DrawZone(KthuraObject obj, int ix = 0, int iy = 0, int scrollx = 0, int scrolly = 0)  {
            if (!UI.InZoneTab) return;
            TQMG.Color((byte)obj.R, (byte)obj.G, (byte)obj.B);
            TQMG.SetAlpha(1);
            TQMG.DrawRectangle(obj.x + ix - scrollx, obj.y + iy - scrolly, obj.w, obj.h);
            TQMG.SetAlpha(255);
            TQMG.Color(0, 0, 0);
            var t = UI.font16.Text(obj.Tag);
            for (int x = -1; x <= 1; x++) for (int y = -1; y <= 1; y++) t.Draw(x + obj.x + ix - scrollx, y + obj.y + iy - scrolly);
            TQMG.Color((byte)obj.R, (byte)obj.G, (byte)obj.B);
            t.Draw(obj.x + ix - scrollx, obj.y + iy - scrolly);
        }

        static void DrawPivot(KthuraObject obj, int ix = 0, int iy = 0, int scrollx = 0, int scrolly = 0) {
            Lua_XStuff.ME = obj;
            Core.Lua("Pivot_Show(Kthura.ME)");
        }


        public static void init() {
            KthuraDraw.DrawZone = DrawZone;
            KthuraDraw.DrawPivot = DrawPivot;
        }
    }
}




