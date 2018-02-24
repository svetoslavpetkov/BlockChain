using BlockChain.Core;
using HBitcoin.KeyManagement;
using NBitcoin;
using System;
using System.Collections.Generic;
using System.Text;

namespace Wallet.Util
{
    public class FullWallet
    {
        private static Network Network = Network.Main;

        private ICryptoUtil CryptoUtil = new CryptoUtil();

        public static string GenerateNewWallet(string jsonFilePath, string password)
        {
            Mnemonic mnemonic;
            var safe = Safe.Create(out mnemonic, password, jsonFilePath, Network);

            return mnemonic.ToString();
        }

        public static FullWallet Recover(string mnemonicString, string password, string walletJsonFilePath, string walletName)
        {
            Mnemonic mnemonic = new Mnemonic(mnemonicString);

            var safe = Safe.Recover(mnemonic, password, walletJsonFilePath, Network);

            return new FullWallet(walletName, safe);
        }


        private Safe _safe;

        public string Name { get; set; }


        public FullWallet(string name,string jsonFilePath, string password)
        {
            _safe = Safe.Load(password, jsonFilePath);
            Name = name;
        }

        private FullWallet(string name, Safe safe)
        {
            _safe = safe;
            Name = name;
        }

        public IList<SimpleWallet> GetAccounts()
        {
            List<SimpleWallet> simpleWallets = new List<SimpleWallet>();

            for (int i = 0; i < 10; i++)
            {
                var bitAdress = _safe.GetAddress(i);
                var extKEy =_safe.ExtKey.Derive(KeyPath.Parse($"/44'/60'/0'/{i}"));
                simpleWallets.Add(new SimpleWallet(extKEy.PrivateKey.ToBytes()));
            }

            return simpleWallets;
        }
    }
}
