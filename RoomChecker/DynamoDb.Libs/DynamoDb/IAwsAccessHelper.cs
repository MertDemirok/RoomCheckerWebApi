using System;
using Amazon.DynamoDBv2;

namespace DynamoDb.Libs.DynamoDb
{

    public interface IAwsAccessHelper
    { 
        AmazonDynamoDBClient AwsConnecttion();
    }
}
