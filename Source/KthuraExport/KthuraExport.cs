// Lic:
// Kthura Exporter
// Exports Kthura map into a different format
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
// Version: 19.04.23
// EndLic

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TrickyUnits;

namespace KthuraExport_NS {




    #region The stuff CSharp "demands"
    class CSharpWantsAMainClassIdontButLetsKeepCSharpHappy {
        static KthuraExport me = new KthuraExport();
        static void Main(string[] args) {
            me.Run(args);
        }
    }
    #endregion

    #region The true program!
    class KthuraExport {
        FlagParse cli_Settings;
        ConsoleColor obcl = Console.BackgroundColor;
        ConsoleColor ofcl = Console.ForegroundColor;
        string Project = "";
        string Map = "";
        string XPTo = "";
        string Target = "";
        TGINI GlobalConfig;
        string GlobalConfigFile => Dirry.C("$AppSupport$/KthuraMapEditor.Config.GINI");
        string WorkSpace => GlobalConfig.C("WorkSpace.Windows").Replace("\\", "/"); // TODO: Support other platforms too!
        TGINI ProjectConfig;
        string ProjectConfigFile => ($"{WorkSpace}/{Project}/{Project}.Project.GINI").Replace("\\","/");

        void OriCol() { Console.ForegroundColor = ofcl;Console.BackgroundColor = obcl; }

        void ColWrite(ConsoleColor c, string m) { Console.ForegroundColor = c; Console.Write(m); }

        void Red(string m) => ColWrite(ConsoleColor.Red, m);
        void Magenta(string m) => ColWrite(ConsoleColor.Magenta, m);
        void Yellow(string m) => ColWrite(ConsoleColor.Yellow, m);
        void Cyan(string m) => ColWrite(ConsoleColor.Cyan, m);
        void White(string m) => ColWrite(ConsoleColor.White, m);

        void Header() {
            MKL.Version("Kthura for C# - KthuraExport.cs","19.04.23");
            MKL.Lic    ("Kthura for C# - KthuraExport.cs","GNU General Public License 3");
            Red($"Kthura Exporter {MKL.Newest}\t");
            Magenta($"Built: {BuildDate.sBuildDate}\n");
            Yellow("Coded by: ");
            Cyan("Jeroen P. Broks\n\n");
        }

        void Uitleg() {
            Console.WriteLine("\n");
            Red("-target  "); Yellow("Target language to export to\n");
            Red("-xpto    "); Yellow("Folder to put the translated map into\n");
            Red("-project "); Yellow("Project to which the map belongs\n");
            Red("-map     "); Yellow("Map itself\n");
            Magenta("\n\nUsage: "); White("KthuraExport "); Cyan("-target "); Yellow("<target> "); Cyan("-project "); Yellow("<project> "); Cyan("-map "); Yellow("<map>\n");
            
        }

        void Error(string e) {
            Red("\n\nERROR! "); Yellow($"{e}\n");
            Console.Beep();
            OriCol();
            Environment.Exit(1);
        }

        void Assert(bool ok,string e) { if (!ok) Error(e); }
        void Assert(string ok, string e) => Assert(ok.Length > 0, e);
        void Assert(int ok, string e) => Assert(ok != 0, e);
        void Assert(TGINI ok, string e) => Assert(ok != null, e);

        void Doing(string a, string b) { Yellow($"{a}: "); Cyan($"{b}\n"); }

        void Init(string[] args) {
            if (args.Length == 0) {
                Uitleg();
                OriCol();
                Environment.Exit(0);
            }
            InitJCR6.Go();
            Dirry.InitAltDrives(AltDrivePlaforms.Windows); // TODO: I may need to expand this later for Linux and Mac.
            cli_Settings = new FlagParse(args);
            cli_Settings.CrString("target");
            cli_Settings.CrString("project");
            cli_Settings.CrString("map");
            cli_Settings.CrString("xpto");
            if (!cli_Settings.Parse()) {
                Uitleg();
                Error("Parsing command line input failed!");
            }
            Project = cli_Settings.GetString("project");
            Map = cli_Settings.GetString("map");
            XPTo = cli_Settings.GetString("xpto");
            Target = cli_Settings.GetString("target");
            Assert(File.Exists(GlobalConfigFile), $"I cannot find {GlobalConfigFile}");
            GlobalConfig = GINI.ReadFromFile(GlobalConfigFile );
            Assert(GlobalConfig != null, "Global config could nt be properly loaded");
            Assert(Project, "Hey! I don't have a project!");
            Assert(WorkSpace, "I can't find out what the workspace is. Is Kthura properly configured?");
            Assert(File.Exists(ProjectConfigFile), $"I could not access {ProjectConfigFile}. It appears it doesn't exist!");
            Doing("Reading project", ProjectConfigFile);
            ProjectConfig = GINI.ReadFromFile(ProjectConfigFile);
            Assert(ProjectConfig, "Project could not be properly read.");
            Target = ProjectConfig.C("EXPORT.TARGET"); if (Target == "") Target = cli_Settings.GetString("target");
            Assert(Target, "No target");
            Doing("Exporting to", Target);
            Assert(ExportBasis.HaveDriver(Target), $"Driver to export to {Target} has not been found!");
        }

        internal void Run(string[] args) {
            Header();
            Init(args);
        }
        #endregion
    }
}

