using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using TrickyUnits;

namespace KthuraEdit.Stages
{
    class DBG : BaseStage {
        public int maxlines = 500;
        static List<string> Lines = new List<string>();
        static int ScrollUp = 0;
        static DBG me = new DBG();
        TQMGFont MonoFont = TQMG.GetFont("Fonts/Monof55.20.jfbf");
        int LinesOnScreen => TQMG.ScrWidth / 16;
        int StartY {
            get {
                if (Lines.Count < LinesOnScreen) return 0;
                var Overlines = Lines.Count - LinesOnScreen;
                return Overlines * 22;
            }
        }

        DBG() {
            Log("Kthura Map Editor for .NET");
            Log("Coded by: Jeroen P. Broks");
            Log($"Build: {BuildDate.sBuildDate}");
            Log("Released under the terms of the GPL 3");
            Log("");
        }


        public static void Log(string line) {
            var lns = line.Split('\n');
            foreach (string l in lns) {
                Lines.Add(l);
                Debug.WriteLine($"DBGLOG> {l}");
            }
            while (Lines.Count > 500) Lines.RemoveAt(0);
            ScrollUp = 0;
        }

        public override void Draw() {
            UI.BackFull()
            var y = ScrollUp - StartY;
            foreach(string line in Lines) {
                if (y > -30)
                    MonoFont.DrawText(line, 2, y);
                y += 22;
            }
        }

        public override void Update() {
            if (TQMGKey.Hit(Microsoft.Xna.Framework.Input.Keys.Escape)) MainEdit.ComeToMe();
        }

        static public void ComeToMe() => Core.GoStage(me);
    }
}
