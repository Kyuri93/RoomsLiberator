using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RoomsLiberatorEngine
{
    public class EventLog
    {
        public int Id { get; set; }
        public int DeviceId { get; set; }
        public int RoomId { get; set; }
        public int DeviceType { get; set; }
        public string Value { get; set; }
        public string Date { get; set; }
    }
}
