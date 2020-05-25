// Lic:
// Kthura in C#
// Question List for Custom Items
// 
// 
// 
// (c) Jeroen P. Broks, 2019
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
// Version: 20.05.25
// EndLic




using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using KthuraEdit;
using TrickyUnits;


namespace KthuraEdit.Stages
{
    class QuestionList:BaseStage {

        #region static links
        static QuestionList me = new QuestionList();
        static public void ComeToMe(string caption, string[] questions, params UISchedule[] scedfunc) {
            Debug.WriteLine($"Question List ComeToMeRequest with {questions.Length} field(s)");
            foreach (UISchedule UIS in scedfunc)
                UI.Schedule(UIS, me.QA);
            if (questions.Length>0)    
                me.Start(caption,questions);
        }
        #endregion

        #region True Class
        internal struct TQA {
            internal string caption;
            internal TQMGText capttext;
            internal int y;
        }

        Dictionary<string, string> QA = new Dictionary<string, string>();
        TQMGText Caption;
        List<TQA> ShowQuestions;
        string curfield = "";
        void Start(string acaption ,string[] questions) {
            int y = 40;
            QA.Clear();
            Caption = UI.font32.Text(acaption);
            ShowQuestions = new List<TQA>();
            foreach (string Q in questions) {
                var Question = Q;
                var eq = Q.IndexOf('=');

                if (eq >= 0) {
                    Question = Q.Substring(0, eq);
                    QA[Question] = Q.Substring(eq + 1);
                } else
                    QA[Question] = "";
                var TQ = new TQA {
                    caption = Question,
                    y = y,
                    capttext = UI.font20.Text(Question)
                };
                y += 25;
                ShowQuestions.Add(TQ);
                
            }
            Core.GoStage(me);
        }

        public override void Draw() {
            var cursor = " ";
            var gotfield = false;
            UI.BackFull();
            TQMG.Color(255, 180, 0);
            Caption.Draw(UI.ScrWidth / 2, 5, TQMG_TextAlign.Center);
            foreach (TQA TQ in ShowQuestions) {
                TQMG.Color(255, 255, 255);
                TQ.capttext.Draw(10, TQ.y);
                if (curfield == "") curfield = TQ.caption;
                if (curfield == TQ.caption) {
                    gotfield = true;
                    var nu = DateTime.Now.Second;
                    if (nu % 2 == 0) cursor = "_";
                    TQMG.Color(0, 255, 255);
                    TQMG.DrawRectangle(300, TQ.y, UI.ScrWidth - 350, 23);
                    TQMG.Color(0, 0, 0);
                    UI.font20.DrawText($"{QA[TQ.caption]}{cursor}", 302, TQ.y);
                } else {
                    TQMG.Color(0, 25, 25);
                    TQMG.DrawRectangle(300, TQ.y, UI.ScrWidth - 350, 23);
                    TQMG.Color(0, 255, 255);
                    UI.font20.DrawText($"{QA[TQ.caption]}", 302, TQ.y);
                    if (Core.MsHit(1) && (Core.ms.Y > TQ.y && Core.ms.Y < TQ.y + 24)) curfield = TQ.caption;
                }
            }
            if (!gotfield) curfield = "";
        }

        public override void Update() {
            if (TQMGKey.Hit(Microsoft.Xna.Framework.Input.Keys.Escape)) MainEdit.ComeToMe();
            if (curfield!="") {
                var ch = TQMGKey.GetChar();
                if (ch >= 32 && ch < 127 && UI.font20.TextWidth(QA[curfield]) < UI.ScrWidth-360) QA[curfield] += ch;
                if (TQMGKey.Hit(Microsoft.Xna.Framework.Input.Keys.Back) && QA[curfield] != "") QA[curfield] = qstr.Left(QA[curfield], QA[curfield].Length - 1);
                if (TQMGKey.Hit(Microsoft.Xna.Framework.Input.Keys.Tab)) {
                    var gotofield = "";
                    var firstfield = "";
                    var lastfield = "";
                    foreach (TQA TQ in ShowQuestions) {
                        if (firstfield == "") firstfield = TQ.caption;
                        if (lastfield == curfield) gotofield = TQ.caption;
                        lastfield = TQ.caption;
                    }
                    if (gotofield == "") {
                        if (firstfield != "")
                            curfield = firstfield;
                    } else
                        curfield = gotofield;
                }
            }
        }
        #endregion

    }
}