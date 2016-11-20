using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace RemoteHelpLogger
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private bool isStarted = false;
        private bool isValidUser = false;
        private string userName = "";
        private DateTime startTime = DateTime.Now;
        string cmd = "";
        System.Diagnostics.Process process = null;

        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>Sets the enabled flag for the btnStart</summary>
        private void CheckIsStartable()
        {
            if (isStarted) return; //not visible
            if (txtDescr.Text.Length < 3)
            {
                btnStart.IsEnabled = false;
                return;
            }
            if (!isValidUser)
            {
                btnStart.IsEnabled = false;
                return;
            }
            btnStart.IsEnabled = true;
        }

        
        private void HandleLogoff()
        {
            isValidUser = false;
            userName = "";
            txtUsername.Content = Properties.Resources.strUserNone;
            btnLogoff.IsEnabled = false;
        }

        private void HandleSessionEnd()
        {
            SetControlsState(false);
            LoggerLogic.LogSessionStop(startTime);
            StopProgram();
        }

        
        private void HandleSessionStart()
        {
            startTime = DateTime.Now;
            SetControlsState(true);
            LoggerLogic.LogSessionStart(startTime);
            StartProgram();
        }

        private void StartProgram()
        {
            cmd = "teamviewer.exe";
            if (cmdProgram.SelectedIndex == 0) cmd = "teamviewer.exe";
            if (cmdProgram.SelectedIndex == 1) cmd = "mstsc.exe";
            if (cmdProgram.SelectedIndex == 2) cmd = "quickassist.exe";

            try
            {
                process = new System.Diagnostics.Process();
                process.EnableRaisingEvents = true;
                process.Exited += Process_Exited;
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = cmd,
                    // Must be false to redirect IO
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    Arguments = null
                };
                process.StartInfo = psi;
                process.Start();
                process.Refresh();
            }
            catch(Exception ee)
            {
                //todo handle exception
            }
        }

        private void Process_Exited(object sender, EventArgs e)
        {
            this.Dispatcher.Invoke((Action)(() =>
            {
                HandleSessionEnd();
                if (Properties.Settings.Default.rh_auto_lofogg_after_session)
                {
                    HandleLogoff();
                }
                CheckIsStartable();
            }));
           
        }

        private void StopProgram()
        {
            if (process != null)
            {
                if (!process.HasExited)
                {
                    process.Kill();
                }
            }
            process = null;
            //double check, because of the hasexited stuff bug
            foreach (Process current in Process.GetProcesses())
            {
                //current
                try
                {
                    if (current.MainModule.ModuleName.Equals(cmd))
                    {
                        current.Kill();
                    }
                }
                catch (Exception ee) { }
            }

        }
        private void SetControlsState(bool started)
        {
            btnChangeUsr.IsEnabled = !started;
            txtDescr.IsEnabled = !started;
            cmdProgram.IsEnabled = !started;
            btnSettings.IsEnabled = !started;
            btnStart.Visibility = (!started) ? Visibility.Visible : Visibility.Hidden;
            btnStop.Visibility = started ? Visibility.Visible : Visibility.Hidden;
        }



        #region User events

        private void btnLogoff_Click(object sender, RoutedEventArgs e)
        {
            HandleSessionEnd();
            HandleLogoff();
            CheckIsStartable();
        }

        private void btnSettings_Click(object sender, RoutedEventArgs e)
        {
            SettingsWindow settings = new SettingsWindow();
            settings.ShowDialog();
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            HandleSessionStart();
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            HandleSessionEnd();
            if (Properties.Settings.Default.rh_auto_lofogg_after_session)
            {
                HandleLogoff();
            }
            CheckIsStartable();
        }

        private void btnChangeUsr_Click(object sender, RoutedEventArgs e)
        {
            HandleLogoff();
            Window frm = null;
            if (Properties.Settings.Default.rh_auth_method.Equals("ldap")) frm = new DialogLoginLdap();
            if (Properties.Settings.Default.rh_auth_method.Equals("single")) frm = new DialogLoginSingle();
            if (frm == null) frm = new DialogLoginSingle();
            if (frm != null)
            {
                frm.ShowDialog();
            }

            if (frm.DialogResult.HasValue && frm.DialogResult.Value)
            {
                if (frm.GetType() == typeof(DialogLoginSingle)) { userName = ((DialogLoginSingle)frm).UserName; }
                if (frm.GetType() == typeof(DialogLoginLdap)) { userName = ((DialogLoginLdap)frm).UserName; }
                btnLogoff.IsEnabled = true;
            }
            else
            {
                //no result -> no user
                userName = "";
                HandleLogoff(); //to set visuals
                btnLogoff.IsEnabled = false;
            }
            isValidUser = !(userName.Length < 3);
            if (isValidUser) txtUsername.Content = userName;
            CheckIsStartable();
        }

        private void txtDescr_TextChanged(object sender, TextChangedEventArgs e)
        {
            CheckIsStartable();
        }

        #endregion


    }
}
