using Dieseltech.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Dieseltech.Controllers
{
    [FilterConfig.AuthorizeActionFilter]
    [HandleError]
    public class TruckController : Controller
    {
        private DieseltechEntities deEntity = new DieseltechEntities();
        Utility ut = new Utility();
        DataTable dt = new DataTable();
        // GET: Truck
        [Customexception]
        public ActionResult TruckBoard()
        {

            try
            {
                Session["CarrierInsuranceExpirationList"] = deEntity.tblNotifications.Where(n => n.NotificationType == "C").OrderBy(n => n.IsRead).ThenByDescending(n => n.CreateDate).ToList();
                Session["AlkaiosnotifyList"] = deEntity.tblNotifications.Where(n => n.NotificationType == "P" && n.CompanyId == 1).OrderBy(n => n.IsRead).ThenByDescending(n => n.CreateDate).ToList();
                Session["JetlinenotifyList"] = deEntity.tblNotifications.Where(n => n.NotificationType == "P" && n.CompanyId == 3).OrderBy(n => n.IsRead).ThenByDescending(n => n.CreateDate).ToList();

                //ViewBag.TruckDetails = deEntity.tblTrucks.ToList();
                ViewBag.LoadType = new ModelHelper().ToSelectLoadTypeItemList(deEntity.tblLoadTypes).ToList();
                ViewBag.TruckDetails = deEntity.Sp_GetAll_Truck_List().ToList();

                //ViewBag.TruckDetails = deEntity.tblTrucks.ToList();

                ViewBag.ZoneSateList = deEntity.Sp_Get_Zone_States().ToList();

                ViewBag.ZoneList = deEntity.Sp_Get_ZoneList().ToList();



                //ViewBag.ZoneList = List<Sp_Get_Zone_States_Result>;

                //ViewBag.TruckDetails = deEntity.tblTrucks.ToList().Where(d => d.CarrierAssignId == CarrierAssignsId).ToList();
                return View();

            }

            catch (Exception ex)

            {
                ViewBag.Error = "Exception Occur While Showing Truck Board" + ex.Message;
            }

            return View();
        }


        [HttpPost]
        public ActionResult createTruckList(List<tblTruck> TruckList)
        {
            string qry = "";
            string AssingId = "";

            
            //Save Truck Information
            if (TruckList == null)
            {
                TruckList = new List<tblTruck>();
            }

            //Loop and insert records.
            foreach (tblTruck Truck in TruckList)
            {
                AssingId = Truck.CarrierAssignId;
                qry = " Exec  SP_Definition_Truck  '" + Truck.CarrierAssignId + "' , '" + Truck.TruckNo + "','" + Truck.TruckYard + "', ";
                qry += " '" + Truck.TrailerNo + "' , " + Truck.TrailerTypeId + "," + Truck.ZipCode + ", '" + Truck.AvailableDate.ToString("yyyy-MM-dd") + "',   ";
                qry += " '" + Truck.DriverName + "' , '" + Truck.DriverPhone + "','" + Truck.DriverLanguage + "', " + Session["User_id"] + ",'" + Truck.StateName + "' ";
                qry += " , '" + Truck.CityName + "','" + Truck.StateCode + "' , 0," + Truck.DriverId + " ,'" + Truck.PrefferedDestination + "','I'   ";
                ut.InsertUpdate(qry);
            }



            return Json(AssingId);
        }
        [Customexception]
        public JsonResult GetCarrierTruckDetail(string AssignID)

        {

            string qry = "Exec Sp_Show_TruckDetails'" + AssignID + "' ";

            dt = ut.GetDatatable(qry);


            List<Sp_Show_TruckDetails_Result> CarrierTruckList = new List<Sp_Show_TruckDetails_Result>();

            foreach (DataRow dr in dt.Rows)

            {

                CarrierTruckList.Add(new Sp_Show_TruckDetails_Result
                {
                    //, MC#,AssignID, ContactName, Phonenumber

                    AssignID = dr["AssignID"].ToString(),
                    CarrierName = dr["CarrierName"].ToString(),
                    ContactName = dr["ContactName"].ToString(),
                    Phonenumber = dr["Phonenumber"].ToString(),
                    TruckNo = dr["TruckNo"].ToString(),
                    TruckYard = dr["TruckYard"].ToString(),
                    TrailerNo = dr["TrailerNo"].ToString(),
                    LoadTypeName = (dr["LoadTypeName"].ToString()),
                    ZipCode = Convert.ToInt32(dr["ZipCode"].ToString()),
                    AvailableDate = Convert.ToDateTime(dr["AvailableDate"]),
                    DriverName = dr["DriverName"].ToString(),
                    DriverPhone = dr["DriverPhone"].ToString(),
                    Language = dr["Language"].ToString(),
                    CityName = (dr["CityName"].ToString()),
                    StateCode = dr["StateCode"].ToString(),
                    Board = dr["Board"].ToString(),
                    TruckId = Convert.ToInt32(dr["TruckId"]),
                    LoadSubType = dr["LoadSubType"].ToString(),
                    Team = dr["Team"].ToString(),
                    Rating = Convert.ToInt32(dr["Rating"].ToString()),
                    Comment = dr["Comment"].ToString(),

                });

            }


            return Json(CarrierTruckList, JsonRequestBehavior.AllowGet);


        }
        [Customexception]
        public JsonResult GetPreferredDestinations(int TruckId)
        {
            string PrefferedDestinations = "";

            try
            {
                string Query = "select ISNULL(PrefferedDestination,'') as PrefferedDestination from tblTruck Where TruckId = " + TruckId + " ";
                PrefferedDestinations = ut.ExecuteScalar(Query);
                return Json(PrefferedDestinations, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                ViewBag.Error = "Exception occur while getting preferred destinations: " + ex.Message;
                return Json("0", JsonRequestBehavior.AllowGet);
            }

            //return Json(PrefferedDestinations, JsonRequestBehavior.AllowGet);
        }
        [Customexception]
        public JsonResult FilterTruckBoardList(string FilterData, string FilterType)

        {

            string qry = "Exec Sp_TruckBoard_Truck_List'" + FilterData + "' ,'" + FilterType + "'";

            dt = ut.GetDatatable(qry);


            List<Sp_TruckBoard_Truck_List_Result> FilterTruckList = new List<Sp_TruckBoard_Truck_List_Result>();

            foreach (DataRow dr in dt.Rows)

            {

                FilterTruckList.Add(new Sp_TruckBoard_Truck_List_Result
                {
                    AssignID = dr["AssignID"].ToString(),
                    CarrierName = dr["CarrierName"].ToString(),
                    ContactName = dr["ContactName"].ToString(),
                    Phonenumber = dr["Phonenumber"].ToString(),
                    TruckNo = dr["TruckNo"].ToString(),
                    TruckYard = dr["TruckYard"].ToString(),
                    TrailerNo = dr["TrailerNo"].ToString(),
                    LoadTypeName = (dr["LoadTypeName"].ToString()),
                    ZipCode = Convert.ToInt32(dr["ZipCode"].ToString()),
                    AvailableDate = Convert.ToDateTime(dr["AvailableDate"]),
                    DriverName = dr["DriverName"].ToString(),
                    DriverPhone = dr["DriverPhone"].ToString(),
                    Language = dr["Language"].ToString(),
                    CityName = (dr["CityName"].ToString()),
                    StateCode = dr["StateCode"].ToString(),
                    Board = dr["Board"].ToString(),
                    TruckId = Convert.ToInt32(dr["TruckId"]),
                    LoadSubType = dr["LoadSubType"].ToString(),
                    Team = dr["Team"].ToString(),
                    Rating = Convert.ToInt32(dr["Rating"]),
                    PrefferedDestination = dr["PrefferedDestination"].ToString(),

                });

            }
            return Json(FilterTruckList, JsonRequestBehavior.AllowGet);
        }

        //Update Truck States
        [Customexception]
        public JsonResult UpdateState(Int32 TruckId, Int64 ZipCode, DateTime AvailableDate, string PreferredDestinations)

        {

            string theDate = AvailableDate.ToString("yyyy-MM-dd");
            string qry = "Exec Sp_Update_Truck_State " + TruckId + "," + ZipCode + ",'" + theDate + "','" + PreferredDestinations + "' ";
            ut.InsertUpdate(qry);
            return Json("1", JsonRequestBehavior.AllowGet);
        }

        //Update Truck States
        [Customexception]
        public JsonResult MarkRating(string CarrierAssignId, Int32 RatingID)

        {
            string Rating = "";
            string qry = "Update tblCarrier set Rating = " + RatingID + " Where AssignID = '" + CarrierAssignId + "' ";
            ut.InsertUpdate(qry);

            qry = "select rating from tblCarrier Where AssignID = " + CarrierAssignId + " ";
            Rating = ut.ExecuteScalar(qry);
            return Json(Rating, JsonRequestBehavior.AllowGet);


        }


        //Update Truck States
        [Customexception]
        public JsonResult MarkCarrierRating(Int32 RatingID, string LoaderNumber)

        {
            string Rating = "";
            if (LoaderNumber !="" )
            {
           
                string qry = "Update tblLoadHead set Rating = " + RatingID + " Where LoaderNumber = '" + LoaderNumber + "' ";
                ut.InsertUpdate(qry);

                qry = "select rating from tblLoadHead Where LoaderNumber = '" + LoaderNumber + "' ";
                Rating = ut.ExecuteScalar(qry);
                return Json(Rating, JsonRequestBehavior.AllowGet);

            }
            return Json(Rating, JsonRequestBehavior.AllowGet);


        }






        [Customexception]
        //Update Truck States
        public JsonResult UpdateComments(string CarrierAssignId, string Comment)

        {
            try
            {
                //Session["User_id"]
                string qry = "Exec Sp_InsertUpdate_Comments " + Session["User_id"] + " , '" + Comment + "' , '" + CarrierAssignId + "' ";

                //string qry = "Update tblCarrier set Comment = '" + Comment + "' Where AssignID = '" + CarrierAssignId + "' ";
                ut.InsertUpdate(qry);

                //qry = "select rating from tblCarrier Where AssignID = " + CarrierAssignId + " ";
                //Rating = ut.ExecuteScalar(qry);
                return Json("1", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Exception Occur While Updating Carrier Comment " + ex.Message;
            }
            return Json("1", JsonRequestBehavior.AllowGet);


        }


        [Customexception]
        public JsonResult UpdateCarrierComments(string LoaderNumber, string Comment, int? Ratings,string CarrierAssignId="")

        {

            if (Ratings == null)
            {
                Ratings = 0;
            }

            if (CarrierAssignId == null)
            {
                CarrierAssignId = "";
            }


            List<CarrierComments> CarrierCommentList = new List<CarrierComments>();
            //List<Comment> CarrierCommentList = new List<Comment>();
            try
            {
                string qry = "";
              
                if(LoaderNumber !=null && LoaderNumber !="")
                {
                qry = "select CarrierId  from tblLoadHead Where LoaderNumber = '" + LoaderNumber + "' ";
                    CarrierAssignId = ut.ExecuteScalar(qry);
                }


                //Session["User_id"]
                qry = "Exec Sp_InsertUpdate_Carrier_Comments " + Session["User_id"] + " , '" + Comment + "',"+ Ratings + " , '" + CarrierAssignId + "' ,'" + LoaderNumber + "' ";

                //string qry = "Update tblCarrier set Comment = '" + Comment + "' Where AssignID = '" + CarrierAssignId + "' ";
                ut.InsertUpdate(qry);

                //qry = "select rating from tblCarrier Where AssignID = " + CarrierAssignId + " ";
                //Rating = ut.ExecuteScalar(qry);


                //qry = " select CommentId, Comment, Userid, UserEmail, UserName, CarrierAssignID,Format(Date,'MM/dd/yyyy') as Date From tblComment Where LoaderNumber ='" + LoaderNumber + "'";

                //qry = "Exec Sp_Get_All_Comment_Carrier  '" + LoaderNumber + "','" + CarrierAssignId + "'";

                //dt = ut.GetDatatable(qry);



                //foreach (DataRow dr in dt.Rows)

                //{

                //    CarrierCommentList.Add(new Comment
                //    {
                //        //, MC#,AssignID, ContactName, Phonenumber

                //        CommentId = Convert.ToInt32(dr["CommentId"]),
                //        Comments = dr["Comment"].ToString(),
                //        Userid = Convert.ToInt32(dr["Userid"]),
                //        UserEmail = dr["UserEmail"].ToString(),
                //        UserName = dr["UserName"].ToString(),
                //        Date = dr["Date"].ToString(),

                //    });

                //}



                //return Json(CarrierCommentList, JsonRequestBehavior.AllowGet);


               
                //qry = "select  LH.LoaderNumber, CommentId, isnull(C.Comment,'') as  Comment, LH.Userid, UserEmail, UserName, CarrierAssignID,Format(Date,'MM/dd/yyyy') as Date, isnull(LH.RATING,0) as Rating from tblLoadHead LH Left Outer Join tblComment C ON LH.LoaderNumber = C.LOADERNUMBER Where C.CarrierAssignID ='" + AssignId + "'";
                qry = "Exec Sp_Get_All_Comment_Carrier  '','" + CarrierAssignId + "'";
                dt = ut.GetDatatable(qry);

                foreach (DataRow dr in dt.Rows)

                {

                    CarrierCommentList.Add(new CarrierComments
                    {
                        //, MC#,AssignID, ContactName, Phonenumber

                        CommentId = Convert.ToInt32(dr["CommentId"]),
                        Comments = dr["Comment"].ToString(),
                        Userid = Convert.ToInt32(dr["Userid"]),
                        UserEmail = dr["UserEmail"].ToString(),
                        UserName = dr["UserName"].ToString(),
                        Date = dr["Date"].ToString(),
                        Rating = Convert.ToInt32(dr["Rating"]),
                        AverageRating = Convert.ToInt32(dr["AverageRating"]),
                        LoaderNumber = dr["LoaderNumber"].ToString(),

                    });

                }
                return Json(CarrierCommentList, JsonRequestBehavior.AllowGet);


                //return Json("1", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Exception Occur While Updating Carrier Comment " + ex.Message;
            }
            //return Json("1", JsonRequestBehavior.AllowGet);

            return Json(CarrierCommentList, JsonRequestBehavior.AllowGet);
        }

        
        public ActionResult GetLoadCommentRating(string LoaderNumber)
        {


            List<CarrierComments> CarrierCommentList = new List<CarrierComments>();
            string qry = "";
            qry = "select LH.LoaderNumber , CommentId, isnull(C.Comment,'') as  Comment, LH.Userid, UserEmail, UserName, CarrierAssignID,Format(Date,'MM/dd/yyyy') as Date, isnull(LH.RATING,0) as Rating from tblLoadHead LH Left Outer Join tblComment C ON LH.LoaderNumber = C.LOADERNUMBER Where LH.LoaderNumber ='" + LoaderNumber + "'";
            dt = ut.GetDatatable(qry);



            foreach (DataRow dr in dt.Rows)

            {

                CarrierCommentList.Add(new CarrierComments
                {
                    //, MC#,AssignID, ContactName, Phonenumber

                    CommentId = Convert.ToInt32(dr["CommentId"]),
                    Comments = dr["Comment"].ToString(),
                    Userid = Convert.ToInt32(dr["Userid"]),
                    UserEmail = dr["UserEmail"].ToString(),
                    UserName = dr["UserName"].ToString(),
                    Date = dr["Date"].ToString(),
                    Rating = Convert.ToInt32(dr["Rating"]),
                    LoaderNumber = dr["LoaderNumber"].ToString(),
                });

            }



            return Json(CarrierCommentList, JsonRequestBehavior.AllowGet);

        }
        [Customexception]
        public ActionResult GetCarrierCommentRating(string AssignId)
        {


            List<CarrierComments> CarrierCommentList = new List<CarrierComments>();
            string qry = "";
            //qry = "select  LH.LoaderNumber, CommentId, isnull(C.Comment,'') as  Comment, LH.Userid, UserEmail, UserName, CarrierAssignID,Format(Date,'MM/dd/yyyy') as Date, isnull(LH.RATING,0) as Rating from tblLoadHead LH Left Outer Join tblComment C ON LH.LoaderNumber = C.LOADERNUMBER Where C.CarrierAssignID ='" + AssignId + "'";
            qry = "Exec Sp_Get_All_Comment_Carrier  '','" + AssignId + "'";
            dt = ut.GetDatatable(qry);

            foreach (DataRow dr in dt.Rows)

            {

                CarrierCommentList.Add(new CarrierComments
                {
                    //, MC#,AssignID, ContactName, Phonenumber

                    CommentId = Convert.ToInt32(dr["CommentId"]),
                    Comments = dr["Comment"].ToString(),
                    Userid = Convert.ToInt32(dr["Userid"]),
                    UserEmail = dr["UserEmail"].ToString(),
                    UserName = dr["UserName"].ToString(),
                    Date = dr["Date"].ToString(),
                    Rating = Convert.ToInt32(dr["Rating"]),
                    AverageRating  = Convert.ToInt32(dr["AverageRating"]),
                    LoaderNumber = dr["LoaderNumber"].ToString(),

                });

            }



            return Json(CarrierCommentList, JsonRequestBehavior.AllowGet);

        }
        [Customexception]
        public JsonResult AllCommentList(string CarrierAssignid)

        {


            string qry = " select CommentId, Comment, Userid, UserEmail, UserName, CarrierAssignID,Format(Date,'MM/dd/yyyy') as Date From tblComment Where CarrierAssignID ='" + CarrierAssignid + "'";

            dt = ut.GetDatatable(qry);


            List<Comment> CarrierCommentList = new List<Comment>();

            foreach (DataRow dr in dt.Rows)

            {

                CarrierCommentList.Add(new Comment
                {
                    //, MC#,AssignID, ContactName, Phonenumber

                    CommentId = Convert.ToInt32(dr["CommentId"]),
                    Comments = dr["Comment"].ToString(),
                    Userid = Convert.ToInt32(dr["Userid"]),
                    UserEmail = dr["UserEmail"].ToString(),
                    UserName = dr["UserName"].ToString(),

                    Date = dr["Date"].ToString(),



                });

            }



            return Json(CarrierCommentList, JsonRequestBehavior.AllowGet);


        }


    }
}