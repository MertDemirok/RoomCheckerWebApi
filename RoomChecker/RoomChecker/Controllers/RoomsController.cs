using System;
using System.Collections.Generic;
using System.Linq;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Runtime;
using Microsoft.AspNetCore.Mvc;

using RoomChecker.Models;
using RoomCheckerApi.Models;

namespace RoomChecker.Controllers
{

    [Route("room-managment/managed-room/[action][controller]")]
    public class RoomsController : Controller
    {
        private readonly RoomContext _context;
        private static AmazonDynamoDBClient client = new AmazonDynamoDBClient();
       

        public RoomsController(RoomContext context)
        {

            _context = context;


        }

        [HttpGet]
        public IEnumerable<Rooms> Get()
        {


                DynamoDBContext aws_Context = new DynamoDBContext(client);

          

            //geti düzelt !!! all room
            _context.HouseKeeper.ToList();
            _context.BaseMaterial.ToList();

            return _context.Room.ToList();
        }

        [HttpGet("{id}", Name = "GetRoom")]
        public async System.Threading.Tasks.Task<IActionResult> GetAsync(int id)
        {
            var product = _context.Room.FirstOrDefault(t => t.Id == id);
            try
            {
                DynamoDBContext aws_Context = new DynamoDBContext(client);
                var roomRetrieved = await aws_Context.LoadAsync<Rooms>(id);


                if (roomRetrieved == null)
                {
                    //Responseları Methoda cevir
                    return new ObjectResult(new
                    {

                        ID = id,
                        Title = "Data not find!",
                        ErrorCode = 1001

                    });
                }

                return new ObjectResult(roomRetrieved);
            }
            catch (AmazonDynamoDBException e) { Console.WriteLine(e.Message); }
            catch (AmazonServiceException e) { Console.WriteLine(e.Message); }
            catch (Exception e) { Console.WriteLine(e.Message); }

            return null;
        }

        [HttpPost]
        public IActionResult Post([FromBody]Rooms newRoom)
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
            //TODO:Yazılacak
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            DynamoDBContext aws_Context = new DynamoDBContext(client);

            try
            {
                aws_Context.DeleteAsync<Rooms>(id);
               
                return new ObjectResult(new
                {

                    ID = id,
                    Title = "Data is Deleted!",
                    ErrorCode = 0

                });
            }
            catch (AmazonDynamoDBException e) { Console.WriteLine(e.Message); }
            catch (AmazonServiceException e) { Console.WriteLine(e.Message); }
            catch (Exception e) { Console.WriteLine(e.Message); }
            return null;
        }
    }
}
