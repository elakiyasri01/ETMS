using System.Net.Http;
using System.Threading.Tasks;

namespace Employee_TMS.Services
{
    public class APIservices
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiUrl;

        public APIservices()
        {
            _apiUrl = "http://localhost:5218/api/Employee/"; // API base address
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(_apiUrl);
        }

        public async Task<byte[]> GenerateExcel()
        {
            HttpResponseMessage response = await _httpClient.GetAsync("GenerateExcel");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsByteArrayAsync();
            }
            else
            {
               
                return null;
            }
        }
    }
}
