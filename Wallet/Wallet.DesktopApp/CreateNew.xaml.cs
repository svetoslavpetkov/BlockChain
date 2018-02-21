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
using Wallet.Util;

namespace Wallet.DesktopApp
{
    /// <summary>
    /// Interaction logic for CreateNew.xaml
    /// </summary>
    public partial class CreateNew : Window
    {
        IWalletStorage walletStorage = new WalletStorage();

        FullWallet wallet = null;

        public CreateNew()
        {
            InitializeComponent();
        }

        private void Generate_Click(object sender, RoutedEventArgs e)
        {
            string password = passwordBox.Password;
            string name = WalletName.Text;
            string path = walletStorage.GetWalletFilePath(name);
            string mnemonic = FullWallet.GenerateNewWallet(path, password);

            wallet = new FullWallet(name, path, password);

            Mnemonic.Text = mnemonic;

            Generate.Visibility = Visibility.Hidden;
            OpenWallet.Visibility = Visibility.Visible;
        }

        private void OpenWallet_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
