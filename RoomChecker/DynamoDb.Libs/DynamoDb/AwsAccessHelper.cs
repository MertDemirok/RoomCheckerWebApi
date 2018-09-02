using System;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.Runtime;
using Microsoft.Extensions.Configuration;

namespace DynamoDb.Libs.DynamoDb
{
    public class AwsAccessHelper : IAwsAccessHelper
    {


        IConfiguration _configuration; 


        public AwsAccessHelper(IConfiguration configuration){
            _configuration = configuration;  
        }

        public AmazonDynamoDBClient AwsConnecttion()
        {


                try
                {
                string access_Key = _configuration.GetSection("AWS").GetSection("AccessKey").Value;
                string secret_Key = _configuration.GetSection("AWS").GetSection("SecretKey").Value;

                    var awsDb = new AmazonDynamoDBClient(access_Key, secret_Key, RegionEndpoint.USEast1);
                    return awsDb;

                    //TODO: We need to add timeout for AmazonDynamoDBClient access!!

                }
                catch (AmazonDynamoDBException e) { Console.WriteLine(e.Message); return null; }
                catch (AmazonServiceException e) { Console.WriteLine(e.Message); return null; }
                catch (Exception e) { Console.WriteLine(e.Message); return null; }

        }
    }
}
