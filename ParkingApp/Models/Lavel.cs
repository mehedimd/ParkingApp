using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ParkingApp.Models
{
    public class Lavel
    {
        public Lavel()
        {
            this.ParkingSpots = new List<ParkingSpot>();
        }
        public int LavelId { get; set; }
        public string? FloorName { get; set; }
        public string? VehicleType { get; set; }
        public int? TotalNumOfSpot {  get; set; } 
        public List<ParkingSpot> ParkingSpots { get; set; }
    }
}
