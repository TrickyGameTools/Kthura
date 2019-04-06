using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Scan4Projects() {

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
                    Project.D("Maps", td);
                }
                Project.SaveSource(prjfile);
                MessageBox.Show("A new project has been created!");
            } catch (Exception E) {
                Afgekeurd($"Creating a new project failed!\n\n{E.Message}");
            }

        }
    }
}
