using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wallet.DesktopApp
{
    public interface IWalletStorage
    {
        IList<string> GetWallets();

        string GetWalletFilePath(string walletName);
    }

    public class WalletStorage : IWalletStorage
    {
        private readonly string WalletFolder = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "WalletFiles");

        public IList<string> GetWallets()
        {
            DirectoryInfo di = new DirectoryInfo(WalletFolder);

            if (!di.Exists)
            {
                di.Create();
            }

            return di.GetFiles("*.json").Select(f=> f.Name).ToList();
        }

        public string GetWalletFilePath(string walletName)
        {
            return Path.Combine(WalletFolder, walletName) + ".json";
        }
    }
}
