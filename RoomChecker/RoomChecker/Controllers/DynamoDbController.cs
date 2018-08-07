using System;
using DynamoDb.Libs.DynamoDb;
using Microsoft.AspNetCore.Mvc;

namespace RoomChecker.Controllers
{
    [Route("api/DynamoDb")]
    public class DynamoDbController : Controller
    {
        private readonly ICreateTable _CreateTable;
        private static readonly string tableName = "TempDynamoDbTable";


        public DynamoDbController(ICreateTable CreateTable)
        {
            _CreateTable = CreateTable;
        }

        [Route("createtable")]
        public IActionResult CreateDynamoDbTable()
        {
            _CreateTable.CreateDynamoDbTable(tableName);

            return Ok();
        }
    }
}
