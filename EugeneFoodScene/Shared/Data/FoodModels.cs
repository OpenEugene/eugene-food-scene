using System;
using System.Collections.Generic;
using AirtableApiClient;
using Newtonsoft.Json;

namespace EugeneFoodScene.Data
{

    // Places, Categories, Cuisines, Delivery Services, Order Delivery Links

    /// <summary>
    /// interface for hoisting Airtable IDs into data objests
    /// </summary>
    public interface IAirtable
    {
        public string Id { get; set; }
    }


    public class DeliveryMethods
    {
        public const string Delivery = "delivery";
        public const string Pickup = "pickup";
    }

    /// <summary>
    /// Name, Category, Phone, URL, Address, Notes, Order Delivery Links, Menu,
    /// </summary>
    public class Place : IAirtable
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<string> Category { get; set; }

        public List<string> Cuisines { get; set; }

        public List<Cuisine> CuisineList { get; set; }
        public string CuisineDisplay { get; set; }

        public string Pickup { get; set; }

        [JsonProperty("Delivery Options")]
        public List<string> DeliveryOptions { get; set; }
        public List<DeliveryService> DeliveryServiceList { get; set; }
        public string DeliveryOptionsDisplay { get; set; }
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
    public class Category : IAirtable
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<string> Places { get; set; }
    }

    public class Cuisine : IAirtable
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<String> Places { get; set; }
        public List<Place> PlacesList { get; set; }
    }

    public class DeliveryService : IAirtable
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
