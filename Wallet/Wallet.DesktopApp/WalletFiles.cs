using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Wallet.DesktopApp
{

    public interface IWalletFileStorage
    {
        IEnumerable<string> GetWallets();
        string GetPath(string walletName);
    }

    public  class WalletFileStorage
    {
        private readonly static string WalletFolder = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory , "Wallets");


        public IEnumerable<string> GetWallets()
        {
            DirectoryInfo di = new DirectoryInfo(WalletFolder);

            return di.GetFiles("*.json").Select(f => f.Name).OrderBy(n => n).ToList();
        }


        public string GetPath(string walletName)
        {
            return Path.Combine(WalletFolder, walletName) + ".json";
        }



    }
}
