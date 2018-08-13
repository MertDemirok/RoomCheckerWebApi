using System;
using Amazon.DynamoDBv2.DataModel;
using RoomCheckerApi.Models;

namespace RoomChecker.Models
{
    [DynamoDBTable("Rooms")]
    public class Rooms
    {
        [DynamoDBHashKey] 
        public Int32 Id { get; set; }
        [DynamoDBProperty]
        public int RoomNumber { get; set; }
        [DynamoDBProperty("HouseKeepers")] 
        public HouseKeepers HouseKeeper { get; set; }
        [DynamoDBProperty]
        public DateTime CheckDate { get; set; }
        [DynamoDBProperty]
        public Int32 Status { get; set; }
        [DynamoDBProperty("BaseMaterials")] 
        public BaseMaterials BaseMaterial { get; set; }
        [DynamoDBProperty]
        public string Note { get; set; }
        [DynamoDBProperty]
        public Creator creator { get; set; }
    }

    public enum Creator
    {
        Admin = 1,
        Personel = 2
    }
}
