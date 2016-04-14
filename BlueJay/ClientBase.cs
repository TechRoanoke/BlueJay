using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BlueJay
{
    public class ClientBase
    {
        HttpClient _httpClient = new HttpClient();

        private readonly AWS4SignerBase _signer;

        public ClientBase(Uri endpoint, string clientId, string secret)
        {
            _signer = new AWS4SignerBase(endpoint, clientId, secret);
        }


        protected async Task<TResult> SendJsonAsync<TResult>(HttpRequestMessage request)
        {
            var response = await _httpClient.SendAsync(request);
            var body = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {                
                throw new BlueJayException(response.StatusCode, body);
            }

            var result = JsonConvert.DeserializeObject<TResult>(body);
            return result;
        }

        protected HttpRequestMessage MakeGetRequest(string path, IDictionary<string, string> queryParams = null)
        {
            var request = _signer.CreateSignedRequest(HttpMethod.Get, path, queryParams, null);
            return request;
        }
        protected HttpRequestMessage MakePostRequest(string path, object body)
        {
            string json = JsonConvert.SerializeObject(body);
            var request = _signer.CreateSignedRequest(HttpMethod.Post, path, null, json);
            return request;
        }
    }
}
