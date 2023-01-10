using Dieseltech.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.Services;
using static Dieseltech.FilterConfig;

namespace Dieseltech.Controllers
{
    //[FilterConfig.AuthorizeActionFilter]
    [HandleError]

    /// Summary description for CountryService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class LoadController : Controller
    {
        private DieseltechEntities deEntity = new DieseltechEntities();
        Utility ut = new Utility();
        string qry = "";
        DataTable dt = new DataTable();
        // GET: Load

        [Customexception]
        public ActionResult ShowLoadDocument()
        {


            return View("Index");
        }

        [HttpGet]
        [Customexception]
        public JsonResult Verifyduplication(string Loadernumber, string Reference, string Type)
        {
            int result = 0;
            try
            {
                var response = deEntity.Sp_Find_Duplicate_Reference(Loadernumber.Trim(), Reference.Trim(), Type.Trim()).FirstOrDefault();
                result = Convert.ToInt32(response);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //public ActionResult SaveLoadInformation([Bind(Include = "BrokerId")] tblLoadHead LoadHeadList)

        //Save Load Information 
        [HttpPost]
        [Customexception]
        public ActionResult SaveLoadInformation(List<tblLoadHead> LoadHeadList, List<tblLoadPickup> LoadPickupInfo, List<tblLoadCharge1> Charge)
        {
            string LoaderNumber = "";
            int TruckId = 0;
            try
            {
                string qry = " Exec SpGetLoadNumber ";
                double quickpercentage = 0;
                int isquickpay = 0;
                LoaderNumber = ut.ExecuteScalar(qry);
                using (DieseltechEntities entities = new DieseltechEntities())
                {
                    //Save Load General Tab Information
                    if (LoadHeadList == null)
                    {
                        LoadHeadList = new List<tblLoadHead>();
                    }
                    //Loop and insert records.
                    foreach (tblLoadHead LoadHead in LoadHeadList)
                    {
                         qry = "select QuickPaypercentage from tblCarrier where AssignID ='" + LoadHead.CarrierId + "'";
                        quickpercentage = Convert.ToDouble(ut.ExecuteScalar(qry));

                        qry = "select IsQuickPay from tblCarrier where AssignID ='" + LoadHead.CarrierId + "'";
                        isquickpay = Convert.ToInt32(ut.ExecuteScalar(qry));

                        LoadHead.UserId = 0;
                        LoadHead.User_ID = Convert.ToInt32(Session["User_id"]);
                        LoadHead.LoaderNumber = LoaderNumber;
                        if (LoadHead.IsFutureLoad == null)
                        {
                            LoadHead.IsFutureLoad = false;
                        }

                        if (LoadHead.FutureLoadText == null)
                        {
                            LoadHead.FutureLoadText = " ";
                        }

                        TruckId = LoadHead.TruckId;
                        LoadHead.QuickPaypercentage = Convert.ToDecimal(quickpercentage);
                        LoadHead.IsQuickPay = 1;
                        //LoadHead.Comment = "xx";
                        entities.tblLoadHeads.Add(LoadHead);
                    }

                    entities.SaveChanges();

                    qry = "Exec Sp_Calculate_Quick_Pay -2,'" + LoaderNumber + "'";
                    ut.InsertUpdate(qry);


                    //Save Load General Tab Information
                    if (Charge == null)
                    {
                        Charge = new List<tblLoadCharge1>();
                    }
                    //Loop and insert records.
                    foreach (tblLoadCharge1 LoadCharge in Charge)
                    {
                        LoadCharge.User_ID = Convert.ToInt32(Session["User_id"]);
                        LoadCharge.LoaderNumber = LoaderNumber;
                        entities.tblLoadCharges1.Add(LoadCharge);
                    }
                    int insertedRecords = entities.SaveChanges();

                    //Update Pickup and Delivery status on save 
                    qry = "Update tblLoadPickup Set IsSave = 1  Where LoadNumber ='" + LoaderNumber + "' ";
                    qry += "Update tblLoadDelivery Set IsSave = 1  Where LoadNumber ='" + LoaderNumber + "' ";
                    ut.InsertUpdate(qry);

                    //Save Upload document to original table and delete from previous table 
                    qry = "insert into tblLoadFilePath SELECT         LoaderNumber, ImagePath, FileName FROM  tblLoadFilePathTemp    Where LoaderNumber ='" + LoaderNumber + "'; ";
                    qry += "delete tblLoadFilePathTemp      Where LoaderNumber ='" + LoaderNumber + "' ";
                    ut.InsertUpdate(qry);


                    qry = "Exec Sp_Update_Truck_Location " + TruckId + ",'" + LoaderNumber + "'";
                    ut.InsertUpdate(qry);

                    return Json(LoaderNumber);
                }



            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
            }
            return Json(LoaderNumber);
        }
        [Customexception]
        public ActionResult getLoadDocument(string LoaderNumber)
        {
            return Json(deEntity.tblLoadFilePaths.Select(x => new
            {
                ImagePath = x.ImagePath,
                FileName = x.FileName,
                LoaderNumber = LoaderNumber,
                //}).ToList(), JsonRequestBehavior.AllowGet);

            }).Where(d => d.LoaderNumber == LoaderNumber).ToList(), JsonRequestBehavior.AllowGet);

            //ViewBag.LoadInformation = deEntity.tblLoadHeads.ToList().Where(d => d.LoaderNumber == LoaderNumber).ToList();

        }

        
        public JsonResult GetState(string ZipCode)

        {
            List<tblStateCityData> StateCityList = new List<tblStateCityData>();

            string query = " SELECT        ZipCode, CityName, StateCode FROM            tblStateCityData where Zipcode = '" + ZipCode + "'";
            dt = ut.GetDatatable(query);
            foreach (DataRow dr in dt.Rows)

            {

                StateCityList.Add(new tblStateCityData
                {
                    //, MC#,AssignID, ContactName, Phonenumber

                    ZipCode =dr["ZipCode"].ToString(),
                    StateCode = dr["StateCode"].ToString(),
                    CityName = dr["CityName"].ToString(),




                });

            }

            return Json(StateCityList, JsonRequestBehavior.AllowGet);
        }
       
        public JsonResult GetCarrierDetail(string AssignID)

        {
            //tblCarrier searchlist = new tblCarrier;
            //string qry = "select * from tblCarrier Where AssignID ='" + AssignID + "' ";

            string qry = "Exec Sp_Get_CarrierTruck_Information'" + AssignID + "' ";

            dt = ut.GetDatatable(qry);

            //tblCarrier CarrierData = new tblCarrier
            //{
            //    CarrierName = dt.Rows[0]["CarrierName"].ToString(),
            //    Phonenumber = dt.Rows[0]["Phonenumber"].ToString(),
            //    ContactName = dt.Rows[0]["ContactName"].ToString(),
            //    MC_ = dt.Rows[0]["MC#"].ToString(),
            //    AssignID= dt.Rows[0]["AssignID"].ToString(),

            //};

            //List<tblCarrier> CarrierData = new List<tblCarrier>();

            //List<tblCarrier> searchlist = new List<tblCarrier>();

            List<Sp_Get_CarrierTruck_Information_Result> CarrierTruckList = new List<Sp_Get_CarrierTruck_Information_Result>();

            foreach (DataRow dr in dt.Rows)

            {

                CarrierTruckList.Add(new Sp_Get_CarrierTruck_Information_Result
                {
                    //, MC#,AssignID, ContactName, Phonenumber

                    CarrierName = dr["CarrierName"].ToString(),
                    MC_ = dr["MC#"].ToString(),
                    AssignID = dr["AssignID"].ToString(),
                    ContactName = dr["ContactName"].ToString(),
                    Phonenumber = dr["Phonenumber"].ToString(),
                    ContactNametwo = dr["ContactNametwo"].ToString(),
                    Phonenumbertwo = dr["Phonenumbertwo"].ToString(),
                    Email = dr["Email"].ToString(),
                    TruckId = Convert.ToInt32(dr["TruckId"]),
                    CarrierAssignId = dr["CarrierAssignId"].ToString(),
                    TruckNo = dr["TruckNo"].ToString(),
                    TruckYard = dr["TruckYard"].ToString(),
                    TrailerNo = dr["TrailerNo"].ToString(),
                    TrailerTypeId = Convert.ToInt32(dr["TrailerTypeId"]),
                    ZipCode =dr["ZipCode"].ToString(),
                    AvailableDate = (dr["AvailableDate"].ToString()),
                    //AvailableDate = Convert.ToDateTime(dr["AvailableDate"]),
                    DriverName = dr["DriverName"].ToString(),
                    DriverPhone = dr["DriverPhone"].ToString(),
                    DriverLanguage = dr["DriverLanguage"].ToString(),
                    DriverId = Convert.ToInt32(dr["DriverId"].ToString()),
                    McAuthorityFileName = dr["McAuthorityFileName"].ToString(),
                    McAuthorityFilePath = dr["McAuthorityFilePath"].ToString(),
                    InsuranceFileName = dr["InsuranceFileName"].ToString(),
                    InsuranceFilePath = dr["InsuranceFilePath"].ToString(),

                    AssignmentFileName = dr["AssignmentFileName"].ToString(),
                    AssignmentFilePath = dr["AssignmentFilePath"].ToString(),


                    VoidChequeFileName = dr["VoidChequeFileName"].ToString(),
                    VoidChequeFilePath = dr["VoidChequeFilePath"].ToString(),


                    W9FileName = dr["W9FileName"].ToString(),
                    W9FilePath = dr["W9FilePath"].ToString(),
                    CarrierCategoryId = Convert.ToInt32(dr["CarrierCategoryId"].ToString()),
                    IsOwnerOperator = Convert.ToInt32(dr["IsOwnerOperator"].ToString()),
                    IsQuickPay = Convert.ToInt32(dr["IsQuickPay"].ToString()),
                    IsActive = Convert.ToBoolean(dr["IsActive"].ToString()),
                    IsBlackList = Convert.ToInt32(dr["IsBlackList"].ToString()),
                    Istrailerinterchange = Convert.ToInt32(dr["Istrailerinterchange"].ToString()),
                    InsuranceExpirationDate = Convert.ToDateTime(dr["InsuranceExpirationDate"]),
                    PrefferedDestination = dr["PrefferedDestination"].ToString(),
                    quickpaypercentage = Convert.ToDouble(dr["quickpaypercentage"].ToString()),
                    IsNeedToAssign = Convert.ToBoolean(dr["IsNeedToAssign"].ToString()),
                });

            }

            //foreach (DataRow dr in dt.Rows)

            //{

            //    CarrierData.Add(new tblCarrier
            //    {
            //        //, MC#,AssignID, ContactName, Phonenumber

            //        CarrierName = dr["CarrierName"].ToString(),
            //        MC_ = dr["MC#"].ToString(),
            //        AssignID = dr["AssignID"].ToString(),
            //        ContactName = dr["ContactName"].ToString(),
            //        Phonenumber = dr["Phonenumber"].ToString()


            //    });

            //}


            return Json(CarrierTruckList, JsonRequestBehavior.AllowGet);


        }
        [Customexception]
        public JsonResult GetRecord(string prefix)

        {

            DataSet ds = ut.GetName(prefix);

            //List<tblCarrier> searchlist = new List<tblCarrier>();

            List<Sp_Get_CarrierTruck_Information_Result> CarrierTruckList = new List<Sp_Get_CarrierTruck_Information_Result>();

            foreach (DataRow dr in ds.Tables[0].Rows)

            {

                CarrierTruckList.Add(new Sp_Get_CarrierTruck_Information_Result
                {
                    //, MC#,AssignID, ContactName, Phonenumber

                    CarrierName = dr["CarrierName"].ToString(),
                    MC_ = dr["MC#"].ToString(),
                    AssignID = dr["AssignID"].ToString(),
                    ContactName = dr["ContactName"].ToString(),
                    Phonenumber = dr["Phonenumber"].ToString(),
                    ContactNametwo = dr["ContactNametwo"].ToString(),
                    Phonenumbertwo = dr["Phonenumbertwo"].ToString(),
                    IsBlackList = Convert.ToInt32(dr["IsBlackList"].ToString()),
                    //CarrierCategoryId = Convert.ToInt32(dr["CarrierCategoryId"]),
                    //IsOwnerOperator = Convert.ToInt32(dr["IsOwnerOperator"]),
                    //TruckId = Convert.ToInt32(dr["TruckId"]),
                    //CarrierAssignId = dr["CarrierAssignId"].ToString(),
                    //TruckNo = dr["TruckNo"].ToString(),
                    //TruckYard = dr["TruckYard"].ToString(),
                    //TrailerNo = dr["TrailerNo"].ToString(),
                    //TrailerTypeId = Convert.ToInt32(dr["TrailerTypeId"]),
                    //ZipCode = Convert.ToInt32(dr["ZipCode"].ToString()),
                    //AvailableDate = Convert.ToDateTime(dr["AvailableDate"]),
                    //DriverName = dr["DriverName"].ToString(),
                    //DriverPhone = dr["DriverPhone"].ToString(),
                    //DriverLanguage = dr["DriverLanguage"].ToString(),
                    //DriverId = Convert.ToInt32(dr["DriverId"].ToString()),

                });

            }

            return Json(CarrierTruckList, JsonRequestBehavior.AllowGet);

        }


      
        public JsonResult GetStateCityName(string prefix)

        {

            DataSet ds = ut.GetCityStateNameDetail(prefix);

            //List<tblCarrier> searchlist = new List<tblCarrier>();

            List<tblStateCityData> StateCityList = new List<tblStateCityData>();

            foreach (DataRow dr in ds.Tables[0].Rows)

            {

                StateCityList.Add(new tblStateCityData
                {
                    //, MC#,AssignID, ContactName, Phonenumber

                    ZipCode = dr["ZipCode"].ToString(),
                    StateCode = dr["StateCode"].ToString(),
                    CityName = dr["CityName"].ToString(),




                });

            }

            return Json(StateCityList, JsonRequestBehavior.AllowGet);

        }




        
        public JsonResult GetShipperRecord(string prefix)

        {

            //DataSet ds = ut.GetName(prefix);
            DataTable dt = new DataTable();
            string qry = "SELECT         ShipperId, ShipperName, ShipperPhone, ShipperAddress, ShipperCity, ShipperStateCode, ShipperStateName, ShipperZipCode, ShipperAssignId FROM tblShipper  where ShipperName like '%" + prefix + "%'  OR ShipperAddress like '%" + prefix + "%' ";
            //qry += " or MC# like '%'+@prefix+'%' OR  AssignID like '%'+@prefix+'%' OR  ContactName  like '%'+@prefix+'%' OR Phonenumber  '%'+@prefix+'%'   ";

            dt = ut.GetDatatable(qry);
            //List<tblCarrier> searchlist = new List<tblCarrier>();

            List<tblShipper> ShipperList = new List<tblShipper>();

            foreach (DataRow dr in dt.Rows)

            {

                ShipperList.Add(new tblShipper
                {
                    //, MC#,AssignID, ContactName, Phonenumber
                    ShipperId = Convert.ToInt32(dr["ShipperId"]),
                    ShipperName = (dr["ShipperName"]).ToString(),
                    ShipperPhone = (dr["ShipperPhone"]).ToString(),
                    ShipperAddress = (dr["ShipperAddress"]).ToString(),
                    ShipperCity = (dr["ShipperCity"]).ToString(),
                    ShipperStateCode = (dr["ShipperStateCode"]).ToString(),
                    ShipperStateName = (dr["ShipperStateName"]).ToString(),
                    //ShipperZipCode = Convert.ToInt32(dr["ShipperZipCode"]),
                    ShipperZipCode = dr["ShipperZipCode"].ToString(),
                    ShipperAssignId = (dr["ShipperAssignId"]).ToString(),



                });

            }

            return Json(ShipperList, JsonRequestBehavior.AllowGet);

        }

     
        public JsonResult GetShipperDetail(string AssignID)

        {
            string qry = " SELECT ShipperId, ShipperName, ShipperPhone, ShipperAddress, ShipperCity, ShipperStateCode, ShipperStateName, ShipperZipCode, ";
            qry += " ShipperAssignId, ISNULL(Longitude,'0') as Longitude, ISNULL( Latitude,'0') as  Latitude From tblShipper  where ShipperId = " + AssignID + "";
            dt = ut.GetDatatable(qry);
            List<tblShipper> ShipperList = new List<tblShipper>();
            foreach (DataRow dr in dt.Rows)

            {

                ShipperList.Add(new tblShipper
                {
                    //, MC#,AssignID, ContactName, Phonenumber
                    //, MC#,AssignID, ContactName, Phonenumber
                    ShipperId = Convert.ToInt32(dr["ShipperId"]),
                    ShipperName = (dr["ShipperName"]).ToString(),
                    ShipperPhone = (dr["ShipperPhone"]).ToString(),
                    ShipperAddress = (dr["ShipperAddress"]).ToString(),
                    ShipperCity = (dr["ShipperCity"]).ToString(),
                    ShipperStateCode = (dr["ShipperStateCode"]).ToString(),
                    ShipperStateName = (dr["ShipperStateName"]).ToString(),
                    ShipperZipCode = dr["ShipperZipCode"].ToString(),
                    ShipperAssignId = (dr["ShipperAssignId"]).ToString(),
                    Longitude = (dr["Longitude"]).ToString(),
                    Latitude = (dr["Latitude"]).ToString(),
                });
            }



            return Json(ShipperList, JsonRequestBehavior.AllowGet);


        }




        [Customexception]
        public ActionResult LoadConfirmation(string LoaderNumber, int? type = 0)
        {
            try
            {
                ViewBag.type = type;

                Session["CarrierInsuranceExpirationList"] = deEntity.tblNotifications.OrderBy(n => n.IsRead).ToList();
                if (LoaderNumber == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                ViewBag.LoadConfirmation = deEntity.SP_Get_Load_Confirmation_Data(LoaderNumber).ToList();
                ViewBag.LoadConfirmationDocuments = deEntity.tblLoadConfirmationDocuments.ToList().Where(d => d.LoadNumber == LoaderNumber).ToList();

                ViewBag.LoadDocuments = deEntity.tblLoadFilePaths.ToList().Where(d => d.LoaderNumber == LoaderNumber).ToList();
                //ViewBag.LoadInformation = deEntity.tblLoadHeads.ToList().Where(d => d.LoaderNumber == LoaderNumber).ToList();

                //ViewBag.LoadPickupDelivery = deEntity.tblLoadPickupDeliveries.ToList().Where(d => d.LoaderNumber == LoaderNumber).ToList();

                //ViewBag.LoadCharges = deEntity.tblLoadCharges1.ToList().Where(d => d.LoaderNumber == LoaderNumber).ToList();

                //ViewBag.LoadDocuments = deEntity.tblLoadFilePaths.ToList().Where(d => d.LoaderNumber == LoaderNumber).ToList();

                ////Fill Broker Dropdown
                //ViewBag.broker = new ModelHelper().ToSelectBrokerItem(deEntity.tblBrokers).ToList();
                //ViewBag.BrokerHelper = new ModelHelper().ToSelectBrokernHelperItem(deEntity.tblBrokerHelpers).ToList();
                //ViewBag.Company = new ModelHelper().ToSelectCompanyItem(deEntity.tblCompanies).ToList();
                //ViewBag.Carrier = new ModelHelper().ToSelectItemList(deEntity.tblCarriers).ToList();
                //ViewBag.CarrierHelper = new ModelHelper().ToSelectCarrierHelperItemList(deEntity.tblCarrierHelpers).ToList();
                //ViewBag.LoadType = new ModelHelper().ToSelectLoadTypeItemList(deEntity.tblLoadTypes).ToList();
                //ViewBag.Truck = new ModelHelper().ToSelectTruckItemList(deEntity.tblTrucks).ToList();
                //ViewBag.LoadSubType = new ModelHelper().ToSelectLoadSubTypeItemList(deEntity.tblLoadSubTypes).ToList();
                //ViewBag.QuantityType = new ModelHelper().ToSelectQuantityTypeItemList(deEntity.tblQuantityTypes).ToList();
                //ViewBag.DriverTypes = new ModelHelper().ToSelectDriverTypeItemList(deEntity.tblDriverTypes).ToList();

                //ViewBag.States = new ModelHelper().ToSelectStateItem(deEntity.tblStates).ToList();
                return View();

                //return View(tblLoadHead);
            }
            catch (Exception ex)

            {
                ViewBag.Error = "Exception Occur While Showing Load Confirmation" + ex.Message;
            }

            return View();


        }

        [Customexception]
        public ActionResult Index(string LoaderNumber)

        {

            //return View("sss");
            try
            {


                Session["CarrierInsuranceExpirationList"] = deEntity.tblNotifications.Where(n => n.NotificationType == "C").OrderBy(n => n.IsRead).ThenByDescending(n => n.CreateDate).ToList();
                Session["AlkaiosnotifyList"] = deEntity.tblNotifications.Where(n => n.NotificationType == "P" && n.CompanyId == 1).OrderBy(n => n.IsRead).ThenByDescending(n => n.CreateDate).ToList();
                Session["JetlinenotifyList"] = deEntity.tblNotifications.Where(n => n.NotificationType == "P" && n.CompanyId == 3).OrderBy(n => n.IsRead).ThenByDescending(n => n.CreateDate).ToList();

                string Loader = Convert.ToString(TempData["CurrentLoad"]);
                bool IsManagerFutureload = true;
                //On load delete unsave Load Documents from temp table 
                qry = "delete tblLoadFilePathTemp;";
                qry += " delete tblLoadPickup Where LoadNumber not in (select LoaderNumber from tblLoadHead);";
                qry += " delete tblLoadDelivery Where LoadNumber not in (select LoaderNumber from tblLoadHead)";
                ut.InsertUpdate(qry);

                string qry1 = " Exec SpGetLoadNumber ";
                ViewBag.LoadNumber = ut.ExecuteScalar(qry1);

                ViewBag.LoadDocuments = deEntity.tblLoadFilePaths.ToList().Where(d => d.LoaderNumber == LoaderNumber).ToList();
                ViewBag.FutureLoad = deEntity.tblLoadHeads.Where(lh => lh.IsManagerFutureLoad == IsManagerFutureload).ToList();
                //ViewBag.StateCityList = deEntity.tblStateCityDatas.ToList();
                if (LoaderNumber != null && LoaderNumber != "")
                {


                    ViewBag.LoadCharges = deEntity.tblLoadCharges1.ToList();

                    //ViewBag.LoadCharges = deEntity.tblLoadCharges.ToList().Where(d => d.Ch == chargeId).ToList();

                    ViewBag.LoadPickupDelivery = deEntity.tblLoadPickupDeliveries.ToList().Where(d => d.LoaderNumber == LoaderNumber).ToList();



                    //Fill Broker Dropdown
                    ViewBag.broker = new ModelHelper().ToSelectBrokerItem(deEntity.tblBrokers).ToList();
                    ViewBag.BrokerHelper = new ModelHelper().ToSelectBrokernHelperItem(deEntity.tblBrokerHelpers).ToList();
                    ViewBag.Company = new ModelHelper().ToSelectCompanyItem(deEntity.tblCompanies).ToList();
                    ViewBag.Carrier = new ModelHelper().ToSelectItemList(deEntity.tblCarriers).ToList();
                    ViewBag.CarrierHelper = new ModelHelper().ToSelectCarrierHelperItemList(deEntity.tblCarrierHelpers).ToList();
                    ViewBag.LoadType = new ModelHelper().ToSelectLoadTypeItemList(deEntity.tblLoadTypes).ToList();
                    //ViewBag.Truck = new ModelHelper().ToSelectTruckItemList(deEntity.tblTrucks).ToList();
                    ViewBag.LoadSubType = new ModelHelper().ToSelectLoadSubTypeItemList(deEntity.tblLoadSubTypes).ToList();
                    ViewBag.QuantityType = new ModelHelper().ToSelectQuantityTypeItemList(deEntity.tblQuantityTypes).ToList();
                    ViewBag.DriverTypes = new ModelHelper().ToSelectDriverTypeItemList(deEntity.tblDriverTypes).ToList();
                    ViewBag.DriverList = new ModelHelper().ToSelectDriverItem(deEntity.tblDrivers).ToList();
                    ViewBag.Shipper = new ModelHelper().ToSelectShipperItem(deEntity.tblShippers).ToList();



                    return View();
                }
                else
                {
                    LoaderNumber = "";

                    //ViewBag.LoadCharges = deEntity.tblLoadCharges1.ToList();

                    ViewBag.LoadCharges = deEntity.tblLoadCharges1.ToList().Where(d => d.LoadChargeId == 0).ToList();

                    ViewBag.LoadPickupDelivery = deEntity.tblLoadPickupDeliveries.ToList().Where(d => d.LoaderNumber == LoaderNumber).ToList();
                    //Fill Broker Dropdown
                    ViewBag.broker = new ModelHelper().ToSelectBrokerItem(deEntity.tblBrokers).ToList();
                    //ViewBag.BrokerHelper = new ModelHelper().ToSelectBrokernHelperItem(deEntity.tblBrokerHelpers).ToList();
                    ViewBag.Company = new ModelHelper().ToSelectCompanyItem(deEntity.tblCompanies).ToList();
                    ViewBag.Carrier = new ModelHelper().ToSelectItemList(deEntity.tblCarriers).ToList();
                    //ViewBag.CarrierHelper = new ModelHelper().ToSelectCarrierHelperItemList(deEntity.tblCarrierHelpers).ToList();
                    ViewBag.LoadType = new ModelHelper().ToSelectLoadTypeItemList(deEntity.tblLoadTypes).ToList();
                    ViewBag.Truck = new ModelHelper().ToSelectTruckItemList(deEntity.tblTrucks).ToList();
                    ViewBag.LoadSubType = new ModelHelper().ToSelectLoadSubTypeItemList(deEntity.tblLoadSubTypes).ToList();
                    ViewBag.QuantityType = new ModelHelper().ToSelectQuantityTypeItemList(deEntity.tblQuantityTypes).ToList();
                    ViewBag.DriverTypes = new ModelHelper().ToSelectDriverTypeItemList(deEntity.tblDriverTypes).ToList();
                    ViewBag.States = new ModelHelper().ToSelectStateItem(deEntity.tblStates).ToList();
                    //ViewBag.Shipper = new ModelHelper().ToSelectShipperItem(deEntity.tblShippers).ToList();
                    return View();
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Exception Occur While Showing Load information" + ex.Message;
            }

            return View();

        }

        [WebMethod]
        [Customexception]
        public List<string> GetShipperNames(string term)
        {
            List<string> listCountryName = new List<string>();
            //qry = "Exec spGetShipperName '"+ term + "' ";
            //string result = ut.ExecuteScalar(qry);
            //listCountryName.Add(result);
            string CS = ConfigurationManager.ConnectionStrings["DieselTechcs"].ConnectionString;
            using (SqlConnection con = new SqlConnection(CS))
            {
                SqlCommand cmd = new SqlCommand("spGetShipperName", con);
                cmd.CommandType = CommandType.StoredProcedure;

                SqlParameter parameter = new SqlParameter()
                {
                    ParameterName = "@term",
                    Value = term
                };
                cmd.Parameters.Add(parameter);
                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    listCountryName.Add(rdr["ShipperName"].ToString());
                }
                return listCountryName;
            }
        }

        // GET: tblLoadHeads/Edit/5
        [Customexception]
        public ActionResult EditLoad(string LoaderNumber)
        {
            if (LoaderNumber == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ViewBag.LoadInformation = deEntity.tblLoadHeads.ToList().Where(d => d.LoaderNumber == LoaderNumber).ToList();

            ViewBag.LoadPickupDelivery = deEntity.tblLoadPickupDeliveries.ToList().Where(d => d.LoaderNumber == LoaderNumber).ToList();

            ViewBag.LoadCharges = deEntity.tblLoadCharges1.ToList().Where(d => d.LoaderNumber == LoaderNumber).ToList();

            ViewBag.LoadDocuments = deEntity.tblLoadFilePaths.ToList().Where(d => d.LoaderNumber == LoaderNumber).ToList();

            //Fill Broker Dropdown
            ViewBag.broker = new ModelHelper().ToSelectBrokerItem(deEntity.tblBrokers).ToList();
            ViewBag.BrokerHelper = new ModelHelper().ToSelectBrokernHelperItem(deEntity.tblBrokerHelpers).ToList();
            ViewBag.Company = new ModelHelper().ToSelectCompanyItem(deEntity.tblCompanies).ToList();
            ViewBag.Carrier = new ModelHelper().ToSelectItemList(deEntity.tblCarriers).ToList();
            ViewBag.CarrierHelper = new ModelHelper().ToSelectCarrierHelperItemList(deEntity.tblCarrierHelpers).ToList();
            ViewBag.LoadType = new ModelHelper().ToSelectLoadTypeItemList(deEntity.tblLoadTypes).ToList();
            ViewBag.Truck = new ModelHelper().ToSelectTruckItemList(deEntity.tblTrucks).ToList();
            ViewBag.LoadSubType = new ModelHelper().ToSelectLoadSubTypeItemList(deEntity.tblLoadSubTypes).ToList();
            ViewBag.QuantityType = new ModelHelper().ToSelectQuantityTypeItemList(deEntity.tblQuantityTypes).ToList();
            ViewBag.DriverTypes = new ModelHelper().ToSelectDriverTypeItemList(deEntity.tblDriverTypes).ToList();

            ViewBag.States = new ModelHelper().ToSelectStateItem(deEntity.tblStates).ToList();
            return View();

            //return View(tblLoadHead);
        }




        [HttpPost]
        [ValidateAntiForgeryToken]
        [Customexception]
        public ActionResult Create([Bind(Include = "LoaderHeadId,LoaderNumber,CarrierId,CarrierHelperId,NumberToText,IsSendText,TruckId,CarrierRate," +
            "LoadTypeId,Commodity,LoadSubTypeId,Available,Weight,QuantityTypeId,Quantity,DriverTypeId,CarrierInstructions,CompanyId,BrokerId,BrokerHelperId," +
            "BrokerRate,ContactName,ContactPhone,Extension,ContactEmail,BrokerReference,RegistrationDate,User_ID,AgentGross,AgentFlat,BranchFlat")] tblLoadHead tblLoadHead)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string qry = " Exec SpGetLoadNumber ";
                    string LoaderNumber = ut.ExecuteScalar(qry);
                    tblLoadHead.LoaderNumber = LoaderNumber;
                    deEntity.tblLoadHeads.Add(tblLoadHead);
                    deEntity.SaveChanges();
                    ViewBag.Message = "Save Successfully";
                    TempData["CurrentLoad"] = LoaderNumber;
                    string test = TempData["CurrentLoad"].ToString();
                    return RedirectToAction("Index");

                }

            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
            }


            return View(tblLoadHead);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        [Customexception]
        public ActionResult UpdateLoad([Bind(Include = "LoaderHeadId,LoaderNumber,CarrierId,CarrierHelperId,NumberToText,IsSendText,TruckId,CarrierRate,LoadTypeId,Commodity,LoadSubTypeId,Available,Weight,QuantityTypeId,Quantity,DriverTypeId,CarrierInstructions,CompanyId,BrokerId,BrokerHelperId,BrokerRate,ContactName,ContactPhone,Extension,ContactEmail,BrokerReference,RegistrationDate,User_ID,AgentGross,AgentFlat,BranchFlat")] tblLoadHead tblLoadHead)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    qry = "Delete tblloadHead Where  LoaderNumber = '" + tblLoadHead.LoaderNumber + "' ";
                    string result = ut.InsertUpdate(qry);
                    //string qry = " Exec SpGetLoadNumber ";
                    //string LoaderNumber = ut.ExecuteScalar(qry);
                    //tblLoadHead.LoaderNumber = LoaderNumber;
                    deEntity.tblLoadHeads.Add(tblLoadHead);
                    deEntity.SaveChanges();
                    ViewBag.Message = "Save Successfully";
                    //TempData["CurrentLoad"] = LoaderNumber;
                    return RedirectToAction("Index");

                }

            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
            }


            return View(tblLoadHead);
        }


        //Function to move pickup to delivery 

        [HttpPost]
        [Customexception]
        public JsonResult MoveLoadPickupToDelivery(Int32 txtPickupId, int[] PickupsId, int[] DeliveriesId)
        {

            string qry = " Exec SpGetLoadNumber ";
            string LoaderNumber = ut.ExecuteScalar(qry);
            using (DieseltechEntities entities = new DieseltechEntities())
            {


                qry = " Exec  Sp_Insert_Update_LoadPickup  '" + LoaderNumber + "' , '',0, ";
                qry += " '' ,1,'','' ";
                qry += " ,'' ,0,'', '','" + DateTime.Now.ToString("yyyy-MM-dd") + "' ";
                qry += ", '" + DateTime.Now.ToString("yyyy-MM-dd") + "' , '', '','' ";
                qry += " ," + Convert.ToInt32(Session["User_id"]) + ",0," + txtPickupId + ",'','',0,'C'   ";
                ut.InsertUpdate(qry);



                int preference = 1;
                foreach (int id in PickupsId)
                {
                    if (id != 0)
                    {
                        var loadpikcup = deEntity.tblLoadPickups.Find(id);
                        if (loadpikcup != null)
                        {
                            loadpikcup.Pickuporder = preference;
                            deEntity.SaveChanges();
                            preference += 1;
                        }

                    }

                }



                preference = 1;
                foreach (int id in DeliveriesId)
                {
                    if (id != 0)
                    {
                        var loaddelivery = deEntity.tblLoadDeliveries.Find(id);
                        if (loaddelivery != null)
                        {
                            loaddelivery.Deliveryorder = preference;
                            deEntity.SaveChanges();
                            preference += 1;
                        }

                    }

                }




                qry = "select max(PickUpId) from tblLoadPickup Where  LoadNumber ='" + LoaderNumber + "' ";
                string PickupId = ut.ExecuteScalar(qry);
                //ViewBag.LastPickupId = PickupId;

                if (PickupId == "")
                {
                    PickupId = "0";
                }
                //qry = " select 'United States' as CountryName,  SD.ZipCode,SD.CityName,SD.StateCode, TLP.PickUpId, LoadNumber, S.ShipperId, ";
                //qry += " ISNULL(S.Longitude,'0') as Longitude, ISNULL( S.Latitude,'0') as  Latitude,S.ShipperName, CountryId,S.ShipperPhone PhoneNumber, Address, DateTimeFrom,";
                //qry += " DateTimeTo, PickupNumber, Traitor, Comments, CreatedBy, CreatedDate, IsSave";
                //qry += ",S.ShipperName + ' , ' + Address + ' , ' + SD.CityName +' , ' + SD.StateCode + ' , ' + CONVERT(varchar(10),DateTimeFrom,103) AS Information";
                //qry += " from tblLoadPickup TLP inner join tblStateCityData SD ON SD.ZipCode = TLP.ZipCode ";
                //qry += " inner join tblShipper S on S.ShipperId = TLP.ShipperId ";
                //qry += " Where TLP.LoadNumber ='" + LoaderNumber + "' ";
                //dt = ut.GetDatatable(qry);


                ////List<tblLoadPickup> LoadPickup = new List<tblLoadPickup>();
                //List<PickupDelivery> LoadPickupDelivery = new List<PickupDelivery>();


                //foreach (DataRow dr in dt.Rows)

                //{

                //    LoadPickupDelivery.Add(new PickupDelivery
                //    {
                //        PickUpId = Convert.ToInt32(dr["PickUpId"]),
                //        Information = (dr["Information"]).ToString(),
                //        ShipperName = (dr["ShipperName"]).ToString(),
                //        Address = (dr["Address"]).ToString(),
                //        PhoneNumber = (dr["PhoneNumber"]).ToString(),
                //        ZipCode = Convert.ToInt32(dr["ZipCode"]),
                //        StateCode = (dr["StateCode"]).ToString(),
                //        CityName = (dr["CityName"]).ToString(),
                //        CountryName = (dr["CountryName"]).ToString(),
                //        DateTimeFrom = Convert.ToDateTime(dr["DateTimeFrom"]),
                //        DateTimeTo = Convert.ToDateTime(dr["DateTimeTo"]),
                //        Traitor = (dr["Traitor"]).ToString(),
                //        PickupNumber = (dr["PickupNumber"]).ToString(),
                //        Comments = (dr["Comments"]).ToString(),
                //        MaxPickupId = Convert.ToInt32(PickupId),
                //        IsSave = 0,
                //        MaxDeliveryId = 0,
                //        LoadType = "P",
                //        Longitude = (dr["Longitude"]).ToString(),
                //        Latitude = (dr["Latitude"]).ToString(),
                //    });

                //}


                List<PickupDelivery> LoadPickupDelivery = new List<PickupDelivery>();

                var query = (from TLP in deEntity.tblLoadPickups
                             join S in deEntity.tblShippers on TLP.ShipperId equals S.ShipperId
                             join SD in deEntity.tblStateCityDatas on TLP.ZipCode equals SD.ZipCode
                             where TLP.LoadNumber == LoaderNumber
                             select new
                             {
                                 CountryName = "United States",
                                 SD.ZipCode,
                                 SD.CityName,
                                 SD.StateCode,
                                 TLP.PickUpId,
                                 TLP.LoadNumber,
                                 S.ShipperId,
                                 S.Longitude,
                                 S.Latitude,
                                 S.ShipperName,
                                 PhoneNumber = S.ShipperPhone,
                                 S.ShipperAddress,
                                 TLP.DateTimeFrom,
                                 TLP.DateTimeTo,
                                 TLP.PickupNumber,
                                 TLP.Traitor,
                                 TLP.Comments,
                                 TLP.CreatedBy,
                                 TLP.CreatedDate,
                                 TLP.IsSave,
                                 TLP.Pickuporder,

                                 Information = S.ShipperName + " , " + S.ShipperAddress + " , " + SD.CityName + " , " + SD.StateCode
                                 //+ " ," + Convert.ToString(TLP.DateTimeFrom)  
                                 //}).OrderBy(p=> p.PickUpId).ToList();
                             }).OrderBy(p => p.Pickuporder).ToList();


                //foreach (DataRow dr in dt.Rows)
                foreach (var dt in query)

                {

                    LoadPickupDelivery.Add(new PickupDelivery
                    {
                        PickUpId = dt.PickUpId,
                        Information = dt.Information,
                        ShipperName = dt.ShipperName,
                        Address = dt.ShipperAddress,
                        PhoneNumber = dt.PhoneNumber,
                        ZipCode = dt.ZipCode,
                        StateCode = dt.StateCode,
                        CityName = dt.CityName,
                        CountryName = dt.CountryName,
                        DateTimeFrom = Convert.ToDateTime(dt.DateTimeFrom),
                        DateTimeTo = Convert.ToDateTime(dt.DateTimeTo),
                        Traitor = dt.Traitor,
                        PickupNumber = dt.PickupNumber,
                        Comments = dt.Comments,
                        MaxPickupId = dt.PickUpId,
                        IsSave = 0,
                        ShipperId = dt.ShipperId,
                        MaxDeliveryId = 0,
                        LoadType = "P",
                        Longitude = dt.Longitude,
                        Latitude = dt.Longitude,
                        Pickuporder = dt.Pickuporder,
                        Deliveryporder = 0,
                        NewDateFrom = dt.DateTimeFrom.ToString("MM-dd-yyyy HH:mm"),
                        NewDateTo = dt.DateTimeTo.ToString("MM-dd-yyyy HH:mm"),
                    });

                }






                var DeliveryQuery = (from TLP in deEntity.tblLoadDeliveries
                                     join S in deEntity.tblShippers on TLP.ShipperId equals S.ShipperId
                                     join SD in deEntity.tblStateCityDatas on TLP.ZipCode equals SD.ZipCode
                                     where TLP.LoadNumber == LoaderNumber
                                     select new
                                     {
                                         PickupId,
                                         TLP.DeliveryId,
                                         CountryName = "United States",
                                         SD.ZipCode,
                                         SD.CityName,
                                         SD.StateCode,
                                         TLP.PickUpId,
                                         TLP.LoadNumber,
                                         S.ShipperId,
                                         S.Longitude,
                                         S.Latitude,
                                         S.ShipperName,
                                         PhoneNumber = S.ShipperPhone,
                                         S.ShipperAddress,
                                         TLP.DateTimeFrom,
                                         TLP.DateTimeTo,
                                         TLP.PickupNumber,
                                         TLP.Traitor,
                                         TLP.Comments,
                                         TLP.CreatedBy,
                                         TLP.CreatedDate,
                                         TLP.IsSave,
                                         TLP.Deliveryorder,

                                         Information = S.ShipperName + " , " + S.ShipperAddress + " , " + SD.CityName + " , " + SD.StateCode
                                         //+ " ," + Convert.ToString(TLP.DateTimeFrom)  
                                         //}).OrderBy(d=> d.DeliveryId).ToList();
                                     }).OrderBy(d => d.Deliveryorder).ToList();


                qry = "select max(DeliveryId) from tblLoadDelivery Where  LoadNumber ='" + LoaderNumber + "' ";

                string DeliverysId = ut.ExecuteScalar(qry);
                if (DeliverysId == "")
                {
                    DeliverysId = "0";
                }



                foreach (var dt in DeliveryQuery)

                //foreach (DataRow dr in dt.Rows)

                {

                    LoadPickupDelivery.Add(new PickupDelivery
                    {
                        PickUpId = dt.PickUpId,
                        Information = dt.Information,
                        ShipperName = dt.ShipperName,
                        Address = dt.ShipperAddress,
                        PhoneNumber = dt.PhoneNumber,
                        ZipCode = dt.ZipCode,
                        StateCode = dt.StateCode,
                        CityName = dt.CityName.ToString(),
                        CountryName = dt.CountryName.ToString(),
                        DateTimeFrom = Convert.ToDateTime(dt.DateTimeFrom),
                        DateTimeTo = Convert.ToDateTime(dt.DateTimeTo),
                        Traitor = dt.Traitor,
                        PickupNumber = dt.PickupNumber,
                        Comments = dt.Comments,
                        MaxDeliveryId = Convert.ToInt32(DeliverysId),
                        MaxPickupId = 0,
                        DeliveryId = dt.DeliveryId,
                        ShipperId = dt.ShipperId,
                        IsSave = 0,
                        LoadType = "D",
                        Longitude = dt.Longitude,
                        Latitude = dt.Latitude,
                        NewDateFrom = dt.DateTimeFrom.ToString("MM-dd-yyyy HH:mm"),
                        NewDateTo = dt.DateTimeTo.ToString("MM-dd-yyyy HH:mm"),
                        Pickuporder = 0,
                        Deliveryporder = dt.Deliveryorder
                    });

                }

                return Json(LoadPickupDelivery, JsonRequestBehavior.AllowGet);

            }
        }

        [HttpPost]
        [Customexception]
        public JsonResult MovePickuptoDelivery(Int32 txtDeliveryId, int[] PickupsId, int[] DeliveriesId)
        {

            string qry = " Exec SpGetLoadNumber ";
            string LoaderNumber = ut.ExecuteScalar(qry);
            using (DieseltechEntities entities = new DieseltechEntities())
            {


                qry = " Exec  Sp_Insert_Update_LoadDelivery  '" + LoaderNumber + "' , '',0, ";
                qry += " '' ,1,'','' ";
                qry += " ,'' ,0,'', '','" + DateTime.Now.ToString("yyyy-MM-dd") + "' ";
                qry += ", '" + DateTime.Now.ToString("yyyy-MM-dd") + "' , '', '','' ";
                qry += " ," + Convert.ToInt32(Session["User_id"]) + ",0,0,'',''," + txtDeliveryId + ",0,'C'   ";
                ut.InsertUpdate(qry);



                int preference = 1;


                foreach (int id in PickupsId)
                {
                    if (id != 0)
                    {
                        var loadpikcup = deEntity.tblLoadPickups.Find(id);
                        if (loadpikcup != null)
                        {
                            loadpikcup.Pickuporder = preference;
                            deEntity.SaveChanges();
                            preference += 1;
                        }

                    }

                }





                preference = 1;
                foreach (int id in DeliveriesId)
                {
                    if (id != 0)
                    {
                        var loaddelivery = deEntity.tblLoadDeliveries.Find(id);
                        if (loaddelivery != null)
                        {
                            loaddelivery.Deliveryorder = preference;
                            deEntity.SaveChanges();
                            preference += 1;
                        }

                    }

                }




                qry = "select max(PickUpId) from tblLoadPickup Where  LoadNumber ='" + LoaderNumber + "' ";
                string PickupId = ut.ExecuteScalar(qry);
                //ViewBag.LastPickupId = PickupId;

                if (PickupId == "")
                {
                    PickupId = "0";
                }

                List<PickupDelivery> LoadPickupDelivery = new List<PickupDelivery>();

                var query = (from TLP in deEntity.tblLoadPickups
                             join S in deEntity.tblShippers on TLP.ShipperId equals S.ShipperId
                             join SD in deEntity.tblStateCityDatas on TLP.ZipCode equals SD.ZipCode
                             where TLP.LoadNumber == LoaderNumber
                             select new
                             {
                                 CountryName = "United States",
                                 SD.ZipCode,
                                 SD.CityName,
                                 SD.StateCode,
                                 TLP.PickUpId,
                                 TLP.LoadNumber,
                                 S.ShipperId,
                                 S.Longitude,
                                 S.Latitude,
                                 S.ShipperName,
                                 PhoneNumber = S.ShipperPhone,
                                 S.ShipperAddress,
                                 TLP.DateTimeFrom,
                                 TLP.DateTimeTo,
                                 TLP.PickupNumber,
                                 TLP.Traitor,
                                 TLP.Comments,
                                 TLP.CreatedBy,
                                 TLP.CreatedDate,
                                 TLP.IsSave,
                                 TLP.Pickuporder,
                                 Information = S.ShipperName + " , " + S.ShipperAddress + " , " + SD.CityName + " , " + SD.StateCode
                                 //+ " ," + Convert.ToString(TLP.DateTimeFrom)  
                             }).OrderBy(p => p.PickUpId).ToList();


                //foreach (DataRow dr in dt.Rows)
                foreach (var dt in query)

                {

                    LoadPickupDelivery.Add(new PickupDelivery
                    {
                        PickUpId = dt.PickUpId,
                        Information = dt.Information,
                        ShipperName = dt.ShipperName,
                        Address = dt.ShipperAddress,
                        PhoneNumber = dt.PhoneNumber,
                        ZipCode = dt.ZipCode,
                        StateCode = dt.StateCode,
                        CityName = dt.CityName,
                        CountryName = dt.CountryName,
                        DateTimeFrom = Convert.ToDateTime(dt.DateTimeFrom),
                        DateTimeTo = Convert.ToDateTime(dt.DateTimeTo),
                        Traitor = dt.Traitor,
                        PickupNumber = dt.PickupNumber,
                        Comments = dt.Comments,
                        MaxPickupId = dt.PickUpId,
                        IsSave = 0,
                        ShipperId = dt.ShipperId,
                        MaxDeliveryId = 0,
                        LoadType = "P",
                        Longitude = dt.Longitude,
                        Latitude = dt.Longitude,
                        Pickuporder = dt.Pickuporder,
                        Deliveryporder = 0,
                        NewDateFrom = dt.DateTimeFrom.ToString("MM-dd-yyyy HH:mm"),
                        NewDateTo = dt.DateTimeTo.ToString("MM-dd-yyyy HH:mm"),
                    });

                }

                //qry = " select 'United States' as CountryName,  SD.ZipCode,SD.CityName,SD.StateCode, TLP.PickUpId, LoadNumber, S.ShipperId, ";
                //qry += "  ISNULL(S.Longitude,'0') as Longitude, ISNULL(S.Latitude,'0') as  Latitude,S.ShipperName, CountryId, S.ShipperPhone as PhoneNumber, Address, DateTimeFrom,";
                //qry += " DateTimeTo, PickupNumber, Traitor, Comments, CreatedBy, CreatedDate, IsSave";
                //qry += ",S.ShipperName + ' , ' + Address + ' , ' + SD.CityName +' , ' + SD.StateCode + ' , ' + CONVERT(varchar(10),DateTimeFrom,103) AS Information";
                //qry += " from tblLoadPickup TLP inner join tblStateCityData SD ON SD.ZipCode = TLP.ZipCode ";
                //qry += " inner join tblShipper S on S.ShipperId = TLP.ShipperId ";
                //qry += " Where TLP.LoadNumber ='" + LoaderNumber + "' ";
                //dt = ut.GetDatatable(qry);

                ////List<tblLoadPickup> LoadPickup = new List<tblLoadPickup>();
                //List<PickupDelivery> LoadPickupDelivery = new List<PickupDelivery>();


                //foreach (DataRow dr in dt.Rows)

                //{

                //    LoadPickupDelivery.Add(new PickupDelivery
                //    {
                //        PickUpId = Convert.ToInt32(dr["PickUpId"]),
                //        Information = (dr["Information"]).ToString(),
                //        ShipperName = (dr["ShipperName"]).ToString(),
                //        Address = (dr["Address"]).ToString(),
                //        PhoneNumber = (dr["PhoneNumber"]).ToString(),
                //        ZipCode = Convert.ToInt32(dr["ZipCode"]),
                //        StateCode = (dr["StateCode"]).ToString(),
                //        CityName = (dr["CityName"]).ToString(),
                //        CountryName = (dr["CountryName"]).ToString(),
                //        DateTimeFrom = Convert.ToDateTime(dr["DateTimeFrom"]),
                //        DateTimeTo = Convert.ToDateTime(dr["DateTimeTo"]),
                //        Traitor = (dr["Traitor"]).ToString(),
                //        PickupNumber = (dr["PickupNumber"]).ToString(),
                //        Comments = (dr["Comments"]).ToString(),
                //        MaxPickupId = Convert.ToInt32(PickupId),
                //        IsSave = 0,
                //        MaxDeliveryId = 0,
                //        LoadType = "P",
                //        Longitude = (dr["Longitude"]).ToString(),
                //        Latitude = (dr["Latitude"]).ToString(),
                //        ShipperId = Convert.ToInt32(dr["ShipperId"]),

                //    });

                //}

                var DeliveryQuery = (from TLP in deEntity.tblLoadDeliveries
                                     join S in deEntity.tblShippers on TLP.ShipperId equals S.ShipperId
                                     join SD in deEntity.tblStateCityDatas on TLP.ZipCode equals SD.ZipCode
                                     where TLP.LoadNumber == LoaderNumber
                                     select new
                                     {
                                         PickupId,
                                         TLP.DeliveryId,
                                         CountryName = "United States",
                                         SD.ZipCode,
                                         SD.CityName,
                                         SD.StateCode,
                                         TLP.PickUpId,
                                         TLP.LoadNumber,
                                         S.ShipperId,
                                         S.Longitude,
                                         S.Latitude,
                                         S.ShipperName,
                                         PhoneNumber = S.ShipperPhone,
                                         S.ShipperAddress,
                                         TLP.DateTimeFrom,
                                         TLP.DateTimeTo,
                                         TLP.PickupNumber,
                                         TLP.Traitor,
                                         TLP.Comments,
                                         TLP.CreatedBy,
                                         TLP.CreatedDate,
                                         TLP.IsSave,
                                         TLP.Deliveryorder,
                                         Information = S.ShipperName + " , " + S.ShipperAddress + " , " + SD.CityName + " , " + SD.StateCode
                                         //+ " ," + Convert.ToString(TLP.DateTimeFrom)  
                                     }).OrderBy(d => d.DeliveryId).ToList();


                qry = "select max(DeliveryId) from tblLoadDelivery Where  LoadNumber ='" + LoaderNumber + "' ";

                string DeliverysId = ut.ExecuteScalar(qry);
                if (DeliverysId == "")
                {
                    DeliverysId = "0";
                }



                foreach (var dt in DeliveryQuery)

                //foreach (DataRow dr in dt.Rows)

                {

                    LoadPickupDelivery.Add(new PickupDelivery
                    {
                        PickUpId = dt.PickUpId,
                        Information = dt.Information,
                        ShipperName = dt.ShipperName,
                        Address = dt.ShipperAddress,
                        PhoneNumber = dt.PhoneNumber,
                        ZipCode = dt.ZipCode,
                        StateCode = dt.StateCode,
                        CityName = dt.CityName.ToString(),
                        CountryName = dt.CountryName.ToString(),
                        DateTimeFrom = Convert.ToDateTime(dt.DateTimeFrom),
                        DateTimeTo = Convert.ToDateTime(dt.DateTimeTo),
                        Traitor = dt.Traitor,
                        PickupNumber = dt.PickupNumber,
                        Comments = dt.Comments,
                        MaxDeliveryId = Convert.ToInt32(DeliverysId),
                        MaxPickupId = 0,
                        DeliveryId = dt.DeliveryId,
                        ShipperId = dt.ShipperId,
                        IsSave = 0,
                        LoadType = "D",
                        Longitude = dt.Longitude,
                        Latitude = dt.Latitude,
                        Pickuporder = 0,
                        Deliveryporder = dt.Deliveryorder,
                        NewDateFrom = dt.DateTimeFrom.ToString("MM-dd-yyyy HH:mm"),
                        NewDateTo = dt.DateTimeTo.ToString("MM-dd-yyyy HH:mm"),
                    });

                }




                //qry = " select  PickUpId, DeliveryId ,'United States' as CountryName,  SD.ZipCode,SD.CityName,SD.StateCode, TLP.PickUpId, LoadNumber, S.ShipperId, ";
                //qry += "  ISNULL(S.Longitude,'0') as Longitude, ISNULL( S.Latitude,'0') as  Latitude,S.ShipperName, CountryId, S.ShipperPhone as PhoneNumber, Address, DateTimeFrom,";
                //qry += " DateTimeTo, PickupNumber, Traitor, Comments, CreatedBy, CreatedDate, IsSave";
                //qry += ",S.ShipperName + ' , ' + Address + ' , ' + SD.CityName +' , ' + SD.StateCode + ' , ' + CONVERT(varchar(10),DateTimeFrom,103) AS Information";
                //qry += " from tblLoadDelivery TLP inner join tblStateCityData SD ON SD.ZipCode = TLP.ZipCode ";
                //qry += " inner join tblShipper S on S.ShipperId = TLP.ShipperId ";
                //qry += " Where TLP.LoadNumber ='" + LoaderNumber + "' ";
                //dt = ut.GetDatatable(qry);



                //qry = "select max(DeliveryId) from tblLoadDelivery Where  LoadNumber ='" + LoaderNumber + "' ";

                //string DeliveryId = ut.ExecuteScalar(qry);
                //if (DeliveryId == "")
                //{
                //    DeliveryId = "0";
                //}



                //foreach (DataRow dr in dt.Rows)

                //{

                //    LoadPickupDelivery.Add(new PickupDelivery
                //    {
                //        PickUpId = Convert.ToInt32(dr["PickUpId"]),
                //        Information = (dr["Information"]).ToString(),
                //        ShipperName = (dr["ShipperName"]).ToString(),
                //        Address = (dr["Address"]).ToString(),
                //        PhoneNumber = (dr["PhoneNumber"]).ToString(),
                //        ZipCode = Convert.ToInt32(dr["ZipCode"]),
                //        StateCode = (dr["StateCode"]).ToString(),
                //        CityName = (dr["CityName"]).ToString(),
                //        CountryName = (dr["CountryName"]).ToString(),
                //        DateTimeFrom = Convert.ToDateTime(dr["DateTimeFrom"]),
                //        DateTimeTo = Convert.ToDateTime(dr["DateTimeTo"]),
                //        Traitor = (dr["Traitor"]).ToString(),
                //        PickupNumber = (dr["PickupNumber"]).ToString(),
                //        Comments = (dr["Comments"]).ToString(),
                //        MaxDeliveryId = Convert.ToInt32(DeliveryId),
                //        MaxPickupId = 0,
                //        DeliveryId = Convert.ToInt32(dr["DeliveryID"]),
                //        ShipperId = Convert.ToInt32(dr["ShipperId"]),
                //        IsSave = 0,
                //        LoadType = "D",
                //        Longitude = (dr["Longitude"]).ToString(),
                //        Latitude = (dr["Latitude"]).ToString(),
                //    });

                //}

                return Json(LoadPickupDelivery, JsonRequestBehavior.AllowGet);

            }
        }




        [HttpPost]
        [Customexception]
        public JsonResult DeleteLoadPickup(Int32 txtPickupId, int[] PickupsId, int[] DeliveriesId)
        {

            string qry = " Exec SpGetLoadNumber ";
            string LoaderNumber = ut.ExecuteScalar(qry);




            using (DieseltechEntities entities = new DieseltechEntities())
            {


                qry = " Exec  Sp_Insert_Update_LoadPickup  '' , '',0, ";
                qry += " '' ,1,'','' ";
                qry += " ,'' ,0,'', '','" + DateTime.Now.ToString("yyyy-MM-dd") + "' ";
                qry += ", '" + DateTime.Now.ToString("yyyy-MM-dd") + "' , '', '','' ";
                qry += " ," + Convert.ToInt32(Session["User_id"]) + ",0," + txtPickupId + ",'','',0 ,'D'   ";
                ut.InsertUpdate(qry);



                int preference = 1;
                foreach (int id in PickupsId)
                {
                    if (id != 0)
                    {
                        var loadpikcup = deEntity.tblLoadPickups.Find(id);
                        if (loadpikcup != null)
                        {
                            loadpikcup.Pickuporder = preference;
                            deEntity.SaveChanges();
                            preference += 1;
                        }

                    }

                }




                preference = 1;
                foreach (int id in DeliveriesId)
                {
                    if (id != 0)
                    {
                        var loaddelivery = deEntity.tblLoadDeliveries.Find(id);
                        if (loaddelivery != null)
                        {
                            loaddelivery.Deliveryorder = preference;
                            deEntity.SaveChanges();
                            preference += 1;
                        }

                    }

                }


                qry = "select max(PickUpId) from tblLoadPickup Where  LoadNumber ='" + LoaderNumber + "' ";
                string PickupId = ut.ExecuteScalar(qry);
                //ViewBag.LastPickupId = PickupId;

                if (PickupId == "")
                {
                    PickupId = "0";
                }
                //qry = " select 'United States' as CountryName,  SD.ZipCode,SD.CityName,SD.StateCode, TLP.PickUpId, LoadNumber, S.ShipperId, ";
                //qry += " ISNULL(S.Longitude,'0') as Longitude, ISNULL( S.Latitude,'0') as  Latitude,S.ShipperName, CountryId,S.ShipperPhone PhoneNumber, Address, DateTimeFrom,";
                //qry += " DateTimeTo, PickupNumber, Traitor, Comments, CreatedBy, CreatedDate, IsSave";
                //qry += ",S.ShipperName + ' , ' + Address + ' , ' + SD.CityName +' , ' + SD.StateCode + ' , ' + CONVERT(varchar(10),DateTimeFrom,103) AS Information";
                //qry += " from tblLoadPickup TLP inner join tblStateCityData SD ON SD.ZipCode = TLP.ZipCode ";
                //qry += " inner join tblShipper S on S.ShipperId = TLP.ShipperId ";
                //qry += " Where TLP.LoadNumber ='" + LoaderNumber + "' ";
                //dt = ut.GetDatatable(qry);

                //List<tblLoadPickup> LoadPickup = new List<tblLoadPickup>();
                List<PickupDelivery> LoadPickupDelivery = new List<PickupDelivery>();

                var query = (from TLP in deEntity.tblLoadPickups
                             join S in deEntity.tblShippers on TLP.ShipperId equals S.ShipperId
                             join SD in deEntity.tblStateCityDatas on TLP.ZipCode equals SD.ZipCode
                             where TLP.LoadNumber == LoaderNumber
                             select new
                             {
                                 CountryName = "United States",
                                 SD.ZipCode,
                                 SD.CityName,
                                 SD.StateCode,
                                 TLP.PickUpId,
                                 TLP.LoadNumber,
                                 S.ShipperId,
                                 S.Longitude,
                                 S.Latitude,
                                 S.ShipperName,
                                 PhoneNumber = S.ShipperPhone,
                                 S.ShipperAddress,
                                 TLP.DateTimeFrom,
                                 TLP.DateTimeTo,
                                 TLP.PickupNumber,
                                 TLP.Traitor,
                                 TLP.Comments,
                                 TLP.CreatedBy,
                                 TLP.CreatedDate,
                                 TLP.IsSave,
                                 TLP.Pickuporder,
                                 Information = S.ShipperName + " , " + S.ShipperAddress + " , " + SD.CityName + " , " + SD.StateCode
                                 //+ " ," + Convert.ToString(TLP.DateTimeFrom)  
                                 //}).OrderBy(p=> p.PickUpId).ToList();
                             }).OrderBy(p => p.Pickuporder).ToList();


                if (query.Count == 0)
                {
                    //LoadPickupDelivery.Add(new PickupDelivery
                    //{
                    //    Pickuporder = 1,
                    //    LoadType = "P"
                    //});
                }
                else if (query.Count > 0)
                {
                    //foreach (DataRow dr in dt.Rows)
                    foreach (var dt in query)

                    {

                        LoadPickupDelivery.Add(new PickupDelivery
                        {
                            PickUpId = dt.PickUpId,
                            Information = dt.Information,
                            ShipperName = dt.ShipperName,
                            Address = dt.ShipperAddress,
                            PhoneNumber = dt.PhoneNumber,
                            ZipCode = dt.ZipCode,
                            StateCode = dt.StateCode,
                            CityName = dt.CityName,
                            CountryName = dt.CountryName,
                            DateTimeFrom = Convert.ToDateTime(dt.DateTimeFrom),
                            DateTimeTo = Convert.ToDateTime(dt.DateTimeTo),
                            Traitor = dt.Traitor,
                            PickupNumber = dt.PickupNumber,
                            Comments = dt.Comments,
                            MaxPickupId = dt.PickUpId,
                            IsSave = 0,
                            ShipperId = dt.ShipperId,
                            MaxDeliveryId = 0,
                            LoadType = "P",
                            Longitude = dt.Longitude,
                            Latitude = dt.Longitude,
                            Pickuporder = dt.Pickuporder,
                            Deliveryporder = 0,
                            NewDateFrom = dt.DateTimeFrom.ToString("MM-dd-yyyy HH:mm"),
                            NewDateTo = dt.DateTimeTo.ToString("MM-dd-yyyy HH:mm")
                        });

                    }
                }








                //qry = " select  PickUpId, DeliveryId ,'United States' as CountryName,  SD.ZipCode,SD.CityName,SD.StateCode, TLP.PickUpId, LoadNumber,S.ShipperId, ";
                //qry += " ISNULL(S.Longitude,'0') as Longitude, ISNULL(S. Latitude,'0') as  Latitude,S.ShipperName, CountryId,S.ShipperPhone as  PhoneNumber, Address, DateTimeFrom,";
                //qry += " DateTimeTo, PickupNumber, Traitor, Comments, CreatedBy, CreatedDate, IsSave";
                //qry += ",S.ShipperName + ' , ' + Address + ' , ' + SD.CityName +' , ' + SD.StateCode + ' , ' + CONVERT(varchar(10),DateTimeFrom,103) AS Information";
                //qry += " from tblLoadDelivery TLP inner join tblStateCityData SD ON SD.ZipCode = TLP.ZipCode ";
                //qry += " inner join tblShipper S on S.ShipperId = TLP.ShipperId ";
                //qry += " Where TLP.LoadNumber ='" + LoaderNumber + "' ";
                //dt = ut.GetDatatable(qry);


                var DeliveryQuery = (from TLP in deEntity.tblLoadDeliveries
                                     join S in deEntity.tblShippers on TLP.ShipperId equals S.ShipperId
                                     join SD in deEntity.tblStateCityDatas on TLP.ZipCode equals SD.ZipCode
                                     where TLP.LoadNumber == LoaderNumber
                                     select new
                                     {
                                         PickupId,
                                         TLP.DeliveryId,
                                         CountryName = "United States",
                                         SD.ZipCode,
                                         SD.CityName,
                                         SD.StateCode,
                                         TLP.PickUpId,
                                         TLP.LoadNumber,
                                         S.ShipperId,
                                         S.Longitude,
                                         S.Latitude,
                                         S.ShipperName,
                                         PhoneNumber = S.ShipperPhone,
                                         S.ShipperAddress,
                                         TLP.DateTimeFrom,
                                         TLP.DateTimeTo,
                                         TLP.PickupNumber,
                                         TLP.Traitor,
                                         TLP.Comments,
                                         TLP.CreatedBy,
                                         TLP.CreatedDate,
                                         TLP.IsSave,
                                         TLP.Deliveryorder,
                                         Information = S.ShipperName + " , " + S.ShipperAddress + " , " + SD.CityName + " , " + SD.StateCode
                                         //+ " ," + Convert.ToString(TLP.DateTimeFrom)  
                                         //}).OrderBy(d=> d.DeliveryId).ToList();
                                     }).OrderBy(d => d.Deliveryorder).ToList();


                qry = "select max(DeliveryId) from tblLoadDelivery Where  LoadNumber ='" + LoaderNumber + "' ";

                string DeliverysId = ut.ExecuteScalar(qry);
                if (DeliverysId == "")
                {
                    DeliverysId = "0";
                }


                if (DeliveryQuery.Count == 0)
                {
                    //LoadPickupDelivery.Add(new PickupDelivery
                    //{
                    //    Deliveryporder = 1
                    //});
                }
                else if (DeliveryQuery.Count > 0)
                {
                    foreach (var dt in DeliveryQuery)

                    //foreach (DataRow dr in dt.Rows)

                    {

                        LoadPickupDelivery.Add(new PickupDelivery
                        {
                            PickUpId = dt.PickUpId,
                            Information = dt.Information,
                            ShipperName = dt.ShipperName,
                            Address = dt.ShipperAddress,
                            PhoneNumber = dt.PhoneNumber,
                            ZipCode = dt.ZipCode,
                            StateCode = dt.StateCode,
                            CityName = dt.CityName.ToString(),
                            CountryName = dt.CountryName.ToString(),
                            DateTimeFrom = Convert.ToDateTime(dt.DateTimeFrom),
                            DateTimeTo = Convert.ToDateTime(dt.DateTimeTo),
                            Traitor = dt.Traitor,
                            PickupNumber = dt.PickupNumber,
                            Comments = dt.Comments,
                            MaxDeliveryId = Convert.ToInt32(DeliverysId),
                            MaxPickupId = 0,
                            DeliveryId = dt.DeliveryId,
                            ShipperId = dt.ShipperId,
                            IsSave = 0,
                            LoadType = "D",
                            Longitude = dt.Longitude,
                            Latitude = dt.Latitude,
                            Pickuporder = 0,
                            Deliveryporder = dt.Deliveryorder,
                            NewDateFrom = dt.DateTimeFrom.ToString("MM-dd-yyyy HH:mm"),
                            NewDateTo = dt.DateTimeTo.ToString("MM-dd-yyyy HH:mm"),
                        });

                    }
                }



                return Json(LoadPickupDelivery, JsonRequestBehavior.AllowGet);

            }
        }


        [HttpPost]
        [Customexception]
        public JsonResult DeleteLoadDelivery(Int32 txtDeliveryId, int[] DeliverysId)
        {
            string qry = " Exec SpGetLoadNumber ";
            string LoaderNumber = ut.ExecuteScalar(qry);
            using (DieseltechEntities entities = new DieseltechEntities())
            {


                qry = " Exec  Sp_Insert_Update_LoadDelivery  '' , '',0, ";
                qry += " '' ,1,'','' ";
                qry += " ,'' ,0,'', '','" + DateTime.Now.ToString("yyyy-MM-dd") + "' ";
                qry += ", '" + DateTime.Now.ToString("yyyy-MM-dd") + "' , '', '','' ";
                qry += " ," + Convert.ToInt32(Session["User_id"]) + ",0,0,'',''," + txtDeliveryId + ",0,'D'   ";
                ut.InsertUpdate(qry);



                int preference = 1;
                foreach (int id in DeliverysId)
                {
                    if (id != 0)
                    {
                        var loaddelivery = deEntity.tblLoadDeliveries.Find(id);
                        if (loaddelivery != null)
                        {
                            loaddelivery.Deliveryorder = preference;
                            deEntity.SaveChanges();
                            preference += 1;
                        }

                    }

                }



                qry = "select max(DeliveryId) from tblLoadDelivery Where  LoadNumber ='" + LoaderNumber + "' ";
                string DeliveryId = ut.ExecuteScalar(qry);
                if (DeliveryId == "")
                {
                    DeliveryId = "0";
                }
                //ViewBag.LastPickupId = PickupId;

                //qry = " select 'United States' as CountryName,  SD.ZipCode,SD.CityName,SD.StateCode, TLP.PickUpId, LoadNumber, ShipperId, ";
                //qry += "ShipperName, CountryId, PhoneNumber, Address, DateTimeFrom,";
                //qry += " DateTimeTo, PickupNumber, Traitor, Comments, CreatedBy, CreatedDate, IsSave";
                //qry += ",ShipperName + ' , ' + Address + ' , ' + SD.CityName +' , ' + SD.StateCode + ' , ' + CONVERT(varchar(10),DateTimeFrom,103) AS Information";
                //qry += " from tblLoadPickup TLP inner join tblStateCityData SD ON SD.ZipCode = TLP.ZipCode ";
                //qry += " Where TLP.LoadNumber ='" + LoaderNumber + "' ";
                //dt = ut.GetDatatable(qry);

                //qry = " select  PickUpId, DeliveryId ,'United States' as CountryName,  SD.ZipCode,SD.CityName,SD.StateCode, TLP.PickUpId, LoadNumber,S.ShipperId, ";
                //qry += " ISNULL(S.Longitude,'0') as Longitude, ISNULL(S. Latitude,'0') as  Latitude,S.ShipperName, CountryId,S.ShipperPhone as  PhoneNumber, Address, DateTimeFrom,";
                //qry += " DateTimeTo, PickupNumber, Traitor, Comments, CreatedBy, CreatedDate, IsSave";
                //qry += ",S.ShipperName + ' , ' + Address + ' , ' + SD.CityName +' , ' + SD.StateCode + ' , ' + CONVERT(varchar(10),DateTimeFrom,103) AS Information";
                //qry += " from tblLoadDelivery TLP inner join tblStateCityData SD ON SD.ZipCode = TLP.ZipCode ";
                //qry += " inner join tblShipper S on S.ShipperId = TLP.ShipperId ";
                //qry += " Where TLP.LoadNumber ='" + LoaderNumber + "' ";
                //dt = ut.GetDatatable(qry);


                var DeliveryQuery = (from TLP in deEntity.tblLoadDeliveries
                                     join S in deEntity.tblShippers on TLP.ShipperId equals S.ShipperId
                                     join SD in deEntity.tblStateCityDatas on TLP.ZipCode equals SD.ZipCode
                                     where TLP.LoadNumber == LoaderNumber
                                     select new
                                     {
                                         TLP.PickUpId,
                                         TLP.DeliveryId,
                                         CountryName = "United States",
                                         SD.ZipCode,
                                         SD.CityName,
                                         SD.StateCode,
                                         TLP.LoadNumber,
                                         S.ShipperId,
                                         S.Longitude,
                                         S.Latitude,
                                         S.ShipperName,
                                         PhoneNumber = S.ShipperPhone,
                                         S.ShipperAddress,
                                         TLP.DateTimeFrom,
                                         TLP.DateTimeTo,
                                         TLP.PickupNumber,
                                         TLP.Traitor,
                                         TLP.Comments,
                                         TLP.CreatedBy,
                                         TLP.CreatedDate,
                                         TLP.IsSave,
                                         TLP.Deliveryorder,
                                         Information = S.ShipperName + " , " + S.ShipperAddress + " , " + SD.CityName + " , " + SD.StateCode
                                         //+ " ," + Convert.ToString(TLP.DateTimeFrom)  
                                         //}).OrderBy(d=> d.DeliveryId).ToList();

                                     }).OrderBy(d => d.Deliveryorder).ToList();



                List<tblLoadDelivery> LoadDelivery = new List<tblLoadDelivery>();

                foreach (var dt in DeliveryQuery)

                //foreach (DataRow dr in dt.Rows)

                {

                    LoadDelivery.Add(new tblLoadDelivery
                    {


                        PickUpId = dt.PickUpId,
                        Information = dt.Information,
                        ShipperName = dt.ShipperName,
                        Address = dt.ShipperAddress,
                        PhoneNumber = dt.PhoneNumber,
                        ZipCode = dt.ZipCode,
                        StateCode = dt.StateCode,
                        CityName = dt.CityName.ToString(),
                        CountryName = dt.CountryName.ToString(),
                        DateTimeFrom = Convert.ToDateTime(dt.DateTimeFrom),
                        DateTimeTo = Convert.ToDateTime(dt.DateTimeTo),
                        Traitor = dt.Traitor,
                        PickupNumber = dt.PickupNumber,
                        Comments = dt.Comments,
                        MaxDeliveryID = Convert.ToInt32(dt.DeliveryId),
                        //DeliveryId = 0,
                        DeliveryId = dt.DeliveryId,
                        ShipperId = dt.ShipperId,
                        IsSave = 2,
                        Longitude = dt.Longitude,
                        Latitude = dt.Latitude,
                        NewDateFrom = dt.DateTimeFrom.ToString("MM-dd-yyyy HH:mm"),
                        NewDateTo = dt.DateTimeTo.ToString("MM-dd-yyyy HH:mm"),
                        Deliveryorder = dt.Deliveryorder
                        //PickUpId = Convert.ToInt32(dr["PickUpId"]),
                        //Information = (dr["Information"]).ToString(),
                        //ShipperName = (dr["ShipperName"]).ToString(),
                        //Address = (dr["Address"]).ToString(),
                        //PhoneNumber = (dr["PhoneNumber"]).ToString(),
                        //ZipCode = Convert.ToInt32(dr["ZipCode"]),
                        //StateCode = (dr["StateCode"]).ToString(),
                        //CityName = (dr["CityName"]).ToString(),
                        //CountryName = (dr["CountryName"]).ToString(),
                        //DateTimeFrom = Convert.ToDateTime(dr["DateTimeFrom"]),
                        //DateTimeTo = Convert.ToDateTime(dr["DateTimeTo"]),
                        //Traitor = (dr["Traitor"]).ToString(),
                        //PickupNumber = (dr["PickupNumber"]).ToString(),
                        //Comments = (dr["Comments"]).ToString(),
                        //MaxDeliveryID = Convert.ToInt32(DeliveryId),
                        //DeliveryId = Convert.ToInt32(dr["DeliveryID"]),
                        //ShipperId = Convert.ToInt32(dr["ShipperId"]),
                        //IsSave = 2,
                        //Longitude = (dr["Longitude"]).ToString(),
                        //Latitude = (dr["Latitude"]).ToString(),
                    });

                }



                return Json(LoadDelivery, JsonRequestBehavior.AllowGet);

            }
        }


        [HttpPost]
        [Customexception]
        public ActionResult ReorderPickup(int[] PickupId, string LoaderNumber="")
        {
            string qry = "";
            if (LoaderNumber == null || LoaderNumber=="")
            {
                qry = " Exec SpGetLoadNumber ";
                LoaderNumber = ut.ExecuteScalar(qry);
            }


            
            
            int preference = 1;
            foreach (int id in PickupId)
            {
                if (id != 0)
                {
                    var loadpikcup = deEntity.tblLoadPickups.Find(id);
                    loadpikcup.Pickuporder = preference;
                    deEntity.SaveChanges();
                    preference += 1;
                }

            }



            var query = (from TLP in deEntity.tblLoadPickups
                         join S in deEntity.tblShippers on TLP.ShipperId equals S.ShipperId
                         join SD in deEntity.tblStateCityDatas on TLP.ZipCode equals SD.ZipCode
                         where TLP.LoadNumber == LoaderNumber
                         select new
                         {
                             CountryName = "United States",
                             SD.ZipCode,
                             SD.CityName,
                             SD.StateCode,
                             TLP.PickUpId,
                             TLP.LoadNumber,
                             S.ShipperId,
                             S.Longitude,
                             S.Latitude,
                             S.ShipperName,
                             PhoneNumber = S.ShipperPhone,
                             S.ShipperAddress,
                             TLP.DateTimeFrom,
                             TLP.DateTimeTo,
                             TLP.PickupNumber,
                             TLP.Traitor,
                             TLP.Comments,
                             TLP.CreatedBy,
                             TLP.CreatedDate,
                             TLP.IsSave,
                             TLP.Pickuporder,
                             Information = S.ShipperName + " , " + S.ShipperAddress + " , " + SD.CityName + " , " + SD.StateCode
                             //+ " ," + Convert.ToString(TLP.DateTimeFrom)  
                         }).OrderBy(P => P.Pickuporder).ToList();

            List<tblLoadPickup> LoadPickup = new List<tblLoadPickup>();

            foreach (var dt in query)

            {

                LoadPickup.Add(new tblLoadPickup
                {
                    PickUpId = dt.PickUpId,
                    Information = dt.Information,
                    ShipperName = dt.ShipperName,
                    Address = dt.ShipperAddress,
                    PhoneNumber = dt.PhoneNumber,
                    ZipCode = dt.ZipCode,
                    StateCode = dt.StateCode,
                    CityName = dt.CityName,
                    CountryName = dt.CountryName,
                    DateTimeFrom = dt.DateTimeFrom,
                    DateTimeTo = dt.DateTimeTo,
                    Traitor = dt.Traitor,
                    PickupNumber = dt.PickupNumber,
                    Comments = dt.Comments,
                    MaxPickupId = dt.PickUpId,
                    ShipperId = dt.ShipperId,
                    Longitude = dt.Longitude,
                    Latitude = dt.Latitude,
                    NewDateFrom = dt.DateTimeFrom.ToString("MM-dd-yyyy HH:mm"),
                    NewDateTo = dt.DateTimeTo.ToString("MM-dd-yyyy HH:mm"),
                    Pickuporder = dt.Pickuporder,
                    //Pickuporder = Convert.ToInt32(Pickuporder),
                    IsSave = 1,
                });

            }



            return Json(LoadPickup, JsonRequestBehavior.AllowGet);

        }


        [HttpPost]
        [Customexception]
        public ActionResult ReorderDelivery(int[] DeliverysId, string LoaderNumber = "")
        {
            List<tblLoadDelivery> LoadDelivery = new List<tblLoadDelivery>();

            string qry = "";
            if (LoaderNumber == null || LoaderNumber == "")
            {
                qry = " Exec SpGetLoadNumber ";
                LoaderNumber = ut.ExecuteScalar(qry);
            }

            try
            {
             
                int preference = 1;
                foreach (int id in DeliverysId)
                {
                    if (id != 0)
                    {
                        var loaddelivery = deEntity.tblLoadDeliveries.Find(id);
                        loaddelivery.Deliveryorder = preference;
                        deEntity.SaveChanges();
                        preference += 1;
                    }

                }


                var DeliveryQuery = (from TLP in deEntity.tblLoadDeliveries
                                     join S in deEntity.tblShippers on TLP.ShipperId equals S.ShipperId
                                     join SD in deEntity.tblStateCityDatas on TLP.ZipCode equals SD.ZipCode
                                     where TLP.LoadNumber == LoaderNumber
                                     select new
                                     {
                                         TLP.PickUpId,
                                         TLP.DeliveryId,
                                         CountryName = "United States",
                                         SD.ZipCode,
                                         SD.CityName,
                                         SD.StateCode,
                                         TLP.LoadNumber,
                                         S.ShipperId,
                                         S.Longitude,
                                         S.Latitude,
                                         S.ShipperName,
                                         PhoneNumber = S.ShipperPhone,
                                         S.ShipperAddress,
                                         TLP.DateTimeFrom,
                                         TLP.DateTimeTo,
                                         TLP.PickupNumber,
                                         TLP.Traitor,
                                         TLP.Comments,
                                         TLP.CreatedBy,
                                         TLP.CreatedDate,
                                         TLP.IsSave,
                                         TLP.Deliveryorder,
                                         Information = S.ShipperName + " , " + S.ShipperAddress + " , " + SD.CityName + " , " + SD.StateCode
                                         //+ " ," + Convert.ToString(TLP.DateTimeFrom)  
                                         //}).OrderBy(d=> d.DeliveryId).ToList();
                                     }).OrderBy(d => d.Deliveryorder).ToList();





                foreach (var dt in DeliveryQuery)

                //foreach (DataRow dr in dt.Rows)

                {

                    LoadDelivery.Add(new tblLoadDelivery
                    {


                        PickUpId = dt.PickUpId,
                        Information = dt.Information,
                        ShipperName = dt.ShipperName,
                        Address = dt.ShipperAddress,
                        PhoneNumber = dt.PhoneNumber,
                        ZipCode = dt.ZipCode,
                        StateCode = dt.StateCode,
                        CityName = dt.CityName.ToString(),
                        CountryName = dt.CountryName.ToString(),
                        DateTimeFrom = Convert.ToDateTime(dt.DateTimeFrom),
                        DateTimeTo = Convert.ToDateTime(dt.DateTimeTo),
                        Traitor = dt.Traitor,
                        PickupNumber = dt.PickupNumber,
                        Comments = dt.Comments,
                        MaxDeliveryID = Convert.ToInt32(dt.DeliveryId),
                        //DeliveryId = 0,
                        DeliveryId = dt.DeliveryId,
                        ShipperId = dt.ShipperId,
                        IsSave = 1,
                        Longitude = dt.Longitude,
                        Latitude = dt.Latitude,
                        NewDateFrom = dt.DateTimeFrom.ToString("MM-dd-yyyy HH:mm"),
                        NewDateTo = dt.DateTimeTo.ToString("MM-dd-yyyy HH:mm"),
                        Deliveryorder = dt.Deliveryorder,

                    });

                }


                return Json(LoadDelivery, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }

            return Json(LoadDelivery, JsonRequestBehavior.AllowGet);

        }

        //Final Function to Save Load Pickup
        [Customexception]
        public JsonResult InsertLoadPickup(Int32 txtPickupId, List<tblLoadPickup> LoadPickupList)
        {

            string qry = " Exec SpGetLoadNumber ";
            string LoaderNumber = ut.ExecuteScalar(qry);

            if (txtPickupId == 0)
            {
                dt = new DataTable();
                using (DieseltechEntities entities = new DieseltechEntities())
                {
                    if (LoadPickupList == null)
                    {
                        LoadPickupList = new List<tblLoadPickup>();
                    }


                    //Loop and insert records.
                    foreach (tblLoadPickup Pickup in LoadPickupList)
                    {
                        //if (Pickup.ShipperId ==0)
                        //{
                        //    qry =""
                        //}
                        string ShipperAssignID = "";
                        if (Pickup.ShipperId == 0)
                        {
                            ShipperAssignID = ut.ExecuteScalar("Exec SpGetShipperNumber");
                        }


                        //Insert or Update Shipper
                        qry = "Exec Sp_InsertUpdate_Shipper " + Pickup.ShipperId + ",'" + Pickup.ShipperName + "','" + Pickup.PhoneNumber + "','" + Pickup.Address + "' ,";
                        qry += " '" + Pickup.CityName + "' , '" + Pickup.StateCode + "', '" + Pickup.StateCode + "','" + Pickup.ZipCode + "' ,'" + ShipperAssignID + "'";
                        qry += " , '" + Pickup.Longitude + "' , '" + Pickup.Latitude + "' ";
                        ut.InsertUpdate(qry);

                        Int32 ShipperPrimaryId = 0;
                        if (Pickup.ShipperId == 0)
                        {
                            ShipperPrimaryId = Convert.ToInt32(ut.ExecuteScalar("select ShipperId FROM tblShipper Where ShipperAssignId ='" + ShipperAssignID + "'"));
                        }
                        else if (Pickup.ShipperId != 0)
                        {
                            ShipperPrimaryId = Convert.ToInt32(ut.ExecuteScalar("select ShipperId FROM tblShipper Where ShipperId ='" + Pickup.ShipperId + "'"));
                        }



                        if (Pickup.Comments == null)
                        {
                            Pickup.Comments = "";
                        }
                        Pickup.LoadNumber = LoaderNumber;
                        Pickup.CountryId = 1;
                        Pickup.CountryName = "United States";
                        Pickup.CreatedBy = Convert.ToInt32(Session["User_id"]);
                        Pickup.IsSave = 0;
                        Pickup.StateCode = "";
                        Pickup.CreatedDate = DateTime.Now;

                        if (Pickup.PickupNumber == null)
                        {
                            Pickup.PickupNumber = " ";
                        }

                        if (Pickup.Pickuporder == null)
                        {
                            Pickup.Pickuporder = 0;
                        }


                        Pickup.Traitor = "";
                        Pickup.PhoneNumber = "";
                        Pickup.ShipperId = ShipperPrimaryId;
                        entities.tblLoadPickups.Add(Pickup);
                    }


                    int insertedRecords = entities.SaveChanges();

                    qry = "select max(PickUpId) from tblLoadPickup Where  LoadNumber ='" + LoaderNumber + "' ";
                    string PickupId = ut.ExecuteScalar(qry);


                    //qry = "select max(Pickuporder) from tblLoadPickup Where  LoadNumber ='" + LoaderNumber + "' ";
                    //string Pickuporder = ut.ExecuteScalar(qry);


                    //ViewBag.LastPickupId = PickupId;

                    //qry = " select 'United States' as CountryName,  SD.ZipCode,SD.CityName,SD.StateCode, TLP.PickUpId, LoadNumber, S.ShipperId, ";
                    //qry += " isnull(S.Longitude,'0') as  Longitude ,isnull(S.Latitude,'0') as  Latitude , S.ShipperName, CountryId, S.ShipperPhone as PhoneNumber, Address, DateTimeFrom,";
                    //qry += " DateTimeTo, PickupNumber, Traitor, Comments, CreatedBy, CreatedDate, IsSave";
                    //qry += ",S.ShipperName + ' , ' + Address + ' , ' + SD.CityName +' , ' + SD.StateCode + ' , ' + CONVERT(varchar(10),DateTimeFrom,103) AS Information";
                    //qry += " from tblLoadPickup TLP inner join tblStateCityData SD ON SD.ZipCode = TLP.ZipCode ";
                    //qry += " inner join tblShipper S on S.ShipperId = TLP.ShipperId ";
                    //qry += " Where TLP.LoadNumber ='" + LoaderNumber + "' ";
                    //dt = ut.GetDatatable(qry);


                    var query = (from TLP in deEntity.tblLoadPickups
                                 join S in deEntity.tblShippers on TLP.ShipperId equals S.ShipperId
                                 join SD in deEntity.tblStateCityDatas on TLP.ZipCode equals SD.ZipCode
                                 where TLP.LoadNumber == LoaderNumber
                                 select new
                                 {
                                     CountryName = "United States",
                                     SD.ZipCode,
                                     SD.CityName,
                                     SD.StateCode,
                                     TLP.PickUpId,
                                     TLP.LoadNumber,
                                     S.ShipperId,
                                     S.Longitude,
                                     S.Latitude,
                                     S.ShipperName,
                                     PhoneNumber = S.ShipperPhone,
                                     S.ShipperAddress,
                                     TLP.DateTimeFrom,
                                     TLP.DateTimeTo,
                                     TLP.PickupNumber,
                                     TLP.Traitor,
                                     TLP.Comments,
                                     TLP.CreatedBy,
                                     TLP.CreatedDate,
                                     TLP.IsSave,
                                     TLP.Pickuporder,
                                     Information = S.ShipperName + " , " + S.ShipperAddress + " , " + SD.CityName + " , " + SD.StateCode
                                     //+ " ," + Convert.ToString(TLP.DateTimeFrom)  
                                 }).OrderBy(P => P.Pickuporder).ToList();

                    //}).OrderBy(P => P.PickUpId).ToList();


                    //select new
                    //{
                    //    CountryName = "United States",
                    //    SD.ZipCode,
                    //    SD.CityName,
                    //    SD.StateCode,
                    //    TLP.PickUpId,
                    //    TLP.LoadNumber,
                    //    S.ShipperId,
                    //    S.Longitude,
                    //    S.Latitude,
                    //    S.ShipperName,
                    //    PhoneNumber = S.ShipperPhone,
                    //    S.ShipperAddress,
                    //    TLP.DateTimeFrom,
                    //    TLP.DateTimeTo,
                    //    TLP.PickupNumber,
                    //    TLP.Traitor,
                    //    TLP.Comments,
                    //    TLP.CreatedBy,
                    //    TLP.CreatedDate,
                    //    TLP.IsSave,

                    //    Information = S.ShipperName + " , " + S.ShipperAddress + " , " + SD.CityName + " , " + SD.StateCode + " ," + TLP.DateTimeFrom.ToString("dd/MM/yyyy")
                    //}).ToList();



                    List<tblLoadPickup> LoadPickup = new List<tblLoadPickup>();

                    foreach (var dt in query)

                    {

                        LoadPickup.Add(new tblLoadPickup
                        {
                            PickUpId = dt.PickUpId,
                            Information = dt.Information,
                            ShipperName = dt.ShipperName,
                            Address = dt.ShipperAddress,
                            PhoneNumber = dt.PhoneNumber,
                            ZipCode = dt.ZipCode,
                            StateCode = dt.StateCode,
                            CityName = dt.CityName,
                            CountryName = dt.CountryName,
                            DateTimeFrom = dt.DateTimeFrom,
                            DateTimeTo = dt.DateTimeTo,
                            Traitor = dt.Traitor,
                            PickupNumber = dt.PickupNumber,
                            Comments = dt.Comments,
                            MaxPickupId = dt.PickUpId,
                            ShipperId = dt.ShipperId,
                            Longitude = dt.Longitude,
                            Latitude = dt.Latitude,
                            NewDateFrom = dt.DateTimeFrom.ToString("MM-dd-yyyy HH:mm"),
                            NewDateTo = dt.DateTimeTo.ToString("MM-dd-yyyy HH:mm"),
                            Pickuporder = dt.Pickuporder,
                            //Pickuporder = Convert.ToInt32(Pickuporder),
                            IsSave = 1,
                        });

                    }



                    return Json(LoadPickup, JsonRequestBehavior.AllowGet);

                }
                //return Json("Test");
            }
            else
            {
                using (DieseltechEntities entities = new DieseltechEntities())
                {


                    //Loop and insert records.
                    foreach (tblLoadPickup Pickup in LoadPickupList)
                    {

                        string ShipperAssignID = "";
                        if (Pickup.ShipperId == 0)
                        {
                            ShipperAssignID = ut.ExecuteScalar("Exec SpGetShipperNumber");
                        }


                        //Insert or Update Shipper
                        qry = "Exec Sp_InsertUpdate_Shipper " + Pickup.ShipperId + ",'" + Pickup.ShipperName + "','" + Pickup.PhoneNumber + "','" + Pickup.Address + "' ,";
                        qry += " '" + Pickup.CityName + "' , '" + Pickup.StateCode + "', '" + Pickup.StateCode + "','" + Pickup.ZipCode + "' ,'" + ShipperAssignID + "'";
                        qry += " , '" + Pickup.Longitude + "' , '" + Pickup.Latitude + "' ";
                        ut.InsertUpdate(qry);


                        //Pickup.LoadNumber = "2010028";
                        //Pickup.CountryId = 1;
                        //Pickup.CountryName = "United States";
                        //Pickup.CreatedBy = Convert.ToInt32(Session["User_id"]);
                        //Pickup.IsSave = 0;
                        //Pickup.ShipperId = 0;
                        //Pickup.StateCode = "";
                        //Pickup.CreatedDate = DateTime.Now;
                        Pickup.PickUpId = txtPickupId;
                        qry = " Exec  Sp_Insert_Update_LoadPickup  '" + LoaderNumber + "' , '" + Pickup.Information + "'," + Pickup.ShipperId + ", ";
                        qry += " '" + Pickup.ShipperName + "' ,1,'" + Pickup.CountryName + "','" + Pickup.PhoneNumber + "' ";
                        qry += " ,'" + Pickup.Address + "' ,'" + Pickup.ZipCode + "','', '" + Pickup.CityName + "','" + Pickup.DateTimeFrom.ToString("yyyy-MM-dd HH:mm") + "' ";
                        qry += ", '" + Pickup.DateTimeTo.ToString("yyyy-MM-dd HH:mm") + "' , '" + Pickup.PickupNumber + "', '" + Pickup.Traitor + "','" + Pickup.Comments + "' ";
                        qry += " ," + Convert.ToInt32(Session["User_id"]) + ",0," + Pickup.PickUpId + " ,'" + Pickup.Longitude + "','" + Pickup.Latitude + "'," + Pickup.Pickuporder + ",'U'   ";
                        ut.InsertUpdate(qry);


                    }


                    qry = "select max(PickUpId) from tblLoadPickup Where  LoadNumber ='" + LoaderNumber + "' ";
                    string PickupId = ut.ExecuteScalar(qry);
                    //ViewBag.LastPickupId = PickupId;


                    //qry = " select 'United States' as CountryName,  SD.ZipCode,SD.CityName,SD.StateCode, TLP.PickUpId, LoadNumber, S.ShipperId, ";
                    //qry += " isnull(S.Longitude,'0') as  Longitude ,isnull(S.Latitude,'0') as  Latitude , S.ShipperName, CountryId,S.ShipperPhone as  PhoneNumber, Address, DateTimeFrom,";
                    //qry += " DateTimeTo, PickupNumber, Traitor, Comments, CreatedBy, CreatedDate, IsSave";
                    //qry += ",S.ShipperName + ' , ' + Address + ' , ' + SD.CityName +' , ' + SD.StateCode + ' , ' + CONVERT(varchar(10),DateTimeFrom,103) AS Information";
                    //qry += " from tblLoadPickup TLP inner join tblStateCityData SD ON SD.ZipCode = TLP.ZipCode ";
                    //qry += " inner join tblShipper S on S.ShipperId = TLP.ShipperId ";
                    //qry += " Where TLP.LoadNumber ='" + LoaderNumber + "' ";
                    //dt = ut.GetDatatable(qry);

                    var query = (from TLP in deEntity.tblLoadPickups
                                 join S in deEntity.tblShippers on TLP.ShipperId equals S.ShipperId
                                 join SD in deEntity.tblStateCityDatas on TLP.ZipCode equals SD.ZipCode
                                 where TLP.LoadNumber == LoaderNumber
                                 select new
                                 {

                                     CountryName = "United States",
                                     SD.ZipCode,
                                     SD.CityName,
                                     SD.StateCode,
                                     TLP.PickUpId,
                                     TLP.LoadNumber,
                                     S.ShipperId,
                                     S.Longitude,
                                     S.Latitude,
                                     S.ShipperName,
                                     PhoneNumber = S.ShipperPhone,
                                     S.ShipperAddress,
                                     TLP.DateTimeFrom,
                                     TLP.DateTimeTo,
                                     TLP.PickupNumber,
                                     TLP.Traitor,
                                     TLP.Comments,
                                     TLP.CreatedBy,
                                     TLP.CreatedDate,
                                     TLP.IsSave,
                                     TLP.Pickuporder,
                                     Information = S.ShipperName + " , " + S.ShipperAddress + " , " + SD.CityName + " , " + SD.StateCode
                                     //+ " ," + TLP.DateTimeFrom.ToString("dd-MMM-yy")  
                                     //}).OrderBy(P => P.PickUpId).ToList();
                                 }).OrderBy(P => P.Pickuporder).ToList();



                    List<tblLoadPickup> LoadPickup = new List<tblLoadPickup>();

                    foreach (var dt in query)

                    {

                        LoadPickup.Add(new tblLoadPickup
                        {
                            PickUpId = dt.PickUpId,
                            Information = dt.Information,
                            ShipperName = dt.ShipperName,
                            Address = dt.ShipperAddress,
                            PhoneNumber = dt.PhoneNumber,
                            ZipCode = dt.ZipCode,
                            StateCode = dt.StateCode,
                            CityName = dt.CityName,
                            CountryName = dt.CountryName,
                            DateTimeFrom = dt.DateTimeFrom,
                            DateTimeTo = dt.DateTimeTo,
                            Traitor = dt.Traitor,
                            PickupNumber = dt.PickupNumber,
                            Comments = dt.Comments,
                            MaxPickupId = dt.PickUpId,
                            ShipperId = dt.ShipperId,
                            Longitude = dt.Longitude,
                            Latitude = dt.Latitude,
                            Pickuporder = dt.Pickuporder,
                            NewDateFrom = dt.DateTimeFrom.ToString("MM-dd-yyyy HH:mm"),
                            NewDateTo = dt.DateTimeTo.ToString("MM-dd-yyyy HH:mm"),
                            IsSave = 2,

                            //PickUpId = Convert.ToInt32(dr["PickUpId"]),
                            //Information = (dr["Information"]).ToString(),
                            //ShipperName = (dr["ShipperName"]).ToString(),
                            //Address = (dr["Address"]).ToString(),
                            //PhoneNumber = (dr["PhoneNumber"]).ToString(),
                            //ZipCode = Convert.ToInt32(dr["ZipCode"]),
                            //StateCode = (dr["StateCode"]).ToString(),
                            //CityName = (dr["CityName"]).ToString(),
                            //CountryName = (dr["CountryName"]).ToString(),
                            //////DateTimeFrom = DateTime.ParseExact (Convert.ToString(dr["DateTimeFrom"]), "dd-MM-yyyy HH:mm:ss",null),
                            //DateTimeFrom = Convert.ToDateTime(dr["DateTimeFrom"]),
                            //DateTimeTo = Convert.ToDateTime(dr["DateTimeTo"]),
                            //Traitor = (dr["Traitor"]).ToString(),
                            //PickupNumber = (dr["PickupNumber"]).ToString(),
                            //Comments = (dr["Comments"]).ToString(),
                            //MaxPickupId = Convert.ToInt32(PickupId),
                            //ShipperId = Convert.ToInt32(dr["ShipperId"]),
                            //IsSave = 2,
                            //Longitude = (dr["Longitude"]).ToString(),
                            //Latitude = (dr["Latitude"]).ToString(),
                        });

                    }
                    return Json(LoadPickup, JsonRequestBehavior.AllowGet);

                }
                //return Json("", JsonRequestBehavior.AllowGet);
            }


        }


        [Customexception]
        public JsonResult UpdateLoadStatus(string LoadNumber, int LoadStatus)
        {
            string Query = "";
            try
            {
                Query = " Update tblLoadHead set IsCancel = " + LoadStatus + " Where LoaderNumber ='" + LoadNumber + "' ";
                ut.InsertUpdate(Query);
                return Json("1", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Exception occur while updating Load Status:" + ex.Message;
            }
            return Json("1", JsonRequestBehavior.AllowGet);
        }

        [Customexception]
        public JsonResult InsertLoadDelivery(Int32 txtDeliveryId, List<tblLoadDelivery> LoadDeliveryList)
        {
            string qry = " Exec SpGetLoadNumber ";
            string LoaderNumber = ut.ExecuteScalar(qry);

            if (txtDeliveryId == 0)
            {
                dt = new DataTable();
                using (DieseltechEntities entities = new DieseltechEntities())
                {
                    if (LoadDeliveryList == null)
                    {
                        LoadDeliveryList = new List<tblLoadDelivery>();
                    }



                    //Loop and insert records.
                    foreach (tblLoadDelivery Load in LoadDeliveryList)
                    {
                        string ShipperAssignID = "";
                        if (Load.ShipperId == 0)
                        {
                            ShipperAssignID = ut.ExecuteScalar("Exec SpGetShipperNumber");
                        }

                        //Insert or Update Shipper
                        qry = "Exec Sp_InsertUpdate_Shipper " + Load.ShipperId + ",'" + Load.ShipperName + "','" + Load.PhoneNumber + "','" + Load.Address + "' ,";
                        qry += " '" + Load.CityName + "' , '" + Load.StateCode + "', '" + Load.StateCode + "','" + Load.ZipCode + "' ,'" + ShipperAssignID + "'";
                        qry += ", '" + Load.Longitude + "' , '" + Load.Latitude + "' ";

                        ut.InsertUpdate(qry);


                        Int32 ShipperPrimaryId = 0;
                        if (Load.ShipperId == 0)
                        {
                            ShipperPrimaryId = Convert.ToInt32(ut.ExecuteScalar("select ShipperId FROM tblShipper Where ShipperAssignId ='" + ShipperAssignID + "'"));
                        }
                        else if (Load.ShipperId != 0)
                        {
                            ShipperPrimaryId = Convert.ToInt32(ut.ExecuteScalar("select ShipperId FROM tblShipper Where ShipperId ='" + Load.ShipperId + "'"));
                        }



                        if (Load.Comments == null)
                        {
                            Load.Comments = "";
                        }
                        Load.LoadNumber = LoaderNumber;
                        Load.CountryId = 1;
                        Load.CountryName = "United States";
                        Load.CreatedBy = Convert.ToInt32(Session["User_id"]);
                        Load.IsSave = 0;
                        Load.StateCode = "";
                        Load.CreatedDate = DateTime.Now;
                        if (Load.PickupNumber == null)
                        {
                            Load.PickupNumber = " ";
                        }

                        if (Load.Deliveryorder == null)
                        {
                            Load.Deliveryorder = 0;
                        }
                        Load.Traitor = "";
                        Load.PhoneNumber = "";
                        Load.ShipperId = ShipperPrimaryId;
                        entities.tblLoadDeliveries.Add(Load);
                    }


                    int insertedRecords = entities.SaveChanges();

                    qry = "select max(DeliveryId) from tblLoadDelivery Where  LoadNumber ='" + LoaderNumber + "' ";
                    string DeliveryId = ut.ExecuteScalar(qry);
                    //ViewBag.LastPickupId = PickupId;

                    var DeliveryQuery = (from TLP in deEntity.tblLoadDeliveries
                                         join S in deEntity.tblShippers on TLP.ShipperId equals S.ShipperId
                                         join SD in deEntity.tblStateCityDatas on TLP.ZipCode equals SD.ZipCode
                                         where TLP.LoadNumber == LoaderNumber
                                         select new
                                         {
                                             TLP.PickUpId,
                                             TLP.DeliveryId,
                                             CountryName = "United States",
                                             SD.ZipCode,
                                             SD.CityName,
                                             SD.StateCode,
                                             TLP.LoadNumber,
                                             S.ShipperId,
                                             S.Longitude,
                                             S.Latitude,
                                             S.ShipperName,
                                             PhoneNumber = S.ShipperPhone,
                                             S.ShipperAddress,
                                             TLP.DateTimeFrom,
                                             TLP.DateTimeTo,
                                             TLP.PickupNumber,
                                             TLP.Traitor,
                                             TLP.Comments,
                                             TLP.CreatedBy,
                                             TLP.CreatedDate,
                                             TLP.IsSave,
                                             TLP.Deliveryorder,
                                             Information = S.ShipperName + " , " + S.ShipperAddress + " , " + SD.CityName + " , " + SD.StateCode
                                             //+ " ," + Convert.ToString(TLP.DateTimeFrom)  
                                             //}).OrderBy(d=> d.DeliveryId).ToList();
                                         }).OrderBy(d => d.Deliveryorder).ToList();



                    List<tblLoadDelivery> LoadDelivery = new List<tblLoadDelivery>();

                    foreach (var dt in DeliveryQuery)

                    //foreach (DataRow dr in dt.Rows)

                    {

                        LoadDelivery.Add(new tblLoadDelivery
                        {


                            PickUpId = dt.PickUpId,
                            Information = dt.Information,
                            ShipperName = dt.ShipperName,
                            Address = dt.ShipperAddress,
                            PhoneNumber = dt.PhoneNumber,
                            ZipCode = dt.ZipCode,
                            StateCode = dt.StateCode,
                            CityName = dt.CityName.ToString(),
                            CountryName = dt.CountryName.ToString(),
                            DateTimeFrom = Convert.ToDateTime(dt.DateTimeFrom),
                            DateTimeTo = Convert.ToDateTime(dt.DateTimeTo),
                            Traitor = dt.Traitor,
                            PickupNumber = dt.PickupNumber,
                            Comments = dt.Comments,
                            MaxDeliveryID = Convert.ToInt32(dt.DeliveryId),
                            //DeliveryId = 0,
                            DeliveryId = dt.DeliveryId,
                            ShipperId = dt.ShipperId,
                            IsSave = 1,
                            Longitude = dt.Longitude,
                            Latitude = dt.Latitude,
                            NewDateFrom = dt.DateTimeFrom.ToString("MM-dd-yyyy HH:mm"),
                            NewDateTo = dt.DateTimeTo.ToString("MM-dd-yyyy HH:mm"),
                            Deliveryorder = dt.Deliveryorder,
                            //PickUpId = Convert.ToInt32(dr["PickUpId"]),
                            //Information = (dr["Information"]).ToString(),
                            //ShipperName = (dr["ShipperName"]).ToString(),
                            //Address = (dr["Address"]).ToString(),
                            //PhoneNumber = (dr["PhoneNumber"]).ToString(),
                            //ZipCode = Convert.ToInt32(dr["ZipCode"]),
                            //StateCode = (dr["StateCode"]).ToString(),
                            //CityName = (dr["CityName"]).ToString(),
                            //CountryName = (dr["CountryName"]).ToString(),
                            //DateTimeFrom = Convert.ToDateTime(dr["DateTimeFrom"]),
                            //DateTimeTo = Convert.ToDateTime(dr["DateTimeTo"]),
                            //Traitor = (dr["Traitor"]).ToString(),
                            //PickupNumber = (dr["PickupNumber"]).ToString(),
                            //Comments = (dr["Comments"]).ToString(),
                            //MaxDeliveryID = Convert.ToInt32(DeliveryId),
                            //DeliveryId = Convert.ToInt32(dr["DeliveryID"]),
                            //ShipperId = Convert.ToInt32(dr["ShipperId"]),
                            //IsSave = 2,
                            //Longitude = (dr["Longitude"]).ToString(),
                            //Latitude = (dr["Latitude"]).ToString(),
                        });

                    }




                    //qry = " select 'United States' as CountryName,  SD.ZipCode,SD.CityName,SD.StateCode, TLP.PickUpId, LoadNumber,S.ShipperId, ";
                    //qry += " ISNULL(S.Longitude,'0') as Longitude, ISNULL(S. Latitude,'0') as  Latitude , S.ShipperName, CountryId, S.ShipperPhone as PhoneNumber, Address, DateTimeFrom,";
                    //qry += " DateTimeTo, PickupNumber, Traitor, Comments, CreatedBy, CreatedDate, IsSave";
                    //qry += ",S.ShipperName + ' , ' + Address + ' , ' + SD.CityName +' , ' + SD.StateCode + ' , ' + CONVERT(varchar(10),DateTimeFrom,103) AS Information";
                    //qry += " , TLP.DeliveryId from tblLoadDelivery TLP inner join tblStateCityData SD ON SD.ZipCode = TLP.ZipCode ";
                    //qry += " inner join tblShipper S on S.ShipperId = TLP.ShipperId ";
                    //qry += " Where TLP.LoadNumber ='" + LoaderNumber + "' ";
                    //dt = ut.GetDatatable(qry);

                    //List<tblLoadDelivery> LoadDelivery = new List<tblLoadDelivery>();

                    //foreach (DataRow dr in dt.Rows)

                    //{
                    //    LoadDelivery.Add(new tblLoadDelivery
                    //    {
                    //        DeliveryId = Convert.ToInt32(dr["DeliveryId"]),
                    //        PickUpId = Convert.ToInt32(dr["PickUpId"]),
                    //        Information = (dr["Information"]).ToString(),
                    //        ShipperName = (dr["ShipperName"]).ToString(),
                    //        Address = (dr["Address"]).ToString(),
                    //        PhoneNumber = (dr["PhoneNumber"]).ToString(),
                    //        ZipCode = Convert.ToInt32(dr["ZipCode"]),
                    //        StateCode = (dr["StateCode"]).ToString(),
                    //        CityName = (dr["CityName"]).ToString(),
                    //        CountryName = (dr["CountryName"]).ToString(),
                    //        DateTimeFrom = Convert.ToDateTime(dr["DateTimeFrom"]),
                    //        DateTimeTo = Convert.ToDateTime(dr["DateTimeTo"]),
                    //        Traitor = (dr["Traitor"]).ToString(),
                    //        PickupNumber = (dr["PickupNumber"]).ToString(),
                    //        Comments = (dr["Comments"]).ToString(),
                    //        MaxDeliveryID = Convert.ToInt32(DeliveryId),
                    //        ShipperId = Convert.ToInt32(dr["ShipperId"]),
                    //        IsSave = 1,
                    //        Longitude = (dr["Longitude"]).ToString(),
                    //        Latitude = (dr["Latitude"]).ToString(),
                    //    });

                    //}
                    return Json(LoadDelivery, JsonRequestBehavior.AllowGet);

                }
                //return Json("Test");
            }
            else
            {
                using (DieseltechEntities entities = new DieseltechEntities())
                {


                    //Loop and insert records.
                    foreach (tblLoadDelivery Delivery in LoadDeliveryList)
                    {
                        //Pickup.LoadNumber = "2010028";
                        //Pickup.CountryId = 1;
                        //Pickup.CountryName = "United States";
                        //Pickup.CreatedBy = Convert.ToInt32(Session["User_id"]);
                        //Pickup.IsSave = 0;
                        //Pickup.ShipperId = 0;
                        //Pickup.StateCode = "";
                        //Pickup.CreatedDate = DateTime.Now;


                        string ShipperAssignID = "";
                        if (Delivery.ShipperId == 0)
                        {
                            ShipperAssignID = ut.ExecuteScalar("Exec SpGetShipperNumber");
                        }

                        //Insert or Update Shipper
                        qry = "Exec Sp_InsertUpdate_Shipper " + Delivery.ShipperId + ",'" + Delivery.ShipperName + "','" + Delivery.PhoneNumber + "','" + Delivery.Address + "' ,";
                        qry += " '" + Delivery.CityName + "' , '" + Delivery.StateCode + "', '" + Delivery.StateCode + "','" + Delivery.ZipCode + "' ,'" + ShipperAssignID + "'";
                        qry += ", '" + Delivery.Longitude + "' , '" + Delivery.Latitude + "' ";
                        ut.InsertUpdate(qry);



                        Delivery.DeliveryId = txtDeliveryId;
                        qry = " Exec  Sp_Insert_Update_LoadDelivery  '" + LoaderNumber + "' , '" + Delivery.Information + "'," + Delivery.ShipperId + ", ";
                        qry += " '" + Delivery.ShipperName + "' ,1,'" + Delivery.CountryName + "','" + Delivery.PhoneNumber + "' ";
                        qry += " ,'" + Delivery.Address + "' , '" + Delivery.ZipCode + "','', '" + Delivery.CityName + "','" + Delivery.DateTimeFrom.ToString("yyyy-MM-dd HH:mm") + "' ";
                        qry += ", '" + Delivery.DateTimeTo.ToString("yyyy-MM-dd HH:mm") + "' , '" + Delivery.PickupNumber + "', '" + Delivery.Traitor + "','" + Delivery.Comments + "' ";
                        qry += " ," + Convert.ToInt32(Session["User_id"]) + ",0, " + Delivery.PickUpId + " ,'" + Delivery.Longitude + "' ,'" + Delivery.Latitude + "' ," + Delivery.DeliveryId + "," + Delivery.Deliveryorder + ",'U'   ";
                        ut.InsertUpdate(qry);


                    }


                    qry = "select max(DeliveryId) from tblLoadDelivery Where  LoadNumber ='" + LoaderNumber + "' ";
                    string DeliveryId = ut.ExecuteScalar(qry);
                    //ViewBag.LastPickupId = PickupId;


                    var DeliveryQuery = (from TLP in deEntity.tblLoadDeliveries
                                         join S in deEntity.tblShippers on TLP.ShipperId equals S.ShipperId
                                         join SD in deEntity.tblStateCityDatas on TLP.ZipCode equals SD.ZipCode
                                         where TLP.LoadNumber == LoaderNumber
                                         select new
                                         {
                                             TLP.PickUpId,
                                             TLP.DeliveryId,
                                             CountryName = "United States",
                                             SD.ZipCode,
                                             SD.CityName,
                                             SD.StateCode,
                                             TLP.LoadNumber,
                                             S.ShipperId,
                                             S.Longitude,
                                             S.Latitude,
                                             S.ShipperName,
                                             PhoneNumber = S.ShipperPhone,
                                             S.ShipperAddress,
                                             TLP.DateTimeFrom,
                                             TLP.DateTimeTo,
                                             TLP.PickupNumber,
                                             TLP.Traitor,
                                             TLP.Comments,
                                             TLP.CreatedBy,
                                             TLP.CreatedDate,
                                             TLP.IsSave,
                                             TLP.Deliveryorder,
                                             Information = S.ShipperName + " , " + S.ShipperAddress + " , " + SD.CityName + " , " + SD.StateCode
                                             //+ " ," + Convert.ToString(TLP.DateTimeFrom)  
                                             //}).OrderBy(d => d.DeliveryId).ToList();
                                         }).OrderBy(d => d.Deliveryorder).ToList();



                    List<tblLoadDelivery> LoadDelivery = new List<tblLoadDelivery>();

                    foreach (var dt in DeliveryQuery)

                    //foreach (DataRow dr in dt.Rows)

                    {

                        LoadDelivery.Add(new tblLoadDelivery
                        {


                            PickUpId = dt.PickUpId,
                            Information = dt.Information,
                            ShipperName = dt.ShipperName,
                            Address = dt.ShipperAddress,
                            PhoneNumber = dt.PhoneNumber,
                            ZipCode = dt.ZipCode,
                            StateCode = dt.StateCode,
                            CityName = dt.CityName.ToString(),
                            CountryName = dt.CountryName.ToString(),
                            DateTimeFrom = Convert.ToDateTime(dt.DateTimeFrom),
                            DateTimeTo = Convert.ToDateTime(dt.DateTimeTo),
                            Traitor = dt.Traitor,
                            PickupNumber = dt.PickupNumber,
                            Comments = dt.Comments,
                            MaxDeliveryID = Convert.ToInt32(dt.DeliveryId),
                            //DeliveryId = 0,
                            DeliveryId = dt.DeliveryId,
                            ShipperId = dt.ShipperId,
                            IsSave = 2,
                            Longitude = dt.Longitude,
                            Latitude = dt.Latitude,
                            NewDateFrom = dt.DateTimeFrom.ToString("MM-dd-yyyy HH:mm"),
                            NewDateTo = dt.DateTimeTo.ToString("MM-dd-yyyy HH:mm"),
                            Deliveryorder = dt.Deliveryorder,
                            //PickUpId = Convert.ToInt32(dr["PickUpId"]),
                            //Information = (dr["Information"]).ToString(),
                            //ShipperName = (dr["ShipperName"]).ToString(),
                            //Address = (dr["Address"]).ToString(),
                            //PhoneNumber = (dr["PhoneNumber"]).ToString(),
                            //ZipCode = Convert.ToInt32(dr["ZipCode"]),
                            //StateCode = (dr["StateCode"]).ToString(),
                            //CityName = (dr["CityName"]).ToString(),
                            //CountryName = (dr["CountryName"]).ToString(),
                            //DateTimeFrom = Convert.ToDateTime(dr["DateTimeFrom"]),
                            //DateTimeTo = Convert.ToDateTime(dr["DateTimeTo"]),
                            //Traitor = (dr["Traitor"]).ToString(),
                            //PickupNumber = (dr["PickupNumber"]).ToString(),
                            //Comments = (dr["Comments"]).ToString(),
                            //MaxDeliveryID = Convert.ToInt32(DeliveryId),
                            //DeliveryId = Convert.ToInt32(dr["DeliveryID"]),
                            //ShipperId = Convert.ToInt32(dr["ShipperId"]),
                            //IsSave = 2,
                            //Longitude = (dr["Longitude"]).ToString(),
                            //Latitude = (dr["Latitude"]).ToString(),
                        });

                    }



                    //qry = " select  PickUpId, DeliveryId ,'United States' as CountryName,  SD.ZipCode,SD.CityName,SD.StateCode, TLP.PickUpId, LoadNumber, S.ShipperId, ";
                    //qry += "  ISNULL(S.Longitude,'0') as Longitude, ISNULL(S.Latitude,'0') as  Latitude,S.ShipperName, CountryId, S.ShipperPhone as PhoneNumber, Address, DateTimeFrom,";
                    //qry += " DateTimeTo, PickupNumber, Traitor, Comments, CreatedBy, CreatedDate, IsSave";
                    //qry += ",S.ShipperName + ' , ' + Address + ' , ' + SD.CityName +' , ' + SD.StateCode + ' , ' + CONVERT(varchar(10),DateTimeFrom,103) AS Information";
                    //qry += " from tblLoadDelivery TLP inner join tblStateCityData SD ON SD.ZipCode = TLP.ZipCode ";
                    //qry += " inner join tblShipper S on S.ShipperId = TLP.ShipperId ";
                    //qry += " Where TLP.LoadNumber ='" + LoaderNumber + "' ";
                    //dt = ut.GetDatatable(qry);

                    //List<tblLoadDelivery> LoadDelivery = new List<tblLoadDelivery>();

                    //foreach (DataRow dr in dt.Rows)

                    //{

                    //    LoadDelivery.Add(new tblLoadDelivery
                    //    {
                    //        PickUpId = Convert.ToInt32(dr["PickUpId"]),
                    //        Information = (dr["Information"]).ToString(),
                    //        ShipperName = (dr["ShipperName"]).ToString(),
                    //        Address = (dr["Address"]).ToString(),
                    //        PhoneNumber = (dr["PhoneNumber"]).ToString(),
                    //        ZipCode = Convert.ToInt32(dr["ZipCode"]),
                    //        StateCode = (dr["StateCode"]).ToString(),
                    //        CityName = (dr["CityName"]).ToString(),
                    //        CountryName = (dr["CountryName"]).ToString(),
                    //        DateTimeFrom = Convert.ToDateTime(dr["DateTimeFrom"]),
                    //        DateTimeTo = Convert.ToDateTime(dr["DateTimeTo"]),
                    //        Traitor = (dr["Traitor"]).ToString(),
                    //        PickupNumber = (dr["PickupNumber"]).ToString(),
                    //        Comments = (dr["Comments"]).ToString(),
                    //        MaxDeliveryID = Convert.ToInt32(DeliveryId),
                    //        DeliveryId = Convert.ToInt32(dr["DeliveryID"]),
                    //        ShipperId = Convert.ToInt32(dr["ShipperId"]),
                    //        IsSave = 2,
                    //        Longitude = (dr["Longitude"]).ToString(),
                    //        Latitude = (dr["Latitude"]).ToString(),
                    //    });

                    //}



                    return Json(LoadDelivery, JsonRequestBehavior.AllowGet);

                }
                //return Json("", JsonRequestBehavior.AllowGet);
            }


        }






        [Customexception]
        public JsonResult InsertDelivery(string LoaderNumber, List<tblLoadPickupDelivery> delivery)
        {


            string LoaderNumbers = LoaderNumber;


            using (DieseltechEntities entities = new DieseltechEntities())
            {
                //int VCardid = Convert.ToInt32(Session["VCardis"]);
                //entities.Database.ExecuteSqlCommand("Delete from tblEducation where VCardID=" + VCardid);

                if (delivery == null)
                {
                    delivery = new List<tblLoadPickupDelivery>();
                }




                //Loop and insert records.
                foreach (tblLoadPickupDelivery customer in delivery)
                {
                    customer.LoaderNumber = LoaderNumber;
                    entities.tblLoadPickupDeliveries.Add(customer);
                }
                int insertedRecords = entities.SaveChanges();
                return Json(insertedRecords);
            }
        }
        [Customexception]

        //Update Pickup and Delivery
        public JsonResult UpdateDelivery(string LoaderNumber, List<tblLoadPickupDelivery> delivery)
        {

            using (DieseltechEntities entities = new DieseltechEntities())
            {
                //int VCardid = Convert.ToInt32(Session["VCardis"]);
                //entities.Database.ExecuteSqlCommand("Delete from tblEducation where VCardID=" + VCardid);

                qry = "delete tblLoadPickupDelivery Where LoaderNumber ='" + LoaderNumber + "' ";
                string result = ut.InsertUpdate(qry);
                if (delivery == null)
                {
                    delivery = new List<tblLoadPickupDelivery>();
                }

                //Loop and insert records.
                foreach (tblLoadPickupDelivery customer in delivery)
                {
                    customer.LoaderNumber = LoaderNumber;

                    entities.tblLoadPickupDeliveries.Add(customer);
                }
                int insertedRecords = entities.SaveChanges();
                return Json(insertedRecords);
            }
        }

        [Customexception]
        public JsonResult InsertCharges(string LoaderNumber, List<tblLoadCharge1> charges)
        {
            using (DieseltechEntities entities = new DieseltechEntities())
            {
                //int VCardid = Convert.ToInt32(Session["VCardis"]);
                //entities.Database.ExecuteSqlCommand("Delete from tblEducation where VCardID=" + VCardid);

                if (charges == null)
                {
                    charges = new List<tblLoadCharge1>();
                }

                //Loop and insert records.
                foreach (tblLoadCharge1 charge in charges)
                {
                    charge.LoaderNumber = LoaderNumber;

                    entities.tblLoadCharges1.Add(charge);
                }
                int insertedRecords = entities.SaveChanges();
                return Json(insertedRecords);
            }
        }

        [Customexception]
        public JsonResult UpdateCharges(string LoaderNumber, List<tblLoadCharge1> charges)
        {
            using (DieseltechEntities entities = new DieseltechEntities())
            {

                qry = "delete tblLoadCharges Where LoaderNumber ='" + LoaderNumber + "' ";
                string result = ut.InsertUpdate(qry);
                //int VCardid = Convert.ToInt32(Session["VCardis"]);
                //entities.Database.ExecuteSqlCommand("Delete from tblEducation where VCardID=" + VCardid);

                if (charges == null)
                {
                    charges = new List<tblLoadCharge1>();
                }

                //Loop and insert records.
                foreach (tblLoadCharge1 charge in charges)
                {
                    charge.LoaderNumber = LoaderNumber;

                    entities.tblLoadCharges1.Add(charge);
                }
                int insertedRecords = entities.SaveChanges();
                return Json(insertedRecords);
            }
        }


        [Customexception]
        public JsonResult InsertCustomersDemo(List<tblLoadCharge1> customers)
        {

            int insertedRecords = 0;
            return Json(insertedRecords);

        }

        [HttpPost]
        [Customexception]
        public ActionResult GetCities(string StateId)
        {
            List<SelectListItem> City = new List<SelectListItem>();
            var ques = deEntity.tblCities.SqlQuery("select * from tblCity  where stateID  =  " + Convert.ToInt32(StateId)).ToList();
            City = new ModelHelper().ToSelectCityItem(ques);
            JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
            string result = javaScriptSerializer.Serialize(City);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [Customexception]
        public ActionResult DeleteLoadData()
        {
            string qry = " Exec SpGetLoadNumber ";
            string LoaderName = ut.ExecuteScalar(qry);

            qry = "Exec Sp_Delete_Load_Data'" + LoaderName + "'";
            ut.InsertUpdate(qry);



            return Json("Form Reload");
        }

        [Customexception]
        //Upload Files 
        public ActionResult UploadFiles(string LoaderName)
        {

            string qry = " Exec SpGetLoadNumber ";
            LoaderName = ut.ExecuteScalar(qry);
            //string LoaderNumbers = TempData["CurrentLoad"].ToString();

            string LoaderNumbers = LoaderName;

            // Checking no of files injected in Request object
            if (Request.Files.Count > 0)
            {
                try
                {
                    // Get all files from Request object
                    HttpFileCollectionBase files = Request.Files;
                    for (int i = 0; i < files.Count; i++)
                    {
                        //string path = AppDomain.CurrentDomain.BaseDirectory + “Uploads/”;
                        //string filename = Path.GetFileName(Request.Files[i].FileName);
                        HttpPostedFileBase file = files[i];
                        string fname;
                        // Checking for Internet Explorer
                        if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
                        {
                            string[] testfiles = file.FileName.Split(new char[] { '\\' });
                            fname = testfiles[testfiles.Length - 1];
                            fname = file.FileName;
                        }
                        else
                        {
                            fname = file.FileName;

                        }
                        // Get the complete folder path and store the file inside it.
                        var FolderPath = Server.MapPath("/Uploads/Loads/LoaderNumbers/" + LoaderNumbers + "/");

                        if (!Directory.Exists(FolderPath))
                        {
                            // Try to create the directory.
                            DirectoryInfo di = Directory.CreateDirectory(FolderPath);
                        }
                        FolderPath = "/Uploads/Loads/LoaderNumbers/" + LoaderNumbers + "/" + fname + "";

                        qry = "Exec Sp_InsertUpdate_FilePath_Temp '" + LoaderNumbers + "' , '" + FolderPath + "' ,  '" + fname + "' ";
                        string result = ut.InsertUpdate(qry);

                        FolderPath = "/Uploads/Loads/LoaderNumbers/" + LoaderNumbers + "/";

                        fname = Path.Combine(Server.MapPath(FolderPath), fname);
                        file.SaveAs(fname);

                        //Return File to Load Controller 
                        qry = "select * from tblLoadFilePathTemp Where LoaderNumber ='" + LoaderNumbers + "' ";

                        dt = ut.GetDatatable(qry);

                        List<tblLoadFilePathTemp> TemporaryUploadedFiles = new List<tblLoadFilePathTemp>();

                        foreach (DataRow dr in dt.Rows)

                        {

                            TemporaryUploadedFiles.Add(new tblLoadFilePathTemp
                            {
                                FilePathId = Convert.ToInt32(dr["FilePathId"]),
                                LoaderNumber = dr["LoaderNumber"].ToString(),
                                ImagePath = dr["ImagePath"].ToString(),
                                FileName = dr["FileName"].ToString(),
                            });

                        }
                        return Json(TemporaryUploadedFiles, JsonRequestBehavior.AllowGet);



                    }
                    // Returns message that successfully uploaded
                    return Json("File Uploaded Successfully!");
                }
                catch (Exception ex)
                {
                    return Json("Error occurred.Error details: " + ex.Message);
                }
            }
            else
            {
                return Json("1");
            }
        }
        [Customexception]
        public ActionResult DeleteDocument(Int32 FilePathId)
        {

            string qry = " Delete tblLoadFilePathTemp Where FilePathId =" + FilePathId;
            ut.InsertUpdate(qry);

            // Checking no of files injected in Request object
            qry = " Exec SpGetLoadNumber ";
            string LoaderName = ut.ExecuteScalar(qry);
            //Return File to Load Controller 
            qry = "select * from tblLoadFilePathTemp Where LoaderNumber ='" + LoaderName + "' ";

            dt = ut.GetDatatable(qry);

            List<tblLoadFilePathTemp> TemporaryUploadedFiles = new List<tblLoadFilePathTemp>();

            foreach (DataRow dr in dt.Rows)

            {

                TemporaryUploadedFiles.Add(new tblLoadFilePathTemp
                {
                    FilePathId = Convert.ToInt32(dr["FilePathId"]),
                    LoaderNumber = dr["LoaderNumber"].ToString(),
                    ImagePath = dr["ImagePath"].ToString(),
                    FileName = dr["FileName"].ToString(),
                });

            }
            return Json(TemporaryUploadedFiles, JsonRequestBehavior.AllowGet);

        }

        [Customexception]
        public JsonResult GetCarrierTruckList(string CarrierAssignId)

        {

            //DataSet ds = ut.GetTruckList(CarrierAssignId);
            string qry = "Exec Sp_Search_Autocomplete_TruckInfo '" + CarrierAssignId + "'";
            DataTable dt = new DataTable();
            dt = ut.GetDatatable(qry);

            //List<tblCarrier> searchlist = new List<tblCarrier>();

            List<Sp_Search_Autocomplete_TruckInfo_Result> CarrierTruckList = new List<Sp_Search_Autocomplete_TruckInfo_Result>();

            foreach (DataRow dr in dt.Rows)

            {

                CarrierTruckList.Add(new Sp_Search_Autocomplete_TruckInfo_Result
                {
                    //, MC#,AssignID, ContactName, Phonenumber
                    CarrierName = dr["CarrierName"].ToString(),
                    AssignID = dr["AssignID"].ToString(),
                    ContactName = dr["ContactName"].ToString(),
                    Phonenumber = dr["Phonenumber"].ToString(),
                    LoadTypeName = dr["LoadTypeName"].ToString(),
                    CityName = dr["CityName"].ToString(),
                    StateCode = dr["StateCode"].ToString(),
                    DriverName = dr["DriverName"].ToString(),
                    TrailerNo = dr["TrailerNo"].ToString(),
                    TruckNo = dr["TruckNo"].ToString(),
                    Phone = dr["DriverPhone"].ToString(),
                    Truckid = Convert.ToInt32(dr["Truckid"]),
                    IsBlackList = Convert.ToInt32(dr["IsBlackList"]),
                    Validcarrier = Convert.ToInt32(dr["Validcarrier"]),
                    //DriverId = Convert.ToInt32(dr["DriverId"]),



                });

            }

            return Json(CarrierTruckList, JsonRequestBehavior.AllowGet);

        }




        [Customexception]
        //Upload Files 
        public ActionResult UploadBrokerConfirmationFiles(string LoaderName)
        {
            // Checking no of files injected in Request object
            if (Request.Files.Count > 0)
            {
                try
                {
                    // Get all files from Request object
                    HttpFileCollectionBase files = Request.Files;
                    for (int i = 0; i < files.Count; i++)
                    {
                        //string path = AppDomain.CurrentDomain.BaseDirectory + “Uploads/”;
                        //string filename = Path.GetFileName(Request.Files[i].FileName);
                        HttpPostedFileBase file = files[i];
                        string fname;
                        // Checking for Internet Explorer
                        if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
                        {
                            string[] testfiles = file.FileName.Split(new char[] { '\\' });
                            fname = testfiles[testfiles.Length - 1];
                            fname = file.FileName;
                        }
                        else
                        {
                            fname = file.FileName;

                        }
                        // Get the complete folder path and store the file inside it.
                        var FolderPath = Server.MapPath("/Uploads/Loads/LoaderNumbers/BrokerConfirmation/" + LoaderName + "/");

                        if (!Directory.Exists(FolderPath))
                        {
                            // Try to create the directory.
                            DirectoryInfo di = Directory.CreateDirectory(FolderPath);
                        }
                        FolderPath = "/Uploads/Loads/LoaderNumbers/BrokerConfirmation/" + LoaderName + "/" + fname + "";

                        qry = "Exec Sp_InsertUpdate_Load_ConfirmationDocument '" + LoaderName + "' , '" + FolderPath + "' ,  '" + fname + "' ,'Broker'";
                        string result = ut.InsertUpdate(qry);

                        FolderPath = "/Uploads/Loads/LoaderNumbers/BrokerConfirmation/" + LoaderName + "/";

                        fname = Path.Combine(Server.MapPath(FolderPath), fname);
                        file.SaveAs(fname);

                        //Return File to Load Controller 
                        qry = "select * from tblLoadConfirmationDocument Where LoadNumber ='" + LoaderName + "' and DocumentType='Broker' ";

                        dt = ut.GetDatatable(qry);

                        List<tblLoadConfirmationDocument> ConfirmationUploadedFiles = new List<tblLoadConfirmationDocument>();

                        foreach (DataRow dr in dt.Rows)

                        {

                            ConfirmationUploadedFiles.Add(new tblLoadConfirmationDocument
                            {
                                DocumentId = Convert.ToInt32(dr["DocumentId"]),
                                LoadNumber = dr["LoadNumber"].ToString(),
                                FilePath = dr["FilePath"].ToString(),
                                FileName = dr["FileName"].ToString(),
                                DocumentType = dr["DocumentType"].ToString(),
                            });

                        }
                        return Json(ConfirmationUploadedFiles, JsonRequestBehavior.AllowGet);



                    }
                    // Returns message that successfully uploaded
                    return Json("File Uploaded Successfully!");
                }
                catch (Exception ex)
                {
                    return Json("Error occurred.Error details: " + ex.Message);
                }
            }
            else
            {
                return Json("1");
            }
        }
        [Customexception]
        public ActionResult DeleteBrokerDocument(Int32 FilePathId, string LoadNumber)
        {

            string qry = "select FilePath from tblLoadConfirmationDocument  Where DocumentId =" + FilePathId;
            string filepath = ut.ExecuteScalar(qry);

            string fullPath = Request.MapPath("~" + filepath);
            if (System.IO.File.Exists(fullPath))
            {
                System.IO.File.Delete(fullPath);
            }

            qry = " Delete tblLoadConfirmationDocument Where DocumentId =" + FilePathId;
            ut.InsertUpdate(qry);

            // Checking no of files injected in Request object
            qry = " Exec SpGetLoadNumber ";
            string LoaderName = ut.ExecuteScalar(qry);
            //Return File to Load Controller 
            qry = "select * from tblLoadConfirmationDocument Where LoadNumber ='" + LoadNumber + "' and DocumentType='Broker' ";

            dt = ut.GetDatatable(qry);

            List<tblLoadConfirmationDocument> ConfirmationUploadedFiles = new List<tblLoadConfirmationDocument>();

            foreach (DataRow dr in dt.Rows)

            {

                ConfirmationUploadedFiles.Add(new tblLoadConfirmationDocument
                {
                    DocumentId = Convert.ToInt32(dr["DocumentId"]),
                    LoadNumber = dr["LoadNumber"].ToString(),
                    FilePath = dr["FilePath"].ToString(),
                    FileName = dr["FileName"].ToString(),
                    DocumentType = dr["DocumentType"].ToString(),
                });

            }
            return Json(ConfirmationUploadedFiles, JsonRequestBehavior.AllowGet);

        }

        [Customexception]
        public ActionResult UploadCarrierConfirmationFiles(string LoaderName)
        {
            // Checking no of files injected in Request object
            if (Request.Files.Count > 0)
            {
                try
                {
                    // Get all files from Request object
                    HttpFileCollectionBase files = Request.Files;
                    for (int i = 0; i < files.Count; i++)
                    {
                        //string path = AppDomain.CurrentDomain.BaseDirectory + “Uploads/”;
                        //string filename = Path.GetFileName(Request.Files[i].FileName);
                        HttpPostedFileBase file = files[i];
                        string fname;
                        // Checking for Internet Explorer
                        if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
                        {
                            string[] testfiles = file.FileName.Split(new char[] { '\\' });
                            fname = testfiles[testfiles.Length - 1];
                            fname = file.FileName;
                        }
                        else
                        {
                            fname = file.FileName;

                        }
                        // Get the complete folder path and store the file inside it.
                        var FolderPath = Server.MapPath("/Uploads/Loads/LoaderNumbers/CarrierConfirmation/" + LoaderName + "/");

                        if (!Directory.Exists(FolderPath))
                        {
                            // Try to create the directory.
                            DirectoryInfo di = Directory.CreateDirectory(FolderPath);
                        }
                        FolderPath = "/Uploads/Loads/LoaderNumbers/CarrierConfirmation/" + LoaderName + "/" + fname + "";

                        qry = "Exec Sp_InsertUpdate_Load_ConfirmationDocument '" + LoaderName + "' , '" + FolderPath + "' ,  '" + fname + "' ,'Carrier'";
                        string result = ut.InsertUpdate(qry);

                        FolderPath = "/Uploads/Loads/LoaderNumbers/CarrierConfirmation/" + LoaderName + "/";

                        fname = Path.Combine(Server.MapPath(FolderPath), fname);
                        file.SaveAs(fname);

                        //Return File to Load Controller 
                        qry = "select * from tblLoadConfirmationDocument Where LoadNumber ='" + LoaderName + "' and DocumentType='Carrier' ";

                        dt = ut.GetDatatable(qry);

                        List<tblLoadConfirmationDocument> ConfirmationUploadedFiles = new List<tblLoadConfirmationDocument>();

                        foreach (DataRow dr in dt.Rows)

                        {

                            ConfirmationUploadedFiles.Add(new tblLoadConfirmationDocument
                            {
                                DocumentId = Convert.ToInt32(dr["DocumentId"]),
                                LoadNumber = dr["LoadNumber"].ToString(),
                                FilePath = dr["FilePath"].ToString(),
                                FileName = dr["FileName"].ToString(),
                                DocumentType = dr["DocumentType"].ToString(),
                            });

                        }
                        return Json(ConfirmationUploadedFiles, JsonRequestBehavior.AllowGet);



                    }
                    // Returns message that successfully uploaded
                    return Json("File Uploaded Successfully!");
                }
                catch (Exception ex)
                {
                    return Json("Error occurred.Error details: " + ex.Message);
                }
            }
            else
            {
                return Json("1");
            }
        }
        [Customexception]
        public ActionResult DeleteCarrierDocument(Int32 FilePathId, string LoadNumber)
        {



            string qry = "select FilePath from tblLoadConfirmationDocument  Where DocumentId =" + FilePathId;
            string filepath = ut.ExecuteScalar(qry);

            string fullPath = Request.MapPath("~" + filepath);
            if (System.IO.File.Exists(fullPath))
            {
                System.IO.File.Delete(fullPath);
            }


            qry = " Delete tblLoadConfirmationDocument Where DocumentId =" + FilePathId;
            ut.InsertUpdate(qry);


            //Return File to Load Controller 
            qry = "select * from tblLoadConfirmationDocument Where LoadNumber ='" + LoadNumber + "' and DocumentType='Carrier' ";

            dt = ut.GetDatatable(qry);

            List<tblLoadConfirmationDocument> ConfirmationUploadedFiles = new List<tblLoadConfirmationDocument>();

            foreach (DataRow dr in dt.Rows)

            {

                ConfirmationUploadedFiles.Add(new tblLoadConfirmationDocument
                {
                    DocumentId = Convert.ToInt32(dr["DocumentId"]),
                    LoadNumber = dr["LoadNumber"].ToString(),
                    FilePath = dr["FilePath"].ToString(),
                    FileName = dr["FileName"].ToString(),
                    DocumentType = dr["DocumentType"].ToString(),
                });

            }
            return Json(ConfirmationUploadedFiles, JsonRequestBehavior.AllowGet);

        }

        [Customexception]
        public ActionResult UploadPODConfirmationFiles(string LoaderName)
        {
            // Checking no of files injected in Request object
            if (Request.Files.Count > 0)
            {
                try
                {
                    // Get all files from Request object
                    HttpFileCollectionBase files = Request.Files;
                    for (int i = 0; i < files.Count; i++)
                    {
                        //string path = AppDomain.CurrentDomain.BaseDirectory + “Uploads/”;
                        //string filename = Path.GetFileName(Request.Files[i].FileName);
                        HttpPostedFileBase file = files[i];
                        string fname;
                        // Checking for Internet Explorer
                        if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
                        {
                            string[] testfiles = file.FileName.Split(new char[] { '\\' });
                            fname = testfiles[testfiles.Length - 1];
                            fname = file.FileName;
                        }
                        else
                        {
                            fname = file.FileName;

                        }
                        // Get the complete folder path and store the file inside it.
                        var FolderPath = Server.MapPath("/Uploads/Loads/LoaderNumbers/PODConfirmation/" + LoaderName + "/");

                        if (!Directory.Exists(FolderPath))
                        {
                            // Try to create the directory.
                            DirectoryInfo di = Directory.CreateDirectory(FolderPath);
                        }

                        FolderPath = "/Uploads/Loads/LoaderNumbers/PODConfirmation/" + LoaderName + "/" + fname + "";

                        qry = "Exec Sp_InsertUpdate_Load_ConfirmationDocument '" + LoaderName + "' , '" + FolderPath + "' ,  '" + fname + "' ,'POD'";
                        string result = ut.InsertUpdate(qry);

                        FolderPath = "/Uploads/Loads/LoaderNumbers/PODConfirmation/" + LoaderName + "/";

                        fname = Path.Combine(Server.MapPath(FolderPath), fname);
                        file.SaveAs(fname);

                        //Update ispod on file upload
                        qry = "update tblloadhead set ispod = 1 Where LoaderNumber = '" + LoaderName + "'";
                        ut.InsertUpdate(qry);


                        //Return File to Load Controller 
                        qry = "select * from tblLoadConfirmationDocument Where LoadNumber ='" + LoaderName + "' and DocumentType='POD' ";

                        dt = ut.GetDatatable(qry);

                        List<tblLoadConfirmationDocument> ConfirmationUploadedFiles = new List<tblLoadConfirmationDocument>();

                        foreach (DataRow dr in dt.Rows)

                        {

                            ConfirmationUploadedFiles.Add(new tblLoadConfirmationDocument
                            {
                                DocumentId = Convert.ToInt32(dr["DocumentId"]),
                                LoadNumber = dr["LoadNumber"].ToString(),
                                FilePath = dr["FilePath"].ToString(),
                                FileName = dr["FileName"].ToString(),
                                DocumentType = dr["DocumentType"].ToString(),
                            });

                        }


                        var loadcompany = (from load in deEntity.tblLoadHeads
                                         where load.LoaderNumber == LoaderName
                                         select load.CompanyId).FirstOrDefault();

                        


                        tblNotification noti = new tblNotification();
                        noti.NotificationType = "P";
                        noti.NotificationTypeReference = "";
                        noti.IsRead = 0;
                        noti.CreateDate = DateTime.Now;
                        noti.NotificationReferenceName = LoaderName;
                        noti.InsuranceExpirationDate = DateTime.Now;
                        noti.CompanyId = Convert.ToInt32(loadcompany);

                        deEntity.tblNotifications.Add(noti);

                        deEntity.SaveChanges();

                        return Json(ConfirmationUploadedFiles, JsonRequestBehavior.AllowGet);




                    }
                    // Returns message that successfully uploaded
                    return Json("File Uploaded Successfully!");
                }
                catch (Exception ex)
                {
                    return Json("Error occurred.Error details: " + ex.Message);
                }
            }
            else
            {
                return Json("1");
            }
        }
        [Customexception]
        public ActionResult DeletePODDocument(Int32 FilePathId, string LoadNumber)
        {



            string qry = "select FilePath from tblLoadConfirmationDocument  Where DocumentId =" + FilePathId;
            string filepath = ut.ExecuteScalar(qry);

            string fullPath = Request.MapPath("~" + filepath);
            if (System.IO.File.Exists(fullPath))
            {
                System.IO.File.Delete(fullPath);
            }


            qry = " Delete tblLoadConfirmationDocument Where DocumentId =" + FilePathId;
            ut.InsertUpdate(qry);

            qry = " Delete tblNotification Where NotificationReferenceName = '" + LoadNumber + "'";
            ut.InsertUpdate(qry);


            //Return File to Load Controller 
            qry = "select * from tblLoadConfirmationDocument Where LoadNumber ='" + LoadNumber + "' and DocumentType='POD' ";

            dt = ut.GetDatatable(qry);


            // if no file in pod then mark pod = zero
            if (dt.Rows.Count == 0)
            {
                qry = "update tblloadhead set ispod = 0  Where LoaderNumber = '" + LoadNumber + "'";

                ut.InsertUpdate(qry);

            }

            List<tblLoadConfirmationDocument> ConfirmationUploadedFiles = new List<tblLoadConfirmationDocument>();

            foreach (DataRow dr in dt.Rows)

            {

                ConfirmationUploadedFiles.Add(new tblLoadConfirmationDocument
                {
                    DocumentId = Convert.ToInt32(dr["DocumentId"]),
                    LoadNumber = dr["LoadNumber"].ToString(),
                    FilePath = dr["FilePath"].ToString(),
                    FileName = dr["FileName"].ToString(),
                    DocumentType = dr["DocumentType"].ToString(),
                });

            }
            return Json(ConfirmationUploadedFiles, JsonRequestBehavior.AllowGet);

        }
        [Customexception]
        public ActionResult UploadLumperConfirmationFiles(string LoaderName)
        {
            // Checking no of files injected in Request object
            if (Request.Files.Count > 0)
            {
                try
                {
                    // Get all files from Request object
                    HttpFileCollectionBase files = Request.Files;
                    for (int i = 0; i < files.Count; i++)
                    {
                        //string path = AppDomain.CurrentDomain.BaseDirectory + “Uploads/”;
                        //string filename = Path.GetFileName(Request.Files[i].FileName);
                        HttpPostedFileBase file = files[i];
                        string fname;
                        // Checking for Internet Explorer
                        if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
                        {
                            string[] testfiles = file.FileName.Split(new char[] { '\\' });
                            fname = testfiles[testfiles.Length - 1];
                            fname = file.FileName;
                        }
                        else
                        {
                            fname = file.FileName;

                        }
                        // Get the complete folder path and store the file inside it.
                        var FolderPath = Server.MapPath("/Uploads/Loads/LoaderNumbers/LumperConfirmation/" + LoaderName + "/");

                        if (!Directory.Exists(FolderPath))
                        {
                            // Try to create the directory.
                            DirectoryInfo di = Directory.CreateDirectory(FolderPath);
                        }
                        FolderPath = "/Uploads/Loads/LoaderNumbers/LumperConfirmation/" + LoaderName + "/" + fname + "";

                        qry = "Exec Sp_InsertUpdate_Load_ConfirmationDocument '" + LoaderName + "' , '" + FolderPath + "' ,  '" + fname + "' ,'Lumper'";
                        string result = ut.InsertUpdate(qry);

                        FolderPath = "/Uploads/Loads/LoaderNumbers/LumperConfirmation/" + LoaderName + "/";

                        fname = Path.Combine(Server.MapPath(FolderPath), fname);
                        file.SaveAs(fname);

                        //Return File to Load Controller 
                        qry = "select * from tblLoadConfirmationDocument Where LoadNumber ='" + LoaderName + "' and DocumentType='Lumper' ";

                        dt = ut.GetDatatable(qry);

                        List<tblLoadConfirmationDocument> ConfirmationUploadedFiles = new List<tblLoadConfirmationDocument>();

                        foreach (DataRow dr in dt.Rows)

                        {

                            ConfirmationUploadedFiles.Add(new tblLoadConfirmationDocument
                            {
                                DocumentId = Convert.ToInt32(dr["DocumentId"]),
                                LoadNumber = dr["LoadNumber"].ToString(),
                                FilePath = dr["FilePath"].ToString(),
                                FileName = dr["FileName"].ToString(),
                                DocumentType = dr["DocumentType"].ToString(),
                            });

                        }
                        return Json(ConfirmationUploadedFiles, JsonRequestBehavior.AllowGet);



                    }
                    // Returns message that successfully uploaded
                    return Json("File Uploaded Successfully!");
                }
                catch (Exception ex)
                {
                    return Json("Error occurred.Error details: " + ex.Message);
                }
            }
            else
            {
                return Json("1");
            }
        }

        [Customexception]
        public ActionResult DeleteLumperDocument(Int32 FilePathId, string LoadNumber)
        {



            string qry = "select FilePath from tblLoadConfirmationDocument  Where DocumentId =" + FilePathId;
            string filepath = ut.ExecuteScalar(qry);

            string fullPath = Request.MapPath("~" + filepath);
            if (System.IO.File.Exists(fullPath))
            {
                System.IO.File.Delete(fullPath);
            }


            qry = " Delete tblLoadConfirmationDocument Where DocumentId =" + FilePathId;
            ut.InsertUpdate(qry);


            //Return File to Load Controller 
            qry = "select * from tblLoadConfirmationDocument Where LoadNumber ='" + LoadNumber + "' and DocumentType='Lumper' ";

            dt = ut.GetDatatable(qry);

            List<tblLoadConfirmationDocument> ConfirmationUploadedFiles = new List<tblLoadConfirmationDocument>();

            foreach (DataRow dr in dt.Rows)

            {

                ConfirmationUploadedFiles.Add(new tblLoadConfirmationDocument
                {
                    DocumentId = Convert.ToInt32(dr["DocumentId"]),
                    LoadNumber = dr["LoadNumber"].ToString(),
                    FilePath = dr["FilePath"].ToString(),
                    FileName = dr["FileName"].ToString(),
                    DocumentType = dr["DocumentType"].ToString(),
                });

            }
            return Json(ConfirmationUploadedFiles, JsonRequestBehavior.AllowGet);

        }

        [Customexception]

        public ActionResult CancelLoad(string LoadNumber)
        {

            //Return File to Load Controller 
            qry = "update tblloadhead set IsCancel = 1  Where LoaderNumber ='" + LoadNumber + "'  ";

            ut.InsertUpdate(qry);

            return Json("Load Cancelled", JsonRequestBehavior.AllowGet);

        }

        [Customexception]
        public ActionResult MarkCodLoad(string LoadNumber)
        {

            //Return File to Load Controller 
            qry = "update tblloadhead set IsMarkCOD = 1  Where LoaderNumber ='" + LoadNumber + "'  ";

            ut.InsertUpdate(qry);

            return Json("Load COD Marked", JsonRequestBehavior.AllowGet);

        }




        //public ActionResult SaveLoadInformation(List<tblLoadHead> LoadHeadList, List<tblLoadPickup> LoadPickupInfo, List<tblLoadCharge1> Charge)
        //{
        //    string LoaderNumber = "";
        //    try
        //    {
        //        string qry = " Exec SpGetLoadNumber ";
        //        LoaderNumber = ut.ExecuteScalar(qry);
        //        using (DieseltechEntities entities = new DieseltechEntities())
        //        {
        //            //Save Load General Tab Information
        //            if (LoadHeadList == null)
        //            {
        //                LoadHeadList = new List<tblLoadHead>();
        //            }
        //            //Loop and insert records.
        //            foreach (tblLoadHead LoadHead in LoadHeadList)
        //            {
        //                LoadHead.User_ID = Convert.ToInt32(Session["User_id"]);
        //                LoadHead.LoaderNumber = LoaderNumber;
        //                //LoadHead.Comment = "xx";
        //                entities.tblLoadHeads.Add(LoadHead);
        //            }

        //            entities.SaveChanges();
        [HttpPost]
        [Customexception]
        public ActionResult SaveShipper(List<tblShipper> ShipperList)
        {
            try
            {


                //Getting basic Data to save 
                qry = "Exec SpGetShipperNumber";
                string ShipperAssignId = ut.ExecuteScalar(qry);


                using (DieseltechEntities entities = new DieseltechEntities())
                {
                    //Save Load General Tab Information
                    if (ShipperList == null)
                    {
                        ShipperList = new List<tblShipper>();
                    }
                    //Loop and insert records.
                    foreach (tblShipper shipperdata in ShipperList)
                    {
                        shipperdata.ShipperAssignId = ShipperAssignId;

                        //entities.tblShippers.Add(shipperdata);

                        qry = "Insert into tblShipper ( ShipperName, ShipperPhone, ShipperAddress, ShipperCity, ShipperStateCode, ShipperStateName, ShipperZipCode, ShipperAssignId)";
                        qry += " values ('" + shipperdata.ShipperName + "' ,'" + shipperdata.ShipperPhone + "','" + shipperdata.ShipperAddress + "','" + shipperdata.ShipperCity + "' ";

                        qry += " ,'" + shipperdata.ShipperStateCode + "' ,'" + shipperdata.ShipperStateName + "','" + shipperdata.ShipperZipCode + "','" + shipperdata.ShipperAssignId + "' )";

                        ut.InsertUpdate(qry);
                    }


                    //entities.SaveChanges();
                }


                //Save Broker Information in table 
                //deEntity.tblShippers.Add(Shipper);
                //deEntity.SaveChanges();
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Exception Occur While Saving Shipper " + ex.Message;
            }


            //Return File to Load Controller 
            qry = "select * from tblShipper Where ShipperId = ( select Max(Shipperid) From tblShipper) ";

            dt = ut.GetDatatable(qry);

            List<tblShipper> NewShipperList = new List<tblShipper>();

            foreach (DataRow dr in dt.Rows)

            {

                NewShipperList.Add(new tblShipper
                {
                    ShipperId = Convert.ToInt32(dr["ShipperId"]),
                    ShipperName = dr["ShipperName"].ToString(),
                    ShipperPhone = dr["ShipperPhone"].ToString(),
                    ShipperAddress = dr["ShipperAddress"].ToString(),
                    ShipperCity = dr["ShipperCity"].ToString(),
                    ShipperStateCode = dr["ShipperStateCode"].ToString(),
                    ShipperStateName = dr["ShipperStateName"].ToString(),
                    ShipperZipCode =dr["ShipperZipCode"].ToString(),
                    ShipperAssignId = dr["ShipperAssignId"].ToString(),
                });

            }
            return Json(NewShipperList, JsonRequestBehavior.AllowGet);

            //return RedirectToAction("Index");

        }



        [HttpPost]
        [Customexception]
        public ActionResult CreateLoadCopy(string LoadNumber)
        {
            string qry = " Exec SpGetLoadNumber ";
            string NewLoadNumber = ut.ExecuteScalar(qry);
            string result = "";

            //Return File to Load Controller 
            qry = "Exec SP_Create_Load_Copy'" + NewLoadNumber + "','" + LoadNumber + "'  ";

            result = ut.InsertUpdate(qry);

            //return Json(result, JsonRequestBehavior.AllowGet);

            return Json(new { data = NewLoadNumber, url = Url.Action("Index", "Report") });
        }



        [HttpPost]
        [Customexception]
        public ActionResult MoveToCopyLoad(string LoadNumber)
        {
            string qry = " Exec SpGetLoadNumber ";

            string NewLoadNumber = ut.ExecuteScalar(qry);

            return Json(new { data = LoadNumber, url = Url.Action("Index", "CopyLoad", new { LoadNumber = LoadNumber }) });
        }


    }

}
