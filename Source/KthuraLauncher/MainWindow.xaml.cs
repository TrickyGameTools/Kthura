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
using System.Text.RegularExpressions;

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

        private void Afgekeurd(string m) => MessageBox.Show(m, "Project Creation Error", MessageBoxButton.OK, MessageBoxImage.Error);
        

        private void CreateProject_Click(object sender, RoutedEventArgs e) {
            var prjallowregex = new Regex(@"^[a-zA-Z0-9_ ]+$");
            var prjname = CrPrjName.Text;
            if (!prjallowregex.IsMatch(prjname)) { Afgekeurd("Illegal characters in Project Title"); }
        }
    }
}
