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
// Version: 19.04.11
// EndLic


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using KthuraEdit.Stages;
using NSKthura;
using TrickyUnits;

namespace KthuraEdit
{

    // This class will not really been used by the Kthura editor itself
    // But merely serve in order to 
    class Lua_API {

        // Should Replace Lua's own print command
        public void KthuraPrint(string content) => DBG.Log(content);

        public string Build => BuildDate.sBuildDate;

        public string GenKey(string prefix="") {
            var time = "";
            var cnt = -1;
            var ret = "";            
            do {
                time = DateTime.Now.ToString();
                cnt++;
                ret = $"{qstr.md5($"{prefix}{time}{cnt}")}";
            } while (Core.Map.Layers[UI.selectedlayer].HasTag(ret,true));
            return ret;
        }

        public string GetScriptToUse(string file) {
            var pad = new List<string>();
            pad.Add($"{Core.GlobalWorkSpace}/{Core.Project}/");
            foreach (string p in Core.ProjectConfig.List("ScriptPath")) pad.Add(Dirry.AD(p));
            foreach (string p in pad) {
                var probeer = $"{p}/{file}";
                if (File.Exists(probeer)) {
                    return QuickStream.LoadString(probeer);
                }
            }
            return $"error('Use request failed. {file} has not been found!";
        }

        // When creating new CSpots, the "ME" object should contain the Kthura object in question.
        public KthuraObject ME;
    }
}


