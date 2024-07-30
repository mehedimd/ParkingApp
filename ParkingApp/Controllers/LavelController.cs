using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ParkingApp.Models;

namespace ParkingApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LavelController : ControllerBase
    {
        private readonly ParkingContext db;
        public LavelController(ParkingContext context)
        {
            this.db = context;
        }
        // Get All Lavel or Floor (with parking spot)
        [HttpGet]
        public IActionResult GetAllLavel()
        {
            var allLavel = db.Lavels.Include(p => p.ParkingSpots).ToList();
            return Ok(allLavel);
        }

        // Get Indivisual Lavel or Floor (with parking spot)
        [HttpGet("{id}")]
        public IActionResult GetLavelById(int id)
        {
            var lavel = db.Lavels.Include(p => p.ParkingSpots).Where(l => l.LavelId == id).FirstOrDefault();
            return Ok(lavel);
        }

        // Create Lavel or Floor (with parking spot)
        [HttpPost]
        public IActionResult Create(Lavel lavel)
        {
            using(var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    if (lavel != null)
                    {
                        foreach(var spot in lavel.ParkingSpots)
                        {
                            spot.VehicleType = lavel.VehicleType;
                        }
                        lavel.TotalNumOfSpot = lavel.ParkingSpots.Count();


                        db.Lavels.Add(lavel);
                        if (db.SaveChanges() > 0)
                        {
                            transaction.Commit();
                            return Ok();
                        }

                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return BadRequest(ex.Message);
                }
            }
            return BadRequest();
        }
        // Update Lavel or Floor (with parking spot)
        [HttpPut("{id}")]
        public IActionResult Update(Lavel lavel)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    if (lavel != null)
                    {
                        // remove previous spot record
                        var previousSpotsForDelete = db.ParkingSpots.Where(p=>p.LavelId == lavel.LavelId).ToList();

                        List<ParkingSpot> listForUpdate = new List<ParkingSpot>();
                        List<ParkingSpot> listForAdd = new List<ParkingSpot>();

                        foreach (var spot in lavel.ParkingSpots)
                        {
                            spot.VehicleType = lavel.VehicleType;

                            if (spot.ParkingSpotId > 0)
                            {
                                previousSpotsForDelete = previousSpotsForDelete.Where(p=>p.ParkingSpotId != spot.ParkingSpotId).ToList();
                                listForUpdate.Add(spot);
                            }
                            else
                            {
                                listForAdd.Add(spot);
                            }

                        }
                        db.ParkingSpots.RemoveRange(previousSpotsForDelete);
                        db.SaveChanges();
                        lavel.TotalNumOfSpot = lavel.ParkingSpots.Count();

                        db.Entry(lavel).State = EntityState.Modified;
                        if (db.SaveChanges() > 0)
                        {
                                db.ParkingSpots.AddRange(listForAdd);
                                if (db.SaveChanges() > 0)
                                {

                                db.ParkingSpots.UpdateRange(listForUpdate);
                                if (db.SaveChanges() > 0)
                                {
                                    transaction.Commit();
                                    return Ok();
                                }
                                }
                            
                            
                        }

                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return BadRequest(ex.Message);
                }
            }
            return BadRequest();
        }

        // Delete Lavel or Floor (with parking spot)
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var lavel = db.Lavels.Find(id);
            if(lavel != null)
            {
                db.Lavels.Remove(lavel);
                if (db.SaveChanges() > 0)
                {
                    return Ok();
                }
            }           
            return BadRequest();
        }
    }
}
