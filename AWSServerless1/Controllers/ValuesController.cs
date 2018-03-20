using System;
using System.Configuration;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Dynamic;
using System.Threading.Tasks;
using Amazon.Kinesis;
using Amazon.Kinesis.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Protocols;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;

namespace AWSServerless1.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        // GET api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]dynamic value)
        {

            string dataAsJson = JsonConvert.SerializeObject(value);
            byte[] dataAsBytes = Encoding.UTF8.GetBytes(dataAsJson);
            using (MemoryStream memoryStream = new MemoryStream(dataAsBytes))
            {
                try
                {
                    AmazonKinesisConfig config = new AmazonKinesisConfig();
                    config.RegionEndpoint = Amazon.RegionEndpoint.USEast1;
                    AmazonKinesisClient kinesisClient = new AmazonKinesisClient(config);
                    String kinesisStreamName = "click-stream";

                    PutRecordRequest requestRecord = new PutRecordRequest();
                    requestRecord.StreamName = kinesisStreamName;
                    requestRecord.PartitionKey = "temp";
                    requestRecord.Data = memoryStream;

                    kinesisClient.PutRecordAsync(requestRecord);
                    //Console.WriteLine("Successfully sent record {0} to Kinesis. Sequence number: {1}", wt.Url, responseRecord.SequenceNumber);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Failed to send record to Kinesis. Exception: {0}", ex.Message);
                }
            }

        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
