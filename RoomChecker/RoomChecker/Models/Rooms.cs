using System;
using RoomCheckerApi.Models;

namespace RoomChecker.Models
{
    public class Rooms
    {
        public long Id { get; set; }
        public int RoomNumber { get; set; }
        public HouseKeepers HouseKeeper { get; set; }
        public DateTime CheckDate { get; set; }
        public Int32 Status { get; set; }
        public BaseMaterials BaseMaterial { get; set; }
        public string Note { get; set; }
    }
}
