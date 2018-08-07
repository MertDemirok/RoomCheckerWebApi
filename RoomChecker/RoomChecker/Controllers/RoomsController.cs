using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RoomChecker.Models;
using RoomCheckerApi.Models;

namespace RoomChecker.Controllers
{

    [Route("Room/restapi/[controller]")]
    public class RoomsController : Controller
    {

        private readonly RoomContext _context;
        /*private readonly DynamoDbController dynamoDb;*/


        public RoomsController(RoomContext context)
        {
            _context = context;

            //Generalcs yazmalısın oradan yonetılmelı db cache log ıslerı
           /* dynamoDb.CreateDynamoDbTable();*/

            if (_context.Room.Count() == 0)
            {
                _context.Room.Add(new Rooms { Id = 19201, RoomNumber = 1 ,  BaseMaterial = new BaseMaterials { Id = 100, Name = "Yatak Örtüsü", Status = 1 },
                                                                          HouseKeeper = new HouseKeepers{Id=1,CheckDate=DateTime.Now,Name="Ayşe Kek",Point="10"} });
                _context.Room.Add(new Rooms { Id = 23942, RoomNumber = 2 ,BaseMaterial = new BaseMaterials { Id = 101, Name = "Banyo", Status = 0 } });
                _context.Room.Add(new Rooms { Id = 30021, RoomNumber = 3, BaseMaterial = new BaseMaterials { Id = 102, Name = "Klozet", Status = 0 } });
                _context.Room.Add(new Rooms { Id = 30492, RoomNumber = 4 ,BaseMaterial = new BaseMaterials { Id = 103, Name = "Zemin", Status = 1 } });

                _context.SaveChanges();
            }

        }

        [HttpGet]
        public IEnumerable<Rooms> Get()
        {
            _context.HouseKeeper.ToList();
            _context.BaseMaterial.ToList();

            return _context.Room.ToList();
        }

        [HttpGet("{id}", Name="GetRoom")]
        public IActionResult Get(int id)
        {
            var product = _context.Room.FirstOrDefault(t => t.Id == id);
            if (product == null)
            {
                return NotFound();
            }
            return new ObjectResult(product);
        }

        [HttpPost]
        public IActionResult Post([FromBody]Rooms newRoom)
        {
            if(newRoom == null){
                return BadRequest();
            }
            _context.Room.Add(newRoom);
            _context.SaveChanges();

            return CreatedAtRoute("GetRoom",new {id=newRoom.Id},newRoom);  
        }

        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
            //TODO:Yazılacak
        }

        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            //TODO:Yazılacak
        }
    }
}
