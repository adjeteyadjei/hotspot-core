using Flurl;
using Flurl.Http;
using System;
using System.Threading.Tasks;

namespace Hotvenues.Integrations
{
    public class Postman
    {
        private const string BaseUrl = "http://postmaster.devnestsystems.com/api/v1/messaging";
        private readonly string _apiKey;
        private readonly string _senderName;

        public Postman(string apiKey, string senderName)
        {
            _apiKey = apiKey;
            _senderName = senderName;
        }

        public async Task<Tuple<bool, string>> SendMessage(string phoneNumbers, string text)
        {
            var response = await BaseUrl.AppendPathSegments("sendsms").PostJsonAsync(new
            {
                ApiKey = _apiKey,
                Sender = _senderName,
                Recipients = phoneNumbers,
                Text = text
            }).ReceiveJson<MsgResponse>();

            return new Tuple<bool, string>(response.Success, response.Message);
        }

        public async Task<Tuple<bool, string>> SendMessage(string emailAddresses, string subject, string text, byte[] attachment = null, string fileName = null)
        {
            var response = await BaseUrl.AppendPathSegments("sendemail").PostJsonAsync(new
            {
                ApiKey = _apiKey,
                Sender = _senderName,
                Subject = subject,
                Recipients = emailAddresses,
                Text = text,
                Attachment = attachment,
                FileName = fileName
            }).ReceiveJson<MsgResponse>();

            return new Tuple<bool, string>(response.Success, response.Message);
        }

        public async Task<decimal> CreditBalance()
        {
            var response = await BaseUrl.AppendPathSegments("creditbalance")
                .SetQueryParams(new { ApiKey = _apiKey })
                .GetJsonAsync<MsgResponse<CreditBalances>>();

            return response.Data.SmsBalance;
        }

        public class CreditBalances
        {
            public decimal SmsBalance { get; set; }
            public decimal EmailBalance { get; set; }
        }

        public class MsgResponse
        {
            public long Total { get; set; }
            public object Data { get; set; }
            public string Message { get; set; }
            public bool Success { get; set; }
        }

        public class MsgResponse<T>
        {
            public long Total { get; set; }
            public T Data { get; set; }
            public string Message { get; set; }
            public bool Success { get; set; }
        }

    }
}