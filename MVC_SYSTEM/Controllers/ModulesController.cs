using MVC_SYSTEM.Attributes;
using MVC_SYSTEM.Class;
using MVC_SYSTEM.ModelsCorporate;
using MVC_SYSTEM.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
//using MVC_SYSTEM.ModelsBudget; //Fitri add 2024

namespace MVC_SYSTEM.Controllers
{
    [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3,Resource,Viewer")]
    public class ModulesController : Controller
    {
        private GetIdentity GetIdentity = new GetIdentity();
        private GetNSWL GetNSWL = new GetNSWL();
        private MVC_SYSTEM_ModelsCorporate db = new MVC_SYSTEM_ModelsCorporate();
        private EncryptDecrypt Encrypt = new EncryptDecrypt();
        private ChangeTimeZone timezone = new ChangeTimeZone();
        //private MVC_SYSTEM_ModelsBudget db2 = new MVC_SYSTEM_ModelsBudget();//Fitri add 2024
        // GET: Modules
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ModulesSelection(string Modules)
        {
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            string RedirectLink = "";
            var ModulesUrl = db.tbl_ModulesUrl.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).ToList();
            if (Modules == "labour")
            {
                var user = db.tblUsers.Where(u => u.fldUserID == getuserid).SingleOrDefault();
                var getestateselection = db.tbl_EstateSelection.Where(x => x.fld_UserID == getuserid).FirstOrDefault();
                GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
                var routeurl = db.tbl_EstateSelection.Where(x => x.fld_UserID == getuserid).Select(s => s.fld_HQUrl).FirstOrDefault();
                string passwordencrypt = Encrypt.Encrypt(user.fldUserPassword);
                string usernameencrypt = Encrypt.Encrypt(user.fldUserName);
                int day = timezone.gettimezone().Day;
                int month = timezone.gettimezone().Month;
                int year = timezone.gettimezone().Year;
                string code = day.ToString() + month.ToString() + year.ToString();
                code = Encrypt.Encrypt(code);

                Response.Cookies.Clear();
                FormsAuthentication.SetAuthCookie(String.Empty, false);
                FormsAuthentication.SignOut();

                RedirectLink = ModulesUrl.Where(x => x.fld_Module.ToUpper() == Modules.ToUpper() && x.fld_LevelAccess == "ALL").Select(s => s.fld_Url).FirstOrDefault();

                RedirectLink = RedirectLink + "/IntegrationLogin?TokenID=" + usernameencrypt + "&PassID=" + passwordencrypt + "&Code=" + code;
            }
            else if (Modules == "checkroll")
            {
                RedirectLink = Url.Action("Index", "Main", null, this.Request.Url.Scheme);
            }
            //Fitri add 2024
            else if (Modules == "budget")
            {
                var user = db.tblUsers.Where(u => u.fldUserID == getuserid).SingleOrDefault();
                string usernameencrypt = Encrypt.Encrypt(user.fldUserName);
                string passwordencrypt = Encrypt.Encrypt(user.fldUserPassword);
                int day = timezone.gettimezone().Day;
                int month = timezone.gettimezone().Month;
                int year = timezone.gettimezone().Year;
                string code = day.ToString() + month.ToString() + year.ToString();
                code = Encrypt.Encrypt(code);

                Response.Cookies.Clear();
                FormsAuthentication.SetAuthCookie(String.Empty, false);
                FormsAuthentication.SignOut();

                RedirectLink = ModulesUrl.Where(x => x.fld_Module.ToUpper() == Modules.ToUpper() && x.fld_LevelAccess == "CRT").Select(s => s.fld_Url).FirstOrDefault();

                RedirectLink = RedirectLink + "/IntegrationLogin?TokenID=" + usernameencrypt + "&PassID=" + passwordencrypt + "&Code=" + code + "&Modules=budget";
            }
            //End add
            else
            {
                RedirectLink = Url.Action("Index", "Modules", null, this.Request.Url.Scheme);
            }

            return Redirect(RedirectLink);
        }
    }
}