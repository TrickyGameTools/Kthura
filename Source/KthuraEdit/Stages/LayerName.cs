using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using TrickyUnits;

namespace KthuraEdit.Stages
{
    class LayerName : BaseStage {

        readonly Dictionary<bool, TQMGText> Header = new Dictionary<bool, TQMGText>();
        bool create;
        string oud, nieuw;
        bool dontconfirm;
        TQMGText AlreadyThere = UI.font64.Text("A layer with that name already exists! Pick another please!");


        public override void Draw() {
            TQMG.Color(Color.White);
            UI.BackFull();
            TQMG.Color(0, 255, 255);
            Header[create].Draw(UI.ScrWidth / 2, 100, TQMG_TextAlign.Center);
            dontconfirm = nieuw == "";
            if (oud != nieuw && nieuw != "" && Core.Map.Layers.ContainsKey(nieuw)) {
                TQMG.Color(255, 100, 100);
                AlreadyThere.Draw(UI.ScrWidth / 2, 300, TQMG_TextAlign.Center);
                dontconfirm = true;
            }
            TQMG.Color(20, 20, 20);
            TQMG.DrawRectangle(20, 200, UI.ScrWidth - 40, 25);
            TQMG.Color(255, 255, 255);
            UI.font20.DrawText($"{nieuw}_", 22, 202);
        }

        public override void Update() {
            char ch = TQMGKey.GetChar();
            Keys k = TQMGKey.GetKey();
            // The functions above do not read out the key buffer. That happens earlier. The functions above only draw conclusions from that.
            if (ch > 32 && ch < 97 && nieuw.Length < 20) nieuw += ch;
            if (ch >= 97 && ch <= 122 && nieuw.Length < 20) nieuw += (char) (ch - 32);
            if (k == Keys.Back && nieuw != "") nieuw = qstr.Left(nieuw, nieuw.Length - 1);
            if (k == Keys.Escape) MainEdit.ComeToMe();
            if (k == Keys.Enter && !dontconfirm) {
                if (create)
                    Core.Map.CreateLayer(nieuw);
                else {
                    var tl = Core.Map.Layers[oud];
                    Core.Map.Layers.Remove(oud);
                    Core.Map.Layers[nieuw] = tl;
                    UI.LayerReset();
                }
                MainEdit.ComeToMe();

            }
        }

        #region Core init
        LayerName() {
            Header[true] = UI.font64.Text("Please enter a name for the new layer:");
            Header[false] = UI.font64.Text("Please enter a new name for the layer:");
        }
        #endregion

        #region Come to me
        readonly static LayerName me = new LayerName();
        static public void ComeToMe() { Core.GoStage(me); me.oud = ""; me.nieuw = ""; me.create = true; }
        static public void ComeToMe(string oldlayername) { Core.GoStage(me); me.oud = oldlayername; me.nieuw = oldlayername; me.create = false; }
        #endregion

    }
}
