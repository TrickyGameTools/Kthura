// This is only a quick tool to make sure all maps of my ongoing projects are ready for the new editor!

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrickyUnits;
using UseJCR6;

namespace KthuraConvert {
    class Program {
        readonly string Project;
        string GlobConfigFile => Dirry.C("$AppSupport$/KthuraMapEditor.Config.Ini");
        GINIE GlobConfig = null;
        string WorkSpace => GlobConfig["Windows", "WorkSpace"]; // Sorry, no multi-platform for just a quick routine.
        string OldFile => $"{WorkSpace}/{Project}/{Project}.Project.GINI";
        TGINI Old = null;
        GINIE Nieuw;
        void Print(params string[] a) {foreach(var ba in a) Console.Write(ba); Console.WriteLine(); }

        Program(string p) { Project = p; }

        void KthuraCheck(string map) {
            var mapfile = $"{Old["Maps"]}/{map}";
            var modified = false;
            Print($"Checking Kthura map: {map}");
            Print($"= File: {mapfile}");
            var jd = JCR6.Dir(mapfile);
            foreach(var e in jd.Entries.Values) {
                if (e.Storage=="lzma") {
                    modified = true;
                    Print($"  = Entry {e.Entry} was packed with lzma, which is not (yet) supported in the new Kthura. So a repack is in order");
                }
            }
            //if (modified) {
            if (true) { 
                Print("= Repacking in zlib");
                var ed = new SortedDictionary<string, byte[]>();
                foreach (var e in jd.Entries.Values) {
                    Print($"  = Reading: {e.Entry}");
                    ed[e.Entry] = jd.JCR_B(e.Entry);
                }
                var st= $"[IGNORECASE]\nLABELS=NO\nTAGS=NO\n\n[Save]\nStorage=zlib\n";
                ed["Options"] = new byte[st.Length];
                for (int i = 0; i < st.Length; ++i) ed["Options"][i] = (byte)st[i];
                Print("= Saving new map");
                var jc = new TJCRCreate(mapfile,"zlib");
                foreach(var e in ed) {
                    Print($"  = Writing: {e.Key}");
                    jc.AddBytes(e.Value, e.Key, "zlib");
                }
                Print("= Finalizing");
                jc.Close();
            } else {
                Print("= Map appears to be in order. No changes needed!");
            }
        }

        void Run() {
            Console.WriteLine("Kthura Convert - Coded by Jeroen P. Broks");
            Console.WriteLine($"Project: {Project}");
            Print("Loading Global Config");
            GlobConfig = GINIE.FromFile(GlobConfigFile);
            Print("Workspace: ", WorkSpace);
            Print("Loading old project: ", OldFile);
            Old = GINI.ReadFromFile(OldFile);
            Print("Data for new project creation");
            Nieuw = GINIE.FromSource($"[CONVERT]\nConvertData={DateTime.Now}");
            Nieuw.AutoSaveSource = OldFile.Replace(".GINI", ".ini");
            Print("- Meta data");
            Nieuw["Meta", "CREATED"] = $"{DateTime.Now} -- By conversion of an old project";
            Nieuw["Meta", "PROJECT"] = Old["Project"];
            Print("- Meta data tags for map");
            foreach (var k in Old.List("GeneralData")) Nieuw.List("Map", "GeneralData").Add(k);
            Print("- Tex convert");
            Nieuw["PATHS.WINDOWS", "MAPS"] = Old["Maps"];
            foreach (var i in Old.List("TEXTURESGRABFOLDERSMERGE")) Nieuw.List("Paths.Windows", "TexMaps").Add(i); 
            Nieuw["PATHS.WINDOWS", "TEXMERGE"] = "YES";
            //TexDir = E:/ Projects / Maps / Kthura / cpp_kthproject / Textures
            Print("- Custom stuff");
            foreach (var c in Old.List("CSpots")) Nieuw.List("Map", "Special").Add(c);
            Print();
            foreach (var mp in FileList.GetTree(Old["Maps"])) KthuraCheck(mp);

            // Last
            Nieuw["Meta", "CONVERTFINISH"] = $"{DateTime.Now}";
        }


        static void Main(string[] args) {
            Dirry.InitAltDrives();
            JCR6_lzma.Init();
            JCR6_zlib.Init();
            if (args.Length != 1) {
                Console.WriteLine("Usage: KthuraConvert <Project>");
            } else {
                var M = new Program(args[0]);
                M.Run();
            }
            TrickyDebug.AttachWait();
        }
    }
}
