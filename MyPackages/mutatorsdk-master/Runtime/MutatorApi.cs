using System.Collections.Generic;
using System.Threading.Tasks;
using MagnusSdk.Core;
using MagnusSdk.Core.DeviceProperties;
using MagnusSdk.Core.Utils;
using Newtonsoft.Json.Linq;

namespace MagnusSdk.Mutator
{
    
    public class MutatorApi
    {
        private const string ApiBaseUrl = "https://mutator.magnus.ms/api/v1.0";
        
        private readonly string _token;
        private readonly JsonWebRequestWrapper _requestWrapper;

        public MutatorApi(string token)
        {
            _requestWrapper = new JsonWebRequestWrapper();
            _requestWrapper
                .SetBaseUrl(ApiBaseUrl)
                .AddHeader("Authorization", $"Bearer {token}");
        }

        public async Task<JObject> FetchConfig()
        {
            return await _requestWrapper.Get("/config");
        }

        public async Task PushSelectedVariants(Dictionary<int, int> selectedVariants)
        {
            Dictionary<string, object> args = GetDefaultArgs();
            
            List<Dictionary<string, int>> selectedVariantsList = new List<Dictionary<string, int>>();
            foreach (KeyValuePair<int, int> sv in selectedVariants)
            {
                selectedVariantsList.Add(new Dictionary<string, int>
                {
                    {"experiment_id", sv.Key},
                    {"variant_id", sv.Value}
                });
            }

            args["variants"] = selectedVariantsList;
            
            JObject body = JObject.FromObject(args);
            await _requestWrapper.Post("/experiment-variant", body);
        }

        private Dictionary<string, object> GetDefaultArgs()
        {
            DeviceProps dp = Magnus.DeviceProps;
            return new Dictionary<string, object>
            {
                {"idfa", dp.GetIDFA()},
                {"idfv", dp.GetIDFV()},
                {"idfm",dp.GetIDFM()}
            };
        }
    }
}