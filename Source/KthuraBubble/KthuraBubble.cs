// Lic:
// Kthura for Bubble
// Links Kthura to Bubble Engines (initiall meant for NALA)
// 
// 
// 
// (c) Jeroen P. Broks, 2019, 2020
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
// Version: 20.07.29
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
		static Dictionary<int,Kthura> KMaps = new Dictionary<int,Kthura>();
        static Dictionary<int, BubKthScroll> KScroll = new Dictionary<int, BubKthScroll>();
        static Dictionary<int, string> Layers = new Dictionary<int, string>();
        static TMap<int, bool> AutoRemap = new TMap<int, bool>();

        internal static Kthura GetMap(int ID) => KMaps[ID];

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

        public bool Blocked(int id, int x, int y, bool real) {
            try {
                var lname = Layers[id];
                var m = KMaps[id];
                var l = m.Layers[lname];
                if (real)
                    return l.Block(x, y);
                return l.PureBlock(x, y);
            } catch (Exception EpicFail) {
                SBubble.MyError($"Kthura.Blocked({id},{x},{y},{real}):", EpicFail.Message, LuaTrace);
            }
            return false;
        }

        public void DomRemap(int id) {
            try {
                var lname = Layers[id];
                var m = KMaps[id];
                var l = m.Layers[lname];
                l.RemapDominance();
            } catch (Exception EpicFail) {
                SBubble.MyError($"Kthura.DominanceRemap({id}):", EpicFail.Message, LuaTrace);
            }            
        }

        public void VisibleByZone(int id,string tag,bool value) {
            try {
                var lname = Layers[id];
                var m = KMaps[id];
                var l = m.Layers[lname];
                if (!l.HasTag(tag)) throw new Exception("I cannot hide or show by a non-existent zone");
                var zone = l.FromTag(tag);
                if (zone.kind != "TiledArea" && zone.kind != "Zone") throw new Exception($"Object '{tag}' is a {zone.kind} which is not valid for a visible by zone change!");
                foreach(KthuraObject o in l.Objects) {
                    if (o != zone && o.x >= zone.x && o.y >= zone.y && o.x <= zone.x + zone.w && o.y <= zone.y + zone.h) o.Visible = value;                
                }
            } catch (Exception EpicFail) {
                SBubble.MyError($"Kthura.VisilityByZone[{id},\"{tag}\"]={value}:", EpicFail.Message, LuaTrace);
            }
        }

        public void VisibleOnlyByZone(int id, string tag, bool inzone) {
            try {
                var lname = Layers[id];
                var m = KMaps[id];
                var l = m.Layers[lname];
                if (!l.HasTag(tag)) throw new Exception("I cannot hide or show by a non-existent zone");
                var zone = l.FromTag(tag);
                if (zone.kind != "TiledArea" && zone.kind != "Zone") throw new Exception($"Object '{tag}' is a {zone.kind} which is not valid for a visible by zone change!");
                foreach (KthuraObject o in l.Objects) {
                    if (o != zone) {
                        if (inzone)
                            o.Visible = o.x >= zone.x && o.y >= zone.y && o.x <= zone.x + zone.w && o.y <= zone.y + zone.h;
                        else
                            o.Visible = !(o.x >= zone.x && o.y >= zone.y && o.x <= zone.x + zone.w && o.y <= zone.y + zone.h);
                    }
                }
            } catch (Exception EpicFail) {
                SBubble.MyError($"Kthura.VisilityOnlyByZone[{id},\"{tag}\"]={inzone}:", EpicFail.Message, LuaTrace);
            }
        }

    

    public bool InObj(int id, string objtag,int x, int y) {
            bool ret = false;
            try {
                var lname = Layers[id];
                var m = KMaps[id];
                var l = m.Layers[lname];
                var o = l.FromTag(objtag);
                if (o.kind != "TiledArea" && o.kind != "Zone") throw new Exception($"Cannot do an incheck for object kind {o.kind}");
                ret = x >= o.x && x <= o.x + o.w && y >= o.y && y <= o.y + o.h;
            } catch (Exception EpicFail) {
                SBubble.MyError($"Kthura.InObj({id},\"{objtag}\",{x},{y}):", EpicFail.Message, LuaTrace);
            }
            return ret;
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

        public bool HasTag(int id,string layer, string tag) {
            try {
                return KMaps[id].Layers[layer].HasTag(tag);
            } catch (Exception FuckYou) {
                SBubble.MyError($"BubbleKthura.Map[{id}].Layer[\"{layer}\"].HasTag(\"{tag}\"): ", FuckYou.Message, LuaTrace);
                return false;
            }
        }

        public bool LHasTag(int id,string tag) {
            try {
                return KMaps[id].Layers[Layers[id]].HasTag(tag);
            } catch (Exception FuckYou) {
                SBubble.MyError($"BubbleKthura.Map[{id}].LHasTag(\"{tag}\"): ", FuckYou.Message, LuaTrace);
                return false;
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

        public bool ObjectInZone(int id,string Obj,string Zon) {
            bool ret = false;
            try {
                var lname = Layers[id];
                var m = KMaps[id];
                var l = m.Layers[lname];
                var o = l.FromTag(Obj);
                ret = o.IsInZone(Zon);                
            } catch ( Exception EpicFail) {
                SBubble.MyError($"Kthura.ObjectInZone({id},\"{Obj}\",\"{Zon}\"):", EpicFail.Message, LuaTrace);
            }
            return ret;
        }


        static private bool _WalkSuccess = false;
        public bool WalkSuccess {
            get { BubConsole.WriteLine($"WalkSuccess asked: {_WalkSuccess}"); return _WalkSuccess; }
            private set { _WalkSuccess = value; BubConsole.WriteLine($"Setting WalkSuccess to {value}"); }
        }
        
        public void WalkToCoords(int ID, string ActorTag,int x, int y,bool real) {
            try {
                if (!KMaps.ContainsKey(ID)) throw new Exception($"Map #{ID} does not exist!");
                var M = KMaps[ID];
                var L = M.Layers[Layers[ID]];
                var O = L.FromTag(ActorTag);
                if (O.kind != "Actor") throw new Exception($"Object \"{ActorTag}\" is a(n) {O.kind} and not an actor!");
                var A = (KthuraActor)O;
                A.WalkTo(x, y, real);
                if (A.FoundPath != null && A.FoundPath.Success) {
                    BubConsole.WriteLine($"Request to walk to ({x},{y}) was succesful!", 0, 255, 0);
                    WalkSuccess = true;
#if DijkstraPathDebug
                    var P = new StringBuilder("Walk: ");
                    foreach (TrickyUnits.Dijkstra.Node N in A.FoundPath.Nodes) P.Append($"({N.x},{N.y}); ");
                    BubConsole.WriteLine(P.ToString(), 255, 180, 0);
#endif
                } else {
                    BubConsole.WriteLine($"Request to walk to ({x},{y}) has failed!", 255, 0, 0);
                    WalkSuccess = false;
                }
            } catch (Exception Klotezooi) {
                Crash($"<Map #{ID}>.<KthuraActor.{ActorTag}>.WalkTo({x},{y},{real}):", Klotezooi);
                WalkSuccess = false;
            }
        }

        public void WalkToSpot(int ID, string ActorTag, string Spot) {
            try {
                if (!KMaps.ContainsKey(ID)) throw new Exception($"Map #{ID} does not exist!");
                var M = KMaps[ID];
                if (!M.Layers.ContainsKey(Layers[ID])) throw new Exception($"Map #{ID} does not have a layer named {Layers[ID]}");
                var L = M.Layers[Layers[ID]];
                if (!L.HasTag(ActorTag)) throw new Exception($"Map #{ID} does not have an object(actor) tagged {ActorTag} on Layer {Layers[ID]}");
                var O = L.FromTag(ActorTag);
                if (O.kind != "Actor") throw new Exception($"Object \"{ActorTag}\" is a(n) {O.kind} and not an actor!");
                var A = (KthuraActor)O;
                if (!L.HasTag(Spot)) throw new Exception($"Map #{ID} does not have an object(target) tagged {ActorTag} on Layer {Layers[ID]}");
                A.WalkTo(Spot);
                if (A.FoundPath!=null && A.FoundPath.Success) {
                    BubConsole.WriteLine($"Request to walk to spot \"{Spot}\" was succesful!", 0, 255, 0);
                    WalkSuccess = true;
                } else {
                    BubConsole.WriteLine($"Request to walk to \"{Spot}\" has failed!", 255, 0, 0);
                    //BubConsole.WriteLine($"Request to walk to \"{Spot}\" has failed!", 255, 0, 0);
                    WalkSuccess = false;
                }
            } catch (Exception Klotezooi) {
                Crash($"<Map #{ID}>.<KthuraActor.{ActorTag}>.WalkTo(\"{Spot}\"):", Klotezooi);
                _WalkSuccess = false;
            }
        }

        public void MoveToCoords(int ID, string ActorTag, int x, int y) {
            try {
                if (!KMaps.ContainsKey(ID)) throw new Exception($"Map #{ID} does not exist!");
                var M = KMaps[ID];
                var L = M.Layers[Layers[ID]];
                var O = L.FromTag(ActorTag);
                if (O.kind != "Actor") throw new Exception($"Object \"{ActorTag}\" is a(n) {O.kind} and not an actor!");
                var A = (KthuraActor)O;
                A.MoveTo(x, y);
                if (A.FoundPath != null && A.FoundPath.Success) {
                    BubConsole.WriteLine($"Request to Move to ({x},{y}) was succesful!", 0, 255, 0);
                    //MoveSuccess = true;
#if DijkstraPathDebug
                    var P = new StringBuilder("Move: ");
                    foreach (TrickyUnits.Dijkstra.Node N in A.FoundPath.Nodes) P.Append($"({N.x},{N.y}); ");
                    BubConsole.WriteLine(P.ToString(), 255, 180, 0);
#endif
                } else {
                    BubConsole.WriteLine($"Request to Move to ({x},{y}) has failed!", 255, 0, 0);
                    //MoveSuccess = false;
                }
            } catch (Exception Klotezooi) {
                Crash($"<Map #{ID}>.<KthuraActor.{ActorTag}>.MoveTo({x},{y}):", Klotezooi);
                //MoveSuccess = false;
            }
        }

        public void MoveToSpot(int ID, string ActorTag, string Spot) {
            try {
                if (!KMaps.ContainsKey(ID)) throw new Exception($"Map #{ID} does not exist!");
                var M = KMaps[ID];
                if (!M.Layers.ContainsKey(Layers[ID])) throw new Exception($"Map #{ID} does not have a layer named {Layers[ID]}");
                var L = M.Layers[Layers[ID]];
                if (!L.HasTag(ActorTag)) throw new Exception($"Map #{ID} does not have an object(actor) tagged {ActorTag} on Layer {Layers[ID]}");
                var O = L.FromTag(ActorTag);
                if (O.kind != "Actor") throw new Exception($"Object \"{ActorTag}\" is a(n) {O.kind} and not an actor!");
                var A = (KthuraActor)O;
                if (!L.HasTag(Spot)) throw new Exception($"Map #{ID} does not have an object(target) tagged {ActorTag} on Layer {Layers[ID]}");
                A.MoveTo(Spot);
                if (A.FoundPath != null && A.FoundPath.Success) {
                    BubConsole.WriteLine($"Request to Move to spot \"{Spot}\" was succesful!", 0, 255, 0);
                    //MoveSuccess = true;
                } else {
                    BubConsole.WriteLine($"Request to Move to \"{Spot}\" has failed!", 255, 0, 0);
                    //BubConsole.WriteLine($"Request to Move to \"{Spot}\" has failed!", 255, 0, 0);
                    //MoveSuccess = false;
                }
            } catch (Exception Klotezooi) {
                Crash($"<Map #{ID}>.<KthuraActor.{ActorTag}>.MoveTo(\"{Spot}\"):", Klotezooi);
                //_MoveSuccess = false;
            }
        }



        public bool Walking(int ID, string ActorTag) {
            try {
                var M = KMaps[ID];
                var L = M.Layers[Layers[ID]];
                var O = L.FromTag(ActorTag);
                if (O.kind != "Actor") throw new Exception($"Object \"{ActorTag}\" is a(n) {O.kind} and not an actor!");
                var A = (KthuraActor)O;
                return A.Walking;
            } catch (Exception Verschrikkelijk) {
                Crash($"<Map #{ID}>.<KthuraActor.{ActorTag}>.GetWalking:", Verschrikkelijk);
                return false;
            }
        }

        public void SetWalking(int ID, string ActorTag,bool v) {
            try {
                var M = KMaps[ID];
                var L = M.Layers[Layers[ID]];
                var O = L.FromTag(ActorTag);
                if (O.kind != "Actor") throw new Exception($"Object \"{ActorTag}\" is a(n) {O.kind} and not an actor!");
                var A = (KthuraActor)O;
                A.Walking = v;
            } catch (Exception Verschrikkelijk) {
                Crash($"<Map #{ID}>.<KthuraActor.{ActorTag}>.SetWalking:", Verschrikkelijk);                
            }
        }

        public bool Moving(int ID, string ActorTag) {
            try {
                var M = KMaps[ID];
                var L = M.Layers[Layers[ID]];
                var O = L.FromTag(ActorTag);
                if (O.kind != "Actor") throw new Exception($"Object \"{ActorTag}\" is a(n) {O.kind} and not an actor!");
                var A = (KthuraActor)O;
                return A.Moving;
            } catch (Exception Verschrikkelijk) {
                Crash($"<Map #{ID}>.<KthuraActor.{ActorTag}>.GetMoving:", Verschrikkelijk);
                return false;
            }
        }

        public void SetMoving(int ID, string ActorTag, bool v) {
            try {
                var M = KMaps[ID];
                var L = M.Layers[Layers[ID]];
                var O = L.FromTag(ActorTag);
                if (O.kind != "Actor") throw new Exception($"Object \"{ActorTag}\" is a(n) {O.kind} and not an actor!");
                var A = (KthuraActor)O;
                A.Moving = v;
            } catch (Exception Verschrikkelijk) {
                Crash($"<Map #{ID}>.<KthuraActor.{ActorTag}>.SetMoving:", Verschrikkelijk);
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
                if (!KMaps[ID].Layers.ContainsKey(Layers[ID])) throw new Exception($"I cannot contact spawn on non-existent layer {Layers[ID]}");
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

        public void SetX(int ID, string tag, int NP) {
            try {
                var L = KMaps[ID].Layers[Layers[ID]];
                if (!L.HasTag(tag)) throw new Exception($"Object not found in layer: {Layers[ID]}!");
                var O = L.FromTag(tag);
                O.x = NP;
            } catch (Exception Verschrikkelijk) {
                SBubble.MyError($"Kthura Object Coordinate Definer (Map: #{ID}, Tag:{tag}):", Verschrikkelijk.Message, LuaTrace);
            }
        }

        public void SetY(int ID, string tag, int NP) {
            try {
                var L = KMaps[ID].Layers[Layers[ID]];
                if (!L.HasTag(tag)) throw new Exception($"Object not found in layer: {Layers[ID]}!");
                var O = L.FromTag(tag);
                O.y = NP;
            } catch (Exception Verschrikkelijk) {
                SBubble.MyError($"Kthura Object Coordinate Definer (Map: #{ID}, Tag:{tag}):", Verschrikkelijk.Message, LuaTrace);
            }
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

        public void SetVisible(int ID,string tag,bool vis) {
            try {
                var L = KMaps[ID].Layers[Layers[ID]];
                var O = L.FromTag(tag);
                O.Visible = vis;
            } catch (Exception VisWijf) {
                SBubble.MyError($"Kthura.SetVisible({ID},\"{tag}\",{vis}):",VisWijf.Message, LuaTrace);
            }
        }
        public void ShowObject(int ID, string tag) => SetVisible(ID, tag, true);
        public void HideObject(int ID, string tag) => SetVisible(ID, tag, false);

        void VisByLabel(int ID, string label, bool value) {
            try {
                var L = KMaps[ID].Layers[Layers[ID]];
                if (!L.LabelMap.ContainsKey(label)) {
                    Console.Beep();
                    BubConsole.CSay($"WARNING! Label '{label}' not found!");
                    foreach (string k in L.LabelMap.Keys) BubConsole.CSay("= Has label: '{label}'");
                }
                foreach (KthuraObject O in L.LabelMap[label]) O.Visible = value;
            } catch (Exception Poepzooitje) {
#if DEBUG
                SBubble.MyError($"Kthura.VisByLabel({ID},\"{label}\",{value}):", Poepzooitje.Message, $"{LuaTrace}\n\nC# Trace{Poepzooitje.StackTrace}");
#else
                SBubble.MyError($"Kthura.VisByLabel({ID},\"{label}\",{value}):", Poepzooitje.Message, LuaTrace);
#endif
            }
        }

        public void HideByLabel(int ID, string label) => VisByLabel(ID, label, false);
        public void ShowByLabel(int ID, string label) => VisByLabel(ID, label, true);




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

        public bool ObjBool(int id, string Lay, string Tag, string stat) {
            try {
                var M = KMaps[id];
                var L = M.Layers[Lay];
                var T = L.FromTag(Tag);
                switch (stat.ToUpper()) {
                    case "VISIBLE":
                        return T.Visible;
                    case "IMPASSIBLE":
                        return T.Impassible;
                    case "FORCEPASSIBLE":
                        return T.ForcePassible;
                    default:
                        throw new Exception($"There is no boolean field named: {stat}");
                }
            } catch (Exception shit) {
                SBubble.MyError($"Kthura.ObjBool({id},\"{Lay}\",\"{Tag}\",\"{stat}\"):", shit.Message, LuaTrace);
                return false;
            }
        }

        public void SetObjBool(int id, string Lay, string Tag, string stat, bool value) {
            try {
                var M = KMaps[id];
                var L = M.Layers[Lay];
                var T = L.FromTag(Tag);
                switch (stat.ToUpper()) {
                    case "VISIBLE":
                        T.Visible = value;
                        break;
                    case "IMPASSIBLE":
                        T.Impassible = value;
                        break;
                    case "FORCEPASSIBLE":
                        T.ForcePassible = value;
                        break;
                    default:
                        throw new Exception($"There is no object integer field named: {stat}!");
                }
            } catch (Exception shit) {
                SBubble.MyError($"Kthura.ObjInt({id},\"{Lay}\",\"{Tag}\",\"{stat}\"):", shit.Message, LuaTrace);
                return;
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
                    case "SCALEX": return T.ScaleX;
                    case "SCALEY": return T.ScaleY;
                    case "INSERTX": return T.insertx;
                    case "INSERTY": return T.inserty;
                    case "ALPHA": return T.Alpha1000;
                    case "ROTATION": case "ROTATIONDEGREES": return T.RotationDegrees;
                    default:
                        throw new Exception($"There is no integer field named: {stat}");
                }
            } catch (Exception shit) {
                SBubble.MyError($"Kthura.ObjNum({id},\"{Lay}\",\"{Tag}\",\"{stat}\"):", shit.Message, LuaTrace);
                return 0;
            }
        }

        public void SetObjInt(int id, string Lay, string Tag, string stat,int value) {
            try {
                var M = KMaps[id];
                var L = M.Layers[Lay];
                var T = L.FromTag(Tag);
                switch (stat.ToUpper()) {
                    case "X": T.x=value; break;
                    case "Y": T.y=value; break;
                    case "W": case "WIDTH":  T.w=value; break;
                    case "H": case "HEIGHT": T.h=value; break;
                    case "DOMINANCE": T.Dominance=value; break;
                    case "R": case "RED": T.R=value; break;
                    case "G": case "GREEN": T.G=value; break;
                    case "B": case "BLUE": T.B=value; break;
                    case "OBJECTWIDTH": throw new Exception("Object Width cannot yet be changed!"); //return KthuraDraw.DrawDriver.ObjectWidth(T);
                    case "OBJECTHEIGHT": throw new Exception("Object Height cannot yet be changed"); // return KthuraDraw.DrawDriver.ObjectHeight(T);
                    case "SCALEX": T.ScaleX = value; break;
                    case "SCALEY": T.ScaleY = value; break;
                    case "INSERTX": T.insertx = value; break;
                    case "INSERTY": T.inserty = value; break;
                    case "ALPHA": T.Alpha1000 = value; break;
                    case "ROTATION": case "ROTATIONDEGREES":  T.RotationDegrees = value; break;
                    //case "ROTATIONRADIANS": T.RotationRadians = value; break;
                    default:
                        throw new Exception($"There is no object integer field named: {stat}!");
                }
            } catch (Exception shit) {
                SBubble.MyError($"Kthura.ObjInt({id},\"{Lay}\",\"{Tag}\",\"{stat}\"):", shit.Message, LuaTrace);
                return ;
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
                SBubble.MyError($"Kthura.ObjString({id},\"{Lay}\",\"{stat}\"):", shit.Message, LuaTrace);
                return "ERROR";
            }
        }  

        public void SetObjString(int id, string Lay, string Tag, string stat,string value) {
            try {
                var M = KMaps[id];
                var L = M.Layers[Lay];
                var T = L.FromTag(Tag);
                switch (stat.ToUpper()) {
                    case "KIND": T.kind=value; break;
                    case "TEXTURE":  T.Texture=value; break;
                    default:
                        throw new Exception($"There is no string field named: {stat}");
                }
            } catch (Exception shit) {
                SBubble.MyError($"Kthura.ObjString({id},\"{Lay}\",\"{stat}\"):", shit.Message, LuaTrace);
            }
        }

        public string ObjData(int id, string Lay, string Tag, string Fld) {
            try {
                var M = KMaps[id];
                var L = M.Layers[Lay];
                var T = L.FromTag(Tag);
                if (!T.MetaData.ContainsKey(Fld)) return "";
                return T.MetaData[Fld];
            } catch (Exception Stront) {
                SBubble.MyError($"Kthura.ObjData({id},\"{Lay}\",\"{Tag}\",\"{Fld}\"):", Stront.Message, LuaTrace);
                return "ERROR!";
            }
        }

        public void SetObjData(int id, string Lay, string Tag, string Fld,string Value) {
            try {
                var M = KMaps[id];
                var L = M.Layers[Lay];
                var T = L.FromTag(Tag);
                T.MetaData[Fld] = Value;
            } catch (Exception Stront) {
                SBubble.MyError($"Kthura.SetObjData({id},\"{Lay}\",\"{Tag}\",\"{Fld}\",\"{Value}\"):", Stront.Message, LuaTrace);
            }

        }

        public string Tags(int id,string Lay) {
                var ret = new StringBuilder();
            try {
                var M = KMaps[id];
                var L = M.Layers[Lay];
                L.RemapTags();
                var T = L.Tags;
                foreach (string t in T) ret.Append($"{L.FromTag(t).kind} {t}\n");
            } catch (Exception mislukt) {
                SBubble.MyError($"Tags({id},\"{Lay}\"):", mislukt.Message, LuaTrace);
            }
            return ret.ToString().Trim();
        }

        public void CreateTiledArea(int id,string texture, int x, int y, int w,int h, string tag) {
            try {
                var M = KMaps[id];
                var L = M.Layers[Layers[id]];
                var O = new KthuraObject("TiledArea", L);
                O.x = x;
                O.y = y;
                O.w = w;
                O.h = h;
                O.Tag = tag;
                O.Texture = texture;
                O.Visible = true;
                O.Alpha255 = 255;
                O.R = 255;
                O.G = 255;
                O.B = 255;
                L.RemapTags();
            } catch (Exception Afgang) {
                SBubble.MyError($"CreateTiledArea({id},\"{texture}\",{x},{y},{w},{h},\"{tag}\"):", Afgang.Message, LuaTrace);
            }
        }

        public void CreateObstacle(int id, string texture, int x, int y,string tag) {
            try {
                var M = KMaps[id];
                var L = M.Layers[Layers[id]];
                var O = new KthuraObject("Obstacle", L);
                O.x = x;
                O.y = y;
                O.Tag = tag;
                O.Texture = texture;
                O.Visible = true;
                O.Alpha255 = 255;
                O.R = 255;
                O.G = 255;
                O.B = 255;
                L.RemapTags();
            } catch (Exception Afgang) {
                SBubble.MyError($"CreateObstacle({id},\"{texture}\",{x},{y},\"{tag}\"):", Afgang.Message, LuaTrace);
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