using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Faucet.Models;
using Wallet.Util;
using Microsoft.AspNetCore.Http;
using BlockChain.Core;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;

namespace Faucet.Controllers
{
    public class HomeController : Controller
    {
        public static Dictionary<string, DateTime> LastFaucetRequest = new Dictionary<string, DateTime>();

        public static readonly SimpleWallet simpleWallet = new SimpleWallet("E9873D79C6D87DC0FB6A5778633389F4453213303DA61F20BD67FC233AA33262");

        public static readonly TimeSpan MinRequestTime = TimeSpan.FromMinutes(5); 

        private IHttpContextAccessor _httpContextAccessor;

        public HomeController(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet()]
        public IActionResult Index()
        {
            FaucetViewModel model = new FaucetViewModel() {
                FaucetAddrees = simpleWallet.Address.ToLower()
            };
            return View(model);
        }


        [HttpPost()]
        public IActionResult Index(FaucetViewModel model)
        {
            try
            {
                model.Clear();

                string userIP = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();
                if (LastFaucetRequest.ContainsKey(userIP) && LastFaucetRequest[userIP].Add(MinRequestTime) > DateTime.Now)
                {//we have error
                    throw new ValidationException(model.ErrorMessage = $"Cannot request money form ip: {userIP} untill {LastFaucetRequest[userIP].Add(MinRequestTime)}");
                }

                var transaction = simpleWallet.Sign(model.ReceiverAddrees, 5 * Token.OneToken);

                var result = MakePost("http://localhost:5555/api/transaction/new", transaction);

                if (result)
                {
                    model.SuccessMessage = "Money were send to " + model.ReceiverAddrees;
                    LastFaucetRequest[userIP] = DateTime.Now;
                }
                else
                {
                    model.ErrorMessage = "Transaction not send";
                }
            }
            catch (ValidationException ex)
            {
                model.ErrorMessage = ex.Message;
            }
            catch (Exception ex)
            {
                model.ErrorMessage = "Error occured. No money. Sorry";
            }


            return View(model);
        }


        private class ValidationException : Exception
        {
            public ValidationException(string errorMessage)
                :base(errorMessage)
            {

            }
        }


        bool MakePost<T>(string url, T postObject)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                string postContent = JsonConvert.SerializeObject(postObject);
                var content = new StringContent(postContent, Encoding.UTF8, "application/json");

                var result = httpClient.PostAsync(url, content).GetAwaiter().GetResult();

                return result.IsSuccessStatusCode;
            }
        }


        T Get<T>(string url)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                var response = httpClient.GetAsync(url).GetAwaiter().GetResult();
                string json = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                T responceData = JsonConvert.DeserializeObject<T>(json);

                return responceData;
            }
        }

    }
}
