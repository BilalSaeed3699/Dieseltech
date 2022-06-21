using Dieseltech.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Dieseltech.Controllers
{
    [FilterConfig.NoDirectAccess]
    [FilterConfig.AuthorizeActionFilter]
    [HandleError]
    public class ProfileController : Controller
    {
        private DieseltechEntities db = new DieseltechEntities();
        // GET: Profile
        [Customexception]
        public ActionResult Index()
        {
            int id = Convert.ToInt32(Session["User_id"]);
            if (id > 0)
                return RedirectToAction("UserProfiles", new { id = id });
            return View();
        }
        public ActionResult EditIndex()
        {
            int id = Convert.ToInt32(Session["User_id"]);
            if (id > 0)
                return RedirectToAction("Edit", new { id = id });

            return View();
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


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Customexception]
        public ActionResult Edit([Bind(Include = "profile_id, Profile_name,phoneNo,Email,youtube,instagram,Twitter,Facebook,Adress,Packageid,Accessid,User_ID")] tblProfile speaker, HttpPostedFileBase Image, string Details, string Image1)
        {
            int id = Convert.ToInt32(Session["User_id"]);

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

                        var FilePath = "/ProfileImg/";
                        //bool exists = System.IO.Directory.Exists(FilePath);
                        //if (!exists)
                        //    System.IO.Directory.CreateDirectory(FilePath);

                        var path = Path.Combine(Server.MapPath(FilePath), myfile);
                      var path1 = Path.Combine(("\\ProfileImg"), myfile);
                        speaker.Image = path1;
                        speaker.IsActive = 1;
                        
                        db.Entry(speaker).State = EntityState.Modified;
                        db.SaveChanges();


                        //Image.SaveAs(path);

                        string MainRoot = "";
                        MainRoot = "/ProfileImg/";

                        bool exists = System.IO.Directory.Exists(Server.MapPath(MainRoot));
                        if (!exists)
                            System.IO.Directory.CreateDirectory(Server.MapPath(MainRoot));



                        var DocumentFilePath = Path.Combine(Server.MapPath(MainRoot) + myfile);

                        MainRoot = "/ProfileImg/" + myfile + " ";

                        //Save Files to path
                        Image.SaveAs(DocumentFilePath);



                        return RedirectToAction("UserProfiles", new { id = id });
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
                    return RedirectToAction("UserProfiles", new { id = id });
                }
            }
            catch(Exception ex)
            {
                ViewBag.Error = "Exception occur while Modifying User Profile:" + ex.Message;
            }
            
            return View(speaker);
        }

        [HttpGet]
        [Customexception]
        public ActionResult UserProfiles(int? id)
        {
            List<tblProfile> list = new List<tblProfile>();
            //if (id == null)
            //{
            //    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            //}
            tblProfile profile = db.tblProfiles.Find(id);
            if (profile == null)
            {
                tblProfile speaker1 = db.tblProfiles.Find(1);
                list.Add(new tblProfile()
                {

                    instagram = "instagram URL",
                    Facebook = "Facebook URL",
                    Twitter = "Twitter URL",
                    youtube = "youtube URL",
                    Profile_name = "XYZ",
                    Image = "\\assets\\img\\user.jpg",
                    phoneNo = "xxx-xxxxxxx",
                    Adress = "--------------",
                    Email = "abc@gmail.com",
                    Details = "",
                });
                ViewBag.speaker = list;
                return View();
            }

            list.Add(new tblProfile()
            {
                instagram = profile.instagram,
                Facebook = profile.Facebook,
                Twitter = profile.Twitter,
                youtube = profile.youtube,
                Profile_name = profile.Profile_name,
                Image = profile.Image,
                phoneNo = profile.phoneNo,
                Adress = profile.Adress,
                Email = profile.Email,
                Details = profile.Details,


            });

            ViewBag.speaker = list;

            return View();
        }
    }
}