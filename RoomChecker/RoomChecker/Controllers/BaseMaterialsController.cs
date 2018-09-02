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
    [Route("BaseMaterial-managment-api/managed-BaseMaterial/[action]")]
    public class BaseMaterialsController : Controller
    {
        private readonly RoomContext _context;
        private static AmazonDynamoDBClient client = new AmazonDynamoDBClient();
        private static HelpController helpController = new HelpController();
        public static string tableName = "BaseMaterials";
        private IConfiguration configuration;

        public BaseMaterialsController(RoomContext context, IConfiguration iconfig)
        {
            _context = context;
            configuration = iconfig;

        }


        [HttpGet]
        public async Task<BaseMaterials> GetbyBaseMaterialName(string name)
        {
            

            var allBaseMaterials = await GetAllBaseMaterials();
            //isim arama geliştirebilirsin.
            var getBaseMaterials = allBaseMaterials.FirstOrDefault(t => String.Format(t.Name).ToUpper() == name.ToUpper());

            return getBaseMaterials;
        }


        [HttpGet]
        public async Task<IEnumerable<BaseMaterials>> GetAllBaseMaterials()
        {

            AwsAccessHelper awsAccessHelper = new AwsAccessHelper(configuration);
            var awsDb = awsAccessHelper.AwsConnecttion();


            var request = new ScanRequest
            {
                TableName = tableName,
                Limit = 1,
            };

            var allBaseMaterials = new List<BaseMaterials>();

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

                        var BaseMaterials = new BaseMaterials
                        {
                            Id = Convert.ToInt32(item["Id"].N),
                            Name = item["Name"].S,
                            Status = Convert.ToInt16(item["Status"].N)
                        };
                        allBaseMaterials.Add(BaseMaterials);
                    }

                } while (response.LastEvaluatedKey != null && response.LastEvaluatedKey.Count > 0);
            }
            catch (AmazonDynamoDBException e) { Console.WriteLine(e.Message); }
            catch (AmazonServiceException e) { Console.WriteLine(e.Message); }
            catch (Exception e) { Console.WriteLine(e.Message); }



            _context.BaseMaterial.ToList();
            _context.BaseMaterial.ToList();



            return allBaseMaterials;
        }

        [HttpGet("{id}", Name = "GetBaseMaterial")]
        public async Task<IActionResult> GetbyIdBaseMaterials(int id)
        {
            var contextRoom = _context.BaseMaterial.FirstOrDefault(t => t.Id == id);

            if (contextRoom != null)
            {
                return new ObjectResult(contextRoom);
            }

            try
            {
                DynamoDBContext aws_Context = new DynamoDBContext(client);
                var BaseMaterialRetrieved = await aws_Context.LoadAsync<BaseMaterials>(id);


                if (BaseMaterialRetrieved == null)
                {
                    return helpController.jsonResponseMessage(id, "Data not find!", 1000);
                }

                return new ObjectResult(BaseMaterialRetrieved);
            }
            catch (AmazonDynamoDBException e) { Console.WriteLine(e.Message); }
            catch (AmazonServiceException e) { Console.WriteLine(e.Message); }
            catch (Exception e) { Console.WriteLine(e.Message); }

            return null;
        }


        [HttpPost]
        public IActionResult AddNewBaseMaterial([FromBody]BaseMaterials newBaseMaterial)
        {

            DynamoDBContext aws_Context = new DynamoDBContext(client);
            if (newBaseMaterial == null)
            {
                return BadRequest();
            }
            else
            {
                try
                {
                    BaseMaterials BaseMaterial = new BaseMaterials
                    {
                        Id = newBaseMaterial.Id,
                        Name = newBaseMaterial.Name,
                        Status = newBaseMaterial.Status
                    };


                    aws_Context.SaveAsync(BaseMaterial);

                    _context.BaseMaterial.Add(newBaseMaterial);
                    _context.SaveChanges();
                }

                catch (AmazonDynamoDBException e) { Console.WriteLine(e.Message); }
                catch (AmazonServiceException e) { Console.WriteLine(e.Message); }
                catch (Exception e) { Console.WriteLine(e.Message); }
            }


            return CreatedAtRoute("GetHouseKeeper", new { id = newBaseMaterial.Id }, newBaseMaterial);
        }


        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
            //TODO:EmptyPutMethod
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteByIdBaseMaterials(int id)
        {
            DynamoDBContext aws_Context = new DynamoDBContext(client);

            try
            {
                aws_Context.DeleteAsync<BaseMaterials>(id);

                return helpController.jsonResponseMessage(id, "Data is Deleted!", 0);
            }
            catch (AmazonDynamoDBException e) { Console.WriteLine(e.Message); }
            catch (AmazonServiceException e) { Console.WriteLine(e.Message); }
            catch (Exception e) { Console.WriteLine(e.Message); }
            return null;
        }
    }
}
