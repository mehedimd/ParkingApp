using System.ComponentModel.DataAnnotations.Schema;

namespace ParkingApp.Models.Vehicle
{
    public class ParkingVehicle
    {
        public int ParkingVehicleId { get; set; }
        public string? LicensePlate { get; set; }
        public string? Type { get; set; }
        [ForeignKey("ParkingSpot")]
        public int ParkingSpotId {  get; set; } = 0;
        public ParkingSpot? ParkingSpot { get; set; }

    }
}
