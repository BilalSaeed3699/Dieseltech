//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Dieseltech.Models
{
    using System;
    
    public partial class Sp_Get_LoadPickupDelivery_Detail_FilterWise_Result
    {
        public string COD { get; set; }
        public string LoaderNumber { get; set; }
        public string RegistrationDate { get; set; }
        public string Agent { get; set; }
        public string Profile { get; set; }
        public string Brokernanme { get; set; }
        public string CarrierName { get; set; }
        public string LoadTypeName { get; set; }
        public string PickupInfo { get; set; }
        public string PickupDate { get; set; }
        public string PickupCityName { get; set; }
        public string DeliveryCityName { get; set; }
        public string DeliveryInfo { get; set; }
        public string DeliveryDate { get; set; }
        public Nullable<int> BrokerRate { get; set; }
        public int CarrierRate { get; set; }
        public Nullable<int> Profit { get; set; }
        public int IsCancel { get; set; }
        public bool IsFutureLoad { get; set; }
        public string FutureLoadText { get; set; }
    }
}
