using System;
using System.Collections.Generic;
using System.Threading;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;

namespace DynamoDb.Libs.DynamoDb
{
    public class CreateTable : ICreateTable
    {

        private readonly IAmazonDynamoDB _CreateTable;

        public CreateTable(IAmazonDynamoDB CreateTable)
        {
            _CreateTable = CreateTable;
        }


        public void CreateDynamoDbTable(String tableName){
            
            try
            {
                CreateTempTable(tableName);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        public void  CreateTempTable(String tableName){

            Console.WriteLine("Create Table");
            var request = new CreateTableRequest
            {
                AttributeDefinitions = new List<AttributeDefinition>
        {
            new AttributeDefinition
            {
                AttributeName = "Id",
                AttributeType = "N"
            },
            new AttributeDefinition
            {
                AttributeName = "ReplyDateTime",
                AttributeType = "N"
            }
        },
                KeySchema = new List<KeySchemaElement>
        {
            new KeySchemaElement
            {
                AttributeName = "Id",
                KeyType = "HASH" // Partition Key
            },
            new KeySchemaElement
            {
                AttributeName = "ReplyDateTime",
                KeyType = "Range" // Sort Key
            }
        },
                ProvisionedThroughput = new ProvisionedThroughput
                {
                    ReadCapacityUnits = 5,
                    WriteCapacityUnits = 5
                },
                TableName = tableName
            };

            var response = _CreateTable.CreateTableAsync(request);

            WaitUntilTableReady(tableName);

        }

        public void WaitUntilTableReady(string tableName)
        {
            string status = null;

            do
            {
                Thread.Sleep(5000);
                try
                {
                    var res = _CreateTable.DescribeTableAsync(new DescribeTableRequest
                    {
                        TableName = tableName
                    });

                    status = res.Result.Table.TableStatus;
                }
                catch (ResourceNotFoundException)
                {

                }

            } while (status != "ACTIVE");
            {
                Console.WriteLine("Table Created Successfully");
            }
        }
    }
}
