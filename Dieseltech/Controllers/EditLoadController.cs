using Dieseltech.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Migrations;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;
using System.Web.Mvc;

namespace Dieseltech.Controllers
{
    [FilterConfig.AuthorizeActionFilter]
    [HandleError]
    public class EditLoadController : Controller
    {
        private DieseltechEntities deEntity = new DieseltechEntities();
        Utility ut = new Utility();
        string qry = "";
        DataTable dt = new DataTable();
        // GET: Load
        // GET: EditLoad
        [Customexception]
        public ActionResult Index(string LoadNumber , int? ddlAgentvalue)
        {
            Int32 userid = 0;
            if (Session["ddlAgent"] != null)
            {
                ddlAgentvalue = Convert.ToInt32(Session["ddlAgent"]);
            }


            if (ddlAgentvalue == null)
            {
                userid = Convert.ToInt32(Session["User_id"]);
            }
            else
            {
                userid = ddlAgentvalue ?? 0;
            }

            if (LoadNumber != null && LoadNumber != "")

            {
                var loaduser = (from load in deEntity.tblLoadHeads
                                   where load.LoaderNumber == LoadNumber
                                select load.User_ID).FirstOrDefault();
                userid = Convert.ToInt32(loaduser);
            }
                

            ViewBag.UserId = userid;
            ViewBag.AgentList = deEntity.Sp_Get_Agents_List().ToList();

            if (LoadNumber != null && LoadNumber != "")
            {
              
                ViewBag.LoadHead = deEntity.Sp_Get_LoadHeadInformation_Edit(LoadNumber).ToList();
                ViewBag.LoadNumber = LoadNumber;
                //ViewBag.LoadHeasd = deEntity.Sp_Get_LoadHeadInformation_Edit(LoadNumber).ToList();
                //ViewBag.LoadHead = deEntity.tblLoadHeads.ToList().Where(d => d.LoaderNumber == LoaderNumber).ToList();

                //ViewBag.LoadPickup = deEntity.tblLoadPickups.ToList().Where(d => d.LoadNumber == LoadNumber).ToList();


                var query = (from TLP in deEntity.tblLoadPickups
                             join S in deEntity.tblShippers on TLP.ShipperId equals S.ShipperId
                             join SD in deEntity.tblStateCityDatas on TLP.ZipCode equals SD.ZipCode
                             where TLP.LoadNumber == LoadNumber
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
                             }).OrderBy(p=> p.Pickuporder).ToList();


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
                        Pickuporder = dt.Pickuporder,
                        NewDateFrom = dt.DateTimeFrom.ToString("MM-dd-yyyy HH:mm"),
                        NewDateTo = dt.DateTimeTo.ToString("MM-dd-yyyy HH:mm"),

                        IsSave = 1,
                    });

                }


                ViewBag.LoadPickup = LoadPickup;

                //ViewBag.LoadDelivery = deEntity.tblLoadDeliveries.ToList().Where(d => d.LoadNumber == LoadNumber).ToList();

                var DeliveryQuery = (from TLP in deEntity.tblLoadDeliveries
                                     join S in deEntity.tblShippers on TLP.ShipperId equals S.ShipperId
                                     join SD in deEntity.tblStateCityDatas on TLP.ZipCode equals SD.ZipCode
                                     where TLP.LoadNumber == LoadNumber
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
                                     }).OrderBy(p => p.Deliveryorder).ToList();




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
                        Deliveryorder = dt.Deliveryorder,
                        NewDateFrom = dt.DateTimeFrom.ToString("MM-dd-yyyy HH:mm"),
                        NewDateTo = dt.DateTimeTo.ToString("MM-dd-yyyy HH:mm"),

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

                ViewBag.LoadDelivery = LoadDelivery;



                ViewBag.LoadCharges = deEntity.tblLoadCharges1.ToList().Where(d => d.LoaderNumber == LoadNumber).ToList();

                ViewBag.LoadDocuments = deEntity.tblLoadFilePaths.ToList().Where(d => d.LoaderNumber == LoadNumber).ToList();


                //ViewBag.LoadCharges = deEntity.tblLoadCharges1.ToList();

                //ViewBag.LoadCharges = deEntity.tblLoadCharges.ToList().Where(d => d.Ch == chargeId).ToList();

                //ViewBag.LoadPickupDelivery = deEntity.tblLoadPickupDeliveries.ToList().Where(d => d.LoaderNumber == LoaderNumber).ToList();

                string qry = " select CarrierId from tblLoadHead where LoaderNumber = '" + LoadNumber +"' ";
                string CarrierId = ut.ExecuteScalar(qry);


                //Fill Broker Dropdown
                ViewBag.broker = new ModelHelper().ToSelectBrokerItem(deEntity.tblBrokers).ToList();
                //ViewBag.BrokerHelper = new ModelHelper().ToSelectBrokernHelperItem(deEntity.tblBrokerHelpers).ToList();
                ViewBag.Company = new ModelHelper().ToSelectCompanyItem(deEntity.tblCompanies).ToList();
                ViewBag.Carrier = new ModelHelper().ToSelectItemList(deEntity.tblCarriers).ToList();
           //     ViewBag.CarrierHelper = new ModelHelper().ToSelectCarrierHelperItemList(deEntity.tblCarrierHelpers).ToList();
                ViewBag.LoadType = new ModelHelper().ToSelectLoadTypeItemList(deEntity.tblLoadTypes).ToList();
                ViewBag.Truck = new ModelHelper().ToSelectTruckItemList(deEntity.tblTrucks.Where(d => d.CarrierAssignId == CarrierId).ToList()).ToList();
                ViewBag.LoadSubType = new ModelHelper().ToSelectLoadSubTypeItemList(deEntity.tblLoadSubTypes).ToList();
                ViewBag.QuantityType = new ModelHelper().ToSelectQuantityTypeItemList(deEntity.tblQuantityTypes).ToList();
                ViewBag.DriverTypes = new ModelHelper().ToSelectDriverTypeItemList(deEntity.tblDriverTypes).ToList();
                ViewBag.DriverList = new ModelHelper().ToSelectDriverItem(deEntity.tblDrivers).ToList();
                ViewBag.Shipper = new ModelHelper().ToSelectShipperItem(deEntity.tblShippers).ToList();



                return View();
            }
            else
            {
                LoadNumber = "";

                //ViewBag.LoadCharges = deEntity.tblLoadCharges1.ToList();

                ViewBag.LoadCharges = deEntity.tblLoadCharges1.ToList().Where(d => d.LoadChargeId == 0).ToList();

                ViewBag.LoadPickupDelivery = deEntity.tblLoadPickupDeliveries.ToList().Where(d => d.LoaderNumber == LoadNumber).ToList();
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

        [Customexception]
        public ActionResult UploadFiles(string LoaderName)
        {

            //string LoaderNumbers = TempData["CurrentLoad"].ToString();

         

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
                        var FolderPath = Server.MapPath("/Uploads/Loads/LoaderNumbers/" + LoaderName + "/");

                        if (!Directory.Exists(FolderPath))
                        {
                            // Try to create the directory.
                            DirectoryInfo di = Directory.CreateDirectory(FolderPath);
                        }
                        FolderPath = "/Uploads/Loads/LoaderNumbers/" + LoaderName + "/" + fname + "";

                        qry = "Exec Sp_InsertUpdate_FilePath_Temp '" + LoaderName + "' , '" + FolderPath + "' ,  '" + fname + "' ";
                        string result = ut.InsertUpdate(qry);

                        FolderPath = "/Uploads/Loads/LoaderNumbers/" + LoaderName + "/";

                        fname = Path.Combine(Server.MapPath(FolderPath), fname);
                        file.SaveAs(fname);


                        //Save Upload document to original table and delete from previous table 
                        qry = "insert into tblLoadFilePath SELECT         LoaderNumber, ImagePath, FileName FROM  tblLoadFilePathTemp    Where LoaderNumber ='" + LoaderName + "'; ";
                        qry += "delete tblLoadFilePathTemp      Where LoaderNumber ='" + LoaderName + "' ";
                        ut.InsertUpdate(qry);

                        //Return File to Load Controller 
                        qry = "select * from tblLoadFilePath Where LoaderNumber ='" + LoaderName + "' ";

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
        [HttpPost]
        [Customexception]
        public JsonResult MoveLoadPickupToDelivery(string LoadNumber,Int32 txtPickupId, int[] PickupsId, int[] DeliveriesId)
        {

            string qry = " Exec SpGetLoadNumber ";
            //string LoaderNumber = ut.ExecuteScalar(qry);

            string LoaderNumber = LoadNumber;
            using (DieseltechEntities entities = new DieseltechEntities())
            {


                qry = " Exec  Sp_Insert_Update_LoadPickup  '' , '',0, ";
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


                qry = "select DeliveryId from tblLoadDelivery where Loadnumber = '"+ LoadNumber + "'";
                dt = ut.GetDatatable(qry);

                DeliveriesId = dt.AsEnumerable().Select(d => d.Field<int>("DeliveryId")).ToArray();

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
                             }).OrderBy(p=>p.Pickuporder).ToList();


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
                                     }).OrderBy(d=> d.Deliveryorder).ToList();


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
        public JsonResult MovePickuptoDelivery(string LoadNumber,Int32 txtDeliveryId, int[] PickupsId, int[] DeliveriesId)
        {

            string qry = " Exec SpGetLoadNumber ";
            //string LoaderNumber = ut.ExecuteScalar(qry);

            string LoaderNumber = LoadNumber;
            using (DieseltechEntities entities = new DieseltechEntities())
            {


                qry = " Exec  Sp_Insert_Update_LoadDelivery  '' , '',0, ";
                qry += " '' ,1,'','' ";
                qry += " ,'' ,0,'', '','" + DateTime.Now.ToString("yyyy-MM-dd") + "' ";
                qry += ", '" + DateTime.Now.ToString("yyyy-MM-dd") + "' , '', '','' ";
                qry += " ," + Convert.ToInt32(Session["User_id"]) + ",0,0,'',''," + txtDeliveryId + ",0,'C'   ";

                //qry = " Exec  Sp_Insert_Update_LoadDelivery  '' , '',0, ";
                //qry += " '' ,1,'','' ";
                //qry += " ,'' ,0,'', '','" + DateTime.Now.ToString("yyyy-MM-dd") + "' ";
                //qry += ", '" + DateTime.Now.ToString("yyyy-MM-dd") + "' , '', '','' ";
                //qry += " ," + Convert.ToInt32(Session["User_id"]) + ",0,0," + txtDeliveryId + ",'C'   ";
                ut.InsertUpdate(qry);

                int preference = 1;



                qry = "select PickUpId from tblLoadPickup where Loadnumber = '" + LoadNumber + "'";
                dt = ut.GetDatatable(qry);

                PickupsId = dt.AsEnumerable().Select(d => d.Field<int>("PickUpId")).ToArray();



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
                             }).OrderBy(p=>p.Pickuporder).ToList();


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
                                     }).OrderBy(d=> d.Deliveryorder).ToList();


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
        [Customexception]
        public ActionResult DeleteDocument(Int32 FilePathId,string LoadNumber)
        {

            string qry = " Delete tblLoadFilePath Where FilePathId =" + FilePathId;
            ut.InsertUpdate(qry);

            // Checking no of files injected in Request object
          
            //Return File to Load Controller 
            qry = "select * from tblLoadFilePath Where LoaderNumber ='" + LoadNumber + "' ";

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
        public JsonResult InsertLoadPickup(Int32 txtPickupId, List<tblLoadPickup> LoadPickupList, string LoaderNumber)
        {


            //string qry = " Exec SpGetLoadNumber ";
            //string LoaderNumber = ut.ExecuteScalar(qry);

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

                        string ShipperAssignID = "";
                        if (Pickup.ShipperId == 0)
                        {
                            ShipperAssignID = ut.ExecuteScalar("Exec SpGetShipperNumber");
                        }


                        //Insert or Update Shipper
                        qry = "Exec Sp_InsertUpdate_Shipper " + Pickup.ShipperId + ",'" + Pickup.ShipperName + "','" + Pickup.PhoneNumber + "','" + Pickup.Address + "' ,";
                        qry += " '" + Pickup.CityName + "' , '" + Pickup.StateCode + "', '" + Pickup.StateCode + "'," + Pickup.ZipCode + " ,'" + ShipperAssignID + "'";
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
                        Pickup.IsSave = 1;
                        Pickup.StateCode = "";
                        Pickup.CreatedDate = DateTime.Now;
                        if (Pickup.PickupNumber == null)
                        {
                            Pickup.PickupNumber = " ";
                        }
                        Pickup.Traitor = "";
                        Pickup.PhoneNumber = "";
                        Pickup.ShipperId = ShipperPrimaryId;
                        entities.tblLoadPickups.Add(Pickup);
                    }


                    int insertedRecords = entities.SaveChanges();

                    qry = "select max(PickUpId) from tblLoadPickup Where  LoadNumber ='" + LoaderNumber + "' ";
                    string PickupId = ut.ExecuteScalar(qry);
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
                                 }).OrderBy(p=> p.Pickuporder).ToList();


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
                            Pickuporder = dt.Pickuporder,
                            NewDateFrom = dt.DateTimeFrom.ToString("MM-dd-yyyy HH:mm"),
                            NewDateTo = dt.DateTimeTo.ToString("MM-dd-yyyy HH:mm"),

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
                        qry += " '" + Pickup.CityName + "' , '" + Pickup.StateCode + "', '" + Pickup.StateCode + "'," + Pickup.ZipCode + " ,'" + ShipperAssignID + "'";
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
                        qry += " ,'" + Pickup.Address + "' , " + Pickup.ZipCode + ",'', '" + Pickup.CityName + "','" + Pickup.DateTimeFrom.ToString("yyyy-MM-dd HH:mm") + "' ";
                        qry += ", '" + Pickup.DateTimeTo.ToString("yyyy-MM-dd HH:mm") + "' , '" + Pickup.PickupNumber + "', '" + Pickup.Traitor + "','" + Pickup.Comments + "' ";
                        qry += " ," + Convert.ToInt32(Session["User_id"]) + ",0," + Pickup.PickUpId + ",'" + Pickup.Longitude + "','" + Pickup.Latitude + "'," + Pickup.Pickuporder + ",'U'   ";
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
                                 }).OrderBy(p => p.Pickuporder).ToList();




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
        public JsonResult InsertLoadDelivery(Int32 txtDeliveryId, List<tblLoadDelivery> LoadDeliveryList ,string LoaderNumber)
        {
            //string qry = " Exec SpGetLoadNumber ";
            //string LoaderNumber = ut.ExecuteScalar(qry);

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
                        qry += " '" + Load.CityName + "' , '" + Load.StateCode + "', '" + Load.StateCode + "'," + Load.ZipCode + " ,'" + ShipperAssignID + "'";
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
                        Load.IsSave = 1;
                        Load.StateCode = "";
                        Load.CreatedDate = DateTime.Now;
                        if (Load.PickupNumber == null)
                        {
                            Load.PickupNumber = " ";
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
                                         }).OrderBy(p=> p.Deliveryorder).ToList();



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
                        string ShipperAssignID = "";
                        if (Delivery.ShipperId == 0)
                        {
                            ShipperAssignID = ut.ExecuteScalar("Exec SpGetShipperNumber");
                        }

                        //Insert or Update Shipper
                        qry = "Exec Sp_InsertUpdate_Shipper " + Delivery.ShipperId + ",'" + Delivery.ShipperName + "','" + Delivery.PhoneNumber + "','" + Delivery.Address + "' ,";
                        qry += " '" + Delivery.CityName + "' , '" + Delivery.StateCode + "', '" + Delivery.StateCode + "'," + Delivery.ZipCode + " ,'" + ShipperAssignID + "'";
                        qry += ", '" + Delivery.Longitude + "' , '" + Delivery.Latitude + "' ";
                        ut.InsertUpdate(qry);
                        //Pickup.LoadNumber = "2010028";
                        //Pickup.CountryId = 1;
                        //Pickup.CountryName = "United States";
                        //Pickup.CreatedBy = Convert.ToInt32(Session["User_id"]);
                        //Pickup.IsSave = 0;
                        //Pickup.ShipperId = 0;
                        //Pickup.StateCode = "";
                        //Pickup.CreatedDate = DateTime.Now;
                        Delivery.DeliveryId = txtDeliveryId;
                        qry = " Exec  Sp_Insert_Update_LoadDelivery  '" + LoaderNumber + "' , '" + Delivery.Information + "'," + Delivery.ShipperId + ", ";
                        qry += " '" + Delivery.ShipperName + "' ,1,'" + Delivery.CountryName + "','" + Delivery.PhoneNumber + "' ";
                        qry += " ,'" + Delivery.Address + "' , " + Delivery.ZipCode + ",'', '" + Delivery.CityName + "','" + Delivery.DateTimeFrom.ToString("yyyy-MM-dd HH:mm") + "' ";
                        qry += ", '" + Delivery.DateTimeTo.ToString("yyyy-MM-dd HH:mm") + "' , '" + Delivery.PickupNumber + "', '" + Delivery.Traitor + "','" + Delivery.Comments + "' ";
                        qry += " ," + Convert.ToInt32(Session["User_id"]) + ",0, " + Delivery.PickUpId + " ,'" + Delivery.Longitude + "' ,'" + Delivery.Latitude + "' ," + Delivery.DeliveryId + "," + Delivery.Deliveryorder + ",'U'   ";
                        ut.InsertUpdate(qry);


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
                                         }).OrderBy(p=> p.Deliveryorder).ToList();



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
                //return Json("", JsonRequestBehavior.AllowGet);
            }


        }



        [HttpPost]
        [Customexception]
        public JsonResult DeleteLoadPickup(Int32 txtPickupId,string LoaderNumber, int[] PickupsId, int[] DeliveriesId)
        {

            //string qry = " Exec SpGetLoadNumber ";
            //string LoaderNumber = ut.ExecuteScalar(qry);
            using (DieseltechEntities entities = new DieseltechEntities())
            {


                qry = " Exec  Sp_Insert_Update_LoadPickup  '' , '',0, ";
                qry += " '' ,1,'','' ";
                qry += " ,'' ,0,'', '','" + DateTime.Now.ToString("yyyy-MM-dd") + "' ";
                qry += ", '" + DateTime.Now.ToString("yyyy-MM-dd") + "' , '', '','' ";
                qry += " ," + Convert.ToInt32(Session["User_id"]) + ",0," + txtPickupId + ",'','',0,'D'   ";


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
                             }).OrderBy(p=>p.Pickuporder).ToList();


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
                                     }).OrderBy(d=> d.Deliveryorder).ToList();


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
                        Pickuporder = 0,
                        Deliveryporder = dt.Deliveryorder,
                        Longitude = dt.Longitude,
                        Latitude = dt.Latitude,
                        NewDateFrom = dt.DateTimeFrom.ToString("MM-dd-yyyy HH:mm"),
                        NewDateTo = dt.DateTimeTo.ToString("MM-dd-yyyy HH:mm"),
                    });

                }

                return Json(LoadPickupDelivery, JsonRequestBehavior.AllowGet);

            }
        }

        [HttpPost]
        [Customexception]
        public JsonResult DeleteLoadDelivery(Int32 txtDeliveryId, string LoaderNumber, int[] DeliverysId)
        {
            //string qry = " Exec SpGetLoadNumber ";
            //string LoaderNumber = ut.ExecuteScalar(qry);
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
                                     }).OrderBy(d=>d.Deliveryorder).ToList();



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




                return Json(LoadDelivery, JsonRequestBehavior.AllowGet);

            }
        }


        //Save Load Information 
        [HttpPost]
        [Customexception]
        public ActionResult SaveLoadInformation(string LoaderNumber, tblLoadHead LoadHeadList, List<tblLoadPickup> LoadPickupInfo, List<tblLoadCharge1> Charge)
        {
            //string LoaderNumber = "";
            try
            {

                //string RegistrationDate;
                //RegistrationDate = Convert.ToDateTime (ut.ExecuteScalar(" select RegistrationDate from tblLoadHead Where LoaderNumber = '" + LoaderNumber + "'")).ToString("yyyy-MM-dd");

                var LoadHeadDetail =
                  (from c in deEntity.tblLoadHeads
                   where c.LoaderNumber == LoaderNumber
                   select c).First();


                LoadHeadDetail.CarrierId = LoadHeadList.CarrierId;
                LoadHeadDetail.TruckId = LoadHeadList.TruckId;
                LoadHeadDetail.CarrierRate = LoadHeadList.CarrierRate;
                LoadHeadDetail.LoadTypeId  = LoadHeadList.LoadTypeId;
                LoadHeadDetail.Commodity = LoadHeadList.Commodity;
                LoadHeadDetail.LoadSubTypeId = LoadHeadList.LoadSubTypeId;
                LoadHeadDetail.Available = LoadHeadList.Available;
                LoadHeadDetail.Weight = LoadHeadList.Weight;
                LoadHeadDetail.QuantityTypeId = LoadHeadList.QuantityTypeId;
                LoadHeadDetail.Quantity = LoadHeadList.Quantity;
                LoadHeadDetail.DriverTypeId = LoadHeadList.DriverTypeId;
                LoadHeadDetail.CarrierInstructions = LoadHeadList.CarrierInstructions;
                LoadHeadDetail.CompanyId = LoadHeadList.CompanyId;
                LoadHeadDetail.BrokerId = LoadHeadList.BrokerId;
                LoadHeadDetail.BrokerRate = LoadHeadList.BrokerRate;
                LoadHeadDetail.Extension = LoadHeadList.Extension;
                LoadHeadDetail.BrokerReference = LoadHeadList.BrokerReference;
                LoadHeadDetail.AgentGross = LoadHeadList.AgentGross;
                LoadHeadDetail.BrokerAmout = LoadHeadList.BrokerAmout;
                LoadHeadDetail.CarrierAmount = LoadHeadList.CarrierAmount;
                LoadHeadDetail.IsFutureLoad = LoadHeadList.IsFutureLoad;
                LoadHeadDetail.IsManagerFutureLoad = LoadHeadList.IsManagerFutureLoad;
                LoadHeadDetail.FutureLoadText = LoadHeadList.FutureLoadText;
                LoadHeadDetail.DispatcherName = LoadHeadList.DispatcherName;
                LoadHeadDetail.DispatcherPhone = LoadHeadList.DispatcherPhone;
                LoadHeadDetail.DriverName = LoadHeadList.DriverName;
                LoadHeadDetail.DriverPhone = LoadHeadList.DriverPhone;
                LoadHeadDetail.Comment = LoadHeadList.Comment;
                LoadHeadDetail.RegistrationDate = LoadHeadList.RegistrationDate;
                LoadHeadDetail.User_ID = LoadHeadList.User_ID;
                deEntity.SaveChanges();


                qry = "Exec Sp_Update_Truck_Location " + LoadHeadList.TruckId + ",'" + LoaderNumber + "'";
                ut.InsertUpdate(qry);


                //string qry = " Exec SpGetLoadNumber ";
                //LoaderNumber = ut.ExecuteScalar(qry);
                using (DieseltechEntities entities = new DieseltechEntities())
                {
                    //Save Load General Tab Information
                    //if (LoadHeadList == null)
                    //{
                    //    LoadHeadList = new List<tblLoadHead>();
                    //}
                    //qry = "Delete tblLoadHead Where LoaderNumber ='" + LoaderNumber + "'";
                    //ut.InsertUpdate(qry);

                  

                    //LoadHeadDetail.CarrierId = LoadHeadList.



                    //Loop and insert records.
                    //foreach (tblLoadHead LoadHead in LoadHeadList)
                    //{
                        //LoadHead.UserId = 0;
                        //LoadHeadDetail.User_ID = Convert.ToInt32(Session["User_id"]);
                        //LoadHeadDetail.LoaderNumber = LoaderNumber;
                        ////LoadHead.Comment = "xx";

                        //if (LoadHeadList.IsFutureLoad == null)
                        //{
                        //    LoadHeadDetail.IsFutureLoad = false;
                        //}

                        //if (LoadHead.FutureLoadText == null)
                        //{
                        //    LoadHeadDetail.FutureLoadText = " ";
                        //}

                        //entities.tblLoadHeads.Add(LoadHead);

                        //entities.tblLoadHeads.AddOrUpdate(LoadHead);
                    //}

                   
                    //entities.SaveChanges();


                    qry = "Exec Sp_Calculate_Quick_Pay -2,'" + LoaderNumber + "' ,"+ LoadHeadDetail.QuickPaypercentage + "";
                    ut.InsertUpdate(qry);

                    //qry = "update tblLoadHead  SET RegistrationDate = '"+ RegistrationDate + "' Where LoaderNumber = '" + LoaderNumber + "'";
                    //ut.InsertUpdate(qry);


                    //Save Load General Tab Information
                    if (Charge == null)
                    {
                        Charge = new List<tblLoadCharge1>();
                    }

                    qry = "delete tblLoadCharges Where  LoaderNumber = '" + LoaderNumber + "'";
                    ut.InsertUpdate(qry);

                    //Loop and insert records.
                    foreach (tblLoadCharge1 LoadCharge in Charge)
                    {
                        LoadCharge.User_ID = Convert.ToInt32(Session["User_id"]);
                        LoadCharge.LoaderNumber = LoaderNumber;
                       // entities.tblLoadCharges1.AddOrUpdate(LoadCharge);

                        entities.tblLoadCharges1.Add(LoadCharge);
                    }
                    int insertedRecords = entities.SaveChanges();

                    ////Update Pickup and Delivery status on save 
                    //qry = "Update tblLoadPickup Set IsSave = 1  Where LoadNumber ='" + LoaderNumber + "' ";
                    //qry += "Update tblLoadDelivery Set IsSave = 1  Where LoadNumber ='" + LoaderNumber + "' ";
                    //ut.InsertUpdate(qry);

                    ////Save Upload document to original table and delete from previous table 
                    //qry = "insert into tblLoadFilePath SELECT         LoaderNumber, ImagePath, FileName FROM  tblLoadFilePathTemp    Where LoaderNumber ='" + LoaderNumber + "'; ";
                    //qry += "delete tblLoadFilePathTemp      Where LoaderNumber ='" + LoaderNumber + "' ";
                    //ut.InsertUpdate(qry);




                    //return RedirectToAction("LoadConfirmation", "Load", new { LoaderNumber = LoaderNumber });

                    //return Json(LoaderNumber);

                    return Json(new { result = "Redirect", url = Url.Action("LoadConfirmation", "Load", new { LoaderNumber = LoaderNumber })});
                }



            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
            }

            //return RedirectToAction("LoadConfirmation", "Load", new { LoaderNumber = LoaderNumber });
            return Json(LoaderNumber);
        }

    }
}