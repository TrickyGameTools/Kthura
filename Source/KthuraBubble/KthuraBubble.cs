// Lic:
// Kthura for Bubble
// Links Kthura to Bubble Engines (initiall meant for NALA)
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
// Version: 19.08.27
// EndLic



#undef DijkstraPathDebug

using NSKthura;
using Bubble;
using System;
using System.Text;
using System.Diagnostics;
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

        string LuaTrace => SBubble.TraceLua(statename);
       
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
        
        public string GetLayers(int id) {
            try {
                var ret = new StringBuilder("local ret = {}\n");
                foreach (string k in KMaps[id].Layers.Keys) ret.Append($"ret[#ret+1] = \"{k}\"\n");
                ret.Append("\n\nreturn ret");
                return $"{ret}";
            } catch (Exception e) {
                SBubble.MyError($"Kthura.GetLayers({id}):", e.Message, LuaTrace);
                return "return {}";
            }
        }

        public string GetTags(int id,string layer) {
            var ret = new StringBuilder("local ret = {}");
            try {
                foreach (string k in KMaps[id].Layers[layer].Tags) ret.Append($"ret[#ret+1] = \"{k}\"\n");
                ret.Append("\n\nreturn ret");
            } catch (Exception e) {
                SBubble.MyError($"Kthura.GetTags({id},\"{layer}\"):", e.Message, LuaTrace);
                return "return {}";
            }
            return ret.ToString();
        }
        
        public void WalkToCoords(int ID, string ActorTag,int x, int y,bool real) {
            try {
                var M = KMaps[ID];
                var L = M.Layers[Layers[ID]];
                var O = L.FromTag(ActorTag);
                if (O.kind != "Actor") throw new Exception($"Object \"{ActorTag}\" is a(n) {O.kind} and not an actor!");
                var A = (KthuraActor)O;
                A.WalkTo(x, y, real);
                if (A.FoundPath!=null && A.FoundPath.Success) {
                    BubConsole.WriteLine($"Request to walk to ({x},{y}) was succesful!", 0, 255, 0);
#if DijkstraPathDebug
                    var P = new StringBuilder("Walk: ");
                    foreach (TrickyUnits.Dijkstra.Node N in A.FoundPath.Nodes) P.Append($"({N.x},{N.y}); ");
                    BubConsole.WriteLine(P.ToString(), 255, 180, 0);
#endif
                } else
                    BubConsole.WriteLine($"Request to walk to ({x},{y}) has failed!", 255, 0, 0);
            } catch (Exception Klotezooi) {
                Crash($"<Map #{ID}>.<KthuraActor.{ActorTag}>.WalkTo({x},{y},{real}):", Klotezooi);
            }
        }

        public void WalkToSpot(int ID, string ActorTag, string Spot) {
            try {
                var M = KMaps[ID];
                var L = M.Layers[Layers[ID]];
                var O = L.FromTag(ActorTag);
                if (O.kind != "Actor") throw new Exception($"Object \"{ActorTag}\" is a(n) {O.kind} and not an actor!");
                var A = (KthuraActor)O;
                A.WalkTo(Spot);
                if (A.FoundPath.Success)
                    BubConsole.WriteLine($"Request to walk to spot \"{Spot}\" was succesful!", 0, 255, 0);
                else
                    BubConsole.WriteLine($"Request to walk to \"{Spot}\" has failed!", 255, 0, 0);
                    BubConsole.WriteLine($"Request to walk to \"{Spot}\" has failed!", 255, 0, 0);
            } catch (Exception Klotezooi) {
                Crash($"<Map #{ID}>.<KthuraActor.{ActorTag}>.WalkTo(\"{Spot}\"):", Klotezooi);
            }
        }

        public bool Walking(int id, string ActorTag) {
            try {
                var M = KMaps[ID];
                var L = M.Layers[Layers[ID]];
                var O = L.FromTag(ActorTag);
                if (O.kind != "Actor") throw new Exception($"Object \"{ActorTag}\" is a(n) {O.kind} and not an actor!");
                var A = (KthuraActor)O;
                return A.Walking;
            } catch (Exception Verschrikkelijk) {
                Crash($"<Map #{ID}>.<KthuraActor.{ActorTag}>.Walking:", Verschrikkelijk);
                return false;
            }
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

        public bool ActorExists(int ID,string ActTag) {
            try {
                var L = KMaps[ID].Layers[Layers[ID]];
                var E = L.HasTag(ActTag);
                return E && L.FromTag(ActTag).kind == "Actor";                
            } catch (Exception Tragedie) {
                SBubble.MyError($"Checking Actor exists on map #{ID}, tag {ActTag}", Tragedie.Message, SBubble.TraceLua(statename));
                return false;
            }
        }

        public void Spawn(int ID,string acttag,string exitpoint) {
            try {
                var L = KMaps[ID].Layers[Layers[ID]];
                if (L.HasTag(acttag)) Kill(ID, acttag);
                var A = KthuraActor.Spawn(L, exitpoint);
                if (A == null) throw new Exception($"Cannot spawn actor on non-existent spot {exitpoint}");
                A.Tag = acttag;
            } catch (Exception DonaldTrump) {
                SBubble.MyError($"<MAP #{ID}>.Actor.{acttag}.Spawn(\"{exitpoint}\"):", DonaldTrump.Message, LuaTrace);
            }
        }

        public void SpawnCoords(int ID ,string acttag,int x,int y) {
            try {
                var L = KMaps[ID].Layers[Layers[ID]];
                if (L.HasTag(acttag)) Kill(ID, acttag);
                var A = KthuraActor.Spawn(L, x, y);
                if (A == null) throw new Exception($"Cannot spawn actor on coordinate ({x},{y})");
                A.Tag = acttag;
            } catch (Exception DonaldTrump) {
                SBubble.MyError($"<MAP #{ID}>.Actor.{acttag}.Spawn({x},{y}):", DonaldTrump.Message, LuaTrace);
            }

}

        void GetCoords(int ID,string tag,ref int x, ref int y) {
            try {
                var L = KMaps[ID].Layers[Layers[ID]];
                if (!L.HasTag(tag)) throw new Exception($"Object not found in layer: {Layers[ID]}!");
                var O = L.FromTag(tag);
                x = O.x;
                y = O.y;
            } catch (Exception Verschrikkelijk) {
                SBubble.MyError($"Kthura Object Coordinate Retriever (Map: #{ID}, Tag:{tag}):", Verschrikkelijk.Message, LuaTrace);
            }
        }

        public int GetX(int ID,string tag) {
            int x = 0;
            int y = 0;
            GetCoords(ID, tag, ref x, ref y);
            return x;
        }

        public int GetY(int ID, string tag) {
            int x = 0;
            int y = 0;
            GetCoords(ID, tag, ref x, ref y);
            return y;
        }

        public string GetObjTex(int ID,string tag) {
            try {
                var L = KMaps[ID].Layers[Layers[ID]];
                return L.FromTag(tag).Texture;
            } catch(Exception Calamity) {
                SBubble.MyError($"Getting texture of object {ID}:{tag}", Calamity.Message, "");
                return "ERROR";
            }
        }

        public void SetObjTex(int ID, string tag, string value) {
            try {
                var L = KMaps[ID].Layers[Layers[ID]];
                L.FromTag(tag).Texture=value;
            } catch (Exception Calamity) {
                SBubble.MyError($"Setting texture of object {ID}:{tag}", Calamity.Message, LuaTrace);                
            }
        }
        
        public string GetActorWind(int ID,string tag) {
            try {
                var L = KMaps[ID].Layers[Layers[ID]];
                var O = L.FromTag(tag);
                if (O.kind != "Actor") throw new Exception("Requested object is not an actor");
                var A = (KthuraActor)O;               
                return A.Wind;
            } catch (Exception Merde) {
                SBubble.MyError($"Getting wind of actor {ID}:{tag}", Merde.Message, LuaTrace);
                return "Merde!";
            }
        }

        public void SetActorWind(int ID, string tag,string value) {
            try {
                var L = KMaps[ID].Layers[Layers[ID]];
                var O = L.FromTag(tag);
                if (O.kind != "Actor") throw new Exception("Requested object is not an actor");
                var A = (KthuraActor)O;
                A.Wind=value;
            } catch (Exception Merde) {
                SBubble.MyError($"Setting wind of actor {ID}:{tag}", Merde.Message, LuaTrace);
            }
        }



        public void Draw(int ID,int x, int y) {
            try {
                var Map = KMaps[ID];
                var Scroll = KScroll[ID];
                var Lay = Layers[ID];
#if DijkstraPathDebug
                var L = Map.Layers[Lay];
                foreach (KthuraObject O in L.Objects) {
                    if (O.kind == "Actor") {
                        var A = (KthuraActor)O;
                        if (A.Walking) {
                            BubConsole.CSay($"Actor: {A.Tag}; Walking {A.Walking}; Moving: {A.Moving}; Path-Index: {A.PathIndex}; Length: {A.PathLength}");
                            BubConsole.CSay($"Current position: ({A.x},{A.y})");
                            BubConsole.CSay($"Moving to: ({A.MoveX},{A.MoveY})");
                            BubConsole.CSay($"Walkingto: ({A.WalkingToX},{A.WalkingToY})");
                        }
                    }
                }
#endif
                KthuraDraw.DrawMap(Map, Lay, Scroll.ScrollX, Scroll.ScrollY,x,y);
            } catch (Exception Moron) {
                Crash($"Draw(Res#{ID},{x},{y}):",Moron);
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
            System.Diagnostics.Debug.WriteLine($"Destroying Kthura Map #{id}");
			KMaps.Remove(id);
		}

        public void SetScrollX(int id,int svalue) {
            try {
                var M = KMaps[id];
                //var L = M.Layers[Layers[id]];
                KScroll[id].ScrollX = svalue;
            } catch (Exception Stront) {
                Crash($"Setting scroll X in resource {id} to {svalue}", Stront.Message);
            }
        }

        public void SetScrollY(int id, int svalue) {
            try {
                var M = KMaps[id];
                //var L = M.Layers[Layers[id]];
                KScroll[id].ScrollY = svalue;
            } catch (Exception Stront) {
                Crash($"Setting scroll Y in resource {id} to {svalue}", Stront.Message);
            }
        }

        public int GetScrollX(int id) {
            try {
                var M = KMaps[id];
                //var L = M.Layers[Layers[id]];
                return KScroll[id].ScrollX;
            } catch (Exception Stront) {
                Crash($"Getting scroll X in resource {id}", Stront.Message);
                return 0;
            }
        }

        public int GetScrollY(int id) {
            try {
                var M = KMaps[id];
                //var L = M.Layers[Layers[id]];
                return KScroll[id].ScrollY;
            } catch (Exception Stront) {
                Crash($"Getting scroll Y in resource {id}", Stront.Message);
                return 0;
            }
        }

        public int ObjInt(int id, string Lay,string Tag,string stat) {
            try {
                var M = KMaps[id]; 
                var L = M.Layers[Lay];
                var T = L.FromTag(Tag);
                switch (stat.ToUpper()) {
                    case "X": return T.x;
                    case "Y": return T.y;
                    case "W": case "WIDTH": return T.w;
                    case "H": case "HEIGHT": return T.h;
                    case "DOMINANCE": return T.Dominance;
                    case "R": case "RED": return T.R;
                    case "G": case "GREEN": return T.G;
                    case "B": case "BLUE": return T.B;
                    case "OBJECTWIDTH": return KthuraDraw.DrawDriver.ObjectWidth(T);
                    case "OBJECTHEIGHT": return KthuraDraw.DrawDriver.ObjectHeight(T);
                    default:
                        throw new Exception($"There is no integer field named: {stat}");
                }
            } catch (Exception shit) {
                SBubble.MyError($"Kthura.ObjNum({id},\"{Lay}\",\"{stat}\"):", shit.Message, LuaTrace);
                return 0;
            }
        }


        public string ObjString(int id, string Lay, string Tag, string stat) {
            try {
                var M = KMaps[id];
                var L = M.Layers[Lay];
                var T = L.FromTag(Tag);
                switch (stat.ToUpper()) {
                    case "KIND": return T.kind;
                    case "TEXTURE": return T.Texture;
                    default:
                        throw new Exception($"There is no string field named: {stat}");
                }
            } catch (Exception shit) {
                SBubble.MyError($"Kthura.ObjNum({id},\"{Lay}\",\"{stat}\"):", shit.Message, LuaTrace);
                return "ERROR";
            }
        }




        #region Link to Bubble
        void Crash(string Head, string msg) => SBubble.MyError(Head, msg, LuaTrace);
#if DEBUG
        void Crash(string head, Exception ex) => SBubble.MyError(head, ex.Message,$"{LuaTrace}\n\nC# Trace:\n{ex.StackTrace}");
#else
        void Crash(string head, Exception ex) => Crash(head, ex.Message);
#endif        
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
            SBubble.DoNIL(astatename, l,"Kthura API Link Script");
           
        }
#endregion
    }

}




