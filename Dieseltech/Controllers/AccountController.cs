
using Dieseltech.Models;
using Rotativa;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Dieseltech.Controllers
{
    [HandleError]
    //[FilterConfig.AuthorizeActionFilter]
    public class AccountController : Controller
    {
        Utility ut = new Utility();
        // GET: Account
        string error="";    
        string name;
        string message = string.Empty;
        string qry="";

        private DieseltechEntities deEntity = new DieseltechEntities();


        // GET: Account
        [Customexception]
        public ActionResult Index()
        {
            Session["User_id"] = null;
            Session["Role_id"] = null;
            Session["type"] = null;
            Session["Menu"] = null;
            Session["CarrierAssignId"] = null;
            qry = "select ColorName from tblThemeColor";
            Session["Color"] = ut.ExecuteScalar(qry);
            return View();
        }
        [HttpPost]
        [Customexception]
        public ActionResult Index(string userLogin, string userPassword)
        {
            try
            {
                string userPasswordnew = Encrypt(userPassword, "sblw-3hn8-sqoy19");
                var EmployeeIDnew = 0;
                var EmployeeIDAlready = 0;
                string UserName = null;
                Int32 EmployeeActiveStatus = 0;
                //string constr = ConfigurationManager.ConnectionStrings["MediaFileEntities"].ConnectionString;
                string activationCode = !string.IsNullOrEmpty(Request.QueryString["ActivationCode"]) ? Request.QueryString["ActivationCode"] : Guid.Empty.ToString();
                if (activationCode != "00000000-0000-0000-0000-000000000000")
                {
                    List<tblActivationCode> info = deEntity.tblActivationCodes.SqlQuery("Exec SP_ActivationCode_Info  '" + activationCode + "'").ToList();
                    foreach (var x in info)
                    {
                        EmployeeIDnew = Convert.ToInt32(x.User_ID );
                    }
                    List<tblUser> newinfo = deEntity.tblUsers.SqlQuery("Exec SP_Verify_UserLogin  '" + userLogin + "' , '" + userPasswordnew + "'").ToList();
                    foreach (var x in newinfo)
                    {
                        EmployeeIDAlready = Convert.ToInt32(x.User_ID);
                        UserName = Convert.ToString(x.UserName);
                    }
                    if (EmployeeIDnew > 0 && EmployeeIDAlready > 0)
                    {

                       
                    }
                }

                string query = "Exec SP_Verify_UserLogin  '" + userLogin + "' , '" + userPasswordnew + "'";
                List<tblUser> info1 = deEntity.tblUsers.SqlQuery(query).ToList();
                
                //Check if User exists or user credentials are incorrect
                if (info1.Count == 0)
                {
                    ViewBag.error = "User Name or Password is incorrect...!!";
                    return View();
                }

                //Read user data if exists
                foreach (var x in info1)
                {
                    EmployeeIDAlready = Convert.ToInt32(x.User_ID);
                    UserName = Convert.ToString(x.UserName);
                    EmployeeActiveStatus = x.IsActive;
                }
                
                if (EmployeeActiveStatus == 0)
                {
                    ViewBag.error = "Your Account is suspended Please contact to Admin..!!";
                    return View();
                }
                //}
                if (EmployeeActiveStatus == 1)
                {
                    int? Accessid = deEntity.tblProfiles.Where(x => x.User_ID == EmployeeIDAlready).Select(s=>s.Accessid).FirstOrDefault();

                    HttpCookie UserID = Request.Cookies["_UserID"];
                    UserID = new HttpCookie("_UserID");
                    UserID.Value = Convert.ToString(EmployeeIDAlready);
                    UserID.Expires = DateTime.Now.AddHours(5);
                    Response.Cookies.Add(UserID);
                    //cookie["UserId"] = EmployeeIDAlready.ToString();
                    Session["User_id"] = EmployeeIDAlready;
                    Session["Role_id"] = Accessid;
                    Session["type"] = UserName;
                    return RedirectToAction("Index", "Home");
                }
                
            }
            catch (Exception ex)
            {
                Session["Error"] = ex.Message;
                ViewBag.error = "Exception Occur while Register User : " + Session["Error"];
                return RedirectToAction("ExceptionPage", "Error");
            }

            return View();
        }





            //(RUS) Regiser User Start *****
            public ActionResult Register()
        {

            return View();
        }

        [HttpPost]
        [Customexception]
        public ActionResult Register(string email, string Username, string pass)
        {
            try
            { 
            //Encrypt Password 
            string password1new = Encrypt(pass, "sblw-3hn8-sqoy19");
            email = email.ToString();
            name = Username;
            int  userId = 0;
            //Create New User  and get User id
                qry = "Exec SP_Insert_User '" + Username + "' , '" + password1new + "' ,'" + email + "','I'";
                userId = Convert.ToInt32(ut.ExecuteScalar(qry));

               if(userId ==0)                {
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

        // Password Encryption 
        [Customexception]
        public static string Encrypt(string input, string key)
        {
            byte[] resultArray={ };
            try
            { 
                byte[] inputArray = UTF8Encoding.UTF8.GetBytes(input);
                TripleDESCryptoServiceProvider tripleDES = new TripleDESCryptoServiceProvider();
                tripleDES.Key = UTF8Encoding.UTF8.GetBytes(key);
                tripleDES.Mode = CipherMode.ECB;
                tripleDES.Padding = PaddingMode.PKCS7;
                ICryptoTransform cTransform = tripleDES.CreateEncryptor();
                resultArray = cTransform.TransformFinalBlock(inputArray, 0, inputArray.Length);
                tripleDES.Clear();
                return Convert.ToBase64String(resultArray, 0, resultArray.Length);
            }
            catch (Exception ex)
            {

            }
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);

        }

        // Send Activation Email
        [Customexception]
        private int SendActivationEmail(int userId, string email)
        {
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["DieselTechEntities"].ConnectionString;
                string activationCode = Guid .NewGuid().ToString();

                using (MailMessage mm = new MailMessage("restock06@gmail.com", email))
                {
                    string link = Request.Url.ToString();
                    link = link.Replace("Register", "Index");
                    mm.Subject = "Account Activation";
                    string body = "Hello " + name + ",";
                    body += "<br /><br />Please click the following link to activate your account";
                    body += "<br /><a href = '" + link + "?ActivationCode=" + activationCode + "'>Click here to activate your account.</a>";
                    body += "<br /><br />Thanks";
                    mm.Body = body;
                    mm.IsBodyHtml = true;
                    SmtpClient smtp = new SmtpClient();
                    smtp.Host = "smtp.gmail.com";
                    smtp.EnableSsl = true;
                    NetworkCredential NetworkCred = new NetworkCredential("restock06@gmail.com", "Developer@123");
                    smtp.UseDefaultCredentials = true;
                    smtp.Credentials = NetworkCred;
                    smtp.Port = 587;
                    smtp.Send(mm);
                }

                qry = "Exec  Sp_ActivationCode '"+ userId + "' , '"+ activationCode  + "'";
                message = ut.ExecuteScalar(qry);
           
                return 1;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
        [Customexception]
        public ActionResult ForgetPassword()
        {
            ViewBag.error = "Please enter your registered email..!!";
            return View();
        }

        [HttpPost]
        [Customexception]
        public ActionResult ForgetPassword(string username)
        {
            var UserId = 0;
            string UserName = null;
            var UserStatus = 0;
            var UserActivationStatus = 0;


            List<tblUser> info1 = deEntity.tblUsers .SqlQuery("Exec SP_SendEmail_ForgetPassword '" + username + "' ").ToList();
            foreach (var x in info1)
            {
                UserId = Convert.ToInt32(x.User_ID);
                UserName = Convert.ToString(x.UserName);
                UserStatus = Convert.ToInt32(x.IsActive);
            }

            //Check if user is not registered
            if(info1.Count == 0)
            {
                ViewBag.error = "Your eneterd Email is not registered..!!";
                return View();
            }

            //Check if user account is suspended
            if (UserStatus == 0)
            {
                ViewBag.error = "Your Email Account is suspended Please contact to Admin..!!";
                return View();
            }


            //List<tblUser> info3 = deEntity.tblUsers.SqlQuery("Select * from [User] where IsActive=0 and Email_Adress = '" + username + "'").ToList();
            //foreach (var x in info3)
            //{
            //    EmployeeIDNotActive1 = Convert.ToInt32(x.User_ID);

            //}
            List<tblActivationCode> info2 = deEntity.tblActivationCodes.SqlQuery("Exec SP_Verify_User_Activation " + UserId).ToList();
            foreach (var x in info2)
            {
            //    EmployeeIDNotActive = Convert.ToInt32(x.User_Id);
                UserActivationStatus = Convert.ToInt32(x.User_ID);
            }

            // check if user activation code  through email is pending
            if (UserActivationStatus > 0)
            {
                ViewBag.error = "Active your account through link that is sent to your registered email.....";
                return View();
            }

            //List<tblUser> info4 = deEntity.tblUsers .SqlQuery("Select * from [User] where Email_Adress = '" + username + "'").ToList();
            //foreach (var x in info4)
            //{
            //    EmployeeIDAlready1 = Convert.ToInt32(x.User_ID);

            //}
           
            if (UserStatus  > 0)
            {
                SendForgetPasswordEmail(UserId, username);
                ViewBag.error = "Password change link is sent to your email address please check your email..!!";
                return View();
            }
            
            //if (EmployeeIDNotActive > 0)
            //{
            //    ViewBag.error = "Active your account through link that is sent to your registered email.....";
            //    return View();
            //}   
            if (UserId > 0)
            {
                Session["User_id"] = UserId;
                Session["type"] = UserName;
                return RedirectToAction("Index", "Home");
            }
           // ViewBag.error = "Password chnage link is sent to ypur email address please check your email..!!";
            return View();

        }

        [Customexception]
        private void SendForgetPasswordEmail(int userId, string email)
        {
            try
            {


                DataTable dt = new DataTable();
                string EmailQuery = "select * from tblEmailSetting";
                dt = ut.GetDatatable(EmailQuery);


                string activationCode = Guid.NewGuid().ToString();
                qry = "Exec sp_Insert_ForgetPassword " + userId + " , '" + activationCode + "' ";
                ut.InsertUpdate(qry);

                //using (SqlConnection con = new SqlConnection(constr))
                //{
                //    using (SqlCommand cmd = new SqlCommand("INSERT INTO ForgetPassword VALUES(@UserId, @ActivationCode)"))
                //    {
                //        using (SqlDataAdapter sda = new SqlDataAdapter())
                //        {
                //            cmd.CommandType = CommandType.Text;
                //            cmd.Parameters.AddWithValue("@UserId", userId);
                //            cmd.Parameters.AddWithValue("@ActivationCode", activationCode);
                //            cmd.Connection = con;
                //            con.Open();
                //            cmd.ExecuteNonQuery();
                //            con.Close();
                //        }
                //    }
                //}
                using (MailMessage mm = new MailMessage(dt.Rows[0]["Email"].ToString(), email))
                {
                    string link = Request.Url.ToString();
                    link = link.Replace("ForgetPassword", "ChangePassword");
                    mm.Subject = "Password reset";
                    string body = "Hello " + name + ",";
                    body += "<br /><br />Please click the following link to reset your password";
                    body += "<br /><a href = '" + link + "?ActivationCode=" + activationCode + "'>Click here to activate your account.</a>";
                    body += "<br /><br />Thanks";
                    mm.Body = body;
                    mm.IsBodyHtml = true;
                    SmtpClient smtp = new SmtpClient();
                    smtp.Host = dt.Rows[0]["Host"].ToString();
                    smtp.EnableSsl = Convert.ToBoolean(dt.Rows[0]["SSLEnable"]);
                    NetworkCredential NetworkCred = new NetworkCredential(dt.Rows[0]["Email"].ToString(), dt.Rows[0]["Password"].ToString());
                    smtp.UseDefaultCredentials = true;
                    smtp.Credentials = NetworkCred;
                    smtp.Port = Convert.ToInt32(dt.Rows[0]["Port"]);
                    smtp.Send(mm);
                }
            }
            catch (Exception e)
            {
                Session["ErrorList"] = e;
            }
        }

        [Customexception]
        public ActionResult ChangePassword()
        {

            ViewBag.error = "Please enter your New Password..!!";
            return View();
        }

        [HttpPost]
        [Customexception]
        public ActionResult ChangePassword(string password1, string password2)
        {
            string password1new = Encrypt(password1, "sblw-3hn8-sqoy19");
            string l = Request.Url.ToString();
            var EmployeeIDnew = 0;
            if (password1 != password2)
            {
                ViewBag.error = "Password not match..!!";
                return View();
            }
            string constr = ConfigurationManager.ConnectionStrings["DieseltechEntities"].ConnectionString;
            string activationCode = !string.IsNullOrEmpty(Request.QueryString["ActivationCode"]) ? Request.QueryString["ActivationCode"] : Guid.Empty.ToString();
            if (activationCode != "00000000-0000-0000-0000-000000000000")
            {
                List<tblForgetPassword> info = deEntity.tblForgetPasswords.SqlQuery("Exec SP_ChangePassword_CodeVerification  '" + activationCode + "'").ToList();
                foreach (var x in info)
                {
                    EmployeeIDnew = Convert.ToInt32(x.User_ID);
                }

                if (EmployeeIDnew > 0)
                {

                    qry = "Exec SP_ChangePassword  '" + activationCode + "','" + password1new + "' , " + EmployeeIDnew + " ";
                    ut.InsertUpdate(qry);
                    return RedirectToAction("Index", "Account");
                    //using (SqlConnection con = new SqlConnection(constr))
                    //{
                    //    using (SqlCommand cmd = new SqlCommand("Exec Sp_UpdatePassword  '"+ activationCode +"','" + password1new + "' , " + EmployeeIDnew + " "))
                    //    {
                    //        using (SqlDataAdapter sda = new SqlDataAdapter())
                    //        {
                    //            cmd.CommandType = CommandType.Text;
                    //            cmd.Parameters.AddWithValue("@Code", activationCode);
                    //            cmd.Connection = con;
                    //            con.Open();
                    //            int rowsAffected = cmd.ExecuteNonQuery();
                    //            con.Close();
                    //            if (rowsAffected == 2)
                    //            {
                    //                return RedirectToAction("Index", "Account");
                    //            }
                    //            else
                    //            {
                    //                return View();
                    //            }
                    //        }
                    //    }

                    //}
                }
                else
                {
                    ViewBag.error = "Password Already changed please make new request..!!";
                    return View();
                }
            }
            return RedirectToAction("Index", "Account");
        }

        [Customexception]
        public ActionResult changepasswordnew()
        {
            ViewBag.error = "Please enter your New Password..!!";
            return View();
        }
        [HttpPost]
        [Customexception]
        public ActionResult changepasswordnew(string password1, string password2)
        {
            string result = "";
            string password1new = Encrypt(password1, "sblw-3hn8-sqoy19");
            //string constr = ConfigurationManager.ConnectionStrings["MediaFileEntities"].ConnectionString;
            if (password1 != password2)
            {
                ViewBag.error = "Password not match..!!";
                return View();
            }


            qry = "update tbluser set Password = '" + password1new + "' where User_ID = " + Session["User_id"] + "; ";
            result= ut.InsertUpdate(qry);

            if (result =="1")
            {
                ViewBag.error = "Your Password has been changed..!!";
                return View();
            }
            else
            {
                return View();
            }
            //using (SqlConnection con = new SqlConnection(constr))
            //{
            //    using (SqlCommand cmd = new SqlCommand(" update [User] set Password='" + password1new + "' where User_ID=" + Session["User_id"] + ";"))
            //    {
            //        using (SqlDataAdapter sda = new SqlDataAdapter())
            //        {
            //            cmd.CommandType = CommandType.Text;
            //            cmd.Connection = con;
            //            con.Open();
            //            int rowsAffected = cmd.ExecuteNonQuery();
            //            con.Close();
            //            if (rowsAffected == 1)
            //            {
            //                ViewBag.error = "Your Password has been changed..!!";
            //                return View();
            //            }
            //            else
            //            {
            //                return View();
            //            }
            //        }
            //    }

            //}
       
        }

        public ActionResult ViewBolPdf()
        {
            
            return View();
        }

        public ActionResult BolDownloadPdf(string LoadNumberModel)
        {
            //Method1
            return new ActionAsPdf("BolPdfDownload", new { LoadNumberModel = LoadNumberModel })
            {
                FileName = "Load_" + LoadNumberModel + ".pdf"

                //FileName = Server.MapPath("~/Content/" + LoadNumberModel + ".pdf")
            };


        }



        public ActionResult BolPdfDownload(string LoadNumberModel)
        {


            return View();
        }

    }
}