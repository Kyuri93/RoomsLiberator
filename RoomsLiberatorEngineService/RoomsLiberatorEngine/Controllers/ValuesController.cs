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
            try
            {
                var _myDbContext = new DatabaseContext();

                _myDbContext.Users.Update(new User()
                {
                    Id = 1,
                    UserId = "bbb",
                    UserMail = "mailbbb"

                });

               // var l = _myDbContext.EventLogs.First();
               //_myDbContext.Users.Add(new User()
               // {
               //     Id = 1,
               //     UserId = "a",
               //     UserMail = "mail"

               // });
                _myDbContext.SaveChanges();
                return
                    new string[]
                        {"a", "b"}; //{ l.DeviceId.ToString(), l.DeviceType.ToString(), l.RoomId.ToString(), l.Value };
            }
            catch (Exception e)
            {
                return new string[] {e.Message};
                Console.WriteLine(e);
                throw;
            }
            
            return new string[] { "value1", "value2" };
        }

        // GET api/PushCardID/5
        [HttpGet("PushCardID{id}")]
        public string PushCardID(string id)
        {
            return $"value: {id}";
        }

        // GET api/PushMotionSensorState/1
        [HttpGet("{id}")]
        public string PushMotionSensorState([FromBody] EventLog value)
        {
            //DatabaseContext context = new DatabaseContext();
            //context.EventLogs.Add(value);
            //context.SaveChanges();
            //var l = context.EventLogs.Last();
            //return l.Value;
            return "as";
        }

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
