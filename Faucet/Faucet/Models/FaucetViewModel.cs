using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Faucet.Models
{
    public class FaucetViewModel
    {
        public string FaucetAddrees { get; set; }

        public string ReceiverAddrees { get; set; }


        public string SuccessMessage { get; set; }
        public string ErrorMessage { get; set; }

        public void Clear()
        {
            SuccessMessage = "";
            ErrorMessage = "";
        }
    }
}
