using System;
using Microsoft.EntityFrameworkCore;
using RoomChecker.Models;

namespace RoomChecker.Models
{
    public class RoomContext : DbContext {

            public DbSet<HouseKeepers> HouseKeeper { get; set; }
            public DbSet<BaseMaterials> BaseMaterial { get; set; }
            public DbSet<Rooms> Room { get; set; }

            public RoomContext(DbContextOptions<RoomContext> options): base(options){}
   }
}                                 