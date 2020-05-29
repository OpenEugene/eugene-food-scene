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
    public class PlacesCache : INotifyPropertyChanged
    {
        private List<Place> _places = null;
        private HttpClient _http;

        public  PlacesCache(HttpClient http) {
            _http = http;
        }

        public List<Place> Places
        {
            get => _places;
            set => SetField(ref _places, value);
        }

        public async Task<List<Place>> GetPlaces()
        {
            if (_places == null) _places = await _http.GetFromJsonAsync<List<Place>>("Places");
            return _places;
        }

        public async Task<Place> GetPlace(string Id)
        {
            var place = Places.SingleOrDefault(p => p.Id == Id); ;
            return place;
        }

        public void Search(string words) {
            var filtered = Places.Where(p => p.Name.Contains(words)).ToList();
            Places = filtered;
            OnCacheUpdated();
        }

        public event EventHandler CacheUpdated;
        protected void OnCacheUpdated() => CacheUpdated?.Invoke(this, new EventArgs());

        #region INotify
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName) 
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
        #endregion

    }
}
