using System;

using System.Globalization;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using NYCTaxiTrips.Models;

namespace NYCTaxiTrips.Data
{
    public class BigQueryNYCTaxiTripsRepo : INYCTaxiTripsRepo
    {
        private readonly IGoogeBigQueryRepo _repository;
        private readonly int _grid;

        public BigQueryNYCTaxiTripsRepo(IGoogeBigQueryRepo repository, int grid)
        {
            _repository = repository;
            _grid = grid;
        }

        public async Task<IEnumerable<TaxiTripGroup>> GetTaxiTripGroups(double startLongitude, 
                                                                        double startLatitude, 
                                                                        double longitudeOffset, 
                                                                        double latitudeOffset, 
                                                                        TripType tripType)
        {
            List<ValueTuple<double, double, int>> range = new List<ValueTuple<double, double, int>>();
            var divLongitudeOffset = (double)longitudeOffset / _grid;
            var divLatitudeOffset = (double)latitudeOffset / _grid;

            for(int i = 0; i < _grid; i++)
            {
                for(int j = 0; j < _grid; j++)
                {
                    range.Add((startLongitude + divLongitudeOffset * i, startLatitude + divLatitudeOffset * j, i * _grid + j));
                }
            }
                
            var groups = string.Join(' ', range.Select(r => $@"WHEN pickup_longitude > {r.Item1} AND 
                                                                    pickup_longitude <= {r.Item1 + divLongitudeOffset} AND 
                                                                    pickup_latitude > {r.Item2} AND 
                                                                    pickup_latitude <= {r.Item2 + divLatitudeOffset} 
                                                               THEN {r.Item3}"));

            
            var strType = tripType == TripType.PickUp ? "pickup" : "dropoff";
            

                    var query = $@" SELECT      CASE {groups} ELSE -1 END loc_range, 
                                                COUNT(*) trips, 
                                                AVG(pickup_latitude) centre_pickup_latitude,
                                                AVG(pickup_longitude) centre_pickup_longitude,
                                                AVG(dropoff_latitude) centre_dropoff_latitude,
                                                AVG(dropoff_longitude) centre_dropoff_longitude,
                                                AVG(passenger_count) average_passenger_count,
                                                AVG(trip_distance) average_distance,
                                                AVG(total_amount) average_total_amount
                                    FROM        [FULL_TABLE_NAME]
                                    WHERE       {strType}_longitude > {startLongitude} AND 
                                                {strType}_longitude <= {startLongitude + longitudeOffset} AND 
                                                {strType}_latitude > {startLatitude} AND 
                                                {strType}_latitude <= {startLatitude + latitudeOffset}  
                                    GROUP BY    1 
                                    ORDER BY    1";

            var result = await _repository.GetData(query);
            return await Task.FromResult(result.Select(row => new TaxiTripGroup{id = Guid.NewGuid().ToString(),
                                                                                trips = (int)(long)row["trips"],
                                                                                average_passenger_count = (decimal)(double)row["average_passenger_count"],
                                                                                average_distance = (decimal)(double)row["average_distance"],
                                                                                centre_pickup_longitude = (double)row["centre_pickup_longitude"],
                                                                                centre_pickup_latitude = (double)row["centre_pickup_latitude"],
                                                                                centre_dropoff_longitude = (double)row["centre_dropoff_longitude"],
                                                                                centre_dropoff_latitude = (double)row["centre_dropoff_latitude"],
                                                                                average_total_amount = (decimal)(double)row["average_total_amount"]
                                                                                }));
        }

        public async Task<IEnumerable<TaxiTrip>> GetTaxiTrips(double startLongitude, double startLatitude, double longitudeOffset, double latitudeOffset, TripType tripType)
        {
            var strType = tripType == TripType.PickUp ? "pickup" : "dropoff";

            var query = $@"SELECT   vendor_id,
                                    pickup_datetime,
                                    dropoff_datetime, 
                                    passenger_count, 
                                    trip_distance,
                                    pickup_longitude, 
                                    pickup_latitude, 
                                    rate_code,
                                    dropoff_longitude,
                                    dropoff_latitude,
                                    payment_type,
                                    fare_amount,
                                    extra,
                                    mta_tax,
                                    tip_amount,
                                    tolls_amount,
                                    imp_surcharge,
                                    total_amount
                            FROM `[FULL_TABLE_NAME]`
                            WHERE {strType}_longitude > {startLongitude} AND 
                                  {strType}_longitude <= {startLongitude + longitudeOffset} AND 
                                  {strType}_latitude > {startLatitude} AND 
                                  {strType}_latitude <= {startLatitude + latitudeOffset}";
            
            var result = await _repository.GetData(query);
            return await Task.FromResult(result.Select(row => new TaxiTrip{vendor_id = (int)row["vendor_id"],
                                                                           pickup_datetime = DateTime.ParseExact(row["pickup_datetime"].ToString(),"yyyy-MM-dd'T'HH:mm:ss",CultureInfo.InvariantCulture),
                                                                           dropoff_datetime = DateTime.ParseExact(row["dropoff_datetime"].ToString(),"yyyy-MM-dd'T'HH:mm:ss",CultureInfo.InvariantCulture), 
                                                                           passenger_count = (int)row["passenger_count"],
                                                                           trip_distance = (decimal)row["trip_distance"],
                                                                           pickup_longitude = (double)row["pickup_longitude"],
                                                                           pickup_latitude = (double)row["pickup_latitude"],
                                                                           rate_code = (int?)row["rate_code"],
                                                                           dropoff_longitude = (double)row["dropoff_longitude"],
                                                                           dropoff_latitude = (double)row["dropoff_latitude"],
                                                                           payment_type = (int)row["payment_type"],
                                                                           fare_amount = (decimal)row["fare_amount"],
                                                                           extra = (decimal)row["extra"],
                                                                           mta_tax = (decimal)row["mta_tax"],
                                                                           tip_amount = (decimal)row["tip_amount"],
                                                                           tolls_amount = (decimal)row["tip_amount"],
                                                                           imp_surcharge = (decimal)row["imp_surcharge"],
                                                                           total_amount = (decimal)row["total_amount"]
                                                                           }));
        }
    }
}