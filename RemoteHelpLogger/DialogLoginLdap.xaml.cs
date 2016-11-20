using System;
using System.Net;
using System.Windows;
using System.DirectoryServices.Protocols;

namespace RemoteHelpLogger
{
    /// <summary>
    /// Interaction logic for DialogLoginLdap.xaml
    /// </summary>
    public partial class DialogLoginLdap : Window
    {

        public string UserName { get; set; }
        public DialogLoginLdap()
        {
            InitializeComponent();
            txtUsername.Focus();
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            UserName = ""; //todo implement an async ldap login check 
            try
            {
                LdapConnection connection = new LdapConnection(Properties.Settings.Default.rh_ldap_domain);
                NetworkCredential credential = new NetworkCredential(txtUsername.Text, txtPassword.Password);
                connection.Credential = credential;
                connection.Bind();
                UserName = txtUsername.Text;
                this.DialogResult = true;
            }
            catch (Exception exc)
            {
                
            }
        }
    }
}
