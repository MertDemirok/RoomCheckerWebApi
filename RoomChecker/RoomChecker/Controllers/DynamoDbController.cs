using System;
using System.Collections.Generic;
using Amazon.DynamoDBv2.Model;
using DynamoDb.Libs.DynamoDb;
using Microsoft.AspNetCore.Mvc;

namespace RoomChecker.Controllers
{
    [Route("api/DynamoDb")]
    public class DynamoDbController : Controller
    {
        private readonly ICreateTable _CreateTable;
        private static readonly string tableName = "TempDynamoDbTable";
        private readonly IPutItem _putItem;

        public DynamoDbController(ICreateTable CreateTable, IPutItem putItem)
        {
            _CreateTable = CreateTable;
            _putItem = putItem;
        }

        [Route("createtable")]
        public IActionResult CreateDynamoDbTable()
        {
            _CreateTable.CreateDynamoDbTable(tableName);

            return Ok();
        }

        [Route ("putitems")]
        public IActionResult PutItem([FromQuery]int id ,string replyDateTime)
        {
            var items = new Dictionary<string, AttributeValue>{
                {"Id", new AttributeValue {N = id.ToString()}},
                {"ReplyDateTime", new AttributeValue {N = replyDateTime}}
            };
            _putItem.AddNewEntry(items,tableName);

            return Ok();
        }
    }
}
