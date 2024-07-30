using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ParkingApp.Models;
using ParkingApp.Models.Vehicle;

namespace ParkingApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ParkingController : ControllerBase
    {
        private readonly ParkingContext db;
        public ParkingController(ParkingContext context)
        {
            this.db = context;
        }

        // get all parking vehicle
        [HttpGet]
        public IActionResult GetAll()
        {
            var parkedVehicleWithSpotSerial = from pv in db.ParkingVehicles
                                            join spot in db.ParkingSpots
                                                on pv.ParkingSpotId equals spot.ParkingSpotId
                                            join lavel in db.Lavels
                                                on spot.LavelId equals lavel.LavelId
                                            select new
                                            {
                                                pv.ParkingVehicleId,
                                                pv.Type,
                                                pv.LicensePlate,
                                                pv.ParkingSpotId,
                                                spot.SerialNo,
                                                lavel.FloorName
                                                
                                            };
            // var parkedVehicle = db.ParkingVehicles.ToList();
            return Ok(parkedVehicleWithSpotSerial.ToList());
        }
        // Park or Create Parking Vehicle
        [HttpPost]
        public IActionResult Create(ParkingVehicle vehicle)
        {
            var parkingSpots = db.ParkingSpots.Where(v => v.VehicleType == vehicle.Type).ToList();

            using (var tran = db.Database.BeginTransaction())
            {
                try
                {
                    foreach (var spot in parkingSpots)
                    {
                        if (spot.IsAvailable)
                        {
                            spot.IsAvailable = false;
                            db.Entry(spot).State = EntityState.Modified;
                            if (db.SaveChanges() > 0)
                            {
                                vehicle.ParkingSpotId = spot.ParkingSpotId;
                                db.ParkingVehicles.Add(vehicle);
                                if (db.SaveChanges() > 0)
                                {
                                    tran.Commit();
                                    return Ok();
                                }
                            }


                        }
                    }
                    
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    return BadRequest(new { Message = ex.Message });
                }
            }
            return NotFound(new { Message = "Parking Spot Not Available" });
        }
        // UnPark or delete Parking item
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            using (var tran = db.Database.BeginTransaction())
            {
                try
                {
                    var parkedVehicle = db.ParkingVehicles.Find(id);
                    if (parkedVehicle != null)
                    {
                        var parkedSpot = db.ParkingSpots.Find(parkedVehicle.ParkingSpotId);
                        if (parkedSpot != null)
                        {
                            parkedSpot.IsAvailable = true;
                            db.ParkingSpots.Update(parkedSpot);
                            if (db.SaveChanges() > 0)
                            {
                                db.ParkingVehicles.Remove(parkedVehicle);
                                if (db.SaveChanges() > 0)
                                {
                                    tran.Commit();
                                    return Ok();
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    return BadRequest(ex.Message);
                    
                }
            }
            return BadRequest();
        }
    }
}
