using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace Dieseltech.Models
{
    public class Customexception : FilterAttribute, IExceptionFilter
    {
        Utility ut = new Utility();
        public void OnException(ExceptionContext filterContext)
        {


            

            //Send error to email
            DataTable dt = new DataTable();
            string EmailQuery = "select * from tblEmailSetting";
            dt = ut.GetDatatable(EmailQuery);
            var controllerName = filterContext.RouteData.Values["controller"];
            var actionName = filterContext.RouteData.Values["action"];
            //using (MailMessage mm = new MailMessage(dt.Rows[0]["Email"].ToString(), "hasanbilal369@gmail.com"))
            //{
              
            //    mm.Subject = "Exception occur";
            //    string body = "Hello Admin" ;
            //    body += "<br /><br />Date :" + DateTime.Now;
            //    body += "<br /><br />Controller Name:" + controllerName;
            //    body += "<br /><br />Action Name:" + actionName;
            //    body += "<br /><br />Exception:" + filterContext.Exception.Message;
            //    mm.Body = body;
            //    mm.IsBodyHtml = true;
            //    SmtpClient smtp = new SmtpClient();
            //    smtp.Host = dt.Rows[0]["Host"].ToString();
            //    smtp.EnableSsl = Convert.ToBoolean(dt.Rows[0]["SSLEnable"]);
            //    NetworkCredential NetworkCred = new NetworkCredential(dt.Rows[0]["Email"].ToString(), dt.Rows[0]["Password"].ToString());
            //    smtp.UseDefaultCredentials = true;
            //    smtp.Credentials = NetworkCred;
            //    smtp.Port = Convert.ToInt32(dt.Rows[0]["Port"]);
            //    smtp.Send(mm);
            //}

            //save error to textfile
            string temp = AppDomain.CurrentDomain.BaseDirectory;
            string sPath = Path.Combine(temp, "Error_Log.txt");
            string errordetail = "";

            errordetail = "----------------------------------------------------------------------------------------------";
            errordetail += "\r\n" +"Date: " + DateTime.Now.ToString();
            errordetail += "\r\n" + "Controller Name: " + controllerName;
            errordetail += "\r\n" + "Action Name: " + actionName;
            errordetail += "\r\n" + "Exception: " + filterContext.Exception.Message;

            bool fileExist = File.Exists(sPath);
            if (fileExist)
            {
                
                File.AppendAllText(sPath, "\r\n" + errordetail);
                //Console.WriteLine("File exists.");
            }
            else
            {
                using (File.Create(sPath)) ;
                //Console.WriteLine("File does not exist.");
            }


            //throw new NotImplementedException();
            filterContext.Result = new ViewResult()
            {
                ViewName = "error"
                //ViewName = actionName.ToString()
            };
            filterContext.ExceptionHandled = true;

        }
    }
}