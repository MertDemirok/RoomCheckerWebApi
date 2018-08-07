using System;
namespace RoomChecker.Models
{
    public class HouseKeepers
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public DateTime CheckDate { get; set; }
        public string Point { get; set; }
    }
}
