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
    
    public partial class Sp_Get_LoadHeadInformation_Edit_Result
    {
        public string BrokerId { get; set; }
        public string BrokerName { get; set; }
        public Nullable<int> CompanyId { get; set; }
        public Nullable<decimal> BrokerRate { get; set; }
        public string BrokerContactName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Extension { get; set; }
        public string BrokerReference { get; set; }
        public string RegistrationDate { get; set; }
        public Nullable<System.DateTime> originalRegistrationDate { get; set; }
        public string CarrierId { get; set; }
        public string CarrierName { get; set; }
        public string CarrierContactName { get; set; }
        public string Phonenumber { get; set; }
        public int TruckId { get; set; }
        public string DriverPhone { get; set; }
        public string DriverName { get; set; }
        public decimal CarrierRate { get; set; }
        public string NumberToText { get; set; }
        public Nullable<bool> IsSendText { get; set; }
        public int LoadTypeId { get; set; }
        public int LoadSubTypeId { get; set; }
        public string Commodity { get; set; }
        public Nullable<int> Available { get; set; }
        public Nullable<int> Weight { get; set; }
        public Nullable<int> QuantityTypeId { get; set; }
        public Nullable<int> Quantity { get; set; }
        public int DriverTypeId { get; set; }
        public string CarrierInstructions { get; set; }
        public Nullable<int> BrokerAmout { get; set; }
        public Nullable<int> CarrierAmount { get; set; }
        public string DispatcherName { get; set; }
        public string DispatcherPhone { get; set; }
        public Nullable<int> AgentGross { get; set; }
        public string LoadTypeName { get; set; }
        public string LoadSubTypeName { get; set; }
        public string Equipment { get; set; }
        public bool IsFutureLoad { get; set; }
        public bool IsManagerFutureLoad { get; set; }
        public string FutureLoadText { get; set; }
    }
}
