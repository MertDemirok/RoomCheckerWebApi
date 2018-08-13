/*using System;
using Amazon.DynamoDBv2;

namespace DynamoDb.Libs.DynamoDb
{
    public class ListTable : ICreateTable
    {
        
        private readonly IAmazonDynamoDB _ListTable;

        public ListTable(IAmazonDynamoDB ListTable)
        {
            _ListTable = ListTable;
        }

        public void ListDynamoDbTable(String tableName)
        {

            try
            {
                ListTempTable(tableName);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }
        public ListTempTable(){


        }

    }
}*/
