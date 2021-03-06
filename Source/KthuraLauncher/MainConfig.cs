// Lic:
// Kthura for C#
// Main Config for Launcher
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
// Version: 21.04.03
// EndLic


using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Diagnostics;
using TrickyUnits;


namespace Kthura
{
    static class MainConfig {

        /*
        public static bool IsLinux {
            get {
                int p = (int)Environment.OSVersion.Platform;
                return (p == 4) || (p == 6) || (p == 128);
            }
        }
        */

        #region Platform detection
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

        static public string ConfigFile => Dirry.C("$AppSupport$/KthuraMapEditor.Config.INI");
        static public string ConfigFileOld => Dirry.C("$AppSupport$/KthuraMapEditor.Config.GINI"); // GINI is deprecated and most stuff I have where it matters it will be replaced with GINIE.
        //static TGINI Config = GINI.ReadFromFile(ConfigFile,true);
        static GINIE Config = null;
        static public string WorkSpace {
            /*
            get => Config.C($"WorkSpace.{Platform}");
            set {
                Config.D($"WorkSpace.{Platform}", value);
                Config.SaveSource(ConfigFile);
            }
            */
            get => Config[Platform, "Workspace"];
            set { Config[Platform, "Workspace"] = value.Replace("\\", "/"); }
        }

        static MainConfig() {
            Debug.WriteLine($"System detected as {Platform}/{ADP}/{Environment.OSVersion.Platform}");
            Dirry.InitAltDrives(ADP);
            Config = GINIE.FromFile(ConfigFile);
            Config.AutoSaveSource=ConfigFile;
            if ((!File.Exists(ConfigFile))  && File.Exists(ConfigFileOld)) {
                var OCFG = GINI.ReadFromFile(ConfigFileOld);
                Config[Platform, "Workspace"] = OCFG.C($"WorkSpace.{Platform}").Replace("\\", "/");
                Config[Platform, "Imported"] = $"This data was imported on {DateTime.Now}. GINI is deprecated and will be discontinued. -- This filed is just a message and may be removed if you like.";
                Debug.WriteLine($"GINI config converted to GINIE");
            }
        }

    }
}