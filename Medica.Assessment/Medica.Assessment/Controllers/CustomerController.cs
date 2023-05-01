using Medica.Assessment.Model;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System.Text;
using System.Text.Json;

namespace Medica.Assessment.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly IHttpClientFactory _clientFactory;

        public CustomerController(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        [HttpPost]
        public async Task<IActionResult> PostCustomer([FromBody] customer customer)
        {
            var client = _clientFactory.CreateClient();
            var json = JsonSerializer.Serialize(customer);
            var content = new StringContent(json, Encoding.UTF8, Properties.Resources.resApplicationJson);
            var response = await client.PostAsync(Properties.Resources.resCustomerPOSTurl, content);

            if (response.IsSuccessStatusCode)
            {
                var resultToLog = response.ToString();
                Log.Information(Properties.Resources.resLogResponseReceivedMsg, resultToLog); 
                return Ok();
            }
            else
            {
                var resultStatusCode = response.StatusCode;
                Log.Warning(Properties.Resources.resLogResponseFailedMsg, resultStatusCode);
                return BadRequest();
            }
        }
    }
}
