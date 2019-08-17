using System;

namespace RoomsLiberatorEngine
{
    public class DeviceState
    {
        public int Id { get; set; }
        public int DeviceId { get; set; }
        public DateTime Date { get; set; }
        public string Value { get; set; }
        public int RoomId { get; set; }
    }
}