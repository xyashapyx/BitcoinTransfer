using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace BitcoinTransfer.Services
{
    public class HttpClientService : HttpClient
    {

        public HttpClientService() : base()
        {
            Timeout = TimeSpan.FromSeconds(15);
            MaxResponseContentBufferSize = 256000;
        }

        public static HttpClientService Instance { get; } = new HttpClientService();

        public async Task<List<T>> GetListItems<T>(string Url)
        {
            var uri = new Uri(string.Concat(Consts.BaseUrlApi, Url));
            var response = await GetAsync(uri);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var Items = JsonConvert.DeserializeObject<List<T>>(content);
                return Items;
            }
            throw new Exception(response.ReasonPhrase);
        }

        public async Task<T> GetItem<T>(string url)
        {
            var uri = new Uri(string.Concat(Consts.BaseUrlApi, url));
            var response = await GetAsync(uri);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var Item = JsonConvert.DeserializeObject<T>(content);
                return Item;
            }
            throw new Exception(response.ReasonPhrase);
        }

        public async Task PostItem<T>(T item, string url)
        {
            var uri = new Uri(string.Concat(Consts.BaseUrlApi));
            var json = JsonConvert.SerializeObject(item);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = null;
            response = await PostAsync(uri, content);
            if (response.IsSuccessStatusCode)
            {
                return;
            }
            throw new Exception(response.ReasonPhrase);
        }
    }
}
