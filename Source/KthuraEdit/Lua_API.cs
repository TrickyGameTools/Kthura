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
// Version: 19.11.23
// EndLic






using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using KthuraEdit.Stages;
using NSKthura;
using TrickyUnits;

namespace KthuraEdit {

    static class Lua_XStuff {
        static public string callbackstage = "";
        static Dictionary<int, TQMGImage> Markers = new Dictionary<int, TQMGImage>();
        static public TQMGImage Marker (int i) {
            if (i < 4 || i > 500 || i % 4 != 0) return null;
            if (!Markers.ContainsKey(i)) {
                Markers[i] = TQMG.GetImage($"Markers/{i}.png");
                Markers[i].HotCenter();
                DBG.Log($"Loaded marker #{i}");                
            }
            return Markers[i];
        }

        static public List<string> Ask = new List<string>();
        static public void ResetAsk() => Ask.Clear();
        static public KthuraObject ME;
        static public int WantX = 0, WantY = 0;
    }

    // This class will not really been used by the Kthura editor itself
    // But merely serve in order to 
    class Lua_API {

        // Should Replace Lua's own print command
        public void KthuraPrint(string content) => DBG.Log(content);

        public void Ask(string question) => Lua_XStuff.Ask.Add(question);
        public void AskSort() => Lua_XStuff.Ask.Sort();
        

        public string Build => BuildDate.sBuildDate;
        public string CallBackStage => Lua_XStuff.callbackstage;

        public string GenKey(string prefix) {
            var time = "";
            var cnt = -1;
            var ret = "";
            UI.MapLayer.RemapTags(); // It appears dupe tags can be created otherwise... :-/
            do {
                time = DateTime.Now.ToString();
                cnt++;
                ret = $"{qstr.md5($"{prefix}{time}{cnt}")}";
            } while (Core.Map.Layers[UI.selectedlayer].HasTag(ret,true));
            return $"{prefix}{ret}";
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
            return $"error('Use request failed. {file} has not been found!')";
        }

        public string LayerName => UI.selectedlayer;

        public void Remap() => UI.MapLayer.TotalRemap();

        public bool Marker(int radius,int x, int y) {
            var m = Lua_XStuff.Marker(radius);
            if (m == null) return false;
            m.XDraw(UI.LayW+x - UI.ScrollX, UI.PDnH+ y - UI.ScrollY);
            return true;
        }

        public void Color(byte r, byte g, byte b) => TQMG.Color(r, g, b);
        public bool IsByte(int n) => (n >= 0 && n <= 255);

        public void Debug(string m) => System.Diagnostics.Debug.WriteLine(m);

        public bool HasTag(string tag) {
            UI.MapLayer.RemapTags();
            return UI.MapLayer.HasTag(tag);
        }

        // When creating new CSpots, the "ME" object should contain the Kthura object in question.
        public KthuraObject ME=>Lua_XStuff.ME;
    }
}







