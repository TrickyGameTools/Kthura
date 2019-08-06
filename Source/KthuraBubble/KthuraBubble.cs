using NSKthura;
using Bubble;
using System;
using System.Text;
using System.Collections.Generic;
using TrickyUnits;
using UseJCR6;

namespace KthuraBubble {

    class BubKthScroll {
        public int ScrollX;
        public int ScrollY;
        public BubKthScroll(int x=0,int y = 0) { ScrollX = x; ScrollY = 0; }
    }

	class KthuraBubble {
		Dictionary<int,Kthura> KMaps = new Dictionary<int,Kthura>();
        Dictionary<int, BubKthScroll> KScroll = new Dictionary<int, BubKthScroll>();
        Dictionary<int, string> Layers = new Dictionary<int, string>();
        TMap<int, bool> AutoRemap = new TMap<int, bool>();
       
        int Assign(Kthura map) {
            int i = 0;
            do i++; while (KMaps.ContainsKey(i));
            KMaps[i] = map;
            KScroll[i] = new BubKthScroll();
            AutoRemap[i] = true;
            foreach(string first in map.Layers.Keys) { 
                // Is there a better way?
                Layers[i] = first;
                break;
            }
            return i;
        }

        public void SetAutoRemap(int ID, bool value) {
            AutoRemap[ID] = value;
        }

        public void TotalRemap(int ID) {
            try {
                KMaps[ID].Layers[Layers[ID]].TotalRemap();
            } catch (Exception Ramp) { // Ramp means "disaster" in Dutch
                if (!KMaps.ContainsKey(ID))
                    Crash($"There is no map index ${ID}");
                else if (!KMaps[ID].Layers.ContainsKey(Layers[ID]))
                    Crash($"Kthura map {ID} does not have a layer named \"{Layers[ID]}\"");
                else
                    Crash(Ramp);
            }           
        }

        public void Kill(int ID,string Tag) {
            try {
                KthuraObject Victim=null;
                foreach (KthuraObject Obj in KMaps[ID].Layers[Layers[ID]].Objects) if (Obj.Tag == Tag) Victim = Obj;
                if (Victim == null) return;
                KMaps[ID].Layers[Layers[ID]].Objects.Remove(Victim);
                if (AutoRemap[ID]) TotalRemap(ID);
            } catch (Exception Disasteriffic) {
                SBubble.MyError($"Kthura.Kill({ID},{Tag}):", Disasteriffic.Message, $"Layer: {Layers[ID]}\n\n{SBubble.TraceLua(statename)}");
            }
        }

        public bool GetAutoRemap(int ID) => AutoRemap[ID];

        public string CountObjects(int ID) {
            var ret = new StringBuilder("return {");
            var cnt = new SortedDictionary<string, int>();
            cnt ["zzz Grand Total"] = 0;
            foreach(KthuraObject O in KMaps[ID].Layers[Layers[ID]].Objects) {
                if (!cnt.ContainsKey(O.kind)) cnt[O.kind] = 0;
                cnt["zzz Grand Total"]++;
                cnt[O.kind]++;
            }
            var comma = false;
            foreach(string K in cnt.Keys) {
                if (comma) ret.Append(", "); comma = true;
                ret.Append($"['{K}'] = {cnt[K]}");
            }
            ret.Append("}");
            return ret.ToString();
        }

        public void SetScroll(int ID,string K,int value) {
            Assert(KScroll.ContainsKey(ID), $"Scroller does not have index #{ID}");
            switch (K.ToUpper()) {
                case "X": KScroll[ID].ScrollX = value; break;
                case "Y": KScroll[ID].ScrollY = value; break;
                default: Crash($"Invalid Scroll field: {K}"); break;
            }
        }

        public int GetScroll(int ID, string K) {
            Assert(KScroll.ContainsKey(ID), $"Scroller does not have index #{ID}");
            switch (K.ToUpper()) {
                case "X": return KScroll[ID].ScrollX;
                case "Y": return KScroll[ID].ScrollY;
                default: Crash($"Invalid Scroll field: {K}"); return 0;
            }
        }

        public void SetLayer(int ID,string K) {
            Assert(Layers.ContainsKey(ID), $"Layer dictionary does not have index #{ID}");
            System.Diagnostics.Debug.WriteLine($"Let's switch map {ID} to layer {K}!");
            Layers[ID] = K;
        }

        public string GetLayer(int ID) {
            Assert(Layers.ContainsKey(ID), $"Layer dictionary does not have index #{ID}");
            return Layers[ID];
        }


        public void Draw(int ID,int x, int y) {
            try {
                var Map = KMaps[ID];
                var Scroll = KScroll[ID];
                var Lay = Layers[ID];
                KthuraDraw.DrawMap(Map, Lay, Scroll.ScrollX, Scroll.ScrollY,x,y);
            } catch (Exception Moron) {
                Crash(Moron);
            }
        }

        public void SetData(int ID,string key, string value) {
            try {
                if (!KMaps.ContainsKey(ID)) throw new Exception($"Map #{ID} does not exist!");
                KMaps[ID].MetaData[key] = value;
            } catch (Exception Boo) { Crash(Boo); }
        }

        public string GetData(int ID, string key) {
            try {
                if (!KMaps.ContainsKey(ID)) throw new Exception($"Map #{ID} does not exist!");
                if (!KMaps[ID].MetaData.ContainsKey(key)) throw new Exception($"Meta Data in Map #{ID} does not contain a field named '{key}'");
                return KMaps[ID].MetaData[key];
            } catch (Exception Boo) { Crash(Boo); return "ERROR!"; }
        }

        public string GetDataKeys(int ID) {
            try {
                var ret = new StringBuilder("return {");
                var comma = false;
                if (!KMaps.ContainsKey(ID)) throw new Exception($"Map #{ID} does not exist!");
                foreach (string k in KMaps[ID].MetaData.Keys) {
                    if (comma) ret.Append(","); comma = true;
                    ret.Append($"['{k}'] = \"{KMaps[ID].MetaData[k]}\"");
                }
                ret.Append("}");
                return ret.ToString();
            } catch (Exception HetGaatMis) {
                Crash(HetGaatMis);
                return "ERROR";
            }
        }

        public int Create() {
            var KMap = Kthura.Create();
            KMap.CreateLayer("__BASE");            
            return Assign(KMap);
        }

        public int Load(string name) {
            try {
                var mappath = $"Maps/Kthura/{name.Trim()}/";
                BubConsole.WriteLine($"Loading Kthura map {mappath}", 180, 0, 255);
                if (!(SBubble.JCR.Exists($"{mappath}Objects") && SBubble.JCR.Exists($"{mappath}Data"))) {
                    SBubble.MyError("Kthura LoadMap Error", $"Error finding the two required files for map {name}", $"Found({mappath}Objects) => {SBubble.JCR.Exists($"{mappath}Objects")}\nFound({mappath}Data) => {SBubble.JCR.Exists($"{mappath}Data")}");
                    return 0;
                }
                var KMap = Kthura.Load(SBubble.JCR,mappath, SBubble.JCR);
                BubConsole.WriteLine($"Kthura map {mappath} loaded", 180, 0, 255);
                return Assign(KMap);
            } catch (Exception Ellende) {
                if (JCR6.JERROR != "")
                    Crash(JCR6.JERROR);
                else
                    Crash(Ellende);
                BubConsole.CSay(Ellende.StackTrace);
                return 0;
            }
        }
		
		public void Destroy(int id) {
			KMaps.Remove(id);
		}



        #region Link to Bubble
        void Crash(string msg) => SBubble.MyError("Kthura Error", msg, SBubble.TraceLua(statename));
        void Crash(Exception ex) => Crash(ex.Message);
        void Assert(bool cond,string err) { if (!cond) Crash(err); }
        void Assert(int cond, string err) => Assert(cond != 0, err);
        void Assert(string cond, string err) => Assert(cond.Length, err);
        void Assert(object cond, string err) => Assert(cond != null, err);

        

        public string statename { get; private set; }
        private KthuraBubble() { }
        static public void Init(string astatename) {
            var Kth = new KthuraBubble();
            var S = SBubble.State(astatename).state;
            S["BKTHURA"] = Kth;
            Kth.statename = astatename;
            var bt = QuickStream.OpenEmbedded("KthuraBubble.nil");
            var l = bt.ReadString((int)bt.Size);
            bt.Close();            
            SBubble.DoNIL(astatename, l);
           
        }
        #endregion
    }

}
