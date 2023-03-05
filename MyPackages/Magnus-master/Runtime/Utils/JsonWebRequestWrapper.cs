using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine.Networking;

namespace MagnusSdk.Core.Utils
{
    public class JsonWebRequestWrapper
    {

        private string _baseUrl;
        private Dictionary<string, string> _headers = new Dictionary<string, string>
        {
            { "Content-Type", "application/json"},
            { "Accept", "application/json"}
        };

        public JsonWebRequestWrapper SetBaseUrl(string baseUrl)
        {
            _baseUrl = baseUrl;
            return this;
        }
        
        public JsonWebRequestWrapper AddHeader(string name, string value)
        {
            _headers.Add(name, value);
            return this;
        }
        
        public async Task<JObject> Get(string url, Dictionary<string, object> queryParams = null)
        {
            if (queryParams != null && queryParams.Count > 0)
            {
                List<string> queryParamsStrings = new List<string>();
                foreach (KeyValuePair<string, object> param in queryParams)
                {
                    queryParamsStrings.Add($"{param.Key}={param.Value}");
                }

                url += $"?{String.Join("&", queryParamsStrings)}";
            }

            UnityWebRequest request = GetFilledRequest(url);
            request.method = UnityWebRequest.kHttpVerbGET;

            await SendRequest(request);

            if (request.isHttpError || request.isNetworkError)
                throw new Exception(request.error);
            
            JObject result = JObject.Parse(request.downloadHandler.text);
            request.Dispose();
            
            return result;
        }

        public async Task<JObject> Post(string url, JObject data)
        {
            UnityWebRequest request = GetFilledRequest(url);
            request.method = UnityWebRequest.kHttpVerbPOST;
            
            string json = JsonConvert.SerializeObject(data);
            byte[] rawBody = Encoding.UTF8.GetBytes(json);

            request.uploadHandler = new UploadHandlerRaw(rawBody);
            
            await SendRequest(request);

            if (request.isHttpError || request.isNetworkError)
                throw new Exception(request.error);
            
            string resultRaw = request.downloadHandler.text;
            request.Dispose();
            
            if (string.IsNullOrEmpty(resultRaw))
                return new JObject();
            
            JObject result = JObject.Parse(resultRaw);
            return result;
            
        }

        private UnityWebRequest GetFilledRequest(string url)
        {
            UnityWebRequest request = new UnityWebRequest();

            request.url = $"{_baseUrl}{url}";

            foreach (KeyValuePair<string, string> header in _headers)
            {
                request.SetRequestHeader(header.Key, header.Value);
            }

            request.downloadHandler = new DownloadHandlerBuffer();

            return request;
        }

        private async Task SendRequest(UnityWebRequest request)
        {
            request.SendWebRequest();
            while (!request.isDone)
                await Task.Delay(100);
        }
        
    }
}