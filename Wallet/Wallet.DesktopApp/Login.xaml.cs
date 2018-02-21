using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace Wallet.DesktopApp
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        IWalletStorage walletStorage = new WalletStorage();

        public ObservableCollection<ComboBoxItem> cbItems { get; set; }
        public ComboBoxItem SelectedcbItem { get; set; }

        public string Password { get; set; }

        public Login()
        {
            cbItems = new ObservableCollection<ComboBoxItem>();

            InitializeComponent();

            var wallets = walletStorage.GetWallets();

            foreach (var wallet in wallets)
            {
                cbItems.Add(new ComboBoxItem() { Content = wallet });
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //MainWindow mainWindow = new MainWindow(Util.SimpleWallet.GenerateNewWallet(mnemonic, password));
            //mainWindow.Show();
            this.Close();
        }

        private void CreateNew_Click(object sender, RoutedEventArgs e)
        {
            Wallet.DesktopApp.CreateNew createNewWindow = new DesktopApp.CreateNew();
            createNewWindow.Show();
            this.Close();

        }
    }
}
