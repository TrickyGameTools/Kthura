// Lic:
// Kthura for C#
// User Interface (like the name of the file implies) :P
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
// Version: 19.04.11
// EndLic


using System;
using System.Collections.Generic;
using System.Linq;
using KthuraEdit.Stages;
using System.Diagnostics;
using Microsoft.Xna.Framework.Input;
using TrickyUnits;

namespace KthuraEdit
{
    static class UI {

        #region PullDownMenus
        static List<PullDownHeader> PullDownMenus;
        static Dictionary<Keys, int> PDKeyToEvent = new Dictionary<Keys, int>();
        static bool PDOpen = false;
        class PullDownItem {
            public string CaptString;
            public TQMGText CaptText;
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
                Debug.WriteLine($"  = Created Item: {caption}");
            }
        }
        class PullDownHeader {
            public string CaptString;
            public TQMGText CaptText;
            public List<PullDownItem> Items;
            public PullDownHeader(string Caption, params PullDownItem[] aItems) {
                Debug.WriteLine($"- Created Menu: {Caption}");
                CaptString = Caption;
                CaptText = font20.Text(Caption);
                Items = new List<PullDownItem>();
                foreach (PullDownItem item in aItems) {
                    item.Parent = this;
                    Items.Add(item);
                }
            }
        }
        static List<PullDownHeader> InitPullDownMenus() {
            var ret = new List<PullDownHeader>();
            ret.Add(new PullDownHeader("General",
                new PullDownItem("Save", 1001, Keys.S),
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
                new PullDownItem("Go To Screen Postion", 3006, Keys.F3)
                ));

            return ret;
        }

        static void DrawPullDown() {
            if (PullDownMenus == null) PullDownMenus = InitPullDownMenus();
            TQMG.Color(255, 255, 255);
            TQMG.SimpleTile(back, 0, 0, ScrWidth, 25);
            TQMG.Color(0, 255, 255);
            var x = 20;
            foreach (PullDownHeader h in PullDownMenus) {
                h.CaptText.Draw(x, 3);
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
        const int LayW = 100;
        const int PDnH = 25;
        static int ToolW => 300 + (back.Width % 100);
        static int ToolX => ScrWidth - ToolW;
        static int ScrollX = 0;
        static int ScrollY = 0;
        static int PosX => ScrollX + Core.ms.X - LayW;
        static int PosY => ScrollY + Core.ms.Y - PDnH;
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
            if (Core.ms.Y > PDnH && Core.ms.X > LayW && Core.ms.Y < ScrHeight - 25 && Core.ms.X < ToolX)
                font20.DrawText($"Scr({ScrollX},{ScrollY}); Mse({Core.ms.X},{Core.ms.Y}); Pos({PosX},{PosY})", ScrWidth - 5, ScrHeight - 24, TQMG_TextAlign.Right);
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
                if (Core.ms.X < LayW && Core.ms.Y > y && Core.ms.Y < y + 16 && Core.MsHit(1)) selectedlayer = layname;
                if (selectedlayer == "") selectedlayer = layname;
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

        class TBItem {
            internal static int initx { get; private set; } = ToolX;
            internal static int inity { get; private set; } = PDnH + 10;
            internal Dictionary<bool, TQMGImage> Button = new Dictionary<bool, TQMGImage>();
            readonly internal int X, Y;
            readonly public TBWork Work;
            readonly internal string Name;
            readonly public bool area;

            internal TBItem(string fbutton,TBWork w,bool doarea=true) {
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
            }
        }
        static TBItem currentTBItem;
        static List<TBItem> TBItems;
        static Dictionary<bool, TQMGImage> CheckboxImage = new Dictionary<bool, TQMGImage>();

        static bool IkZegAltijdNee(object d = null) => false;
        static bool ModifyEnable(object d = null) => false; // This will change later, but for now, this must do.

        class tbfields {
            readonly TBEnabled GetEnabled;
            public string dtype;
            public string value;
            public int intvalue => qstr.ToInt(value);
            public int x = 0, y = 0;
            public int w = 0, h = 0;
            bool _Enabled=true;
            object EnabledData;
            public bool Enabled {
                get {
                    if (GetEnabled != null) return GetEnabled(EnabledData); else return _Enabled;
                }
                set {
                    if (GetEnabled == null) _Enabled = value; else DBG.Log("ERROR! Tried to changed the enabled state of an auto-enabled field");
                }
            }
            public tbfields(int sx,int sy, int sw, int sh, string otype, string defaultvalue, TBEnabled EnabledFunction = null)         {
                x = sx; y = sy; w = sw; h = sh;
                value = defaultvalue;
                GetEnabled = EnabledFunction;
                dtype = otype;
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
            object EnabledData;
            public bool Enabled {
                get {
                    if (GetEnabled != null) return GetEnabled(EnabledData); else return _Enabled;
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
                    if (curfield == null) curfield = field;
                    if (Core.MsHit(1) && Core.ms.X > field.x && Core.ms.X < field.x + field.w && Core.ms.Y > field.y && Core.ms.Y < field.y + field.h) curfield = field;
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
                    if (Core.MsHit(1) && Core.ms.X >= chkbox.x && Core.ms.X <= chkbox.x + 20 && Core.ms.Y >= chkbox.y && Core.ms.Y <= chkbox.y + 20) chkbox.Toggle();
                }
                CheckboxImage[chkbox.value].Draw(chkbox.x, chkbox.y);
            }
        }
        static bool EnableIns(object o = null) => !ObjectCheckBoxes["TiledArea"]["AutoIns"].value;

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
                new TBItem("TiledArea",ObjectParameters),
                new TBItem("Obstacles",ObjectParameters,false),
                new TBItem("Zones",ObjectParameters),
                new TBItem("Other",null,false),
                new TBItem("Modify",ObjectParameters,false)
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
                new tblabels(x,y+294,"Tag:")
            });
            foreach(TBItem i in TBItems) {
                if (i.Name != "Other") {
                    var ct = ObjectParamFields[i.Name];
                    var cb = ObjectCheckBoxes[i.Name];
                    if (i.Name != "Modify") {
                        var form = "click"; if (i.Name == "Obstacles") form = "N/A";
                        ct["Kind"] = new tbfields(x + 150, y-21, 150, 20, "string", i.Name, IkZegAltijdNee);
                        ct["X"] = new tbfields(x + 150, y + 21, 70, 20, "int", "click", IkZegAltijdNee);
                        ct["Y"] = new tbfields(x + 230, y + 21, 70, 20, "int", "click", IkZegAltijdNee);
                        if (i.Name == "TiledArea") {
                            ct["InsX"] = new tbfields(x + 150, y + 42, 70, 20, "int", "0",EnableIns);
                            ct["InsY"] = new tbfields(x + 230, y + 42, 70, 20, "int", "0",EnableIns);
                            cb["AutoIns"] = new tbcheckbox(x + 110, y + 42);
                        } else {
                            ct["InsX"] = new tbfields(x + 150, y + 42, 70, 20, "int", "N/A", IkZegAltijdNee);
                            ct["InsY"] = new tbfields(x + 230, y + 42, 70, 20, "int", "N/A", IkZegAltijdNee);
                        }
                        ct["Width"] = new tbfields(x + 150, y + 63, 70, 20, "int", form, IkZegAltijdNee);
                        ct["Height"] = new tbfields(x + 230, y + 63, 70, 20, "int", form, IkZegAltijdNee);
                        ct["Labels"] = new tbfields(x + 150, y + 84, 150, 20, "string", "");
                        ct["Dominance"] = new tbfields(x + 150, y + 105, 150, 20, "int", "20");
                        if (i.Name != "Zones") {
                            ct["Texture"] = new tbfields(x + 150, y, 150, 20, "string", "");
                            ct["Alpha"] = new tbfields(x + 150, y + 126, 150, 20, "int", "1000");
                            ct["cR"] = new tbfields(x + 150, y + 210, 45, 20, "int", "255");
                            ct["cG"] = new tbfields(x + 200, y + 210, 45, 20, "int", "255");
                            ct["cB"] = new tbfields(x + 250, y + 210, 45, 20, "int", "255");
                            ct["AnimSpeed"] = new tbfields(x + 150, y + 231, 150, 20, "int", "-1");
                            ct["Frame"] = new tbfields(x + 150, y + 252, 150, 20, "int", "0");
                        } else {
                            ct["Texture"] = new tbfields(x + 150, y, 150, 20, "string", "", IkZegAltijdNee);
                            ct["Alpha"] = new tbfields(x + 150, y + 126, 150, 20, "int", "N/A", IkZegAltijdNee);
                            ct["cR"] = new tbfields(x + 150, y + 210, 45, 20, "int", "Rnd", IkZegAltijdNee);
                            ct["cG"] = new tbfields(x + 200, y + 210, 45, 20, "int", "Rnd", IkZegAltijdNee);
                            ct["cB"] = new tbfields(x + 250, y + 210, 45, 20, "int", "Rnd", IkZegAltijdNee);
                            ct["AnimSpeed"] = new tbfields(x + 150, y + 231, 150, 20, "int", "N/A", IkZegAltijdNee);
                            ct["Frame"] = new tbfields(x + 150, y + 252, 150, 20, "int", "0", IkZegAltijdNee);
                        }
                        if (i.Name == "Obstacles") {
                            ct["RotDeg"] = new tbfields(x + 150, y + 189, 150, 20, "int", "1000");
                            ct["ScaleX"] = new tbfields(x + 150, y + 273, 70, 20, "int", "1000");
                            ct["ScaleY"] = new tbfields(x + 230, y + 273, 70, 20, "int", "1000");
                        } else {
                            ct["RotDeg"] = new tbfields(x + 150, y + 189, 150, 20, "int", "N/A", IkZegAltijdNee);
                            ct["ScaleX"] = new tbfields(x + 150, y + 273, 70, 20, "int", "1000", IkZegAltijdNee);
                            ct["ScaleY"] = new tbfields(x + 230, y + 273, 70, 20, "int", "1000", IkZegAltijdNee);
                        }
                        ct["Tag"] = new tbfields(x + 150, y + 294, 150, 20, "string", "Tag => modify",IkZegAltijdNee);
                        cb["Impassible"] = new tbcheckbox(x + 150, y + 147);
                        cb["ForcePassible"] = new tbcheckbox(x + 150, y + 168);
                    } else {
                        ct["Kind"] = new tbfields(x + 150, y-21, 150, 20, "string", "", IkZegAltijdNee);
                        ct["Texture"] = new tbfields(x + 150, y, 150, 20, "string", "", IkZegAltijdNee);
                        ct["X"] = new tbfields(x + 150, y + 21, 70, 20, "int", "", ModifyEnable);
                        ct["Y"] = new tbfields(x + 230, y + 21, 70, 20, "int", "", ModifyEnable);
                        ct["InsX"] = new tbfields(x + 150, y + 42, 70, 20, "int", "", ModifyEnable);
                        ct["InsY"] = new tbfields(x + 230, y + 42, 70, 20, "int", "", ModifyEnable);
                        ct["Width"] = new tbfields(x + 150, y + 63, 70, 20, "int", "", ModifyEnable);
                        ct["Height"] = new tbfields(x + 230, y + 63, 70, 20, "int", "", ModifyEnable);
                        ct["Labels"] = new tbfields(x + 150, y + 84, 150, 20, "string", "", ModifyEnable);
                        ct["Dominance"] = new tbfields(x + 150, y + 105, 150, 20, "int", "20", ModifyEnable);
                        ct["Alpha"] = new tbfields(x + 150, y + 126, 150, 20, "int", "1000", ModifyEnable);
                        ct["RotDeg"] = new tbfields(x + 150, y + 189, 150, 20, "int", "", ModifyEnable);
                        ct["cR"] = new tbfields(x + 150, y + 210, 45, 20, "int", "255", ModifyEnable);
                        ct["cG"] = new tbfields(x + 200, y + 210, 45, 20, "int", "255", ModifyEnable);
                        ct["cB"] = new tbfields(x + 250, y + 210, 45, 20, "int", "255", ModifyEnable);
                        ct["AnimSpeed"] = new tbfields(x + 150, y + 231, 150, 20, "int", "-1", ModifyEnable);
                        ct["Frame"] = new tbfields(x + 150, y + 252, 150, 20, "int", "0", ModifyEnable);
                        ct["ScaleX"] = new tbfields(x + 150, y + 273, 70, 20, "int", "1000", ModifyEnable);
                        ct["ScaleY"] = new tbfields(x + 230, y + 273, 70, 20, "int", "1000", ModifyEnable);
                        ct["Tag"] = new tbfields(x + 150, y + 294, 150, 20, "string", "", ModifyEnable);
                        cb["Impassible"] = new tbcheckbox(x + 150, y + 147, ModifyEnable);
                        cb["ForcePassible"] = new tbcheckbox(x + 150, y + 168, ModifyEnable);
                    }
                }
            }
        }

        static public void DrawToolBox() {
            TQMG.Color(255, 255, 255);
            TQMG.SimpleTile(back, ToolX, 0, ToolW, ScrHeight, 0);
            if (TBItems == null) InitToolBox();
            foreach(TBItem i in TBItems) {
                if (currentTBItem == null) currentTBItem = i;
                i.Button[currentTBItem == i].Draw(i.X, i.Y);
                if (Core.MsHit(1) && Core.ms.X > i.X && Core.ms.X < i.X + i.Button[true].Width && Core.ms.Y > i.Y && Core.ms.Y < i.Y + 50) currentTBItem = i;
            }            
            currentTBItem.Work?.Invoke();
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

        static public void DrawMap() {
            if (selectedlayer == "") return;
            DrawGrid();
            DrawOrigin();
        }
        #endregion

        #region Start Call
        static UI() {
            InitToolBox();
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
                }
            }

            // Update Pulldown stuff
            PDEvent = 0;
            PullDownQuickKeys();
            //Debug.WriteLine(PDEvent);
            switch (PDEvent) {
                // Grid
                case 2001: ShowGrid = !ShowGrid; break;
                case 2002: GridMode = !GridMode; break;
                // Debug
                case 3001: DBG.ComeToMe(); break;
                // Layers
                case 4001: LayerName.ComeToMe(); break;
                case 4002: Yes.ComeToMe($"Do you really want to remove layer \"{selectedlayer}\"?", DeleteLayer, selectedlayer); break;
                case 4003: LayerName.ComeToMe(selectedlayer); break;
                // Quit
                case 9999: Core.Quit(); break;
            }
        }
        #endregion
    }
}


