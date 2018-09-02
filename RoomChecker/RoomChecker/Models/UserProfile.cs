using System;
using Amazon.DynamoDBv2.DataModel;
namespace RoomChecker.Models
{
    [DynamoDBTable("UserProfile")]
    public class UserProfile
    {
        [DynamoDBHashKey]
        public Int32 Id { get; set; }
        [DynamoDBProperty]
        public int UserName { get; set; }
        [DynamoDBProperty]
        public String Password { get; set; }//hash
        [DynamoDBProperty]
        public Int32 Status { get; set; }
        [DynamoDBProperty]
        public String AcountType { get; set; }
    }
}
