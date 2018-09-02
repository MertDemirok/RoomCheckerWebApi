using System;
using System.Collections.Generic;
using System.Linq;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime;
using Microsoft.AspNetCore.Mvc;

using DynamoDb.Libs.DynamoDb;
using RoomChecker.Models;
using RoomCheckerApi.Controllers;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Amazon;
using System.Globalization;

namespace RoomChecker.Controllers
{

    [Route("room-managment-api/managed-room/[action]")]
    public class RoomsController : Controller
    {
        private readonly RoomContext _context;
        private static AmazonDynamoDBClient client = new AmazonDynamoDBClient();
        private static HelpController helpController = new HelpController();
        public static string tableName = "Rooms";
        private IConfiguration configuration;
       
  

        public RoomsController(RoomContext context, IConfiguration iconfig)
        {     
            _context = context;
            configuration = iconfig;

        }


        [HttpGet]
        public async Task<IEnumerable<Rooms>> GetAllRooms()
        {
            
            AwsAccessHelper awsAccessHelper = new AwsAccessHelper(configuration);
            var awsDb = awsAccessHelper.AwsConnecttion();


            var request = new ScanRequest
            {
                TableName = tableName,
                Limit = 1,
            };

            var allRooms = new List<Rooms>();

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

                        var room = new Rooms
                        {
                            Id = Convert.ToInt32(item["Id"].N),
                            RoomNumber = Convert.ToInt32(item["RoomNumber"].N),
                            Status = Convert.ToInt32(item["Status"].N),
                            Note = item["Note"].S,
                            CheckDate = Convert.ToDateTime(item["CheckDate"].S, CultureInfo.InvariantCulture),
                            BaseMaterial = new BaseMaterials
                            {
                                Id = Convert.ToInt32(item["BaseMaterials"].M["Id"].N),
                                Name = item["BaseMaterials"].M["Name"].S,
                                Status = Convert.ToInt16(item["BaseMaterials"].M["Status"].N)
                            },
                            HouseKeeper = new HouseKeepers
                            {
                                Id = Convert.ToInt32(item["HouseKeepers"].M["Id"].N),
                                Name = item["HouseKeepers"].M["Name"].S,
                                Point = item["HouseKeepers"].M["Point"].S,
                                CheckDate = Convert.ToDateTime(item["HouseKeepers"].M["CheckDate"].S, CultureInfo.InvariantCulture)
                            },
                            creator = Convert.ToInt32(item["creator"].N) == 1 ? Creator.Admin  : Creator.Personel 

                        };
                        allRooms.Add(room);
                    }

                } while (response.LastEvaluatedKey != null && response.LastEvaluatedKey.Count > 0);
            }
            catch (AmazonDynamoDBException e) { Console.WriteLine(e.Message); }
            catch (AmazonServiceException e) { Console.WriteLine(e.Message); }
            catch (Exception e) { Console.WriteLine(e.Message); }



            _context.HouseKeeper.ToList();
            _context.BaseMaterial.ToList();

            //reponse null control yap

            return allRooms;
        }




        [HttpGet]
        public async Task<Rooms> GetbyRoomId(int id)
        {


            var allRooms = await GetAllRooms();
            var getRoom = allRooms.FirstOrDefault(t => t.RoomNumber == id);
          
            return getRoom;
        }


        [HttpGet("{id}", Name = "GetRoom")]
        public async Task<IActionResult> GetbyIdRoom(int id)
        {
            var contextRoom = _context.Room.FirstOrDefault(t => t.Id == id);

            if(contextRoom != null ){
                return new ObjectResult(contextRoom);
            }

            try
            {
                DynamoDBContext aws_Context = new DynamoDBContext(client);
                var roomRetrieved = await aws_Context.LoadAsync<Rooms>(id);


                if (roomRetrieved == null)
                {
                    return helpController.jsonResponseMessage(id, "Data not find!", 1000);
                }

                return new ObjectResult(roomRetrieved);
            }
            catch (AmazonDynamoDBException e) { Console.WriteLine(e.Message); }
            catch (AmazonServiceException e) { Console.WriteLine(e.Message); }
            catch (Exception e) { Console.WriteLine(e.Message); }

            return null;
        }

        [HttpPost]
        public IActionResult AddNewRoom([FromBody]Rooms newRoom)
        {

            DynamoDBContext aws_Context = new DynamoDBContext(client);
            if (newRoom == null)
            {
                return BadRequest();
            }
            else
            {
                try
                {

                    //If user is Admin HouseKeeper,Status is must empty 
                    Rooms room = new Rooms
                    {
                        Id = newRoom.Id,
                        RoomNumber = newRoom.RoomNumber,
                        BaseMaterial = newRoom.BaseMaterial,
                        CheckDate = DateTime.Now,
                        Status = newRoom.Status,
                        HouseKeeper = newRoom.HouseKeeper,
                        Note = newRoom.Note,
                        creator = newRoom.creator
                    };


                    aws_Context.SaveAsync(room);
                    _context.Room.Add(newRoom);
                    _context.SaveChanges();
                }

                catch (AmazonDynamoDBException e) { Console.WriteLine(e.Message); }
                catch (AmazonServiceException e) { Console.WriteLine(e.Message); }
                catch (Exception e) { Console.WriteLine(e.Message); }
            }


            return CreatedAtRoute("GetRoom", new { id = newRoom.Id }, newRoom);
        }

        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
            //TODO:EmptyPutMethod
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteByIdRoom(int id)
        {
            DynamoDBContext aws_Context = new DynamoDBContext(client);

            try
            {
                aws_Context.DeleteAsync<Rooms>(id);

                return helpController.jsonResponseMessage(id, "Data is Deleted!", 0);
            }
            catch (AmazonDynamoDBException e) { Console.WriteLine(e.Message); }
            catch (AmazonServiceException e) { Console.WriteLine(e.Message); }
            catch (Exception e) { Console.WriteLine(e.Message); }
            return null;
        }
    }
}
