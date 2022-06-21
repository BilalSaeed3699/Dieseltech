using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dieseltech.Models
{
    public class PickupDeliveryInformation
    {
        public string ShipperName { get; set; }
        public string ShipperAddress { get; set; }
        public string CityName { get; set; }
        public string StateCode { get; set; }
        public string ZipCode { get; set; }
        public string ShipperPhone { get; set; }
        public string DateTime { get; set; }
        public string DateTimeTo { get; set; }

        public string PickupNumber { get; set; }
        public string Comment { get; set; }
        public int ordernumber { get; set; }
        public string Name { get; set; }
    }
}