// Lic:
// Class/KthuraDrawMonoGame.cs
// MonoGame Driver for Kthura for C#
// version: 20.05.25
// Copyright (C) 2019 Jeroen P. Broks
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
        static public TQMGImage NoTexture = null;
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
                    if (bt == null) {
                        if (NoTexture != null) {
                            Textures[tag] = NoTexture; //TQMG.GetImage(NoTexture.GetTex(0));
                            System.Console.Beep();
                            Debug.WriteLine($"Texture {file} for {tag} could not be loaded , so set to alternet picture in stead ({UseJCR6.JCR6.JERROR})");
                            //return NoTexture;
                        } else {
                            CrashOnNoTex?.Invoke($"Couldn't open texture file {file} for {tag}");
                            Debug.WriteLine($"Couldn't open texture file {file} for {tag}");
                            return null;
                        }
                    } else {
                        Textures[tag] = TQMG.GetImage(bt);
                        if (Textures[tag] == null) {
                            if (NoTexture != null) {
                                Textures[tag] = NoTexture;
                                System.Console.Beep();
                                Debug.WriteLine($"Texture {tag} could not be loaded, so set to alternet picture in stead");
                            } else
                                CrashOnNoTex?.Invoke($"Texture `{file}` didn't load at all on tag {tag}.\n{UseJCR6.JCR6.JERROR}");

                        }
                    }
                }
                if (Textures[tag].Frames == 0) {
                    CrashOnNoTex?.Invoke($"Texture `{file}` for tag `{tag}` has no frames");
                }
                if (obj.kind=="Obstacle" || obj.kind=="Actor") Textures[tag].HotBottomCenter();
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
            if (tx != null) TQMG.Tile(tx, obj.insertx, obj.inserty, obj.x + ix - scrollx, obj.y + iy - scrolly, obj.w, obj.h, obj.AnimFrame);
            TQMG.SetAlpha(255);
        }

        public override void DrawStretchedArea(KthuraObject obj , int ix = 0, int iy = 0, int scrollx = 0, int scrolly = 0) {
            var tx = GetTex(obj);
            TQMG.Color((byte)obj.R, (byte)obj.G, (byte)obj.B);
            TQMG.SetAlpha((byte)obj.Alpha255);
            if (tx != null) tx.StretchDraw(obj.x + ix - scrollx, obj.y + iy - scrolly, obj.w, obj.h, obj.AnimFrame);
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
                tx.XDraw(obj.x + ix - scrollx, obj.y + iy - scrolly, obj.AnimFrame);
                TQMG.Scale(1000, 1000);
                TQMG.RotateRAD(0);
                TQMG.SetAlpha(255);
            } else CrashOnNoTex?.Invoke($"Obstacle-texture '{obj.Texture}' did somehow not load?");
        }

        override public void DrawPic(KthuraObject obj, int ix = 0, int iy = 0, int scrollx = 0, int scrolly = 0) {
            DrawObstacle(obj, ix, iy, scrollx, scrolly); // Seems odd, but trust me... :-P
        }

        public override void DrawActor(KthuraActor obj, int ix = 0, int iy = 0, int scrollx = 0, int scrolly = 0) {
            var tx = GetTex(obj);
            if (tx != null) {
                obj.UpdateMoves();
                TQMG.Color((byte)obj.R, (byte)obj.G, (byte)obj.B);
                //TQMG.SetAlphaFloat((float)obj.Alpha1000 / 1000);
                TQMG.SetAlpha((byte)obj.Alpha255);
                //TQMG.RotateRAD((float)obj.RotationRadians);
                TQMG.RotateDEG(obj.RotationDegrees);
                TQMG.Scale(obj.ScaleX, obj.ScaleY);
                if (obj.AnimFrame >= tx.Frames) obj.AnimFrame = 0;
                tx.XDraw(obj.x + ix - scrollx, obj.y + iy - scrolly,obj.AnimFrame);
                TQMG.Scale(1000, 1000);
                TQMG.RotateRAD(0);
                TQMG.SetAlpha(255);
            } else CrashOnNoTex?.Invoke($"Actor-texture '{obj.Texture}' did somehow not load?");            
        }

        public override int ObjectHeight(KthuraObject obj) {
            TQMGImage tex;
            switch (obj.kind) {
                case "Zone":
                case "TiledArea":
                case "StretchedArea":
                    return obj.h;
                case "Obstacle":
                case "Pic":
                case "Actor":
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
                case "StretchedArea":
                    return obj.w;
                case "Obstacle":
                case "Pic":
                case "Actor":
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

        public override void AnimReset(KthuraObject obj) {
            var tex = GetTex(obj);
            if (tex == null) return;
            if (obj.AnimFrame >= tex.Frames) obj.AnimFrame = 0;
        }

        #endregion
    }

}