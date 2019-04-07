using System;
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
