using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Dieseltech.Models
{
    public class ModelHelper
    {
        public List<SelectListItem> ToSelectItemList(dynamic values)
        {
            List<SelectListItem> tempList = null;
            if (values != null)
            {
                tempList = new List<SelectListItem>();
                foreach (var v in values)
                {
                    tempList.Add(new SelectListItem { Text = v.CarrierName, Value = Convert.ToString(v.CarrierID) });
                }
                tempList.TrimExcess();
            }
            return tempList;
        }

        public List<SelectListItem> ToSelectCarrierHelperItemList(dynamic values)
        {
            List<SelectListItem> tempList = null;
            if (values != null)
            {
                tempList = new List<SelectListItem>();
                foreach (var v in values)
                {
                    tempList.Add(new SelectListItem { Text = v.CarrierHelperName, Value = Convert.ToString(v.CarrierHelperId) });
                }
                tempList.TrimExcess();
            }
            return tempList;
        }

        public List<SelectListItem> ToSelectLoadTypeItemList(dynamic values)
        {
            List<SelectListItem> tempList = null;
            if (values != null)
            {
                tempList = new List<SelectListItem>();
                foreach (var v in values)
                {
                    tempList.Add(new SelectListItem { Text = v.LoadTypeName, Value = Convert.ToString(v.LoadTypeId) });
                }
                tempList.TrimExcess();
            }
            return tempList;
        }

        public List<SelectListItem> ToSelectTruckItemList(dynamic values)
        {
            List<SelectListItem> tempList = null;
            if (values != null)
            {
                tempList = new List<SelectListItem>();
                foreach (var v in values)
                {
                    tempList.Add(new SelectListItem { Text = v.TruckNo + ", Phone :" + v.DriverPhone, Value = Convert.ToString(v.TruckId) });
                }
                tempList.TrimExcess();
            }
            return tempList;
        }

        public List<SelectListItem> ToSelectLoadSubTypeItemList(dynamic values)
        {
            List<SelectListItem> tempList = null;
            if (values != null)
            {
                tempList = new List<SelectListItem>();
                foreach (var v in values)
                {
                    tempList.Add(new SelectListItem { Text = v.LoadSubTypeName, Value = Convert.ToString(v.LoadSubTypeId) });
                }
                tempList.TrimExcess();
            }
            return tempList;
        }


        public List<SelectListItem> ToSelectQuantityTypeItemList(dynamic values)
        {
            List<SelectListItem> tempList = null;
            if (values != null)
            {
                tempList = new List<SelectListItem>();
                foreach (var v in values)
                {
                    tempList.Add(new SelectListItem { Text = v.QuantityTypeName, Value = Convert.ToString(v.QuantityTypeId) });
                }
                tempList.TrimExcess();
            }
            return tempList;
        }

        public List<SelectListItem> ToSelectDriverTypeItemList(dynamic values)
        {
            List<SelectListItem> tempList = null;
            if (values != null)
            {
                tempList = new List<SelectListItem>();
                foreach (var v in values)
                {
                    tempList.Add(new SelectListItem { Text = v.DriverTypeName, Value = Convert.ToString(v.DriverTypeId) });
                }
                tempList.TrimExcess();
            }
            return tempList;
        }






        public List<SelectListItem> ToSelectZoneList(dynamic values)
        {
            List<SelectListItem> tempList = null;
            if (values != null)
            {
                tempList = new List<SelectListItem>();
                foreach (var v in values)
                {
                    tempList.Add(new SelectListItem { Text = v.ZoneName, Value = Convert.ToString(v.ZoneId) });
                }
                tempList.TrimExcess();
            }
            return tempList;
        }




        public List<SelectListItem> ToSelectCityItemList(dynamic values)
        {
            List<SelectListItem> tempList = null;
            if (values != null)
            {
                tempList = new List<SelectListItem>();
                foreach (var v in values)
                {
                    tempList.Add(new SelectListItem { Text = v.CityName, Value = Convert.ToString(v.CityId) });
                }
                tempList.TrimExcess();
            }
            return tempList;
        }

        public List<SelectListItem> ToSelectCountryItem(dynamic values)
        {
            List<SelectListItem> tempList = null;
            if (values != null)
            {
                tempList = new List<SelectListItem>();
                foreach (var v in values)
                {
                    tempList.Add(new SelectListItem { Text = v.CountryName, Value = Convert.ToString(v.CountryId) });
                }
                tempList.TrimExcess();
            }
            return tempList;
        }


        public List<SelectListItem> ToSelectZoneListItem(dynamic values)
        {
            List<SelectListItem> tempList = null;
            if (values != null)
            {
                tempList = new List<SelectListItem>();
                foreach (var v in values)
                {
                    tempList.Add(new SelectListItem { Text = v.ZoneName, Value = Convert.ToString(v.ZoneId) });
                }
                tempList.TrimExcess();
            }
            return tempList;
        }


        public List<SelectListItem> ToSelectStateItem(dynamic values)
        {
            List<SelectListItem> tempList = null;
            if (values != null)
            {
                tempList = new List<SelectListItem>();
                foreach (var v in values)
                {
                    tempList.Add(new SelectListItem { Text = v.StateName, Value = Convert.ToString(v.StateID) });
                }
                tempList.TrimExcess();
            }
            return tempList;
        }

        public List<SelectListItem> ToSelectShipperItem(dynamic values)
        {
            List<SelectListItem> tempList = null;
            if (values != null)
            {
                tempList = new List<SelectListItem>();
                foreach (var v in values)
                {
                    tempList.Add(new SelectListItem { Text = v.ShipperName, Value = Convert.ToString(v.ShipperId) });
                }
                tempList.TrimExcess();
            }
            return tempList;
        }




        public List<SelectListItem> ToSelectCityItem(dynamic values)
        {
            List<SelectListItem> tempList = null;
            if (values != null)
            {
                tempList = new List<SelectListItem>();
                foreach (var v in values)
                {
                    tempList.Add(new SelectListItem { Text = v.CityName, Value = Convert.ToString(v.CityId) });
                }
                tempList.TrimExcess();
            }
            return tempList;
        }

        public List<SelectListItem> ToSelectCarrierDriversItem(dynamic values)
        {
            List<SelectListItem> tempList = null;
            if (values != null)
            {
                tempList = new List<SelectListItem>();
                foreach (var v in values)
                {
                    tempList.Add(new SelectListItem { Text = v.Name, Value = Convert.ToString(v.DriverId) });
                }
                tempList.TrimExcess();
            }
            return tempList;
        }


        //public List<SelectListItem> ToSelectCityItem(dynamic values)
        //{
        //    List<SelectListItem> tempList = null;
        //    if (values != null)
        //    {
        //        tempList = new List<SelectListItem>();
        //        foreach (var v in values)
        //        {
        //            tempList.Add(new SelectListItem { Text = v.CityName, Value = Convert.ToString(v.CityId) });
        //        }
        //        tempList.TrimExcess();
        //    }
        //    return tempList;
        //}


        public List<SelectListItem> ToSelectDriverItem(dynamic values)
        {
            List<SelectListItem> tempList = null;
            if (values != null)
            {
                tempList = new List<SelectListItem>();
                foreach (var v in values)
                {
                    tempList.Add(new SelectListItem { Text = v.Name, Value = Convert.ToString(v.DriverId) });
                }
                tempList.TrimExcess();
            }
            return tempList;
        }



        public List<SelectListItem> ToSelectBrokerItem(dynamic values)
        {
            List<SelectListItem> tempList = null;
            if (values != null)
            {
                tempList = new List<SelectListItem>();
                foreach (var v in values)
                {
                    tempList.Add(new SelectListItem { Text = v.Name, Value = Convert.ToString(v.Brokerid) });
                }
                tempList.TrimExcess();
            }
            return tempList;
        }


        public List<SelectListItem> ToSelectBrokernHelperItem(dynamic values)
        {
            List<SelectListItem> tempList = null;
            if (values != null)
            {
                tempList = new List<SelectListItem>();
                foreach (var v in values)
                {
                    tempList.Add(new SelectListItem { Text = v.BrokerHelperName, Value = Convert.ToString(v.BrokerHelperId) });
                }
                tempList.TrimExcess();
            }
            return tempList;
        }

        public List<SelectListItem> ToSelectCompanyItem(dynamic values)
        {
            List<SelectListItem> tempList = null;
            if (values != null)
            {
                tempList = new List<SelectListItem>();
                foreach (var v in values)
                {
                    tempList.Add(new SelectListItem { Text = v.CompanyName, Value = Convert.ToString(v.CompanyId) });
                }
                tempList.TrimExcess();
            }
            return tempList;
        }

        public List<SelectListItem> ToSelectCarrierCategory(dynamic values)
        {
            List<SelectListItem> tempList = null;
            if (values != null)
            {
                tempList = new List<SelectListItem>();
                foreach (var v in values)
                {
                    tempList.Add(new SelectListItem { Text = v.CategoryName, Value = Convert.ToString(v.CarrierCategoryId) });
                }
                tempList.TrimExcess();
            }
            return tempList;
        }

        public List<SelectListItem> GetAccessList(dynamic values)
        {
            List<SelectListItem> tempList = null;
            if (values != null)
            {
                tempList = new List<SelectListItem>();
                foreach (var v in values)
                {
                    tempList.Add(new SelectListItem { Text = v.Access_Level_Type, Value = Convert.ToString(v.Access_ID) });
                }
                tempList.TrimExcess();
            }
            return tempList;
        }
        public List<SelectListItem> GetLocationList(dynamic values)
        {
            List<SelectListItem> tempList = null;
            if (values != null)
            {
                tempList = new List<SelectListItem>();
                foreach (var v in values)
                {
                    tempList.Add(new SelectListItem { Text = v.Name, Value = Convert.ToString(v.id) });
                }
                tempList.TrimExcess();
            }
            return tempList;
        }

    }
}