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

        public async Task<List<Place>> GetAllPlaces()
        {
            if (_allPlaces == null) _allPlaces = await Http.GetFromJsonAsync<List<Place>>("Places");
            return _allPlaces;
        }
        public async Task<List<Place>> GetFoundPlaces()
        {
            if (_foundPlaces == null) _foundPlaces = await GetAllPlaces();
            return _foundPlaces;
        }

        public async Task Clear()
        {
            _foundPlaces = null;
            _allPlaces = null;
        }
        public async Task Reset()
        {
            await Clear();
            await GetAllPlaces();
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

    }
}
