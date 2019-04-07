﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            public PullDownItem(string caption,int evCode, Keys QuickKey) {
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
        static List<PullDownHeader> InitPullDownMenus()  {
            var ret = new List<PullDownHeader>();
            ret.Add(new PullDownHeader("General",
                new PullDownItem("Save", 1001, Keys.S),
                new PullDownItem("Quit",9999,Keys.Q)
                ));
            ret.Add(new PullDownHeader("Grid",
                new PullDownItem("Show Grid Blocks",2001,Keys.D),
                new PullDownItem("Grid Mode",2002,Keys.G)
                ));
            ret.Add(new PullDownHeader("Debug",
                new PullDownItem("Show Debug Log",3001,Keys.F1),
                new PullDownItem("Show Blockmap",3002,Keys.B),
                new PullDownItem("Count Objects",3003,Keys.T),
                new PullDownItem("Scan and remove \"Rotten\" objects",3004,Keys.F2),
                new PullDownItem("List Object Tags",3005,Keys.Z),
                new PullDownItem("Go To Screen Postion",3006,Keys.F3)
                ));

            return ret;
        }

        static void DrawPullDown() {
            if (PullDownMenus==null) PullDownMenus = InitPullDownMenus();
            TQMG.Color(255, 255, 255);
            TQMG.SimpleTile(back,0, 0, ScrWidth, 25);
            TQMG.Color(0, 255, 255);
            var x = 20;
            foreach(PullDownHeader h in PullDownMenus) {
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

        static public void DrawScreen() {
            // DrawMap()
            // DrawLayerBox()
            // DrawToolBox()
            DrawPullDown();
            // DrawStatus()
        }

        static public void UI_Update() {
            PDEvent = 0;
            PullDownQuickKeys();
            //Debug.WriteLine(PDEvent);
            switch (PDEvent) {
                case 9999: Core.Quit(); break;
            }
        }
    }
}
