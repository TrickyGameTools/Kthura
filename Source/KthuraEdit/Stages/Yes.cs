// Lic:
// Kthura for C#
// Yes
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
// Version: 19.04.10
// EndLic

ï»¿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrickyUnits;
using Microsoft.Xna.Framework.Input;


namespace KthuraEdit.Stages
{

    delegate void IfYes(params string[] arg);

    class Yes:BaseStage {

        static Yes Me = new Yes();
        IfYes Callback;
        TQMGText Question;
        string[] CallbackArgs;
        TQMGImage B_Yes = TQMG.GetImage("Yes.png");
        TQMGImage B_No = TQMG.GetImage("No.png");
        readonly int Kwartje = UI.ScrWidth / 4;

        void DoYes() {
            MainEdit.ComeToMe();
            Callback(CallbackArgs);
        }

        void DoNo() => MainEdit.ComeToMe();
        
        public override void Draw() {
            TQMG.Color(255, 255, 255);
            UI.BackFull();            
            TQMG.Color(0, 255, 255);
            Question.Draw(UI.ScrWidth / 2, 100);
            TQMG.Color(255, 255, 255);
            B_Yes.Draw(Kwartje - (B_Yes.Width / 2), 200);
            B_No.Draw((Kwartje * 3) - (B_No.Width / 2), 200);
        }

        public override void Update() {
            bool Links = Core.MsHit(1); // "Links" means "Left" in Dutch. For the LEFT MOUSE BUTTON!
            Keys ch = TQMGKey.GetKey();
            if (Core.ms.Y>200 && Core.ms.Y<200+B_Yes.Height && Links) {
                if (Core.ms.X > Kwartje - (B_Yes.Width / 2) && Core.ms.X < Kwartje + Kwartje - (B_Yes.Width / 2)) ch = Keys.Y;
                if (Core.ms.X > (Kwartje * 3) - (B_No.Width / 2) && (Core.ms.X * 3) < Kwartje + Kwartje - (B_No.Width / 2)) ch = Keys.N;
            }
            switch (ch) {
                case Keys.Y:
                    DoYes();
                    break;
                case Keys.N:
                case Keys.Escape:
                    DoNo();
                    break;
            }
        }

        static public void ComeToMe(string Question, IfYes Callback, params string[] CallBackargs) {
            try {
                Me.Callback = Callback;
                Me.Question = UI.font64.Text(Question);
                if (Me.Question.Width > UI.ScrWidth - 20) Me.Question = UI.font32.Text(Question);
                if (Me.Question.Width > UI.ScrWidth - 20) Me.Question = UI.font20.Text(Question);
                if (Me.Question.Width > UI.ScrWidth - 20) Me.Question = UI.font16.Text(Question);
                Me.CallbackArgs = CallBackargs;
                Core.GoStage(Me);
            } catch (Exception e){
                DBG.Log($"INTERNAL ERROR!!! (Please report this)\n{e.Message}\n\nTraceback:\n{e.StackTrace}");
            }
        }
    }
}

