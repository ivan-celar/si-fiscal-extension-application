using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Mikos.SI.Fiscal.Dtos;

namespace Mikos.SI.Fiscal.Services
{
    public class ApiHelper
    {
        public static async Task<FiscalResponseData> PostAsync(FiscalRequestData content, string url)
        {
            try
            {
                using (var client = new HttpClient())
                using (var request = new HttpRequestMessage(HttpMethod.Post, url))
                {
                    var json = JsonConvert.SerializeObject(content);
                    using (var stringContent = new StringContent(json, Encoding.UTF8, "application/json"))
                    {
                        request.Content = stringContent;
                        using (var response = await client.SendAsync(request, HttpCompletionOption.ResponseContentRead).ConfigureAwait(true))
                        {
                            var contents = await response.Content.ReadAsStringAsync();
                            FiscalResponseData responseModel = JsonConvert.DeserializeObject<FiscalResponseData>(contents);
                            return responseModel;
                        }
                    }
                }
            } catch (Exception ex)
            {
                return toNoFiscalResponse(ex);
            }
        }

        public static async Task<ReferenceResponse> ValidateInvoiceReference(ReferenceRequest content, string url)
        {
            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage(HttpMethod.Post, url))
            {
                var json = JsonConvert.SerializeObject(content);
                using (var stringContent = new StringContent(json, Encoding.UTF8, "application/json"))
                {
                    request.Content = stringContent;
                    using (var response = await client.SendAsync(request, HttpCompletionOption.ResponseContentRead).ConfigureAwait(true))
                    {
                        var contents = await response.Content.ReadAsStringAsync();
                        ReferenceResponse isValid = JsonConvert.DeserializeObject<ReferenceResponse>(contents);

                        return isValid;
                    }
                }
            }
        }

        private static FiscalResponseData toNoFiscalResponse(Exception ex)
        {
            FiscalResponseData data = new FiscalResponseData()
            {
                FiscalOutputs = new FiscalOutputs(),
                StatusMessages = toNoResponseStatusMessages(ex)
            };


            return data;
        }

        private static StatusMessages toNoResponseStatusMessages(Exception ex)
        {
            StatusMessages statusMessages = new StatusMessages();
            List<Message> Messages = new List<Message>();

            Messages.Add(new Message()
            {
                Type = "Error",
                Description = "Fiskalna storitev ni na voljo. Obrnite se na službo za stranke z naslednjim sporočilom o napaki: " + ex.Message
            });

            statusMessages.Messages = Messages;


            return statusMessages;
        }
    }
}
