using System;
using System.Collections.Generic;
using System.Linq;
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

        static public string ConfigFile => Dirry.C("$AppSupport$/KthuraMapEditor.Config.GINI");
        static TGINI Config = GINI.ReadFromFile(ConfigFile,true);
        static public string WorkSpace {
            get => Config.C($"WorkSpace.{Platform}");
            set {
                Config.D($"WorkSpace.{Platform}", value);
                Config.SaveSource(ConfigFile);
            }
        }

        static MainConfig() {
            Debug.WriteLine($"System detected as {Platform}/{ADP}/{Environment.OSVersion.Platform}");
            Dirry.InitAltDrives(ADP);            
            
        }

    }
}
