using MVC_SYSTEM.Attributes;
using MVC_SYSTEM.AuthModels;
using MVC_SYSTEM.Class;
using MVC_SYSTEM.Security;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace MVC_SYSTEM.Controllers
{

    [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3,Super Power User,Super User,Normal User,Resource,Viewer")]
    public class MainController : Controller
    {
        // GET: Main
        private MVC_SYSTEM_Auth db = new MVC_SYSTEM_Auth();
        GetIdentity getidentity = new GetIdentity();
        EncryptDecrypt crypto = new EncryptDecrypt();
        GetNSWL GetNSWL = new GetNSWL();

        public ActionResult Index()
        {
            ViewBag.Main = "class = active";
            ViewBag.Dropdown = "dropdown";
            //Response.Cache.SetCacheability(HttpCacheability.NoCache);
            return View();
        }

        public JsonResult ChangePassword(string oldpswd, string newpswd, string confirmpswd)
        {
            if (!string.IsNullOrEmpty(oldpswd))
            {
                int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
                int? getuserid = getidentity.ID(User.Identity.Name);
                GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
                var getdata = db.tblUsers.Where(x => x.fldUserID == getuserid && x.fldNegaraID == NegaraID && x.fldSyarikatID == SyarikatID && x.fldWilayahID == WilayahID && x.fldLadangID == LadangID).FirstOrDefault();
                string userpswd = crypto.Encrypt(oldpswd);
                if (getdata != null && getdata.fldUserPassword == userpswd)
                {
                    if (!string.IsNullOrEmpty(newpswd) && confirmpswd == newpswd && newpswd != oldpswd)
                    {
                        //var pswdpattern = "((?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[@#$%]).{6,20})";
                        var pswdpattern = new Regex(@"((?=.*\d)(?=.*[a-z])(?=.*[A-Z]).{6,20})");
                        if (pswdpattern.IsMatch(newpswd))
                        {
                            getdata.fldUserPassword = crypto.Encrypt(newpswd);
                            db.Entry(getdata).State = EntityState.Modified;
                            db.SaveChanges();
                            return Json(new { success = true, msg = "Password successfully changed.", status = "success" });
                        }
                        else
                        {
                            return Json(new { success = false, msg = "Password tidak sah.", status = "warning" });
                        }
                    }
                    else
                    {
                        return Json(new { success = false, msg = "Error.", status = "warning" });
                    }

                }
                else
                {
                    return Json(new { success = false, msg = "Please contact IT", status = "warning" });
                }
            }
            else
            {
                return Json(new { success = false, msg = "Please enter your password", status = "warning" });
            }

        }

        public ActionResult pwd()
        {
            return View();
        }

        [HttpPost]
        public JsonResult pwdchnge(string pass, int processType)
        {
            string code = "";
            if (!string.IsNullOrEmpty(pass))
            {
                if (processType == 1)
                {
                    code = crypto.Encrypt(pass);
                }
                else
                {
                    code = crypto.Decrypt(pass);
                }
            }
            return Json(code);
        }
    }
}