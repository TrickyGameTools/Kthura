// Lic:
// Kthura in C#
// Texture Selector
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
using Microsoft.Xna.Framework.Input;
using TrickyUnits;
using UseJCR6;

namespace KthuraEdit.Stages
{
    internal struct TexItem{
        private int _y;
        public int uoy;
        public int y {
            set { _y = y; }
            get {
                if (TexSelector.UsedOnly)
                    return uoy;
                return _y;
            }
        }
        public string name;
        public TQMGText text;
        public TexItem(int ny, int nuoy, string nname) { _y = ny; uoy = nuoy; name = nname; text = UI.font20.Text(nname); }
    }

    class TexSelector:BaseStage {

        #region Static HardCore
        static TexSelector me = new TexSelector();
        static public void ComeToMe()        {
            Core.GoStage(me);
        }
        #endregion

        #region Overrides
        public override void Draw() {
            GetTextures();
            ReloadTextures();
            ShowTexture();
            ListTextures();
            AutoChecks();
        }

        public override void Update() {
            bool goDown = false;
            bool goUp = false;
            // Keyboad
            if (TQMGKey.Hit(Keys.Escape)) MainEdit.ComeToMe();
            if (TQMGKey.Held(Keys.Down)) goDown = true;
            if (TQMGKey.Held(Keys.Up)) goUp = true;
            // mouse
            if (goDown && ScrollY < MaxScroll) ScrollY += 3;
            if (goUp && ScrollY > 0) ScrollY -= 3;
        }
        #endregion

        #region stuff
        bool GoToTab = true;
        bool TabData = true;
        internal static bool UsedOnly = false;
        List<TexItem> Textures;
        List<TexItem> UsedTextures;
        int time2reload = 250;
        TQMGImage LoadedTex = UI.back;
        string chosen="";
        int ScrollY = 0;
        int MaxScroll => (Textures.Count * 25) - (UI.ScrHeight - 50);
        int W => (int)(UI.ScrWidth * .75);
        TQMGText tGoToTab = UI.font20.Text("Go to last used object kind");
        TQMGText tTabData = UI.font20.Text("Retreive last used object data");
        TQMGText tUsedOnly = UI.font20.Text("Used Textures Only");

        void GetTextures() {
            if (Textures == null) {
                Textures = new List<TexItem>();
                var y = 0;
                var uoy = 0;
                var lastbundle = "";
                foreach (TJCREntry ent in Core.Map.TextureJCR.Entries.Values) {
                    var e = qstr.ExtractExt(ent.Entry);
                    if (e.ToUpper() == "PNG") {
                        var dirsplit = ent.Entry.Split('/');
                        var bundle = -1;
                        var font = false;
                        for(int i=0; i<dirsplit.Length;i++) {
                            var d = dirsplit[i];
                            if (bundle < 0 && qstr.Suffixed(d.ToUpper(), ".JPBF"))
                                bundle = i;
                            else if (qstr.Suffixed(d.ToUpper(), ".JFBF"))
                                font = true;
                        }
                        if (!font) {
                            if (bundle < 0) {
                                Textures.Add(new TexItem(y, uoy, ent.Entry)); y += 25;
                                if (Core.Used[ent.Entry]) uoy += 25;
                            } else if (lastbundle == "" || !qstr.Prefixed(ent.Entry, lastbundle)) {
                                lastbundle = "";
                                for (int i = 0; i <= bundle; i++) {
                                    if (lastbundle != "") lastbundle += "/";
                                    lastbundle += dirsplit[i];

                                }
                                Textures.Add(new TexItem(y, uoy, lastbundle)); y += 25;
                                if (Core.Used[lastbundle]) uoy += 25;
                            }
                        }
                    }
                }
            }
            if (UsedTextures == null) UsedTextures = new List<TexItem>();
            UsedTextures.Clear();
            foreach(TexItem Tex in Textures) {
                if (Core.Used[Tex.name]) UsedTextures.Add(Tex);
            }
        }

        void ReloadTextures() {
            if (time2reload > 0) {
                time2reload--;
                if (time2reload<=0) {
                    if (chosen == "")
                        LoadedTex = UI.back;
                    else {
                        var v = Core.Map.TextureJCR.AsMemoryStream(chosen);
                        if (v == null) { DBG.Log($"Failure on loading texture: '{chosen}': {JCR6.JERROR}"); LoadedTex = UI.back; return; }
                        var qs = new QuickStream(v);
                        LoadedTex = TQMG.GetImage(qs);
                        if (LoadedTex == null) LoadedTex = UI.back;
                    }
                }
            }
        }

        void ShowTexture() {
            // This color change was done to make sure the screen text would remain readable!
            // It's only a quick preview after all
            TQMG.Color(180, 0, 255);
            if (LoadedTex != null) TQMG.SimpleTile(LoadedTex, 0, 0, UI.ScrWidth, UI.ScrHeight);
        }

        List<TexItem> Tex2List {
            get {
                if (UsedOnly) return UsedTextures;
                return Textures;
            }
        }

        void ListTextures() {
            foreach(TexItem TI in Tex2List) {                
                if (Core.ms.X < W && Core.ms.Y+ScrollY > TI.y && Core.ms.Y+ScrollY < TI.y + 22) {
                    if (chosen != TI.name) time2reload = 250;
                    chosen = TI.name;
                    if (Core.MsHit(1)) {
                        UI.SetTexture(TI.name, GoToTab, TabData);
                        MainEdit.ComeToMe();
                    }
                }
                TQMG.Color(0, 255, 255);
                if (Core.Used[TI.name]) TQMG.Color(255, 180, 0);
                if (TI.name == chosen) {
                    TQMG.DrawRectangle(0, TI.y-ScrollY, W, 21);
                    TQMG.Color(0, 0, 0);
                }
                TI.text.Draw(5, TI.y - ScrollY);
            }
        }

        void AutoChecks() {
            TQMG.Color(0, 255, 255);
            UI.CheckboxImage[GoToTab].Draw(W, 20);
            UI.CheckboxImage[TabData].Draw(W, 40);
            UI.CheckboxImage[UsedOnly].Draw(W, 60);
            tGoToTab.Draw(W + 22, 20);
            tTabData.Draw(W + 22, 40);
            tUsedOnly.Draw(W + 22, 60);
            if (Core.MsHit(1) && Core.ms.X>W) {
                if (Core.ms.Y > 20 && Core.ms.Y < 40) GoToTab = !GoToTab;
                if (Core.ms.Y > 40 && Core.ms.Y < 60) TabData = !TabData;
                if (Core.ms.Y > 60 && Core.ms.Y < 80) UsedOnly = !UsedOnly;
            }
        }
        
        #endregion
    }
}





