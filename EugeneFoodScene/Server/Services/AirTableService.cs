using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using AirtableApiClient;
using Microsoft.AspNetCore.Mvc.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using EugeneFoodScene.Data;


namespace EugeneFoodScene.Services
{
    /// <summary>
    /// a bruit force airtable data caching service
    /// </summary>
    public class AirTableService : AirTableBase
    {
        private IEnumerable<Place> _placesPop;  // the higest level populated places list.
        private IEnumerable<Place> _places;
        private IEnumerable<Category> _categories;
        private IEnumerable<Cuisine> _cuisines;
        private IEnumerable<OrderingService> _orderingServices;

        public AirTableService(IConfiguration configuration) : base(configuration) {}

        public async Task<IEnumerable<Place>> GetPlacesAsync()
        {
            return _places ??= await GetTableAsync<Place>("Places");
        }

        public async Task<IEnumerable<Category>> GetCategoriesAsync()
        {
            return _categories ??= await GetTableAsync<Category>("Categories");
        }

        public async Task<IEnumerable<Cuisine>> GetCuisinesAsync()
        {
            return _cuisines ??= await GetTableAsync<Cuisine>("Cuisines");
        }

        public async Task<IEnumerable<OrderingService>> GetOrderingServicesAsync()
        {
            return _orderingServices ??= await GetTableAsync<OrderingService>("Ordering Services");
        }

        public async Task<OrderingService> GetOrderingServiceAsync(string id)
        {
            _orderingServices ??= await GetOrderingServicesAsync();
            return _orderingServices.FirstOrDefault(c => c.Id == id);
        }

     
        public async Task<IEnumerable<Place>> GetPlacesPopulatedAsync()
        {
            if (_placesPop != null)
            {
                return _placesPop;
            }
            var places = await GetPlacesAsync();
          
            // populate lookups
            foreach (var place in places)
            {
                place.CuisineList = new List<Cuisine>();
                if (place.Cuisines != null)
                {
                    foreach (var cuisine in place.Cuisines)
                    {
                        var fullCuisine = await GetCuisineAsync(cuisine);
                        place.CuisineList.Add(fullCuisine);
                    }
                }
                if (place.OrderingServices != null)
                {
                    place.OrderingServiceList = new List<OrderingService>();
                    foreach (var option in place.OrderingServices)
                    {
                        var fullOrderingService = await GetOrderingServiceAsync(option);
                        place.OrderingServiceList.Add(fullOrderingService);
                    }
                }

                // convert to list like: "one, two, three"
                place.CuisineDisplay = String.Join(",", place.CuisineList.Select(c => c.Name));
                if (place.OrderingServices != null)
                {
                    place.DeliveryOptionsDisplay = String.Join(",", place.OrderingServiceList.Select(c => c.Name));
                }
            }
            _placesPop = places.ToArray();
            return _placesPop;

        }

        public void ResetAll()
        {
            ResetPlaces();
            ResetCuisines();
        }

        public void ResetPlaces()
        {
            _placesPop = null;
            _places = null;
        }

        public void ResetCuisines()
        {
            _cuisines = null;
        }

        public async Task<Cuisine> GetCuisineAsync(string id)
        {
            _cuisines ??= await GetCuisinesAsync();

            return _cuisines.FirstOrDefault(c => c.Id == id);
        }

    }
}
