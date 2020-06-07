using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Threading.Tasks;
using EugeneFoodScene.Data;
using Radzen;

namespace EugeneFoodScene.Client.Services
{
    public class ClientCache : BaseCache
    {
        //places data
        private List<Place> _allPlaces;
        private List<Place> _foundPlaces;

        //lookups
        private List<Category> _categories;
        private List<Cuisine> _cuisines;
        private List<Tag> _tags;

        // filters
        private string[] _selectedCuisines;
        private string[] _selectedMethods;
        private string[] _selectedCategories;
        private string[] _selectedTags;
        private string _searchWords;

        private NotificationService _notificationService;
 

        public ClientCache(HttpClient http, NotificationService notificationService ) : base(http)
        {
            _notificationService = notificationService;
        }

        public List<Place> AllPlaces
        {
            get => _allPlaces;
            set => SetField(ref _allPlaces, value);
        }

        public List<Place> FoundPlaces
        {
            get => _foundPlaces;
            set => SetField(ref _foundPlaces, value);
        }

        public List<Category> Categories
        {
            get => _categories;
            set => SetField(ref _categories, value);
        }

        public List<Tag> Tags
        {
            get => _tags;
            set => SetField(ref _tags, value);
        }
        public List<Cuisine> Cuisines
        {
            get => _cuisines;
            set => SetField(ref _cuisines, value);
        }

        public string[] OrderMethods  =>  new [] { "Delivery","Pick-up", "Curbside", "Dine-in" };
        

        public async Task<List<Place>> GetAllPlaces()
        {
            return _allPlaces ??= await Http.GetFromJsonAsync<List<Place>>("Places");
        }
        public async Task<List<Place>> GetFoundPlaces()
        {
            return _foundPlaces ??= await GetAllPlaces();
        }

        public async Task<List<Category>> GetCategories()
        {
            return _categories ??= await Http.GetFromJsonAsync<List<Category>>("Categories");
        }

        public async Task<List<Cuisine>> GetCuisines()
        {
            return _cuisines ??= await Http.GetFromJsonAsync<List<Cuisine>>("Cuisines");
        }

        public async Task<List<Tag>> GetTags()
        {
            return _tags ??= await Http.GetFromJsonAsync<List<Tag>>("Tags");
        }
        public async Task Clear()
        {
            _foundPlaces = null;
            _allPlaces = null;
            _categories = null;
            _selectedCuisines = null;
            _selectedMethods = null;
            _searchWords = null;
            _tags = null;
        }
        public async Task Reset()
        {
            await Clear();
            await GetAllPlaces();
            await GetCategories();
            await GetCuisines();
            await GetTags();
        }

        public async Task<Place> GetPlace(string Id)
        {
            await GetAllPlaces();
            var place = AllPlaces.SingleOrDefault(p => p.Id == Id); ;
            return place;
        }

        public async Task Search(string words)
        {
            _searchWords = words;
            await ApplyFilters();
        }

        public async Task FilterCuisine(string[] selectedCuisines)
        {
            _selectedCuisines = selectedCuisines;
            await ApplyFilters();
        }

        public async Task FilterCategory(string[] selectedCategories)
        {
            _selectedCategories = selectedCategories;
            await ApplyFilters();
        }

        public async Task FilterTag(string[] selectedTags)
        {
            _selectedTags = selectedTags;
            await ApplyFilters();
        }

        public async Task FilterMethod(string[] selectedMethods)
        {
            _selectedMethods = selectedMethods;
            await ApplyFilters();
        }

        private async Task ApplyFilters()
        {
            await GetAllPlaces();

            var query = from p in AllPlaces select p;

            if (_selectedMethods?.Length>0)
            {
                query = from p in query
                    where p.OrderingServiceList.Any(
                        o => o.DeliveryMethods.Any(_selectedMethods.Contains))
                    select p;
            }

            if (_selectedCuisines?.Length > 0)
            {
                query = from p in query
                    where p.CuisineList.Any(c=>_selectedCuisines.Contains(c.Id))
                    select p;
            }

            if (_selectedCategories?.Length > 0)
            {
                query = from p in query
                    where p.CategoryList.Any(c => _selectedCategories.Contains(c.Id))
                    select p;
            }

            if (_selectedTags?.Length > 0)
            {
                query = from p in query
                    where p.TagList.Any(t => _selectedTags.Contains(t.Id))
                    select p;
            }

            if (_searchWords != null)
            {
                query = query.Where(p => p.Name.Contains(_searchWords, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            var list = query.ToList();  // deferred execution
            if (list.Count == 0)
            {
                var msg = new NotificationMessage()
                {
                    Severity = NotificationSeverity.Info, 
                    Summary = "No matches", 
                    Detail = "Nothing found with those filters, showing everything.",
                    Duration = 4000
                };
                _notificationService.Notify(msg);
                FoundPlaces = AllPlaces;
            }
            else
            {
                var msg = new NotificationMessage()
                {
                    Severity = NotificationSeverity.Info,
                    Summary = "Matches!",
                    Detail = $"found {list.Count} matching places.",
                    Duration = 2000
                };
                _notificationService.Notify(msg);
                FoundPlaces = list;
            }

            OnCacheUpdated();
        }

    }
}
