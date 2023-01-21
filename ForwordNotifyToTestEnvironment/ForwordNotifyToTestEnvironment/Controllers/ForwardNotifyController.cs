using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text;
using System.Web;

namespace ForwordNotifyToTestEnvironment.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ForwardNotifyController : ControllerBase
    {

        private readonly ILogger<ForwardNotifyController> _logger;

        public ForwardNotifyController(ILogger<ForwardNotifyController> logger)
        {
            _logger = logger;
        }

        [HttpPost("ToEnvironment")]
        public async Task<ActionResult> ToEnvironment([FromBody] string dataAndLen = null)
        {
            string responseContent = string.Empty;
            try
            {
                _logger.LogInformation(">>>>received: " + dataAndLen);
                var nameValueCollection = HttpUtility.ParseQueryString(dataAndLen);
                int len = 0;
                string data = string.Empty;
                foreach (string key in nameValueCollection.AllKeys.Where(x => !string.IsNullOrEmpty(x)))
                {
                    if (0 == string.Compare("len", key, StringComparison.OrdinalIgnoreCase))
                    {
                        len = Convert.ToInt32(nameValueCollection[key]);
                    }

                    if (0 == string.Compare("data", key, StringComparison.OrdinalIgnoreCase))
                    {
                        data = nameValueCollection[key] ?? String.Empty;
                    }
                }
                var url = "https://test.journaway.com/backend/api/payment/notify?auth=20Journaway19";
                using var client = new HttpClient();
                var values = new Dictionary<string, string> {
                    { "data", data },
                    { "len", len.ToString() },
                };
                //neusta:9hX!X75fxh
                var byteArray = new UTF8Encoding().GetBytes("neusta:9hX!X75fxh");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
                var formData = new FormUrlEncodedContent(values);
                var response = await client.PostAsync(url, formData);
                responseContent = response.Content.ReadAsStringAsync().Result;
                response.EnsureSuccessStatusCode();
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "<<<<Error: " + responseContent);
                return BadRequest();
            }
        }
    }
}