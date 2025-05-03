using System.Net.Http.Headers;
using System.Text;

namespace KiMa_API.Services
{
    public interface IRapidmailService
    {
        Task AddSubscriberAsync(string email);
        Task RemoveSubscriberAsync(string email);
    }

    public class RapidmailService : IRapidmailService
    {
        private readonly HttpClient _http;
        private readonly string _listId;

        public RapidmailService(IConfiguration cfg, HttpClient http)
        {
            _http = http;
            _listId = cfg["Rapidmail:ListId"];
            var user = cfg["Rapidmail:ApiUsername"];
            var pw = cfg["Rapidmail:ApiPassword"];
            var token = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{user}:{pw}"));

            _http.BaseAddress = new Uri("https://apiv2.rapidmail.de");
            _http.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Basic", token);
        }

        public async Task AddSubscriberAsync(string email)
        {
            var payload = new
            {
                list_id = _listId,
                subscribers = new[] { new { email } }
            };
            var res = await _http.PostAsJsonAsync("/lists/receiver/add", payload);
            res.EnsureSuccessStatusCode();
        }

        public async Task RemoveSubscriberAsync(string email)
        {
            var payload = new
            {
                list_id = _listId,
                subscribers = new[] { email }
            };
            var res = await _http.PostAsJsonAsync("/lists/receiver/remove", payload);
            res.EnsureSuccessStatusCode();
        }
    }
}
