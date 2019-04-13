// Lic:
// Class/KthuraDrawMonoGame.cs
// MonoGame Driver for Kthura for C#
// version: 19.04.13
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
using TrickyUnits;


namespace NSKthura {

	class KthuraDrawMonoGame:KthuraDraw{

        static KthuraDrawMonoGame me = new KthuraDrawMonoGame();
        static public void UseMe() => DrawDriver = me;
        

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
                var bt = map.TextureJCR.ReadFile(file);
                Textures[tag] = TQMG.GetImage(bt);
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

        void Ambtenaar() { }

        public override void DrawTiledArea(KthuraObject obj, int ix = 0, int iy = 0, int scrollx = 0, int scrolly = 0) {
            var tx = GetTex(obj);
            TQMG.Color((byte)obj.R, (byte)obj.G, (byte)obj.B);
            if (tx != null) TQMG.Tile(tx, obj.insertx, obj.inserty, obj.x + ix - scrollx, obj.y + iy - scrolly, obj.w, obj.h);
        }
    }

}




