using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime;
using DynamoDb.Libs.DynamoDb;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using RoomChecker.Models;
using RoomCheckerApi.Controllers;


namespace RoomChecker.Controllers
{
	[Route("houseKeeper-managment-api/managed-houseKeeper/[action]")]
    public class HouseKeepersController : Controller
    {
        private readonly RoomContext _context;
        private static AmazonDynamoDBClient client = new AmazonDynamoDBClient();
        private static HelpController helpController = new HelpController();
        public static string tableName = "HouseKeepers";
        private IConfiguration configuration;
       
        public HouseKeepersController(RoomContext context, IConfiguration iconfig)
        {
            _context = context; 
            configuration = iconfig;

        }


        [HttpGet]
        public async Task<HouseKeepers> GetbyHouseKeeperName(string name)
        {


            var allHouseKeepers = await GetAllHouseKeepers();
            //isim arama geliştirebilirsin.
            var getHouseKeepers = allHouseKeepers.FirstOrDefault(t => String.Format(t.Name).ToUpper() == name.ToUpper());

            return getHouseKeepers;
        }


        [HttpGet]
        public async Task<IEnumerable<HouseKeepers>> GetAllHouseKeepers()
        {

            AwsAccessHelper awsAccessHelper = new AwsAccessHelper(configuration);
            var awsDb = awsAccessHelper.AwsConnecttion();


            var request = new ScanRequest
            {
                TableName = tableName,
                Limit = 1,
            };

            var allHouseKeepers = new List<HouseKeepers>();

            ScanResponse response = null;


            try
            {
                do
                {
                    if (response != null)
                        request.ExclusiveStartKey = response.LastEvaluatedKey;

                    response = await awsDb.ScanAsync(request);

                    foreach (var item in response.Items)
                    {

                        var houseKeepers = new HouseKeepers
                        {
                            Id = Convert.ToInt32(item["Id"].N),
                            Name = item["Name"].S,
                            Point = item["Point"].S,
                            CheckDate = Convert.ToDateTime(item["CheckDate"].S, CultureInfo.InvariantCulture)
                        };
                        allHouseKeepers.Add(houseKeepers);
                    }

                } while (response.LastEvaluatedKey != null && response.LastEvaluatedKey.Count > 0);
            }
            catch (AmazonDynamoDBException e) { Console.WriteLine(e.Message); }
            catch (AmazonServiceException e) { Console.WriteLine(e.Message); }
            catch (Exception e) { Console.WriteLine(e.Message); }



            _context.HouseKeeper.ToList();
            _context.BaseMaterial.ToList();



            return allHouseKeepers;
        }

        [HttpGet("{id}", Name = "GetHouseKeeper")]
        public async Task<IActionResult> GetbyIdHouseKeepers(int id)
        {
            var contextRoom = _context.HouseKeeper.FirstOrDefault(t => t.Id == id);

            if (contextRoom != null)
            {
                return new ObjectResult(contextRoom);
            }

            try
            {
                DynamoDBContext aws_Context = new DynamoDBContext(client);
                var houseKeeperRetrieved = await aws_Context.LoadAsync<HouseKeepers>(id);


                if (houseKeeperRetrieved == null)
                {
                    return helpController.jsonResponseMessage(id, "Data not find!", 1000);
                }

                return new ObjectResult(houseKeeperRetrieved);
            }
            catch (AmazonDynamoDBException e) { Console.WriteLine(e.Message); }
            catch (AmazonServiceException e) { Console.WriteLine(e.Message); }
            catch (Exception e) { Console.WriteLine(e.Message); }

            return null;
        }


        [HttpPost]
        public IActionResult AddNewHouseKeeper([FromBody]HouseKeepers newHouseKeeper)
        {

            DynamoDBContext aws_Context = new DynamoDBContext(client);
            if (newHouseKeeper == null)
            {
                return BadRequest();
            }
            else
            {
                try
                {
                    HouseKeepers housekeeper = new HouseKeepers
                    {
                        Id=newHouseKeeper.Id,
                        Name=newHouseKeeper.Name,
                        CheckDate=newHouseKeeper.CheckDate,
                        Point= newHouseKeeper.Point
                    };


                    aws_Context.SaveAsync(housekeeper);

                    _context.HouseKeeper.Add(newHouseKeeper);
                    _context.SaveChanges();
                }

                catch (AmazonDynamoDBException e) { Console.WriteLine(e.Message); }
                catch (AmazonServiceException e) { Console.WriteLine(e.Message); }
                catch (Exception e) { Console.WriteLine(e.Message); }
            }


            return CreatedAtRoute("GetHouseKeeper", new { id = newHouseKeeper.Id }, newHouseKeeper);
        }


        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
            //TODO:EmptyPutMethod
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteByIdHouseKeepers(int id)
        {
            DynamoDBContext aws_Context = new DynamoDBContext(client);

            try
            {
                aws_Context.DeleteAsync<HouseKeepers>(id);

                return helpController.jsonResponseMessage(id, "Data is Deleted!", 0);
            }
            catch (AmazonDynamoDBException e) { Console.WriteLine(e.Message); }
            catch (AmazonServiceException e) { Console.WriteLine(e.Message); }
            catch (Exception e) { Console.WriteLine(e.Message); }
            return null;
        }
    }
}
