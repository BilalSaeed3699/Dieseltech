using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dieseltech.Models
{
    public class DriverLicense
    {
        public int DriverId { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public bool IsActive { get; set; }
        public string Language { get; set; }
        public int TruckId { get; set; }
        public string CarrierAssignId { get; set; }
        public string LicenseFileName { get; set; }
        public string LicenseFilePath { get; set; }

    }
}