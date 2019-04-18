// Lic:
// Kthura in C#
// Question List for Custom Items
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
// Version: 19.04.18
// EndLic

using System;
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
        void Start(string acaption ,string[] questions) {
            int y = 10;
            QA.Clear();
            Caption = UI.font32.Text(acaption);
            ShowQuestions = new List<TQA>();
            foreach (string Q in questions) {
                var TQ = new TQA {
                    caption = Q,
                    y = y,
                    capttext = UI.font20.Text(Q)
                };
                ShowQuestions.Add(TQ);
            }
        }

        public override void Draw() {
            
        }

        public override void Update() {
            if (TQMGKey.Hit(Microsoft.Xna.Framework.Input.Keys.Escape)) MainEdit.ComeToMe();
            
        }
        #endregion

    }
}

