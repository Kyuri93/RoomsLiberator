using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace AgentApp.Wrapers
{
    class HttpSender
    {
        const string ServerUrl = "http://172.25.10.161:17377";
        private const string SaveCardUrl = "api/PushCardID";

        public void ClientHeaderInfo(HttpClient client)
        {
            client.BaseAddress = new Uri(ServerUrl);
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public virtual async Task Get(string url)
        {
            try
            {
                HttpClientHandler handler = new HttpClientHandler {UseDefaultCredentials = true};
                using (var client = new HttpClient(handler))
                {
                    ClientHeaderInfo(client);

                    await client.GetAsync(url);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        public async Task SendUserId(string id)
        {
            var url = SaveCardUrl + $"?={id}";

            await Get(url);
        }
    }
}
