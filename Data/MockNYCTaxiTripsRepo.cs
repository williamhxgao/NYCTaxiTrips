using System;
using System.Threading.Tasks;
using System.Globalization;
using System.Linq;
using System.Collections.Generic;
using NYCTaxiTrips.Models;

namespace NYCTaxiTrips.Data
{
    public class MockNYCTaxiTripsRepo : INYCTaxiTripsRepo
    {
        private int _grid;
        private IEnumerable<TaxiTrip> _mockTaxiTrips = new TaxiTrip[] {new TaxiTrip{vendor_id = 2,
                                                                     pickup_datetime = DateTime.ParseExact("2015-10-19T14:42:59","yyyy-MM-dd'T'HH:mm:ss",CultureInfo.InvariantCulture),
                                                                     dropoff_datetime = DateTime.ParseExact("2015-10-19T14:47:06","yyyy-MM-dd'T'HH:mm:ss",CultureInfo.InvariantCulture),
                                                                     passenger_count = 2,
                                                                     trip_distance = 0.65M,
                                                                     pickup_longitude = -73.964736938476562,
                                                                     pickup_latitude = 40.7670783996582,
                                                                     rate_code = 1,
                                                                     dropoff_longitude = -73.962371826171875,
                                                                     dropoff_latitude = 40.773128509521484,
                                                                     payment_type = 2,
                                                                     fare_amount = 5.0M,
                                                                     extra = 0.0M,
                                                                     mta_tax = 0.5M,
                                                                     tip_amount = 0.0M,
                                                                     tolls_amount = 0.0M,
                                                                     imp_surcharge = 0.3M,
                                                                     total_amount = 5.8M
                                                                    },
                                                        new TaxiTrip{vendor_id = 1,
                                                                     pickup_datetime = DateTime.ParseExact("2015-11-24T12:28:25","yyyy-MM-dd'T'HH:mm:ss",CultureInfo.InvariantCulture),
                                                                     dropoff_datetime = DateTime.ParseExact("2015-11-24T12:37:33","yyyy-MM-dd'T'HH:mm:ss",CultureInfo.InvariantCulture),
                                                                     passenger_count = 1,
                                                                     trip_distance = 0.6M,
                                                                     pickup_longitude = -73.985336303710938,
                                                                     pickup_latitude = 40.7440185546875,
                                                                     rate_code = 1,
                                                                     dropoff_longitude = -73.9911117553711,
                                                                     dropoff_latitude = 40.748569488525391,
                                                                     payment_type = 1,
                                                                     fare_amount = 7.0M,
                                                                     extra = 0.0M,
                                                                     mta_tax = 0.5M,
                                                                     tip_amount = 1.55M,
                                                                     tolls_amount = 0.0M,
                                                                     imp_surcharge = 0.3M,
                                                                     total_amount = 9.35M
                                                                    },
                                                        new TaxiTrip{vendor_id = 1,
                                                                     pickup_datetime = DateTime.ParseExact("2015-11-21T20:21:37","yyyy-MM-dd'T'HH:mm:ss",CultureInfo.InvariantCulture),
                                                                     dropoff_datetime = DateTime.ParseExact("2015-11-21T20:44:19","yyyy-MM-dd'T'HH:mm:ss",CultureInfo.InvariantCulture),
                                                                     passenger_count = 1,
                                                                     trip_distance = 2.65M,
                                                                     pickup_longitude = -73.972587585449219,
                                                                     pickup_latitude = 40.755905151367188,
                                                                     rate_code = 1,
                                                                     dropoff_longitude = -74.0,
                                                                     dropoff_latitude = 40.730560302734375,
                                                                     payment_type = 2,
                                                                     fare_amount = 15.0M,
                                                                     extra = 0.0M,
                                                                     mta_tax = 0.5M,
                                                                     tip_amount = 0.0M,
                                                                     tolls_amount = 0.0M,
                                                                     imp_surcharge = 0.3M,
                                                                     total_amount = 16.3M
                                                                    }};

        public MockNYCTaxiTripsRepo(int grid)
        {
            _grid = grid;
        }
        public async Task<IEnumerable<TaxiTripGroup>> GetTaxiTripGroups(double startLongitude, double startLatitude, double longitudeOffset, double latitudeOffset, TripType tripType)
        {
            IEnumerable<double[]> ranges = new List<double[]>(); 
            
            for(int i = 0; i < _grid; i++)
                for(int j = 0; j < _grid; j++)
                    ranges.Append(new double[]{startLongitude + longitudeOffset / _grid * (i + 1), startLatitude + latitudeOffset / _grid * (j + 1)});

            if(tripType == TripType.PickUp)
            {
               return await Task.FromResult(_mockTaxiTrips.Where(t=>t.pickup_longitude > startLongitude && 
                                                                  t.pickup_longitude <= startLongitude + longitudeOffset &&
                                                                  t.pickup_latitude > startLatitude &&
                                                                  t.pickup_latitude <= startLatitude + latitudeOffset)
                                                            .GroupBy(g=> ranges.FirstOrDefault(r=>r[0] > g.pickup_longitude && r[1] > g.pickup_latitude))
                                                            .Select(g=> new TaxiTripGroup{trips = g.Count(),
                                                                                            average_distance = g.Average(t=>t.trip_distance),
                                                                                            average_passenger_count = (decimal)g.Average(t=>t.passenger_count), 
                                                                                            average_total_amount = g.Average(t=>t.total_amount),
                                                                                            centre_pickup_longitude = g.Average(t=>t.pickup_longitude),
                                                                                            centre_pickup_latitude = g.Average(t=>t.pickup_latitude),
                                                                                            centre_dropoff_longitude = g.Average(t=>t.dropoff_longitude),
                                                                                            centre_dropoff_latitude = g.Average(t=>t.dropoff_latitude)}));
            }
            else
            {
                return await Task.FromResult(_mockTaxiTrips.Where(t=>t.dropoff_longitude > startLongitude && 
                                                                     t.dropoff_longitude <= startLongitude + longitudeOffset &&
                                                                     t.dropoff_latitude > startLatitude &&
                                                                     t.dropoff_latitude <= startLatitude + latitudeOffset)
                                                            .GroupBy(g=> ranges.FirstOrDefault(r=>r[0] > g.dropoff_longitude && r[1] > g.dropoff_latitude))
                                                            .Select(g=> new TaxiTripGroup{trips = g.Count(),
                                                                                            average_distance = g.Average(t=>t.trip_distance),
                                                                                            average_passenger_count = (decimal)g.Average(t=>t.passenger_count), 
                                                                                            average_total_amount = g.Average(t=>t.total_amount),
                                                                                            centre_pickup_longitude = g.Average(t=>t.pickup_longitude),
                                                                                            centre_pickup_latitude = g.Average(t=>t.pickup_latitude),
                                                                                            centre_dropoff_longitude = g.Average(t=>t.dropoff_longitude),
                                                                                            centre_dropoff_latitude = g.Average(t=>t.dropoff_latitude)}));
            }
        }

            

        public async Task<IEnumerable<TaxiTrip>> GetTaxiTrips(double startLongitude, double startLatitude, double longitudeOffset, double latitudeOffset, TripType tripType)
        {
            if(tripType == TripType.PickUp)
                return await Task.FromResult(_mockTaxiTrips.Where(t=>t.pickup_longitude > startLongitude && 
                                                                  t.pickup_longitude <= startLongitude + longitudeOffset &&
                                                                  t.pickup_latitude > startLatitude &&
                                                                  t.pickup_latitude <= startLatitude + latitudeOffset));
            else
                return await Task.FromResult(_mockTaxiTrips.Where(t=>t.dropoff_longitude > startLongitude && 
                                                                  t.dropoff_longitude <= startLongitude + longitudeOffset &&
                                                                  t.dropoff_latitude > startLatitude &&
                                                                  t.dropoff_latitude <= startLatitude + latitudeOffset));
        }
    }
}