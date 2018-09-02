using System;
using Amazon.DynamoDBv2.DataModel;

namespace RoomChecker.Models
{
    [DynamoDBTable("BaseMaterials")]
    public class BaseMaterials
    {
            [DynamoDBHashKey] 
            public Int32 Id { get; set; }
            [DynamoDBProperty]
            public string Name { get; set; }
            [DynamoDBProperty]
            public Int16 Status { get; set; }
    }
}
