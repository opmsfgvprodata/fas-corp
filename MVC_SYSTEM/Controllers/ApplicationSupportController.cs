using MVC_SYSTEM.App_LocalResources;
using MVC_SYSTEM.Attributes;
using MVC_SYSTEM.AuthModels;
using MVC_SYSTEM.Class;
using MVC_SYSTEM.log;
using MVC_SYSTEM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MVC_SYSTEM.ModelsCorporate;
using MVC_SYSTEM.ViewingModelsOPMS;

namespace MVC_SYSTEM.Controllers
{
    public class ApplicationSupportController : Controller
    {
        private MVC_SYSTEM_ModelsCorporate db = new MVC_SYSTEM_ModelsCorporate();
        private MVC_SYSTEM_Auth db2 = new MVC_SYSTEM_Auth();
        private ChangeTimeZone timezone = new ChangeTimeZone();
        private GetIdentity getidentity = new GetIdentity();
        private GetNSWL GetNSWL = new GetNSWL();
        private GetWilayah getwilyah = new GetWilayah();
        private DatabaseAction DatabaseAction = new DatabaseAction();
        private GetIdentity GetIdentity = new GetIdentity();
        private SendEmailNotification SendEmailNotification = new SendEmailNotification();
        private errorlog geterror = new errorlog();

        //Added by Shazana 9/3/2023
        private GetLadang getladang = new GetLadang();

        Connection Connection = new Connection();
        // GET: ApplicationSupport
        [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3")]
        public ActionResult Index()
        {
            int? NegaraID = 0;
            int? SyarikatID = 0;
            int? WilayahID = 0;
            int? LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            ViewBag.ApplicationSupport = "class = active";

            ViewBag.ApplicationSupportList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "applicationsupportlist" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).OrderBy(o => o.fldOptConfDesc), "fldOptConfValue", "fldOptConfDesc");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3")]
        public ActionResult Index(string ApplicationSupportList)
        {
            return RedirectToAction(ApplicationSupportList, "ApplicationSupport");
        }

        [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3")]
        public ActionResult ApplicationSupportRegion()
        {
            int[] wlyhid = new int[] { };
            //string mywlyid = "";
            int? NegaraID = 0;
            int? SyarikatID = 0;
            int? WilayahID = 0;
            int? LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            int year = timezone.gettimezone().Year;

            ViewBag.ApplicationSupport = "class = active";

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            List<SelectListItem> WilayahIDList = new List<SelectListItem>();
            List<SelectListItem> LadangIDList = new List<SelectListItem>();
            List<SelectListItem> SyarikatIDList = new List<SelectListItem>(); //fatin added 30/08/2023

            if (WilayahID == 0 && LadangID == 0)
            {
                wlyhid = getwilyah.GetWilayahID(SyarikatID);
                //mywlyid = String.Join("", wlyhid); ;
                //WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
                //WilayahIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
                //fatin added 30/08/2023
                SyarikatIDList = new SelectList(db2.tbl_Syarikat.OrderBy(x => x.fld_NamaPndkSyarikat), "fld_NamaPndkSyarikat", "fld_NamaPndkSyarikat").ToList();
                SyarikatIDList.Insert(0, (new SelectListItem { Text = "Please Select", Value = "0" }));
                //end
                LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgCode).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
                LadangIDList.Insert(0, (new SelectListItem { Text = "Please Select", Value = "0" })); // fatin added - 30/08/2023
                //LadangIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
            }
            else if (WilayahID != 0 && LadangID == 0)
            {
                //mywlyid = String.Join("", WilayahID); ;
                wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
                //WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
                //fatin added 30/08/2023
                SyarikatIDList = new SelectList(db2.tbl_Syarikat.OrderBy(x => x.fld_NamaPndkSyarikat), "fld_NamaPndkSyarikat", "fld_NamaPndkSyarikat").ToList();
                SyarikatIDList.Insert(0, (new SelectListItem { Text = "Please Select", Value = "0" }));
                //end
                LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgCode).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
                LadangIDList.Insert(0, (new SelectListItem { Text = "Please Select", Value = "0" })); //fatin added - 30/08/2023
                //LadangIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));

            }
            else if (WilayahID != 0 && LadangID != 0)
            {
                //mywlyid = String.Join("", WilayahID); ;
                wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
                //WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
                //fatin added 30/08/2023
                SyarikatIDList = new SelectList(db2.tbl_Syarikat.OrderBy(x => x.fld_NamaPndkSyarikat), "fld_NamaPndkSyarikat", "fld_NamaPndkSyarikat").ToList();
                SyarikatIDList.Insert(0, (new SelectListItem { Text = "Please Select", Value = "0" }));
                //end
                LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_ID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_LdgCode).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
                LadangIDList.Insert(0, (new SelectListItem { Text = "Please Select", Value = "0" })); //fatin added - 30/08/2023
            }

            //ViewBag.WilayahIDList = WilayahIDList;
            ViewBag.SyarikatIDList = SyarikatIDList; //fatin added 30/08/2023
            ViewBag.LadangIDList = LadangIDList;
            ViewBag.GetView = 1;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3")]
        public ActionResult ApplicationSupportRegion(int LadangIDList, string SyarikatIDList) //fatin modified add SyarikatIDList - 30/08/2023
        {
            int[] wlyhid = new int[] { };
            //string mywlyid = "";
            int? NegaraID = 0;
            int? SyarikatID = 0;
            int? WilayahID = 0;
            int? LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            DateTime getdate = timezone.gettimezone().AddMonths(-1);
            //DateTime getdate = timezone.gettimezone();

            ViewBag.ApplicationSupport = "class = active";

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            //List<SelectListItem> WilayahIDList2 = new List<SelectListItem>();
            List<SelectListItem> LadangIDList2 = new List<SelectListItem>();

            //fatin added - 30/08/2023
            string SyarikatID2 = "0";
            List<SelectListItem> SyarikatIDList2 = new List<SelectListItem>();

            if (WilayahID == 0 && LadangID == 0)
            {
                wlyhid = getwilyah.GetWilayahID(SyarikatID);
                //mywlyid = String.Join("", wlyhid); ;
                //WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName", WilayahIDList).ToList();
                //WilayahIDList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
                
                //fatin added - 30/08/2023
                SyarikatIDList2 = new SelectList(db2.tbl_Syarikat.Where(x => x.fld_Deleted == false).OrderBy(o => o.fld_NamaPndkSyarikat).Select(s => new SelectListItem { Value = s.fld_NamaPndkSyarikat.ToString(), Text = s.fld_NamaPndkSyarikat }), "Value", "Text", SyarikatIDList2).ToList();
                SyarikatIDList2.Insert(0, (new SelectListItem { Text = "Please Select", Value = "0" }));
                //fatin added fld_CostCentre - 30/08/2023
                LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => x.fld_Deleted == false && x.fld_CostCentre == SyarikatIDList).OrderBy(o => o.fld_LdgCode).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
                LadangIDList2.Insert(0, (new SelectListItem { Text = "Please Select", Value = "0" })); //fatin added - 30/08/2023
            }
            else if (WilayahID != 0 && LadangID == 0)
            {
                //mywlyid = String.Join("", WilayahID); ;
                wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
                //WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName", WilayahIDList).ToList();
                
                //fatin added - 30/08/2023
                SyarikatIDList2 = new SelectList(db2.tbl_Syarikat.Where(x => x.fld_Deleted == false).OrderBy(o => o.fld_NamaPndkSyarikat).Select(s => new SelectListItem { Value = s.fld_NamaPndkSyarikat.ToString(), Text = s.fld_NamaPndkSyarikat }), "Value", "Text", SyarikatIDList2).ToList();
                SyarikatIDList2.Insert(0, (new SelectListItem { Text = "Please Select", Value = "0" }));
                //fatin added fld_CostCentre - 30/08/2023
                LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => x.fld_Deleted == false && x.fld_CostCentre == SyarikatIDList).OrderBy(o => o.fld_LdgCode).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
                LadangIDList2.Insert(0, (new SelectListItem { Text = "Please Select", Value = "0" })); //fatin added - 30/08/2023

            }
            else if (WilayahID != 0 && LadangID != 0)
            {
                //mywlyid = String.Join("", WilayahID); ;
                wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
                //WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName", WilayahIDList).ToList();

                //fatin added - 30/08/2023
                SyarikatIDList2 = new SelectList(db2.tbl_Syarikat.Where(x => x.fld_Deleted == false).OrderBy(o => o.fld_NamaPndkSyarikat).Select(s => new SelectListItem { Value = s.fld_NamaPndkSyarikat.ToString(), Text = s.fld_NamaPndkSyarikat }), "Value", "Text", SyarikatIDList2).ToList();
                SyarikatIDList2.Insert(0, (new SelectListItem { Text = "Please Select", Value = "0" }));
                //fatin added fld_CostCentre - 30/08/2023
                LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => x.fld_ID == LadangID && x.fld_Deleted == false && x.fld_CostCentre == SyarikatIDList).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
                LadangIDList2.Insert(0, (new SelectListItem { Text = "Please Select", Value = "0" })); //fatin added - 30/08/2023
            }

            //ViewBag.WilayahIDList = WilayahIDList2;
            ViewBag.SyarikatIDList = SyarikatIDList2; //fatin added - 30/08/2023
            ViewBag.LadangIDList = LadangIDList2;
            //if(WilayahIDList == 0)
            //{
            //    ViewBag.WilayahSelection = getwilyah.GetWilayahIDForApplicationSupport2(NegaraID, SyarikatID, getdate.Month, getdate.Year);
            //}
            //else
            //{
            //    ViewBag.WilayahSelection = getwilyah.GetWilayahIDForApplicationSupport2(NegaraID,SyarikatID,WilayahIDList, getdate.Month, getdate.Year);
            //}
            ViewBag.NegaraID = NegaraID;
            ViewBag.SyarikatID = SyarikatID;
            ViewBag.LadangID = LadangIDList;
            ViewBag.Month = getdate.Month;
            ViewBag.Year = getdate.Year;
            //ViewBag.Month = 8;
            //ViewBag.Year = 2020;

            //fatin added - 30/08/2023
            if (LadangIDList != 0 && SyarikatIDList != "0")
            {
                ViewBag.GetView = 0;
            }
            else if (LadangIDList == 0 && SyarikatIDList != "0")
            {
                ViewBag.GetView = 1;
            }
            else
            {
                ViewBag.GetView = 1;
            }

            //fatin commented - 30/08/2023
            //ViewBag.GetView = 0;
            return View();
        }
        
        public ActionResult ApplicationSupportRegionDetail(List<long> eachid)
        {
            var getdata = db.vw_PermohonanKewangan.Where(x=> eachid.Contains(x.fld_ID) && x.fld_StsTtpUrsNiaga == true).ToList();
            ViewBag.getgmstatus = getdata.Where(x => x.fld_SokongWilGM_Status == 1).Count();
            return View(getdata);
        }

        [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2")]
        public ActionResult ApplicationSupportRegionGm()
        {
            int[] wlyhid = new int[] { };
            //string mywlyid = "";
            int? NegaraID = 0;
            int? SyarikatID = 0;
            int? WilayahID = 0;
            int? LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            int year = timezone.gettimezone().Year;

            ViewBag.ApplicationSupport = "class = active";

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            //List<SelectListItem> WilayahIDList = new List<SelectListItem>();
            List<SelectListItem> LadangIDList = new List<SelectListItem>();
            List<SelectListItem> SyarikatIDList = new List<SelectListItem>(); // fatin added - 30/08/2023

            if (WilayahID == 0 && LadangID == 0)
            {
                wlyhid = getwilyah.GetWilayahID(SyarikatID);
                //mywlyid = String.Join("", wlyhid); ;
                //WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
                //WilayahIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));

                //fatin added - 30/08/2023
                SyarikatIDList = new SelectList(db2.tbl_Syarikat.OrderBy(x => x.fld_NamaPndkSyarikat), "fld_NamaPndkSyarikat", "fld_NamaPndkSyarikat").ToList();
                SyarikatIDList.Insert(0, (new SelectListItem { Text = "Please Select", Value = "0" }));
                //end
                LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => x.fld_Deleted == false).OrderBy(o => o.fld_LdgCode).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
                LadangIDList.Insert(0, (new SelectListItem { Text = "Please Select", Value = "0" })); //fatin added - 30/08/2023
                //LadangIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
            }
            else if (WilayahID != 0 && LadangID == 0)
            {
                //mywlyid = String.Join("", WilayahID); ;
                //wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
                //WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();

                //fatin added - 30/08/2023
                SyarikatIDList = new SelectList(db2.tbl_Syarikat.OrderBy(x => x.fld_NamaPndkSyarikat), "fld_NamaPndkSyarikat", "fld_NamaPndkSyarikat").ToList();
                SyarikatIDList.Insert(0, (new SelectListItem { Text = "Please Select", Value = "0" }));
                //end
                LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => x.fld_Deleted == false).OrderBy(o => o.fld_LdgCode).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
                LadangIDList.Insert(0, (new SelectListItem { Text = "Please Select", Value = "0" })); //fatin added - 30/08/2023
                //LadangIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));

            }
            else if (WilayahID != 0 && LadangID != 0)
            {
                //mywlyid = String.Join("", WilayahID); ;
                //wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
                //WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();

                //fatin added - 30/08/2023
                SyarikatIDList = new SelectList(db2.tbl_Syarikat.OrderBy(x => x.fld_NamaPndkSyarikat), "fld_NamaPndkSyarikat", "fld_NamaPndkSyarikat").ToList();
                SyarikatIDList.Insert(0, (new SelectListItem { Text = "Please Select", Value = "0" }));
                //end
                LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => x.fld_ID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_LdgCode).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
                LadangIDList.Insert(0, (new SelectListItem { Text = "Please Select", Value = "0" })); //fatin added - 30/08/2023
            }

            //ViewBag.WilayahIDList = WilayahIDList;
            ViewBag.SyarikatIDList = SyarikatIDList; //fatin added - 30/08/2023
            ViewBag.LadangIDList = LadangIDList;
            ViewBag.GetView = 1;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2")]
        public ActionResult ApplicationSupportRegionGm(int LadangIDList, string SyarikatIDList) //fatin modified add SyarikatIDList - 30/08/2023
        {
            int[] wlyhid = new int[] { };
            //string mywlyid = "";
            int? NegaraID = 0;
            int? SyarikatID = 0;
            int? WilayahID = 0;
            int? LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            DateTime getdate = timezone.gettimezone().AddMonths(-1);
            //DateTime getdate = timezone.gettimezone();
            //int LadangIDList = 0;

            ViewBag.ApplicationSupport = "class = active";

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            //List<SelectListItem> WilayahIDList2 = new List<SelectListItem>();
            List<SelectListItem> LadangIDList2 = new List<SelectListItem>();

            //fatin added - 30/08/2023
            string SyarikatID2 = "0";
            List<SelectListItem> SyarikatIDList2 = new List<SelectListItem>();

            if (WilayahID == 0 && LadangID == 0)
            {
                //wlyhid = getwilyah.GetWilayahID(SyarikatID);
                ////mywlyid = String.Join("", wlyhid); ;
                //WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName", WilayahIDList).ToList();
                //WilayahIDList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));

                //fatin added - 30/08/2023
                SyarikatIDList2 = new SelectList(db2.tbl_Syarikat.OrderBy(x => x.fld_NamaPndkSyarikat), "fld_NamaPndkSyarikat", "fld_NamaPndkSyarikat").ToList();
                SyarikatIDList2.Insert(0, (new SelectListItem { Text = "Please Select", Value = "0" }));
                //fatin modified add fld_CostCentre - 30/08/2023
                LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => x.fld_Deleted == false && x.fld_CostCentre == SyarikatIDList).OrderBy(o => o.fld_LdgCode).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
                LadangIDList2.Insert(0, (new SelectListItem { Text = "Please Select", Value = "0" })); //fatin added - 30/08/2023
            }
            else if (WilayahID != 0 && LadangID == 0)
            {
                //mywlyid = String.Join("", WilayahID); ;
                //wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
                //WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName", WilayahIDList).ToList();

                //fatin added - 30/08/2023
                SyarikatIDList2 = new SelectList(db2.tbl_Syarikat.OrderBy(x => x.fld_NamaPndkSyarikat), "fld_NamaPndkSyarikat", "fld_NamaPndkSyarikat").ToList();
                SyarikatIDList2.Insert(0, (new SelectListItem { Text = "Please Select", Value = "0" }));
                //fatin modified add fld_CostCentre - 30/08/2023
                LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => x.fld_Deleted == false && x.fld_CostCentre == SyarikatIDList).OrderBy(o => o.fld_LdgCode).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
                LadangIDList2.Insert(0, (new SelectListItem { Text = "Please Select", Value = "0" })); //fatin added - 30/08/2023

            }
            else if (WilayahID != 0 && LadangID != 0)
            {
                //mywlyid = String.Join("", WilayahID); ;
                //wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
                //WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName", WilayahIDList).ToList();

                //fatin added - 30/08/2023
                SyarikatIDList2 = new SelectList(db2.tbl_Syarikat.OrderBy(x => x.fld_NamaPndkSyarikat), "fld_NamaPndkSyarikat", "fld_NamaPndkSyarikat").ToList();
                SyarikatIDList2.Insert(0, (new SelectListItem { Text = "Please Select", Value = "0" }));
                //fatin modified add fld_CostCentre - 30/08/2023
                LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => x.fld_ID == LadangIDList && x.fld_Deleted == false && x.fld_CostCentre == SyarikatIDList).OrderBy(o => o.fld_LdgCode).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
                LadangIDList2.Insert(0, (new SelectListItem { Text = "Please Select", Value = "0" })); //fatin added - 30/08/2023
            }

            WilayahID = getwilyah.GetWilayahID3(LadangIDList);

            //ViewBag.WilayahIDList = WilayahIDList2;
            ViewBag.SyarikatIDList = SyarikatIDList2; //fatin added - 30/08/2023
            ViewBag.LadangIDList = LadangIDList2;
            //ViewBag.LadangIDList = LadangIDList2;
            //if (WilayahIDList == 0)
            //{
            //    ViewBag.WilayahSelection = getwilyah.GetWilayahIDForApplicationSupport2(NegaraID, SyarikatID, getdate.Month, getdate.Year);
            //}
            //else
            //{
            //    ViewBag.WilayahSelection = getwilyah.GetWilayahIDForApplicationSupport2(NegaraID, SyarikatID, WilayahIDList, getdate.Month, getdate.Year);
            //}
            ViewBag.NegaraID = NegaraID;
            ViewBag.SyarikatID = SyarikatID;
            ViewBag.WilayahID = WilayahID;
            ViewBag.LadangID = LadangIDList;
            ViewBag.Month = getdate.Month;
            ViewBag.Year = getdate.Year;
            //ViewBag.Month = 8;
            //ViewBag.Year = 2020;

            //fatin added - 30/08/2023
            if (LadangIDList != 0 && SyarikatIDList != "0")
            {
                ViewBag.GetView = 0;
            }
            else if (LadangIDList == 0 && SyarikatIDList != "0")
            {
                ViewBag.GetView = 1;
            }
            else
            {
                ViewBag.GetView = 1;
            }
            //fatin commented - 30/08/2023
            //ViewBag.GetView = 0;
            return View();
        }

        //public ActionResult ApplicationSupportRegionGMDetail(List<long> eachid, int NegaraID, int SyarikatID, int WilayahID, int Month, int Year)
        //{
        //    bool matchtotal = false;
        //    var getdata = db.vw_PermohonanKewangan.Where(x => eachid.Contains(x.fld_ID) && x.fld_StsTtpUrsNiaga == true).ToList();
        //    var getcoundata = db.vw_PermohonanKewangan.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_Month == Month && x.fld_Year == Year && x.fld_SemakWil_Status == 1).Count();
        //    if (getdata.Count() == getcoundata)
        //    {
        //        matchtotal = true;
        //    }

        //    ViewBag.matchtotal = matchtotal;
        //    return View(getdata);
        //}

        public ActionResult ApplicationSupportRegionGMDetail(List<long> eachid, int NegaraID, int SyarikatID, int LadangID, int Month, int Year)
        {
            bool matchtotal = false;
            var getdata = db.vw_PermohonanKewangan.Where(x => eachid.Contains(x.fld_ID) && x.fld_StsTtpUrsNiaga == true).ToList();
            var getcoundata = db.vw_PermohonanKewangan.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_LadangID == LadangID && x.fld_Month == Month && x.fld_Year == Year && x.fld_SemakWil_Status == 1).Count();
            if (getdata.Count() == getcoundata)
            {
                matchtotal = true;
            }

            ViewBag.matchtotal = matchtotal;
            return View(getdata);
        }

        [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1")]
        public ActionResult ApplicationSupportRegionHQ()
        {
            int[] wlyhid = new int[] { };
            //string mywlyid = "";
            int? NegaraID = 0;
            int? SyarikatID = 0;
            int? WilayahID = 0;
            int? LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            int year = timezone.gettimezone().Year;

            ViewBag.ApplicationSupport = "class = active";

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            //List<SelectListItem> WilayahIDList = new List<SelectListItem>();
            List<SelectListItem> LadangIDList = new List<SelectListItem>();
            List<SelectListItem> SyarikatIDList = new List<SelectListItem>(); //Added by Shazana 9/3/2023

            if (WilayahID == 0 && LadangID == 0)
            {
                //wlyhid = getwilyah.GetWilayahID(SyarikatID);
                ////mywlyid = String.Join("", wlyhid); ;
                //WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
                //WilayahIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
                //Commented by Shazana 9/3/2023
                //LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => x.fld_Deleted == false).OrderBy(o => o.fld_LdgCode).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
                //LadangIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));

                //modified by faeza
                SyarikatIDList = new SelectList(db2.tbl_Syarikat.OrderBy(x => x.fld_NamaPndkSyarikat), "fld_NamaPndkSyarikat", "fld_NamaPndkSyarikat").ToList();
                SyarikatIDList.Insert(0, (new SelectListItem { Text = "Please Select", Value = "0" }));
                LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => x.fld_Deleted == false).OrderBy(o => o.fld_LdgCode).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
                LadangIDList.Insert(0, (new SelectListItem { Text = "Please Select", Value = "0" }));
            }
            else if (WilayahID != 0 && LadangID == 0)
            {
                //mywlyid = String.Join("", WilayahID); ;
                //wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
                //WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
                //Commented by Shazana 9/3/2023
                //LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => x.fld_Deleted == false).OrderBy(o => o.fld_LdgCode).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
                //LadangIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));

                //modified by faeza
                SyarikatIDList = new SelectList(db2.tbl_Syarikat.OrderBy(x => x.fld_NamaPndkSyarikat), "fld_NamaPndkSyarikat", "fld_NamaPndkSyarikat").ToList();
                SyarikatIDList.Insert(0, (new SelectListItem { Text = "Please Select", Value = "0" }));
                LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => x.fld_Deleted == false && x.fld_WlyhID == WilayahID).OrderBy(o => o.fld_LdgCode).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
                LadangIDList.Insert(0, (new SelectListItem { Text = "Please Select", Value = "0" }));
            }
            else if (WilayahID != 0 && LadangID != 0)
            {
                //mywlyid = String.Join("", WilayahID); ;
                //wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
                //WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
                //Commented by Shazana 9/3/2023
                //LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => x.fld_ID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_LdgCode).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();

                //modified by faeza
                SyarikatIDList = new SelectList(db2.tbl_Syarikat.OrderBy(x => x.fld_NamaPndkSyarikat), "fld_NamaPndkSyarikat", "fld_NamaPndkSyarikat").ToList();
                SyarikatIDList.Insert(0, (new SelectListItem { Text = "Please Select", Value = "0" }));
                LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => x.fld_Deleted == false && x.fld_WlyhID == WilayahID).OrderBy(o => o.fld_LdgCode).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
                LadangIDList.Insert(0, (new SelectListItem { Text = "Please Select", Value = "0" }));
            }

            //ViewBag.WilayahIDList = WilayahIDList;
            ViewBag.SyarikatIDList = SyarikatIDList;
            ViewBag.LadangIDList = LadangIDList;
            ViewBag.GetView = 1;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1")]
        //Modified by Shazana 9/3/2023
        public ActionResult ApplicationSupportRegionHQ(int LadangIDList, string SyarikatIDList)
        {
            int[] wlyhid = new int[] { };
            //string mywlyid = "";
            int? NegaraID = 0;
            int? SyarikatID = 0;
            int? WilayahID = 0;
            int? LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            DateTime getdate = timezone.gettimezone().AddMonths(-1);
            //DateTime getdate = timezone.gettimezone();
            //int LadangIDList = 0;

            ViewBag.ApplicationSupport = "class = active";

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            //List<SelectListItem> WilayahIDList2 = new List<SelectListItem>();
            List<SelectListItem> LadangIDList2 = new List<SelectListItem>();

            //Added by Shazana 9/3/2023
            string SyarikatID2 = "0";
            List<SelectListItem> SyarikatIDList2 = new List<SelectListItem>();

            if (WilayahID == 0 && LadangID == 0)
            {
                //wlyhid = getwilyah.GetWilayahID(SyarikatID);
                ////mywlyid = String.Join("", wlyhid); ;
                //WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName", WilayahIDList).ToList();
                //WilayahIDList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));

                //Modified by Shazana 9/3/2023
                SyarikatIDList2 = new SelectList(db2.tbl_Syarikat.Where(x => x.fld_Deleted == false).OrderBy(o => o.fld_NamaPndkSyarikat).Select(s => new SelectListItem { Value = s.fld_NamaPndkSyarikat.ToString(), Text = s.fld_NamaPndkSyarikat }), "Value", "Text", SyarikatIDList2).ToList();
                SyarikatIDList2.Insert(0, (new SelectListItem { Text = "Please Select", Value = "0" }));
                LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => x.fld_Deleted == false && x.fld_CostCentre == SyarikatIDList).OrderBy(o => o.fld_LdgCode).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
                LadangIDList2.Insert(0, (new SelectListItem { Text = "Please Select", Value = "0" }));
            }
            else if (WilayahID != 0 && LadangID == 0)
            {
                ////mywlyid = String.Join("", WilayahID); ;
                //wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
                //WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName", WilayahIDList).ToList();
                //Commented by Shazana 9/3/2023
                //LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => x.fld_Deleted == false).OrderBy(o => o.fld_LdgCode).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
                //LadangIDList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));

                //Modified by faeza
                SyarikatIDList2 = new SelectList(db2.tbl_Syarikat.Where(x => x.fld_Deleted == false).OrderBy(o => o.fld_NamaPndkSyarikat).Select(s => new SelectListItem { Value = s.fld_NamaPndkSyarikat.ToString(), Text = s.fld_NamaPndkSyarikat }), "Value", "Text", SyarikatIDList2).ToList();
                SyarikatIDList2.Insert(0, (new SelectListItem { Text = "Please Select", Value = "0" }));
                LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => x.fld_Deleted == false && x.fld_CostCentre == SyarikatIDList).OrderBy(o => o.fld_LdgCode).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
                LadangIDList2.Insert(0, (new SelectListItem { Text = "Please Select", Value = "0" }));
            }
            else if (WilayahID != 0 && LadangID != 0)
            {
                ////mywlyid = String.Join("", WilayahID); ;
                //wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
                //WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName", WilayahIDList).ToList();
                //LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => x.fld_ID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_LdgCode).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();

                //Modified by faeza
                SyarikatIDList2 = new SelectList(db2.tbl_Syarikat.Where(x => x.fld_Deleted == false).OrderBy(o => o.fld_NamaPndkSyarikat).Select(s => new SelectListItem { Value = s.fld_NamaPndkSyarikat.ToString(), Text = s.fld_NamaPndkSyarikat }), "Value", "Text", SyarikatIDList2).ToList();
                SyarikatIDList2.Insert(0, (new SelectListItem { Text = "Please Select", Value = "0" }));
                LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => x.fld_Deleted == false && x.fld_CostCentre == SyarikatIDList).OrderBy(o => o.fld_LdgCode).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
                LadangIDList2.Insert(0, (new SelectListItem { Text = "Please Select", Value = "0" }));
            }

            WilayahID = getwilyah.GetWilayahID3(LadangIDList);

            //commented by faeza
            //Added by Shazana 9/3/2023
            //SyarikatID2 = GetSyarikatID3(WilayahID);
            //SyarikatIDList2 = new SelectList(db2.tbl_Syarikat.Where(x => x.fld_Deleted == false).OrderBy(o => o.fld_NamaPndkSyarikat).Select(s => new SelectListItem { Value = s.fld_NamaPndkSyarikat.ToString(), Text = s.fld_NamaPndkSyarikat }), "Value", "Text", SyarikatIDList2).ToList();
            //if ((WilayahID == 0 && LadangID == 0) || (WilayahID != 0 && LadangID == 0))
            //{
            //    SyarikatIDList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
            //    LadangIDList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
            //}

            ViewBag.SyarikatIDList = SyarikatIDList2;
            ViewBag.LadangIDList = LadangIDList2;
            ViewBag.NegaraID = NegaraID;
            ViewBag.SyarikatID = SyarikatID;
            ViewBag.LadangID = LadangIDList;
            ViewBag.WilayahID = WilayahID;
            ViewBag.Month = getdate.Month;
            ViewBag.Year = getdate.Year;            

            if (LadangIDList != 0 && SyarikatIDList != "0")
            {
                ViewBag.GetView = 0;
            }
            else if (LadangIDList == 0 && SyarikatIDList != "0")
            {
                ViewBag.GetView = 1;
            }
            else
            {
                ViewBag.GetView = 1;
            }
            return View();
        }

        public ActionResult ApplicationSupportRegionHQDetail(List<long> eachid, int NegaraID, int SyarikatID, int LadangID, int Month, int Year)
        {
            bool matchtotal = false;
            var getdata = db.vw_PermohonanKewangan.Where(x => eachid.Contains(x.fld_ID) && x.fld_StsTtpUrsNiaga == true).ToList();
            var getcoundata = db.vw_PermohonanKewangan.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_LadangID == LadangID && x.fld_Month == Month && x.fld_Year == Year && x.fld_SemakWil_Status == 1 && x.fld_SokongWilGM_Status == 1).Count();
            if (getdata.Count() == getcoundata)
            {
                matchtotal = true;
            }

            ViewBag.matchtotal = matchtotal;
            return View(getdata);
        }

        public string ApplicationSupportHistoryDetail(long SPWID)
        {
            string returndetail = "";
            string fontcolor = "";
            var getdata = db.tblSokPermhnWangHisActions.Where(x => x.fldHisSPWID == SPWID).OrderBy(o=>o.fldHisDT).ToArray();
            if (getdata != null)
            {
                foreach (var data in getdata)
                {
                    if (data.fldHisDesc == "Telah Ditolak" || data.fldHisDesc == "Urus Niaga Dibuka Semula")
                    {
                        fontcolor = "red";
                        returndetail = returndetail + "<font color=\"" + fontcolor + "\"><p class=\"specialClass\">" + data.fldHisDesc + " oleh " + getidentity.MyNameFullName(data.fldHisUserID) + " pada " + data.fldHisDT + "</p><p> Reason: " + data.fldHisReason + "</p>";
                    }
                    else
                    {
                        fontcolor = "green";
                        returndetail = returndetail + "<font color=\"" + fontcolor + "\"><p class=\"specialClass\">" + data.fldHisDesc + " oleh " + getidentity.MyNameFullName(data.fldHisUserID) + " pada " + data.fldHisDT + "</p>";
                    }
                    //returndetail = returndetail + "<font color=\""+ fontcolor + "\"><p class=\"specialClass\">" + data.fldHisDesc + " oleh " + getidentity.MyNameFullName(data.fldHisUserID) + " pada " + data.fldHisDT + "</p>";
                }
            }
            return returndetail;
        }

        //public JsonResult UpdateData(long DataID, string UpdateFlag, decimal PDP, decimal CIT, int NegaraId, int SyarikatId, int WilayahId, decimal JumlahWang, int Month, int Year, string NoAcc, string NoGL, string NoCIT, decimal Manual, string SebabTolak)
        public JsonResult UpdateData(long DataID, string UpdateFlag, int NegaraId, int SyarikatId, int WilayahId, decimal JumlahWang, int Month, int Year, string NoAcc, string NoCIT, string SebabTolak)
        {
            string DescStatus = "";
            int getuserid = getidentity.ID(User.Identity.Name);
            string ActionBy = GetIdentity.MyNameFullName(getuserid);
            string NamaWilayah = getwilyah.GetWilayahName(WilayahId);
            string subject = "";
            string msg = "";
            string DepartmentHR = "";
            string DepartmentAM = "";
            string DepartmentCL = "";
            string DepartmentMGR = "";
            string[] to = new string[] { };
            List<string> tolist = new List<string>();
            string[] cc = new string[] { };
            List<string> cclist = new List<string>();
            string[] bcc = new string[] { };
            List<string> bcclist = new List<string>();
            DateTime getdatetime = timezone.gettimezone();
            //bool matchtotal = false;

            var GetEstate = db.tbl_SokPermhnWang.Where(x => x.fld_ID == DataID && x.fld_NegaraID == NegaraId && x.fld_SyarikatID == SyarikatId && x.fld_WilayahID == WilayahId).FirstOrDefault();
            var GetEstateDetail = db.tbl_Ladang.Where(x => x.fld_ID == GetEstate.fld_LadangID && x.fld_NegaraID == NegaraId && x.fld_SyarikatID == SyarikatId && x.fld_WlyhID == WilayahId).FirstOrDefault();

            if (GetEstateDetail.fld_CostCentre == "FASSB")
            {
                DepartmentHR = "HR_FINANCE_APPROVAL_FASSB";
                DepartmentAM = "AM_FINANCE_APPROVAL_FASSB";
                DepartmentCL = "CL_FINANCE_APPROVAL_FASSB";
                DepartmentMGR = "MGR_FINANCE_APPROVAL_FASSB";
            }
            else
            {
                DepartmentHR = "HR_FINANCE_APPROVAL_RNDSB";
                DepartmentAM = "AM_FINANCE_APPROVAL_RNDSB";
                DepartmentCL = "CL_FINANCE_APPROVAL_RNDSB";
                DepartmentMGR = "MGR_FINANCE_APPROVAL_RNDSB";
            }

            switch (UpdateFlag)
            {
                case "SemakWil":
                    DescStatus = "Telah Disemak";
                    //DatabaseAction.UpdateDataTotblSokPermhnWang(DataID, 1, 0, 0, 0, 0, 0, "SemakWil", getuserid, getdatetime, PDP, CIT, NoAcc, NoGL, NoCIT, Manual);
                    DatabaseAction.UpdateDataTotblSokPermhnWang(DataID, 1, 0, 0, 0, 0, 0, "SemakWil", getuserid, getdatetime, NoAcc, NoCIT);
                    DatabaseAction.InsertDataTotblSokPermhnWangHisAction(DescStatus, getuserid, getdatetime, DataID, 1, "");

                    //subject = "Sokongan Permohonan Gaji";
                    subject = "FGVASSB - Sokongan Permohonan Gaji (" + GetEstateDetail.fld_LdgName + ")"; //modified by faeza 24.12.2021


                    //var ToEmail = db.tblEmailLists.Where(x => x.fldNegaraID == GetEstate.fld_NegaraID && x.fldSyarikatID == GetEstate.fld_SyarikatID && x.fldDepartment == DepartmentHR && x.fldCategory == "TO" && x.fldDeleted == false).Select(s => new { s.fldEmail, s.fldName }).FirstOrDefault();

                    msg = "<html>";
                    msg += "<body>";
                    msg += "<p>Assalamualaikum WBT & Salam sejahtera,</p>";
                    //msg += "<p>Tn/Pn " + ToEmail.fldName + ",</p>";
                    msg += "<p>Tuan/Puan (HR JTK), </p>";
                    msg += "<p>Sokongan permohonan gaji (Gaji Pekerja Buruh) untuk kelulusan diperlukan dari pihak Tuan/Puan (HR JTK). Keterangan seperti dibawah:-</p>";
                    msg += "<table border=\"1\">";
                    msg += "<thead>";
                    msg += "<tr>";
                    msg += "<th>Kod Ladang</th><th>Nama Ladang</th><th>Jumlah Permohonan (RM)</th><th>Disemak Oleh</th><th>Waktu Disemak</th><th>Pautan</th>";
                    msg += "</tr>";
                    msg += "</thead>";
                    msg += "<tbody>";
                    msg += "<tr>";
                    msg += "<td>" + GetEstateDetail.fld_LdgCode + "</td><td>" + GetEstateDetail.fld_LdgName + "</td><td>" + JumlahWang + "</td><td align=\"center\">" + ActionBy + "</td><td align=\"center\">" + getdatetime + "</td><td><a href=\"" + Url.Action("ApplicationSupportRegionGm", "ApplicationSupport", null, this.Request.Url.Scheme) + "\">Klik ke pautan sokongan</a></td>";
                    msg += "</tr>";
                    msg += "</tbody>";
                    msg += "</table>";
                    msg += "<p>Terima Kasih.</p>";
                    msg += "</body>";
                    msg += "</html>";

                    var emailtolist = db.tblEmailLists.Where(x => x.fldNegaraID == GetEstate.fld_NegaraID && x.fldSyarikatID == GetEstate.fld_SyarikatID && x.fldDepartment == DepartmentHR && x.fldCategory == "TO" && x.fldDeleted == false).Select(s => new { s.fldEmail, s.fldName }).ToList();
                    if (emailtolist != null)
                    {
                        foreach (var toemail in emailtolist)
                        {
                            tolist.Add(toemail.fldEmail);
                        }
                        to = tolist.ToArray();
                    }

                    var emailcclist = db.tblEmailLists.Where(x => x.fldNegaraID == NegaraId && x.fldSyarikatID == SyarikatId && ((x.fldDepartment == DepartmentMGR && x.fldCategory == "CC" && x.fldLadangID == GetEstateDetail.fld_ID) || (x.fldDepartment == DepartmentAM && x.fldCategory == "CC" && x.fldLadangID == GetEstateDetail.fld_ID) || (x.fldDepartment == DepartmentCL && x.fldCategory == "CC" && x.fldLadangID == GetEstateDetail.fld_ID) || (x.fldDepartment == DepartmentHR && x.fldCategory == "CC")) && x.fldDeleted == false).Select(s => new { s.fldEmail, s.fldName }).ToList();
                    if (emailcclist != null)
                    {
                        foreach (var ccemail in emailcclist)
                        {
                            cclist.Add(ccemail.fldEmail);
                        }
                        cc = cclist.ToArray();
                    }

                    var emailbcclist = db.tblEmailLists.Where(x => x.fldNegaraID == NegaraId && x.fldSyarikatID == SyarikatId && x.fldDepartment == "Developer" && x.fldCategory == "BCC" && x.fldDeleted == false).Select(s => new { s.fldEmail, s.fldName }).ToList();
                    if (emailbcclist != null)
                    {
                        foreach (var bccemail in emailbcclist)
                        {
                            bcclist.Add(bccemail.fldEmail);
                        }
                        bcc = bcclist.ToArray();
                    }

                    //SendEmailNotification.SendEmail(subject, msg, ToEmail.fldEmail, cc, bcc);
                    SendEmailNotification.SendEmail2(subject, msg, to, cc, bcc);

                    break;
                case "TolakWil":
                    DescStatus = "Telah Ditolak";
                    //DatabaseAction.UpdateDataTotblSokPermhnWang(DataID, 0, 1, 0, 0, 0, 0, "TolakWil", getuserid, getdatetime, 0, 0, "", "", "", 0);
                    DatabaseAction.UpdateDataTotblSokPermhnWang(DataID, 0, 1, 0, 0, 0, 0, "TolakWil", getuserid, getdatetime, "", "");
                    DatabaseAction.InsertDataTotblSokPermhnWangHisAction(DescStatus, getuserid, getdatetime, DataID, 1, SebabTolak);

                    //subject = "Penolakkan Permohonan Gaji";
                    subject = "FGVASSB - Penolakkan Permohonan Gaji (" + GetEstateDetail.fld_LdgName + ")"; //modified by faeza 24.12.2021

                    //var GetLdgID = db.tbl_SokPermhnWang.Where(x => x.fld_ID == DataID &&  x.fld_NegaraID == NegaraId && x.fld_SyarikatID == SyarikatId && x.fld_WilayahID == WilayahId).FirstOrDefault();
                    //var GetLdgDetail = db.tbl_Ladang.Where(x => x.fld_ID == GetLdgID.fld_LadangID && x.fld_WlyhID == WilayahId).FirstOrDefault();

                    msg = "<html>";
                    msg += "<body>";
                    msg += "<p>Assalamualaikum WBT & Salam sejahtera,</p>";
                    msg += "<p>Kepada Ladang " + GetEstateDetail.fld_LdgName + ",</p>";
                    msg += "<p>Dukacita dimaklumkan, permohonan gaji (Gaji Pekerja Buruh) telah ditolak oleh Pengurus. Mohon pihak ladang buat semakkan kembali. Keterangan seperti dibawah :-</p>";
                    msg += "<table border=\"1\">";
                    msg += "<thead>";
                    msg += "<tr>";
                    msg += "<th>Kod Ladang</th><th>Nama Ladang</th><th>Jumlah Permohonan(RM)</th><th>Sebab</th><th>Tindakan Oleh</th><th>Waktu Tindakan</th>";
                    msg += "</tr>";
                    msg += "</thead>";
                    msg += "<tbody>";
                    msg += "<tr>";
                    msg += "<td align=\"center\">" + GetEstateDetail.fld_LdgCode + "</td><td align=\"center\">" + GetEstateDetail.fld_LdgName + "</td><td align=\"center\">" + JumlahWang + "</td><td align=\"center\">" + SebabTolak + "</td><td align=\"center\">" + ActionBy + "</td><td align=\"center\">" + getdatetime + "</td>";
                    msg += "</tr>";
                    msg += "</tbody>";
                    msg += "</table>";
                    msg += "<p>Terima Kasih.</p>";
                    msg += "</body>";
                    msg += "</html>";

                    var ToEmailT = db.tblEmailLists.Where(x => x.fldNegaraID == NegaraId && x.fldSyarikatID == SyarikatId && x.fldWilayahID == WilayahId && x.fldLadangID == GetEstateDetail.fld_ID && ((x.fldDepartment == DepartmentCL && x.fldCategory == "TO") || x.fldDepartment == DepartmentAM && x.fldCategory == "TO") && x.fldDeleted == false).Select(s => new { s.fldEmail, s.fldName }).ToList();
                    if (ToEmailT != null)
                    {
                        foreach (var toemailt in ToEmailT)
                        {
                            tolist.Add(toemailt.fldEmail);
                        }
                        to = tolist.ToArray();
                    }

                    var CcEmailT = db.tblEmailLists.Where(x => x.fldNegaraID == NegaraId && x.fldSyarikatID == SyarikatId && ((x.fldDepartment == DepartmentHR && x.fldCategory == "CC") || (x.fldDepartment == DepartmentMGR && x.fldCategory == "CC" && x.fldWilayahID == WilayahId && x.fldLadangID == GetEstateDetail.fld_ID)) && x.fldDeleted == false).Select(s => new { s.fldEmail, s.fldName }).ToList();
                    if (CcEmailT != null)
                    {
                        foreach (var ccemailt in CcEmailT)
                        {
                            cclist.Add(ccemailt.fldEmail);
                        }
                        cc = cclist.ToArray();
                    }

                    var BccEmailT = db.tblEmailLists.Where(x => x.fldNegaraID == NegaraId && x.fldSyarikatID == SyarikatId && x.fldDepartment == "Developer" && x.fldCategory == "BCC" && x.fldDeleted == false).Select(s => new { s.fldEmail, s.fldName }).ToList();
                    if (BccEmailT != null)
                    {
                        foreach (var bccemailt in BccEmailT)
                        {
                            bcclist.Add(bccemailt.fldEmail);
                        }
                        bcc = bcclist.ToArray();
                    }

                    //SendEmailNotification.SendEmail(subject, msg, GetEstateDetail.fld_LdgEmail, cc, bcc);
                    SendEmailNotification.SendEmail2(subject, msg, to, null, bcc);

                    break;
            }
            return Json(new { DescStatus = DescStatus, ActionBy = ActionBy, getdatetime = getdatetime, SebabTolak = SebabTolak });
        }

        public JsonResult AcknowlagdeEmail(int NegaraId, int SyarikatId, int WilayahId, int Month, int Year)
        {
            string msg2 = "";
            string statusmsg = "";
            bool status = false;
            int getuserid = getidentity.ID(User.Identity.Name);
            string ActionBy = GetIdentity.MyNameFullName(getuserid);
            DateTime getdatetime = timezone.gettimezone();
            var getcoundata = db.vw_PermohonanWang.Where(x => x.fld_NegaraID == NegaraId && x.fld_SyarikatID == SyarikatId && x.fld_WilayahID == WilayahId && x.fld_Month == Month && x.fld_Year == Year && x.fld_SemakWil_Status == 1).Count();
            if (getcoundata > 0)
            {
                try
                {
                    string[] cc = new string[] { };
                    List<string> cclist = new List<string>();
                    string[] bcc = new string[] { };
                    List<string> bcclist = new List<string>();
                    string subject = "";
                    string msg = "";
                    string NamaWilayah = getwilyah.GetWilayahName(WilayahId);
                    var getJumlahKeseluruhanPermohonan = db.vw_PermohonanWang.Where(x => x.fld_NegaraID == NegaraId && x.fld_SyarikatID == SyarikatId && x.fld_WilayahID == WilayahId && x.fld_Month == Month && x.fld_Year == Year).Select(s => s.fld_JumlahPermohonan).Sum();

                    subject = "Sokongan Permohonan Wang";

                    var GetSentToGM = db2.tblUsers.Where(x => x.fldNegaraID == NegaraId && x.fldSyarikatID == SyarikatId && x.fldWilayahID == WilayahId && x.fldRoleID == 4).FirstOrDefault();

                    msg = "<html>";
                    msg += "<body>";
                    msg += "<p>Assalamualaikum WBT & Salam sejahtera,</p>";
                    msg += "<p>Tn/Pn " + GetSentToGM.fldUserShortName + ",</p>";
                    msg += "<p>Sokongan permohonan kewangan (Gaji Pekerja Buruh) untuk kelulusan diperlukan dari pihak Tn/Pn. Keterangan seperti dibawah:-</p>";
                    msg += "<table border=\"1\">";
                    msg += "<thead>";
                    msg += "<tr>";
                    msg += "<th>Nama Wilayah</th><th>Jumlah Permohonan(RM)</th><th>Disemak Oleh</th><th>Waktu Disemak</th><th>Pautan</th>";
                    msg += "</tr>";
                    msg += "</thead>";
                    msg += "<tbody>";
                    msg += "<tr>";
                    msg += "<td align=\"center\">" + NamaWilayah + "</td><td align=\"center\">" + getJumlahKeseluruhanPermohonan + "</td><td align=\"center\">" + ActionBy + "</td><td align=\"center\">" + getdatetime + "</td><td><a href=\"" + Url.Action("ApplicationSupportRegionGm", "ApplicationSupport", null, "http") + "\">Klik ke pautan sokongan</a></td>";
                    msg += "</tr>";
                    msg += "</tbody>";
                    msg += "</table>";
                    msg += "<p>Terima Kasih.</p>";
                    msg += "</body>";
                    msg += "</html>";

                    var emailbcclist1 = db.tblEmailLists.Where(x => x.fldNegaraID == NegaraId && x.fldSyarikatID == SyarikatId && x.fldCategory == "BCC" && x.fldDeleted == false).Select(s => new { s.fldEmail, s.fldName }).ToList();
                    if (emailbcclist1 != null)
                    {
                        foreach (var bccemail in emailbcclist1)
                        {
                            bcclist.Add(bccemail.fldEmail);
                        }
                        bcc = bcclist.ToArray();
                    }

                    if (SendEmailNotification.SendEmail(subject, msg, GetSentToGM.fldUserEmail, cc, bcc))
                    {
                        msg2 = "Email telah dihantar kepada GM Wilayah " + NamaWilayah;
                        statusmsg = "success";
                        status = true;
                    }
                    else
                    {
                        msg2 = "Email gagal dihantar kepada GM Wilayah " + NamaWilayah;
                        statusmsg = "warning";
                        status = true;
                    }
                }
                catch (Exception ex)
                {
                    geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                    msg2 = "Email gagal untuk dihantar";
                    statusmsg = "warning";
                    status = true;
                }
            }
            else
            {
                msg2 = "Sila buat semakan terlebih dahulu sebelum email dihantar kepada GM Wilayah";
                statusmsg = "warning";
                status = true;
            }
            return Json(new { msg = msg2, statusmsg = statusmsg, status = status });
        }

        //JTK
        public JsonResult UpdateDataGM(int LdgID, string UpdateFlag, int Month, int Year, int NegaraId, int SyarikatId, int WilayahId, string SebabTolak)
        {
            string DescStatus = "";
            int getuserid = getidentity.ID(User.Identity.Name);
            string ActionBy = GetIdentity.MyNameFullName(getuserid);
            string NamaWilayah = getwilyah.GetWilayahName(WilayahId);
            string subject = "";
            string msg = "";
            string DepartmentFMGR = "";
            string DepartmentAM = "";
            string DepartmentMGR = "";
            string DepartmentCL = "";
            string DepartmentHR = "";
            string[] to = new string[] { };
            List<string> tolist = new List<string>();
            string[] cc = new string[] { };
            List<string> cclist = new List<string>();
            string[] bcc = new string[] { };
            List<string> bcclist = new List<string>();
            DateTime getdatetime = timezone.gettimezone();
            int? SemakWilById;
            //var getdata = db.tbl_SokPermhnWang.Where(x => x.fld_WilayahID == DataID && x.fld_Month == Month && x.fld_Year == Year).Select(s=>s.fld_ID).ToList();
            var getdata = db.tbl_SokPermhnWang.Where(x => x.fld_NegaraID == NegaraId && x.fld_SyarikatID == SyarikatId && x.fld_WilayahID == WilayahId && x.fld_LadangID == LdgID && x.fld_Month == Month && x.fld_Year == Year).Select(s=>s.fld_ID).ToList();
            //var getJumlahKeseluruhanPermohonan = db.vw_PermohonanKewangan.Where(x => x.fld_NegaraID == NegaraId && x.fld_SyarikatID == SyarikatId && x.fld_WilayahID == WilayahId && x.fld_Month == Month && x.fld_Year == Year).Select(s => s.fld_JumlahPermohonan).Sum();

            var GetEstate = db.tbl_SokPermhnWang.Where(x => x.fld_LadangID == LdgID && x.fld_Month == Month && x.fld_Year == Year && x.fld_NegaraID == NegaraId && x.fld_SyarikatID == SyarikatId && x.fld_WilayahID == WilayahId).FirstOrDefault();
            var GetEstateDetail = db.tbl_Ladang.Where(x => x.fld_ID == LdgID && x.fld_NegaraID == NegaraId && x.fld_SyarikatID == SyarikatId && x.fld_WlyhID == WilayahId).FirstOrDefault();

            if (GetEstateDetail.fld_CostCentre == "FASSB")
            {
                DepartmentFMGR = "FMGR_FINANCE_APPROVAL_FASSB";
                DepartmentMGR = "MGR_FINANCE_APPROVAL_FASSB";
                DepartmentAM = "AM_FINANCE_APPROVAL_FASSB";
                DepartmentCL = "CL_FINANCE_APPROVAL_FASSB";
                DepartmentHR = "HR_FINANCE_APPROVAL_FASSB";
            }
            else
            {
                DepartmentFMGR = "FMGR_FINANCE_APPROVAL_RNDSB";
                DepartmentMGR = "MGR_FINANCE_APPROVAL_RNDSB";
                DepartmentAM = "AM_FINANCE_APPROVAL_RNDSB";
                DepartmentCL = "CL_FINANCE_APPROVAL_RNDSB";
                DepartmentHR = "HR_FINANCE_APPROVAL_RNDSB";
            }

            switch (UpdateFlag)
            {
                case "SokongGMWil":
                    DescStatus = "Telah Disokong";
                    DatabaseAction.UpdateDataTotblSokPermhnWangGM2(LdgID, UpdateFlag, Month, Year, getuserid, getdatetime);
                    foreach (var getdataid in getdata)
                    {
                        DatabaseAction.InsertDataTotblSokPermhnWangHisAction(DescStatus, getuserid, getdatetime, getdataid, 2, "");
                    }

                    //subject = "Kelulusan Permohonan Gaji";
                    subject = "FGVASSB - Kelulusan Permohonan Gaji (" + GetEstateDetail.fld_LdgName + ")"; //modified by faeza 24.12.2021

                    SemakWilById = GetEstate.fld_SemakWil_By;
                    //var GetSentToFinManager = db.tblEmailLists.Where(x => x.fldNegaraID == GetEstateDetail.fld_NegaraID && x.fldSyarikatID == GetEstateDetail.fld_SyarikatID && x.fldDepartment == DepartmentFGMR && x.fldCategory == "TO" && x.fldDeleted == false).Select(s => new { s.fldEmail, s.fldName }).FirstOrDefault();

                    //var GetSentToWil1 = db2.tbl_Wilayah.Where(x => x.fld_ID == DataID && x.fld_SyarikatID == SyarikatId).FirstOrDefault();
                    //var GetSentToHQ = db.tblEmailLists.Where(x => x.fldNegaraID == NegaraId && x.fldSyarikatID == SyarikatId && x.fldCategory == GetSentToWil1.fld_ApprovalZone && x.fldDeleted == false).Select(s => new { s.fldEmail, s.fldName }).FirstOrDefault();
                    //var GetSentCC = db.tblEmailLists.Where(x => x.fldNegaraID == NegaraId && x.fldSyarikatID == SyarikatId && x.fldCategory == "CCFINANCE" && x.fldDeleted == false).Select(s => new { s.fldEmail, s.fldName }).FirstOrDefault();

                    msg = "<html>";
                    msg += "<body>";
                    msg += "<p>Assalamualaikum WBT & Salam sejahtera,</p>";
                    msg += "<p>Tuan/Puan (Unit Kewangan),</p>";
                    msg += "<p>Mohon kelulusan permohonan gaji (Gaji Pekerja Buruh) dari pihak Tuan/Puan (Unit Kewangan). Keterangan seperti dibawah:-</p>";
                    msg += "<table border=\"1\">";
                    msg += "<thead>";
                    msg += "<tr>";
                    msg += "<th>Kod Ladang</th><th>Nama Ladang</th><th>Jumlah Permohonan(RM)</th><th>Disemak Oleh</th><th>Waktu Tindakan</th><th>Disokong Oleh</th><th>Waktu Tindakan</th><th>Pautan</th>";
                    msg += "</tr>";
                    msg += "</thead>";
                    msg += "<tbody>";
                    msg += "<tr>";
                    msg += "<td align=\"center\">" + GetEstateDetail.fld_LdgCode + "</td><td align=\"center\">" + GetEstateDetail.fld_LdgName + "</td><td align=\"center\">" + GetEstate.fld_JumlahPermohonan + "</td><td align=\"center\">" + GetIdentity.MyNameFullName(SemakWilById) + "</td><td align=\"center\">" + GetEstate.fld_SemakWil_DT + "</td><td align=\"center\">" + ActionBy + "</td><td align=\"center\">" + getdatetime + "</td><td align=\"center\"><a href=\"" + Url.Action("ApplicationSupportRegionHQ", "ApplicationSupport", null, this.Request.Url.Scheme) + "\">Klik ke pautan sokongan</a></td>";
                    msg += "</tr>";
                    msg += "</tbody>";
                    msg += "</table>";
                    msg += "<p>Terima Kasih.</p>";
                    msg += "</body>";
                    msg += "</html>";

                    var ToEmail = db.tblEmailLists.Where(x => x.fldNegaraID == GetEstateDetail.fld_NegaraID && x.fldSyarikatID == GetEstateDetail.fld_SyarikatID && x.fldDepartment == DepartmentFMGR && x.fldCategory == "TO" && x.fldDeleted == false).Select(s => new { s.fldEmail, s.fldName }).ToList();
                    if (ToEmail != null)
                    {
                        foreach (var toemail in ToEmail)
                        {
                            tolist.Add(toemail.fldEmail);
                        }
                        to = tolist.ToArray();
                    }

                    var CcEmail = db.tblEmailLists.Where(x => x.fldNegaraID == NegaraId && x.fldSyarikatID == SyarikatId && ((x.fldDepartment == DepartmentMGR && x.fldCategory == "CC" && x.fldWilayahID == WilayahId && x.fldLadangID == LdgID) || (x.fldDepartment == DepartmentAM && x.fldCategory == "CC" && x.fldWilayahID == WilayahId && x.fldLadangID == LdgID) || (x.fldDepartment == DepartmentCL && x.fldCategory == "CC" && x.fldWilayahID == WilayahId && x.fldLadangID == LdgID) || (x.fldDepartment == DepartmentHR && x.fldCategory == "CC") || (x.fldDepartment == DepartmentFMGR && x.fldCategory == "CC")) && x.fldDeleted == false).Select(s => new { s.fldEmail, s.fldName }).ToList();
                    if (CcEmail != null)
                    {
                        foreach (var ccemail in CcEmail)
                        {
                            cclist.Add(ccemail.fldEmail);
                        }
                        cc = cclist.ToArray();
                    }

                    var BccEmail = db.tblEmailLists.Where(x => x.fldNegaraID == NegaraId && x.fldSyarikatID == SyarikatId && x.fldDepartment == "Developer" && x.fldCategory == "BCC" && x.fldDeleted == false).Select(s => new { s.fldEmail, s.fldName }).ToList();
                    if (BccEmail != null)
                    {
                        foreach (var bccemail in BccEmail)
                        {
                            bcclist.Add(bccemail.fldEmail);
                        }
                        bcc = bcclist.ToArray();
                    }

                    //SendEmailNotification.SendEmail(subject, msg, GetSentToFinManager.fldEmail, cc, bcc);
                    SendEmailNotification.SendEmail2(subject, msg, to, cc, bcc);

                    break;
                case "TolakGMWil":
                    DescStatus = "Telah Ditolak";
                    DatabaseAction.UpdateDataTotblSokPermhnWangGM2(LdgID, UpdateFlag, Month, Year, getuserid, getdatetime);
                    foreach (var getdataid in getdata)
                    {
                        DatabaseAction.InsertDataTotblSokPermhnWangHisAction(DescStatus, getuserid, getdatetime, getdataid, 2, SebabTolak);
                    }

                    subject = "Penolakkan Permohonan Gaji";
                    subject = "FGVASSB - Penolakkan Permohonan Gaji (" + GetEstateDetail.fld_LdgName + ")"; //modified by faeza 24.12.2021

                    //var GetSentToWil = db2.tbl_Wilayah.Where(x => x.fld_ID == WilayahId && x.fld_SyarikatID == SyarikatId).FirstOrDefault();

                    msg = "<html>";
                    msg += "<body>";
                    msg += "<p>Assalamualaikum WBT & Salam sejahtera,</p>";
                    msg += "<p>Kepada Ladang " + GetEstateDetail.fld_LdgName + ",</p>";
                    msg += "<p>Dukacita dimaklumkan, permohonan gaji (Gaji Pekerja Buruh) telah ditolak oleh HR JTK. Mohon pihak ladang buat semakkan kembali. Keterangan seperti dibawah :-</p>";
                    msg += "<table border=\"1\">";
                    msg += "<thead>";
                    msg += "<tr>";
                    msg += "<th>Kod Ladang</th><th>Nama Ladang</th><th>Jumlah Permohonan(RM)</th><th>Sebab</th><th>Tindakan Oleh</th><th>Waktu Tindakan</th>";
                    msg += "</tr>";
                    msg += "</thead>";
                    msg += "<tbody>";
                    msg += "<tr>";
                    msg += "<td align=\"center\">" + GetEstateDetail.fld_LdgCode + "</td><td align=\"center\">" + GetEstateDetail.fld_LdgName + "</td><td align=\"center\">" + GetEstate.fld_JumlahPermohonan + "</td><td align=\"center\">" + SebabTolak + "</td><td align=\"center\">" + ActionBy + "</td><td align=\"center\">" + getdatetime + "</td>";
                    msg += "</tr>";
                    msg += "</tbody>";
                    msg += "</table>";
                    msg += "<p>Terima Kasih.</p>";
                    msg += "</body>";
                    msg += "</html>";

                    var ToEmailT = db.tblEmailLists.Where(x => x.fldNegaraID == NegaraId && x.fldSyarikatID == SyarikatId && x.fldWilayahID == WilayahId && x.fldLadangID == LdgID && ((x.fldDepartment == DepartmentCL && x.fldCategory == "TO") || x.fldDepartment == DepartmentAM && x.fldCategory == "TO") && x.fldDeleted == false).Select(s => new { s.fldEmail, s.fldName }).ToList();
                    if (ToEmailT != null)
                    {
                        foreach (var toemail in ToEmailT)
                        {
                            tolist.Add(toemail.fldEmail);
                        }
                        to = tolist.ToArray();
                    }
                    
                    var CcEmailT = db.tblEmailLists.Where(x => x.fldNegaraID == NegaraId && x.fldSyarikatID == SyarikatId && ((x.fldDepartment == DepartmentMGR && x.fldCategory == "CC" && x.fldWilayahID == WilayahId && x.fldLadangID == LdgID) || (x.fldDepartment == DepartmentHR && x.fldCategory == "CC")) && x.fldDeleted == false).Select(s => new { s.fldEmail, s.fldName }).ToList();
                    if (CcEmailT != null)
                    {
                        foreach (var ccemail in CcEmailT)
                        {
                            cclist.Add(ccemail.fldEmail);
                        }
                        cc = cclist.ToArray();
                    }

                    var BccEmailT = db.tblEmailLists.Where(x => x.fldNegaraID == NegaraId && x.fldSyarikatID == SyarikatId && x.fldDepartment == "Developer" && x.fldCategory == "BCC" && x.fldDeleted == false).Select(s => new { s.fldEmail, s.fldName }).ToList();
                    if (BccEmailT != null)
                    {
                        foreach (var bccemail in BccEmailT)
                        {
                            bcclist.Add(bccemail.fldEmail);
                        }
                        bcc = bcclist.ToArray();
                    }

                    //SendEmailNotification.SendEmail(subject, msg, GetEstateDetail.fld_LdgEmail, cc, bcc);
                    SendEmailNotification.SendEmail2(subject, msg, to, cc, bcc);
                    break;
            }
            return Json(new { DescStatus = DescStatus, ActionBy = ActionBy, getdatetime = getdatetime, SebabTolak = SebabTolak });
        }

        //Finance
        public JsonResult UpdateDataHQ(int DataID, string UpdateFlag, int Month, int Year, int NegaraId, int SyarikatId, int WilayahId, string SebabTolak)
        {
            string DescStatus = "";
            int getuserid = getidentity.ID(User.Identity.Name);
            string ActionBy = GetIdentity.MyNameFullName(getuserid);
            string subject = "";
            string msg = "";
            string DepartmentHR = "";
            string DepartmentAM = "";
            string DepartmentMGR = "";
            string DepartmentCL = "";
            string DepartmentFMGR = "";
            string[] to = new string[] { };
            List<string> tolist = new List<string>();
            string[] cc = new string[] { };
            List<string> cclist = new List<string>();
            string[] bcc = new string[] { };
            List<string> bcclist = new List<string>();
            DateTime getdatetime = timezone.gettimezone();
            string NamaWilayah = getwilyah.GetWilayahName(WilayahId);
            //var getdata = db.tbl_SokPermhnWang.Where(x => x.fld_WilayahID == DataID && x.fld_Month == Month && x.fld_Year == Year).Select(s => s.fld_ID).ToList();
            var getdata = db.tbl_SokPermhnWang.Where(x => x.fld_LadangID == DataID && x.fld_Month == Month && x.fld_Year == Year).Select(s => s.fld_ID).ToList();
            //var getJumlahKeseluruhanPermohonan = db.vw_PermohonanKewangan.Where(x => x.fld_NegaraID == NegaraId && x.fld_SyarikatID == SyarikatId && x.fld_WilayahID == DataID && x.fld_Month == Month && x.fld_Year == Year).Select(s => s.fld_JumlahPermohonan).Sum();

            var GetEstate = db.tbl_SokPermhnWang.Where(x => x.fld_LadangID == DataID && x.fld_Month == Month && x.fld_Year == Year && x.fld_NegaraID == NegaraId && x.fld_SyarikatID == SyarikatId && x.fld_WilayahID == WilayahId).FirstOrDefault();
            var GetEstateDetail = db.tbl_Ladang.Where(x => x.fld_ID == DataID && x.fld_NegaraID == NegaraId && x.fld_SyarikatID == SyarikatId && x.fld_WlyhID == WilayahId).FirstOrDefault();

            if (GetEstateDetail.fld_CostCentre == "FASSB")
            {
                DepartmentHR = "HR_FINANCE_APPROVAL_FASSB";
                DepartmentAM = "AM_FINANCE_APPROVAL_FASSB";
                DepartmentMGR = "MGR_FINANCE_APPROVAL_FASSB";
                DepartmentCL = "CL_FINANCE_APPROVAL_FASSB";
                DepartmentFMGR = "FMGR_FINANCE_APPROVAL_FASSB";
            }
            else
            {
                DepartmentHR = "HR_FINANCE_APPROVAL_RNDSB";
                DepartmentAM = "AM_FINANCE_APPROVAL_RNDSB";
                DepartmentMGR = "MGR_FINANCE_APPROVAL_RNDSB";
                DepartmentCL = "CL_FINANCE_APPROVAL_RNDSB";
                DepartmentFMGR = "FMGR_FINANCE_APPROVAL_RNDSB";
            }

            switch (UpdateFlag)
            {
                case "TerimaHQ":
                    DescStatus = "Telah Diterima";
                    DatabaseAction.UpdateDataTotblSokPermhnWangHQ2(DataID, UpdateFlag, Month, Year, getuserid, getdatetime);
                    foreach (var getdataid in getdata)
                    {
                        DatabaseAction.InsertDataTotblSokPermhnWangHisAction(DescStatus, getuserid, getdatetime, getdataid, 3, "");
                    }

                    //subject = "Kelulusan Permohonan Gaji";
                    subject = "FGVASSB - Kelulusan Permohonan Gaji (" + GetEstateDetail.fld_LdgName + ")"; //modified by faeza 24.12.2021

                    //var GetSentToLdg = db.tbl_Ladang.Where(x => x.fld_NegaraID == NegaraId && x.fld_SyarikatID == SyarikatId && x.fld_ID == GetEstateDetail.fld_ID).Select(s => new { s.fld_LdgEmail, s.fld_LdgName }).FirstOrDefault();

                    //var GetSentToWil = db2.tbl_Wilayah.Where(x => x.fld_ID == DataID && x.fld_SyarikatID == SyarikatId).FirstOrDefault();
                    //var GetSentToGM = db2.tblUsers.Where(x => x.fldNegaraID == NegaraId && x.fldSyarikatID == SyarikatId && x.fldWilayahID == DataID && x.fldRoleID == 4).FirstOrDefault();

                    msg = "<html>";
                    msg += "<body>";
                    msg += "<p>Assalamualaikum WBT & Salam sejahtera,</p>";
                    msg += "<p>Kepada Ladang " + GetEstateDetail.fld_LdgName + ",</p>";
                    msg += "<p>Sukacita dimaklumkan, permohonan gaji (Gaji Pekerja Buruh) telah diluluskan oleh Pengurus Kewangan. Keterangan seperti dibawah:-</p>";
                    msg += "<table border=\"1\">";
                    msg += "<thead>";
                    msg += "<tr>";
                    msg += "<th>Kod Ladang</th><th>Nama Ladang</th><th>Jumlah Permohonan(RM)</th><th>Diluluskan Oleh</th><th>Waktu Diluluskan</th>";
                    msg += "</tr>";
                    msg += "</thead>";
                    msg += "<tbody>";
                    msg += "<tr>";
                    msg += "<td align=\"center\">" + GetEstateDetail.fld_LdgCode + "</td><td align=\"center\">" + GetEstateDetail.fld_LdgName + "</td><td align=\"center\">" + GetEstate.fld_JumlahPermohonan + "</td><td align=\"center\">" + ActionBy + "</td><td align=\"center\">" + getdatetime + "</td>";
                    msg += "</tr>";
                    msg += "</tbody>";
                    msg += "</table>";
                    msg += "<p>Terima Kasih.</p>";
                    msg += "</body>";
                    msg += "</html>";

                    var ToEmail = db.tblEmailLists.Where(x => x.fldNegaraID == NegaraId && x.fldSyarikatID == SyarikatId && x.fldWilayahID == WilayahId && x.fldLadangID == DataID && ((x.fldDepartment == DepartmentMGR && x.fldCategory == "TO") || (x.fldDepartment == DepartmentAM && x.fldCategory == "TO") || (x.fldDepartment == DepartmentCL && x.fldCategory == "TO")) && x.fldDeleted == false).Select(s => new { s.fldEmail, s.fldName }).ToList();
                    if (ToEmail != null)
                    {
                        foreach (var toemail in ToEmail)
                        {
                            tolist.Add(toemail.fldEmail);
                        }
                        to = tolist.ToArray();
                    }

                    var CcEmail = db.tblEmailLists.Where(x => x.fldNegaraID == NegaraId && x.fldSyarikatID == SyarikatId && ((x.fldDepartment == DepartmentHR && x.fldCategory == "CC") || (x.fldDepartment == DepartmentFMGR && x.fldCategory == "CC") || (x.fldDepartment == DepartmentMGR && x.fldCategory == "CC" && x.fldWilayahID == WilayahId && x.fldLadangID == DataID)) && x.fldDeleted == false).Select(s => new { s.fldEmail, s.fldName }).ToList();
                    if (CcEmail != null)
                    {
                        foreach (var ccemail in CcEmail)
                        {
                            cclist.Add(ccemail.fldEmail);
                        }
                        cc = cclist.ToArray();
                    }

                    var BccEmail = db.tblEmailLists.Where(x => x.fldNegaraID == NegaraId && x.fldSyarikatID == SyarikatId && x.fldDepartment == "Developer" && x.fldCategory == "BCC" && x.fldDeleted == false).Select(s => new { s.fldEmail, s.fldName }).ToList();
                    if (BccEmail != null)
                    {
                        foreach (var bccemail in BccEmail)
                        {
                            bcclist.Add(bccemail.fldEmail);
                        }
                        bcc = bcclist.ToArray();
                    }

                    //SendEmailNotification.SendEmail(subject, msg, GetSentToLdg.fld_LdgEmail, cc, bcc);
                    SendEmailNotification.SendEmail2(subject, msg, to, cc, bcc);

                    break;
                case "TolakHQ":
                    DescStatus = "Telah Ditolak";
                    DatabaseAction.UpdateDataTotblSokPermhnWangHQ2(DataID, UpdateFlag, Month, Year, getuserid, getdatetime);
                    foreach (var getdataid in getdata)
                    {
                        DatabaseAction.InsertDataTotblSokPermhnWangHisAction(DescStatus, getuserid, getdatetime, getdataid, 3, SebabTolak);
                    }

                    //subject = "Penolakkan Permohonan Gaji";
                    subject = "FGVASSB - Penolakkan Permohonan Gaji (" + GetEstateDetail.fld_LdgName + ")"; //modified by faeza 24.12.2021

                    //var GetSentToLdgT = db.tbl_Ladang.Where(x => x.fld_NegaraID == NegaraId && x.fld_SyarikatID == SyarikatId && x.fld_ID == GetEstateDetail.fld_ID).Select(s => new { s.fld_LdgEmail, s.fld_LdgName }).FirstOrDefault();

                    msg = "<html>";
                    msg += "<body>";
                    msg += "<p>Assalamualaikum WBT & Salam sejahtera,</p>";
                    msg += "<p>Kepada Ladang " + GetEstateDetail.fld_LdgName + ",</p>";
                    msg += "<p>Dukacita dimaklumkan, permohonan gaji (Gaji Pekerja Buruh) telah ditolak oleh Pengurus Kewangan. Mohon pihak ladang buat semakkan kembali. Keterangan seperti dibawah:-</p>";
                    msg += "<table border=\"1\">";
                    msg += "<thead>";
                    msg += "<tr>";
                    msg += "<th>Kod Ladang</th><th>Nama Ladang</th><th>Jumlah Permohonan(RM)</th><th>Sebab</th><th>Tindakan Oleh</th><th>Waktu Tindakan</th>";
                    msg += "</tr>";
                    msg += "</thead>";
                    msg += "<tbody>";
                    msg += "<tr>";
                    msg += "<td align=\"center\">" + GetEstateDetail.fld_LdgCode + "</td><td align=\"center\">" + GetEstateDetail.fld_LdgName + "</td><td align=\"center\">" + GetEstate.fld_JumlahPermohonan + "</td><td align=\"center\">" + SebabTolak + "</td><td align=\"center\">" + ActionBy + "</td><td align=\"center\">" + getdatetime + "</td>";
                    msg += "</tr>";
                    msg += "</tbody>";
                    msg += "</table>";
                    msg += "<p>Terima Kasih.</p>";
                    msg += "</body>";
                    msg += "</html>";

                    var ToEmailT = db.tblEmailLists.Where(x => x.fldNegaraID == NegaraId && x.fldSyarikatID == SyarikatId && x.fldWilayahID == WilayahId && x.fldLadangID == DataID && ((x.fldDepartment == DepartmentAM && x.fldCategory == "TO") || (x.fldDepartment == DepartmentCL && x.fldCategory == "TO")) && x.fldDeleted == false).Select(s => new { s.fldEmail, s.fldName }).ToList();
                    if (ToEmailT != null)
                    {
                        foreach (var toemail in ToEmailT)
                        {
                            tolist.Add(toemail.fldEmail);
                        }
                        to = tolist.ToArray();
                    }

                    var CcEmailT = db.tblEmailLists.Where(x => x.fldNegaraID == NegaraId && x.fldSyarikatID == SyarikatId && ((x.fldDepartment == DepartmentMGR && x.fldCategory == "CC" && x.fldWilayahID == WilayahId && x.fldLadangID == DataID) || (x.fldDepartment == DepartmentHR && x.fldCategory == "CC") || (x.fldDepartment == DepartmentFMGR && x.fldCategory == "CC")) && x.fldDeleted == false).Select(s => new { s.fldEmail, s.fldName }).ToList();
                    if (CcEmailT != null)
                    {
                        foreach (var ccemail in CcEmailT)
                        {
                            cclist.Add(ccemail.fldEmail);
                        }
                        cc = cclist.ToArray();
                    }

                    var BccEmailT = db.tblEmailLists.Where(x => x.fldNegaraID == NegaraId && x.fldSyarikatID == SyarikatId && x.fldDepartment == "Developer" && x.fldCategory == "BCC" && x.fldDeleted == false).Select(s => new { s.fldEmail, s.fldName }).ToList();
                    if (BccEmailT != null)
                    {
                        foreach (var bccemail in BccEmailT)
                        {
                            bcclist.Add(bccemail.fldEmail);
                        }
                        bcc = bcclist.ToArray();
                    }

                    //SendEmailNotification.SendEmail(subject, msg, GetSentToLdgT.fld_LdgEmail, cc, bcc);
                    SendEmailNotification.SendEmail2(subject, msg, to, cc, bcc);

                    break;
            }
            return Json(new { DescStatus = DescStatus, ActionBy = ActionBy, getdatetime = getdatetime });
        }

        public ActionResult TransactionListingRptSearch(int NegaraID, int SyarikatID, int WilayahID, int LadangID, int Month, int Year)
        {
            //int? NegaraID = NegaraId;
            //int? SyarikatID = SyarikatId;
            //int? WilayahID = WilayahId;
            int? DivisionID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            //GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID, SyarikatID, NegaraID);
            DivisionID = GetNSWL.GetDivisionSelection(getuserid, NegaraID, SyarikatID, WilayahID, LadangID);
            MVC_SYSTEM_ViewingModels dbest = MVC_SYSTEM_ViewingModels.ConnectToSqlServer(host, catalog, user, pass);
            MVC_SYSTEM_ViewingModels dbest2 = new MVC_SYSTEM_ViewingModels();

            ViewBag.MonthList = Month;
            ViewBag.YearList = Year;

            ViewBag.NamaSyarikat = db.tbl_Syarikat
                .Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID)
                .Select(s => s.fld_NamaSyarikat)
                .FirstOrDefault();
            ViewBag.NoSyarikat = db.tbl_Syarikat
                .Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID)
                .Select(s => s.fld_NoSyarikat)
                .FirstOrDefault();
            ViewBag.NegaraID = NegaraID;
            ViewBag.SyarikatID = SyarikatID;
            //ViewBag.UserID = getuserid;
            //ViewBag.UserName = User.Identity.Name;
            ViewBag.Date = DateTime.Now.ToShortDateString();
            //ViewBag.NamaPengurus = dbest2.tbl_Ladang
            //    .Where(x => x.fld_ID == LadangID)
            //    .Select(s => s.fld_Pengurus).Single();
            //ViewBag.NamaPenyelia = dbest2.tblUsers
            //    .Where(x => x.fldUserID == getuserid)
            //    .Select(s => s.fldUserFullName).Single();
            //ViewBag.IDPenyelia = getuserid;


            //if (Month == 0 && Year == 0)
            //{
            //    ViewBag.Message = GlobalResEstateDetail.msgChooseMonthYear;
            //    //return View();
            //}

            //else
            //{
                var GetCotribution = db.tblOptionConfigsWebs.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldOptConfFlag3 == "Employee" && x.fldDeleted == false).Select(s => s.fldOptConfValue).ToList();

                var TransactionListingList = dbest.vw_RptSctran
                    .Where(x => !GetCotribution.Contains(x.fld_KodAktvt) && x.fld_Month == Month &&
                                x.fld_Year == Year && x.fld_NegaraID == NegaraID &&
                                x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID &&
                                x.fld_LadangID == LadangID && x.fld_DivisionID == DivisionID)
                    .OrderBy(o => new { o.fld_Kategori, o.fld_Amt }).ToList();

                if (!TransactionListingList.Any())
                {
                    ViewBag.Message = GlobalResEstateDetail.msgNoRecord;
                    return View();
                }

                ViewBag.UserID = getuserid;
                return View(TransactionListingList);
            //}
        }

        public ActionResult WorkerPaySheetRptSearch(int NegaraID, int SyarikatID, int WilayahID, int LadangID, int Month, int Year)
        {
            //int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? DivisionID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID, SyarikatID, NegaraID);
            DivisionID = GetNSWL.GetDivisionSelection(getuserid, NegaraID, SyarikatID, WilayahID, LadangID);
            MVC_SYSTEM_ViewingModels dbest = MVC_SYSTEM_ViewingModels.ConnectToSqlServer(host, catalog, user, pass);
            MVC_SYSTEM_ViewingModels dbest2 = new MVC_SYSTEM_ViewingModels();
            List<vw_PaySheetPekerjaCustomModel> PaySheetPekerjaList = new List<vw_PaySheetPekerjaCustomModel>();

            ViewBag.MonthList = Month;
            ViewBag.YearList = Year;
            //ViewBag.WorkerList = SelectionList;
            ViewBag.NamaSyarikat = db.tbl_Syarikat
                .Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID)
                .Select(s => s.fld_NamaSyarikat)
                .FirstOrDefault();
            ViewBag.NoSyarikat = db.tbl_Syarikat
                .Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID)
                .Select(s => s.fld_NoSyarikat)
                .FirstOrDefault();
            ViewBag.Ladang = db.tbl_Ladang
                .Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_ID == LadangID)
                .Select(s => s.fld_LdgName)
                .FirstOrDefault();
            ViewBag.NegaraID = NegaraID;
            ViewBag.SyarikatID = SyarikatID;
            ViewBag.UserID = getuserid;
            ViewBag.UserName = User.Identity.Name;
            ViewBag.Date = DateTime.Now.ToShortDateString();
            //ViewBag.NamaPengurus = dbview2.tbl_Ladang
            //    .Where(x => x.fld_ID == LadangID)
            //    .Select(s => s.fld_Pengurus).Single();
            //ViewBag.NamaPenyelia = dbview2.tblUsers
            //    .Where(x => x.fldUserID == getuserid)
            //    .Select(s => s.fldUserFullName).Single();
            //ViewBag.IDPenyelia = getuserid;
            //List<SelectListItem> JnsPkjList2 = new List<SelectListItem>();
            //JnsPkjList2 = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "jnsPkj" && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text", JnsPkjList).ToList();
            //JnsPkjList2.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));
            //ViewBag.JnsPkjList = JnsPkjList2;
            //ViewBag.Print = print;

            IOrderedQueryable<ViewingModelsOPMS.vw_PaySheetPekerja> salaryData;
            salaryData = dbest.vw_PaySheetPekerja
                .Where(x => x.fld_NegaraID == NegaraID &&
                            x.fld_Year == Year && x.fld_Month == Month &&
                            x.fld_SyarikatID == SyarikatID &&
                            x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_DivisionID == DivisionID)
                .OrderBy(x => x.fld_Nama);

            foreach (var salary in salaryData)
            {
                var workerAdditionalContribution = dbest.tbl_ByrCarumanTambahan
                    .Where(x => x.fld_GajiID == salary.fld_ID && x.fld_NegaraID == NegaraID &&
                                x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID &&
                                x.fld_LadangID == LadangID);
                List<CarumanTambahanCustomModel> carumanTambahanCustomModelList = new List<CarumanTambahanCustomModel>();
                foreach (var caruman in workerAdditionalContribution)
                {
                    CarumanTambahanCustomModel carumanTambahanCustomModel = new CarumanTambahanCustomModel();
                    carumanTambahanCustomModel.fld_ID = caruman.fld_ID;
                    carumanTambahanCustomModel.fld_KodCarumanTambahan = caruman.fld_KodSubCaruman;
                    carumanTambahanCustomModel.fld_CarumanMajikan = caruman.fld_CarumanMajikan;
                    carumanTambahanCustomModel.fld_CarumanPekerja = caruman.fld_CarumanPekerja;
                    carumanTambahanCustomModelList.Add(carumanTambahanCustomModel);
                }
                PaySheetPekerjaList.Add(
                    new vw_PaySheetPekerjaCustomModel()
                    {
                        PaySheetPekerja = salary,
                        CarumanTambahan = carumanTambahanCustomModelList
                    });
            }
            if (PaySheetPekerjaList.Count == 0)
            {
                ViewBag.Message = GlobalResEstateDetail.msgNoRecord;
            }
            return View(PaySheetPekerjaList);
           
        }

        public JsonResult GetLadang(int WilayahID)
        {
            List<SelectListItem> ladanglist = new List<SelectListItem>();

            int? NegaraID = 0;
            int? SyarikatID = 0;
            int? WilayahID2 = 0;
            int? LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID2, out LadangID, getuserid, User.Identity.Name);

            if (getwilyah.GetAvailableWilayah(SyarikatID))
            {
                if (WilayahID == 0)
                {
                    ladanglist = new SelectList(db2.vw_NSWL.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted_L  == false).OrderBy(o => o.fld_NamaLadang).Select(s => new SelectListItem { Value = s.fld_LadangID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_NamaLadang }), "Value", "Text").ToList();
                }
                else
                {
                    ladanglist = new SelectList(db2.vw_NSWL.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_Deleted_L == false).OrderBy(o => o.fld_NamaLadang).Select(s => new SelectListItem { Value = s.fld_LadangID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_NamaLadang }), "Value", "Text").ToList();
                }
            }

            return Json(ladanglist);
        }

        //Added by Shazana 9/3/2023
        public string GetSyarikatID3(int? WilayahID)
        {
            string SyarikatNamaPendek;
            int? SyarikatId;

            SyarikatId = db.tbl_Wilayah.Where(x => x.fld_ID == WilayahID).Select(s => s.fld_SyarikatID).FirstOrDefault();
            SyarikatNamaPendek = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatId).Select(s => s.fld_NamaPndkSyarikat).FirstOrDefault();

            return SyarikatNamaPendek;
        }

        //Added by Shazana 9/3/2023

        public JsonResult GetLadangbySyarikat(string SyarikatNamaPendek)
        {
            int? NegaraID = 0;
            int? SyarikatID2 = 0;
            int? WilayahID2 = 0;
            int? LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);

            GetNSWL.GetData(out NegaraID, out SyarikatID2, out WilayahID2, out LadangID, getuserid, User.Identity.Name);

            List<SelectListItem> LadangIDReturn = new List<SelectListItem>();

            if (SyarikatNamaPendek == "")
            {
                //fatin modified orderby from fld_LdgName to fld_LdgCode
                LadangIDReturn = new SelectList(db.tbl_Ladang.Where(x => x.fld_NegaraID == NegaraID && x.fld_Deleted == false).OrderBy(o => o.fld_LdgCode).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
            }
            else
            {
                //fatin modified orderby from fld_LdgName to fld_LdgCode
                LadangIDReturn = new SelectList(db.tbl_Ladang.Where(x => x.fld_NegaraID == NegaraID && x.fld_CostCentre == SyarikatNamaPendek && x.fld_Deleted == false).OrderBy(o => o.fld_LdgCode).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
            }

            return Json(LadangIDReturn);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
                db2.Dispose();
            }
            base.Dispose(disposing);
        }

    }
}