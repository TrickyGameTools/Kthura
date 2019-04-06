using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Text.RegularExpressions;
using TrickyUnits;

namespace Kthura
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        static public string MyExe => System.Reflection.Assembly.GetEntryAssembly().Location;

        #region Init Window
        public MainWindow()
        {
            InitializeComponent();
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
            var parameters = $"\"{LstProjects.SelectedItem.ToString()}\" ";
            if (LstMaps.SelectedItem.ToString() == "** New Map **")
                parameters += $"\"{NewMap.Text}\"";
            else
                parameters += $"\"{LstMaps.SelectedItem.ToString()}\"";
            try {
                var editor = $"{qstr.ExtractDir(MyExe)}/KthuraEdit.exe";
                Process.Start(editor, parameters);
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
            StartTheEditor.IsEnabled = canstart;
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
                Debug.WriteLine($"Scanning projecT: {prj}");
                var prjfile = $"{MainConfig.WorkSpace}/{prj}/{prj}.Project.GINI";
                TGINI Project = GINI.ReadFromFile(prjfile);
                if (Project==null) { MessageBox.Show($"Reading {prjfile} failed!", "Project scanning errorr", MessageBoxButton.OK, MessageBoxImage.Error); return; }
                var maps = FileList.GetDir(Project.C("Maps"));
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
