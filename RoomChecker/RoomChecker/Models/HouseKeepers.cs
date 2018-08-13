using System;
using Amazon.DynamoDBv2.DataModel;

namespace RoomChecker.Models
{
    [DynamoDBTable("HouseKeepers")]
    public class HouseKeepers
    {
        [DynamoDBHashKey] 
        public Int32 Id { get; set; }
        [DynamoDBProperty]
        public string Name { get; set; }
        [DynamoDBProperty]
        public DateTime CheckDate { get; set; }
        [DynamoDBProperty]
        public string Point { get; set; }
    }
}
