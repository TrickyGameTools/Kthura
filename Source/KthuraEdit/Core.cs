using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Diagnostics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using TrickyUnits;
using UseJCR6;
using KthuraEdit.Stages;
using NSKthura;

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
            TQMGKey.Start(kb);
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
            CurrentStage.Update();
        }
        #endregion

        #region Shutdown
        static public void Quit() {
            MGCore.Quit = true;
        }
        #endregion

        #region Platform Detection        
        public static AltDrivePlaforms ADP {
            get {
                switch (Environment.OSVersion.Platform) {
                    case PlatformID.MacOSX: return AltDrivePlaforms.Mac;
                    case PlatformID.Unix: return AltDrivePlaforms.Linux; // Most likely the case, or else the Linux rules apply anyway
                    case PlatformID.Win32NT:
                    case PlatformID.Win32S:
                    case PlatformID.Win32Windows:
                        return AltDrivePlaforms.Windows;
                    default:
                        throw new Exception("Unknown Platform");
                }
            }
        }

        public static string Platform {
            get {
                switch (ADP) {
                    case AltDrivePlaforms.Mac: return "Mac";
                    case AltDrivePlaforms.Linux: return "Linux";
                    case AltDrivePlaforms.Windows: return "Windows";
                    default:
                        throw new Exception("If you see this error you hacked the system! Very cool, but not very nice! Fix it please!");
                        // This error instruction just had to exist as the C# is not sophisticated enough to understand that this scenario can never happen!
                }
            }
        }
        #endregion


        #region Global configuration
        static public string ConfigFile => Dirry.C("$AppSupport$/KthuraMapEditor.Config.GINI");
        static TGINI GlobalConfig = GINI.ReadFromFile(ConfigFile);
        static string GlobalWorkSpace => Dirry.AD(GlobalConfig.C($"WorkSpace.{Platform}"));
        static TJCRDIR TexJCR;
        #endregion


        #region Project Data
        private static string _prj = ""; // "Real" variable. PSST! This is very very secret!
        public static string Project {
            get => _prj;
            set {
                if (_prj != "") throw new Exception("Duplicate Project defintion!");
                Dirry.InitAltDrives(ADP);
                _prj = value;
                DBG.Log($"Opening Project: {value}");
                ProjectConfig = GINI.ReadFromFile(ProjectFile);
                if (ProjectConfig==null) {
                    Crash($"Loading project {ProjectFile} failed!");
                    return;
                }
                TexJCR = new TJCRDIR();
                foreach (string dd in ProjectConfig.List("Textures")) {
                    var d = dd.Replace("\\", "/");
                    DBG.Log($"Adding Texture Resource: {d}");
                    TexJCR.PatchFile(d);
                }
                foreach (string d in ProjectConfig.List("TEXTURESGRABFOLDERSMERGE")) {
                    var dir = FileList.GetDir(d, 1);
                    foreach (string p in dir) {
                        var path = $"{d.Replace("\\","/")}/{p}";
                        DBG.Log($"Adding Texture Resource: {path}");
                        if (JCR6.Recognize(path) != "NONE") TexJCR.PatchFile(path);
                    }
                }
            }
        }
        static string ProjectFile => $"{GlobalWorkSpace}/{_prj}/{_prj}.Project.GINI";
        public static TGINI ProjectConfig { get; private set; }
        #endregion

        #region The action map
        static public Kthura Map { get; private set; }
        private static string _map = "";
        public static string MapPath => Dirry.AD(ProjectConfig.C("MAPS"));
        public static string MapFile {
            get => _map;
            set {
                if (_map != "") throw new Exception("Duplicate mapfile definition");
                _map = value;
                if (File.Exists(FPMapFile)) {
                    DBG.Log($"Loading Map: {value}");
                    Map = Kthura.Load(value,"",TexJCR);
                } else {
                    DBG.Log($"Creating Map: {value}");
                    Map = Kthura.Create(TexJCR);
                }
                    
            }
        }
        public static string FPMapFile => $"{MapPath}/{_map}";
        #endregion
    }
}
