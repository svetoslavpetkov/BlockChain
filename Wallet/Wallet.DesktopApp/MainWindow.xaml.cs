using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
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
using Wallet.Util;

namespace Wallet.DesktopApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private SimpleWallet Wallet = null;

        public MainWindow(SimpleWallet wallet)
        {
            InitializeComponent();
            Wallet = wallet;

            this.ownAddress.Text = wallet.GetAddress();

            this.ballace.Content = $"{wallet.GetAmount()} coins";

            this.ownAddress.Text = wallet.GetAddress();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string from = this.ownAddress.Text;
            string to = this.remoteAddress.Text;
            decimal amount = decimal.Parse(this.amount.Text);

            var transaction = Wallet.Sign(to, amount);

            this.result.Content = "Sending transaction ....";
            this.result.Visibility = Visibility.Visible;
            
            var result = MakePost("http://localhost:5555/api/transaction/new",transaction);

            this.result.Content = result ? "Transaction send" : "Transaction rejected";
        }



        bool MakePost<T>(string url,T postObject)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                string postContent = JsonConvert.SerializeObject(postObject);
                var content = new StringContent(postContent, Encoding.UTF8, "application/json");

                var result = httpClient.PostAsync(url, content).GetAwaiter().GetResult();

                return result.IsSuccessStatusCode;
            }
        }


        void Get(string url)
        {

            using (HttpClient httpClient = new HttpClient())
            {
                //string postContent = JsonConvert.SerializeObject(postObject);
                //svar content = new StringContent(postContent, Encoding.UTF8, "application/json");
                var task = httpClient.GetAsync(url);

                var result = task.GetAwaiter().GetResult();
            }
        }
    }
}
