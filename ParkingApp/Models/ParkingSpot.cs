using ParkingApp.Models.Vehicle;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ParkingApp.Models
{
    public class ParkingSpot
    {
        public int ParkingSpotId { get; set; }
        public string? SerialNo {  get; set; }
        public bool IsAvailable {  get; set; } = true;
        public string? VehicleType { get; set; }
        [ForeignKey("Lavel")]
        public int LavelId { get; set; }
        public Lavel? Lavel {  get; set; }
    }
}
