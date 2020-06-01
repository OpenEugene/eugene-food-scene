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

namespace EugeneFoodScene.Client.Services
{
    public class ClientCache : BaseCache
    {
        private List<Place> _allPlaces = null; 
        private List<Place> _foundPlaces = null;
        private List<Category> _categories = null;
        private List<Cuisine> _cuisines = null;

        private IEnumerable<string> _selectedCuisines = null;
        private IEnumerable<string> _selectedMethods = null;
        private string _searchWords = null;

        public  ClientCache(HttpClient http) : base(http) {}

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
        public List<Cuisine> Cuisines
        {
            get => _cuisines;
            set => SetField(ref _cuisines, value);
        }

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
        public async Task Clear()
        {
            _foundPlaces = null;
            _allPlaces = null;
            _categories = null;
            _selectedCuisines = null;
            _selectedMethods = null;
            _searchWords = null;
        }
        public async Task Reset()
        {
            await Clear();
            await GetAllPlaces();
            await GetCategories();
            await GetCuisines();
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

        public async Task FilterCuisine(IEnumerable<string> selectedCuisines)
        {
            _selectedCuisines = selectedCuisines;
            await ApplyFilters();
        }

        public async Task FilterMethod(IEnumerable<string> selectedMethods)
        {
            _selectedMethods = selectedMethods;
            await ApplyFilters();
        }

        private async Task ApplyFilters()
        {
            await GetAllPlaces();

            var query = from p in AllPlaces select p;

            if (_selectedMethods != null)
            {
                query = from p in query
                    where p.Pickup=="Yes" & _selectedMethods.Contains("pickup")
                    select p;
            }

            if (_selectedCuisines != null)
            {
                query = from p in query
                    where p.Cuisines.Any(_selectedCuisines.Contains)
                    select p;
            }

            if (_searchWords != null)
            {
                query = query.Where(p => p.Name.Contains(_searchWords, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            FoundPlaces = query.ToList();

            OnCacheUpdated();
        }

    }
}
