using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using TrickyUnits;
using UseJCR6;
using KthuraEdit.Stages;

namespace KthuraEdit
{
    class Core {
        #region Init & Core config
        static public string MyExe => System.Reflection.Assembly.GetEntryAssembly().Location;
        static Kthura_EditCore TrueCore;
        static GraphicsDeviceManager GDM;
        static GraphicsDevice GD;
        static SpriteBatch SB;
        static public Kthura_EditCore MGCore { get; private set; }

        static TJCRDIR JCR;
        static Core() {
            JCR6_lzma.Init();
            JCR6_zlib.Init();
            JCR6_jxsrcca.Init();
            new JCR6_RealDir();
#if DEBUG
            JCR = JCR6.Dir($"E:/Projects/Applications/VisualStudio/Kthura/Releases/KthuraEdit.jcr"); // This is where my jcr file lives... Silly me...
#else
            JCR = JCR6.Dir($"{qstr.ExtractDir(MyExe)}/KthuraEdit.jcr");
#endif
        }
        public static void CoreInit(Kthura_EditCore TC) => TrueCore = TC;
        public static void Start(GraphicsDeviceManager _GDM, GraphicsDevice _GD, SpriteBatch SB, Kthura_EditCore core) {
            GDM = _GDM;
            GD = _GD;
            MGCore = core;
        }
        public static void StartStep3(SpriteBatch _SB) {
            SB = _SB;
            if (JCR == null) Crash($"JCR Null: {JCR6.JERROR}");
            TQMG.Init(GDM, GD, SB, JCR);
            MousePointer = TQMG.GetImage("MousePointer.png");
            MainEdit.ComeToMe();
        }
        #endregion

        #region Error Handling
        public static void Crash(string Message) {
            Debug.WriteLine($"ERROR!\n{Message} ");
        }

        public static void Crash(Exception e) {
            Crash($"{e.Message}\n\nTraceback:\n{e.StackTrace}\n\nIt's likely you encountered a bug. Please report this!");
        }
        #endregion

        #region Input State
        static public KeyboardState kb { get; private set; }
        static public MouseState ms { get; private set; }
        static public JoystickState joy { get; private set; }
        static TQMGImage MousePointer;
        static public void UpdateStates() {
            kb = Keyboard.GetState();
            ms = Mouse.GetState();
            joy = Joystick.GetState(0);
        }
        static public void ShowMouse() { TQMG.Color(255, 255, 255); MousePointer.Draw(ms.X, ms.Y); }
        #endregion

        #region Flow State
        static BaseStage CurrentStage;
        public static void GoStage(BaseStage stage) => CurrentStage = stage;

        static public void PerformDraw()  {
            try {
                CurrentStage.Draw();
                ShowMouse();
            } catch (Exception EX) { Crash(EX); }
        }

        static public void PerformUpdate() {
            UpdateStates();
        }
        #endregion
    }
}
