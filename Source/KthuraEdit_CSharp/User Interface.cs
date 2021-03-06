// Lic:
// Kthura for C#
// User Interface (like the name of the file implies) :P
// 
// 
// 
// (c) Jeroen P. Broks, 2019, 2020, 2021
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
// Version: 21.03.24
// EndLic





using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Text;
using NSKthura;
using KthuraEdit.Stages;
using System.Diagnostics;
using Microsoft.Xna.Framework.Input;
using TrickyUnits;

namespace KthuraEdit {

    delegate void UISchedule(object data);

    static class UI {

        #region PullDownMenus
        static List<PullDownHeader> PullDownMenus;
        static Dictionary<Keys, int> PDKeyToEvent = new Dictionary<Keys, int>();
        static public bool PDOpen => PDOpenMenu != null; //{ get; private set; } = false;
        static PullDownHeader PDOpenMenu = null;
        static PullDownItem PDOpenItem = null;

        class PullDownItem {
            public string CaptString;
            public TQMGText CaptText;
            public TQMGText QKeyText;
            public int EventCode;
            public Keys QKey;
            public PullDownHeader Parent;
            public PullDownItem(string caption, int evCode, Keys QuickKey) {
                if (font20 == null) Core.Crash("Hey! No font!");
                CaptString = caption;
                CaptText = font20.Text(caption);
                EventCode = evCode;
                QKey = QuickKey;
                PDKeyToEvent[QuickKey] = evCode;
                QKeyText = font20.Text($"Ctrl-{QuickKey}");
                Debug.WriteLine($"  = Created Item: {caption}");
            }
        }
        class PullDownHeader {
            public string CaptString;
            public TQMGText CaptText;
            public List<PullDownItem> Items;
            public int mtxwidth { get; private set; } = 0;
            public int Width => mtxwidth + 150;
            public PullDownHeader(string Caption, params PullDownItem[] aItems) {
                Debug.WriteLine($"- Created Menu: {Caption}");
                CaptString = Caption;
                CaptText = font20.Text(Caption);
                Items = new List<PullDownItem>();
                foreach (PullDownItem item in aItems) {
                    item.Parent = this;
                    Items.Add(item);
                    if (item.CaptText.Width > mtxwidth) mtxwidth = item.CaptText.Width;
                }
            }
        }
        static List<PullDownHeader> InitPullDownMenus() {
            var ret = new List<PullDownHeader>();
            ret.Add(new PullDownHeader("General",
                new PullDownItem("Save", 1001, Keys.S),
                new PullDownItem("Meta Data",1002, Keys.M),
                new PullDownItem("Quit", 9999, Keys.Q)
                ));
            ret.Add(new PullDownHeader("Grid",
                new PullDownItem("Show Grid Blocks", 2001, Keys.D),
                new PullDownItem("Grid Mode", 2002, Keys.G)
                ));
            ret.Add(new PullDownHeader("Layers",
                new PullDownItem("New Layer", 4001, Keys.N),
                new PullDownItem("Remove Layer", 4002, Keys.NumPad0),
                new PullDownItem("Rename Layer", 4003, Keys.NumPad5)
                ));
            ret.Add(new PullDownHeader("Debug",
                new PullDownItem("Show Debug Log", 3001, Keys.F1),
                new PullDownItem("Show Blockmap", 3002, Keys.B),
                new PullDownItem("Count Objects", 3003, Keys.T),
                new PullDownItem("Scan and remove \"Rotten\" objects", 3004, Keys.F2),
                new PullDownItem("List Object Tags", 3005, Keys.Z),
                new PullDownItem("Go To Screen Postion", 3006, Keys.F3),
                new PullDownItem("Fix UnderOrigin",3007,Keys.U),
                new PullDownItem("Close to Origin",3008,Keys.Insert)
                ));

            return ret;
        }

        static void DrawPullDown() {
            if (PullDownMenus == null) PullDownMenus = InitPullDownMenus();
            TQMG.Color(255, 255, 255);
            TQMG.SimpleTile(back, 0, 0, ScrWidth, 25);

            var x = 20;
            foreach (PullDownHeader h in PullDownMenus) {
                TQMG.Color(0, 255, 255);
                if (Core.ms.Y < PDnH && Core.MsHit(1, true) && Core.ms.X > x - 5 && Core.ms.X < x + h.CaptText.Width + 5) PDOpenMenu = h;
                var thisisit = h == PDOpenMenu;
                if (thisisit) {
                    TQMG.DrawRectangle(x - 5, 0, h.CaptText.Width + 10, PDnH);
                    TQMG.Color(0, 0, 0);
                }
                h.CaptText.Draw(x, 3);
                if (thisisit) {
                    TQMG.Color(18, 0, 25);
                    TQMG.DrawRectangle(x - 5, PDnH + 1, h.Width + 5, h.Items.Count * 25);
                    TQMG.Color(0, 255, 255);
                    TQMG.DrawLineRect(x - 5, PDnH + 1, h.Width + 5, h.Items.Count * 25);
                    var y = PDnH + 3;
                    foreach (PullDownItem i in h.Items) {
                        TQMG.Color(0, 255, 255);
                        if (Core.ms.X > x && Core.ms.X < x + h.Width && Core.ms.Y > y && Core.ms.Y < y + 24) {
                            TQMG.DrawRectangle(x - 2, y, h.Width, 24);
                            TQMG.Color(0, 0, 0);
                            PDOpenItem = i;
                            if (Core.MsHit(1, true)) {
                                PDEvent = i.EventCode;
                                Core.DontMouse = true;
                                PDOpenItem = null;
                                PDOpenMenu = null;
                            }
                        }
                        i.CaptText.Draw(x, y);
                        i.QKeyText.Draw(x + h.Width - 2, y, TQMG_TextAlign.Right);
                        y += 24;
                    }
                    if (Core.ms.Y > PDnH && (Core.ms.Y > y || Core.ms.X < x - 10 || Core.ms.X > x + h.Width + 10) && Core.MsHit(1, true)) { PDOpenMenu = null; Core.DontMouse = true; }
                    if (TQMGKey.Hit(Keys.Escape)) PDOpenMenu = null;
                }
                x += h.CaptText.Width + 10;
            }
        }

        static int PDEvent = 0;
        static void PullDownQuickKeys() {
            //Debug.WriteLine($"Ctrl keys held {TQMGKey.Held(Keys.LeftControl)} || {TQMGKey.Held(Keys.RightControl)} = {TQMGKey.Held(Keys.LeftControl) || TQMGKey.Held(Keys.RightControl)} ");
            if (TQMGKey.Held(Keys.LeftControl) || TQMGKey.Held(Keys.RightControl)) {
                var k = TQMGKey.GetKey();
                //if (k != Keys.F19) Debug.WriteLine($"Is {k} a quick key?");
                if (PDKeyToEvent.ContainsKey(k)) PDEvent = PDKeyToEvent[k];
            }
        }

        #endregion

        #region Screen Size
        static public int ScrWidth => Core.MGCore.Window.ClientBounds.Width;
        static public int ScrHeight => Core.MGCore.Window.ClientBounds.Height;
        #endregion

        #region background picture
        static public TQMGImage back = TQMG.GetImage("BackPattern.png");
        #endregion
        #region fonts
        static public TQMGFont font12 = TQMG.GetFont("Fonts/nasa21.12.jfbf");
        static public TQMGFont font16 = TQMG.GetFont("Fonts/nasa21.16.jfbf");
        static public TQMGFont font20 = TQMG.GetFont("Fonts/nasa21.20.jfbf");
        static public TQMGFont font32 = TQMG.GetFont("Fonts/nasa21.32.jfbf");
        static public TQMGFont font64 = TQMG.GetFont("Fonts/nasa21.64.jfbf");
        #endregion
        #region Postions
        public const int LayW = 100;
        public const int PDnH = 25;
        static int ToolW => 300 + (back.Width % 100);
        static int ToolX => ScrWidth - ToolW;
        static public int ScrollX = 0;
        static public int ScrollY = 0;
        static int PosX => ScrollX + Core.ms.X - LayW;
        static int PosY => ScrollY + Core.ms.Y - PDnH;
        static int HoldX = 0, HoldY = 0, HoldEX = 0, HoldEY = 0;
        static bool HoldArea = false;
        static int gPosX {
            get {
                if (!GridMode) return PosX;
                return ((int)Math.Floor((decimal)PosX / Core.Map.Layers[selectedlayer].GridX) * Core.Map.Layers[selectedlayer].GridX) + (ScrollX % Core.Map.Layers[selectedlayer].GridX);
            }
        }
        static int gPosY {
            get {
                if (!GridMode) return PosY;
                return ((int)Math.Floor((decimal)PosY / Core.Map.Layers[selectedlayer].GridY) * Core.Map.Layers[selectedlayer].GridY) + (ScrollY % Core.Map.Layers[selectedlayer].GridY);
            }
        }
        static int ogPosX { get { if (!GridMode) return PosX; return gPosX + (MapLayer.GridX / 2); } }
        static int ogPosY { get { if (!GridMode) return PosY; return gPosY + (MapLayer.GridX); } }
        #endregion

        #region TexMemory
        // In order to ease things up, I've set this system up.
        // In my own experience in which I used Kthura, you use most textures 90% of the time with the same settings
        // But as you easily forget to set them all in order every time you call it, this system has been set up to make sure
        // This cannot go wrong that easily.
        static string TexMemorySettingsDir => ($"{Core.GlobalWorkSpace}/{Core.Project}/TexSettings").Replace("\\","/");
        static string TexMemorySettingsFile => $"{TexMemorySettingsDir}/{Core.MapFile}.GINI";
        static TGINI TexMemory;
        static void SealTexMemory() {
            var opf = ObjectParamFields[currentTBItem.Name];
            var opc = ObjectCheckBoxes[currentTBItem.Name];
            var Tex = opf["Texture"].value;
            Debug.WriteLine("Sealing TexMemory");
            TexMemory.D($"TEX[{Tex}].TAB", currentTBItem.Name);
            foreach(string key in opf.Keys) {
                var opfo = opf[key];
                var opfv = opf[key].value;
                if ((!opfo.AltijdNee) && key != "Texture") { // It makes no sense to collect data from a field that never has it due to always being disabled in this setting. It can only lead to conflicts!
                    TexMemory.D($"TEX[{Tex}].{currentTBItem.Name}.{key}", opfv);
                }
            }
            foreach (string key in opc.Keys) {
                var opco = opc[key];
                var opcv = opc[key].value;
                if (!opco.AltijdNee ) { // It makes no sense to collect data from a field that never has it due to always being disabled in this setting. It can only lead to conflicts!
                    TexMemory.D($"TEX[{Tex}].{currentTBItem.Name}.{key}", $"{opcv}");
                }
            }

        }

        static public void SetTexture(string Tex, bool calltab, bool calldata) {
            // Call tab (always comes first)
            var totab = currentTBItem.Name;
            if (calltab) {
                TexMemory.DefIfHave($"TEX[{Tex}].TAB", ref totab);
                foreach (TBItem i in TBItems) if (i.Name == totab) currentTBItem = i;
            }
            var opf = ObjectParamFields[currentTBItem.Name];
            var opc = ObjectCheckBoxes[currentTBItem.Name];
            // Set the texture (always comes in between)
            opf["Texture"].value = Tex;
            // Call data (always comes last)            
            foreach (string key in opf.Keys) {
                var opfo = opf[key];
                var opfv = opf[key].value;
                if ((!opfo.AltijdNee) && key != "Texture") { // It makes no sense to collect data from a field that never has it due to always being disabled in this setting. It can only lead to conflicts!
                    TexMemory.DefIfHave($"TEX[{Tex}].{currentTBItem.Name}.{key}", ref opfv);
                    opf[key].value = opfv;
                }
            }
            foreach (string key in opc.Keys) {
                var opco = opc[key];
                var opcv = $"{opc[key].value}";
                if (!opco.AltijdNee) { // It makes no sense to collect data from a field that never has it due to always being disabled in this setting. It can only lead to conflicts!
                    TexMemory.DefIfHave($"TEX[{Tex}].{currentTBItem.Name}.{key}", ref opcv);
                    opc[key].value = opcv.ToLower() == "true";
                }
            }
        }

        static public void SaveTexMemory() {            
            if (!Directory.Exists(TexMemorySettingsDir)) {
                DBG.Log($"Creating dir: {TexMemorySettingsDir}");
                Directory.CreateDirectory(TexMemorySettingsDir);
            }
            TexMemory.D("CAMX", ScrollX.ToString());
            TexMemory.D("CAMY", ScrollY.ToString());
            TexMemory.D("LAYR", selectedlayer);
            DBG.Log($"Saving: {TexMemorySettingsFile}");
            TexMemory.SaveSource(TexMemorySettingsFile);
        }
        #endregion

        #region Status
        static TQMGText BottomLine;
        static TQMGText TextGridMode;
        static public void DrawStatus() {
            TQMG.Color(255, 255, 255);
            TQMG.SimpleTile(back, 0, ScrHeight - 25, ScrWidth, 25);
            TQMG.Color(0, 255, 255);
            if (BottomLine == null)
                BottomLine = font20.Text($"{Core.Project}::{Core.MapFile}");
            BottomLine.Draw(5, ScrHeight - 24);
            if (Core.ms.Y > PDnH && Core.ms.X > LayW && Core.ms.Y < ScrHeight - 25 && Core.ms.X < ToolX) {
                if (HoldArea) {
                    font20.DrawText($"Marking for {currentTBItem.Name}: ({HoldX},{HoldY})-({HoldEX},{HoldEY})", ScrWidth - 5, ScrHeight - 25, TQMG_TextAlign.Right);
                } else {
                    font20.DrawText($"Scr({ScrollX},{ScrollY}); Mse({Core.ms.X},{Core.ms.Y}); Pos({PosX},{PosY})", ScrWidth - 5, ScrHeight - 24, TQMG_TextAlign.Right);
                }
            }
            if (GridMode) {
                if (TextGridMode == null) TextGridMode = font20.Text("GridMode");
                TextGridMode.Draw(ScrWidth / 2, ScrHeight - 24, TQMG_TextAlign.Center);
            }
        }
        #endregion

        #region Layers
        static public string selectedlayer { get; private set; } = "";
        static public NSKthura.KthuraLayer MapLayer { get => Core.Map.Layers[selectedlayer]; }
        static public void DrawLayerBox() {
            TQMG.Color(255, 255, 255);
            TQMG.SimpleTile(back, 0, 0, LayW, ScrHeight);
            var y = PDnH + 5;
            foreach(string layname in Core.Map.Layers.Keys) {
                TQMG.Color(0, 255, 255);
                if (selectedlayer == layname) {
                    TQMG.DrawRectangle(0, y, LayW, 16);
                    TQMG.Color(0, 0, 0);
                }
                font16.DrawMax(layname, 2, y, LayW - 4);
                if (Core.ms.X < LayW && Core.ms.Y > y && Core.ms.Y < y + 16 && Core.MsHit(1)) { selectedlayer = layname; M_SelectedObject = null; }
                if (selectedlayer == "") { selectedlayer = layname; M_SelectedObject = null; }
                y += 18;
            }
        }
        static public void LayerReset() => selectedlayer = "";
        static public void DeleteLayer(params string[] arg)  {
            if (Core.Map.Layers.Count<=1) {
                DBG.Log("ERROR: A Kthura map MUST have at least one layer, so I cannot remove the last one!");
                return;
            }
            Core.Map.Layers.Remove(arg[0]);
        }
        #endregion

        #region Just some public crap
        static public void BackFull() => TQMG.SimpleTile(back,0, 0, ScrWidth, ScrHeight);
        #endregion

        #region Toolbox
        delegate void TBWork();
        delegate bool TBEnabled(object data = null);
        delegate void AreaRelease(int x1, int y1, int x2, int y2);
        delegate void MapClick(int x, int y);

        class TBItem {
            internal static int initx { get; private set; } = ToolX;
            internal static int inity { get; private set; } = PDnH + 10;
            internal Dictionary<bool, TQMGImage> Button = new Dictionary<bool, TQMGImage>();
            readonly internal int X, Y;
            readonly public TBWork Work;
            readonly internal string Name;
            public bool area =>AR!=null;
            readonly public AreaRelease AR;
            public bool mapclick => MC != null;
            readonly public MapClick MC;
            

            internal TBItem(string fbutton,TBWork w,AreaRelease aAR,MapClick aMC) {
                DBG.Log($"  = Init button {fbutton}");
                Button[true] = TQMG.GetImage($"CTb_{fbutton}.png");
                Button[false] = TQMG.GetImage($"Tab_{fbutton}.png");
                if (initx + Button[true].Width > ScrWidth - 10) {
                    initx = ToolX;
                    inity += Button[false].Height+3;
                }
                X = initx;
                Y = inity;
                initx += Button[true].Width+3;
                Work = w;
                ObjectParamFields[fbutton] = new Dictionary<string, tbfields>();
                ObjectCheckBoxes[fbutton] = new Dictionary<string, tbcheckbox>();
                Name = fbutton;
                AR = aAR;
                MC = aMC;
            }
        }
        static TBItem currentTBItem;
        static List<TBItem> TBItems;
        static public Dictionary<bool, TQMGImage> CheckboxImage { get; private set; }  = new Dictionary<bool, TQMGImage>();
        static public bool InZoneTab => currentTBItem != null && currentTBItem.Name == "Zones";
        static public bool ModifyShowZone => currentTBItem!=null && currentTBItem.Name == "Modify" && ObjectCheckBoxes["Modify"]["ShowZones"].value ;

        static bool IkZegAltijdNee(object d = null) => false;
        static bool ModifyEnable(object d = null) {
            if (M_SelectedObject == null) return false;
            var kind = M_SelectedObject.kind;
            if (kind[0] == '$') kind = "Pivot";
            if (d.GetType().Equals(typeof(tbfields))) {
                var f = (tbfields)d;
                switch (f.Name) {
                    case "X":
                    case "Y":
                    case "Labels":
                    case "Dominance":
                    case "Tag":
                    case "cR":
                    case "cG":
                    case "cB":
                    case "R":
                    case "G":
                    case "B":
                        return true;
                    case "Width":
                    case "Height":
                    case "W":
                    case "H":
                        return kind == "TiledArea" || kind == "Zone" || kind == "StretchedArea";
                    case "InsX":
                    case "InsY":
                        return kind == "TiledArea";
                    case "Alpha":
                    case "AnimSpeed":
                    case "Frame":
                        return kind == "TiledArea" || kind == "Pic" || kind == "Obstacle" || kind == "StretchedArea";
                    case "RotDeg":
                    case "ScaleX":
                    case "ScaleY":
                        return kind == "Obstacle";
                    default:
                        Debug.WriteLine($"Warning! Unkown field name {f.Name} for kind {kind}");
                        break;

                }

            } else if (d.GetType().Equals(typeof(tbcheckbox))) {
                return kind != "Exit" && kind != "Pivot";
            } else {
                throw new Exception($"Unknown datafield to work with {d.GetType()}");
            }
            return false; // Should be unreachable!
        }

        class tbfields {
            readonly TBEnabled GetEnabled;
            public string dtype;
            public string value;
            public int intvalue => qstr.ToInt(value);
            public int x = 0, y = 0;
            public int w = 0, h = 0;
            readonly public string Name;
            bool _Enabled=true;
            //object EnabledData;
            public bool AltijdNee => GetEnabled == IkZegAltijdNee;
            public bool Enabled {
                get {
                    if (GetEnabled != null) return GetEnabled(this); else return _Enabled;
                }
                set {
                    if (GetEnabled == null) _Enabled = value; else DBG.Log("ERROR! Tried to changed the enabled state of an auto-enabled field");
                }
            }
            public tbfields(string aName, int sx,int sy, int sw, int sh, string otype, string defaultvalue, TBEnabled EnabledFunction = null)         {
                x = sx; y = sy; w = sw; h = sh;
                value = defaultvalue;
                GetEnabled = EnabledFunction;
                dtype = otype;
                Name = aName;
            }
        }

        struct tblabels {
            public int x, y;
            public string capt;
            public TQMGText capttext;
            public tblabels(int ax, int ay, string acapt) { x = ax; y = ay;capt = acapt;capttext = font20.Text(acapt); }
        }

        class tbcheckbox {
            readonly TBEnabled GetEnabled;
            bool _Enabled = true;
            //object EnabledData;
            public string Name;
            public bool AltijdNee => GetEnabled == IkZegAltijdNee;
            public bool Enabled {
                get {
                    if (GetEnabled != null) return GetEnabled(this); else return _Enabled;
                }
                set {
                    if (GetEnabled == null) _Enabled = value; else DBG.Log("ERROR! Tried to changed the enabled state of an auto-enabled field");
                }
            }
            public bool value = false;
            public int x, y;
            public void Toggle()  => this.value = !this.value;
            public tbcheckbox(int ax, int ay, TBEnabled fEnabled=null) {
                x = ax;
                y = ay;
                GetEnabled = fEnabled;
            }
        }
        static Dictionary<string, Dictionary<string, tbcheckbox>> ObjectCheckBoxes = new Dictionary<string, Dictionary<string, tbcheckbox>>();
        static List<tblabels> oplabs;
        static Dictionary<string, Dictionary<string, tbfields>> ObjectParamFields = new Dictionary<string, Dictionary<string, tbfields>>();
        static Dictionary<string, tbfields> tbcurfield = new Dictionary<string, tbfields>();
        static tbfields curfield {
            get {
                if (!tbcurfield.ContainsKey(currentTBItem.Name)) return null;
                return tbcurfield[currentTBItem.Name];
            }
            set {
                tbcurfield[currentTBItem.Name] = value;
            }
        }

        static void ObjectParameters() {
            TQMG.Color(255, 255, 255);
            foreach(tblabels label in oplabs) {
                label.capttext.Draw(label.x, label.y);
            }
            foreach (tbfields field in ObjectParamFields[currentTBItem.Name].Values) {
                if (!field.Enabled) {
                    TQMG.Color(25, 0, 0);
                    if (curfield == field) curfield = null;
                } else {
                    if (curfield == null && field.dtype!="string") curfield = field;
                    if (Core.MsHit(1) && Core.ms.X > field.x && Core.ms.X < field.x + field.w && Core.ms.Y > field.y && Core.ms.Y < field.y + field.h) {
                        if (field.dtype != "string")
                            curfield = field;
                        else {
                            switch (field.Name) {
                                case "Texture":
                                    TexSelector.ComeToMe();
                                    break;
                                case "Tag":
                                    QuestionList.ComeToMe("Enter tag",new string[]{"Tag"},CatchTag);
                                    break;
                                case "Labels":
                                    var mq = 9;
                                    var q = new string[mq]; for (int i = 0; i < mq; i++) q[i] = $"Label #{i+1}";                                    
                                    if (currentTBItem.Name=="Modify") {
                                        var lab = M_SelectedObject.Labels.Split(',');
                                        for (int i = 0; i < lab.Length; i++) {
                                            if (i >= mq) q[mq-1] += $",{lab[i]}"; else q[i] += $"={lab[i]}";
                                        }
                                        QuestionList.ComeToMe("Labels Editor", q, CatchLabels);
                                    }
                                    break;
                                case "NewData":
                                    QuestionList.ComeToMe("Please add a new data field", new string[] { "FieldName", "Value" }, delegate (object o) {
                                        var data = (Dictionary<string, string>)o;
                                        if (data["FieldName"].IndexOf('=')>=0) { DBG.Log("ERROR! Illegal character in field name!"); Console.Beep(3700, 1500); return; }
                                        if (M_SelectedObject.MetaData.ContainsKey(data["FieldName"])) { DBG.Log("ERROR! That field already exists/"); Console.Beep(3700, 1500); return; }
                                        M_SelectedObject.MetaData[data["FieldName"]] = data["Value"];
                                    });
                                    break;
                                case "ModData": {
                                        var fl = new List<string>();
                                        foreach(string k in M_SelectedObject.MetaData.Keys) {
                                            fl.Add($"{k}={M_SelectedObject.MetaData[k]}");
                                        }
                                        fl.Sort();
                                        QuestionList.ComeToMe("Please modify the data and hit Escape with done", fl.ToArray(), delegate (object o) {
                                            var data = (Dictionary<string, string>)o;
                                            foreach(string k in data.Keys) {
                                                M_SelectedObject.MetaData[k] = data[k];
                                            }
                                        });
                                    }
                                    break;
                                default:
                                    DBG.Log($"ERROR! I don't know what to do with field {field.Name}");
                                    break;
                            }
                        }
                    }
                    if (curfield == field)
                        TQMG.Color(0, 255, 255);
                    else
                        TQMG.Color(0, 25, 25);
                }
                TQMG.DrawRectangle(field.x, field.y, field.w, field.h);
                if (!field.Enabled)
                    TQMG.Color(255, 0, 0);
                else if (curfield == field)
                    TQMG.Color(0, 25, 25);
                else
                    TQMG.Color(0, 255, 255);
                font20.DrawMax(field.value, field.x + 2, field.y + 1, field.w - 4);
            }
            foreach(tbcheckbox chkbox in ObjectCheckBoxes[currentTBItem.Name].Values) {
                if (!chkbox.Enabled)
                    TQMG.Color(255, 0, 0);
                else {
                    TQMG.Color(0, 255, 255);
                    if (Core.MsHit(1) && Core.ms.X >= chkbox.x && Core.ms.X <= chkbox.x + 20 && Core.ms.Y >= chkbox.y && Core.ms.Y <= chkbox.y + 20) {
                        chkbox.Toggle();
                        if (currentTBItem.Name == "Modify") ModifyField(chkbox);
                    }
                }
                CheckboxImage[chkbox.value].Draw(chkbox.x, chkbox.y);
            }
        }
        static bool EnableIns(object o = null) => !ObjectCheckBoxes["TiledArea"]["AutoIns"].value;
        static bool EnHaveObj(object o = null) => M_SelectedObject != null;

        static void InitToolBox() {
            DBG.Log("- Setting up toolbox");
            // Load checkboxes
            try {
                CheckboxImage[false] = TQMG.GetImage("Check_Unchecked.png"); if (CheckboxImage[false] == null) Core.Crash($"Uncheck load fail: {UseJCR6.JCR6.JERROR}");
                CheckboxImage[true] = TQMG.GetImage("Check_Checked.png"); if (CheckboxImage[true] == null) Core.Crash($"Check load fail: {UseJCR6.JCR6.JERROR}");
            } catch (Exception e) {
                Core.Crash($"{e.Message}\n\n{UseJCR6.JCR6.JERROR}\n\nIs there something wrong with your installation?");
            }

            // Tabs
            TBItems = new List<TBItem>(new TBItem[] {
                new TBItem("TiledArea",ObjectParameters,AR_Tiled,null),
                new TBItem("StrechedArea",ObjectParameters,AR_Stretched,null),
                new TBItem("Obstacles",ObjectParameters,null,MC_Obstacle),
                new TBItem("Zones",ObjectParameters,AR_Zones,null),
                new TBItem("Other",Other,null,MC_CSpot),
                new TBItem("Modify",ObjectParameters,null,MC_Modify)
            });

            // Labels in Object paramters
            var y = TBItem.inity+60+21;
            var x = ToolX + 5;
            oplabs = new List<tblabels>(new tblabels[] {
                new tblabels(x,y-21,"Kind:"),
                new tblabels(x,y,"Texture:"),
                new tblabels(x,y+21,"Coords:"),
                new tblabels(x,y+42,"Insert:"),
                new tblabels(x,y+63,"Format:"),
                new tblabels(x,y+84,"Labels:"),
                new tblabels(x,y+105,"Dominance:"),
                new tblabels(x,y+126,"Alpha:"),
                new tblabels(x,y+147,"Impassible:"),
                new tblabels(x,y+168,"Force Passible:"),
                new tblabels(x,y+189,"Rotation (deg):"),
                new tblabels(x,y+210,"Color:"),
                new tblabels(x,y+231,"Anim Speed:"),
                new tblabels(x,y+252,"Frame:"),
                new tblabels(x,y+273,"Scale:"),
                new tblabels(x,y+294,"Visible:"),
                new tblabels(x,y+315,"Tag:")
            });
            foreach(TBItem i in TBItems) {
                if (i.Name != "Other") {
                    var ct = ObjectParamFields[i.Name];
                    var cb = ObjectCheckBoxes[i.Name];
                    if (i.Name != "Modify") {
                        var form = "click"; if (i.Name == "Obstacles") form = "N/A";
                        ct["Kind"] = new tbfields("Kind",x + 150, y-21, 150, 20, "string", i.Name, IkZegAltijdNee);
                        ct["X"] = new tbfields("X",x + 150, y + 21, 70, 20, "int", "click", IkZegAltijdNee);
                        ct["Y"] = new tbfields("Y",x + 230, y + 21, 70, 20, "int", "click", IkZegAltijdNee);
                        if (i.Name == "TiledArea") {
                            ct["InsX"] = new tbfields("Ins",x + 150, y + 42, 70, 20, "int", "0",EnableIns);
                            ct["InsY"] = new tbfields("Ins",x + 230, y + 42, 70, 20, "int", "0",EnableIns);
                            cb["AutoIns"] = new tbcheckbox(x + 110, y + 42);
                        } else {
                            ct["InsX"] = new tbfields("Ins",x + 150, y + 42, 70, 20, "int", "N/A", IkZegAltijdNee);
                            ct["InsY"] = new tbfields("Ins",x + 230, y + 42, 70, 20, "int", "N/A", IkZegAltijdNee);
                        }
                        ct["Width"] = new tbfields("W",x + 150, y + 63, 70, 20, "int", form, IkZegAltijdNee);
                        ct["Height"] = new tbfields("H",x + 230, y + 63, 70, 20, "int", form, IkZegAltijdNee);
                        ct["Labels"] = new tbfields("Lab",x + 150, y + 84, 150, 20, "string", "");
                        ct["Dominance"] = new tbfields("Dom",x + 150, y + 105, 150, 20, "int", "20");
                        if (i.Name != "Zones") {
                            ct["Texture"] = new tbfields("Texture",x + 150, y, 150, 20, "string", "");
                            ct["Alpha"] = new tbfields("",x + 150, y + 126, 150, 20, "int", "1000");
                            ct["cR"] = new tbfields("",x + 150, y + 210, 45, 20, "int", "255");
                            ct["cG"] = new tbfields("",x + 200, y + 210, 45, 20, "int", "255");
                            ct["cB"] = new tbfields("",x + 250, y + 210, 45, 20, "int", "255");
                            ct["AnimSpeed"] = new tbfields("",x + 150, y + 231, 150, 20, "int", "-1");
                            ct["Frame"] = new tbfields("Frame",x + 150, y + 252, 150, 20, "int", "0");
                        } else {
                            ct["Texture"] = new tbfields("Texture",x + 150, y, 150, 20, "string", "", IkZegAltijdNee);
                            ct["Alpha"] = new tbfields("Alf",x + 150, y + 126, 150, 20, "int", "N/A", IkZegAltijdNee);
                            ct["cR"] = new tbfields("R",x + 150, y + 210, 45, 20, "int", "Rnd", IkZegAltijdNee);
                            ct["cG"] = new tbfields("G",x + 200, y + 210, 45, 20, "int", "Rnd", IkZegAltijdNee);
                            ct["cB"] = new tbfields("B",x + 250, y + 210, 45, 20, "int", "Rnd", IkZegAltijdNee);
                            ct["AnimSpeed"] = new tbfields("",x + 150, y + 231, 150, 20, "int", "N/A", IkZegAltijdNee);
                            ct["Frame"] = new tbfields("F",x + 150, y + 252, 150, 20, "int", "0", IkZegAltijdNee);
                        }
                        if (i.Name == "Obstacles") {
                            ct["RotDeg"] = new tbfields("RD",x + 150, y + 189, 150, 20, "int", "0");
                            ct["ScaleX"] = new tbfields("SX",x + 150, y + 273, 70, 20, "int", "1000");
                            ct["ScaleY"] = new tbfields("SY",x + 230, y + 273, 70, 20, "int", "1000");
                        } else {
                            ct["RotDeg"] = new tbfields("RD",x + 150, y + 189, 150, 20, "int", "N/A", IkZegAltijdNee);
                            ct["ScaleX"] = new tbfields("SX",x + 150, y + 273, 70, 20, "int", "1000", IkZegAltijdNee);
                            ct["ScaleY"] = new tbfields("SY",x + 230, y + 273, 70, 20, "int", "1000", IkZegAltijdNee);
                        }
                        ct["Tag"] = new tbfields("Tag",x + 150, y + 315, 150, 20, "string", "Tag => modify",IkZegAltijdNee);
                        if (i.Name != "Zones" && i.Name != "Other")
                            cb["Visible"] = new tbcheckbox(x + 150, y + 294);
                        else 
                            cb["Visible"] = new tbcheckbox(x + 150, y + 294, IkZegAltijdNee);
                        cb["Visible"].value = true;                        
                        cb["Impassible"] = new tbcheckbox(x + 150, y + 147);
                        cb["ForcePassible"] = new tbcheckbox(x + 150, y + 168);
                    } else {
                        ct["Kind"] = new tbfields("",x + 150, y-21, 150, 20, "string", "", IkZegAltijdNee);
                        ct["Texture"] = new tbfields("",x + 150, y, 150, 20, "string", "", IkZegAltijdNee);
                        ct["X"] = new tbfields("X",x + 150, y + 21, 70, 20, "int", "", ModifyEnable);
                        ct["Y"] = new tbfields("Y",x + 230, y + 21, 70, 20, "int", "", ModifyEnable);
                        ct["InsX"] = new tbfields("InsX",x + 150, y + 42, 70, 20, "int", "", ModifyEnable);
                        ct["InsY"] = new tbfields("InsY",x + 230, y + 42, 70, 20, "int", "", ModifyEnable);
                        ct["Width"] = new tbfields("W",x + 150, y + 63, 70, 20, "int", "", ModifyEnable);
                        ct["Height"] = new tbfields("H",x + 230, y + 63, 70, 20, "int", "", ModifyEnable);
                        ct["Labels"] = new tbfields("Labels",x + 150, y + 84, 150, 20, "string", "", ModifyEnable);
                        ct["Dominance"] = new tbfields("Dominance",x + 150, y + 105, 150, 20, "int", "20", ModifyEnable);
                        ct["Alpha"] = new tbfields("Alpha",x + 150, y + 126, 150, 20, "int", "1000", ModifyEnable);
                        ct["RotDeg"] = new tbfields("RotDeg",x + 150, y + 189, 150, 20, "int", "", ModifyEnable);
                        ct["cR"] = new tbfields("R",x + 150, y + 210, 45, 20, "int", "255", ModifyEnable);
                        ct["cG"] = new tbfields("G",x + 200, y + 210, 45, 20, "int", "255", ModifyEnable);
                        ct["cB"] = new tbfields("B",x + 250, y + 210, 45, 20, "int", "255", ModifyEnable);
                        ct["AnimSpeed"] = new tbfields("AnimSpeed",x + 150, y + 231, 150, 20, "int", "-1", ModifyEnable);
                        ct["Frame"] = new tbfields("Frame",x + 150, y + 252, 150, 20, "int", "0", ModifyEnable);
                        ct["ScaleX"] = new tbfields("ScaleX",x + 150, y + 273, 70, 20, "int", "1000", ModifyEnable);
                        ct["ScaleY"] = new tbfields("ScaleY",x + 230, y + 273, 70, 20, "int", "1000", ModifyEnable);
                        ct["Tag"] = new tbfields("Tag",x + 150, y + 315, 150, 20, "string", "", ModifyEnable);
                        cb["Impassible"] = new tbcheckbox(x + 150, y + 147, ModifyEnable);
                        cb["ForcePassible"] = new tbcheckbox(x + 150, y + 168, ModifyEnable);
                        cb["ShowZones"] = new tbcheckbox(x + 150, y + 400, delegate { return true; });
                        cb["ShowZones"].value = true;
                        cb["ShowZones"].Name = "ShowZones";
                        cb["Visible"] = new tbcheckbox(x + 150, y + 294,ModifyEnable);
                        cb["Visible"].Name = "Visible";
                        ct["NewData"] = new tbfields("NewData", x, ScrHeight - 100, 200, 20, "string", "New Data Field", EnHaveObj);
                        ct["ModData"] = new tbfields("ModData", x, ScrHeight - 75, 200, 20, "string", "Modify Data", delegate { return M_SelectedObject != null && M_SelectedObject.MetaData.Count > 0; });
                    }
                    if (cb.ContainsKey("Impassible")) cb["Impassible"].Name = "Impassible";
                    if (cb.ContainsKey("ForcePassible")) cb["ForcePassible"].Name = "ForcePassible";
                }                
            }
        }

        static SortedDictionary<string,TQMGText> OtherStuff;
        static string chosencspot = "";
        static void InitOther() {
            DBG.Log("Init: Other Tab");
            OtherStuff = new SortedDictionary<string, TQMGText>();
            OtherStuff["Exit"] = font20.Text("Exit");
            OtherStuff["Pivot"] = font20.Text("Pivot");
            foreach (string cspot in Core.ProjectConfig.List("CSpots")) {
                var allowregex = new Regex(@"^[a-zA-Z0-9_]+$");
                if (cspot[0] != '$')
                    DBG.Log($"ERROR! CSpot {cspot} not prefixed with a '$'");
                else if (!allowregex.IsMatch(cspot.Substring(1)))
                    DBG.Log($"ERROR! CSpot {cspot} has illegal characters in its name");
                else if (cspot.Length < 4)
                    DBG.Log($"ERROR! CSpot must have at least 3 characters after the $ ({cspot})");
                else {
                    DBG.Log($"Adding CSpot: {cspot}");
                    OtherStuff[cspot] = font20.Text(cspot);
                }
            }
            
        }

        static void Other() {
            var y = TBItem.inity + 60 + 21;
            if (OtherStuff == null) InitOther();
            foreach(string cspot in OtherStuff.Keys) {
                var txt = OtherStuff[cspot];
                TQMG.Color(0, 255, 255);
                if (Core.ms.X > ToolX && Core.ms.Y > y && Core.ms.Y < y + 25 && Core.MsHit(1))
                    chosencspot = cspot;
                if (chosencspot==cspot) {
                    TQMG.DrawRectangle(ToolX, y, ToolW, 24);
                    TQMG.Color(0, 0, 0);
                }
                txt.Draw(ToolX + 3, y);
                y += 25;
            }
        }

        static public void DrawToolBox() {
            TQMG.Color(255, 255, 255);
            TQMG.SimpleTile(back, ToolX, 0, ToolW, ScrHeight, 0);
            if (TBItems == null) InitToolBox();
            foreach(TBItem i in TBItems) {
                if (currentTBItem == null) {
                    currentTBItem = i;
                    Debug.WriteLine($"currentTBItem was null, so I set it to TBItem {i.Name}");
                }
                i.Button[currentTBItem == i].Draw(i.X, i.Y);
                if (Core.MsHit(1) && Core.ms.X > i.X && Core.ms.X < i.X + i.Button[true].Width && Core.ms.Y > i.Y && Core.ms.Y < i.Y + 50) currentTBItem = i;
            }            
            currentTBItem.Work?.Invoke();
        }
        #endregion

        #region AreaRelease
        static Random rnd = new Random();
        static void AR_Tiled(int x1,int y1,int x2, int y2) {
            var opm = ObjectParamFields[currentTBItem.Name];
            var opc = ObjectCheckBoxes[currentTBItem.Name];
            var startx = x1;
            var starty = y1;
            var width = Math.Abs(x2 - x1);
            var height = Math.Abs(y2 - y1);
            if (x1 > x2) startx = x2;
            if (y1 > y2) starty = y2;
            if (opm["Texture"].value == "") return;
            var Area = new KthuraObject("TiledArea", MapLayer)
            {
                x = startx,
                y = starty,
                w = width,
                h = height,
                Texture = opm["Texture"].value,
                insertx = qstr.ToInt(opm["InsX"].value),
                inserty = qstr.ToInt(opm["InsY"].value),
                R = qstr.ToInt(opm["cR"].value),
                G = qstr.ToInt(opm["cG"].value),
                B = qstr.ToInt(opm["cB"].value),
                Dominance = qstr.ToInt(opm["Dominance"].value),
                Alpha1000 = qstr.ToInt(opm["Alpha"].value),
                AnimSpeed = qstr.ToInt(opm["AnimSpeed"].value),
                AnimFrame = qstr.ToInt(opm["Frame"].value),
                Impassible = opc["Impassible"].value,
                ForcePassible = opc["ForcePassible"].value,
                Visible = opc["Visible"].value
            };
            if (!Area.Visible) Console.Beep();
            // insert modulos
            if (opc["AutoIns"].value) {
                int w = 0;
                int h = 0;
                KthuraDrawMonoGame.TexSizes(Area, ref w, ref h);
                Area.insertx = Area.x % w;
                Area.inserty = Area.y % h;
            }
            SealTexMemory();
            DBG.Log($"Created TiledArea at ({Area.x},{Area.y}), size: {Area.w}x{Area.h}");
        }

        static void AR_Stretched(int x1, int y1, int x2, int y2) {
            var opm = ObjectParamFields[currentTBItem.Name];
            var opc = ObjectCheckBoxes[currentTBItem.Name];
            var startx = x1;
            var starty = y1;
            var width = Math.Abs(x2 - x1);
            var height = Math.Abs(y2 - y1);
            if (x1 > x2) startx = x2;
            if (y1 > y2) starty = y2;
            if (opm["Texture"].value == "") return;
            var Area = new KthuraObject("StretchedArea", MapLayer) {
                x = startx,
                y = starty,
                w = width,
                h = height,
                Texture = opm["Texture"].value,
                //insertx = qstr.ToInt(opm["InsX"].value),
                //inserty = qstr.ToInt(opm["InsY"].value),
                R = qstr.ToInt(opm["cR"].value),
                G = qstr.ToInt(opm["cG"].value),
                B = qstr.ToInt(opm["cB"].value),
                Dominance = qstr.ToInt(opm["Dominance"].value),
                Alpha1000 = qstr.ToInt(opm["Alpha"].value),
                AnimSpeed = qstr.ToInt(opm["AnimSpeed"].value),
                AnimFrame = qstr.ToInt(opm["Frame"].value),
                Impassible = opc["Impassible"].value,
                ForcePassible = opc["ForcePassible"].value,
                Visible = opc["Visible"].value
            };
            if (!Area.Visible) Console.Beep();
            // insert modulos
            /* No value here!
            if (opc["AutoIns"].value) {
                int w = 0;
                int h = 0;
                KthuraDrawMonoGame.TexSizes(Area, ref w, ref h);
                Area.insertx = Area.x % w;
                Area.inserty = Area.y % h;
            }
            SealTexMemory();
            // */
            DBG.Log($"Created StretchedArea at ({Area.x},{Area.y}), size: {Area.w}x{Area.h}");
        }


        static void AR_Zones(int x1, int y1, int x2, int y2) {
            var startx = x1;
            var starty = y1;
            var width = Math.Abs(x2 - x1);
            var height = Math.Abs(y2 - y1);
            if (x1 > x2) startx = x2;
            if (y1 > y2) starty = y2;
            var zone = new KthuraObject("Zone",MapLayer);
            var opm = ObjectParamFields[currentTBItem.Name];
            var opc = ObjectCheckBoxes[currentTBItem.Name];
            zone.x = startx;
            zone.y = starty;
            zone.w = width;
            zone.h = height;
            zone.R = rnd.Next(100, 255);
            zone.G = rnd.Next(100, 255);
            zone.B = rnd.Next(100, 255);
            zone.Dominance = qstr.ToInt(opm["Dominance"].value);
            var cnt = -1;
            var tag = "";
            do { cnt++; tag = $"Zone #{cnt}"; } while (MapLayer.HasTag(tag));
            zone.Tag = tag;
            DBG.Log($"Added zone: {tag}");
            zone.Impassible = opc["Impassible"].value;
            zone.ForcePassible = opc["ForcePassible"].value;
        }
        #endregion

        #region Modify
        static KthuraObject _M_SelectedObject = null;
        static KthuraObject M_SelectedObject        {
            get => _M_SelectedObject;
            set {
                _M_SelectedObject = value;
                if (value == null) return;
                var Fields = ObjectParamFields[currentTBItem.Name];
                var Checkboxes = ObjectCheckBoxes[currentTBItem.Name];
                Fields["Kind"].value = value.kind;
                Fields["Texture"].value = value.Texture;
                Fields["X"].value = value.x.ToString();
                Fields["Y"].value = value.y.ToString();
                Fields["InsX"].value = value.insertx.ToString();
                Fields["InsY"].value = value.inserty.ToString();
                Fields["Width"].value = value.w.ToString();
                Fields["Height"].value = value.h.ToString();
                Fields["Labels"].value = value.Labels;
                Fields["Dominance"].value = value.Dominance.ToString();
                Fields["AnimSpeed"].value = value.AnimSpeed.ToString();
                Fields["Alpha"].value = value.Alpha1000.ToString();
                Fields["RotDeg"].value = value.RotationDegrees.ToString();
                Fields["cR"].value = value.R.ToString();
                Fields["cG"].value = value.G.ToString();
                Fields["cB"].value = value.B.ToString();
                Fields["Tag"].value = value.Tag;
                Fields["ScaleX"].value = value.ScaleX.ToString();
                Fields["ScaleY"].value = value.ScaleY.ToString();
                Checkboxes["Impassible"].value = value.Impassible;
                Checkboxes["ForcePassible"].value = value.ForcePassible;
                Checkboxes["Visible"].value = value.Visible;
            }

        }

        static void ModifyField(object f) {
            var strval = "";
            var intval = 0;
            var bolval = false;
            var fldtype = f.GetType();
            var fieldname = "";
            if (fldtype.Equals(typeof(tbfields))) {
                fieldname = ((tbfields)f).Name;
                strval = ((tbfields)f).value;
                intval = qstr.ToInt(strval);
            } else if (fldtype.Equals(typeof(tbcheckbox))) {
                fieldname = ((tbcheckbox)f).Name;
                bolval = ((tbcheckbox)f).value;
            } else {
                throw new Exception($"Unknown Modifyfield type {fldtype}");
            }
            switch (fieldname) {
                case "ShowZones":
                    return;
                case "Kind":
                case "Texture":
                    throw new Exception($"{fieldname} may never be changed! Is your version of Kthura hacked?");
                case "X":
                    M_SelectedObject.x = intval; break;
                case "Y":
                    M_SelectedObject.y = intval; break;
                case "InsX":
                    M_SelectedObject.insertx = intval; break;
                case "InsY":
                    M_SelectedObject.inserty = intval; break;
                case "W": case "Width":
                    M_SelectedObject.w = intval; break;
                case "H": case "Height":
                    M_SelectedObject.h = intval; break;
                case "Labels":
                    M_SelectedObject.Labels = strval; break;
                case "Dominance":
                    M_SelectedObject.Dominance = intval; break;
                case "AnimSpeed":
                    M_SelectedObject.AnimSpeed = intval; break;
                case "Alpha":
                    M_SelectedObject.Alpha1000 = intval; break;
                case "RotDeg":
                    M_SelectedObject.RotationDegrees = intval; break;
                case "cR": case "R":
                    M_SelectedObject.R = intval; break;
                case "cG": case "G":
                    M_SelectedObject.G = intval; break;
                case "cB": case "B":
                    M_SelectedObject.B = intval; break;
                case "Tag":
                    M_SelectedObject.Tag = strval; break;
                case "Impassible":
                    M_SelectedObject.Impassible = bolval; break;
                case "ForcePassible":
                    M_SelectedObject.ForcePassible = bolval; break;
                case "Visible":
                    M_SelectedObject.Visible = bolval;
                    if (!bolval) Console.Beep();
                    break;
                case "ScaleX":
                    M_SelectedObject.ScaleX = intval;
                    break;
                case "ScaleY":
                    M_SelectedObject.ScaleY = intval;
                    break;
                default:
                    Debug.WriteLine($"WARNING! I do not know field name {fieldname}");
                    break;
            }
        }

        static void CatchTag(object d) {
            var tag = ((Dictionary<string, string>)d)["Tag"];
            if (MapLayer.HasTag(tag, true)) {
                Console.Beep();
                DBG.Log($"There's already an other object tagged {tag}");
                return;
            }
            ObjectParamFields["Modify"]["Tag"].value = tag;
            ModifyField(ObjectParamFields["Modify"]["Tag"]);
        }
        static void CatchLabels(object d) {
            var labels = ((Dictionary<string, string>)d);
            var r = "";
            foreach(string k in labels.Keys) {
                var v = labels[k];
                var s = v.Split(',');
                foreach(string label in s) {
                    if (r != "") r += ",";
                    r += label;
                }
            }
            ObjectParamFields[currentTBItem.Name]["Labels"].value = r;
            if (currentTBItem.Name=="Modify") ModifyField(ObjectParamFields["Modify"]["Labels"]);
        }
        #endregion

        #region MapClick
        static void MC_Obstacle(int x,int y) {
            var opm = ObjectParamFields[currentTBItem.Name];
            var opc = ObjectCheckBoxes[currentTBItem.Name];
            if (opm["Texture"].value == "") return;
            var obs = new KthuraObject("Obstacle", MapLayer)
            {
                x = ogPosX,
                y = ogPosY,
                Texture = opm["Texture"].value,
                R = qstr.ToInt(opm["cR"].value),
                G = qstr.ToInt(opm["cG"].value),
                B = qstr.ToInt(opm["cB"].value),
                Dominance = qstr.ToInt(opm["Dominance"].value),
                Alpha1000 = qstr.ToInt(opm["Alpha"].value),
                AnimSpeed = qstr.ToInt(opm["AnimSpeed"].value),
                AnimFrame = qstr.ToInt(opm["Frame"].value),
                Impassible = opc["Impassible"].value,
                ForcePassible = opc["ForcePassible"].value,
                ScaleX = qstr.ToInt(opm["ScaleX"].value),
                ScaleY = qstr.ToInt(opm["ScaleY"].value),
                Visible = opc["Visible"].value,
                RotationDegrees = qstr.ToInt(opm["RotDeg"].value) // Will automatically assign the "radian" value too.
            };
            if (!obs.Visible) Console.Beep();
            DBG.Log($"Created Obstacle at ({obs.x},{obs.y})");
        }
        static void MC_CSpot(int x,int y) {
            if (chosencspot == "") return;
            var cst = chosencspot.Replace("$", "CSPOT_");
            Lua_XStuff.callbackstage = "INIT";
            Lua_XStuff.Ask.Clear();
            Core.Lua($"if type({cst})=='table' then {cst}.Init() else {cst}_Init() end");
            Lua_XStuff.WantX = ogPosX;
            Lua_XStuff.WantY = ogPosY;
            QuestionList.ComeToMe($"Data for creating {chosencspot}",Lua_XStuff.Ask.ToArray(),CSpot_Place);
        }

        static void CSpot_Place(object data) {
            var cst = chosencspot.Replace("$", "CSPOT_");
            var answers = (Dictionary<string, string>)data;
            Lua_XStuff.callbackstage = "CREATE";
            var bs = new StringBuilder(1);
            bs.Append("local KTHURAQDATA = {}\n");
            foreach (string k in answers.Keys)
                bs.Append($"KTHURAQDATA[\"{k}\"] = \"{answers[k]}\"\n");
            Lua_XStuff.ME = new KthuraObject(chosencspot, MapLayer);
            Lua_XStuff.ME.x = Lua_XStuff.WantX;
            Lua_XStuff.ME.y = Lua_XStuff.WantY;
            Lua_XStuff.ME.Alpha1000 = 1000;
            bs.Append($"if type({cst})=='table' then {cst}.Create(Kthura.ME,KTHURAQDATA) elseif type({cst}_Create)=='function' then {cst}_Create(Kthura.ME,KTHURAQDATA) else error('No {cst}_Create or {cst}.Create') end\n");
            bs.Append("KTHURAQDATA = nil\n");
            Core.Lua(bs.ToString());
        }

        static void MC_Modify(int x, int y) {
            var ms = new MouseState(Core.ms.X + ScrollX - LayW, Core.ms.Y + ScrollY - PDnH,0,ButtonState.Pressed,ButtonState.Released, ButtonState.Released, ButtonState.Released, ButtonState.Released);
            M_SelectedObject = null;
            foreach(KthuraObject obj in MapLayer.ObjectDrawOrder) {
                var kind = obj.kind; if (kind[0] == '$') kind = "Pivot";
                switch (kind) {
                    case "TiledArea": case "Zone": case "StretchedArea":
                        if (ms.X>=obj.x && ms.Y>obj.y && ms.X<=obj.x+obj.w && ms.Y <= obj.y + obj.h) 
                            M_SelectedObject = obj;
                        break;
                    case "Pic":
                        if (ms.Y >= obj.y && ms.Y <= obj.y + KthuraDraw.DrawDriver.ObjectHeight(obj) && ms.X >= obj.x  && ms.X <= obj.x + KthuraDraw.DrawDriver.ObjectWidth(obj) )
                            M_SelectedObject = obj;
                        break;
                    case "Obstacle":
                        if (ms.Y<=obj.y && ms.Y>=obj.y-KthuraDraw.DrawDriver.ObjectHeight(obj) && ms.X>=obj.x-(KthuraDraw.DrawDriver.ObjectWidth(obj)/2) && ms.X <= obj.x + (KthuraDraw.DrawDriver.ObjectWidth(obj) / 2)) 
                            M_SelectedObject = obj;
                        break;
                    case "Exit": case "Pivot":
                        if (ms.X>=obj.x-8 && ms.X<=obj.x+8 && ms.Y>=obj.y-8 && ms.Y <= obj.y + 8) 
                            M_SelectedObject = obj;
                        break;
                }
            }
        }
        #endregion

        #region Draw Map
        static bool ShowGrid = true;
        static bool GridMode = true;
        static void DrawGrid() {
            if (!ShowGrid) return;
            var modx = ScrollX % MapLayer.GridX;
            var mody = ScrollY % MapLayer.GridY;
            var bl = false;
            for(int y = -mody; y < ScrHeight + MapLayer.GridY; y += MapLayer.GridY) {
                var cl = bl; bl = !bl;
                for (int x = -modx; x < ScrWidth + MapLayer.GridX; x += MapLayer.GridX) {
                    cl = !cl;
                    if (cl)
                        TQMG.Color(15, 0, 20);
                    else
                        TQMG.Color(0, 20, 20);
                    TQMG.DrawRectangle(x + LayW, y + PDnH, MapLayer.GridX, MapLayer.GridY);
                }
            }
        }

        static public void DrawOrigin() {
            TQMG.Color(200, 0, 0);
            if (ScrollX < 0) 
                TQMG.DrawRectangle(LayW, 0, -ScrollX, ScrHeight);
            if (ScrollY < 0)
                TQMG.DrawRectangle(0, PDnH, ScrWidth, -ScrollY);
        }

        static void DrawHold() {
            if (currentTBItem == null) return;
            if (!currentTBItem.area) return;
            if (!HoldArea) return;
            var s = DateTime.Now.Second;
            var s6 = (s * 6)*(Math.PI*180);
            var r = (int)(Math.Abs(Math.Sin(s6)) * 255);
            var g = (int)(Math.Abs(Math.Cos(s6)) * 255);
            var b = s * 2;
            TQMG.Color((byte)r, (byte)g, (byte)b);
            TQMG.SetAlpha(15);
            TQMG.DrRect((HoldX-ScrollX)+LayW, (HoldY-ScrollY)+PDnH, (HoldEX-ScrollX)+LayW, (HoldEY-ScrollY)+PDnH);
            TQMG.SetAlpha(255);
        }

        static void DrawModifyObject() {
            if (currentTBItem == null) return;
            if (currentTBItem.Name != "Modify") return;
            if (M_SelectedObject == null) return;
            var d = (DateTime.Now.Millisecond * 36)/100; //Second * 6;
            var r = d * (Math.PI / 180);
            var c = (int)(Math.Sin(r) * 255); if (c < 0) c = c * (-1);
            var cb = (byte)c;
            var x = M_SelectedObject.x - ScrollX + LayW;
            var y = M_SelectedObject.y - ScrollY + PDnH;
            var kind = M_SelectedObject.kind; if (kind[0] == '$') kind = "Pivot";
            var KD = KthuraDraw.DrawDriver;
            TQMG.Color(cb, cb, (byte)(((byte)255)-cb));
            switch (kind) {
                case "TiledArea":
                case "Zone":
                case "StretchedArea":
                    TQMG.DrawLineRect(x, y, M_SelectedObject.w, M_SelectedObject.h);
                    break;
                case "Pivot":
                case "Exit":
                    TQMG.DrawLineRect(x - 8, y - 8, 16, 16);
                    break;
                case "Pic":
                    TQMG.DrawLineRect(x , y , KD.ObjectWidth(M_SelectedObject), KD.ObjectHeight(M_SelectedObject));
                    break;
                case "Obstacle":
                    TQMG.DrawLineRect(x - (KD.ObjectWidth(M_SelectedObject) / 2), y - KD.ObjectHeight(M_SelectedObject),  KD.ObjectWidth(M_SelectedObject) , KD.ObjectHeight(M_SelectedObject));
                    break;
            }
        }

        static public void DrawMap() {
            if (selectedlayer == "") return;
            DrawGrid();
            DrawOrigin();
            KthuraDraw.DrawMap(MapLayer, ScrollX, ScrollY, LayW, PDnH);
            DrawHold();
            DrawModifyObject();
        }
        #endregion

        #region Start Call
        static UI() {
            InitToolBox();
            if (File.Exists(TexMemorySettingsFile)) {
                DBG.Log($"Loading TexMemory file: {TexMemorySettingsFile}");
                TexMemory = GINI.ReadFromFile(TexMemorySettingsFile);
                ScrollX = qstr.ToInt(TexMemory.C("CAMX"));
                ScrollY = qstr.ToInt(TexMemory.C("CAMY"));
                selectedlayer = TexMemory.C("LAYR");
            } else {
                DBG.Log($"Creating TexMemory GINI base since {TexMemorySettingsFile} does not yet exist");
                TexMemory = new TGINI();
            }
                
        }
        #endregion

        #region Schedule Manager
        struct MiniSchedule {
            internal UISchedule func;
            internal object param;
        }
        static List<MiniSchedule> SchedFuncs = new List<MiniSchedule>();
        static public void Schedule(UISchedule func, object param) {
            var s = new MiniSchedule
            {
                func = func,
                param = param
            };
            SchedFuncs.Add(s);
            Debug.Print("Function scheduled");
        }
        static void SchedulePop() {
            if (SchedFuncs.Count >= 1) {
                var sf = SchedFuncs.ToArray()[0];
                sf.func(sf.param);
                SchedFuncs.RemoveAt(0);
                Debug.Print("Function popped");
            }
        }
        #endregion

        #region Quick PullDown Debug Link functions
        static void CountObjects() {
            DBG.Log($"Counting objects on layer: {selectedlayer}");
            SortedDictionary<string, int> counts = new SortedDictionary<string, int>();
            foreach(KthuraObject o in MapLayer.Objects) {
                if (!counts.ContainsKey(o.kind)) counts[o.kind] = 0;
                counts[o.kind]++;
            }
            foreach (string k in counts.Keys) DBG.Log($"- {k}:{counts[k]}");
            DBG.Log($"Grand total: {MapLayer.Objects.Count}");
            DBG.Log("Hit Escape to get back to the editor");
            DBG.ComeToMe();
        }

        static void TagMap() {
            DBG.Log("== TagMap ==");
            MapLayer.RemapTags();
            foreach (string k in MapLayer.Tags)
                DBG.Log($"{MapLayer.FromTag(k).kind} {k}");
            DBG.Log("Hit Escape to get back to the editor");
            DBG.ComeToMe();
        }

        static void GoToPos(object o) {
            var pos = (Dictionary<string, string>)o;
            var X = qstr.ToInt(pos["X"]);
            var Y = qstr.ToInt(pos["Y"]);
            ScrollX = X;
            ScrollY = Y;
        }

        static void ShowBlockMap() {
            DBG.Log($"Blockmap layer: {selectedlayer};   {MapLayer.BlockMapWidth}x{MapLayer.BlockMapHeight}");
            for (int y = 0; y <= MapLayer.BlockMapHeight; y++) {
                var l = new StringBuilder(MapLayer.BlockMapWidth);
                for (int x = 0; x <= MapLayer.BlockMapWidth; x++) {
                    if (MapLayer.PureBlock(x, y))
                        l.Append("X");
                    else
                        l.Append("_");
                }
                DBG.Log($"{y.ToString("D4")}: {l.ToString()}");
            }
            DBG.Log("Hit Escape to get back to the editor");
            DBG.ComeToMe();
        }

        static void UnOrigin()        {
            DBG.Log("UnOrigin Request done!");
            int mx = 0, my = 0;
            foreach (KthuraObject o in MapLayer.Objects) {
                if (mx > o.x) mx = o.x;
                if (my > o.y) my = o.y;
            }
            if (mx == 0 && my == 0) { DBG.Log("Nothing underorigin, so let's get outta here!"); return; }
            DBG.Log($"UnderOrigin Objects found. {Math.Abs(mx)} x-dist UnderOrigin, and {Math.Abs(my)} y=dist UnderOrigin. Let's fix that!");
            foreach (KthuraObject o in MapLayer.Objects) {
                o.x -= mx;
                o.y -= my;
                // Please note, since mx and my always contain a negative number, you get --, which will always generate a + in mathematics.
            }
        }

        static void OptimizeToOrigin() {
            UnOrigin();
            DBG.Log("OptimizeOrigin Request done!");
            int mx = -1, my = -1;
            foreach (KthuraObject o in MapLayer.Objects) {
                if (mx > o.x || mx<0) mx = o.x;
                if (my > o.y || my<0) my = o.y;
            }
            if (mx < 0) mx = 0;
            if (my < 0) my = 0;
            if (mx == 0 && my == 0) { DBG.Log("Nothing wrong, so let's get outta here!"); return; }
            DBG.Log($"Origin WhiteSpace found. {Math.Abs(mx)} x-dist from Origin, and {Math.Abs(my)} y=dist from Origin. Let's fix that!");
            foreach (KthuraObject o in MapLayer.Objects) {
                o.x -= mx;
                o.y -= my;
            }

        }

        static void ScanRotten() {
            var kill = new List<KthuraObject>();
            foreach(KthuraObject o in MapLayer.Objects) {
                var Rotten = false;
                // Scan
                Rotten = Rotten || ((o.kind == "TiledArea" || o.kind == "Zone") && (o.w == 0 || o.h == 0));
                // Declared 'rotten'
                if (Rotten && Confirm.Yes($"Object {o.kind}({o.x},{o.y}) {o.w}x{o.h} has been declared 'rotten'.\n\nShall I remove it?")) kill.Add(o);
                if (o.kind == "Obstacle" && (o.ForcePassible || o.Impassible) && Confirm.Yes($"Obstacle has been found with the 'Impassible'/'Forcepassible' bit set to true.\n(Imp:{o.Impassible}; FcP:{o.ForcePassible})\n\nThe possibility to do this has been deprecated, and in future versions these bits will likely be ignored on obstacles. I therefore recommend to remove these bits from this obstacle.\n\nAre you okay with that?")) { o.Impassible = false; o.ForcePassible = false; }
                if (o.Impassible && o.ForcePassible && Confirm.Yes($"Object { o.kind} ({ o.x},{ o.y}) { o.w} has both Impassible and ForcePassible checked!\n\n\nI recommend to either remove this object or to edit that out!\n\nDo you want to remove it?")) kill.Add(o);                
            }
            foreach (KthuraObject o in kill) MapLayer.Objects.Remove(o);
            MapLayer.TotalRemap();
        }
        #endregion

        #region Meta
        static void Meta() {
            var outlist = new List<string>();
            
            foreach(string l in Core.ProjectConfig.List("GeneralData")) {
                var eq = l.IndexOf("=");
                var q = l; // question
                var d = ""; // default value
                if (eq>=0) {
                    q = l.Substring(0, eq);
                    d = l.Substring(eq + 1);
                }
                if (Core.Map.MetaData.ContainsKey(q))
                    outlist.Add($"{q}={Core.Map.MetaData[q]}");
                else
                    outlist.Add($"{q}={d}");
            }
            if (outlist.Count == 0)
                DBG.Log("No fields for Meta Data Edit!");
            else
                QuestionList.ComeToMe("Enter Meta Data for this map", outlist.ToArray(), MetaUpdate);                    
        }

        static void MetaUpdate(object a) {
            var d = (Dictionary<string, string>)a;
            foreach(string k in d.Keys) {
                Core.Map.MetaData[k] = d[k];
            }
            DBG.Log("Meta updated");
        }
        #endregion

        #region int main() :P
        static public void DrawScreen() {
            DrawMap();            
            DrawLayerBox();
            DrawToolBox();
            DrawPullDown();
            DrawStatus();
        }



        static public void UI_Update() {
            // Schedule pop
            SchedulePop();
            // Scroll
            if (Core.kb.IsKeyDown(Keys.LeftControl) || Core.kb.IsKeyDown(Keys.RightControl)) {
                var k = TQMGKey.GetKey();
                switch (k) {
                    case Keys.Up:
                        ScrollY -= MapLayer.GridY / 2;
                        break;
                    case Keys.Down:
                        ScrollY += MapLayer.GridY / 2;
                        break;
                    case Keys.Left:
                        ScrollX -= MapLayer.GridX / 2;
                        break;
                    case Keys.Right:
                        ScrollX += MapLayer.GridX / 2;
                        break;
                    case Keys.PageDown:
                        ScrollY += (int)(Math.Floor((decimal)TQMG.ScrHeight/ MapLayer.GridY)* (MapLayer.GridY-2));
                        break;
                    case Keys.PageUp:
                        ScrollY -= (int)(Math.Floor((decimal)TQMG.ScrHeight / MapLayer.GridY) * (MapLayer.GridY - 2));
                        break;
                }
            }
            // Delete
            if (currentTBItem!=null && currentTBItem.Name=="Modify" && MapLayer!=null && M_SelectedObject != null) {
                var k = TQMGKey.GetKey();
                if (k == Keys.Delete) {
                    if (Confirm.Yes($"Are you sure you wish to delete this {M_SelectedObject.kind}?")) {
                        MapLayer.Objects.Remove(M_SelectedObject);
                        M_SelectedObject = null;
                        MapLayer.TotalRemap();
                    }
                }
            }
            // Fields in toolbox
            var ch = TQMGKey.GetChar();
            var key = TQMGKey.GetKey();
            if (currentTBItem != null && curfield != null) {
                if (ch >= 32 && ch < 127) {
                    if (curfield.dtype == "int" && (ch < 48 || ch > 57) && (!(ch == '-' && curfield.value == ""))) {
                        Debug.Print($"Ignore nun-number: {ch}");
                    } else {
                        curfield.value += ch;
                        if (curfield.dtype == "int" && curfield.value.Length > 1 && curfield.value[0] == '0') {
                            curfield.value = qstr.Right(curfield.value, curfield.value.Length - 1);
                        }
                        if (currentTBItem.Name == "Modify") ModifyField(curfield);
                    }
                } else if (key == Keys.Back && curfield.value != "") {
                    curfield.value = qstr.Left(curfield.value, curfield.value.Length - 1);
                    if (currentTBItem.Name == "Modify") ModifyField(curfield);
                }
            }
            // Mousedown/up
            if (Core.ms.X < LayW || Core.ms.X > ToolX || Core.ms.Y < PDnH || Core.ms.Y > ScrHeight - 25)
                HoldArea = false;
            else if (currentTBItem != null) {
                //Debug.Print($"{currentTBItem.Name}: a-area{currentTBItem.area}; MouseDown{Core.MsDown(1)}");
                if (currentTBItem.mapclick && Core.MsHit(1)) {
                    currentTBItem.MC(PosX, PosY);
                } else if (currentTBItem.area && Core.MsDown(1)) {
                    if (!HoldArea) {
                        HoldArea = true;
                        if (GridMode) {
                            HoldX = ((int)Math.Floor((decimal)PosX / Core.Map.Layers[selectedlayer].GridX) * Core.Map.Layers[selectedlayer].GridX) + (ScrollX % Core.Map.Layers[selectedlayer].GridX);
                            HoldY = ((int)Math.Floor((decimal)PosY / Core.Map.Layers[selectedlayer].GridY) * Core.Map.Layers[selectedlayer].GridY) + (ScrollY % Core.Map.Layers[selectedlayer].GridY);
                        } else {
                            HoldX = PosX;
                            HoldY = PosY;
                        }
                    }
                    if (GridMode) {
                        HoldEX = ((int)Math.Floor((decimal)PosX / Core.Map.Layers[selectedlayer].GridX) * Core.Map.Layers[selectedlayer].GridX) + (ScrollX % Core.Map.Layers[selectedlayer].GridX);
                        HoldEY = ((int)Math.Floor((decimal)PosY / Core.Map.Layers[selectedlayer].GridY) * Core.Map.Layers[selectedlayer].GridY) + (ScrollY % Core.Map.Layers[selectedlayer].GridY);
                    } else {
                        HoldEX = PosX;
                        HoldEY = PosY;
                    }

                }
                if (HoldArea && !Core.MsDown(1)) {
                    currentTBItem.AR(HoldX, HoldY, HoldEX, HoldEY);
                    HoldArea = false;
                }

            } else {
                Debug.WriteLine("Holding check not performed, due to currentTBItem being null");
            }

            // Update Pulldown stuff
            PullDownQuickKeys();
            //Debug.WriteLine(PDEvent);
            switch (PDEvent) {
                // File Menu
                case 1001: Core.Save(); break;
                case 1002: Meta(); break;
                // Grid
                case 2001: ShowGrid = !ShowGrid; break;
                case 2002: GridMode = !GridMode; break;
                // Debug
                case 3001: DBG.ComeToMe(); break;
                case 3002: ShowBlockMap(); break;
                case 3003: CountObjects(); break;
                case 3004: ScanRotten(); break;
                case 3005: TagMap(); break;
                case 3006:
                    QuestionList.ComeToMe("Please enter the new cam position:", new string[] { "X", "Y" }, GoToPos);
                    break;
                case 3007: UnOrigin(); break;
                case 3008: OptimizeToOrigin(); break;
                // Layers
                case 4001: LayerName.ComeToMe(); break;
                case 4002: Yes.ComeToMe($"Do you really want to remove layer \"{selectedlayer}\"?", DeleteLayer, selectedlayer); break;
                case 4003: LayerName.ComeToMe(selectedlayer); break;
                // Quit
                case 9999: Core.Quit(); break;
            }
            PDEvent = 0;
        }
        #endregion
    }
}