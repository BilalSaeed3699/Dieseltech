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
    using System.Collections.Generic;
    
    public partial class tblLoadCharge1
    {
        public int LoadChargeId { get; set; }
        public string LoaderNumber { get; set; }
        public int BrokerCharges { get; set; }
        public string ChargesFor { get; set; }
        public Nullable<bool> IsFlatToAgent { get; set; }
        public Nullable<decimal> AgentCharges { get; set; }
        public Nullable<bool> IsFlatToBranch { get; set; }
        public Nullable<decimal> BranchCharges { get; set; }
        public Nullable<bool> IsFlatToCompany { get; set; }
        public Nullable<decimal> CompanyCharges { get; set; }
        public Nullable<decimal> CarrierCharges { get; set; }
        public Nullable<bool> IsQuickPay { get; set; }
        public Nullable<bool> IsCommissionCharge { get; set; }
        public Nullable<bool> IsDeductionCharge { get; set; }
        public string Comments { get; set; }
        public Nullable<int> User_ID { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
    }
}
