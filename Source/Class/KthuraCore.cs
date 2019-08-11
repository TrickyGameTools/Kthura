// Lic:
// Class/KthuraCore.cs
// Kthura Core in C#
// version: 19.08.11
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







// For Debugging in Editor ONLY!
#undef DEBUGLOGCHAT

using System.Collections.Generic;
using System.Diagnostics;
using System;
using UseJCR6;
using TrickyUnits;
using TrickyUnits.Dijkstra;



namespace NSKthura {

    class KthuraObject {
        int cnt = 0;
        readonly public Dictionary<string, string> MetaData = new Dictionary<string, string>();
        public string kind { get; internal set; }
        readonly public KthuraLayer Parent;
        public string Texture = "";
        int _x = 0, _y = 0;
        public bool Visible = true;
        public int w = 0, h = 0;
        public int insertx = 0, inserty = 0;
        public int R = 255, G = 255, B = 255;
        public int ScaleX = 1000, ScaleY = 1000;
        public int AnimSpeed = 0;
        public int AnimFrame = 0;
        int _Dominance = 20;
        public float TrueScaleX => (float)ScaleX / 1000;
        public float TrueScaleY => (float)ScaleY / 1000;
        string _Labels = "";
        string _Tag = "";
        bool _impassible = false;
        bool _forcepassible = false;
        public string Tag {
            get => _Tag; set {
                _Tag = value;
                if (Kthura.automap) Parent.RemapTags();
            }
        }

        public int x {
            get => _x;
            set {
                _x = value;
                if (Kthura.automap) Parent.RemapDominance();
            }
        }

        public int y {
            get => _y;
            set {
                _y = value;
                if (Kthura.automap) Parent.RemapDominance();
            }
        }
        public int Dominance {
            get => _Dominance;
            set {
                _Dominance = value;
                if (Kthura.automap) Parent.RemapDominance();
            }
        }
        public string Labels {
            get => _Labels;
            set {
                _Labels = value;
                if (Kthura.automap) Parent.RemapDominance();
            }
        }



        public bool Impassible {
            get => _impassible;
            set {
                _impassible = value;
                if (Kthura.automap) Parent.BuildBlockMap();
            }
        }



        public bool ForcePassible {
            get => _forcepassible;
            set {
                _forcepassible = value;
                if (Kthura.automap) Parent.BuildBlockMap();
            }
        }



        public string DomMapVal => $"{Dominance.ToString("D9")}.{y.ToString("D9")}.{x.ToString("D9")}.{cnt.ToString("D9")}";
        // Dominance takes prioity over all. When domincance is the same then y will play a role, and then the x value.
        // cnt just has to make sure every value is unique.

        #region recalc values

        int _alpha1000 = 0;
        int _alpha255 = 0;
        public int Alpha255 {
            get => _alpha255;
            set {
                _alpha255 = value;
                if (_alpha255 < 0) _alpha255 = 0;
                if (_alpha255 > 255) _alpha255 = 255;
                _alpha1000 = (int)((float)(((float)value / (float)255) * (float)1000));
                if (_alpha1000 < 0) _alpha1000 = 0;
                if (_alpha1000 > 1000) _alpha1000 = 1000;
                Debug.WriteLine($"Alpha255 set. 1000={Alpha1000}; 255={Alpha255};");
            }
        }
        public int Alpha1000 {
            // Due to BlitzMax settings I had to use a 1=max scale, as I needed Kthura not to suffer from rouding errors, the 1000 scale has been set up.
            // Now MonoGame uses the (actual) 0-255 scale, but as Kthura maps do not cover that properly, this measure was taken.
            get => _alpha1000; /*{
                float A = Alpha255;
                return (int)(((float)A / 255) * 1000);
            }*/
            set {
                _alpha1000 = value;
                if (_alpha1000 < 0) _alpha1000 = 0;
                if (_alpha1000 > 1000) _alpha1000 = 1000;
                _alpha255 = (int)((float)(((float)value / 1000) * 255));
                if (_alpha255 < 0) _alpha255 = 0;
                if (_alpha255 > 255) _alpha255 = 255;
                Debug.WriteLine($"Alpha1000 set. 1000={Alpha1000}; 255={Alpha255};");
            }
        }



        double _rotrad = 0;
        int _rotdeg = 0;
        public double RotationRadians {
            set {
                _rotrad = value;
                _rotdeg = (int)(value * (180 / Math.PI));
            }
            get => _rotrad;
        }

        public int RotationDegrees {
            get => _rotdeg;
            set {
                _rotdeg = value;
                _rotrad = (double)(((double)value) * (180 / Math.PI));
            }
        }
        #endregion

        public KthuraObject(string objectkind, KthuraLayer prnt) {
            kind = objectkind;
            Parent = prnt;
            cnt = Parent.cnt;
            Parent.cnt++;
            prnt.Objects.Add(this);
        }
    }

    class KthuraActor : KthuraObject {
        /* Stuff which came from the original Kthura, as a kind of personal note!
    Field SinglePicFile$,PicBundleDir$
	Field SinglePic:TImage
	Field PicBundle:TMap
	Field ChosenPic$
	Field InMotion = False
	Field NotInMotionThen0 = True
	Field Walking
	Field WalkingIsMotion = True
	Field WalkX, WalkY
	Field Moving,MoveIgnoreBlock
	Field UnMoveTimer = 4
	Field MoveX, MoveY
	Field MoveSkip = 4
	Field FrameSpeed = 4
	Field FrameSpeedCount
	'Field WalkSkip = 4 'no longer needed
	Field WalkSpot = 0
	Field Wind$ = "North"
	Field WalkingToX,WalkingToY	
	Field FoundPath:PathFinderUnit
	Field PathLength
	Field Cycle = -1
    */
        public object DriverTextureObject = null; // To be defined by the graphics driver for its own needs
        public string ChosenPic = "";
        bool _InMotion;
        public bool InMotion {
            set => _InMotion = value;
            get {
                if (WalkingIsInMotion)
                    return Walking || Moving;
                else
                    return _InMotion;
            }
        }
        public bool NotInMotionThen0 = true;
        public bool Walking = false;
        public bool WalkingIsInMotion = true;
        public bool Moving = false;
        public bool MoveIgnoreBlock = false;
        public bool AutoWind = true;
        public int UnMoveTimer = 4;
        public int MoveX = 0, MoveY = 0;
        public int MoveSkip = 4;
        public int FrameSpeed = 4;
        public int FrameSpeedCount = 0;
        public int WalkSpot = 0;
        public string Wind = "North";
        public int WalkingToX = 0, WalkingToY = 0;
        public Path FoundPath = null;
        public int PathIndex = 0;
        public int PathLength {
            get {
                if (FoundPath == null) return 0;
                return FoundPath.Nodes.Length;
            }
        }
        public int Cycle = -1;
                
        public int CWalkX {
            get {
                if (Walking) return FoundPath.Nodes[PathIndex].x;
                return 0;
            }
        }


        public int CWalkY {
            get {
                if (Walking) return FoundPath.Nodes[PathIndex].y;
                return 0;
            }
        }

        public KthuraActor(KthuraLayer prnt):base("Actor",prnt) {
                        
        }

        void Walk2Move() {
            MoveX = (CWalkX * Parent.GridX) + (Parent.GridX / 2);
            MoveY = (CWalkY * Parent.GridY) + (Parent.GridY);
            Moving = true;
        }

        /// <summary>
        /// Will make an actor walk to the given spot if it can be reached. Kthura will use my Dijkstra class for this.
        /// </summary>
        /// <param name="to_x"></param>
        /// <param name="to_y"></param>
        /// <param name="real"></param>
        public void WalkTo(int to_x, int to_y,bool real=true) {
            var gridx = Parent.GridX;
            var gridy = Parent.GridY;
            int tox = to_x, toy = to_y;
            int fromx = x, fromy = y;
            if (real) {
                tox = to_x / gridx;
                toy = to_y / gridy;
                fromx = x / gridx;
                fromy = y / gridy;
            }
            FoundPath = Dijkstra.QuickPath(Parent.PureBlockRev, Parent.BlockMapWidth, Parent.BlockMapHeight, fromx, fromy, tox, toy);
            if (FoundPath.Success) {
                PathIndex = 0;
                Walking = true;
                WalkingToX = to_x; //FoundPath.Nodes[0].x;
                WalkingToY = to_y; //FoundPath.Nodes[1].y;
                MoveX = x;
                MoveY = y;
                Walk2Move();
            } else {
                Walking = false;
                FoundPath = null;                
            }
        }


        public void WalkTo(KthuraObject obj) {
            WalkTo(obj.x, obj.y, true);
        }

        public void WalkTo(string tag) {
            if (!Parent.HasTag(tag)) throw new Exception($"Object tagged `{tag}` not found");
            WalkTo(Parent.FromTag(tag));
        }

        static public KthuraActor Spawn(KthuraLayer parent,string spot) {
            var ret = new KthuraActor(parent);
            if (!parent.HasTag(spot)) {
                Debug.WriteLine($"WARNING! I cannot spawn an actor on not existent spot {spot}!");
                return null;
            }
            var obj = parent.FromTag(spot);
            ret.x = obj.x;
            ret.y = obj.y;
            ret.Dominance = obj.Dominance;
            ret.Alpha255 = 255;
            ret.R = 255;
            ret.G = 255;
            ret.B = 255;
            ret.Visible = true;
            return ret;
        }

        public void UpdateMoves() {
            if (Moving || Walking) {
                if (MoveX < x) { x -= MoveSkip; if (x < MoveX) x = MoveX; if (AutoWind) Wind = "WEST"; }
                if (MoveX > x) { x += MoveSkip; if (x > MoveX) x = MoveX; if (AutoWind) Wind = "EAST"; }
                if (MoveY < y) { y -= MoveSkip; if (y < MoveY) y = MoveY; if (AutoWind) Wind = "NORTH"; }
                if (MoveY > y) { y += MoveSkip; if (y > MoveY) y = MoveY; if (AutoWind) Wind = "SOUTH"; }
                if (MoveX == x && MoveY == y) {
                    if (!Walking)
                        Moving = false;
                    else {
                        PathIndex++;
                        if (PathIndex >= PathLength) {
                            Walking = false;
                            Moving = false;
                        } else {
                            Walk2Move();
                        }
                    }
                }
            } else {
                MoveX = x;
                MoveY = y;
            }
            if ((WalkingIsInMotion && Walking) || InMotion) {
                FrameSpeedCount++;
                if (FrameSpeedCount >= FrameSpeed) {
                    FrameSpeedCount = 0;
                    AnimFrame++;
                }
            } else if (WalkingIsInMotion && (!Walking)) AnimFrame = 0;
        }
    }



    class KthuraLayer {
        internal int cnt = 0;
        internal List<KthuraObject> Objects = new List<KthuraObject>(); // Really this is basically the true core of Kthura!
        Dictionary<string, KthuraObject> TagMap = new Dictionary<string, KthuraObject>();
        Dictionary<string, List<KthuraObject>> LabelMap = new Dictionary<string, List<KthuraObject>>();
        public List<KthuraObject> ObjectDrawOrder { get; private set; } = new List<KthuraObject>();
        readonly public Kthura Parent;
        int BlockMapBoundW = 100;
        int BlockMapBoundH = 100;
        bool[,] BlockMap;
        public int BlockMapWidth => BlockMap.GetLength(0);
        public int BlockMapHeight => BlockMap.GetLength(1);



        public int GridX = 32, GridY = 32;
        public KthuraLayer(Kthura hufter) {
            Parent = hufter ?? throw new Exception("What the....... do you think you're doing???");
        }
        public bool HasTag(string Tag, bool AlwaysRemapFirst = false) {
            if (AlwaysRemapFirst) RemapTags();
            return TagMap.ContainsKey(Tag);
        }

        #region Tagfunctions
        public KthuraObject FromTag(string tag) {
            if (!TagMap.ContainsKey(tag)) return null;
            return TagMap[tag];
        }

        public Dictionary<string, KthuraObject>.KeyCollection Tags => TagMap.Keys;
        #endregion


        #region Remap
        /// <summary>
        /// Updates the tagmap. Will rebuild the blockmap. Please note that if Kthura.automap is set to true (that's by default the case), this will happen automatically, when needed. However for performance optimization reasons you can set it to false, and use this function manually.
        /// </summary>
        public void RemapTags() {
            TagMap.Clear();
            foreach (KthuraObject o in Objects) {
                if (o.Tag != "") {
                    var ok = true;
                    if (o.Tag != o.Tag.Trim()) { ok = false; Kthura.Log($"RemapTags: \"{o.Tag}\": invalid tag!"); }
                    if (TagMap.ContainsKey(o.Tag) && o != TagMap[o.Tag]) { ok = false; Kthura.Log($"RemapTags: \"{o.Tag}\": duplicate tag!"); }
                    if (ok) TagMap[o.Tag] = o;
                }
            }
        }

        /// <summary>
        /// Remaps drawing order based on dominance settings. Will rebuild the blockmap. Please note that if Kthura.automap is set to true (that's by default the case), this will happen automatically, when needed. However for performance optimization reasons you can set it to false, and use this function manually.
        /// </summary>
        public void RemapDominance() {
            // This was (for now) the easiest way to go, but maybe not the most optimal.
            // If you think you got a faster method... Lemme know!
            var DominanceMap = new SortedDictionary<string, KthuraObject>();
            foreach (KthuraObject o in Objects)
                DominanceMap[o.DomMapVal] = o;
            ObjectDrawOrder.Clear();
            foreach (KthuraObject o in DominanceMap.Values) ObjectDrawOrder.Add(o); //.Insert(0, o);
        }


        /// <summary>
        /// Remaps labels. Will rebuild the blockmap. Please note that if Kthura.automap is set to true (that's by default the case), this will happen automatically, when needed. However for performance optimization reasons you can set it to false, and use this function manually.
        /// </summary>
        public void RemapLabels() {
            LabelMap.Clear();
            foreach (KthuraObject o in Objects) {
                if (o.Labels != "") {
                    var sp = o.Labels.Split(',');
                    foreach (string s in sp) {
                        var tl = s.Trim();
                        if (tl == "")
                            Kthura.Log("RemapLabels: Label syntax error");
                        else {
                            if (!LabelMap.ContainsKey(tl))
                                LabelMap[tl] = new List<KthuraObject>();
                            LabelMap[tl].Add(o);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Will rebuild the blockmap. Please note that if Kthura.automap is set to true (that's by default the case), this will happen automatically, when needed. However for performance optimization reasons you can set it to false, and use this function manually.
        /// </summary>
        public void BuildBlockMap() { /* TODO: Code the damn blockmap builder! */
        // KthuraObject O;
        var GW = GridX;
            var GH = GridY;
            int X, Y, W, H; //BX, BY,
            int TX, TY, AX, AY, TW, TH;
            int BoundX = 0, BoundY = 0;
            int iw, tiw, ih, tih;
            // Let's first get the bounderies
            foreach (KthuraObject O in Objects) {
                X = O.x; if (X < 0) X = 0;
                Y = O.y; if (Y < 0) Y = 0;
                W = O.w - 1; if (W < 0) W = 0;
                H = O.h - 1; if (H < 0) H = 0;
                switch (O.kind) {
                    case "TiledArea":
                    case "Zone":
                        TX = (int)Math.Ceiling((decimal)((X + W) / GW));
                        TY = (int)Math.Ceiling((decimal)((Y + H) / GH));
                        if (TX > BoundX) BoundX = TX;
                        if (TY > BoundY) BoundY = TY;
                        break;
                    case "Obstacle":
                        TX = (int)Math.Floor((decimal)(X / GW));
                        TY = (int)Math.Floor((decimal)(Y / GH));
                        if (TX > BoundX) BoundX = TX;
                        if (TY > BoundY) BoundY = TY;
                        break;
                    case "Pic":
                        TX = (int)Math.Floor((decimal)X / GW);
                        TY = (int)Math.Floor((decimal)Y / GW);
                        if (TX > BoundX) BoundX = TX;
                        if (TY > BoundY) BoundY = TY;
                        break;
                }
            }
            BlockMapBoundW = BoundX;
            BlockMapBoundH = BoundY;
            BlockMap = new bool[BoundX + 1, BoundY + 1];
            // And now for the REAL work.		
            foreach (KthuraObject O in Objects) {
                if (O.Impassible) {
                    X = O.x; if (X < 0) X = 0;
                    Y = O.y; if (Y < 0) Y = 0;
                    W = O.w - 1; if (W < 0) W = 0;
                    H = O.h - 1; if (H < 0) H = 0;
                    switch (O.kind) {
                        case "TiledArea":
                        case "Zone":
                            Kthura.EDITTORLOG($"Working on Impassible {O.kind} {O.Tag}");
                            TX = (int)Math.Floor((decimal)X / GW);
                            TY = (int)Math.Floor((decimal)Y / GH);
                            TW = (int)Math.Ceiling((decimal)((X + W) / GW));
                            TH = (int)Math.Ceiling((decimal)((Y + H) / GH));
                            //Print "DEBUG: Blockmapping area ("+TX+","+TY+") to ("+TW+","+TH+")"
                            for (AX = TX; AX <= TW; AX++) {
                                for (AY = TY; AY <= TH; AY++) {
                                    //for (AX = TX; AX < TW; AX++) {
                                    //    for (AY = TY; AY < TH; AY++) {
                                    try {
                                        Kthura.EDITTORLOG($"Blocking {AX},{AY}");
                                        BlockMap[AX, AY] = true;
                                    } catch {
                                        throw new Exception($"BlockMap[{AX},{AY}]: Out of bounds ({BlockMap.GetLength(0)}x{BlockMap.GetLength(1)})");
                                    }
                                }
                            }
                            break;
                        case "Obstacle":
                            TX = (int)Math.Floor((decimal)(X / GW));
                            TY = (int)Math.Floor((decimal)((Y - 1) / GH));
                            BlockMap[TX, TY] = true;
                            if (KthuraDraw.DrawDriver.HasTexture(O))
                                iw = KthuraDraw.DrawDriver.ObjectWidth(O);
                            else
                                iw = 0;
                            tiw = (int)Math.Ceiling((decimal)iw / GW) - 1;

                            for (AX = TX - (tiw); AX <= TX + (tiw); AX++) {
                                if (AX >= 0 && AX <= BoundX && TY <= BoundY && TY >= 0) BlockMap[AX, TY] = true;
                            }
                            break;
                        case "Pic":
                            TX = (int)Math.Floor((decimal)X / GW);
                            TY = (int)Math.Floor((decimal)Y / GH);
                            BlockMap[TX, TY] = true;
                            if (KthuraDraw.DrawDriver.HasTexture(O)) {
                                iw = KthuraDraw.DrawDriver.ObjectWidth(O); //ImageWidth(o.textureimage)
                                tiw = (int)Math.Ceiling((decimal)iw / GW);
                                ih = KthuraDraw.DrawDriver.ObjectHeight(O); //ImageHeight(o.textureimage)
                                tih = (int)Math.Ceiling((decimal)ih / GH);
                                for (AX = TX; AX < TX + (tiw); AX++) for (AY = TY; AY < TY + tih; AY++) {
                                        if (AX >= 0 && AX <= BoundX && AY <= BoundY && AY >= 0) BlockMap[AX, AY] = true;
                                    }
                            }
                            break;
                    }
                }
            }
            // And this will force a way open if applicable	
            foreach (KthuraObject O in Objects) {
                if (O.ForcePassible) {
                    X = O.x; if (X < 0) X = 0;
                    Y = O.y; if (Y < 0) Y = 0;
                    W = O.w - 1; if (W < 0) W = 0;
                    H = O.h - 1; if (H < 0) H = 0;
                    switch (O.kind) {
                        case "TiledArea":
                        case "Zone":
                            TX = (int)Math.Floor((decimal)(X / GW));
                            TY = (int)Math.Floor((decimal)(Y / GH));
                            TW = (int)Math.Ceiling((decimal)((X + W) / GW));
                            TH = (int)Math.Ceiling((decimal)((Y + H) / GH));
                            //Print "DEBUG: Blockmapping area ("+TX+","+TY+") to ("+TW+","+TH+")"
                            for (AX = TX; AX <= TW; AX++) {
                                for (AY = TY; AY <= TH; AY++) {
                                    BlockMap[AX, AY] = false;
                                }
                            }
                            break;
                    }
                }
            }
        }



        public bool Block(int X, int Y) {
            //Kthura.EDITTORLOG($"Block({X},{Y})");
            int GW = GridX;
            int GH = GridY;
            int TX = (int)Math.Floor((decimal)X / GW);
            int TY = (int)Math.Floor((decimal)Y / GH);
            int BX = BlockMapBoundW;
            int BY = BlockMapBoundH;
            if (TX < 0) return true;
            if (TY < 0) return true;
            if (TX > BX) return true;
            if (TY > BY) return true;
            //Kthura.EDITTORLOG($"Not stuck in boundaries, so let's return {BlockMap[TX,TY]}");
            return BlockMap[TX, TY];
        }

        public bool PureBlock(int x, int y) {
            int BX = BlockMapBoundW;
            int BY = BlockMapBoundH;
            if (x < 0) return true;
            if (y < 0) return true;
            if (x > BX) return true;
            if (y > BY) return true;
            //Kthura.EDITTORLOG($"Not stuck in boundaries, so let's return {BlockMap[TX,TY]}");
            return BlockMap[x, y];
        }

        /// <summary>
        /// Returns true if passible and false if not, which is reversed to what Kthura normally does. This function has been set up most of all for compatibility issues with my Dijkstra routine, but as most other Dijkstra and A* routines want 'true' for walkable areas and false for impassible ones, I decided to follow suit.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public bool PureBlockRev(int x, int y) => !PureBlock(x, y);


        public void TotalRemap() {
            RemapDominance();
            RemapTags();
            RemapLabels();
            BuildBlockMap();
        }
        #endregion
    }


    delegate void KthuraLog(string l);


    /// <summary>
    /// The class containing Kthura core features and some specific data about a Kthura map.
    /// </summary>
    class Kthura {
        #region debug
        /// <summary>
        /// This delegate function can put messages on the debug console of Visual Studio. Kthura can use it to find errors in its own system or in your maps. If you want another way to make Kthura do that, hey write your own function and put it in this delegate.
        /// </summary>
        static public KthuraLog Log = delegate (string m) { Debug.Print(m); };
        static internal void EDITTORLOG(string m) {
#if DEBUGLOGCHAT
            KthuraEdit.Stages.DBG.Log(m);
#endif
        }
        #endregion

        #region Data specific to the map!
        /// <summary>
        /// When set to true, values requiring remapping (such as Tags, Lables, Dominance, BlockMapValues and (believe it or not) the y coordinate) will immediately cause that to happen.
        /// Please note that that if you have a lot of modifications to do, this will take away performance, so then you can best turn it off, do your modifications and remap everything later.
        /// </summary>
        static public bool automap = true;
        public TJCRDIR TextureJCR;
        public SortedDictionary<string, KthuraLayer> Layers = new SortedDictionary<string, KthuraLayer>();
        public SortedDictionary<string, string> MetaData = new SortedDictionary<string, string>();

        #endregion

        #region Control Methods
        public void CreateLayer(string name) {
            if (Layers.ContainsKey(name)) {
                Debug.Print($"Kthura map already has a layer named {name}. Please pick a different name!");
                return;
            }
            Layers[name] = new KthuraLayer(this);
        }
        #endregion

        #region New Map constructors
        private Kthura() { } // I won't allow "New". A bit dangerous. So some static functions act as "constructors".
        public static Kthura Create() => Create(DefaultTextureJCR);
        public static Kthura Create(TJCRDIR TexJCR) {

            var ret = new Kthura {
                TextureJCR = TexJCR
            };
            // Please note! Single layer maps, were already officially deprecated a few years before this project began in C#
            // And if BlitzMax didn't get into the state it's now, this C# version may actually never have been created (at least not by me)
            // It's only logical I will not support it any more in the C# version :P
            ret.Layers["_BASE"] = new KthuraLayer(ret);
            // All done
            return ret;
        }

        public static Kthura Load(TJCRDIR sourcedir, string prefix, TJCRDIR TexJcr) {

            bool dochat = true;

            void crash(string er) => throw new Exception($"KthuraLoad: {er}");
            void assert(bool o, string er) {
                if (!o) crash(er);
            }
            void chat(string cm) {
                if (dochat) Debug.WriteLine($"KTHURA.LOAD.CHAT: {cm}");
            }


            // This is the TRUE load routine. All overloads eventually lead to this one! ;-P
            var ret = new Kthura {
                MetaData = new SortedDictionary<string, string>(),
                TextureJCR = TexJcr
            };
            var m = sourcedir.LoadStringMap($"{prefix}Data");
            foreach (string K in m.Keys)
                ret.MetaData[K] = m[K];
            var olines = sourcedir.ReadLines($"{prefix}Objects", true);
            var readlayers = false;
            bool tempautomap = automap; automap = false; // In order to fasten up the process this will be off temporarily. It can be turned on again, afterward!            
            string curlayername = "";
            KthuraObject obj = null;
            KthuraLayer curlayer = null;
            // I should have done better than this, but what works that works!
            var cnt = 0;
            foreach (string rl in olines) {
                var l = rl.Trim();
                cnt++;
                if ((!qstr.Prefixed(l, "--")) && (!(qstr.Prefixed(l, "#") && readlayers==false)) && l != "") {
                    if (l == "LAYERS")
                        readlayers = true;
                    else if (l == "__END")
                        readlayers = false;
                    else if (readlayers)
                        ret.Layers[l] = new KthuraLayer(ret);
                    else if (l == "NEW") {
                        obj = new KthuraObject("?", ret.Layers[curlayername]);
                        chat($"New object in {curlayername}");
                    } else {
                        var pi = l.IndexOf('='); if (pi < 0) throw new Exception($"Syntax error: \"{l}\" in {cnt}");
                        var key = l.Substring(0, pi).Trim();
                        var value = l.Substring(pi + 1).Trim();
                        string[] s;
                        chat($"{key} = \"{value}\"");
                        switch (key) {
                            // Layer as a whole
                            case "LAYER":
                                obj = null;
                                curlayername = value;
                                curlayer = ret.Layers[value];
                                break;
                            case "GRID":
                                if (curlayer == null) crash("GRID needs a layer!");
                                s = value.Split('x');
                                if (s.Length != 2) crash("GRID syntax error!");
                                curlayer.GridX = qstr.ToInt(s[0].Trim());
                                curlayer.GridY = qstr.ToInt(s[1].Trim());
                                break;
                            // Object specific
                            case "KIND":
                                assert(obj != null, "KIND: No Object");
                                obj.kind = value;
                                break;
                            case "COORD":
                                assert(obj != null, "COORD: No Object");
                                s = value.Split(',');
                                assert(s.Length == 2, "COORD syntax error!");
                                obj.x = qstr.ToInt(s[0].Trim());
                                obj.y = qstr.ToInt(s[1].Trim());
                                chat($"Coordinates set({obj.x},{obj.y}) of {obj.kind}");
                                break;
                            case "INSERT":
                                assert(obj != null, "INSERT: No Object");
                                s = value.Split(',');
                                assert(s.Length == 2, "INERT syntax error!");
                                obj.insertx = qstr.ToInt(s[0].Trim()) * (-1);
                                obj.inserty = qstr.ToInt(s[1].Trim()) * (-1);
                                break;
                            case "ROTATION":
                                assert(obj != null, "ROTATION: No object");
                                obj.RotationDegrees = qstr.ToInt(value);
                                break;
                            case "SIZE":
                                assert(obj != null, "SIZE: No Object");
                                s = value.Split('x');
                                assert(s.Length == 2, "SIZE syntax error!");
                                obj.w = qstr.ToInt(s[0].Trim());
                                obj.h = qstr.ToInt(s[1].Trim());
                                break;
                            case "TAG":
                                assert(obj != null, "TAG: No Object!");
                                obj.Tag = value;
                                break;
                            case "LABELS":
                                assert(obj != null, "LABELS: No Object!");
                                obj.Labels = value;
                                break;
                            case "DOMINANCE":
                                assert(obj != null, "DOMINANCE: No object");
                                obj.Dominance = qstr.ToInt(value);
                                break;
                            case "TEXTURE":
                                assert(obj != null, "TEXTURE: No object!");
                                obj.Texture = value;
                                break;
                            case "CURRENTFRAME":
                                assert(obj != null, "CURRENT FRAME: No object");
                                obj.AnimFrame = qstr.ToInt(value);
                                break;
                            case "FRAMESPEED":
                                assert(obj != null, "FRAME SPEED: No object");
                                obj.AnimSpeed = qstr.ToInt(value);
                                break;
                            case "ALPHA":
                                assert(obj != null, "ALPHA: No object");
                                obj.Alpha1000 = (int)(qstr.ToDouble(value) * 1000);
                                break;
                            case "VISIBLE":
                                assert(obj != null, "VISIBLE: No object");
                                obj.Visible = qstr.ToInt(value) == 1;
                                break;
                            case "COLOR":
                                assert(obj != null, "COLOR: No Object");
                                s = value.Split(',');
                                assert(s.Length == 3, "COLOR syntax error!");
                                obj.R = qstr.ToInt(s[0].Trim());
                                obj.G = qstr.ToInt(s[1].Trim());
                                obj.B = qstr.ToInt(s[2].Trim());
                                break;
                            case "IMPASSIBLE":
                                assert(obj != null, "IMPASSIBLE: No object");
                                obj.Impassible = qstr.ToInt(value) == 1;
                                break;
                            case "FORCEPASSIBLE":
                                assert(obj != null, "FORCE PASSIBLE: No object");
                                obj.ForcePassible = qstr.ToInt(value) == 1;
                                break;
                            case "SCALE":
                                assert(obj != null, "SCALE: No Object");
                                s = value.Split(',');
                                assert(s.Length == 2, "SCALE syntax error!");
                                obj.ScaleX = qstr.ToInt(s[0].Trim());
                                obj.ScaleY = qstr.ToInt(s[1].Trim());
                                break;
                            case "BLEND":
                                if (qstr.ToInt(value) != 0) Debug.Print("Alternate Blends are only supported in the BlitzMax version of Kthura!");
                                break;
                            default:
                                if (qstr.Prefixed(key, "DATA.")) {
                                    var _Key = key.Substring(5).Trim();
                                    obj.MetaData[_Key] = value.Trim();
                                }
                                Debug.Print($"Unknown object key {key}");
                                break;
                        }
                    }
                }
            }
            automap = tempautomap;
            foreach (KthuraLayer lay in ret.Layers.Values) lay.TotalRemap();
            return ret;
        }






        public static Kthura Load(string sourcefile, string prefix, TJCRDIR TexJCR) {

            var sf = JCR6.Dir(sourcefile);

            return Load(sf, prefix, TexJCR);

        }

        public static Kthura Load(TJCRDIR sourcedir, string prefix = "") {

            var tj = DefaultTextureJCR;

            if (tj == null) tj = sourcedir;

            return Load(sourcedir, prefix, tj);

        }

        public static Kthura Load(string sourcefile, string prefix = "") {

            var sf = JCR6.Dir(sourcefile);

            return Load(sf, prefix);

        }

        #endregion

        #region Core functions as statics (since C# requires classes even when you don't need them).
        static TJCRDIR DefaultTextureJCR = null;
        static public void SetDefaultTextureJCR(TJCRDIR j) => DefaultTextureJCR = j;
        #endregion
    }

}

















