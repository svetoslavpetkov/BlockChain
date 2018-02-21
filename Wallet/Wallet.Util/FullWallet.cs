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


        private Safe _safe;


        public FullWallet(string jsonFilePath, string password)
        {
            _safe = Safe.Load(password, jsonFilePath);
        }

        public IList<SimpleWallet> GetAccounts()
        {
            List<SimpleWallet> simpleWallets = new List<SimpleWallet>();

            for (int i = 0; i < 10; i++)
            {
                var bitAdress = _safe.GetAddress(i);

                var extKEy =_safe.ExtKey.Derive(KeyPath.Parse("/44'/60'/0'/0"));

                var privKey = _safe.FindPrivateKey(bitAdress);

                simpleWallets.Add(new SimpleWallet(extKEy.ToBytes()));
            }

            return simpleWallets;
        }
    }
}
