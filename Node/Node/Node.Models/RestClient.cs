using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;

namespace BlockChain.Core
{
    public class RestClient : IDisposable
    {
        protected HttpClient Client { get; private set; } = new HttpClient();

        protected Uri ApiUri { get; private set; }

        public RestClient(string apiUrl)
        {
            ApiUri = new Uri(apiUrl);
        }

        public void Dispose()
        {
            Client.Dispose();
        }

        public TResult Post<TRequest, TResult>(string methodPath, TRequest request)
        {
            Uri requestUri = new Uri(ApiUri, methodPath);
            HttpResponseMessage response = Client.PostAsync(requestUri,
                ObjectToJsonContent(request)).GetAwaiter().GetResult();
            response.EnsureSuccessStatusCode();
            TResult result = ReadContent<TResult>(response);

            return result;

        }

        public void Post<TRequest>(string methodPath, TRequest request)
        {
            Uri requestUri = new Uri(ApiUri, methodPath);
            HttpResponseMessage response = Client.PostAsync(requestUri,
                ObjectToJsonContent(request)).GetAwaiter().GetResult();
            response.EnsureSuccessStatusCode();
        }

        public TResult Get<TResult>(string methodPath)
        {
            Uri requestUri = new Uri(ApiUri, methodPath);
            string json = null;
            try
            {
                HttpResponseMessage response = Client.GetAsync(requestUri).GetAwaiter().GetResult();
                response.EnsureSuccessStatusCode();
                json = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            }
            catch
            {
                return default(TResult);
            }
            if (json == null)
                return default(TResult);
            else
                return JsonConvert.DeserializeObject<TResult>(json);
        }

        private StringContent ObjectToJsonContent(Object data)
        {
            string jsonData = data == null ? String.Empty : JsonConvert.SerializeObject(data);

            return new StringContent(jsonData, Encoding.UTF8, "application/json");
        }

        private T ReadContent<T>(HttpResponseMessage response)
        {
            string json = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}
