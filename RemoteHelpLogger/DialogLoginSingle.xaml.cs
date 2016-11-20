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
using System.Windows.Shapes;

namespace RemoteHelpLogger
{
    /// <summary>
    /// Interaction logic for DialogLoginSingle.xaml
    /// </summary>
    public partial class DialogLoginSingle : Window
    {
        public string UserName { get; set; }
        public DialogLoginSingle()
        {
            InitializeComponent();
            txtUsername.Focus();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            UserName = txtUsername.Text;
            this.DialogResult = true;
        }
    }
}
