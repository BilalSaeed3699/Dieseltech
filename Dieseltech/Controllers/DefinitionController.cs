using Dieseltech.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace Dieseltech.Controllers
{
    [FilterConfig.AuthorizeActionFilter]
    [HandleError]
    public class DefinitionController : Controller
    {
        private DieseltechEntities deEntity = new DieseltechEntities();
        string qry = "";
        Utility ul = new Utility();
        DataTable dt = new DataTable();
        // GET: Definition
        [Customexception]
        public ActionResult Index()
        {
            return View();
        }


        [HttpPost]
        [Customexception]
        public ActionResult GetTasks(string stateID)
        {
            List<SelectListItem> Task = new List<SelectListItem>();
            var ques = deEntity.tblCities.SqlQuery("select * from tblCity  where stateID =  " + Convert.ToInt32(stateID)).ToList();
            Task = new ModelHelper().ToSelectCityItemList(ques);
            JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
            string result = javaScriptSerializer.Serialize(Task);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Customexception]
        public ActionResult GetStates(string countryId)
        {
            List<SelectListItem> Task = new List<SelectListItem>();
            var ques = deEntity.tblStates.SqlQuery("select * from tblState  where countryId  =  " + Convert.ToInt32(countryId)).ToList();
            Task = new ModelHelper().ToSelectStateItem(ques);
            JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
            string result = javaScriptSerializer.Serialize(Task);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Customexception]
        public ActionResult GetCities(string stateId)
        {
            List<SelectListItem> Task = new List<SelectListItem>();
            var ques = deEntity.tblCities.SqlQuery("select * from tblCity  where stateId  =  " + Convert.ToInt32(stateId)).ToList();
            Task = new ModelHelper().ToSelectCityItem(ques);
            JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
            string result = javaScriptSerializer.Serialize(Task);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [Customexception]
        public ActionResult Showcarrierloadlist(string AssignId)
        {
            List<Sp_Get_Carrier_LoadList_Result> CarrierLoadList = new List<Sp_Get_Carrier_LoadList_Result>();
            try
            {
                CarrierLoadList = deEntity.Sp_Get_Carrier_LoadList(AssignId).ToList();
                return Json(CarrierLoadList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                return Json("-1", JsonRequestBehavior.AllowGet);
            }


            return Json(CarrierLoadList, JsonRequestBehavior.AllowGet);

        }

        [HttpGet]
        [Customexception]
        //Carrier Info 
        public ActionResult CarrierInfo(string CarrierAssignsId,string AllCarrier)
        {
              
            try
            {
                Session["CarrierInsuranceExpirationList"] = deEntity.tblNotifications.Where(n => n.NotificationType == "C").OrderBy(n => n.IsRead).ThenByDescending(n => n.CreateDate).ToList();
                Session["AlkaiosnotifyList"] = deEntity.tblNotifications.Where(n => n.NotificationType == "P" && n.CompanyId == 1).OrderBy(n => n.IsRead).ThenByDescending(n => n.CreateDate).ToList();
                Session["JetlinenotifyList"] = deEntity.tblNotifications.Where(n => n.NotificationType == "P" && n.CompanyId == 3).OrderBy(n => n.IsRead).ThenByDescending(n => n.CreateDate).ToList();


                if (CarrierAssignsId == null)
                {
                    //Session["CarrierAssignId"] = null;

                    if (Session["CarrierAssignId"] != null)
                    {
                        CarrierAssignsId = "";
                        //qry = "Exec SpGetCarrierNumber";
                        //string NewCarrierAssignID = ul.ExecuteScalar(qry);
                        ViewBag.CarrierAssignID = Session["CarrierAssignId"].ToString();
                        CarrierAssignsId = Session["CarrierAssignId"].ToString();

                    }
                    else
                    {
                        CarrierAssignsId = "";
                        qry = "Exec SpGetCarrierNumber";
                        string NewCarrierAssignID = ul.ExecuteScalar(qry);
                        ViewBag.CarrierAssignID = NewCarrierAssignID;
                        CarrierAssignsId = NewCarrierAssignID;

                        qry = "Exec Sp_Delete_Carrier '" + CarrierAssignsId + "'";
                        CarrierAssignsId = ul.ExecuteScalar(qry);

                    }

                }
                //First Time Load
                else if (CarrierAssignsId == "1")
                {
                    CarrierAssignsId = "0";
                    qry = "Exec SpGetCarrierNumber";
                    ViewBag.CarrierAssignID = ul.ExecuteScalar(qry);
                }
                //Reload on cancel button click
                else if (CarrierAssignsId == "0")
                {
                    CarrierAssignsId = "0";
                    qry = "Exec SpGetCarrierNumber";
                    ViewBag.CarrierAssignID = ul.ExecuteScalar(qry);
                    qry = "Exec Sp_Delete_Carrier '" + CarrierAssignsId + "'";
                    ul.ExecuteScalar(qry);
                }
                else
                {
                    ViewBag.CarrierAssignID = CarrierAssignsId;
                }


                if (AllCarrier == null)
                {
                    AllCarrier = "0";
                    ViewBag.AllCarrierDetials = deEntity.tblCarriers.ToList().Where(d => d.AssignID == AllCarrier).ToList();
                }
                else
                {
                    ViewBag.AllCarrierDetials = deEntity.tblCarriers.ToList();
                }


                ViewBag.LoadType = new ModelHelper().ToSelectLoadTypeItemList(deEntity.tblLoadTypes).ToList();
                ViewBag.Driver = new ModelHelper().ToSelectDriverItem(deEntity.tblDrivers).ToList();
                //ViewBag.TruckDetails = deEntity.tblTrucks.Where(c => c.CarrierAssignId == "0001").FirstOrDefault();



                
                int Isdeleted = (from carrierinfo in deEntity.tblCarriers
                               where carrierinfo.AssignID == CarrierAssignsId
                               select carrierinfo.Isdeleted).SingleOrDefault();

              if(Isdeleted ==0)
                {
                    ViewBag.CarrierDetials = deEntity.tblCarriers.ToList().Where(d => d.AssignID == CarrierAssignsId).ToList();
                }
               else  if (Isdeleted == 1)
                {
                    //if carrier is Deleted then show next generated carrier id
                    qry = "Exec SpGetCarrierNumber";
                    CarrierAssignsId = ul.ExecuteScalar(qry);
                    ViewBag.CarrierDetials = deEntity.tblCarriers.ToList().Where(d => d.AssignID == CarrierAssignsId && d.Isdeleted==0).FirstOrDefault();
                    ViewBag.CarrierAssignID = CarrierAssignsId;
                }
                 

                

                ViewBag.TruckDetails = deEntity.tblTrucks.ToList().Where(d => d.CarrierAssignId == CarrierAssignsId).ToList();

                ViewBag.CarrierDocuments = deEntity.tblCarrierDocuments.ToList().Where(d => d.CarrierAssignId == CarrierAssignsId).ToList();
                //ViewBag.CarrierCategory = deEntity.tblCarrierCategories.ToList();

                ViewBag.CarrierCategory = new ModelHelper().ToSelectCarrierCategory(deEntity.tblCarrierCategories).ToList();

            }
            catch (Exception ex)
            {
                ViewBag.Error = "Exception Occur While Loading Carrier Information" + ex.Message;
            }

            return View();
        }


        [Customexception]
        public JsonResult AllCarrierList()

        {
      

            string qry = "Exec Sp_Get_All_Carrier_List ";

            dt = ul.GetDatatable(qry);


            List<Sp_Get_All_Carrier_List_Result> AllCarrierTruckList = new List<Sp_Get_All_Carrier_List_Result>();

            foreach (DataRow dr in dt.Rows)

            {

                AllCarrierTruckList.Add(new Sp_Get_All_Carrier_List_Result
                {
                    //, MC#,AssignID, ContactName, Phonenumber

                    CarrierName = dr["CarrierName"].ToString(),
                    MC_ = dr["MC#"].ToString(),
                    AssignID = dr["AssignID"].ToString(),
                    ContactName = dr["ContactName"].ToString(),
                    Phonenumber = dr["Phonenumber"].ToString(),
                    Email = dr["Email"].ToString(),
                    TrailerNo = dr["TrailerNo"].ToString(),
                    TruckYard = dr["TruckYard"].ToString(),
                    ZipCode = Convert.ToInt32(dr["ZipCode"]),
                    StateCode = dr["StateCode"].ToString(),
                    DriverName = dr["DriverName"].ToString(),
                    DriverPhone = dr["DriverPhone"].ToString(),
                    CategoryName = dr["CategoryName"].ToString(),
                    Rating = Convert.ToInt32( dr["Rating"]),
                    CarrierCategoryId = Convert.ToInt32(dr["CarrierCategoryId"]),
                    isactive = Convert.ToBoolean(dr["isactive"]),
                    IsBlackList = Convert.ToInt32(dr["IsBlackList"]),




                });

            }

            

            return Json(AllCarrierTruckList, JsonRequestBehavior.AllowGet);


        }
        [Customexception]
        public JsonResult AllCarrierListWithFilter(int FilterType,int BlackList,int operatorlist)

        {
            string qry = "Exec Sp_Get_All_Carrier_List_Filter  "+ FilterType + ","+ BlackList + "," + operatorlist + " ";

            dt = ul.GetDatatable(qry);


            List<Sp_Get_All_Carrier_List_Result> AllCarrierTruckList = new List<Sp_Get_All_Carrier_List_Result>();

            foreach (DataRow dr in dt.Rows)

            {

                AllCarrierTruckList.Add(new Sp_Get_All_Carrier_List_Result
                {
                    //, MC#,AssignID, ContactName, Phonenumber

                    CarrierName = dr["CarrierName"].ToString(),
                    MC_ = dr["MC#"].ToString(),
                    AssignID = dr["AssignID"].ToString(),
                    ContactName = dr["ContactName"].ToString(),
                    Phonenumber = dr["Phonenumber"].ToString(),
                    Email = dr["Email"].ToString(),
                    TrailerNo = dr["TrailerNo"].ToString(),
                    TruckYard = dr["TruckYard"].ToString(),
                    ZipCode = Convert.ToInt32(dr["ZipCode"]),
                    StateCode = dr["StateCode"].ToString(),
                    DriverName = dr["DriverName"].ToString(),
                    DriverPhone = dr["DriverPhone"].ToString(),
                    CategoryName = dr["CategoryName"].ToString(),
                    Rating = Convert.ToInt32(dr["Rating"]),
                    CarrierCategoryId = Convert.ToInt32(dr["CarrierCategoryId"]),
                    IsBlackList = Convert.ToInt32(dr["IsBlackList"]),




                });

            }



            return Json(AllCarrierTruckList, JsonRequestBehavior.AllowGet);


        }

        [HttpGet]
        [Customexception]
        public JsonResult GetStateCity(Int64 ZipCode)
        {
            
            dt = new DataTable();
            qry = "Exec Sp_Get_StateCityList " + ZipCode+"";
            dt = ul.GetDatatable(qry);
            if(dt.Rows[0]["ZipCodeResult"].ToString()=="0")
            {
                tblStateCityData StateCityData = new tblStateCityData
                {
                    StateName = "0",
                    CityName = "0",
                    StateCode = "0",
                };
                return Json(StateCityData, JsonRequestBehavior.AllowGet);
            }
            else 
            {
                tblStateCityData StateCityData = new tblStateCityData
                {
                    StateName = dt.Rows[0]["StateName"].ToString(),
                    CityName = dt.Rows[0]["CityName"].ToString(),
                    StateCode = dt.Rows[0]["StateCode"].ToString(),
                };
                return Json(StateCityData, JsonRequestBehavior.AllowGet);
            }
        }



        [HttpGet]
        [Customexception]
        public JsonResult GetStateCityInformation(string type, string Filter)
        {

            dt = new DataTable();
            qry = "Exec Sp_Get_StateCityInformation "+ type + ","+Filter+ " ";
            dt = ul.GetDatatable(qry);
            if (dt.Rows[0]["ZipCodeResult"].ToString() == "0")
            {
                tblStateCityData StateCityData = new tblStateCityData
                {
                    StateName = "0",
                    CityName = "0",
                    StateCode = "0",
                    ZipCode=0,
                };
                return Json(StateCityData, JsonRequestBehavior.AllowGet);
            }
            else
            {
                tblStateCityData StateCityData = new tblStateCityData
                {
                    StateName = dt.Rows[0]["StateName"].ToString(),
                    CityName = dt.Rows[0]["CityName"].ToString(),
                    StateCode = dt.Rows[0]["StateCode"].ToString(),
                    ZipCode = Convert.ToInt32(dt.Rows[0]["ZipCode"].ToString()),
                };
                return Json(StateCityData, JsonRequestBehavior.AllowGet);
            }
        }



        [HttpGet]
        [Customexception]
        public JsonResult GetCarrierDrivers(string CarrierAssignId)
        {


            List<SelectListItem> Task = new List<SelectListItem>();
            qry = "select * from tblDriver Where CarrierAssignId =" + CarrierAssignId + "";
            var ques = deEntity.tblDrivers.SqlQuery(qry).ToList();
            Task = new ModelHelper().ToSelectCarrierDriversItem(ques);
            JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
            string result = javaScriptSerializer.Serialize(Task);
            return Json(result, JsonRequestBehavior.AllowGet);

            //dt = new DataTable();

            //dt = ul.GetDatatable(qry);

            //    tblDriver CarrierDriver = new tblDriver
            //    {
            //        DriverId = Convert.ToInt32(dt.Rows[0]["DriverId"]),
            //       Name = dt.Rows[0]["Name"].ToString(),
            //        Phone = dt.Rows[0]["Phone"].ToString(),
            //        Language = dt.Rows[0]["Language"].ToString(),
            //        TruckId = Convert.ToInt32(dt.Rows[0]["TruckId"]),
            //        CarrierAssignId = dt.Rows[0]["CarrierAssignId"].ToString(),
            //        CreatedBy = Convert.ToInt32(dt.Rows[0]["CreatedBy"]),
            //       CreatedDate =Convert.ToDateTime (dt.Rows[0]["CreatedDate"]),
            //        IsActive = Convert.ToBoolean (dt.Rows[0]["IsActive"]),
            //    };
            //    return Json(CarrierDriver, JsonRequestBehavior.AllowGet);

        }



        //Get New Assign ID
        [HttpGet]
        [Customexception]
        public JsonResult GetNewAssignId()
        {

            qry = "Exec SpGetCarrierNumber";
            string NewCarrierAssignID = ul.ExecuteScalar(qry);

            tblCarrier CarrierAssignId = new tblCarrier
            {
                AssignID = NewCarrierAssignID,
            };
            return Json(CarrierAssignId, JsonRequestBehavior.AllowGet);
          
        }


        //Get Driver Details
        [HttpGet]
        [Customexception]
        public JsonResult GetDriverDetail(Int32 DriverId)
        {

            dt = new DataTable();
            qry = "Exec Sp_Get_DriverDetails " + DriverId + "";
            dt = ul.GetDatatable(qry);
            if (dt.Rows[0]["DriverResult"].ToString() == "0")
            {
                DriverLicense DriverDetails = new DriverLicense
                {
                    Phone = "0",
                    Language = "0",
                    LicenseFileName ="",
                    LicenseFilePath = "",

                };
                return Json(DriverDetails, JsonRequestBehavior.AllowGet);
            }
            else
            {
                DriverLicense DriverDetails = new DriverLicense
                {
                    Phone = dt.Rows[0]["Phone"].ToString(),
                    Language = dt.Rows[0]["Language"].ToString(),
                    LicenseFileName = dt.Rows[0]["LicenseFileName"].ToString(),
                    LicenseFilePath = dt.Rows[0]["LicenseFilePath"].ToString(),
                };
                return Json(DriverDetails, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        [Customexception]
        public ActionResult SaveDriver(string DriverName, string DriverPhoneNumber, string Language,string CarrierAssignId)

            //public ActionResult SaveDriver(string DriverName, string DriverPhoneNumber, string Language, HttpPostedFileBase[] DriverLicense)
        {

            List<SelectListItem> Task = new List<SelectListItem>();


            qry = "select * from tblDriver Where CarrierAssignId =" + CarrierAssignId + "";
            var ques = deEntity.tblDrivers.SqlQuery(qry).ToList();
            Task = new ModelHelper().ToSelectCarrierDriversItem(ques);
            JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
            //string result = javaScriptSerializer.Serialize(Task);
            string result = "";


            try
            {
                qry = "Exec SP_InsertUpdate_Driver '" + DriverName + "' , '" + DriverPhoneNumber + "' , '" + Language + "','0','"+ CarrierAssignId + "'," + Session["User_id"] + ",'I' ";
                ul.InsertUpdate(qry);
                ViewBag.Driver = new ModelHelper().ToSelectDriverItem(deEntity.tblDrivers).ToList();
              string MainRoot = "";

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
                            var MCAuthorityFileName = Path.GetFileName(file.FileName);
                            MainRoot = "/Uploads/Carrier/Driver/";


                            bool exists = System.IO.Directory.Exists(Server.MapPath(MainRoot));
                            if (!exists)
                                System.IO.Directory.CreateDirectory(Server.MapPath(MainRoot));



                            var MCAuhtorityServerFilePath = Path.Combine(Server.MapPath(MainRoot) + MCAuthorityFileName);

                            MainRoot = "/Uploads/Carrier/Driver/" + MCAuthorityFileName;

                            //Save Files to path
                            file.SaveAs(MCAuhtorityServerFilePath);
                            //Save Files to Database
                            qry = "Exec Sp_Save_Driver_License '',0,1,'" + MCAuthorityFileName + "' ,'" + MainRoot + "','" + Convert.ToString(Session["User_id"]) + "','ID'";
                            ul.InsertUpdate(qry);


                        }




                        qry = "select * from tblDriver Where CarrierAssignId =" + CarrierAssignId + "";
                        ques = deEntity.tblDrivers.SqlQuery(qry).ToList();
                        Task = new ModelHelper().ToSelectCarrierDriversItem(ques);
                        result = javaScriptSerializer.Serialize(Task);
                        return Json(result, JsonRequestBehavior.AllowGet);

                        // Returns message that successfully uploaded
                        //return Json("1");
                    }
                    catch (Exception ex)
                    {
                        return Json("Error occurred.Error details: " + ex.Message);
                    }
                }
                //else
                //{
                //    return Json("1");
                //}


                //Save Driver License Document Files
                //foreach (HttpPostedFileBase License in DriverLicense)
                //{

                //    if (License != null)
                //    {


                //        var MCAuthorityFileName = Path.GetFileName(License.FileName);
                //        MainRoot = "/Uploads/Carrier/Driver/";


                //        bool exists = System.IO.Directory.Exists(Server.MapPath(MainRoot));
                //        if (!exists)
                //            System.IO.Directory.CreateDirectory(Server.MapPath(MainRoot));



                //        var MCAuhtorityServerFilePath = Path.Combine(Server.MapPath(MainRoot) + MCAuthorityFileName);

                //        MainRoot = "/Uploads/Carrier/Driver/" + MCAuthorityFileName;

                //        //Save Files to path
                //        License.SaveAs(MCAuhtorityServerFilePath);
                //        //Save Files to Database
                //        qry = "Exec Sp_Save_Driver_License '',0,1,'" + MCAuthorityFileName + "' ,'" + MainRoot + "','" + Convert.ToString(Session["User_id"]) + "','ID'";
                //        ul.InsertUpdate(qry);


                //    }
                //}
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Exception Occur While Creating Driver" + ex.Message;
            }


            //List<SelectListItem> Task = new List<SelectListItem>();
            qry = "select * from tblDriver Where CarrierAssignId =" + CarrierAssignId + "";
            ques = deEntity.tblDrivers.SqlQuery(qry).ToList();
            Task = new ModelHelper().ToSelectCarrierDriversItem(ques);
            //JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
            result = javaScriptSerializer.Serialize(Task);
            return Json(result, JsonRequestBehavior.AllowGet);


            //return Json("1");
          
        }


        [HttpPost]
        [Customexception]
        public ActionResult DeleteCarrierDocument(string CarrierAssignId, string DocumentType)
        {
            try
            {
                if(DocumentType== "MC")
                {
                    string query = "update  tblCarrierDocuments set IsMcAuthorityUploaded=0,McAuthorityFileName='',McAuthorityFilePath='' Where CarrierAssignId='" + CarrierAssignId + "'";
                    ul.InsertUpdate(query);
                }
                else  if (DocumentType == "W9")
                {
                    string query = "update  tblCarrierDocuments set IsW9Uploaded=0,W9FileName='',W9FilePath='' Where CarrierAssignId='" + CarrierAssignId + "'";
                    ul.InsertUpdate(query);
                }
                else if (DocumentType == "IN")
                {
                    string query = "update  tblCarrierDocuments set IsInsuranceUploaded=0,InsuranceFileName='',InsuranceFilePath='' Where CarrierAssignId='" + CarrierAssignId + "'";
                    ul.InsertUpdate(query);
                }
                else if (DocumentType == "NA")
                {
                    string query = "update  tblCarrierDocuments set IsNoticeOfAssignment=0,AssignmentFileName='',AssignmentFilePath='' Where CarrierAssignId='" + CarrierAssignId + "'";
                    ul.InsertUpdate(query);
                }
                else if (DocumentType == "VC")
                {
                    string query = "update  tblCarrierDocuments set IsVoidCheque=0,VoidChequeFileName='',VoidChequeFilePath='' Where CarrierAssignId='" + CarrierAssignId + "'";
                    ul.InsertUpdate(query);
                }

                return Json("1", JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                return Json("0", JsonRequestBehavior.AllowGet);

            }
            return Json("0", JsonRequestBehavior.AllowGet);
        }

        [Customexception]
        public ActionResult CarrierDocumentUpload(string CarrierAssignId,string DocumentType)
        {

            string MainRoot = "";

            //Upload Document For MC
            if (DocumentType =="MC")
            {
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

                            var MCAuthorityFileName = Path.GetFileName(file.FileName);
                            MainRoot = "/Uploads/Carrier/" + CarrierAssignId;

                            bool exists = System.IO.Directory.Exists(Server.MapPath(MainRoot));
                            if (!exists)
                                System.IO.Directory.CreateDirectory(Server.MapPath(MainRoot));



                            var MCAuhtorityServerFilePath = Path.Combine(Server.MapPath(MainRoot) + "\\" + MCAuthorityFileName);

                            MainRoot = "/Uploads/Carrier/" + CarrierAssignId + "/" + MCAuthorityFileName;

                            //Save Files to path
                            file.SaveAs(MCAuhtorityServerFilePath);
                            //Save Files to Database
                            qry = "Exec Sp_Save_Carrier_Documents '" + CarrierAssignId + "',1,'" + MCAuthorityFileName + "' ,'" + MainRoot + "',0,'','',0,'','',0,'','',0,'','','" + Convert.ToString(Session["User_id"]) + "','IM'";
                            ul.InsertUpdate(qry);


                            //Return File to Load Controller 
                            qry = "select * from tblCarrierDocuments Where CarrierAssignId ='" + CarrierAssignId + "' ";

                            dt = ul.GetDatatable(qry);

                            List<tblCarrierDocument> TemporaryUploadedFiles = new List<tblCarrierDocument>();

                            foreach (DataRow dr in dt.Rows)

                            {

                                TemporaryUploadedFiles.Add(new tblCarrierDocument
                                {


                                    CarrierDocumentId = Convert.ToInt32(dr["CarrierDocumentId"]),
                                    CarrierAssignId = dr["CarrierAssignId"].ToString(),
                                    IsMcAuthorityUploaded = Convert.ToInt32(dr["IsMcAuthorityUploaded"]),
                                    McAuthorityFileName = dr["McAuthorityFileName"].ToString(),
                                    McAuthorityFilePath = dr["McAuthorityFilePath"].ToString(),
                                    IsW9Uploaded = Convert.ToInt32(dr["IsW9Uploaded"]),
                                    W9FileName = dr["W9FileName"].ToString(),
                                    W9FilePath = dr["W9FilePath"].ToString(),
                                    IsInsuranceUploaded = Convert.ToInt32(dr["IsInsuranceUploaded"]),
                                    InsuranceFileName = dr["InsuranceFileName"].ToString(),
                                    InsuranceFilePath = dr["InsuranceFilePath"].ToString(),
                                    UploadedBy = (dr["UploadedBy"].ToString()),
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


            //Upload Document For W9
            if (DocumentType == "W9")
            {
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


                            var MCAuthorityFileName = Path.GetFileName(file.FileName);
                            MainRoot = "/Uploads/Carrier/" + CarrierAssignId;

                            bool exists = System.IO.Directory.Exists(Server.MapPath(MainRoot));
                            if (!exists)
                                System.IO.Directory.CreateDirectory(Server.MapPath(MainRoot));



                            var MCAuhtorityServerFilePath = Path.Combine(Server.MapPath(MainRoot) + "\\" + MCAuthorityFileName);

                            MainRoot = "/Uploads/Carrier/" + CarrierAssignId + "/" + MCAuthorityFileName;

                            //Save Files to path
                            file.SaveAs(MCAuhtorityServerFilePath);
                            //Save Files to Database
                            qry = "Exec Sp_Save_Carrier_Documents '" + CarrierAssignId + "', 0,'','' ,1,'" + MCAuthorityFileName + "' ,'" + MainRoot + "',0,'','',0,'','',0,'','','" + Convert.ToString(Session["User_id"]) + "','IW'";
                            ul.InsertUpdate(qry);

                            //Return File to Load Controller 
                            qry = "select * from tblCarrierDocuments Where CarrierAssignId ='" + CarrierAssignId + "' ";

                            dt = ul.GetDatatable(qry);

                            List<tblCarrierDocument> TemporaryUploadedFiles = new List<tblCarrierDocument>();

                            foreach (DataRow dr in dt.Rows)

                            {

                                TemporaryUploadedFiles.Add(new tblCarrierDocument
                                {


                                    CarrierDocumentId = Convert.ToInt32(dr["CarrierDocumentId"]),
                                    CarrierAssignId = dr["CarrierAssignId"].ToString(),
                                    IsMcAuthorityUploaded = Convert.ToInt32(dr["IsMcAuthorityUploaded"]),
                                    McAuthorityFileName = dr["McAuthorityFileName"].ToString(),
                                    McAuthorityFilePath = dr["McAuthorityFilePath"].ToString(),
                                    IsW9Uploaded = Convert.ToInt32(dr["IsW9Uploaded"]),
                                    W9FileName = dr["W9FileName"].ToString(),
                                    W9FilePath = dr["W9FilePath"].ToString(),
                                    IsInsuranceUploaded = Convert.ToInt32(dr["IsInsuranceUploaded"]),
                                    InsuranceFileName = dr["InsuranceFileName"].ToString(),
                                    InsuranceFilePath = dr["InsuranceFilePath"].ToString(),
                                    UploadedBy = (dr["UploadedBy"].ToString()),
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


            //Upload Document For Insurance Document
            if (DocumentType == "ID")
            {
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



                            var MCAuthorityFileName = Path.GetFileName(file.FileName);
                            MainRoot = "/Uploads/Carrier/" + CarrierAssignId;

                            bool exists = System.IO.Directory.Exists(Server.MapPath(MainRoot));
                            if (!exists)
                                System.IO.Directory.CreateDirectory(Server.MapPath(MainRoot));



                            var MCAuhtorityServerFilePath = Path.Combine(Server.MapPath(MainRoot) + "\\" + MCAuthorityFileName);

                            MainRoot = "/Uploads/Carrier/" + CarrierAssignId + "/" + MCAuthorityFileName;

                            //Save Files to path
                            file.SaveAs(MCAuhtorityServerFilePath);
                            //Save Files to Database
                            qry = "Exec Sp_Save_Carrier_Documents '" + CarrierAssignId + "',0,'','',0,'','' ,1,'" + MCAuthorityFileName + "' ,'" + MainRoot + "',0,'','',0,'','','" + Convert.ToString(Session["User_id"]) + "','IA'";
                            ul.InsertUpdate(qry);

                            //Return File to Load Controller 
                            qry = "select * from tblCarrierDocuments Where CarrierAssignId ='" + CarrierAssignId + "' ";

                            dt = ul.GetDatatable(qry);

                            List<tblCarrierDocument> TemporaryUploadedFiles = new List<tblCarrierDocument>();

                            foreach (DataRow dr in dt.Rows)

                            {

                                TemporaryUploadedFiles.Add(new tblCarrierDocument
                                {


                                    CarrierDocumentId = Convert.ToInt32(dr["CarrierDocumentId"]),
                                    CarrierAssignId = dr["CarrierAssignId"].ToString(),
                                    IsMcAuthorityUploaded = Convert.ToInt32(dr["IsMcAuthorityUploaded"]),
                                    McAuthorityFileName = dr["McAuthorityFileName"].ToString(),
                                    McAuthorityFilePath = dr["McAuthorityFilePath"].ToString(),
                                    IsW9Uploaded = Convert.ToInt32(dr["IsW9Uploaded"]),
                                    W9FileName = dr["W9FileName"].ToString(),
                                    W9FilePath = dr["W9FilePath"].ToString(),
                                    IsInsuranceUploaded = Convert.ToInt32(dr["IsInsuranceUploaded"]),
                                    InsuranceFileName = dr["InsuranceFileName"].ToString(),
                                    InsuranceFilePath = dr["InsuranceFilePath"].ToString(),
                                    UploadedBy = (dr["UploadedBy"].ToString()),
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



            //Upload Document For Notice Of Assignment
            if (DocumentType == "AN")
            {
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


                            var MCAuthorityFileName = Path.GetFileName(file.FileName);
                            MainRoot = "/Uploads/Carrier/" + CarrierAssignId;

                            bool exists = System.IO.Directory.Exists(Server.MapPath(MainRoot));
                            if (!exists)
                                System.IO.Directory.CreateDirectory(Server.MapPath(MainRoot));



                            var MCAuhtorityServerFilePath = Path.Combine(Server.MapPath(MainRoot) + "\\" + MCAuthorityFileName);

                            MainRoot = "/Uploads/Carrier/" + CarrierAssignId + "/" + MCAuthorityFileName;

                            //Save Files to path
                            file.SaveAs(MCAuhtorityServerFilePath);
                            //Save Files to Database
                            //qry = "Exec Sp_Save_Carrier_Documents '" + CarrierAssignId + "', 0,'','' ,1,'" + MCAuthorityFileName + "' ,'" + MainRoot + "',0,'','','" + Convert.ToString(Session["User_id"]) + "','AN'";

                            qry = "Exec Sp_Save_Carrier_Documents '" + CarrierAssignId + "', 0,'','' ,0,'' ,'',0,'','',1,'" + MCAuthorityFileName + "' ,'" + MainRoot + "',0,'','','" + Convert.ToString(Session["User_id"]) + "','AN'";
                            ul.InsertUpdate(qry);

                            //Return File to Load Controller 
                            qry = "select * from tblCarrierDocuments Where CarrierAssignId ='" + CarrierAssignId + "' ";

                            dt = ul.GetDatatable(qry);

                            List<tblCarrierDocument> TemporaryUploadedFiles = new List<tblCarrierDocument>();

                            foreach (DataRow dr in dt.Rows)

                            {

                                TemporaryUploadedFiles.Add(new tblCarrierDocument
                                {


                                    CarrierDocumentId = Convert.ToInt32(dr["CarrierDocumentId"]),
                                    CarrierAssignId = dr["CarrierAssignId"].ToString(),
                                    IsMcAuthorityUploaded = Convert.ToInt32(dr["IsMcAuthorityUploaded"]),
                                    McAuthorityFileName = dr["McAuthorityFileName"].ToString(),
                                    McAuthorityFilePath = dr["McAuthorityFilePath"].ToString(),
                                    IsW9Uploaded = Convert.ToInt32(dr["IsW9Uploaded"]),
                                    W9FileName = dr["W9FileName"].ToString(),
                                    W9FilePath = dr["W9FilePath"].ToString(),
                                    IsInsuranceUploaded = Convert.ToInt32(dr["IsInsuranceUploaded"]),
                                    InsuranceFileName = dr["InsuranceFileName"].ToString(),
                                    InsuranceFilePath = dr["InsuranceFilePath"].ToString(),

                                    IsNoticeOfAssignment = Convert.ToInt32(dr["IsNoticeOfAssignment"]),
                                    AssignmentFileName = dr["AssignmentFileName"].ToString(),
                                    AssignmentFilePath = dr["AssignmentFilePath"].ToString(),

                                    IsVoidCheque = Convert.ToInt32(dr["IsVoidCheque"]),
                                    VoidChequeFileName = dr["VoidChequeFileName"].ToString(),
                                    VoidChequeFilePath = dr["VoidChequeFilePath"].ToString(),
                                    UploadedBy = (dr["UploadedBy"].ToString()),
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


            //Upload Document For Void Cheque 
            if (DocumentType == "VC")
            {
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


                            var MCAuthorityFileName = Path.GetFileName(file.FileName);
                            MainRoot = "/Uploads/Carrier/" + CarrierAssignId;

                            bool exists = System.IO.Directory.Exists(Server.MapPath(MainRoot));
                            if (!exists)
                                System.IO.Directory.CreateDirectory(Server.MapPath(MainRoot));



                            var MCAuhtorityServerFilePath = Path.Combine(Server.MapPath(MainRoot) + "\\" + MCAuthorityFileName);

                            MainRoot = "/Uploads/Carrier/" + CarrierAssignId + "/" + MCAuthorityFileName;

                            //Save Files to path
                            file.SaveAs(MCAuhtorityServerFilePath);
                            //Save Files to Database
                            //qry = "Exec Sp_Save_Carrier_Documents '" + CarrierAssignId + "', 0,'','' ,1,'" + MCAuthorityFileName + "' ,'" + MainRoot + "',0,'','','" + Convert.ToString(Session["User_id"]) + "','AN'";

                            qry = "Exec Sp_Save_Carrier_Documents '" + CarrierAssignId + "', 0,'','' ,0,'' ,'',0,'','',0,'','',1,'" + MCAuthorityFileName + "' ,'" + MainRoot + "','" + Convert.ToString(Session["User_id"]) + "','VC'";
                            ul.InsertUpdate(qry);

                            //Return File to Load Controller 
                            qry = "select * from tblCarrierDocuments Where CarrierAssignId ='" + CarrierAssignId + "' ";

                            dt = ul.GetDatatable(qry);

                            List<tblCarrierDocument> TemporaryUploadedFiles = new List<tblCarrierDocument>();

                            foreach (DataRow dr in dt.Rows)

                            {

                                TemporaryUploadedFiles.Add(new tblCarrierDocument
                                {


                                    CarrierDocumentId = Convert.ToInt32(dr["CarrierDocumentId"]),
                                    CarrierAssignId = dr["CarrierAssignId"].ToString(),
                                    IsMcAuthorityUploaded = Convert.ToInt32(dr["IsMcAuthorityUploaded"]),
                                    McAuthorityFileName = dr["McAuthorityFileName"].ToString(),
                                    McAuthorityFilePath = dr["McAuthorityFilePath"].ToString(),
                                    IsW9Uploaded = Convert.ToInt32(dr["IsW9Uploaded"]),
                                    W9FileName = dr["W9FileName"].ToString(),
                                    W9FilePath = dr["W9FilePath"].ToString(),
                                    IsInsuranceUploaded = Convert.ToInt32(dr["IsInsuranceUploaded"]),
                                    InsuranceFileName = dr["InsuranceFileName"].ToString(),
                                    InsuranceFilePath = dr["InsuranceFilePath"].ToString(),

                                    IsNoticeOfAssignment = Convert.ToInt32(dr["IsNoticeOfAssignment"]),
                                    AssignmentFileName = dr["AssignmentFileName"].ToString(),
                                    AssignmentFilePath = dr["AssignmentFilePath"].ToString(),

                                    IsVoidCheque = Convert.ToInt32(dr["IsVoidCheque"]),
                                    VoidChequeFileName = dr["VoidChequeFileName"].ToString(),
                                    VoidChequeFilePath = dr["VoidChequeFilePath"].ToString(),
                                    UploadedBy = (dr["UploadedBy"].ToString()),
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


            return Json("File Uploaded Successfully!");








            //CarrierAssignId = "0001";
            //string MainRoot = "";
            ////Save MC Authority Files

            //if (MCAuthorityDocument !=null)
            //{
            //    foreach (HttpPostedFileBase AuhtorityFile in MCAuthorityDocument)
            //    {

            //        if (MCAuthorityDocument != null)
            //        {


            //            var MCAuthorityFileName = Path.GetFileName(AuhtorityFile.FileName);
            //            MainRoot = "/Uploads/Carrier/" + CarrierAssignId;

            //            bool exists = System.IO.Directory.Exists(Server.MapPath(MainRoot));
            //            if (!exists)
            //                System.IO.Directory.CreateDirectory(Server.MapPath(MainRoot));



            //            var MCAuhtorityServerFilePath = Path.Combine(Server.MapPath(MainRoot) + "\\" + MCAuthorityFileName);

            //            MainRoot = "/Uploads/Carrier/" + CarrierAssignId + "/" + MCAuthorityFileName;

            //            //Save Files to path
            //            AuhtorityFile.SaveAs(MCAuhtorityServerFilePath);
            //            //Save Files to Database
            //            qry = "Exec Sp_Save_Carrier_Documents '" + CarrierAssignId + "',1,'" + MCAuthorityFileName + "' ,'" + MainRoot + "',0,'','',0,'','','" + Convert.ToString(Session["User_id"]) + "','IM'";
            //            ul.InsertUpdate(qry);

            //            return Json("File Uploaded Successfully!");

            //        }

            //    }

            //}

            //if(W9Document !=null)
            //{
            //    //Save W9 Document Files
            //    foreach (HttpPostedFileBase W9Documents in W9Document)
            //    {

            //        if (W9Documents != null)
            //        {


            //            var MCAuthorityFileName = Path.GetFileName(W9Documents.FileName);
            //            MainRoot = "/Uploads/Carrier/" + CarrierAssignId;

            //            bool exists = System.IO.Directory.Exists(Server.MapPath(MainRoot));
            //            if (!exists)
            //                System.IO.Directory.CreateDirectory(Server.MapPath(MainRoot));



            //            var MCAuhtorityServerFilePath = Path.Combine(Server.MapPath(MainRoot) + "\\" + MCAuthorityFileName);

            //            MainRoot = "/Uploads/Carrier/" + CarrierAssignId + "/" + MCAuthorityFileName;

            //            //Save Files to path
            //            W9Documents.SaveAs(MCAuhtorityServerFilePath);
            //            //Save Files to Database
            //            qry = "Exec Sp_Save_Carrier_Documents '" + CarrierAssignId + "', 0,'','' ,1,'" + MCAuthorityFileName + "' ,'" + MainRoot + "',0,'','','" + Convert.ToString(Session["User_id"]) + "','IW'";
            //            ul.InsertUpdate(qry);
            //            return Json("File Uploaded Successfully!");

            //        }
            //    }
            //}






            //if(InsuranceDocument !=null)
            //{
            //    //Save Insurance Document Files
            //    foreach (HttpPostedFileBase Insurance in InsuranceDocument)
            //    {

            //        if (Insurance != null)
            //        {


            //            var MCAuthorityFileName = Path.GetFileName(Insurance.FileName);
            //            MainRoot = "/Uploads/Carrier/" + CarrierAssignId;

            //            bool exists = System.IO.Directory.Exists(Server.MapPath(MainRoot));
            //            if (!exists)
            //                System.IO.Directory.CreateDirectory(Server.MapPath(MainRoot));



            //            var MCAuhtorityServerFilePath = Path.Combine(Server.MapPath(MainRoot) + "\\" + MCAuthorityFileName);

            //            MainRoot = "/Uploads/Carrier/" + CarrierAssignId + "/" + MCAuthorityFileName;

            //            //Save Files to path
            //            Insurance.SaveAs(MCAuhtorityServerFilePath);
            //            //Save Files to Database
            //            qry = "Exec Sp_Save_Carrier_Documents '" + CarrierAssignId + "',0,'','',0,'','' ,1,'" + MCAuthorityFileName + "' ,'" + MainRoot + "','" + Convert.ToString(Session["User_id"]) + "','IA'";
            //            ul.InsertUpdate(qry);
            //            return Json("File Uploaded Successfully!");

            //        }
            //    }
            //}

            //return RedirectToAction("CarrierInfo");
        }



        [HttpPost]
        [Customexception]
        public ActionResult SaveCarrierInformation(FormCollection FC, HttpPostedFileBase[] MCAuthorityDocument, HttpPostedFileBase[] W9Document,
            HttpPostedFileBase[] InsuranceDocument, HttpPostedFileBase[] DriverLicense ,tblTruck tblTruckList)
        {

            string result = "";
            int IsOwnerOpertor = 0;
            int IsQuickPay = 0;
            int IsBlackList = 0;
            int Istrailerinterchange = 0;
            Decimal Quickpaypercentagevalue = 0;

            var OwnerOperator = (FC.Get("OwnerOperator"));
            var QuickPay = (FC.Get("IsQuickPay"));
            var BlackList = (FC.Get("IsBlackList"));
            var trailerinterchange = (FC.Get("Istrailerinterchange"));
            var Quickpaypercentage = (FC.Get("Quickpaypercentage"));


            Quickpaypercentagevalue = Convert.ToDecimal(Quickpaypercentage);
            //bool IsActive =  Convert.ToBoolean(Active);
            var CarrierAssignId = (FC.Get("CarrierAssignId"));
            if (OwnerOperator == null)
            {
                IsOwnerOpertor = 0;

            }
            else if (OwnerOperator == "on")
            {
                IsOwnerOpertor = 1;

            }

            if (QuickPay == null)
            {
                IsQuickPay = 0;

            }
            else if (QuickPay == "on")
            {
                IsQuickPay = 1;

            }


            if (BlackList == null)
            {
                IsBlackList = 0;

            }
            else if (BlackList == "on")
            {
                IsBlackList = 1;

            }


            if (trailerinterchange == null)
            {
                Istrailerinterchange = 0;

            }
            else if (trailerinterchange == "on")
            {
                Istrailerinterchange = 1;

            }

            

            try
            {

                if(CarrierAssignId !=null)
                { 
                }
                

                string name = FC.Get("CarrierName");
                name = name.Replace("'", @"''");
                //Save Carrier Data
                qry = " Exec  SP_Definition_Carrier '" + name + "','" + FC.Get("ModernCarrier") + "', '" + FC.Get("CarrierAssignId") + "'  ";
                qry += " , '" + FC.Get("ContactName") + "', '" + FC.Get("PhoneNumber") + "' , 'true',"+ IsBlackList + "," + Istrailerinterchange + "," + Session["User_id"] + " ,'I' ";
                qry += " , " + FC.Get("CategoryId") + "," + IsOwnerOpertor + " , " + IsQuickPay + ",0  ,  '" + FC.Get("Email") + "'  ,  '" + FC.Get("ContactName2") + "' ,  '" + FC.Get("PhoneNumber2") + "', '" + FC.Get("InsuranceExpirationDate") + "'," + Quickpaypercentagevalue + "";
                result = ul.ExecuteScalar(qry);

                //Save Carrier Document
                string MainRoot = "";

                ////Save MC Authority Files
                //foreach (HttpPostedFileBase AuhtorityFile in MCAuthorityDocument)
                //{

                //    if (AuhtorityFile != null)
                //    {


                //        var MCAuthorityFileName = Path.GetFileName(AuhtorityFile.FileName);
                //        MainRoot = "/Uploads/Carrier/" + FC.Get("CarrierAssignId");

                //        bool exists = System.IO.Directory.Exists(Server.MapPath(MainRoot));
                //        if (!exists)
                //            System.IO.Directory.CreateDirectory(Server.MapPath(MainRoot));



                //        var MCAuhtorityServerFilePath = Path.Combine(Server.MapPath(MainRoot) + "\\" + MCAuthorityFileName);

                //        MainRoot = "/Uploads/Carrier/" + FC.Get("CarrierAssignId") + "/" + MCAuthorityFileName;

                //        //Save Files to path
                //        AuhtorityFile.SaveAs(MCAuhtorityServerFilePath);
                //        //Save Files to Database
                //        qry = "Exec Sp_Save_Carrier_Documents '" + FC.Get("CarrierAssignId") + "',1,'" + MCAuthorityFileName + "' ,'" + MainRoot + "',0,'','',0,'','','" + Convert.ToString(Session["User_id"]) + "','IM'";
                //        ul.InsertUpdate(qry);


                //    }
                //}

                ////Save W9 Document Files
                //foreach (HttpPostedFileBase W9Documents in W9Document)
                //{

                //    if (W9Documents != null)
                //    {


                //        var MCAuthorityFileName = Path.GetFileName(W9Documents.FileName);
                //        MainRoot = "/Uploads/Carrier/" + FC.Get("CarrierAssignId");

                //        bool exists = System.IO.Directory.Exists(Server.MapPath(MainRoot));
                //        if (!exists)
                //            System.IO.Directory.CreateDirectory(Server.MapPath(MainRoot));



                //        var MCAuhtorityServerFilePath = Path.Combine(Server.MapPath(MainRoot) + "\\" + MCAuthorityFileName);

                //        MainRoot = "/Uploads/Carrier/" + FC.Get("CarrierAssignId") + "/" + MCAuthorityFileName;

                //        //Save Files to path
                //        W9Documents.SaveAs(MCAuhtorityServerFilePath);
                //        //Save Files to Database
                //        qry = "Exec Sp_Save_Carrier_Documents '" + FC.Get("CarrierAssignId") + "', 0,'','' ,1,'" + MCAuthorityFileName + "' ,'" + MainRoot + "',0,'','','" + Convert.ToString(Session["User_id"]) + "','IW'";
                //        ul.InsertUpdate(qry);


                //    }
                //}




                ////Save Insurance Document Files
                //foreach (HttpPostedFileBase Insurance in InsuranceDocument)
                //{

                //    if (Insurance != null)
                //    {


                //        var MCAuthorityFileName = Path.GetFileName(Insurance.FileName);
                //        MainRoot = "/Uploads/Carrier/" + FC.Get("CarrierAssignId");

                //        bool exists = System.IO.Directory.Exists(Server.MapPath(MainRoot));
                //        if (!exists)
                //            System.IO.Directory.CreateDirectory(Server.MapPath(MainRoot));



                //        var MCAuhtorityServerFilePath = Path.Combine(Server.MapPath(MainRoot) + "\\" + MCAuthorityFileName);

                //        MainRoot = "/Uploads/Carrier/" + FC.Get("CarrierAssignId") + "/" + MCAuthorityFileName;

                //        //Save Files to path
                //        Insurance.SaveAs(MCAuhtorityServerFilePath);
                //        //Save Files to Database
                //        qry = "Exec Sp_Save_Carrier_Documents '" + FC.Get("CarrierAssignId") + "',0,'','',0,'','' ,1,'" + MCAuthorityFileName + "' ,'" + MainRoot + "','" + Convert.ToString(Session["User_id"]) + "','IA'";
                //        ul.InsertUpdate(qry);


                //    }
                //}

               // //Save Truck and Driver Data
               // qry = " Exec  SP_Definition_Truck  '" + FC.Get("CarrierAssignId") + "' , '" + FC.Get("TruckNumber") + "','" + FC.Get("TruckYard") + "', ";

               // qry += " '" + FC.Get("TrailerNumber") + "' , " + FC.Get("TrailerTypeId") + "," + FC.Get("ZipCode") + ", '" + FC.Get("AvailableDate") + "',   ";
               // qry += " '" + FC.Get("DriverName") + "' , '" + FC.Get("DriverNumber") + "','" + FC.Get("DriverLanguage") + "', " + Session["User_id"] + ",0," + FC.Get("Driver") + ",'I'   ";
               //ul.InsertUpdate(qry);


                if(tblTruckList ==null)
                {
                    //New Carrier Assign ID
                    qry = "Exec SpGetCarrierNumber";
                    string CarrierAssignID = ul.ExecuteScalar(qry);
                    //Get Form Data to Save
                    ViewBag.CarrierAssignID = CarrierAssignID;
                    ViewBag.LoadType = new ModelHelper().ToSelectLoadTypeItemList(deEntity.tblLoadTypes).ToList();
                    Session["CarrierAssignId"] = CarrierAssignID;
                }
                else
                {

                    //Get Form Data to Save
                    ViewBag.CarrierAssignID = FC.Get("CarrierAssignId");
                    ViewBag.LoadType = new ModelHelper().ToSelectLoadTypeItemList(deEntity.tblLoadTypes).ToList();
                    Session["CarrierAssignId"] = FC.Get("CarrierAssignId");
                }

               
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Exception Occur While Saving Carrier Information" + ex.Message;
            }

           
            return RedirectToAction("CarrierInfo",new { CarrierAssignsId  = ViewBag.CarrierAssignID });
        }


        //Save Carrier info on save button click
        [HttpPost]
        [Customexception]
        public string SaveCarrierInfo(string CarrierAssignsId, string CarrierName, string ContactName, string PhoneNumber, string MC_,string ContactName2, string PhoneNumber2
            ,DateTime InsuranceExpirationDate)
        {
            string AssignIds = CarrierAssignsId;

            CarrierName = CarrierName.Replace("'", @"''");
            //Save Carrier Data
            qry = " Exec  SP_Definition_Carrier '" + CarrierName + "','" + MC_ + "', '" + CarrierAssignsId + "'  ";
            qry += " , '" + ContactName + "', '" + PhoneNumber + "' , 'true'," + Session["User_id"] + " ,'I' ,1,1,0,0,'' ,   '" + ContactName2 + "' ,  '" + PhoneNumber2 + "','" + InsuranceExpirationDate + "',0 ";
            ul.ExecuteScalar(qry);
            return "Carrier Save Successfully";
        }

            [HttpPost]
        [Customexception]
        public ActionResult SaveTruckList(string CarrierAssignsId,string CarrierName,string ContactName,string PhoneNumber,string MC_,int IsOwnerOperator, int IsQuickPay, int IsBlackList,
                int Istrailerinterchange,
                int CarrierCategoryId,string Email, string ContactName2, string PhoneNumber2,DateTime InsuranceExpirationDate,decimal Quickpaypercentage,
                List<tblTruck> TruckList)
        {

            string AssingId = "";

            CarrierName = CarrierName.Replace("'", @"''");
            //Save Carrier Information
            qry = " Exec  SP_Definition_Carrier '" + CarrierName + "','" + MC_ + "', '" + CarrierAssignsId + "'  ";
            qry += " , '" + ContactName + "', '" + PhoneNumber + "' , 'True'," + IsBlackList + "," + Istrailerinterchange + "," + Session["User_id"] + " ,'I',"+ IsOwnerOperator+ "," + IsQuickPay + "," + CarrierCategoryId + ",0,'"+ Email + "' ,   '" + ContactName2 + "' ,  '" + PhoneNumber2 + "','"+ InsuranceExpirationDate + "',"+ Quickpaypercentage + " ";
            //string query = "insert into  tblCountry values('" + country.CountryName + "',"+country.IsActive +",1)";
            ul.ExecuteScalar(qry);
            //Save Truck Information
                if (TruckList == null)
                {
                    TruckList = new List<tblTruck>();
                }

                //Loop and insert records.
                foreach (tblTruck Truck in TruckList)
                {
                    AssingId = Truck.CarrierAssignId;
                    qry = " Exec  SP_Definition_Truck  '" + Truck.CarrierAssignId + "' , '" + Truck.TruckNo + "','" + Truck.TruckYard+ "', ";
                    qry += " '" + Truck.TrailerNo + "' , " + Truck.TrailerTypeId + "," + Truck.ZipCode + ", '" + Truck. AvailableDate.ToString("yyyy-MM-dd") + "',   ";
                qry += " '" + Truck.DriverName + "' , '" + Truck.DriverPhone + "','" + Truck.DriverLanguage + "', " + Session["User_id"] + ",'" + Truck.StateName + "' ";
                qry += " , '"+Truck.CityName +"','"+ Truck.StateCode +"' , 0," + Truck.DriverId + " ,'" + Truck.PrefferedDestination + "','I'   ";
                    ul.InsertUpdate(qry);
                }


            Session["CarrierAssignId"] = CarrierAssignsId;
            //return   RedirectToAction("CarrierInfo",new { CarrierAssignId = AssingId });

            return Json(CarrierAssignsId);
        }

        //CC*****Start Country Crud *****//
        [Customexception]
        public ActionResult Country()
        {
            ViewBag.tblCountry = deEntity.tblCountries.OrderBy(a => a.CountryId).ToList();
            return View();
        }
        [Customexception]
        public ActionResult CreateCountry()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateCountry([Bind(Include = "CountryName,IsActive")] tblCountry country)
        {
            string result = "";

            Utility ul = new Utility();
            qry = "Exec  SP_Definition_Country '" + country.CountryName + "'," + country.IsActive + ","+ Session["User_id"] + " ,'I',0";
            //string query = "insert into  tblCountry values('" + country.CountryName + "',"+country.IsActive +",1)";
            result =ul.ExecuteScalar(qry);
            if(result == "-1")
            {
                ViewBag.Error = "Country Already Open";
                return RedirectToAction("Country");
            }
            else if (result == "0")
            {
                ViewBag.Error = "Country  Open Successfully";
                //return RedirectToAction("Country");
                return RedirectToAction("Country");
            }

            //ul.InsertUpdate(qry);
            return RedirectToAction("Country");
        }

        [Customexception]
        public ActionResult EditCountry(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblCountry country = deEntity.tblCountries.Find(id);
            if (country == null)
            {
                return HttpNotFound();
            }

            return View(country);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Customexception]
        public ActionResult EditCountry(string CountryName, int? id, bool IsActives=false)
                {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblCountry country = deEntity.tblCountries.Find(id);
            if (country == null)
            {
                return HttpNotFound();
            }
            country.CountryName = CountryName;
            country.IsActive = IsActives;
            deEntity.Entry(country).State = EntityState.Modified;
            deEntity.SaveChanges();
            return RedirectToAction("Country");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Customexception]
        public ActionResult DeleteCountry(int id)
        {
            Utility ul = new Utility();
            //string query = "Delete from tblCountry where countryid=" + id;

            qry = "Exec  SP_Definition_Country '',true," + Session["User_id"] + " ,'D',"+ id +" ";
            ul.InsertUpdate(qry);
            return RedirectToAction("Country");
        }
        //CC*****End Country Crud *****//


        //*****Start Carrier Crud *****//
        [Customexception]
        public ActionResult Carrier()
        {
            ViewBag.tblCarriers = deEntity.tblCarriers.OrderBy(a => a.CarrierID).ToList();
            return View();
        }
        [Customexception]
        public ActionResult CreateCarrier()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Customexception]
        public ActionResult CreateCarrier([Bind(Include = "CarrierName,MC,AssignID,ContactName,PhoneNumber,IsActive,ContactName2,PhoneNumber2,InsuranceExpirationDate")] tblCarrier  carrier)
        {
            string result = "";



            Utility ul = new Utility();
            qry = " Exec  SP_Definition_Carrier '" + carrier.CarrierName + "','" + carrier.MC_ + "', "+carrier.AssignID +"  ";
            qry += " , '"+carrier.ContactName +"', '"+carrier.Phonenumber+"' ,  " + carrier.IsActive + "," + Session["User_id"] + " ,'I',0,0,0,'' , '" + carrier.ContactName2 + "', '" + carrier.Phonenumber2 + "', '" + carrier.InsuranceExpirationDate + "',0  ";
            //string query = "insert into  tblCountry values('" + country.CountryName + "',"+country.IsActive +",1)";
            result = ul.ExecuteScalar(qry);
            if (result == "-1")
            {
                ViewBag.Error = "Carrier Already Open";
                return RedirectToAction("Carrier");
            }
            else if (result == "0")
            {
                ViewBag.Error = "Carrier  Open Successfully";
                //return RedirectToAction("Country");
                return RedirectToAction("Carrier");
            }

            //ul.InsertUpdate(qry);
            return RedirectToAction("Carrier");
        }

        [HttpPost]
        public ActionResult DeleteCarrier(string id)
        {
            Utility ul = new Utility();
            //string query = "Delete from tblCountry where countryid=" + id;

            qry = " Update tblcarrier set isdeleted = 1  where AssignID ='"+ id + "' ";

            ul.InsertUpdate(qry);

            qry = "Exec SpGetCarrierNumber";

            string carrierassignid = ul.ExecuteScalar(qry);

            return Json(carrierassignid, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Customexception]
        public ActionResult EditCarrier(string CarrierName, string MC_,string AssignID, string ContactName,string PhoneNumber, int? id, bool IsActives = false)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblCarrier carrier = deEntity.tblCarriers.Find(id);
            if (carrier == null)
            {
                return HttpNotFound();
            }
            carrier.CarrierName = CarrierName;
            carrier.MC_ = MC_;
            carrier.AssignID = AssignID;
            carrier.ContactName = ContactName;
            carrier.Phonenumber = PhoneNumber;
            carrier.IsActive = IsActives;
            deEntity.Entry(carrier).State = EntityState.Modified;
            deEntity.SaveChanges();
            return RedirectToAction("carrier");
        }
        //*****End Carrier Crud *****//

        //*****Start Carrier Helper Crud *****//
        [Customexception]
        public ActionResult CarrierHelper(int? id)
        {
            //ViewBag.tblCarrierHelper = deEntity.tblCarrierHelpers.OrderBy(a => a.CarrierHelperId).ToList();
            //return View();

          
            if (id == null)
            {

                ViewBag.CarrierHelperList = deEntity.sp_Get_Carrier_Helper_List().ToList();
                ViewBag.carriers = new ModelHelper().ToSelectItemList(deEntity.tblCarriers).ToList();
                return View();
            }
            else
            {
                ViewBag.CarrierHelperList = deEntity.sp_Get_Carrier_Helper_List().ToList();

                ViewBag.carriers = new ModelHelper().ToSelectItemList(deEntity.tblCarriers.Where(a => a.CarrierID == id )).ToList();
                return View();
            }
        }


        [Customexception]
        public ActionResult Save([Bind(Include = "CarrierID,CarrierHelperName,IsActive")] tblCarrierHelper carrierHelper)
        {
            carrierHelper.CreatedBy = 1;
            carrierHelper.CreatedDate = DateTime.Now;
            deEntity.tblCarrierHelpers.Add(carrierHelper);
            deEntity.SaveChanges();
            return RedirectToAction("CarrierHelper");
        }

    
        [HttpPost]
        [Customexception]
        public ActionResult DeleteCarrierHelp(int? CarrierHelperId)
        {
            if (CarrierHelperId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblCarrierHelper ticket = deEntity.tblCarrierHelpers.Find(CarrierHelperId);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            deEntity.Entry(ticket).State = EntityState.Deleted;
            deEntity.SaveChanges();
            Utility ul = new Utility();
            string query = " Delete from tblCarrierHelper where CarrierHelperId=" + CarrierHelperId;
            ul.InsertUpdate(query);
            return RedirectToAction("CarrierHelper");
        }

        [Customexception]
        public ActionResult EditCarrierHelper(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblCarrierHelper carrierhelper = deEntity.tblCarrierHelpers.Find(id);
            if (carrierhelper == null)
            {
                return HttpNotFound();
            }

            return View(carrierhelper);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Customexception]
        public ActionResult EditCarrierHelper(string CarrierName_id, int? CarrierID,  int? id, bool IsActives = false)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblCarrierHelper carrierhelper = deEntity.tblCarrierHelpers.Find(id);
            if (carrierhelper == null)
            {
                return HttpNotFound();
            }
            carrierhelper.CarrierHelperName = CarrierName_id;
            carrierhelper.IsActive = IsActives;
            carrierhelper.CarrierID = Convert.ToInt32(CarrierID);
            deEntity.Entry(carrierhelper).State = EntityState.Modified;
            deEntity.SaveChanges();
            return RedirectToAction("carrierhelper");
        }

        //*****Start Broker  Crud *****//
        [Customexception]
        public ActionResult Customer(int? id )
            {

            try
            {

                Session["CarrierInsuranceExpirationList"] = deEntity.tblNotifications.Where(n => n.NotificationType == "C").OrderBy(n => n.IsRead).ThenByDescending(n => n.CreateDate).ToList();
                Session["AlkaiosnotifyList"] = deEntity.tblNotifications.Where(n => n.NotificationType == "P" && n.CompanyId == 1).OrderBy(n => n.IsRead).ThenByDescending(n => n.CreateDate).ToList();
                Session["JetlinenotifyList"] = deEntity.tblNotifications.Where(n => n.NotificationType == "P" && n.CompanyId == 3).OrderBy(n => n.IsRead).ThenByDescending(n => n.CreateDate).ToList();

                string query = "";

                ViewBag.Broker = deEntity.tblBrokers.OrderBy(a => a.Brokerid).ToList();
                //return View();


                if (id == null)
                {
                    query = "Exec SpGetBrokerNumber";
                    string CutomerAssignNumber = ul.ExecuteScalar(query);
                    ViewBag.CustomerAssignId = CutomerAssignNumber;

                    ViewBag.Broker = deEntity.sp_Broker_List().OrderBy(a => a.Brokerid).ToList();

                    return View("Customer");
                }
                else
                {
                    ViewBag.Broker = deEntity.sp_Broker_List().OrderBy(a => a.Brokerid).ToList();

                    return View();
                }
            }

            catch (Exception ex)
            {

            }

            return View();
        }


        [HttpPost]
        [Customexception]
        public ActionResult SaveBroker([Bind(Include = "Name,MC,ContactName,Phone,Email,AccountingEmail,IsActive")] tblBroker broker , HttpPostedFileBase[] MCAuhtortiy, HttpPostedFileBase[] W9Documents
            , HttpPostedFileBase[] AssurityBond)
        {
            try
            {


                //Getting basic Data to save 
                qry = "Exec SpGetBrokerNumber";
                string BrokerAssignId = ul.ExecuteScalar(qry);
                broker.CreatedBy = Convert.ToString(Session["User_id"]);
                broker.CreatedDate = DateTime.Now;
                broker.AssignID = BrokerAssignId;


                //Save Uploaded Documents for Broker 
                string MainRoot =""; 

                //IList<HttpPostedFileBase> MCAuhtortiys = Request.Files.GetMultiple("MCAuhtortiy");
                //Save MC Authority Files
                foreach (HttpPostedFileBase AuhtorityFile in MCAuhtortiy)
                {
                    
                    if (AuhtorityFile != null)
                    {
                       

                        var MCAuthorityFileName = Path.GetFileName(AuhtorityFile.FileName);
                        MainRoot = "/Uploads/Broker/"+BrokerAssignId;

                        bool exists = System.IO.Directory.Exists(Server.MapPath(MainRoot));
                        if (!exists)
                            System.IO.Directory.CreateDirectory(Server.MapPath(MainRoot));

                        

                        var MCAuhtorityServerFilePath = Path.Combine(Server.MapPath(MainRoot)+"\\"+ MCAuthorityFileName);

                        MainRoot = "/Uploads/Broker/"+BrokerAssignId+"/"+MCAuthorityFileName;

                        //Save Files to path
                        AuhtorityFile.SaveAs(MCAuhtorityServerFilePath);
                        //Save Files to Database
                        qry = "Exec Sp_Save_Broker_Documents '"+ BrokerAssignId + "',1,'"+ MCAuthorityFileName + "' ,'"+ MainRoot + "',0,'','',0,'','','"+ Convert.ToString(Session["User_id"]) + "','IM'";
                        ul.InsertUpdate(qry);
                       
                       
                    }
                }
                //Save  W9 Files
                foreach (HttpPostedFileBase W9Document in W9Documents)
                {
                    if (W9Document != null)
                    {
                        MainRoot = "/Uploads/Broker/" + BrokerAssignId;
                        bool exists = System.IO.Directory.Exists(Server.MapPath(MainRoot));
                        if (!exists)
                            System.IO.Directory.CreateDirectory(Server.MapPath(MainRoot));

                        var W9DocumentFileName = Path.GetFileName(W9Document.FileName);
                        var W9DocumentServerFilePath = Path.Combine(Server.MapPath(MainRoot)+"\\"+ W9DocumentFileName);

                        MainRoot = "/Uploads/Broker/"+BrokerAssignId+"/"+W9DocumentFileName;
                        //Save Files to path
                        W9Document.SaveAs(W9DocumentServerFilePath);
                        //Save Files to Database
                        qry = "Exec Sp_Save_Broker_Documents '" + BrokerAssignId + "',0,'','',1,'" + W9DocumentFileName + "' ,'" + MainRoot + "',0,'','','" + Convert.ToString(Session["User_id"]) + "','IW'";
                        ul.InsertUpdate(qry);


                    }
                }


                //Save  Assurity Files
                foreach (HttpPostedFileBase Assurity in AssurityBond)
                {
                    if (Assurity != null)
                    {
                        MainRoot = "/Uploads/Broker/" + BrokerAssignId;
                        bool exists = System.IO.Directory.Exists(Server.MapPath(MainRoot));
                        if (!exists)
                            System.IO.Directory.CreateDirectory(Server.MapPath(MainRoot));

                        var AssurityFileName = Path.GetFileName(Assurity.FileName);
                        var AssurityServerFilePath = Path.Combine(Server.MapPath(MainRoot)+"\\"+ AssurityFileName);

                        MainRoot = "/Uploads/Broker/"+BrokerAssignId+"/"+AssurityFileName;
                        //Save Files to path
                        Assurity.SaveAs(AssurityServerFilePath);
                        //Save Files to Database
                        qry = "Exec Sp_Save_Broker_Documents '" + BrokerAssignId + "',0,'','',0,'','',1,'" + AssurityFileName + "' ,'" + MainRoot + "','" + Convert.ToString(Session["User_id"]) + "','IA'";
                        ul.InsertUpdate(qry);


                    }
                }


                //Save Broker Information in table 
                deEntity.tblBrokers.Add(broker);
                deEntity.SaveChanges();
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Exception Occur While Saving Broker " + ex.Message;
            }
            
            return RedirectToAction("Customer");
        }

        [HttpPost]
        [Customexception]
        public ActionResult DeleteBroker(int? Brokerid)
        {
            if (Brokerid == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblBroker broker = deEntity.tblBrokers.Find(Brokerid);
            if (broker == null)
            {
                return HttpNotFound();
            }
            deEntity.Entry(broker).State = EntityState.Deleted;
            deEntity.SaveChanges();
            Utility ul = new Utility();
            string query = " Delete from tblbroker where brokerId=" + Brokerid;
            ul.InsertUpdate(query);
            return RedirectToAction("Customer");
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        [Customexception]
        //Name, MC,AssignID,ContactName,StateId,CityId,Phone,IsActive
        public ActionResult EditBroker(HttpPostedFileBase[] MCEditAuhtortiy
            , HttpPostedFileBase[] W9EditDocuments
            , HttpPostedFileBase[] AssurityBondEdit ,string Name, string MC, string AssignID, string ContactName, string Phone,string Email,string AccountingEmail,   int? id, bool IsActives = false)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblBroker broker = deEntity.tblBrokers.Find(id);
            if (broker == null)
            {
                return HttpNotFound();
            }
            broker.Name = Name;
            broker.MC = MC;
            broker.AssignID = AssignID;
            broker.ContactName = ContactName;
            broker.Phone = Phone;
            broker.IsActive = IsActives;
            broker.Email = Email;
            broker.AccountingEmail = AccountingEmail;


            //Save Uploaded Documents for Broker 
            string MainRoot = "";

            //IList<HttpPostedFileBase> MCAuhtortiys = Request.Files.GetMultiple("MCAuhtortiy");
            //Save MC Authority Files
            foreach (HttpPostedFileBase AuhtorityFile in MCEditAuhtortiy)
            {

                if (AuhtorityFile != null)
                {


                    var MCAuthorityFileName = Path.GetFileName(AuhtorityFile.FileName);
                    MainRoot = "/Uploads/Broker/" + AssignID;

                    bool exists = System.IO.Directory.Exists(Server.MapPath(MainRoot));
                    if (!exists)
                        System.IO.Directory.CreateDirectory(Server.MapPath(MainRoot));



                    var MCAuhtorityServerFilePath = Path.Combine(Server.MapPath(MainRoot) + "\\" + MCAuthorityFileName);

                    MainRoot = "/Uploads/Broker/" + AssignID + "/" + MCAuthorityFileName;

                    //Save Files to path
                    AuhtorityFile.SaveAs(MCAuhtorityServerFilePath);
                    //Save Files to Database
                    qry = "Exec Sp_Save_Broker_Documents '" + AssignID + "',1,'" + MCAuthorityFileName + "' ,'" + MainRoot + "',0,'','',0,'','','" + Convert.ToString(Session["User_id"]) + "','IM'";
                    ul.InsertUpdate(qry);


                }
            }
            //Save  W9 Files
            foreach (HttpPostedFileBase W9Document in W9EditDocuments)
            {
                if (W9Document != null)
                {
                    MainRoot = "/Uploads/Broker/" + AssignID;
                    bool exists = System.IO.Directory.Exists(Server.MapPath(MainRoot));
                    if (!exists)
                        System.IO.Directory.CreateDirectory(Server.MapPath(MainRoot));

                    var W9DocumentFileName = Path.GetFileName(W9Document.FileName);
                    var W9DocumentServerFilePath = Path.Combine(Server.MapPath(MainRoot) + "\\" + W9DocumentFileName);

                    MainRoot = "/Uploads/Broker/" + AssignID + "/" + W9DocumentFileName;
                    //Save Files to path
                    W9Document.SaveAs(W9DocumentServerFilePath);
                    //Save Files to Database
                    qry = "Exec Sp_Save_Broker_Documents '" + AssignID + "',0,'','',1,'" + W9DocumentFileName + "' ,'" + MainRoot + "',0,'','','" + Convert.ToString(Session["User_id"]) + "','IW'";
                    ul.InsertUpdate(qry);


                }
            }


            //Save  Assurity Files
            foreach (HttpPostedFileBase Assurity in AssurityBondEdit)
            {
                if (Assurity != null)
                {
                    MainRoot = "/Uploads/Broker/" + AssignID;
                    bool exists = System.IO.Directory.Exists(Server.MapPath(MainRoot));
                    if (!exists)
                        System.IO.Directory.CreateDirectory(Server.MapPath(MainRoot));

                    var AssurityFileName = Path.GetFileName(Assurity.FileName);
                    var AssurityServerFilePath = Path.Combine(Server.MapPath(MainRoot) + "\\" + AssurityFileName);

                    MainRoot = "/Uploads/Broker/" + AssignID + "/" + AssurityFileName;
                    //Save Files to path
                    Assurity.SaveAs(AssurityServerFilePath);
                    //Save Files to Database
                    qry = "Exec Sp_Save_Broker_Documents '" + AssignID + "',0,'','',0,'','',1,'" + AssurityFileName + "' ,'" + MainRoot + "','" + Convert.ToString(Session["User_id"]) + "','IA'";
                    ul.InsertUpdate(qry);


                }
            }







            deEntity.Entry(broker).State = EntityState.Modified;
            deEntity.SaveChanges();






            return RedirectToAction("Customer");
        }


        //*****Start State  Crud *****//
        [Customexception]
        public ActionResult State(int? id)
        {

            ViewBag.StateList = deEntity.tblStates.OrderBy(a => a.StateID).ToList();
            //return View();


            if (id == null)
            {

                ViewBag.StateList = deEntity.sp_Get_Country_State().ToList();
                ViewBag.countries = new ModelHelper().ToSelectCountryItem(deEntity.tblCountries).ToList();
                ViewBag.ZoneList = new ModelHelper().ToSelectZoneListItem(deEntity.tblZoneLists).ToList();
                return View();
            }
            else
            {
                ViewBag.StateList = deEntity.sp_Get_Country_State().ToList();

                ViewBag.countries = new ModelHelper().ToSelectCountryItem(deEntity.tblCountries.Where(a => a.CountryId == id)).ToList();
                ViewBag.ZoneList = new ModelHelper().ToSelectZoneListItem(deEntity.tblZoneLists).ToList();
                return View();
            }
       
        }


        [Customexception]
        public ActionResult Savestate([Bind(Include = "CountryId,stateName,IsActive,ZoneListID")] tblState state)
        {
            state.CreatedBy = 1;
            state.CreatedDate = DateTime.Now;
            deEntity.tblStates.Add(state);
            deEntity.SaveChanges();
            return RedirectToAction("State");
        }


        [HttpPost]
        [Customexception]
        public ActionResult DeleteState(int? CarrierHelperId)
        {
            if (CarrierHelperId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblState ticket = deEntity.tblStates.Find(CarrierHelperId);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            deEntity.Entry(ticket).State = EntityState.Deleted;
            deEntity.SaveChanges();
            Utility ul = new Utility();
            string query = " Delete from tblstate where stateid=" + CarrierHelperId;
            ul.InsertUpdate(query);
            return RedirectToAction("State");
        }

        [Customexception]
        public ActionResult Editstate(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblState state = deEntity.tblStates.Find(id);
            if (state == null)
            {
                return HttpNotFound();
            }

            return View(state);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Customexception]
        public ActionResult Editstate(string CarrierName_id, int? CountryId, int? id, bool IsActives = false)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblState state = deEntity.tblStates.Find(id);
            if (state == null)
            {
                return HttpNotFound();
            }
            state.StateName = CarrierName_id;
            state.IsActive = IsActives;
            state.countryId = Convert.ToInt32(CountryId);
            deEntity.Entry(state).State = EntityState.Modified;
            deEntity.SaveChanges();
            return RedirectToAction("state");
        }

        //*****Start City  Crud *****//
        [Customexception]
        public ActionResult City(int? id)
        {

            ViewBag.CityList = deEntity.tblCities.OrderBy(a => a.CityId).ToList();
            //return View();


            if (id == null)
            {

                ViewBag.CityList = deEntity.sp_Get_City_List().ToList();
                ViewBag.countries = new ModelHelper().ToSelectCountryItem(deEntity.tblCountries).ToList();
                ViewBag.states = new ModelHelper().ToSelectStateItem(deEntity.tblStates).ToList();
                return View();
            }
            else
            {
                ViewBag.CityList = deEntity.sp_Get_City_List().ToList();

                ViewBag.countries = new ModelHelper().ToSelectCountryItem(deEntity.tblCountries.Where(a => a.CountryId == id)).ToList();
                ViewBag.states = new ModelHelper().ToSelectStateItem(deEntity.tblStates).ToList();
                return View();
            }

        }

        [Customexception]
        public ActionResult Savecity([Bind(Include = "CountryId,StateId,CityName,IsActive")] tblCity city)
        {
            city.CreatedBy = 1;
            city.CreatedDate = DateTime.Now;
            deEntity.tblCities.Add(city);
            deEntity.SaveChanges();
            return RedirectToAction("city");
        }


        [HttpPost]
        [Customexception]
        public ActionResult DeleteCity(int? CityId)
        {
            if (CityId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblCity ticket = deEntity.tblCities.Find(CityId);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            deEntity.Entry(ticket).State = EntityState.Deleted;
            deEntity.SaveChanges();
            Utility ul = new Utility();
            string query = " Delete from tblcity where cityid=" + CityId;
            ul.InsertUpdate(query);
            return RedirectToAction("City");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Customexception]
        public ActionResult Editcity(string CityName, int? StateId, int? CountryId, int? id, bool IsActives = false)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblCity city = deEntity.tblCities.Find(id);
            if (city == null)
            {
                return HttpNotFound();
            }
            city.CityName = CityName;
            city.IsActive = IsActives;
            city.countryId = Convert.ToInt32(CountryId);
            city.stateID  = Convert.ToInt32(StateId);
            deEntity.Entry(city).State = EntityState.Modified;
            deEntity.SaveChanges();
            return RedirectToAction("city");
        }

        //*****Start Zone  Crud *****//

        [Customexception]
        public ActionResult Zone(int? id)
        {

            ViewBag.ZoneList = deEntity.tblZones.OrderBy(a => a.ZoneId).ToList();
            //return View();


            if (id == null)
            {

                ViewBag.ZoneList = deEntity.sp_Get_Zone_List().ToList();
                ViewBag.countries = new ModelHelper().ToSelectCountryItem(deEntity.tblCountries).ToList();
                ViewBag.states = new ModelHelper().ToSelectStateItem(deEntity.tblStates).ToList();
                ViewBag.cities = new ModelHelper().ToSelectCityItem(deEntity.tblCities).ToList();
                return View();
            }
            else
            {
                ViewBag.CityList = deEntity.sp_Get_City_List().ToList();

                ViewBag.countries = new ModelHelper().ToSelectCountryItem(deEntity.tblCountries.Where(a => a.CountryId == id)).ToList();
                ViewBag.states = new ModelHelper().ToSelectStateItem(deEntity.tblStates).ToList();
                ViewBag.cities = new ModelHelper().ToSelectCityItem(deEntity.tblCities).ToList();
                return View();
            }

        }


        [Customexception]
        public ActionResult SaveZone([Bind(Include = "CountryId,StateId,CityId,ZoneName,IsActive")] tblZone zone)
        {
            zone.CreatedBy = 1;
            zone.CreatedDate = DateTime.Now;
            deEntity.tblZones.Add(zone);
            deEntity.SaveChanges();
            return RedirectToAction("zone");
        }

        [HttpPost]
        [Customexception]
        public ActionResult DeleteZone(int? ZoneId)
        {
            if (ZoneId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblZone ticket = deEntity.tblZones.Find(ZoneId);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            deEntity.Entry(ticket).State = EntityState.Deleted;
            deEntity.SaveChanges();
            Utility ul = new Utility();
            string query = " Delete from tblzone where zoneid=" + ZoneId;
            ul.InsertUpdate(query);
            return RedirectToAction("Zone");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Customexception]
        public ActionResult EditZone(string ZoneName, int? CityId, int? StateId, int? CountryId, int? id, bool IsActives = false)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblZone zone = deEntity.tblZones.Find(id);
            if (zone == null)
            {
                return HttpNotFound();
            }
            zone.CityId = Convert.ToInt32(CityId);
            zone.IsActive = IsActives;
            zone.CountryId = Convert.ToInt32(CountryId);
            zone.StateId = Convert.ToInt32(StateId);
            zone.ZoneName = ZoneName;
            deEntity.Entry(zone).State = EntityState.Modified;
            deEntity.SaveChanges();
            return RedirectToAction("zone");
        }



        //*****Start Driver  Crud *****//
        [Customexception]
        public ActionResult Driver(int? id)
        {

            ViewBag.DriverList = deEntity.tblDrivers.OrderBy(a => a.DriverId).ToList();
            //return View();


            if (id == null)
            {

                ViewBag.DriverList = deEntity.sp_Driver_List().ToList();
                //ViewBag.countries = new ModelHelper().ToSelectCountryItem(deEntity.tblCountries).ToList();
                ViewBag.states = new ModelHelper().ToSelectStateItem(deEntity.tblStates).ToList();
                ViewBag.cities = new ModelHelper().ToSelectCityItem(deEntity.tblCities).ToList();
                return View();
            }
            else
            {
                ViewBag.DriverList = deEntity.sp_Driver_List().ToList();

                //ViewBag.countries = new ModelHelper().ToSelectCountryItem(deEntity.tblCountries.Where(a => a.CountryId == id)).ToList();
                ViewBag.states = new ModelHelper().ToSelectStateItem(deEntity.tblStates).ToList();
                ViewBag.cities = new ModelHelper().ToSelectCityItem(deEntity.tblCities).ToList();
                return View();
            }

        }


        //public ActionResult SaveDriver([Bind(Include = "StateId,CityId,Name,Phone,ZipCode,Address,IsActive")] tblDriver driver)
        //{
        //    driver.CreatedBy = 1;
        //    driver.CreatedDate = DateTime.Now;
        //    deEntity.tblDrivers.Add(driver);
        //    deEntity.SaveChanges();
        //    return RedirectToAction("Driver");
        //}


        [HttpPost]
        [Customexception]
        public ActionResult DeleteDriver(int? DriverId)
        {
            if (DriverId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblDriver ticket = deEntity.tblDrivers.Find(DriverId);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            deEntity.Entry(ticket).State = EntityState.Deleted;
            deEntity.SaveChanges();
            Utility ul = new Utility();
            string query = " Delete from tblDriver where DriverId=" + DriverId;
            ul.InsertUpdate(query);
            return RedirectToAction("Driver");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Customexception]
        public ActionResult EditDriver(string Name,string Phone, string Address, string ZipCode, int? CityId, int? StateId, int? id, bool IsActives = false)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblDriver driver = deEntity.tblDrivers.Find(id);
            if (driver == null)
            {
                return HttpNotFound();
            }
            //driver.CityId = Convert.ToInt32(CityId);
            driver.IsActive = IsActives;

            //driver.StateId = Convert.ToInt32(StateId);
            driver.Name = Name;
            driver.Phone = Phone;
            //driver.Address = Address;
            //driver.ZipCode = ZipCode;
            deEntity.Entry(driver).State = EntityState.Modified;
            deEntity.SaveChanges();
            return RedirectToAction("Driver");
        }

        //*****Start Truck  Crud *****//
        [Customexception]
        public ActionResult Truck(int? id)
        {

            ViewBag.TruckList = deEntity.tblTrucks.OrderBy(a => a.TruckId).ToList();
            //return View();


            if (id == null)
            {


                //if (id == null)
                //{

                //    ViewBag.CarrierHelperList = deEntity.sp_Get_Carrier_Helper_List().ToList();
                //    ViewBag.carriers = new ModelHelper().ToSelectItemList(deEntity.tblCarriers).ToList();
                //    return View();
                //}
                //else
                //{
                //    ViewBag.CarrierHelperList = deEntity.sp_Get_Carrier_Helper_List().ToList();

                //    ViewBag.carriers = new ModelHelper().ToSelectItemList(deEntity.tblCarriers.Where(a => a.CarrierID == id)).ToList();
                //    return View();
                //}

                ViewBag.carriers = new ModelHelper().ToSelectItemList(deEntity.tblCarriers).ToList();
                ViewBag.Zones = new ModelHelper().ToSelectZoneList(deEntity.tblZones).ToList();
                ViewBag.TruckList = deEntity.Sp_Get_Truck_List().ToList();

                //ViewBag.countries = new ModelHelper().ToSelectCountryItem(deEntity.tblCountries).ToList();
                ViewBag.driver = new ModelHelper().ToSelectDriverItem(deEntity.tblDrivers).ToList();
                //ViewBag.cities = new ModelHelper().ToSelectCityItem(deEntity.tblCities).ToList();
                return View();
            }
            else
            {
                ViewBag.TruckList = deEntity.Sp_Get_Truck_List().ToList();
                ViewBag.carriers = new ModelHelper().ToSelectItemList(deEntity.tblCarriers).ToList();
                //ViewBag.countries = new ModelHelper().ToSelectCountryItem(deEntity.tblCountries.Where(a => a.CountryId == id)).ToList();
                ViewBag.driver = new ModelHelper().ToSelectDriverItem(deEntity.tblDrivers).ToList();
                ViewBag.Zones = new ModelHelper().ToSelectZoneList(deEntity.tblZones).ToList();
                return View();
            }

        }

        [Customexception]
        public ActionResult SaveTruck([Bind(Include = "DriverId,Name,IsActive,CarrierID,ZoneID")] tblTruck truck)
        {
            truck.CreatedBy = 1;
            truck.CreatedDate = DateTime.Now;
            deEntity.tblTrucks.Add(truck);
            deEntity.SaveChanges();
            return RedirectToAction("Truck");
        }

        [HttpPost]
        [Customexception]
        public ActionResult DeleteTruck(int? TruckId)
        {
            if (TruckId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblTruck ticket = deEntity.tblTrucks.Find(TruckId);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            deEntity.Entry(ticket).State = EntityState.Deleted;
            deEntity.SaveChanges();
            Utility ul = new Utility();
            string query = " Delete from tbltruck where truckId =" + TruckId;
            ul.InsertUpdate(query);
            return RedirectToAction("Truck");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Customexception]
        public ActionResult EditTruck(string TruckName,  int? DriverId, int? TruckId, int?ZoneID, int?CarrierID, bool IsActives = false)
        {

            if (TruckId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblTruck truck = deEntity.tblTrucks.Find(TruckId);
            if (truck == null)
            {
                return HttpNotFound();
            }
          
            truck.IsActive = IsActives;

           
         
            deEntity.Entry(truck).State = EntityState.Modified;
            deEntity.SaveChanges();
            return RedirectToAction("Truck");
        }
        [Customexception]
        public ActionResult TruckDetail(int? id,string type)
        {

            //DataTable dt = new DataTable();
            //dt = ul.GetDatatable("Exec Sp_Get_Truck_Detailed_List " + id + " ,'" + type + "' ");
            //ViewBag.TruckList = dt.AsEnumerable().ToList(); ;

            ViewBag.TruckList = deEntity.Sp_Get_Truck_Detailed_List(id,type).ToList();

            return View();
        }

        //*****Start Broker Help  Crud *****//
        [Customexception]
        public ActionResult brokerHelper(int? id)
        {
            //ViewBag.tblCarrierHelper = deEntity.tblCarrierHelpers.OrderBy(a => a.CarrierHelperId).ToList();
            //return View();


            if (id == null)
            {

                ViewBag.BrokerHelperList = deEntity.sp_Get_Broker_Helper_List().ToList();
                ViewBag.broker = new ModelHelper().ToSelectBrokerItem(deEntity.tblBrokers).ToList();
                return View();
            }
            else
            {
                ViewBag.BrokerHelperList = deEntity.sp_Get_Broker_Helper_List().ToList();

                ViewBag.broker = new ModelHelper().ToSelectBrokerItem(deEntity.tblBrokers.Where(a => a.Brokerid == id)).ToList();
                return View();
            }
        }

        [HttpPost]
        [Customexception]
        public ActionResult SaveBrokerHelper([Bind(Include = "Brokerid,BrokerHelperName,IsActive")] tblBrokerHelper brokerHelper)
        {
            brokerHelper.CreatedBy = 1;
            brokerHelper.CreatedDate = DateTime.Now;
            deEntity.tblBrokerHelpers.Add(brokerHelper);
            deEntity.SaveChanges();
            return RedirectToAction("brokerHelper");
        }

        [HttpPost]
        [Customexception]
        public ActionResult DeleteBrokerHelper(int? BrokerHelperId)
        {
            if (BrokerHelperId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblBrokerHelper ticket = deEntity.tblBrokerHelpers.Find(BrokerHelperId);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            deEntity.Entry(ticket).State = EntityState.Deleted;
            deEntity.SaveChanges();
            Utility ul = new Utility();
            string query = " Delete from BrokerHelperId where BrokerHelperId =" + BrokerHelperId;
            ul.InsertUpdate(query);
            return RedirectToAction("brokerHelper");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Customexception]
        public ActionResult EditBrokerHelper(string BrokerName_id, int ? Brokerid, int? id, bool IsActives = false)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblBrokerHelper brokerHelper = deEntity.tblBrokerHelpers.Find(id);
            if (brokerHelper == null)
            {
                return HttpNotFound();
            }
            brokerHelper.BrokerHelperId  = Convert.ToInt32(id);
            brokerHelper.IsActive = IsActives;

            brokerHelper.BrokerHelperName = BrokerName_id;

            deEntity.Entry(brokerHelper).State = EntityState.Modified;
            deEntity.SaveChanges();
            return RedirectToAction("brokerHelper");
        }
        [Customexception]
        public JsonResult GetBrokerRecord(string prefix)

        {

            DataSet ds = ul.GetBroker(prefix);

            //List<tblCarrier> searchlist = new List<tblCarrier>();

            List<tblBroker> BrokerList = new List<tblBroker>();

            foreach (DataRow dr in ds.Tables[0].Rows)

            {

                BrokerList.Add(new tblBroker
                {
                    //, MC#,AssignID, ContactName, Phonenumber

                    Name = dr["Name"].ToString(),
                    MC = dr["MC"].ToString(),
                    AssignID = dr["AssignID"].ToString(),
                    ContactName = dr["ContactName"].ToString(),
                    Phone = dr["Phone"].ToString(),




                });

            }

            return Json(BrokerList, JsonRequestBehavior.AllowGet);

        }


        [Customexception]
        public JsonResult GetBrokerDetail(string AssignID)

        {
            string qry = "SELECT ContactName,Name,Phone,Email,AssignID FROM tblBroker Where AssignID='"+AssignID+"' ";
            dt = ul.GetDatatable(qry);
            List<tblBroker> BrokerList = new List<tblBroker>();
            foreach (DataRow dr in dt.Rows)

            {

                BrokerList.Add(new tblBroker
                {
                    //, MC#,AssignID, ContactName, Phonenumber

                    ContactName = dr["ContactName"].ToString(),
                    Name = dr["Name"].ToString(),
                    AssignID = dr["AssignID"].ToString(),
                    Phone = dr["Phone"].ToString(),
                    Email = dr["Email"].ToString(),



                });

            }



            return Json(BrokerList, JsonRequestBehavior.AllowGet);


        }

        [Customexception]
        public JsonResult GetTruckRecord(string prefix)

        {

            DataSet ds = ul.GetTruckList(prefix);

            //List<tblCarrier> searchlist = new List<tblCarrier>();

            List<tblBroker> BrokerList = new List<tblBroker>();

            foreach (DataRow dr in ds.Tables[0].Rows)

            {

                BrokerList.Add(new tblBroker
                {
                    //, MC#,AssignID, ContactName, Phonenumber

                    Name = dr["Name"].ToString(),
                    MC = dr["MC"].ToString(),
                    AssignID = dr["AssignID"].ToString(),
                    ContactName = dr["ContactName"].ToString(),
                    Phone = dr["Phone"].ToString(),




                });

            }

            return Json(BrokerList, JsonRequestBehavior.AllowGet);

        }
        [Customexception]
        public JsonResult GetDriverInfo(Int32 TruckId)

        {

            dt = new DataTable();
            //DataSet ds = ut.GetTruckList(CarrierAssignId);
            string qry = "select D.DriverId,Name,Phone from tblDriver D inner join tblTruck T ON T.DriverId = D.DriverId Where T.TruckId =" + TruckId + "";

            dt = ul.GetDatatable(qry);
            if (dt.Rows.Count == 0)
            {
                tblDriver DriverInfomation = new tblDriver
                {
                    Name = "",
                    DriverId = 0,
                    Phone = "",
                };
                return Json(DriverInfomation, JsonRequestBehavior.AllowGet);
            }
            else
            {
                tblDriver DriverInfomation = new tblDriver
                {
                    Name = dt.Rows[0]["Name"].ToString(),
                    DriverId = Convert.ToInt32(dt.Rows[0]["DriverId"]),
                    Phone = dt.Rows[0]["Phone"].ToString(),
                };
                return Json(DriverInfomation, JsonRequestBehavior.AllowGet);
            }


        }

    }
}