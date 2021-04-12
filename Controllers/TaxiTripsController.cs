using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using NYCTaxiTrips.Models;
using NYCTaxiTrips.Data;

namespace NYCTaxiTrips.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowOrigin")]
    public class TaxiTripsController: ControllerBase
    {
        private INYCTaxiTripsRepo _repository;
        public TaxiTripsController(INYCTaxiTripsRepo repository)
        {
            _repository = repository;
        }

        [HttpGet("{startLongitude}/{longitudeOffset}/{startLatitude}/{latitudeOffset}/{tripType}")]
        [EnableCors("AllowOrigin")]
        public async Task<ActionResult<IEnumerable<TaxiTrip>>> GetTaxiTrips(double startLongitude, double longitudeOffset, double startLatitude, double latitudeOffset, string tripType)
        {
            TripType tripTypeEnum;
            if(Enum.TryParse(tripType, out tripTypeEnum))
            {
                var taxitrips = await _repository.GetTaxiTrips(startLongitude,startLatitude, longitudeOffset,latitudeOffset, tripTypeEnum);
                
                return Ok(taxitrips);
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpGet("groups/{startLongitude}/{longitudeOffset}/{startLatitude}/{latitudeOffset}/{tripType}")]
        [EnableCors("AllowOrigin")]
        public async Task<ActionResult<IEnumerable<TaxiTripGroup>>> GetTaxiTripGroups(double startLongitude, double longitudeOffset, double startLatitude, double latitudeOffset, string tripType)
        {
            TripType tripTypeEnum;
            if(Enum.TryParse(tripType, out tripTypeEnum))
            {
                var taxitripgroups = await _repository.GetTaxiTripGroups(startLongitude,startLatitude, longitudeOffset,latitudeOffset, tripTypeEnum);
                return Ok(taxitripgroups);
            }
            else
            {
                return BadRequest();
            }
        }
    }
}