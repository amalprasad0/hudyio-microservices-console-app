using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace UserMicroservices.Dependencies
{
    public class Fast2SmsApi
    {
        private const string BaseUrl = "https://www.fast2sms.com/dev/bulkV2";

        private const string apiKey = "BZOj7UXNCUdluhgwBcEI8GfCEE9CGa2GfQgZIjF2SZttVmBD9e2SSEfwubqe";
        public async Task<string> SendSmsAsync(string otp, string phoneNumber)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("authorization", apiKey);

                    var queryParams = $"?authorization={apiKey}&route=otp&variables_values={otp}&flash=0&numbers={phoneNumber}";
                    string url = $"{BaseUrl}{queryParams}";

                    HttpResponseMessage response = await client.GetAsync(url);

                    if (response.IsSuccessStatusCode)
                    {
                        string responseContent = await response.Content.ReadAsStringAsync();
                        return responseContent;
                    }
                    else
                    {
                        return $"Error: {response.ReasonPhrase}";
                    }
                }
            }
            catch (Exception ex)
            {
                return $"Exception: {ex.Message}";
            }
        }
    }
}
