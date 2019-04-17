// Lic:
// Kthura for C#
// Core
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
// Version: 19.04.17
// EndLic







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
using NLua;

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
            DBG.Log($"ERROR!\n{Message}\n\nHit Escape to exit this program");
            dontsave = true;
            DBG.TimeToCrash = true;
            DBG.ComeToMe();
        }

        public static void Crash(Exception e) {
            Crash($"{e.Message}\n\nTraceback:\n{e.StackTrace}\n\nIt's likely you encountered a bug. Please report this!\n\nHit Escape to exit this program");
        }
        #endregion

        #region Input State
        static public KeyboardState kb { get; private set; }
        static public MouseState ms { get; private set; }
        static public JoystickState joy { get; private set; }
        static MouseState oldms = new MouseState(0, 0, 0, ButtonState.Pressed, ButtonState.Pressed, ButtonState.Pressed, ButtonState.Pressed, ButtonState.Pressed);
        static TQMGImage MousePointer;
        static public void UpdateStates() {
            oldms = ms;
            kb = Keyboard.GetState();
            ms = Mouse.GetState();
            joy = Joystick.GetState(0);
            TQMGKey.Start(kb);
            DontMouse = DontMouse && ms.LeftButton == ButtonState.Released && ms.RightButton == ButtonState.Released;
        }
        static public bool DontMouse = false;
        static public void ShowMouse() { TQMG.Color(255, 255, 255); MousePointer.Draw(ms.X, ms.Y); }
        static public bool MsHit(byte b) {
            switch (b) {
                case 1:
                    return (!DontMouse) && oldms.LeftButton == ButtonState.Released && ms.LeftButton == ButtonState.Pressed;
                case 2:
                    return (!DontMouse) && oldms.RightButton == ButtonState.Released && ms.RightButton == ButtonState.Pressed;
                default:
                    throw new Exception($"Core.MsHit: Unknown mouse button required: {b}");
            }
        }
        static public bool MsDown(byte b) {
            switch (b) {
                case 1:
                    return (!DontMouse) && ms.LeftButton == ButtonState.Pressed;
                case 2:
                    return (!DontMouse) &&  ms.RightButton == ButtonState.Pressed;
                default:
                    throw new Exception($"Core.MsDown: Unknown mouse button required: {b}");
            }
        }
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

        #region Lua init
        static public Lua Script { get; private set; } = new Lua();
        static Lua_API LAPI = new Lua_API();
        static public void InitLua() {
            try {
                var basescript = JCR.LoadString("Script/BasisScript.lua");
                var scriptfile = ($"{GlobalWorkSpace}/{Project}/{Project}.Script.lua").Replace("\\","/");
                var tscript = basescript;
                DBG.Log("- Setting up Lua");
                DBG.Log("  = Setting up API");
                Script["Kthura"] = LAPI;
                DBG.Log("  = Compiling Main Script");
                Script.DoString(tscript, "Kthura Script");
                if (File.Exists(scriptfile)) {
                    DBG.Log($"  = Loading {scriptfile}");
                    //var impscript = QuickStream.LoadString(scriptfile);
                    DBG.Log($"  = Compiling {scriptfile}");
                    //tscript = tscript.Replace("--[[CONTENT]]", impscript);
                    Script.DoFile(scriptfile);
                    Script.DoString("function NOTHING() end\n;(init or NOTHING)()", "Kthura Init");
                }

            } catch (Exception e) {
                DBG.Log($"   = ERROR: {e.Message}");
                DBG.Log("An error popped up during setting up Lua.\nPlease note that placing custom spots may not work properly now!");
            }
        }
        #endregion

        #region Global configuration
        static public string ConfigFile => Dirry.C("$AppSupport$/KthuraMapEditor.Config.GINI");
        static TGINI GlobalConfig = GINI.ReadFromFile(ConfigFile);
        static public string GlobalWorkSpace => Dirry.AD(GlobalConfig.C($"WorkSpace.{Platform}"));
        static TJCRDIR TexJCR;
        #endregion

        #region Save
        static public bool dontsave = false;
        public static void Save() {
            if (dontsave) return;
            var storage = ProjectConfig.C("Compression").Trim();
            if (storage == "") storage = "lzma";
            DBG.Log($"Saving {MapFile}, storage method: {storage}");

            try {
                KthuraSave.Save(Map, $"{MapPath}/{MapFile}", storage);
            } catch (Exception e) {
                DBG.Log($"ERROR Saving failed!\n{e.Message}\n{JCR6.JERROR}");
            }
            UI.SaveTexMemory();
        }
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

        #region The actual map
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
                    try {
                        if (TexJCR == null) DBG.Log("HEY HEY HEY! Texture JCR is null! Should not be possible!");
                        Map = Kthura.Load(FPMapFile, "", TexJCR);
                    } catch (Exception e) {
                        Crash($"Loading Kthura map failed!\n{e.Message}\n{JCR6.JERROR}");
                        return;
                    }
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







