using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RoomsLiberatorEngine.ViewModel;

namespace RoomsLiberatorEngine.Controllers
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

        // GET api/PushCardID/5
        [HttpGet("/PushCardID/{id}", Name = "PushCardID")]
        [Route("PushCardID")]
        public string PushCardID(string id)
        {
            try
            {
                var DbContext = new DatabaseContext();

                if (!string.IsNullOrEmpty(id))
                {
                    var eventLog = new EventLog()
                    {
                        Date = DateTime.Now,
                        DeviceId = 1,
                        Value = id,
                        RoomId = 1,
                        DeviceType = 1,
                    };

                    DbContext.EventLogs.Add(eventLog);

                    DbContext.DeviceStates.Update(new DeviceState()
                    {
                        Date = DateTime.Now,
                        DeviceId = 1,
                        RoomId = 1,
                        Value = id,
                        Id = 1
                    });

                    DbContext.SaveChanges();
                }
            }
            catch (Exception e)
            {
                return e.Message;
                throw;
            }

            return $"value: {id}";
        }

        // GET api/PushMotionSensorState/1
        //[HttpGet("{id}")]
        //public string PushMotionSensorState(string value)
        //{
        //    //DatabaseContext context = new DatabaseContext();
        //    //context.EventLogs.Add(value);
        //    //context.SaveChanges();
        //    //var l = context.EventLogs.Last();
        //    //return l.Value;
        //    return "as";
        //}

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
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
