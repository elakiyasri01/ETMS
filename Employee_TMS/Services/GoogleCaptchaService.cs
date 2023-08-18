using Employee_TMS.Entities;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Employee_TMS.Services
{
    public class GoogleCaptchaService
    {
        private IOptionsMonitor<GoogleCaptchaConfig> _captchaConfig;

        public GoogleCaptchaService(IOptionsMonitor<GoogleCaptchaConfig>CaptchaConfig) 
		{
            _captchaConfig = CaptchaConfig;
		}
        public async Task<bool> VerifyToken(string token)
        {
			try
			{
				var url = $"https://www.google.com/recaptcha/api/siteverify?secret={_captchaConfig.CurrentValue.SecretKey}&response={token}";
				using (var client = new HttpClient())
				{
					var httpResult=await client.GetAsync(url);
					if (httpResult.StatusCode != System.Net.HttpStatusCode.OK)
					{
						return false;
					}
					var responseString=await httpResult.Content.ReadAsStringAsync();
					var googleResult = JsonConvert.DeserializeObject<GoogleCaptchaResponse>(responseString);
					return googleResult.success && googleResult.score >= 0.5;
				}	
			}
			catch 
			{

				return false;
			}
        }
    }
}
