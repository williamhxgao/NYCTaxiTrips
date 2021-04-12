using System.Collections.Generic;
using System.Threading.Tasks;
using NYCTaxiTrips.Models;

namespace NYCTaxiTrips.Data
{
    public interface INYCTaxiTripsRepo
    {
        Task<IEnumerable<TaxiTrip>> GetTaxiTrips(double startLongitude, double startLatitude, double longitudeOffset, double latitudeOffset, TripType tripType);
        Task<IEnumerable<TaxiTripGroup>> GetTaxiTripGroups(double startLongitude, double startLatitude, double longitudeOffset, double latitudeOffset, TripType tripType);
    }

    public enum TripType
    {
        PickUp,
        DropOff
    }
}