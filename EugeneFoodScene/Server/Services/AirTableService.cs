using System;
using System.Collections.Generic;
using System.Collections.Immutable;
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
        private IEnumerable<Tag> _tags;
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
        public async Task<IEnumerable<Tag>> GetTagsAsync()
        {
            _tags ??= await GetTableAsync<Tag>("Tags");
            return _tags;
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
                if (place.Cuisines != null)
                {
                    foreach (var id in place.Cuisines)
                    {
                        place.CuisineList.Add(await GetCuisineAsync(id));
                    }

                    place.Cuisines = null; // remove from payload after hydration.
                }

                if (place.Categories != null)
                {
                    foreach (var id in place.Categories)
                    {
                        place.CategoryList.Add(await GetCategoryAsync(id));
                    }
                    place.Categories = null; // remove from payload after hydration.
                }

                if (place.OrderingServices != null)
                {
                    foreach (var id in place.OrderingServices)
                    {
                        place.OrderingServiceList.Add(await GetOrderingServiceAsync(id));
                    }
                    place.OrderingServices = null; // remove from payload after hydration.
                }

                if (place.Tags != null)
                {
                    foreach (var tag in place.Tags)
                    {
                        place.TagList.Add(await GetTagAsync(tag));
                    }
                    place.Tags = null; // remove from payload after hydration.
                }

            }
            _placesPop = places.ToArray();
            return _placesPop;

        }

        public void ResetAll()
        {
            ResetPlaces();
            ResetCuisines();
            ResetCategories();
            ResetTags();
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

        public void ResetCategories()
        {
            _categories = null;
        }

        public void ResetTags()
        {
            _tags = null;
        }

        public async Task<Cuisine> GetCuisineAsync(string id)
        {
            _cuisines ??= await GetCuisinesAsync();

            return _cuisines.FirstOrDefault(c => c.Id == id);
        }

        public async Task<Category> GetCategoryAsync(string id)
        {
            _categories ??= await GetCategoriesAsync();

            return _categories.FirstOrDefault(c => c.Id == id);
        }

        public async Task<Tag> GetTagAsync(string id)
        {
            _tags ??= await GetTagsAsync();

            return _tags.FirstOrDefault(c => c.Id == id);
        }

    }
}
