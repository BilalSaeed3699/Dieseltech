using Dieseltech.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using static Dieseltech.FilterConfig;

namespace Dieseltech.Controllers
{
    [HandleError]
    [FilterConfig.AuthorizeActionFilter]
    public class UserController : Controller
    {
        // GET: User
        private DieseltechEntities db = new DieseltechEntities();
        string error = "";
        string name;
        string message = string.Empty;
        string qry = "";
        Utility ut = new Utility();
        // GET: User
        [Customexception]
        public ActionResult Index(int? Userid)
        {
            try
            {
                Session["CarrierInsuranceExpirationList"] = db.tblNotifications.Where(n => n.NotificationType == "C").OrderBy(n => n.IsRead).ThenByDescending(n => n.CreateDate).ToList();
                Session["AlkaiosnotifyList"] = db.tblNotifications.Where(n => n.NotificationType == "P" && n.CompanyId == 1).OrderBy(n => n.IsRead).ThenByDescending(n => n.CreateDate).ToList();
                Session["JetlinenotifyList"] = db.tblNotifications.Where(n => n.NotificationType == "P" && n.CompanyId == 3).OrderBy(n => n.IsRead).ThenByDescending(n => n.CreateDate).ToList();


                if (Userid != null)
                {
                    var Users = (from c in db.tblUsers where c.User_ID == Userid select c).FirstOrDefault();
                    Users.Isdeleted = 1;
                    db.SaveChanges();


                    var Profile = (from c in db.tblProfiles where c.User_ID == Userid select c).FirstOrDefault();
                    Profile.Isdeleted = 1;
                    db.SaveChanges();
                }

                ViewBag.User = db.tblProfiles.Where(p => p.IsActive == 1 && p.Isdeleted == 0).ToList();

                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Exception Occur While Showing Users List" + ex.Message;
            }

            return View();
        }

        [Customexception]
        public ActionResult Create()
        {
            ViewBag.error = "Please Complete user profile..!!";
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Customexception]
        public ActionResult Create(string UserName, string Email_Adress, string pass)
        {
            try
            {
                //Encrypt Password 
                string password1new = Encrypt(pass, "sblw-3hn8-sqoy19");
                Email_Adress = Email_Adress.ToString();
                name = UserName;
                int userId = 0;
                //Create New User  and get User id
                qry = "Exec SP_Insert_User '" + UserName + "' , '" + password1new + "' ,'" + Email_Adress + "','I'";
                userId = Convert.ToInt32(ut.ExecuteScalar(qry));

                if (userId == 0)
                {
                    ViewBag.error = "Supplied email address has already been used.";
                    return View();
                }


            }
            catch (Exception ex)
            {
                Session["Error"] = ex.Message;
                ViewBag.error = "Exception Occur while Register User : " + Session["Error"];
                return RedirectToAction("Error", "ExceptionPage");
            }


            return RedirectToAction("Index");
        }

        [Customexception]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ViewBag.access = new ModelHelper().GetAccessList(db.tblAccess_Level.ToList()).ToList();
            ViewBag.Package = new ModelHelper().GetLocationList(db.tblPackages).ToList();
            tblProfile speaker = db.tblProfiles.Find(id);
            if (speaker == null)
            {
                tblProfile speaker1 = db.tblProfiles.Find(1);
                ViewBag.image = speaker1.Image;
                ViewBag.Details = speaker1.Details;
                return View(speaker1);
            }


            ViewBag.image = speaker.Image;
            ViewBag.Details = speaker.Details;
            return View(speaker);
        }

        public ActionResult Verifyemail(string Email, int userid)
        {

            string Query = "";
            DataTable dt = new DataTable();
            try
            {


                Query = "select * from tblUser Where Email_Adress = "+ Email  + " and User_ID <> "+ userid + " ";
                dt = ut.GetDatatable(Query);

                if(dt.Rows.Count>0)
                {
                    //User with same email already exists
                    return Json("0", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    // No user exists with same email 
                    return Json("1", JsonRequestBehavior.AllowGet);
                }
                

            }
            catch (Exception ex)
            {

            }
            return Json("1", JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Customexception]
        public ActionResult Edit([Bind(Include = "profile_id, Profile_name,phoneNo,Email,youtube,instagram,Twitter,Facebook,Adress,Active,Packageid,Accessid,User_ID")] tblProfile speaker, HttpPostedFileBase Image, string Details, string Image1)
        {

            int id = Convert.ToInt32(Session["User_id"]);
            string Query = "";
            Query = "update tblUser set Email_Adress = '" + speaker.Email + "'  where  User_ID = " + speaker.User_ID+ "  ";
            ut.InsertUpdate(Query);



            speaker.Details = Details.ToString();
            var allowedExtensions = new[] { ".Jpg", ".png", ".jpg", ".jpeg" };



            try
            {
                if (Image != null)
                {
                    var fileName = Path.GetFileName(Image.FileName); //getting only file name(ex-ganesh.jpg)  
                    var ext = Path.GetExtension(Image.FileName); //getting the extension(ex-.jpg)  
                    if (allowedExtensions.Contains(ext)) //check what type of extension  
                    {
                        string name = Path.GetFileNameWithoutExtension(fileName); //getting file name without extension  
                        string myfile = name + "_" + speaker.profile_id + ext; //appending the name with id                                                                             // store the file inside ~/project folder(Img)  
                        var path = Path.Combine(Server.MapPath("~/ProfileImg"), myfile);
                        var path1 = Path.Combine(("\\ProfileImg"), myfile);
                        speaker.Image = path1;
                        speaker.IsActive = 1;
                        db.Entry(speaker).State = EntityState.Modified;
                        db.SaveChanges();

                        //bool exists = System.IO.Directory.Exists(Server.MapPath(path1));
                        //if (!exists)
                        //    System.IO.Directory.CreateDirectory(Server.MapPath(path1));
                        //Image.SaveAs(path1);
                        string MainRoot = "";
                        MainRoot = "/ProfileImg/";

                        bool exists = System.IO.Directory.Exists(Server.MapPath(MainRoot));
                        if (!exists)
                            System.IO.Directory.CreateDirectory(Server.MapPath(MainRoot));



                        var DocumentFilePath = Path.Combine(Server.MapPath(MainRoot) + myfile);

                        MainRoot = "/ProfileImg/" + myfile + " ";

                        //Save Files to path
                        Image.SaveAs(DocumentFilePath);


                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ViewBag.message = "Please choose only Image file";
                    }
                }
                else
                {
                    speaker.Image = Image1;
                    speaker.IsActive = 1;
                    db.Entry(speaker).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Exception occur while uploading profile image:" + ex.Message;
            }

            return View(speaker);
        }




        //[HttpPost]
        //[ValidateAntiForgeryToken]
        [Customexception]
        public ActionResult ResetPassword(int? id)
        {
            //Utility ul = new Utility();
            //string query = "update tblUsers set Paswword='admin' where User_ID=" + User_ID;
            //ul.InsertUpdate(query);

            tblUser user = db.tblUsers.Find(id);
            if (user == null)
            {
                tblUser user1 = db.tblUsers.Find(1);

                return View(user1);
            }


            ViewBag.user = user.UserName.Trim();
            ViewBag.User_ID = user.User_ID;
            return View("ResetPassword");



        }




        [HttpPost]
        [ValidateAntiForgeryToken]
        [Customexception]
        public ActionResult ResetPassword(int? UserID, string password1, string password2)
        {
            string result = "";
            string password1new = Encrypt(password1, "sblw-3hn8-sqoy19");
            //string constr = ConfigurationManager.ConnectionStrings["MediaFileEntities"].ConnectionString;
            if (password1 != password2)
            {
                ViewBag.error = "Password not match..!!";
                return View();
            }


            qry = "update tbluser set Password = '" + password1new + "' where User_ID = " + UserID + "; ";
            result = ut.InsertUpdate(qry);

            if (result == "1")
            {
                ViewBag.error = "Your Password has been changed..!!";
                return View();
            }
            else
            {
                return View();
            }

        }

        [Customexception]
        public static string Encrypt(string input, string key)
        {
            byte[] inputArray = UTF8Encoding.UTF8.GetBytes(input);
            TripleDESCryptoServiceProvider tripleDES = new TripleDESCryptoServiceProvider();
            tripleDES.Key = UTF8Encoding.UTF8.GetBytes(key);
            tripleDES.Mode = CipherMode.ECB;
            tripleDES.Padding = PaddingMode.PKCS7;
            ICryptoTransform cTransform = tripleDES.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(inputArray, 0, inputArray.Length);
            tripleDES.Clear();
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }


        [Customexception]
        public ActionResult Userextension()
        {
            try
            {

                ViewBag.Userextensionlist = db.Sp_Get_User_Extensions_List().ToList();

                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Exception occur : " + ex.Message;
            }
            return View();
        }

        [HttpPost]
        [Customexception]
        public ActionResult UpdateExtenstion(int Userid, int companyid, string extension)
        {
            string Query = "";
            try
            {
                Query = "Exec Sp_Insert_Update_User_Extension  " + Userid + " , " + companyid + ", '" + extension + "'";
                ut.InsertUpdate(Query);
                return Json("1", JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {

            }
            return Json("1", JsonRequestBehavior.AllowGet);
        }
    }
}