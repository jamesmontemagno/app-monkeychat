using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using Plugin.DeviceInfo;

namespace MonkeyChat
{
    public static class TwilioHelper
    {
        public static string Identity { get; private set;}

        public static async Task<string> GetTokenAsync()
        {
            var id = CrossDeviceInfo.Current.Id;

            var tokenEndpoint = $"https://xamarinchat.azurewebsites.net/token?device={id}";

            var http = new HttpClient();
            var data = await http.GetStringAsync(tokenEndpoint);

            var response = Newtonsoft.Json.JsonConvert.DeserializeObject<TwilioResponse>(data);

            Identity = response.Identity?.Trim('"') ?? string.Empty;

            return response?.Token?.Trim('"') ?? string.Empty;
        }
    }

    public class TwilioResponse
    {
        [Newtonsoft.Json.JsonProperty("identity")]
        public string Identity { get; set; } = string.Empty;

        [Newtonsoft.Json.JsonProperty("token")]
        public string Token { get; set; } = string.Empty;
    }


}

