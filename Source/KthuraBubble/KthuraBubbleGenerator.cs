// Lic:
// KthuraBubble/KthuraBubbleGenerator.cs
// Kthura Bubble Map Generator
// version: 20.05.25
// Copyright (C) 2020 Jeroen P. Broks
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
using System;
using System.Collections.Generic;
using NSKthura;
using Bubble;
using TrickyUnits;
using UseJCR6;


namespace KthuraBubble {

    internal class KGPath {
        public int x, y,w,h;
        public byte dir;
        public int id;
        public bool _exit;
        public bool stopped = false;
        public override string ToString() {
            return $"[{id}]:KGPATH({x},{y}). Dir{dir}, AllowExit:{_exit}, Stopped:{stopped}";
        }
    }

    /// <summary>
    /// This class contains the map generator I used for "The Abyss" mission in Dyrt.NET
    /// Although you can customize it to your own design, the dungeon the way I set it up
    /// is my primary set up
    /// </summary>
    class KthuraAbyssGenerator {
        static Dictionary<string, string> CFG = new Dictionary<string, string>();
        static Kthura Map = null;
        public string statename { get; private set; }
        public string LuaTrace => SBubble.TraceLua(statename);
        static List<string> _IgnoreDestroy = new List<string>();
        static string PlatformTexture;
        static string BottomTexture;
        static string ExitTexture;
        static int SzX;
        static int SzY;
        static int BotW = 32, BotH = 26, TopW = 32, TopH = 32;


        // This looks idiotic, but for NLua this was the better way to go!
        public string TexPlatform {
            get => PlatformTexture;
            set { PlatformTexture = value; }
        }

        public string TexBottom {
            get => BottomTexture;
            set { BottomTexture = value; }
        }

        public void SetConfig(string k, string v) { CFG[k.ToUpper()] = v; }
        public string GetConfig(string k) {
            k = k.ToUpper();
            if (!CFG.ContainsKey(k)) return "";
            return CFG[k];
        }

        public int SizeX {
            set { SzX = value; }
            get => SzX;
        }
        public int SizeY {
            set { SzY = value; }
            get => SzY;
        }

        public string TexExit {
            get => ExitTexture;
            set { ExitTexture = value; }
        }




        string Config(string k) => GetConfig(k);
        void Config(string k, string v) => SetConfig(k, v);


        public void SetMap(int Tag) {
            Map = KthuraBubble.GetMap(Tag);
        }

        public void IgnoreDestroy(string Tag) { _IgnoreDestroy.Add(Tag); }

        public void SchoneOpruiming(string Layer, bool force) {
            if (!Map.Layers.ContainsKey(Layer)) { SBubble.MyError("Kthura Map Generator Error!", $"No layer named \"{Layer}\"!", LuaTrace); return; }
            var Oud = Map.Layers[Layer].Objects;
            var Nieuw = new List<KthuraObject>();
            if (!force) {
                foreach (KthuraObject o in Oud) {
                    if (o.Tag != "" && _IgnoreDestroy.Contains(o.Tag)) Nieuw.Add(o);
                }
            }
            Map.Layers[Layer].Objects = Nieuw;
        }

        public void Generate(string Layer, bool createifneeded) {
            if (!Map.Layers.ContainsKey(Layer)) {
                if (createifneeded) Map.Layers[Layer] = new KthuraLayer(Map); else { SBubble.MyError("Kthura Map Generator Error!", $"No layer named \"{Layer}\"!", LuaTrace); return; }
            }
            SchoneOpruiming(Layer,false);
            var Lay = Map.Layers[Layer];
            var Obj = Lay.Objects;
            // Back zone
            var Back = new KthuraObject("Zone", Lay);
            Back.x = 0;
            Back.y = 0;
            Back.w = SizeX;
            Back.h = SizeY;
            Back.Impassible = true;
            Back.Tag = $"Kthura_Abyss_Generator_TotalBlock_{Layer}";
            // var Plates
            var Plates = new List<KthuraObject>();
            // Start Plate
            var StartX = Rand.StepInt(0, SizeX - (TopW * 6), 32);
            var StartY = Rand.StepInt(0, SizeY - (TopH * 6), 32);
            var StartPlate = new KthuraObject("TiledArea", Lay);
            var StartPoint = new KthuraObject("Exit", Lay);
            StartPlate.x = StartX;
            StartPlate.y = StartY;
            StartPlate.w = TopW * 5;
            StartPlate.h = TopH * 5;
            StartPoint.Tag = "Start";
            StartPoint.MetaData["Wind"] = "South";
            StartPoint.x = StartX + (int)Math.Floor(TopW * 2.5);
            StartPoint.y = StartY + TopH * 3;
            Lay.Objects.Add(StartPoint);
            Plates.Add(StartPlate);
            // Generate Map
            int paths = Rand.Int(1, 6);
            var plist = new List<KGPath>();
            for (int q = 0; q < paths ; q++) {
                var p = new KGPath();
                p.x = StartX + TopW;
                p.y = StartY + TopH;
                p.dir = (byte)Rand.Int(1, 4);
                p._exit = q == 0;
                p.id = q;
                plist.Add(p);
            }
            bool alldone = false;
            do {
                alldone = true;
                foreach(KGPath p in plist) {
                    p.stopped = p.stopped || Rand.Int(1, 20) % 8 == 0;
                    alldone = alldone && p.stopped;
                    if (p.stopped) {
                        if (p._exit) {
                            //var O = new KthuraObject("Obstacle",Lay);
                            var O = new KthuraObject("TiledArea", Lay);
                            Lay.Objects.Add(O);
                            O.x = p.x + TopW; //+ (TopW / 2);
                            O.y = p.y + TopH;
                            O.w = TopW;
                            O.h = TopH;
                            O.Alpha1000 = 1000;
                            O.Dominance = 20;
                            O.Visible = true;
                            //O.Tag = "NPC_Next";
                            O.Tag = "ExitMap";
                            O.Texture = TexExit;
                            Lay.RemapTags();
                            p._exit = false;
                            BubConsole.CSay("Exit placed!");
                        }
                        continue;
                    }
                    byte nd;
                    do {
                        nd = (byte)Rand.Int(1, 4);
                    } while (nd == p.dir);
                    p.dir = nd;                    
                    switch (nd) {
                        case 1: { // North
                                if (p.y < TopH * 2) break;
                                var y = Rand.StepInt(TopH, p.y,TopH);
                                p.w = 3 * TopW;
                                p.h = p.y - y;
                                p.y = y;
                                var O = new KthuraObject("TiledArea", Lay);
                                O.x = p.x;
                                O.y = p.y;
                                O.w = p.w;
                                O.h = p.h;
                                Plates.Add(O);
                                BubConsole.CSay($"N: ({O.x},{O.y})/{O.w}x{O.h} .. {p}");
                            }
                            break;
                        case 2: {  // South
                                if (p.y > SizeY - (TopH * 2)) break;
                                var y = Rand.StepInt(p.y, SizeY - (TopH * 2), TopH);
                                p.w = 3 * TopW;
                                var O = new KthuraObject("TiledArea", Lay);
                                O.x = p.x;
                                O.y = p.y;
                                O.w = p.w;
                                O.h = y-p.y;
                                Plates.Add(O);
                                p.y = y - (TopH * 3);
                                BubConsole.CSay($"S: ({O.x},{O.y})/{O.w}x{O.h} .. {p}");
                            }
                            break;
                        case 3: { // West
                                if (p.x < TopW * 2) break;
                                var x = Rand.StepInt(TopW, p.x, TopW);
                                p.h = 3 * TopH;
                                p.w = p.x - x;
                                p.x = x;
                                var O = new KthuraObject("TiledArea", Lay);
                                O.x = p.x;
                                O.y = p.y;
                                O.w = p.w;
                                O.h = p.h;
                                Plates.Add(O);
                                BubConsole.CSay($"W: ({O.x},{O.y})/{O.w}x{O.h} .. {p}");

                            }
                            break;
                        case 4: {  // East
                                if (p.x > SizeX - (TopW * 2)) break;
                                var x = Rand.StepInt(p.x, SizeX - (TopW * 2), TopW);
                                p.h = 3 * TopH;
                                var O = new KthuraObject("TiledArea", Lay);
                                O.x = p.x;
                                O.y = p.y;
                                O.h = p.h;
                                O.w = x - p.x;
                                Plates.Add(O);
                                p.x = x - (TopW * 3);
                                BubConsole.CSay($"E: ({O.x},{O.y})/{O.w}x{O.h} .. {p}");
                            }
                            break;
                    }
                }
            } while (!alldone);

            // End processing Plates
            foreach(KthuraObject O in Plates) {
                O.ForcePassible = true;
                O.Dominance = 5;
                O.Texture = TexPlatform;
                O.Visible = true;
                O.Alpha1000 = 1000;
                Lay.Objects.Add(O);
                BubConsole.WriteLine($"Building with {O.kind}: ({O.x}x{O.y}) -- Visible: {O.Visible} -- Dominance: {O.Dominance}",(byte)Rand.Int(0,255), (byte)Rand.Int(0, 255), (byte)Rand.Int(0, 255));
                if (O.w > 0 && O.h > 0) {
                    var Bottom = new KthuraObject("TiledArea", Lay);
                    Bottom.Dominance = 2; // Make sure that if it collides with a plate on the south, that plate will always cover it
                    Bottom.x = O.x;
                    Bottom.y = O.y + O.h;
                    Bottom.w = O.w;
                    Bottom.h = BotH;
                    Bottom.Visible = true;
                    Bottom.Alpha1000 = 1000;
                    Bottom.Texture = TexBottom;
                    Lay.Objects.Add(Bottom);
                }
            }
            // Remap
            Lay.TotalRemap();
        }

        private KthuraAbyssGenerator() { }
        static public void Init(string astatename) {
            var Kth = new KthuraAbyssGenerator();
            var S = SBubble.State(astatename).state;
            S["BKTHURAGEN"] = Kth;
            Kth.statename = astatename;
            var bt = QuickStream.OpenEmbedded("KthuraBubbleGenerator.nil");
            var l = bt.ReadString((int)bt.Size);
            bt.Close();
            SBubble.DoNIL(astatename, l, "Kthura API Link Script");
        }

    }


}