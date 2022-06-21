using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dieseltech.Models
{
    public class PickupDelivery
    {

        public int PickUpId { get; set; }
        public int DeliveryId { get; set; }
        public string LoadNumber { get; set; }
        public string Information { get; set; }
        public int ShipperId { get; set; }
        public string ShipperName { get; set; }
        public int CountryId { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public int ZipCode { get; set; }
        public System.DateTime DateTimeFrom { get; set; }
        public System.DateTime DateTimeTo { get; set; }
        public string PickupNumber { get; set; }
        public string Traitor { get; set; }
        public string Comments { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public int IsSave { get; set; }
        public string StateCode { get; set; }
        public string CityName { get; set; }
        public string CountryName { get; set; }
        public Nullable<int> MaxPickupId { get; set; }

        public Nullable<int> MaxDeliveryId { get; set; }

        public string LoadType { get; set; }
        public string Longitude { get; set; }
        public string Latitude { get; set; }
        public int Pickuporder { get; set; }
        public int Deliveryporder { get; set; }


        public string NewDateFrom { get; set; }
        public string NewDateTo { get; set; }



    }
}