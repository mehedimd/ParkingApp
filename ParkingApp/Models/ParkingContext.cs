using Microsoft.EntityFrameworkCore;
using ParkingApp.Models.Vehicle;


namespace ParkingApp.Models
{
    public class ParkingContext : DbContext
    {
        public ParkingContext(DbContextOptions<ParkingContext> option) : base(option)
        {
            
        }
        public DbSet<Lavel> Lavels { get; set; }
        public DbSet<ParkingSpot> ParkingSpots { get; set; }
        public DbSet<ParkingVehicle> ParkingVehicles { get; set; }
    }
}
