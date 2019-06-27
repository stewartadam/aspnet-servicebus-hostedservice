using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace mvp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BusController : ControllerBase
    {
        private readonly IConfiguration configuration;
        private readonly ILogger logger;
        public BusController(IConfiguration configuration, ILoggerFactory loggerFactory) {
            this.configuration = configuration;
            this.logger = loggerFactory.CreateLogger<BusController>();
        }

        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public ActionResult<Dictionary<string, string>> Post()
        {
            var messageInfo = new MessageInfo();
            var queueClient = new QueueClient(configuration.GetValue<string>("serviceBusConnectionString"), configuration.GetValue<string>("serviceBusQueueName"));
            var json = JsonConvert.SerializeObject(messageInfo);
            var message = new Message(Encoding.UTF8.GetBytes(json));
            queueClient.SendAsync(message);
            logger.LogDebug($"{messageInfo.CorrelationId.ToString()} | BusListenerService sent item.");

            return Ok(new Dictionary<string, string>() { {"CorrelationId", messageInfo.CorrelationId.ToString() }});
        }
    }
}
