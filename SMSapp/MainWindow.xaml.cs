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

namespace SMSapp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string V = "COM4";

        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnSend(object sender, RoutedEventArgs e)
        {
            string num = TelNumBox.Text;
            string message = MessageTextBox.Text;
            SMSCOMMS SMSEngine = new SMSCOMMS(ref V);
            SMSEngine.Open();
            SMSEngine.SendSMS(num, message);
            SMSEngine.Close();

            MessageBox.Show("The message was sent to '" + num + "', now close your app!");
        }
    }
}
