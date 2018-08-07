using System;
namespace DynamoDb.Libs.DynamoDb
{
    public interface ICreateTable
    {
        void CreateDynamoDbTable(String tableName);
    }
}
