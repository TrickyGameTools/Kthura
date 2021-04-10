// Lic:
// Kthura for C#
// Main Window and all its callbacks (Launcher)
// 
// 
// 
// (c) Jeroen P. Broks, 2019, 2021
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
// Version: 21.04.10
// EndLic

#define GINIE_Project


using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using System.Text.RegularExpressions;
using TrickyUnits;

namespace Kthura {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool NieuwSysteem = false;

        static public string MyExe => System.Reflection.Assembly.GetEntryAssembly().Location;

        #region Init Window
        public MainWindow()
        {
            InitializeComponent();
            CShell.Init(this,C_Command,C_Status,C_Output);
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            BDate.Content = $"Build date: {BuildDate.sBuildDate}";
            if (MainConfig.WorkSpace == "") {
                MessageBox.Show("Workspace has not yet been set.\nPlease select a folder that I can use as Workspace (the folder where your Kthura project data will be stored)!");
                using (var dialog = new System.Windows.Forms.FolderBrowserDialog()) {
                    System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                    if (result == System.Windows.Forms.DialogResult.OK) {
                        MainConfig.WorkSpace = dialog.SelectedPath;
                    } else {
                        Environment.Exit(1);
                    }
                }
            }
            Scan4Projects();
        }
        #endregion

        #region GUI Callbacks

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Afgekeurd(string m) => MessageBox.Show(m, "Project Creation Error", MessageBoxButton.OK, MessageBoxImage.Error);
        

        private void CreateProject_Click(object sender, RoutedEventArgs e) {            
            Dictionary<bool, string> TexField = new Dictionary<bool, string>();
            TexField[false] = "Textures";
            TexField[true] = "TexturesGrabFoldersMerge";
            var prjallowregex = new Regex(@"^[a-zA-Z0-9_ ]+$");
            var prjname = CrPrjName.Text;
            if (prjname.Trim()=="") { Afgekeurd("No Project Title"); return; }
            if (!prjallowregex.IsMatch(prjname)) { Afgekeurd("Illegal characters in Project Title"); return; }
            var prjdir = $"{MainConfig.WorkSpace}/{prjname}";
            var prjfile = $"{prjdir}/{prjname}.Project.GINI";
            var prjmeta = CrPrjMeta.Text.Split(';');
            var prjtexmerge = qstr.Prefixed(CrPrjTextureFolders.Text, "@MERGE@");
            var prjtex = CrPrjTextureFolders.Text.Split(';');
            if (prjtexmerge) prjtex = qstr.RemPrefix(CrPrjTextureFolders.Text, "@MERGE@").Split(';');
            if (CrPrjTextureFolders.Text == "*InProject*") { prjtex = new string[] { $"{prjdir}/Textures/" }; }
#if GINIE_Project
            var Paths = $"Paths.{MainConfig.Platform}";
            GINIE OutPrj = GINIE.FromSource($"[meta]\nCreated={DateTime.Now}\n");
            prjfile = $"{prjdir}/{prjname}.Project.INI";
            if (File.Exists(prjdir)) {
                Afgekeurd($"Hey!\nThere is a file named {prjdir}!\n\nRemove it first please (files do not belong in the workspace root)!");
                return;
            }
            if (Directory.Exists(prjdir)) {
                Afgekeurd("There already appears to be a project directory with that name.\nEither remove or rename that project, or pick a different name for this project!");
                return;
            }
            Directory.CreateDirectory(prjdir);
            OutPrj.AutoSaveSource = prjfile;
            try {
                OutPrj["Meta", "Project"] = prjname;
                OutPrj.List("Map", "Special");
                if (CrPrjMapFolder.Text == "*InProject*") {
                    var td = $"{prjdir}/Maps";
                    Directory.CreateDirectory(td);
                    OutPrj[Paths, "Maps"] = td;
                } else
                    OutPrj[Paths, "Maps"] = CrPrjMapFolder.Text.Replace("\\", "/");
                OutPrj.List("Map", "GeneralData");
                foreach (string m in prjmeta) OutPrj.List("Map", "GeneralData").Add(m.Trim());
                if (prjtexmerge) 
                    OutPrj[Paths, "TexMerge"] = "YES";
                else 
                    OutPrj[Paths, "TexMerge"] = "NO";
                if (CrPrjTextureFolders.Text == "*InProject*") {
                    Directory.CreateDirectory($"{prjdir}/Textures/");
                    OutPrj.List(Paths, "Textures").Add($"{prjdir}/Textures");
                } else {
                    foreach (string f in prjtex) OutPrj.List(Paths,"Textures").Add(f.Trim());
                }
            } catch (Exception E) {
                Afgekeurd($"Creating a new project failed!\n\n{E.Message}");
            }
#else
            TGINI Project = new TGINI();
            Project.D("Project", prjname);
            Project.D("Maps", CrPrjMapFolder.Text);
            Project.CL("GeneralData");
            foreach (string m in prjmeta) Project.List("GeneralData").Add(m.Trim());
            Project.CL(TexField[prjtexmerge]);
            foreach (string f in prjtex) Project.List(TexField[prjtexmerge]).Add(f.Trim());
            if (File.Exists(prjdir)) {
                Afgekeurd($"Hey!\nThere is a file named {prjdir}!\n\nRemove it first please (files do not belong in the workspace root)!");
                return;
            }
            if (Directory.Exists(prjdir)) {
                Afgekeurd("There already appears to be a project directory with that name.\nEither remove or rename that project, or pick a different name for this project!");
                return;
            }
            try {
                Directory.CreateDirectory(prjdir);
                if (CrPrjTextureFolders.Text == "*InProject*") { Directory.CreateDirectory( $"{prjdir}/Textures/"); }
                if (CrPrjMapFolder.Text == "*InProject*") {
                    var td = $"{prjdir}/Maps";
                    Directory.CreateDirectory(td);
                    Project.D("Maps", td.Replace("\\","/"));
                }
                Project.SaveSource(prjfile);
                MessageBox.Show("A new project has been created!");
                Scan4Projects();
            } catch (Exception E) {
                Afgekeurd($"Creating a new project failed!\n\n{E.Message}");
            }
#endif
        }

        private void LstProjects_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            AutoEnable();
            Scan4Maps();
        }

        private void LstMaps_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            AutoEnable();
        }

        private void NewMap_TextChanged(object sender, TextChangedEventArgs e) => AutoEnable();


        private void StartTheEditor_Click(object sender, RoutedEventArgs e) {            
            if (!StartTheEditor.IsEnabled) return;
            var parameters = $"-FORCEWINDOWED YES \"{LstProjects.SelectedItem.ToString()}\" ";
            if (LstMaps.SelectedItem.ToString() == "** New Map **")
                parameters += $"\"{NewMap.Text}\"";
            else
                parameters += $"\"{LstMaps.SelectedItem.ToString()}\"";
            try {
                var editor = $"{qstr.ExtractDir(MyExe)}/KthuraEdit.exe";
                //Process.Start(editor, parameters); // This will need to be done differently!
                CShell.Start(editor, parameters);
            } catch (Exception err) {
                Debug.WriteLine($"Launching the editor failed!\n{err.Message}");
                MessageBox.Show($"Launching the editor failed!\n{err.Message}");
            }
        }

        #endregion

        #region AutoSet
        void AutoEnable() {
            //var prj = LstProjects.SelectedItem.ToString();
            Dictionary<bool, Visibility> b2v = new Dictionary<bool, Visibility>();
            b2v[false] = Visibility.Hidden;
            b2v[true] = Visibility.Visible;
            LstMaps.IsEnabled = LstProjects.SelectedItem != null; 
            LstMaps.Visibility=b2v[LstProjects.SelectedItem != null];
            LabelMaps.Visibility = LstMaps.Visibility;
            NewMap.Visibility=b2v[LstProjects.SelectedItem != null && LstMaps.SelectedItem != null && LstMaps.SelectedItem.ToString() == "** New Map **"];
            LabelNewMap.Visibility = NewMap.Visibility;
            var canstart = LstProjects.SelectedItem != null;
            canstart = canstart && LstProjects.SelectedItem != null;
            canstart = canstart && LstMaps.SelectedItem != null;
            canstart = canstart && (LstMaps.SelectedItem.ToString() != "** New Map **" || NewMap.Text != "");
            StartTheEditor.IsEnabled = canstart && NieuwSysteem;
            StartTheEditor.Visibility = b2v[canstart];
        }
        #endregion

        #region General functionality
        private void Scan4Projects() {
            LstProjects.Items.Clear();
            foreach (string p in FileList.GetDir(MainConfig.WorkSpace, 2)) {
                LstProjects.Items.Add(p);
            }
            AutoEnable();
        }

        void Scan4Maps() {
            if (LstProjects.SelectedItem == null) return; // Crash prevention
            try {
                var prj = LstProjects.SelectedItem.ToString();
                Debug.WriteLine($"Scanning project: {prj}");
                var projectfile = $"{MainConfig.WorkSpace}/{prj}/{prj}.Project.ini";
                var prjfile = $"{MainConfig.WorkSpace}/{prj}/{prj}.Project.GINI";
                string[] maps = null;
                if (File.Exists(projectfile)) {
                    GINIE PRJ = GINIE.FromFile(projectfile);
                    maps = FileList.GetDir(PRJ[$"Paths.{MainConfig.Platform}", "Maps"]);
                    StartTheEditor.IsEnabled = true;
                    NieuwSysteem = true;
                } else if (File.Exists(prjfile)) {
                    TGINI Project = GINI.ReadFromFile(prjfile);
                    if (Project == null) { MessageBox.Show($"Reading {prjfile} failed!", "Project scanning error", MessageBoxButton.OK, MessageBoxImage.Error); return; }
                    maps = FileList.GetDir(Project.C("Maps"));
                    StartTheEditor.IsEnabled = false;
                    NieuwSysteem = false;
                }
                LstMaps.Items.Clear();
                LstMaps.Items.Add("** New Map **");
                foreach (string m in maps) LstMaps.Items.Add(m);
            } catch (Exception E) {
                MessageBox.Show(E.Message, "Project scanning errorr", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion
    }
}