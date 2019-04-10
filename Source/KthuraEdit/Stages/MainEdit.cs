// Lic:
// Kthura for C#
// Editor core
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

namespace KthuraEdit.Stages
{
    class MainEdit : BaseStage {
        static MainEdit me = new MainEdit();
        public override void Draw() { 
            UI.DrawScreen();            
        }

        public override void Update() {
            UI.UI_Update();
        }

        public static void ComeToMe() => Core.GoStage(me);
    }
}

