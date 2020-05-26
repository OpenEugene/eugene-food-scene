﻿using System;
using System.Collections.Generic;
using AirtableApiClient;
using Newtonsoft.Json;

namespace EugeneFoodScene.Data
{

    // Places, Categories, Cuisines, Delivery Services, Order Delivery Links

    /// <summary>
    /// Name, Category, Phone, URL, Address, Notes, Order Delivery Links, Menu,
    /// </summary>
    public class Place
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<String> Category { get; set; }

        public List<String> Cuisines { get; set; }

        public List<Cuisine> CuisineList { get; set; }
        public String CuisineDisplay { get; set; }

        [JsonProperty("Delivery Options")]
        public List<String> DeliveryOptions { get; set; }
        public List<DeliveryService> DeliveryServiceList { get; set; }
        public String DeliveryOptionsDisplay { get; set; }
        public string Phone { get; set; }
        public string URL { get; set; }
        public string Address { get; set; }
        public string Specials { get; set; }
        public string Notes { get; set; }
        public List<OrderDeliveryLink> OrderDeliveryLinks { get; set; }
        public List<AirtableAttachment> Menu { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

    }

    /// <summary>
    /// Category, Places, 
    /// </summary>
    public class Category
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<Place> Places { get; set; }
    }

    public class Cuisine
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<String> Places { get; set; }
        public List<Place> PlacesList { get; set; }
    }

    public class DeliveryService
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<string> Places { get; set; }
        public List<string> OrderDeliveryLinks { get; set; }

        public string ImageName { 
            get {
                return Name.Replace(" '","").ToLower().Trim();
            } 
        }

    }

    public class OrderDeliveryLink
    {
        public string Id { get; set; }
        public string URL { get; set; }
        public List<String> Places { get; set; }
        public List<String> DeliveryServices { get; set; }
    }
}
