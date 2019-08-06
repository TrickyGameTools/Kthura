// Lic:
// Class/KthuraDrawMonoGame.cs
// MonoGame Driver for Kthura for C#
// version: 19.08.06
// Copyright (C)  Jeroen P. Broks
// This software is provided 'as-is', without any express or implied
// warranty.  In no event will the authors be held liable for any damages
// arising from the use of this software.
// Permission is granted to anyone to use this software for any purpose,
// including commercial applications, and to alter it and redistribute it
// freely, subject to the following restrictions:
// 1. The origin of this software must not be misrepresented; you must not
// claim that you wrote the original software. If you use this software
// in a product, an acknowledgment in the product documentation would be
// appreciated but is not required.
// 2. Altered source versions must be plainly marked as such, and must not be
// misrepresented as being the original software.
// 3. This notice may not be removed or altered from any source distribution.
// EndLic



using System.Collections.Generic;
using System.Diagnostics;
using TrickyUnits;


namespace NSKthura {

    delegate void dCrashOnNoTex(string tex);

	class KthuraDrawMonoGame:KthuraDraw{

        public static dCrashOnNoTex CrashOnNoTex = null;

        #region Chain me into Kthura
        static KthuraDrawMonoGame me = new KthuraDrawMonoGame();
        static public void UseMe() => DrawDriver = me;
        #endregion

        #region Textures
        Dictionary<string, TQMGImage> Textures = new Dictionary<string, TQMGImage>();
        Kthura LastUsedMap;

        TQMGImage GetTex(KthuraObject obj) {
            var file = obj.Texture;
            var kind = obj.kind;
            var lay = obj.Parent;
            var map = lay.Parent;
            if (map != LastUsedMap) Textures.Clear(); // Only store texture per map. Will take too much RAM otherwise!
            LastUsedMap = map;
            var tag = $"{kind}::{file}";
            if (!Textures.ContainsKey(tag)) {
                if (qstr.ExtractExt(file.ToUpper()) == "JPBF") {
                    Textures[tag] = TQMG.GetBundle(map.TextureJCR, $"{file}/");
                    //Bubble.BubConsole.WriteLine($"KTHURA DRAW DEBUG: Loading Bundle {file}", 255, 255, 0); // debug! (must be on comment when not in use)
                } else {
                    if (map.TextureJCR == null) Debug.WriteLine("TextureJCR is null???");
                    var bt = map.TextureJCR.ReadFile(file);
                    Textures[tag] = TQMG.GetImage(bt);
                }
                Textures[tag].HotBottomCenter();
            }
            return Textures[tag];
        }

        public static void TexSizes(KthuraObject obj, ref int w, ref int h) {
            var tex = me.GetTex(obj);
            w = tex.Width;
            h = tex.Height;
        }

        public static int TexWidth(KthuraObject obj) { int w = 0, h = 0; TexSizes(obj, ref w, ref h); return w; }
        public static int TexHeight(KthuraObject obj) { int w = 0, h = 0; TexSizes(obj, ref w, ref h); return h; }
        #endregion

        void Ambtenaar() { }

        #region override
        public override void DrawTiledArea(KthuraObject obj, int ix = 0, int iy = 0, int scrollx = 0, int scrolly = 0) {
            var tx = GetTex(obj);
            TQMG.Color((byte)obj.R, (byte)obj.G, (byte)obj.B);
            //TQMG.SetAlphaFloat((float)obj.Alpha1000 / 1000);
            TQMG.SetAlpha((byte)obj.Alpha255);
            if (tx != null) TQMG.Tile(tx, obj.insertx, obj.inserty, obj.x + ix - scrollx, obj.y + iy - scrolly, obj.w, obj.h);
            TQMG.SetAlpha(255);
        }

        public override void DrawObstacle(KthuraObject obj, int ix = 0, int iy = 0, int scrollx = 0, int scrolly = 0) {
            var tx = GetTex(obj);
            if (tx != null) {
                TQMG.Color((byte)obj.R, (byte)obj.G, (byte)obj.B);
                //TQMG.SetAlphaFloat((float)obj.Alpha1000 / 1000);
                TQMG.SetAlpha((byte)obj.Alpha255);
                //TQMG.RotateRAD((float)obj.RotationRadians);
                TQMG.RotateDEG(obj.RotationDegrees);
                TQMG.Scale(obj.ScaleX, obj.ScaleY);
                tx.XDraw(obj.x + ix - scrollx, obj.y + iy - scrolly);
                TQMG.Scale(1000, 1000);
                TQMG.RotateRAD(0);
                TQMG.SetAlpha(255);
            } else CrashOnNoTex?.Invoke($"Obstacle-texture '{obj.Texture}' did somehow not load!");
        }

        public override int ObjectHeight(KthuraObject obj) {
            TQMGImage tex;
            switch (obj.kind) {
                case "Zone":
                case "TiledArea":
                    return obj.h;
                case "Obstacle":
                case "Pic":
                    tex = GetTex(obj);
                    return tex.Height;
                default:
                    return 0;
            }
        }

        public override int ObjectWidth(KthuraObject obj) {
            TQMGImage tex;
            switch (obj.kind) {
                case "Zone":
                case "TiledArea":
                    return obj.w;
                case "Obstacle":
                case "Pic":
                    tex = GetTex(obj);
                    return tex.Width;
                default:
                    return 0;
            }
        }

        public override bool HasTexture(KthuraObject obj) {
            var tag = $"{obj.kind}::{obj.Texture}";
            return Textures.ContainsKey(tag);
        }

        #endregion
    }

}









