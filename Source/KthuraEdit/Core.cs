using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using TrickyUnits;
using UseJCR6;

namespace KthuraEdit
{
    class Core {
        #region Init & Core config
        static public string MyExe => System.Reflection.Assembly.GetEntryAssembly().Location;
        static Kthura_EditCore TrueCore;
        static GraphicsDeviceManager GDM;
        static GraphicsDevice GD;
        static TJCRDIR JCR = JCR6.Dir($"{qstr.ExtractDir(MyExe)}/KthuraEdit.jcr");
        public static void CoreInit(Kthura_EditCore TC) => TrueCore = TC;
        public static void Start(GraphicsDeviceManager _GDM, GraphicsDevice _GD, SpriteBatch SB) {
            GDM = _GDM;
            GD = _GD;
            
        }
        public static void StartStep3(SpriteBatch SB) {
            TQMG.Init(GDM, GD, SB, JCR);
        }
        #endregion

        #region State
        static public KeyboardState kb { get; private set; }
        static public MouseState ms { get; private set; }
        static public JoystickState joy { get; private set; }
        static public void UpdateStates() {
            kb = Keyboard.GetState();
            ms = Mouse.GetState();
            joy = Joystick.GetState(0);
        }
        #endregion
    }
}
