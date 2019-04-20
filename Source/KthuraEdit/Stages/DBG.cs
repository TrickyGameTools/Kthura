// Lic:
// Kthura for C#
// Debug console output
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
// Version: 19.04.20
// EndLic





using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using TrickyUnits;
using Microsoft.Xna.Framework.Input;

namespace KthuraEdit.Stages
{
    class DBG : BaseStage {
        public int maxlines = 500;
        static List<string> Lines = new List<string>();
        static int ScrollUp = 0;
        static DBG me = new DBG();
        static public bool TimeToCrash = false;
        TQMGFont MonoFont = TQMG.GetFont("Fonts/Monof55.20.jfbf");
        int LinesOnScreen => TQMG.ScrHeight / 22;
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
            try {
                UI.BackFull();
                var y = ScrollUp - StartY;
                foreach (string line in Lines) {
                    if (y > -30)
                        MonoFont.DrawText(line, 2, y,TQMG_TextAlign.Left,true);
                    y += 22;
                }
            } catch(Exception error) {
                Debug.WriteLine($"Exception during the debug log rendering!\n{error.Message}\nTraceback:\n{error.StackTrace}\n\n"); 
            }
        }

        public override void Update() {
            try {
                if (TQMGKey.Hit(Microsoft.Xna.Framework.Input.Keys.Escape)) {
                    Debug.Print("Leaving the debug screen");
                    if (TimeToCrash)
                        Core.Quit();
                    else
                        MainEdit.ComeToMe();
                }
            } catch (Exception error) {
                Debug.WriteLine($"Exception during the debug log updating!\n{error.Message}\nTraceback:\n{error.StackTrace}\n\n");
            }
            if (TQMGKey.Held(Keys.Down) && ScrollUp > 0) ScrollUp -= 2;
            if (TQMGKey.Held(Keys.Up) && ScrollUp < StartY) ScrollUp += 2;
        }

        static public void ComeToMe() {
            Debug.Print("Coming to the Debug Screen");
            Core.GoStage(me);
        }
    }
}





