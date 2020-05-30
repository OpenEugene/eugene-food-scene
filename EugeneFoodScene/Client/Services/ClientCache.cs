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
        }
        public async Task Reset()
        {
            await Clear();
            await GetAllPlaces();
            await GetCategories();
        }

        public async Task<Place> GetPlace(string Id)
        {
            await GetAllPlaces();
            var place = AllPlaces.SingleOrDefault(p => p.Id == Id); ;
            return place;
        }

        public async Task Search(string words) {
            await GetAllPlaces();
            FoundPlaces = AllPlaces.Where(p => p.Name.Contains(words,StringComparison.OrdinalIgnoreCase)).ToList();
            OnCacheUpdated();
        }

        public async Task FilterCuisine(IEnumerable<string> selectedCuisines)
        {
            await GetAllPlaces();

            var query = 
                from p in AllPlaces
                where p.Cuisines.Any(selectedCuisines.Contains)
                select p;

            FoundPlaces = query.ToList();

            OnCacheUpdated();
        }
    }
}
