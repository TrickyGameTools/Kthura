// Lic:
// Kthura Launcher
// Shell Manager
// 
// 
// 
// (c) Jeroen P. Broks, 2021
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
using System;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Threading;
using TrickyUnits;

namespace Kthura {
    static class CShell {
        #region Variables
        static private MainWindow Win = null;
        static private TextBox
            Comm = null,
            Stat = null,
            Outp = null;
        static private bool Running = false;
        static Process Pr = null;
        #endregion

        static public void Init(MainWindow MW, TextBox CComm, TextBox CStat, TextBox COutp) {
            Win = MW;
            Comm = CComm;
            Stat = CStat;
            Outp = COutp;
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(1);
            timer.Tick += timer_Tick;
            timer.Start();
        }

        static private void timer_Tick(object sender, EventArgs e) {
            if (!Running) return;
            if (Pr == null) { Stat.Text = "ERROR! Tried to run process while process is null (internal error, please report!)"; return; }
            if (Pr.StandardOutput.EndOfStream) {
                Pr.WaitForExit();
                if (Pr.ExitCode == 0)
                    Stat.Text = "Process ended";
                else
                    Stat.Text = $"Process ended with exit code {Pr.ExitCode}";
                Pr.Close();
                Running = false;
            } else {
                var p = Pr.StandardOutput.ReadLine();                
                Outp.Text += $"{p}\n";
            }
        }

        static public void Start(string Exe, string Arg="") {
            if (Running) {
                Confirm.Annoy("There is already a process running!");
                return;
            }
            try {
                Pr = new Process();
                Pr.StartInfo.FileName = Exe;
                Pr.StartInfo.Arguments = Arg;
                Pr.StartInfo.CreateNoWindow = true;
                Pr.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                Pr.StartInfo.RedirectStandardOutput = true;
                Pr.StartInfo.UseShellExecute = false;
                Comm.Text = $"\"{Exe}\" {Arg}";
                Pr.Start();
                Running = true;
                Stat.Text = "Process running";
                Outp.Text = "";
            } catch (Exception Foutje) {
                Console.Beep();
                Stat.Text = $"ERROR: {Foutje.Message}";
            }
        }
    }

}