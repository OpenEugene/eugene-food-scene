using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Threading.Tasks;
using EugeneFoodScene.Data;

namespace EugeneFoodScene.Client.Services
{
    public class PlacesCache
    {
        private List<Place> _places;
        private HttpClient _http;

        public PlacesCache(HttpClient http) {
            _http = http; 
        }

        public async Task<List<Place>> GetPlaces()
        {
            if (_places == null) _places = await _http.GetFromJsonAsync<List<Place>>("Places");
            return _places;
        }

        public async Task<Place> GetPlace(string Id)
        {
            var places = await GetPlaces();
            var place = places.SingleOrDefault(p => p.Id == Id); ;
            return place;
        }

    }
}
