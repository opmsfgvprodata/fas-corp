﻿using MVC_SYSTEM.App_LocalResources;
using MVC_SYSTEM.Attributes;
using MVC_SYSTEM.AuthModels;
using MVC_SYSTEM.Class;
using MVC_SYSTEM.log;
using MVC_SYSTEM.Models;
using MVC_SYSTEM.ModelsCorporate;
using MVC_SYSTEM.ModelsEstate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity;
using MVC_SYSTEM.ModelsSP;
using MVC_SYSTEM.ViewingModels;
using MVC_SYSTEM.ModelsCustom;
using Dapper;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;

namespace MVC_SYSTEM.Controllers
{

    public class ReportController : Controller
    {
        private ChangeTimeZone timezone = new ChangeTimeZone();
        private MVC_SYSTEM_Models db = new MVC_SYSTEM_Models();
        private MVC_SYSTEM_Auth db2 = new MVC_SYSTEM_Auth();
        //private MVC_SYSTEM_SP_Models db3 = new MVC_SYSTEM_SP_Models();
        //private MVC_SYSTEM_SP_Models db4 = new MVC_SYSTEM_SP_Models();
        private MVC_SYSTEM_ModelsCorporate db5 = new MVC_SYSTEM_ModelsCorporate();
        private GetIdentity getidentity = new GetIdentity();
        errorlog geterror = new errorlog();
        GetWilayah getwilyah = new GetWilayah();
        GetNSWL GetNSWL = new GetNSWL();
        GetConfig GetConfig = new GetConfig();
        ConvertToPdf ConvertToPdf = new ConvertToPdf();
        GetTriager GetTriager = new GetTriager();
        
        private MVC_SYSTEM_ModelsEstate dbE = new MVC_SYSTEM_ModelsEstate();
        private GetIdentity GetIdentity = new GetIdentity();
        private Connection Connection = new Connection();
        private GlobalFunction GlobalFunction = new GlobalFunction();

        //new Models
        private MVC_SYSTEM_ModelsCorporate dbC = new MVC_SYSTEM_ModelsCorporate();
        private MVC_SYSTEM_SP2_Models dbSP = new MVC_SYSTEM_SP2_Models();

        // GET: Report
        [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3,Resource,Viewer")]
        public ActionResult Index()
        {
            int? getuserid = getidentity.ID(User.Identity.Name);
            int? getroleid = getidentity.getRoleID(getuserid);
            int?[] reportid = new int?[] { };

            int? NegaraID = 0;
            int? SyarikatID = 0;
            int? WilayahID = 0;
            int? LadangID = 0;

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            ViewBag.Report = "class = active";
            reportid = dbC.tblRoleReports.Where(x => x.fldRoleID == getroleid && x.fldNegaraID == NegaraID && x.fldSyarikatID == SyarikatID).Select(s => s.fldReportID).ToArray();

            List<SelectListItem> SubReportList = new List<SelectListItem>();
            if (!getidentity.NegaraSumber(User.Identity.Name))
            {
                ViewBag.ReportList = new SelectList(dbC.tblReportLists.Where(x => reportid.Contains((x.fldReportListID)) && x.fldDeleted == false).OrderBy(o => o.fldReportListID).Select(s => new SelectListItem { Value = s.fldReportListID.ToString(), Text = s.fldReportListName }), "Value", "Text").ToList();
            }
            else
            {
                ViewBag.ReportList = new SelectList(dbC.tblReportLists.Where(x => reportid.Contains((x.fldReportListID)) && x.fldDeleted == false).OrderBy(o => o.fldReportListID).Select(s => new SelectListItem { Value = s.fldReportListID.ToString(), Text = s.fldReportListName }), "Value", "Text").ToList();
            }
            ViewBag.SubReportList = SubReportList;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3,Resource,Viewer")]
        public ActionResult Index(int ReportList, int SubReportList)
        {
            string action = "", controller = "";
            var getreport = dbC.tblReportLists.Find(ReportList);

            if (getreport.fldSubReport == true && SubReportList > 0)
            {
                var getsubreport = dbC.tblSubReportLists.Where(x => x.fldSubReportListID == SubReportList && x.fldMainReportID == ReportList).FirstOrDefault();
                action = getsubreport.fldSubReportListAction;
                controller = getsubreport.fldSubReportListController;
            }
            else
            {
                action = getreport.fldReportListAction;
                controller = getreport.fldReportListController;
            }
            return RedirectToAction(action, controller);

        }

        [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3,Viewer")]
        public ActionResult ActiveWorker()
        {
            int[] wlyhid = new int[] { };
            int? NegaraID = 0;
            int? SyarikatID = 0;
            int? WilayahID = 0;
            int? LadangID = 0;
            int year = timezone.gettimezone().Year;
            int drpyear = 0;
            int drprangeyear = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            int? wilayahselection = 0;
            int? ladangselection = 0;
            int incldg = 0;
            string appname = Request.ApplicationPath;
            string domain = Request.Url.GetLeftPart(UriPartial.Authority);

            if (appname != "/")
            {
                domain = domain + appname;
            }

            ViewBag.Report = "class = active";

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            List<SelectListItem> WilayahIDList = new List<SelectListItem>();
            List<SelectListItem> LadangIDList = new List<SelectListItem>();

            if (WilayahID == 0 && LadangID == 0)
            {
                wlyhid = getwilyah.GetWilayahID(SyarikatID);
                WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
                WilayahIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
                LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
                LadangIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
            }
            else if (WilayahID != 0 && LadangID == 0)
            {
                wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
                WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
                LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
                wilayahselection = WilayahID;
                incldg = 1;

            }
            else if (WilayahID != 0 && LadangID != 0)
            {
                wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
                WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
                LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_ID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();

                wilayahselection = WilayahID;
                ladangselection = LadangID;
                incldg = 1;
            }

            drpyear = timezone.gettimezone().Year - int.Parse(GetConfig.GetData("yeardisplay")) + 1;
            drprangeyear = timezone.gettimezone().Year;

            var yearlist = new List<SelectListItem>();
            for (var i = drpyear; i <= drprangeyear; i++)
            {
                if (i == timezone.gettimezone().Year)
                {
                    yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString(), Selected = true });
                }
                else
                {
                    yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
                }
            }

            List<SelectListItem> status = new List<SelectListItem>();

            //status.Add(new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" });
            status.Add(new SelectListItem { Text = GlobalResReport.sltActive, Value = "1", Selected = true });
            status.Add(new SelectListItem { Text = GlobalResReport.sltNotActive, Value = "2" });

            ViewBag.Status = status;
            ViewBag.Incldg = incldg;
            ViewBag.YearList = yearlist;
            ViewBag.Year = year;
            ViewBag.WilayahIDList = WilayahIDList;
            ViewBag.LadangIDList = LadangIDList;
            ViewBag.LadangID = 0;
            ViewBag.ladangvalue = LadangID;
            ViewBag.UserID = getuserid;
            ViewBag.NamaSyarikat = db2.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
            ViewBag.NoSyarikat = db2.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();
            ViewBag.Link = domain;
            ViewBag.Status1 = 1;

            List<sp_RptRumKedKepPekLad_Result> resultreport = new List<sp_RptRumKedKepPekLad_Result>();

            return View("ActiveWorker", resultreport);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3,Viewer")]
        public ActionResult ActiveWorker(int? YearList, int WilayahIDList, int LadangIDList, int Status)
        {
            int[] wlyhid = new int[] { };
            int? NegaraID = 0;
            int? SyarikatID = 0;
            int? WilayahID = 0;
            int? LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            int drpyear = 0;
            int drprangeyear = 0;
            bool activestatus0 = false, activestatus1 = false, activestatus2 = false;
            int? wilayahselection = 0;
            int? ladangselection = 0;
            int incldg = 0;
            int incldg2 = 0;
            string appname = Request.ApplicationPath;
            string domain = Request.Url.GetLeftPart(UriPartial.Authority);

            if (appname != "/")
            {
                domain = domain + appname;
            }

            ViewBag.Report = "class = active";

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            List<SelectListItem> WilayahIDList2 = new List<SelectListItem>();
            List<SelectListItem> LadangIDList2 = new List<SelectListItem>();

            if (WilayahID == 0 && LadangID == 0)
            {
                wlyhid = getwilyah.GetWilayahID(SyarikatID);
                WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName", WilayahID).ToList();
                WilayahIDList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
                if (WilayahIDList == 0)
                {
                    LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
                }
                else
                {
                    LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => x.fld_WlyhID == WilayahIDList && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
                }
                LadangIDList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));

                wilayahselection = WilayahIDList;
                ladangselection = LadangIDList;
            }
            else if (WilayahID != 0 && LadangID == 0)
            {
                wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
                WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName", WilayahID).ToList();
                if (WilayahIDList == 0)
                {
                    LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
                }
                else
                {
                    LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => x.fld_WlyhID == WilayahIDList && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
                }
                LadangIDList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));

                wilayahselection = WilayahIDList;
                ladangselection = LadangIDList;
            }
            else if (WilayahID != 0 && LadangID != 0)
            {
                wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
                WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName", WilayahID).ToList();
                LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_ID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();

                wilayahselection = WilayahIDList;
                ladangselection = LadangIDList;
            }

            drpyear = timezone.gettimezone().Year - int.Parse(GetConfig.GetData("yeardisplay")) + 1;
            drprangeyear = timezone.gettimezone().Year;

            var yearlist = new List<SelectListItem>();
            for (var i = drpyear; i <= drprangeyear; i++)
            {
                if (i == YearList)
                {
                    yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString(), Selected = true });
                }
                else
                {
                    yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
                }
            }

            List<sp_RptRumKedKepPekLad_Result> resultreport = new List<sp_RptRumKedKepPekLad_Result>();

            if (WilayahIDList == 0)
            {
                dbSP.SetCommandTimeout(120);
                resultreport = dbSP.sp_RptRumKedKepPekLad(NegaraID, SyarikatID, WilayahIDList, LadangIDList, Status, getuserid).ToList();
            }
            else
            {
                if (LadangIDList == 0)
                {
                    dbSP.SetCommandTimeout(120);
                    resultreport = dbSP.sp_RptRumKedKepPekLad(NegaraID, SyarikatID, WilayahIDList, LadangIDList, Status, getuserid).ToList();
                    incldg = 1;
                }
                else
                {
                    dbSP.SetCommandTimeout(120);
                    resultreport = dbSP.sp_RptRumKedKepPekLad(NegaraID, SyarikatID, WilayahIDList, LadangIDList, Status, getuserid).ToList();
                    incldg = 1;
                }
            }

            ViewBag.WilayahName = db2.tbl_Wilayah.Where(x => x.fld_ID == WilayahIDList && x.fld_SyarikatID == SyarikatID).Select(s => s.fld_WlyhName).FirstOrDefault();
            ViewBag.LadangCode = db2.tbl_Ladang.Where(x => x.fld_ID == LadangIDList && x.fld_WlyhID == WilayahIDList).Select(s => s.fld_LdgCode).FirstOrDefault();
            ViewBag.LadangName = db2.tbl_Ladang.Where(x => x.fld_ID == LadangIDList && x.fld_WlyhID == WilayahIDList).Select(s => s.fld_LdgName).FirstOrDefault();

            if (WilayahIDList == 0)
            {
                incldg2 = 1;
            }
            else
            {
                if (LadangIDList == 0)
                {
                    incldg2 = 2;
                }
                else
                {
                    incldg2 = 3;
                }
            }

            switch (Status)
            {
                case 0:
                    activestatus0 = true;
                    break;
                case 1:
                    activestatus1 = true;
                    break;
                case 2:
                    activestatus2 = true;
                    break;
            }

            List<SelectListItem> status = new List<SelectListItem>();

            //status.Add(new SelectListItem { Text = GlobalResReport.sltAll, Value = "2", Selected = activestatus2 });
            status.Add(new SelectListItem { Text = GlobalResReport.sltActive, Value = "1", Selected = activestatus1 });
            status.Add(new SelectListItem { Text = GlobalResReport.sltNotActive, Value = "2", Selected = activestatus2 });

            ViewBag.Status = status;
            ViewBag.Incldg = incldg;
            ViewBag.Incldg2 = incldg2;
            ViewBag.YearList = yearlist; // list dalam dropdown
            ViewBag.Year = YearList; // year yg user select
            ViewBag.WilayahIDList = WilayahIDList2;
            ViewBag.LadangIDList = LadangIDList2;
            ViewBag.LadangID = LadangIDList;
            ViewBag.ladangvalue = LadangID;
            ViewBag.UserID = getuserid;
            ViewBag.Link = domain;
            ViewBag.Status1 = Status;

            ViewBag.NamaSyarikat = db2.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
            ViewBag.NoSyarikat = db2.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();

            return View("ActiveWorker", resultreport);
        }

        public ActionResult ActiveWorker2()
        {
            int[] wlyhid = new int[] { };
            int? NegaraID = 0;
            int? SyarikatID = 0;
            int? WilayahID = 0;
            int? LadangID = 0;
            int year = timezone.gettimezone().Year;
            int drpyear = 0;
            int drprangeyear = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            int? wilayahselection = 0;
            int? ladangselection = 0;
            int incldg = 0;
            string appname = Request.ApplicationPath;
            string domain = Request.Url.GetLeftPart(UriPartial.Authority);

            if (appname != "/")
            {
                domain = domain + appname;
            }

            ViewBag.Report = "class = active";

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            List<SelectListItem> WilayahIDList = new List<SelectListItem>();
            List<SelectListItem> LadangIDList = new List<SelectListItem>();

            if (WilayahID == 0 && LadangID == 0)
            {
                wlyhid = getwilyah.GetWilayahID(SyarikatID);
                WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
                WilayahIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
                LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
                LadangIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
            }
            else if (WilayahID != 0 && LadangID == 0)
            {
                wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
                WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
                LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
                wilayahselection = WilayahID;
                incldg = 1;

            }
            else if (WilayahID != 0 && LadangID != 0)
            {
                wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
                WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
                LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_ID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();

                wilayahselection = WilayahID;
                ladangselection = LadangID;
                incldg = 1;
            }

            drpyear = timezone.gettimezone().Year - int.Parse(GetConfig.GetData("yeardisplay")) + 1;
            drprangeyear = timezone.gettimezone().Year;

            var yearlist = new List<SelectListItem>();
            for (var i = drpyear; i <= drprangeyear; i++)
            {
                if (i == timezone.gettimezone().Year)
                {
                    yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString(), Selected = true });
                }
                else
                {
                    yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
                }
            }

            List<SelectListItem> status = new List<SelectListItem>();

            //status.Add(new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" });
            status.Add(new SelectListItem { Text = GlobalResReport.sltActive, Value = "1", Selected = true });
            status.Add(new SelectListItem { Text = GlobalResReport.sltNotActive, Value = "2" });

            ViewBag.Status = status;
            ViewBag.Incldg = incldg;
            ViewBag.YearList = yearlist;
            ViewBag.Year = year;
            ViewBag.WilayahIDList = WilayahIDList;
            ViewBag.LadangIDList = LadangIDList;
            ViewBag.LadangID = 0;
            ViewBag.ladangvalue = LadangID;
            ViewBag.UserID = getuserid;
            ViewBag.NamaSyarikat = db2.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
            ViewBag.NoSyarikat = db2.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();
            ViewBag.Link = domain;
            ViewBag.Status1 = 1;

            List<sp_RptRumKedKepPekLad_Result> resultreport = new List<sp_RptRumKedKepPekLad_Result>();

            return View("ActiveWorker2", resultreport);
        }

        [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3,Viewer")]
        public ActionResult ActiveWorkerGMN()
        {
            int[] wlyhid = new int[] { };
            int? NegaraID = 0;
            int? SyarikatID = 0;
            int? WilayahID = 0;
            int? LadangID = 0;
            int year = timezone.gettimezone().Year;
            int drpyear = 0;
            int drprangeyear = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            int? wilayahselection = 0;
            int? ladangselection = 0;
            int incldg = 0;
            string appname = Request.ApplicationPath;
            string domain = Request.Url.GetLeftPart(UriPartial.Authority);

            if (appname != "/")
            {
                domain = domain + appname;
            }

            ViewBag.Report = "class = active";

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            List<SelectListItem> WilayahIDList = new List<SelectListItem>();
            List<SelectListItem> LadangIDList = new List<SelectListItem>();

            if (WilayahID == 0 && LadangID == 0)
            {
                wlyhid = getwilyah.GetWilayahID(SyarikatID);
                WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
                WilayahIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
                LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
                LadangIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
            }
            else if (WilayahID != 0 && LadangID == 0)
            {
                wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
                WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
                LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
                wilayahselection = WilayahID;
                incldg = 1;

            }
            else if (WilayahID != 0 && LadangID != 0)
            {
                wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
                WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
                LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_ID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();

                wilayahselection = WilayahID;
                ladangselection = LadangID;
                incldg = 1;
            }

            drpyear = timezone.gettimezone().Year - int.Parse(GetConfig.GetData("yeardisplay")) + 1;
            drprangeyear = timezone.gettimezone().Year;

            var yearlist = new List<SelectListItem>();
            for (var i = drpyear; i <= drprangeyear; i++)
            {
                if (i == timezone.gettimezone().Year)
                {
                    yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString(), Selected = true });
                }
                else
                {
                    yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
                }
            }

            List<SelectListItem> status = new List<SelectListItem>();

            //status.Add(new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" });
            status.Add(new SelectListItem { Text = GlobalResReport.sltActive, Value = "1", Selected = true });
            status.Add(new SelectListItem { Text = GlobalResReport.sltNotActive, Value = "2" });

            ViewBag.Status = status;
            ViewBag.Incldg = incldg;
            ViewBag.YearList = yearlist;
            ViewBag.Year = year;
            ViewBag.WilayahIDList = WilayahIDList;
            ViewBag.LadangIDList = LadangIDList;
            ViewBag.LadangID = 0;
            ViewBag.ladangvalue = LadangID;
            ViewBag.UserID = getuserid;
            ViewBag.NamaSyarikat = db2.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
            ViewBag.NoSyarikat = db2.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();
            ViewBag.Link = domain;
            ViewBag.Status1 = 1;

            List<sp_RptRumKedKepPekLad_Result> resultreport = new List<sp_RptRumKedKepPekLad_Result>();

            return View("ActiveWorkerGMN", resultreport);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3,Viewer")]
        public ActionResult ActiveWorkerGMN(int? YearList, int WilayahIDList, int LadangIDList, int Status)
        {
            int[] wlyhid = new int[] { };
            int? NegaraID = 0;
            int? SyarikatID = 0;
            int? WilayahID = 0;
            int? LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            int drpyear = 0;
            int drprangeyear = 0;
            bool activestatus0 = false, activestatus1 = false, activestatus2 = false;
            int? wilayahselection = 0;
            int? ladangselection = 0;
            int incldg = 0;
            int incldg2 = 0;
            string appname = Request.ApplicationPath;
            string domain = Request.Url.GetLeftPart(UriPartial.Authority);

            if (appname != "/")
            {
                domain = domain + appname;
            }

            ViewBag.Report = "class = active";

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            List<SelectListItem> WilayahIDList2 = new List<SelectListItem>();
            List<SelectListItem> LadangIDList2 = new List<SelectListItem>();

            if (WilayahID == 0 && LadangID == 0)
            {
                wlyhid = getwilyah.GetWilayahID(SyarikatID);
                WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName", WilayahID).ToList();
                WilayahIDList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
                if (WilayahIDList == 0)
                {
                    LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
                }
                else
                {
                    LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => x.fld_WlyhID == WilayahIDList && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
                }
                LadangIDList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));

                wilayahselection = WilayahIDList;
                ladangselection = LadangIDList;
            }
            else if (WilayahID != 0 && LadangID == 0)
            {
                wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
                WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName", WilayahID).ToList();
                if (WilayahIDList == 0)
                {
                    LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
                }
                else
                {
                    LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => x.fld_WlyhID == WilayahIDList && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
                }
                LadangIDList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));

                wilayahselection = WilayahIDList;
                ladangselection = LadangIDList;
            }
            else if (WilayahID != 0 && LadangID != 0)
            {
                wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
                WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName", WilayahID).ToList();
                LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_ID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();

                wilayahselection = WilayahIDList;
                ladangselection = LadangIDList;
            }

            drpyear = timezone.gettimezone().Year - int.Parse(GetConfig.GetData("yeardisplay")) + 1;
            drprangeyear = timezone.gettimezone().Year;

            var yearlist = new List<SelectListItem>();
            for (var i = drpyear; i <= drprangeyear; i++)
            {
                if (i == YearList)
                {
                    yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString(), Selected = true });
                }
                else
                {
                    yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
                }
            }

            List<sp_RptRumKedKepPekLad_Result> resultreport = new List<sp_RptRumKedKepPekLad_Result>();

            if (WilayahIDList == 0)
            {
                dbSP.SetCommandTimeout(120);
                resultreport = dbSP.sp_RptRumKedKepPekLad(NegaraID, SyarikatID, WilayahIDList, LadangIDList, Status, getuserid).ToList();
            }
            else
            {
                if (LadangIDList == 0)
                {
                    dbSP.SetCommandTimeout(120);
                    resultreport = dbSP.sp_RptRumKedKepPekLad(NegaraID, SyarikatID, WilayahIDList, LadangIDList, Status, getuserid).ToList();
                    incldg = 1;
                }
                else
                {
                    dbSP.SetCommandTimeout(120);
                    resultreport = dbSP.sp_RptRumKedKepPekLad(NegaraID, SyarikatID, WilayahIDList, LadangIDList, Status, getuserid).ToList();
                    incldg = 1;
                }
            }

            ViewBag.WilayahName = db2.tbl_Wilayah.Where(x => x.fld_ID == WilayahIDList && x.fld_SyarikatID == SyarikatID).Select(s => s.fld_WlyhName).FirstOrDefault();
            ViewBag.LadangCode = db2.tbl_Ladang.Where(x => x.fld_ID == LadangIDList && x.fld_WlyhID == WilayahIDList).Select(s => s.fld_LdgCode).FirstOrDefault();
            ViewBag.LadangName = db2.tbl_Ladang.Where(x => x.fld_ID == LadangIDList && x.fld_WlyhID == WilayahIDList).Select(s => s.fld_LdgName).FirstOrDefault();

            if (WilayahIDList == 0)
            {
                incldg2 = 1;
            }
            else
            {
                if (LadangIDList == 0)
                {
                    incldg2 = 2;
                }
                else
                {
                    incldg2 = 3;
                }
            }

            switch (Status)
            {
                case 0:
                    activestatus0 = true;
                    break;
                case 1:
                    activestatus1 = true;
                    break;
                case 2:
                    activestatus2 = true;
                    break;
            }

            List<SelectListItem> status = new List<SelectListItem>();

            //status.Add(new SelectListItem { Text = GlobalResReport.sltAll, Value = "2", Selected = activestatus2 });
            status.Add(new SelectListItem { Text = GlobalResReport.sltActive, Value = "1", Selected = activestatus1 });
            status.Add(new SelectListItem { Text = GlobalResReport.sltNotActive, Value = "2", Selected = activestatus2 });

            ViewBag.Status = status;
            ViewBag.Incldg = incldg;
            ViewBag.Incldg2 = incldg2;
            ViewBag.YearList = yearlist; // list dalam dropdown
            ViewBag.Year = YearList; // year yg user select
            ViewBag.WilayahIDList = WilayahIDList2;
            ViewBag.LadangIDList = LadangIDList2;
            ViewBag.LadangID = LadangIDList;
            ViewBag.ladangvalue = LadangID;
            ViewBag.UserID = getuserid;
            ViewBag.Link = domain;
            ViewBag.Status1 = Status;

            ViewBag.NamaSyarikat = db2.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
            ViewBag.NoSyarikat = db2.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();

            return View("ActiveWorkerGMN", resultreport);
        }

        [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3,Viewer")]
        public ActionResult WorkerSalary()
        {
            int[] wlyhid = new int[] { };
            int? NegaraID = 0;
            int? SyarikatID = 0;
            int? WilayahID = 0;
            int? LadangID = 0;
            int year = timezone.gettimezone().Year;
            int month = timezone.gettimezone().AddMonths(-1).Month;
            int drpyear = 0;
            int drprangeyear = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            int? wilayahselection = 0;
            int? ladangselection = 0;

            ViewBag.Report = "class = active";

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            List<SelectListItem> WilayahIDList = new List<SelectListItem>();
            List<SelectListItem> LadangIDList = new List<SelectListItem>();

            if (WilayahID == 0 && LadangID == 0)
            {
                wlyhid = getwilyah.GetWilayahID(SyarikatID);
                WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
                WilayahIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
                LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
                LadangIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
                wilayahselection = WilayahID;
                ladangselection = LadangID;
            }
            else if (WilayahID != 0 && LadangID == 0)
            {
                wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
                WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
                LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
                LadangIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
                wilayahselection = WilayahID;
                ladangselection = LadangID;

            }
            else if (WilayahID != 0 && LadangID != 0)
            {
                wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
                WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
                LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_ID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
                wilayahselection = WilayahID;
                ladangselection = LadangID;
            }

            drpyear = timezone.gettimezone().Year - int.Parse(GetConfig.GetData("yeardisplay")) + 1;
            drprangeyear = timezone.gettimezone().Year;

            var yearlist = new List<SelectListItem>();
            for (var i = drpyear; i <= drprangeyear; i++)
            {
                if (i == timezone.gettimezone().Year)
                {
                    yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString(), Selected = true });
                }
                else
                {
                    yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
                }
            }

            List<SelectListItem> KerakyatanList = new List<SelectListItem>();
            List<ModelsSP.sp_RptBulPenPekLad_Result> resultreport = new List<ModelsSP.sp_RptBulPenPekLad_Result>();
            KerakyatanList = new SelectList(dbC.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "krytnlist" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false), "fldOptConfValue", "fldOptConfDesc").ToList();
            KerakyatanList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "ALL" }));

            ViewBag.MonthList = new SelectList(dbC.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "monthlist" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false), "fldOptConfValue", "fldOptConfDesc", month);
            ViewBag.YearList = yearlist;
            ViewBag.WilayahIDList = WilayahIDList;
            ViewBag.LadangIDList = LadangIDList;
            ViewBag.KerakyatanList = KerakyatanList;

            ViewBag.UserID = getuserid;
            ViewBag.GetFlag = 1;

            return View(resultreport);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3,Viewer")]
        public ActionResult WorkerSalary(int? MonthList, int YearList, int WilayahIDList, int LadangIDList, string KerakyatanList)
        {
            int[] wlyhid = new int[] { };
            int? NegaraID = 0;
            int? SyarikatID = 0;
            int? WilayahID = 0;
            int? LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            int drpyear = 0;
            int drprangeyear = 0;
            int? wilayahselection = 0;
            int? ladangselection = 0;
            int incldg = 0;

            ViewBag.Report = "class = active";

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            List<SelectListItem> WilayahIDList2 = new List<SelectListItem>();
            List<SelectListItem> LadangIDList2 = new List<SelectListItem>();

            if (WilayahID == 0 && LadangID == 0)
            {
                wlyhid = getwilyah.GetWilayahID(SyarikatID);
                WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName", WilayahID).ToList();
                WilayahIDList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
                if (WilayahIDList == 0)
                {
                    LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
                }
                else
                {
                    LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => x.fld_WlyhID == WilayahIDList && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
                }
                LadangIDList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
                wilayahselection = WilayahIDList;
                ladangselection = LadangIDList;
            }
            else if (WilayahID != 0 && LadangID == 0)
            {
                wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
                WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName", WilayahID).ToList();
                if (WilayahIDList == 0)
                {
                    LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
                }
                else
                {
                    LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => x.fld_WlyhID == WilayahIDList && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
                }
                LadangIDList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
                wilayahselection = WilayahIDList;
                ladangselection = LadangIDList;
            }
            else if (WilayahID != 0 && LadangID != 0)
            {
                wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
                WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName", WilayahID).ToList();
                LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_ID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
                wilayahselection = WilayahIDList;
                ladangselection = LadangIDList;
            }

            drpyear = timezone.gettimezone().Year - int.Parse(GetConfig.GetData("yeardisplay")) + 1;
            drprangeyear = timezone.gettimezone().Year;

            var yearlist = new List<SelectListItem>();
            for (var i = drpyear; i <= drprangeyear; i++)
            {
                if (i == YearList)
                {
                    yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString(), Selected = true });
                }
                else
                {
                    yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
                }
            }

            List<SelectListItem> KerakyatanList2 = new List<SelectListItem>();
            KerakyatanList2 = new SelectList(dbC.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "krytnlist" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false), "fldOptConfValue", "fldOptConfDesc", KerakyatanList).ToList();
            KerakyatanList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "ALL" }));

            if (WilayahIDList == 0)
            {
                incldg = 1;
            }
            else
            {
                if (LadangIDList == 0)
                {
                    incldg = 2;
                }
                else
                {
                    incldg = 3;
                }
            }

            ViewBag.MonthList = new SelectList(dbC.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "monthlist" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false), "fldOptConfValue", "fldOptConfDesc", MonthList);

            List<ModelsSP.sp_RptBulPenPekLad_Result> resultreport = new List<ModelsSP.sp_RptBulPenPekLad_Result>();

            dbSP.SetCommandTimeout(120);
            resultreport = dbSP.sp_RptBulPenPekLad(NegaraID, SyarikatID, WilayahIDList, LadangIDList, KerakyatanList, MonthList, YearList, getuserid).ToList();


            ViewBag.YearList = yearlist;
            ViewBag.WilayahIDList = WilayahIDList2;
            ViewBag.LadangIDList = LadangIDList2;
            ViewBag.KerakyatanList = KerakyatanList2;
            ViewBag.NeragaID = NegaraID;
            ViewBag.SyarikatID = SyarikatID;
            ViewBag.WilayahSelection = WilayahIDList;
            ViewBag.LadangSelection = LadangIDList;
            ViewBag.IncLdg = incldg;
            ViewBag.UserID = getuserid;
            ViewBag.GetFlag = 2;

            ViewBag.Month = MonthList;
            ViewBag.Year = YearList;

            ViewBag.NamaSyarikat = db2.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
            ViewBag.NoSyarikat = db2.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();

            return View(resultreport);
        }

        [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3,Viewer")]
        public ActionResult MasterDataPkjReport()
        {
            ViewBag.Report = "class = active";
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            List<SelectListItem> statusList = new List<SelectListItem>();
            statusList = new SelectList(
                db.tblOptionConfigsWeb
                    .Where(x => x.fldOptConfFlag1 == "statusaktif" && x.fld_NegaraID == NegaraID &&
                                x.fld_SyarikatID == SyarikatID && x.fldDeleted == false)
                    .OrderBy(o => o.fldOptConfDesc)
                    .Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }),
                "Value", "Text").ToList();
            statusList.Insert(0, (new SelectListItem { Text = "Semua", Value = "" }));

            ViewBag.StatusList = statusList;

            List<SelectListItem> wilayahList = new List<SelectListItem>();

            wilayahList = new SelectList(
                db.tbl_Wilayah
                    .Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false)
                    .OrderBy(o => o.fld_WlyhName)
                    .Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_WlyhName }),
                "Value", "Text").ToList();
            wilayahList.Insert(0, (new SelectListItem { Text = "Semua", Value = "0" }));

            ViewBag.WilayahList = wilayahList;

            List<SelectListItem> ladangList = new List<SelectListItem>();

            ladangList = new SelectList(
                db.tbl_Ladang
                    .Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false)
                    .OrderBy(o => o.fld_LdgName)
                    .Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgName }),
                "Value", "Text").ToList();
            ladangList.Insert(0, (new SelectListItem { Text = "Semua", Value = "0" }));

            ViewBag.LadangList = ladangList;

            List<SelectListItem> kategoriPekerjaList = new List<SelectListItem>();
            kategoriPekerjaList = new SelectList(
                db.tblOptionConfigsWeb
                    .Where(x => x.fldOptConfFlag1 == "designation" && x.fld_NegaraID == NegaraID &&
                                x.fld_SyarikatID == SyarikatID && x.fldDeleted == false)
                    .OrderBy(o => o.fldOptConfDesc)
                    .Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }),
                "Value", "Text").ToList();
            kategoriPekerjaList.Insert(0, (new SelectListItem { Text = "Semua", Value = "" }));

            ViewBag.KategoriPekerjaList = kategoriPekerjaList;

            List<SelectListItem> kerakyatanList = new List<SelectListItem>();
            kerakyatanList = new SelectList(
                db.tblOptionConfigsWeb
                    .Where(x => x.fldOptConfFlag1 == "krytnlist" && x.fld_NegaraID == NegaraID &&
                                x.fld_SyarikatID == SyarikatID && x.fldDeleted == false)
                    .OrderBy(o => o.fldOptConfDesc)
                    .Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }),
                "Value", "Text").ToList();
            kerakyatanList.Insert(0, (new SelectListItem { Text = "Semua", Value = "" }));

            ViewBag.KerakyatanList = kerakyatanList;

            return View();
        }

        [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3,Viewer")]
        public ViewResult _MasterDataPkjReport(string StatusList, int? WilayahList, int? LadangList, string KategoriPekerjaList, string KerakyatanList, string filter, string print)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            List<sp_RptMasterDataPkj_Result> rptMasterDataPkjResults = new List<sp_RptMasterDataPkj_Result>();

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
            ViewBag.UserID = getuserid;
            ViewBag.UserName = User.Identity.Name;
            ViewBag.Date = DateTime.Now.ToShortDateString();
            ViewBag.Print = print;

            if (String.IsNullOrEmpty(WilayahList.ToString()) || String.IsNullOrEmpty(LadangList.ToString()))
            {
                ViewBag.Message = @GlobalResCorp.lblChooseWorkerMasterDataReport;
            }

            else
            {
                if (!String.IsNullOrEmpty(filter))
                {
                    rptMasterDataPkjResults = dbSP.sp_RptMasterDataPkj(NegaraID, SyarikatID, WilayahList, LadangList,
                            KerakyatanList, StatusList, KategoriPekerjaList, getuserid)
                        .Where(x => x.fld_Nama.ToUpper().Contains(filter.ToUpper()) ||
                                    x.fld_NoPkj.ToUpper().Contains(filter.ToUpper()) ||
                                    x.fld_NoKP.ToUpper().Contains(filter.ToUpper())).ToList();
                }

                else
                {
                    rptMasterDataPkjResults = dbSP.sp_RptMasterDataPkj(NegaraID, SyarikatID, WilayahList, LadangList,
                            KerakyatanList, StatusList, KategoriPekerjaList, getuserid)
                        .ToList();
                }

                if (rptMasterDataPkjResults.Count == 0)
                {
                    ViewBag.Message = @GlobalResCorp.msgNoRecord;
                }
            }

            return View(rptMasterDataPkjResults);
        }

        [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3,Viewer")]
        public ActionResult GajiMinimaReport()
        {
            ViewBag.Report = "class = active";
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            List<SelectListItem> wilayahList = new List<SelectListItem>();

            wilayahList = new SelectList(
                db.tbl_Wilayah
                    .Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false)
                    .OrderBy(o => o.fld_WlyhName)
                    .Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_WlyhName }),
                "Value", "Text").ToList();
            wilayahList.Insert(0, (new SelectListItem { Text = "Semua", Value = "0" }));

            ViewBag.WilayahList = wilayahList;

            List<SelectListItem> ladangList = new List<SelectListItem>();

            ladangList = new SelectList(
                db.tbl_Ladang
                    .Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false)
                    .OrderBy(o => o.fld_LdgName)
                    .Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgName }),
                "Value", "Text").ToList();
            ladangList.Insert(0, (new SelectListItem { Text = "Semua", Value = "0" }));

            ViewBag.LadangList = ladangList;

            int drpyear = 0;
            int drprangeyear = 0;

            drpyear = timezone.gettimezone().Year - int.Parse(GetConfig.GetData("yeardisplay")) + 1;
            drprangeyear = timezone.gettimezone().Year;

            var yearlist = new List<SelectListItem>();
            for (var i = drpyear; i <= drprangeyear; i++)
            {
                if (i == timezone.gettimezone().Year)
                {
                    yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString(), Selected = true });
                }
                else
                {
                    yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
                }
            }

            ViewBag.YearList = yearlist;

            int month = timezone.gettimezone().Month;

            ViewBag.MonthList = new SelectList(
                db.tblOptionConfigsWeb.Where(x => x.fldOptConfFlag1 == "monthlist" && x.fldDeleted == false &&
                                                   x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID),
                "fldOptConfValue", "fldOptConfDesc", month - 1);

            return View();
        }

        [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3,Viewer")]
        public ViewResult _GajiMinimaReport(int? WilayahList, int? LadangList, int? MonthList, int? YearList, string print)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            List<sp_RptGajiMinima_Result> rptGajiMinimaResults = new List<sp_RptGajiMinima_Result>();

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
            ViewBag.UserID = getuserid;
            ViewBag.UserName = User.Identity.Name;
            ViewBag.Date = DateTime.Now.ToShortDateString();
            ViewBag.Print = print;

            if (String.IsNullOrEmpty(WilayahList.ToString()) || String.IsNullOrEmpty(LadangList.ToString()))
            {
                ViewBag.Message = @GlobalResCorp.lblChooseMinimumWageReport;
            }

            else
            {
                rptGajiMinimaResults = dbSP.sp_RptGajiMinima(NegaraID, SyarikatID, WilayahList, LadangList, MonthList, YearList, getuserid)
                    .ToList();

                if (rptGajiMinimaResults.Count == 0)
                {
                    ViewBag.Message = @GlobalResCorp.msgNoRecord;
                }
            }

            return View(rptGajiMinimaResults);
        }

        [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3,Viewer")]
        public ActionResult SocsoFileGen()
        {
            int[] wlyhid = new int[] { };
            int? NegaraID = 0;
            int? SyarikatID = 0;
            int? WilayahID = 0;
            int? LadangID = 0;
            int year = timezone.gettimezone().Year;
            int month = timezone.gettimezone().AddMonths(-1).Month;
            int drpyear = 0;
            int drprangeyear = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            int? wilayahselection = 0;
            int? ladangselection = 0;

            ViewBag.Report = "class = active";

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            List<SelectListItem> CompanyListList = new List<SelectListItem>();
            List<SelectListItem> TypeContribution = new List<SelectListItem>();
            
            CompanyListList.Add(new SelectListItem { Text = "FGV R&D Sdn Bhd", Value = "RNDSB", Selected = false });
            CompanyListList.Add(new SelectListItem { Text = "FGV Agri Services Sdn Bhd", Value = "FASSB", Selected = false });

            TypeContribution.Add(new SelectListItem { Text = "SOCSO", Value = "1", Selected = false });
            TypeContribution.Add(new SelectListItem { Text = "SIP", Value = "2", Selected = false });

            drpyear = timezone.gettimezone().Year - int.Parse(GetConfig.GetData("yeardisplay")) + 1;
            drprangeyear = timezone.gettimezone().Year;

            var yearlist = new List<SelectListItem>();
            for (var i = drpyear; i <= drprangeyear; i++)
            {
                if (i == timezone.gettimezone().Year)
                {
                    yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString(), Selected = true });
                }
                else
                {
                    yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
                }
            }

            List<SelectListItem> wilayahList = new List<SelectListItem>();
            wilayahList = new SelectList(
               db.tbl_Wilayah
                   .Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).OrderBy(o => o.fld_WlyhName)
                   .Select(
                       s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_WlyhName }), "Value", "Text").ToList();
            wilayahList.Insert(0, new SelectListItem { Text = "all", Value = "0" });


            List<SelectListItem> kerakyatan = new List<SelectListItem>();
            kerakyatan = new SelectList(dbC.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "jnsPkj" &&
            x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).OrderBy(o => o.fldOptConfValue)
            .Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();
          

            //List<ModelsSP.sp_RptRumKedKepPekLad_Result> resultreport = new List<ModelsSP.sp_RptRumKedKepPekLad_Result>();
            ViewBag.MonthList = new SelectList(dbC.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "monthlist" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false), "fldOptConfValue", "fldOptConfDesc", month);
            ViewBag.YearList = yearlist;
            ViewBag.CompanyListList = CompanyListList;
            ViewBag.TypeContribution = TypeContribution;
            ViewBag.UserID = getuserid;
            ViewBag.wilayahList = wilayahList;
            ViewBag.kerakyatan = kerakyatan;
            return View();
        }

        [HttpPost]
        public ActionResult SocsoFileGen(int? Month, int Year, string CompanyListList, int? wilayahlist, string kerakyatan, int TypeContribution, int? WilayahList, string EmployeeCode)
        {
            GetGenerateFile GetGenerateFile = new GetGenerateFile();
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string msg = "";
            string statusmsg = "";
            string filePath = "";
            string filename = "";

            string stringyear = "";
            string stringmonth = "";
            string link = "";
            stringyear = Year.ToString();
            stringmonth = Month.ToString();
            stringmonth = (stringmonth.Length == 1 ? "0" + stringmonth : stringmonth);

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            List<ModelsSP.sp_SocsoFileGen_Result> SocsoDetails = new List<ModelsSP.sp_SocsoFileGen_Result>();

            try
            {
                SocsoDetails = dbSP.sp_SocsoFileGen(NegaraID, SyarikatID, wilayahlist,  CompanyListList, kerakyatan, EmployeeCode, TypeContribution, Month, Year, getuserid).ToList();
                
                filePath = GetGenerateFile.GenFileMaybank(SocsoDetails, stringmonth, stringyear, out filename);

                link = Url.Action("Download", "Report", new { filePath, filename });

                dbSP.Dispose();

                msg ="Success to generate";
                statusmsg = "success";
            }
            catch (Exception ex)
            {
                geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                msg = "Please refer developer";
                statusmsg = "warning";
            }

            return Json(new { msg, statusmsg, link });
        }

        public FileResult Download(string filePath, string filename)
        {
            string path = HttpContext.Server.MapPath(filePath);

            DownloadFiles.FileDownloads objs = new DownloadFiles.FileDownloads();

            var filesCol = objs.GetFiles(path);
            var CurrentFileName = filesCol.Where(x => x.FileName == filename).FirstOrDefault();

            string contentType = string.Empty;
            contentType = "application/txt";
            return File(CurrentFileName.FilePath, contentType, CurrentFileName.FileName);
        }

        //Added by Shazana on 16/1     
        public JsonResult GetLadangList(int WilayahIDParam)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            List<SelectListItem> LadangIDReturn = new List<SelectListItem>();

            var GetLadangList = db.tbl_Ladang.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WlyhID == WilayahIDParam && x.fld_Deleted == false).ToList();
            LadangIDReturn = new SelectList(GetLadangList.Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode.Trim() + " - " + s.fld_LdgName.Trim() }), "Value", "Text").ToList();
            LadangIDReturn.Insert(0, (new SelectListItem { Text = "All", Value = "0" }));

            return Json(LadangIDReturn);
        }
        //Close Added by Shazana on 16/1

        [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3,Viewer")]
        public ActionResult WorkerContribution()
        {
            int[] wlyhid = new int[] { };
            int? NegaraID = 0;
            int? SyarikatID = 0;
            int? WilayahID = 0;
            int? LadangID = 0;
            int year = timezone.gettimezone().Year;
            int month = timezone.gettimezone().AddMonths(-1).Month;
            int drpyear = 0;
            int drprangeyear = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            int? wilayahselection = 0;
            int? ladangselection = 0;

            ViewBag.Report = "class = active";

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            List<SelectListItem> CompanyListList = new List<SelectListItem>();

            CompanyListList.Add(new SelectListItem { Text = "FGV R&D Sdn Bhd", Value = "RNDSB", Selected = false });
            CompanyListList.Add(new SelectListItem { Text = "FGV Agri Services Sdn Bhd", Value = "FASSB", Selected = false });
            
            drpyear = timezone.gettimezone().Year - int.Parse(GetConfig.GetData("yeardisplay")) + 1;
            drprangeyear = timezone.gettimezone().Year;

            var yearlist = new List<SelectListItem>();
            for (var i = drpyear; i <= drprangeyear; i++)
            {
                if (i == timezone.gettimezone().Year)
                {
                    yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString(), Selected = true });
                }
                else
                {
                    yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
                }
            }

            List<SelectListItem> wilayahList = new List<SelectListItem>();
            wilayahList = new SelectList(
               db.tbl_Wilayah
                   .Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).OrderBy(o => o.fld_WlyhName)
                   .Select(
                       s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_WlyhName }), "Value", "Text").ToList();
            wilayahList.Insert(0, new SelectListItem { Text = GlobalResCorp.lblChoose, Value = "0" });

            //Added by Shazana on 16/1
            List<SelectListItem> ladangList = new List<SelectListItem>();
            ladangList = new SelectList(
               db.tbl_Ladang
                   .Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WlyhID == WilayahID && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName)
                   .Select(
                       s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgName }), "Value", "Text").ToList();
            ladangList.Insert(0, new SelectListItem { Text = GlobalResCorp.lblChoose, Value = "0" });
            ViewBag.ladangList = ladangList;
            //Close Added by Shazana on 16/1

            //List<ModelsSP.sp_RptRumKedKepPekLad_Result> resultreport = new List<ModelsSP.sp_RptRumKedKepPekLad_Result>();
            ViewBag.MonthList = new SelectList(dbC.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "monthlist" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false), "fldOptConfValue", "fldOptConfDesc", month);
            ViewBag.YearList = yearlist;
            ViewBag.CompanyListList = wilayahList;
            ViewBag.CompanyListList = CompanyListList;
            ViewBag.UserID = getuserid;
            ViewBag.WilayahList = wilayahList;

            return View();
        }

        //Commented by Shazana on 16/1
        //public ViewResult _WorkerContribution(int? MonthList, int? YearList, string CompanyListList, int? wilayahlist)
        //Added by Shazana on 16/1
        public ViewResult _WorkerContribution(int? MonthList, int? YearList, string CompanyListList, int? wilayahlist, int? ladanglist)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? DivisionID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            DateTime? CurrentDT = timezone.gettimezone();
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            //Added by Shazana on 16/1
            var GetLadangDetail = db.vw_NSWL.Where(x => x.fld_Deleted_L == false && x.fld_CostCentre == CompanyListList).ToList();
            if (ladanglist != 0)
            {
                GetLadangDetail = db.vw_NSWL.Where(x => x.fld_Deleted_L == false && x.fld_CostCentre == CompanyListList && x.fld_LadangID == ladanglist).ToList();
            }
            //Close Added by Shazana on 16/1

            var GetLadangID = GetLadangDetail.Select(s => s.fld_LadangID).ToList();
            var GetWilLists = GetLadangDetail.Select(s => s.fld_WilayahID).Distinct().ToList();
            var GetInsetifEffectCode = db5.tbl_JenisInsentif.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_JenisInsentif == "P" && x.fld_AdaCaruman == true && x.fld_Deleted == false).Select(s => s.fld_KodInsentif).ToList();
            List<ContributionReport> ContributionReportList = new List<ContributionReport>();

            foreach (var GetWilList in GetWilLists)
            {
                Connection.GetConnection(out host, out catalog, out user, out pass, GetWilList, SyarikatID.Value, NegaraID.Value);
                MVC_SYSTEM_ModelsEstate EstateConn = MVC_SYSTEM_ModelsEstate.ConnectToSqlServer(host, catalog, user, pass);
                DivisionID = GetNSWL.GetDivisionSelection(getuserid, NegaraID, SyarikatID, WilayahID, LadangID);
                List<ModelsEstate.tbl_GajiBulanan> tbl_GajiBulananList = new List<ModelsEstate.tbl_GajiBulanan>();
                List<ModelsEstate.tbl_Insentif> tbl_InsentifList = new List<ModelsEstate.tbl_Insentif>();
                List<ModelsEstate.tbl_Pkjmast> tbl_PkjmastList = new List<ModelsEstate.tbl_Pkjmast>();
                List<tbl_ByrCarumanTambahan> tbl_ByrCarumanTambahanList = new List<tbl_ByrCarumanTambahan>();
               
                decimal? TotalInsentifEfected = 0;
                decimal? TotalSalaryForKWSP = 0;
                decimal? TotalSalaryForPerkeso = 0;
                decimal? KWSPEmplyee = 0;
                decimal? KWSPEmplyer = 0;
                decimal? SocsoEmplyee = 0;
                decimal? SocsoEmplyer = 0;
                decimal? SIPEmplyee = 0;
                decimal? SIPEmplyer = 0;
                decimal? SBKPEmplyee = 0;
                decimal? SBKPEmplyer = 0;
                int ID = 1;
                string WorkerName = "";
                string WorkerIDNo = "";
                string WorkerSoscoNo = "";
                string WilayahName = "";
                string LadangName = "";

                tbl_PkjmastList = EstateConn.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == GetWilList && GetLadangID.Contains(x.fld_LadangID.Value) && x.fld_Kdaktf == "1").ToList();

                var GetNoPkjas = tbl_PkjmastList.Select(s => s.fld_Nopkj).ToList();
                tbl_GajiBulananList = EstateConn.tbl_GajiBulanan.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == wilayahlist && GetLadangID.Contains(x.fld_LadangID.Value) && GetNoPkjas.Contains(x.fld_Nopkj) && x.fld_Month == MonthList && x.fld_Year == YearList).OrderBy(o => o.fld_LadangID).ToList();
                tbl_InsentifList = EstateConn.tbl_Insentif.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == wilayahlist && GetNoPkjas.Contains(x.fld_Nopkj) && GetLadangID.Contains(x.fld_LadangID.Value) && GetInsetifEffectCode.Contains(x.fld_KodInsentif) && x.fld_Deleted == false && x.fld_Month == MonthList && x.fld_Year == YearList).ToList();

                var GetGajiBulananID = tbl_GajiBulananList.Select(s => s.fld_ID).ToList();
                tbl_ByrCarumanTambahanList = EstateConn.tbl_ByrCarumanTambahan.Where(x => GetGajiBulananID.Contains(x.fld_GajiID.Value)).ToList();
                
                foreach (var GajiBulananDetail in tbl_GajiBulananList)
                {
                    KWSPEmplyee = 0;
                    KWSPEmplyer = 0;
                    SocsoEmplyee = 0;
                    SocsoEmplyer = 0;
                    SIPEmplyee = 0;
                    SIPEmplyer = 0;
                    SBKPEmplyee = 0;
                    SBKPEmplyer = 0;

                    TotalInsentifEfected = tbl_InsentifList.Where(x => x.fld_Nopkj == GajiBulananDetail.fld_Nopkj).Sum(s => s.fld_NilaiInsentif);
                    TotalInsentifEfected = TotalInsentifEfected == null ? 0 : TotalInsentifEfected;

                    TotalSalaryForKWSP = GajiBulananDetail.fld_ByrKerja + GajiBulananDetail.fld_ByrCuti + GajiBulananDetail.fld_BonusHarian + TotalInsentifEfected + GajiBulananDetail.fld_AIPS + GajiBulananDetail.fld_ByrKwsnSkr;
                    TotalSalaryForPerkeso = GajiBulananDetail.fld_ByrKerja + GajiBulananDetail.fld_ByrCuti + GajiBulananDetail.fld_OT + TotalInsentifEfected + GajiBulananDetail.fld_AIPS + GajiBulananDetail.fld_ByrKwsnSkr;

                    KWSPEmplyee = GajiBulananDetail.fld_KWSPPkj;
                    KWSPEmplyer = GajiBulananDetail.fld_KWSPMjk;

                    SocsoEmplyee = GajiBulananDetail.fld_SocsoPkj;
                    SocsoEmplyer = GajiBulananDetail.fld_SocsoMjk;

                    var GetAddContribution = tbl_ByrCarumanTambahanList.Where(x => x.fld_GajiID == GajiBulananDetail.fld_ID).FirstOrDefault();

                    if (GetAddContribution != null)
                    {
                        if (GetAddContribution.fld_KodCaruman == "SIP")
                        {
                            SIPEmplyee = GetAddContribution.fld_CarumanPekerja;
                            SIPEmplyer = GetAddContribution.fld_CarumanMajikan;
                        }
                        else
                        {
                            SBKPEmplyee = GetAddContribution.fld_CarumanPekerja;
                            SBKPEmplyer = GetAddContribution.fld_CarumanMajikan;
                        }
                    }

                    WorkerName = tbl_PkjmastList.Where(x => x.fld_Nopkj == GajiBulananDetail.fld_Nopkj).Select(s => s.fld_Nama).FirstOrDefault();
                    WorkerIDNo = tbl_PkjmastList.Where(x => x.fld_Nopkj == GajiBulananDetail.fld_Nopkj).Select(s => s.fld_Nokp).FirstOrDefault();
                    WorkerSoscoNo = tbl_PkjmastList.Where(x => x.fld_Nopkj == GajiBulananDetail.fld_Nopkj).Select(s => s.fld_Noperkeso).FirstOrDefault();
                    WilayahName = GetLadangDetail.Where(x => x.fld_LadangID == GajiBulananDetail.fld_LadangID).Select(s => s.fld_NamaWilayah).FirstOrDefault();
                    LadangName = GetLadangDetail.Where(x => x.fld_LadangID == GajiBulananDetail.fld_LadangID).Select(s => s.fld_NamaLadang).FirstOrDefault();
                    if (SBKPEmplyer != 0 || SocsoEmplyer != 0 || KWSPEmplyer != 0)
                    {
                        ContributionReportList.Add(new ContributionReport() { ID = ID, WorkerName = WorkerName, TotalSalaryForKwsp = TotalSalaryForKWSP.Value, TotalSalaryForPerkeso = TotalSalaryForPerkeso.Value, KwspContributionEmplyee = KWSPEmplyee.Value, KwspContributionEmplyer = KWSPEmplyer.Value, SipContributionEmplyee = SIPEmplyee.Value, SipContributionEmplyer = SIPEmplyer.Value, SocsoContributionEmplyee = SocsoEmplyee.Value, SocsoContributionEmplyer = SocsoEmplyer.Value, SbkpContributionEmplyee = SBKPEmplyee.Value, SbkpContributionEmplyer = SBKPEmplyer.Value, WorkerNo = GajiBulananDetail.fld_Nopkj, WorkerIDNo = WorkerIDNo, NegaraID = GajiBulananDetail.fld_NegaraID.Value, SyarikatID = GajiBulananDetail.fld_SyarikatID.Value, WilayahID = GajiBulananDetail.fld_WilayahID.Value, LadangID = GajiBulananDetail.fld_LadangID.Value, WorkerSocsoNo = WorkerSoscoNo, WilayahName = WilayahName, LadangName = LadangName });
                        ID++;
                    }
                }

                EstateConn.Dispose();
            }
            
            return View(ContributionReportList);
        }

        //added by faeza 09.06.2021
        public ActionResult PaymentMode()
        {
            int[] wlyhid = new int[] { };
            int? NegaraID = 0;
            int? SyarikatID = 0;
            int? WilayahID = 0;
            int? LadangID = 0;
            int year = timezone.gettimezone().Year;
            int month = timezone.gettimezone().AddMonths(-1).Month;
            int drpyear = 0;
            int drprangeyear = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);

            ViewBag.Report = "class = active";

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            List<SelectListItem> CompanyListList = new List<SelectListItem>();

            CompanyListList.Add(new SelectListItem { Text = "All", Value = "0", Selected = false });
            CompanyListList.Add(new SelectListItem { Text = "FGV R&D Sdn Bhd", Value = "RNDSB", Selected = false });
            CompanyListList.Add(new SelectListItem { Text = "FGV Agri Services Sdn Bhd", Value = "FASSB", Selected = false });

            drpyear = timezone.gettimezone().Year - int.Parse(GetConfig.GetData("yeardisplay")) + 1;
            drprangeyear = timezone.gettimezone().Year;

            var yearlist = new List<SelectListItem>();
            for (var i = drpyear; i <= drprangeyear; i++)
            {
                if (i == timezone.gettimezone().Year)
                {
                    yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString(), Selected = true });
                }
                else
                {
                    yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
                }
            }

            //List<ModelsSP.sp_RptRumKedKepPekLad_Result> resultreport = new List<ModelsSP.sp_RptRumKedKepPekLad_Result>();
            ViewBag.MonthList = new SelectList(dbC.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "monthlist" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false), "fldOptConfValue", "fldOptConfDesc", month);
            ViewBag.YearList = yearlist;
            ViewBag.CompanyListList = CompanyListList;
            ViewBag.UserID = getuserid;

            return View();
        }

        //added by faeza 09.06.2021
        public ViewResult _PaymentMode(int? MonthList, int? YearList, string CompanyListList)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            DateTime? CurrentDT = timezone.gettimezone();
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            List<sp_PaymentModeReport_Result> rptPaymentMode = new List<sp_PaymentModeReport_Result>();

            ViewBag.NegaraID = NegaraID;
            ViewBag.SyarikatID = SyarikatID;
            ViewBag.UserID = getuserid;
            ViewBag.UserName = User.Identity.Name;
            ViewBag.NamaSyarikat = db2.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
            ViewBag.NoSyarikat = db2.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();
            ViewBag.MonthList = MonthList;
            ViewBag.YearList = YearList;

            try
            {

                if (String.IsNullOrEmpty(MonthList.ToString()) || String.IsNullOrEmpty(YearList.ToString()) || (String.IsNullOrEmpty(CompanyListList)))
                {
                    ViewBag.Message = @GlobalResCorp.lblChooseWorkerMasterDataReport;
                }
                else
                {
                    if (CompanyListList == "0")
                    {
                        //rptPaymentMode = dbSP.sp_PaymentModeReport(NegaraID, SyarikatID, CompanyListList, MonthList, YearList, getuserid).ToList();
                        rptPaymentMode = dbSP.sp_PaymentModeReport(NegaraID, SyarikatID, CompanyListList, MonthList, YearList, getuserid)
                        .Where(x => x.fld_Month == MonthList &&
                                x.fld_Year == YearList).OrderBy(o => o.fld_LdgCode).ToList();
                    }
                    else
                    {
                        //rptPaymentMode = dbSP.sp_PaymentModeReport(NegaraID, SyarikatID, CompanyListList, MonthList, YearList, getuserid).ToList();
                        rptPaymentMode = dbSP.sp_PaymentModeReport(NegaraID, SyarikatID, CompanyListList, MonthList, YearList, getuserid)
                        .Where(x => x.fld_Month == MonthList &&
                                x.fld_Year == YearList &&
                                x.fld_CostCentre.Contains(CompanyListList)).OrderBy(o => o.fld_LdgCode).ToList();
                    }

                    if (rptPaymentMode.Count == 0)
                    {
                        ViewBag.Message = @GlobalResCorp.msgNoRecord;
                    }
                }
            }
            catch (Exception ex)
            {
                geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
            }

            return View(rptPaymentMode);
        }


        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //[AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3,Viewer")]
        //public ActionResult SocsoFileGen(int? MonthList, int YearList, string CompanyListList, int TypeContribution)
        //{
        //int[] wlyhid = new int[] { };
        //int? NegaraID = 0;
        //int? SyarikatID = 0;
        //int? WilayahID = 0;
        //int? LadangID = 0;
        //int? getuserid = getidentity.ID(User.Identity.Name);
        //int drpyear = 0;
        //int drprangeyear = 0;
        //int? wilayahselection = 0;
        //int? ladangselection = 0;
        //int incldg = 0;

        //ViewBag.Report = "class = active";

        //GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

        //List<SelectListItem> WilayahIDList2 = new List<SelectListItem>();
        //List<SelectListItem> LadangIDList2 = new List<SelectListItem>();

        //if (WilayahID == 0 && LadangID == 0)
        //{
        //    wlyhid = getwilyah.GetWilayahID(SyarikatID);
        //    WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName", WilayahID).ToList();
        //    WilayahIDList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //    if (WilayahIDList == 0)
        //    {
        //        LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
        //    }
        //    else
        //    {
        //        LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => x.fld_WlyhID == WilayahIDList && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
        //    }
        //    LadangIDList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //    wilayahselection = WilayahIDList;
        //    ladangselection = LadangIDList;
        //}
        //else if (WilayahID != 0 && LadangID == 0)
        //{
        //    wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
        //    WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName", WilayahID).ToList();
        //    if (WilayahIDList == 0)
        //    {
        //        LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
        //    }
        //    else
        //    {
        //        LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => x.fld_WlyhID == WilayahIDList && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
        //    }
        //    LadangIDList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //    wilayahselection = WilayahIDList;
        //    ladangselection = LadangIDList;
        //}
        //else if (WilayahID != 0 && LadangID != 0)
        //{
        //    wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
        //    WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName", WilayahID).ToList();
        //    LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_ID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
        //    wilayahselection = WilayahIDList;
        //    ladangselection = LadangIDList;
        //}

        //drpyear = timezone.gettimezone().Year - int.Parse(GetConfig.GetData("yeardisplay")) + 1;
        //drprangeyear = timezone.gettimezone().Year;

        //var yearlist = new List<SelectListItem>();
        //for (var i = drpyear; i <= drprangeyear; i++)
        //{
        //    if (i == YearList)
        //    {
        //        yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString(), Selected = true });
        //    }
        //    else
        //    {
        //        yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
        //    }
        //}

        //List<SelectListItem> KerakyatanList2 = new List<SelectListItem>();
        //KerakyatanList2 = new SelectList(dbC.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "krytnlist" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false), "fldOptConfValue", "fldOptConfDesc", KerakyatanList).ToList();
        //KerakyatanList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "ALL" }));

        //if (WilayahIDList == 0)
        //{
        //    incldg = 1;
        //}
        //else
        //{
        //    if (LadangIDList == 0)
        //    {
        //        incldg = 2;
        //    }
        //    else
        //    {
        //        incldg = 3;
        //    }
        //}

        //ViewBag.MonthList = new SelectList(dbC.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "monthlist" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false), "fldOptConfValue", "fldOptConfDesc", MonthList);

        //List<ModelsSP.sp_RptBulPenPekLad_Result> resultreport = new List<ModelsSP.sp_RptBulPenPekLad_Result>();

        //dbSP.SetCommandTimeout(120);
        //resultreport = dbSP.sp_RptBulPenPekLad(NegaraID, SyarikatID, WilayahIDList, LadangIDList, KerakyatanList, MonthList, YearList, getuserid).ToList();


        //ViewBag.YearList = yearlist;
        //ViewBag.WilayahIDList = WilayahIDList2;
        //ViewBag.LadangIDList = LadangIDList2;
        //ViewBag.KerakyatanList = KerakyatanList2;
        //ViewBag.NeragaID = NegaraID;
        //ViewBag.SyarikatID = SyarikatID;
        //ViewBag.WilayahSelection = WilayahIDList;
        //ViewBag.LadangSelection = LadangIDList;
        //ViewBag.IncLdg = incldg;
        //ViewBag.UserID = getuserid;
        //ViewBag.GetFlag = 2;

        //ViewBag.Month = MonthList;
        //ViewBag.Year = YearList;

        //ViewBag.NamaSyarikat = db2.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
        //ViewBag.NoSyarikat = db2.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();

        //return View(resultreport);
        // }

        //[AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3,Viewer")]
        //public PartialViewResult WorkerSalaryDetail(int selection, int negaraid, int syarikatid, int wilayahid, int ladangid, string krytan, int month, int year, int bill, int incldg)
        //{
        //    List<sp_RptBulPenPekLad_Result> resultreport = new List<sp_RptBulPenPekLad_Result>();
        //    dbC.SetCommandTimeout(120);
        //    resultreport = db3.sp_RptBulPenPekLad(selection, negaraid, syarikatid, wilayahid, ladangid, krytan, month, year).ToList();
        //    ViewBag.Month = month;
        //    ViewBag.Year = year;
        //    ViewBag.NamaKyrtan = db.tblOptionConfigsWebs.Where(x => x.fldOptConfValue == krytan && x.fldOptConfFlag1 == "krytnlist").Select(s => s.fldOptConfDesc).FirstOrDefault();
        //    ViewBag.NamaSyarikat = db2.tbl_Syarikat.Where(x => x.fld_SyarikatID == syarikatid && x.fld_NegaraID == negaraid).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
        //    ViewBag.NoSyarikat = db2.tbl_Syarikat.Where(x => x.fld_SyarikatID == syarikatid && x.fld_NegaraID == negaraid).Select(s => s.fld_NoSyarikat).FirstOrDefault();
        //    ViewBag.WilayahName = db2.tbl_Wilayah.Where(x => x.fld_ID == wilayahid && x.fld_SyarikatID == syarikatid).Select(s => s.fld_WlyhName).FirstOrDefault();
        //    ViewBag.LadangCode = db2.tbl_Ladang.Where(x => x.fld_ID == ladangid && x.fld_WlyhID == wilayahid).Select(s => s.fld_LdgCode).FirstOrDefault();
        //    ViewBag.LadangName = db2.tbl_Ladang.Where(x => x.fld_ID == ladangid && x.fld_WlyhID == wilayahid).Select(s => s.fld_LdgName).FirstOrDefault();
        //    ViewBag.Bill = bill;
        //    ViewBag.IncLdg = incldg;
        //    ViewBag.DataCount = resultreport.Count(); //db3.sp_RptBulPenPekLad(selection, negaraid, syarikatid, wilayahid, ladangid, krytan, month, year).Count();
        //    return View("WorkerSalaryDetail", resultreport);
        //}

        //[AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3,Viewer")]
        //public ActionResult WorkerSalarySummary(int negaraid, int syarikatid, int wilayahid, int ladangid, int month, int year)
        //{
        //    List<sp_RptBulPenPekLadSum_Result> resultreport = new List<sp_RptBulPenPekLadSum_Result>();
        //    //db4.SetCommandTimeout(120);
        //    resultreport = db4.sp_RptBulPenPekLadSum(negaraid, syarikatid, wilayahid, ladangid, "MA", month, year).ToList();
        //    ViewBag.NamaSyarikat = db2.tbl_Syarikat.Where(x => x.fld_SyarikatID == syarikatid && x.fld_NegaraID == negaraid).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
        //    ViewBag.NoSyarikat = db2.tbl_Syarikat.Where(x => x.fld_SyarikatID == syarikatid && x.fld_NegaraID == negaraid).Select(s => s.fld_NoSyarikat).FirstOrDefault();
        //    ViewBag.WilayahName = db2.tbl_Wilayah.Where(x => x.fld_ID == wilayahid && x.fld_SyarikatID == syarikatid).Select(s => s.fld_WlyhName).FirstOrDefault();
        //    ViewBag.LadangCode = db2.tbl_Ladang.Where(x => x.fld_ID == ladangid && x.fld_WlyhID == wilayahid).Select(s => s.fld_LdgCode).FirstOrDefault();
        //    ViewBag.LadangName = db2.tbl_Ladang.Where(x => x.fld_ID == ladangid && x.fld_WlyhID == wilayahid).Select(s => s.fld_LdgName).FirstOrDefault();
        //    ViewBag.Month = month;
        //    ViewBag.Year = year;
        //    return View("WorkerSalarySummary", resultreport);
        //}

        [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3,Viewer")]
        public ActionResult LocalWorkerInfo()
        {
            int[] wlyhid = new int[] { };
            //string mywlyid = "";
            int? NegaraID = 0;
            int? SyarikatID = 0;
            int? WilayahID = 0;
            int? LadangID = 0;
            int year = timezone.gettimezone().Year;
            int drpyear = 0;
            int drprangeyear = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            int? wilayahselection = 0;
            int? ladangselection = 0;
            int incldg = 0;
            string appname = Request.ApplicationPath;
            string domain = Request.Url.GetLeftPart(UriPartial.Authority);

            if (appname != "/")
            {
                domain = domain + appname;
            }

            ViewBag.Report = "class = active";

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            List<SelectListItem> WilayahIDList = new List<SelectListItem>();
            List<SelectListItem> LadangIDList = new List<SelectListItem>();

            if (WilayahID == 0 && LadangID == 0)
            {
                wlyhid = getwilyah.GetWilayahID(SyarikatID);
                WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
                WilayahIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
                LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
                LadangIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
                incldg = 1;
            }
            else if (WilayahID != 0 && LadangID == 0)
            {
                wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
                WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
                LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
                LadangIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
                wilayahselection = WilayahID;
                incldg = 2;

            }
            else if (WilayahID != 0 && LadangID != 0)
            {
                wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
                WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
                LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_ID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
                wilayahselection = WilayahID;
                ladangselection = LadangID;
                incldg = 3;
            }

            drpyear = timezone.gettimezone().Year - int.Parse(GetConfig.GetData("yeardisplay")) + 1;
            drprangeyear = timezone.gettimezone().Year;

            var yearlist = new List<SelectListItem>();
            for (var i = drpyear; i <= drprangeyear; i++)
            {
                if (i == timezone.gettimezone().Year)
                {
                    yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString(), Selected = true });
                }
                else
                {
                    yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
                }
            }

            List<SelectListItem> status = new List<SelectListItem>();

            status.Add(new SelectListItem { Text = GlobalResReport.sltActive, Value = "1", Selected = true });

            status.Add(new SelectListItem { Text = GlobalResReport.sltNotActive, Value = "2" });

            ViewBag.Status = status;
            ViewBag.Incldg = incldg;
            ViewBag.YearList = yearlist;
            ViewBag.Year = year;
            ViewBag.WilayahIDList = WilayahIDList;
            ViewBag.LadangIDList = LadangIDList;
            ViewBag.LadangID = 0;
            ViewBag.ladangvalue = LadangID;
            ViewBag.UserID = getuserid;
            ViewBag.NamaSyarikat = db2.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
            ViewBag.NoSyarikat = db2.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();
            ViewBag.Link = domain;
            ViewBag.Status1 = 1;

            List<sp_RptMakPekTem_Result> resultreport = new List<sp_RptMakPekTem_Result>();

            return View("LocalWorkerInfo", resultreport);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3,Viewer")]
        public ActionResult LocalWorkerInfo(int? YearList, int WilayahIDList, int LadangIDList, int Status)
        {
            int[] wlyhid = new int[] { };
            int? NegaraID = 0;
            int? SyarikatID = 0;
            int? WilayahID = 0;
            int? LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            int drpyear = 0;
            int drprangeyear = 0;
            bool activestatus0 = false, activestatus1 = false;
            int? wilayahselection = 0;
            int? ladangselection = 0;
            int incldg = 0;
            string appname = Request.ApplicationPath;
            string domain = Request.Url.GetLeftPart(UriPartial.Authority);

            if (appname != "/")
            {
                domain = domain + appname;
            }

            ViewBag.Report = "class = active";

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            List<SelectListItem> WilayahIDList2 = new List<SelectListItem>();
            List<SelectListItem> LadangIDList2 = new List<SelectListItem>();

            if (WilayahID == 0 && LadangID == 0)
            {
                wlyhid = getwilyah.GetWilayahID(SyarikatID);
                WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName", WilayahID).ToList();
                WilayahIDList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
                if (WilayahIDList == 0)
                {
                    LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
                }
                else
                {
                    LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => x.fld_WlyhID == WilayahIDList && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
                }
                LadangIDList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
                wilayahselection = WilayahIDList;
                ladangselection = LadangIDList;
            }
            else if (WilayahID != 0 && LadangID == 0)
            {
                wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
                WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName", WilayahID).ToList();
                if (WilayahIDList == 0)
                {
                    LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
                }
                else
                {
                    LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => x.fld_WlyhID == WilayahIDList && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
                }
                LadangIDList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
                wilayahselection = WilayahIDList;
                ladangselection = LadangIDList;
            }
            else if (WilayahID != 0 && LadangID != 0)
            {
                wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
                WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName", WilayahID).ToList();
                LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_ID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
                wilayahselection = WilayahIDList;
                ladangselection = LadangIDList;
            }

            drpyear = timezone.gettimezone().Year - int.Parse(GetConfig.GetData("yeardisplay")) + 1;
            drprangeyear = timezone.gettimezone().Year;

            var yearlist = new List<SelectListItem>();
            for (var i = drpyear; i <= drprangeyear; i++)
            {
                if (i == YearList)
                {
                    yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString(), Selected = true });
                }
                else
                {
                    yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
                }
            }

            ViewBag.WilayahName = db2.tbl_Wilayah.Where(x => x.fld_ID == WilayahIDList && x.fld_SyarikatID == SyarikatID).Select(s => s.fld_WlyhName).FirstOrDefault();
            ViewBag.LadangCode = db2.tbl_Ladang.Where(x => x.fld_ID == LadangIDList && x.fld_WlyhID == WilayahIDList).Select(s => s.fld_LdgCode).FirstOrDefault();
            ViewBag.LadangName = db2.tbl_Ladang.Where(x => x.fld_ID == LadangIDList && x.fld_WlyhID == WilayahIDList).Select(s => s.fld_LdgName).FirstOrDefault();

            List<sp_RptMakPekTem_Result> resultreport = new List<sp_RptMakPekTem_Result>();

            if (WilayahIDList == 0)
            {
                dbSP.SetCommandTimeout(120);
                resultreport = dbSP.sp_RptMakPekTem(NegaraID, SyarikatID, WilayahIDList, LadangIDList, Status, getuserid).ToList();
                incldg = 1;
            }
            else
            {
                if (LadangIDList == 0)
                {
                    dbSP.SetCommandTimeout(120);
                    resultreport = dbSP.sp_RptMakPekTem(NegaraID, SyarikatID, WilayahIDList, LadangIDList, Status, getuserid).ToList();
                    incldg = 2;
                }
                else
                {
                    dbSP.SetCommandTimeout(120);
                    resultreport = dbSP.sp_RptMakPekTem(NegaraID, SyarikatID, WilayahIDList, LadangIDList, Status, getuserid).ToList();
                    incldg = 3;
                }

            }

            switch (Status)
            {
                case 1:
                    activestatus0 = true;
                    break;
                case 2:
                    activestatus1 = true;
                    break;
            }

            List<SelectListItem> status = new List<SelectListItem>();

            status.Add(new SelectListItem { Text = GlobalResReport.sltActive, Value = "1", Selected = activestatus0 });

            status.Add(new SelectListItem { Text = GlobalResReport.sltNotActive, Value = "2", Selected = activestatus1 });

            ViewBag.Status = status;
            ViewBag.Incldg = incldg;
            ViewBag.YearList = yearlist; // list dalam dropdown
            ViewBag.Year = YearList; // year yg user select
            ViewBag.WilayahIDList = WilayahIDList2;
            ViewBag.LadangIDList = LadangIDList2;
            ViewBag.LadangID = LadangIDList;
            ViewBag.ladangvalue = LadangID;
            ViewBag.UserID = getuserid;
            ViewBag.Link = domain;
            ViewBag.Status1 = Status;

            ViewBag.NamaSyarikat = db2.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
            ViewBag.NoSyarikat = db2.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();

            return View("LocalWorkerInfo", resultreport);
        }



        //[AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3,Viewer")]
        //public ActionResult WorkerTransac()
        //{
        //    int[] wlyhid = new int[] { };
        //    //string mywlyid = "";
        //    int? NegaraID = 0;
        //    int? SyarikatID = 0;
        //    int? WilayahID = 0;
        //    int? LadangID = 0;
        //    int year = timezone.gettimezone().Year;
        //    int month = timezone.gettimezone().AddMonths(-1).Month;
        //    int drpyear = 0;
        //    int drprangeyear = 0;
        //    int? getuserid = getidentity.ID(User.Identity.Name);
        //    int? selectionrpt = 0;
        //    int? wilayahselection = 0;
        //    int? ladangselection = 0;
        //    int incldg = 0;
        //    int getflag = 0;
        //    int blgn = 0;

        //    ViewBag.Report = "class = active";

        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

        //    List<SelectListItem> WilayahIDList = new List<SelectListItem>();
        //    List<SelectListItem> LadangIDList = new List<SelectListItem>();

        //    if (WilayahID == 0 && LadangID == 0)
        //    {
        //        wlyhid = getwilyah.GetWilayahID(SyarikatID);
        //        // mywlyid = String.Join("", wlyhid); ;
        //        WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)).OrderBy(o => o.fld_ID), "fld_ID", "fld_WlyhName").ToList();
        //        var wlyhID = db2.tbl_Wilayah.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).Select(s => s.fld_ID).Take(1).FirstOrDefault();
        //        LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => x.fld_WlyhID == wlyhID && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
        //        LadangIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        selectionrpt = 1;
        //        wilayahselection = WilayahID;
        //        ladangselection = LadangID;
        //        incldg = 1;
        //        getflag = 1;
        //    }
        //    else if (WilayahID != 0 && LadangID == 0)
        //    {
        //        //mywlyid = String.Join("", WilayahID); ;
        //        wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
        //        WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
        //        LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
        //        LadangIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        selectionrpt = 2;
        //        wilayahselection = WilayahID;
        //        ladangselection = LadangID;
        //        incldg = 2;
        //        getflag = 1;

        //    }
        //    else if (WilayahID != 0 && LadangID != 0)
        //    {
        //        //mywlyid = String.Join("", WilayahID); ;
        //        wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
        //        WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
        //        LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_ID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
        //        selectionrpt = 3;
        //        wilayahselection = WilayahID;
        //        ladangselection = LadangID;
        //        incldg = 3;
        //        getflag = 1;
        //    }
        //    var getAllWilayahID = db.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)).Select(s => s.fld_ID).Distinct().ToList();
        //    drpyear = timezone.gettimezone().Year - int.Parse(GetConfig.GetData("yeardisplay")) + 1;
        //    drprangeyear = timezone.gettimezone().Year;

        //    var yearlist = new List<SelectListItem>();
        //    for (var i = drpyear; i <= drprangeyear; i++)
        //    {
        //        if (i == timezone.gettimezone().Year)
        //        {
        //            yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString(), Selected = true });
        //        }
        //        else
        //        {
        //            yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
        //        }
        //    }

        //    ViewBag.MonthList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "monthlist" && x.fldDeleted == false), "fldOptConfValue", "fldOptConfDesc", month);
        //    ViewBag.GetAllLdg = LadangIDList;
        //    ViewBag.YearList = yearlist;
        //    ViewBag.WilayahIDList = WilayahIDList;
        //    ViewBag.LadangIDList = LadangIDList;
        //    ViewBag.NeragaID = NegaraID;
        //    ViewBag.SyarikatID = SyarikatID;
        //    ViewBag.WilayahSelection = wilayahselection;
        //    ViewBag.LadangSelection = ladangselection;
        //    ViewBag.Year = year;
        //    ViewBag.Month = month;
        //    ViewBag.Selectionrpt = selectionrpt;
        //    ViewBag.IncLdg = incldg;
        //    ViewBag.UserID = getuserid;
        //    ViewBag.getAllWilayahID = getAllWilayahID;
        //    ViewBag.getflag = getflag;
        //    ViewBag.blgn = blgn;

        //    return View();
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //[AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3,Viewer")]
        //public ActionResult WorkerTransac(int yearlist, int MonthList, int WilayahIDList, int LadangIDList)
        //{
        //    int[] wlyhid = new int[] { };
        //    //string mywlyid = "";
        //    int? NegaraID = 0;
        //    int? SyarikatID = 0;
        //    int? WilayahID = 0;
        //    int? LadangID = 0;
        //    int year = timezone.gettimezone().Year;
        //    int month = timezone.gettimezone().AddMonths(-1).Month;
        //    int drpyear = 0;
        //    int drprangeyear = 0;
        //    int? getuserid = getidentity.ID(User.Identity.Name);
        //    int? selectionrpt = 0;
        //    int? wilayahselection = 0;
        //    int? ladangselection = 0;
        //    int incldg = 0;
        //    int getflag = 2;

        //    ViewBag.Report = "class = active";

        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

        //    List<SelectListItem> WilayahIDList2 = new List<SelectListItem>();
        //    List<SelectListItem> LadangIDList2 = new List<SelectListItem>();

        //    if (WilayahID == 0 && LadangID == 0)
        //    {
        //        wlyhid = getwilyah.GetWilayahID(SyarikatID);
        //        // mywlyid = String.Join("", wlyhid); ;
        //        WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName", WilayahID).ToList();
        //        LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => x.fld_WlyhID == WilayahIDList && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
        //        LadangIDList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        selectionrpt = 1;
        //        wilayahselection = WilayahIDList;
        //        ladangselection = LadangIDList;
        //    }
        //    else if (WilayahID != 0 && LadangID == 0)
        //    {
        //        //mywlyid = String.Join("", WilayahID); ;
        //        wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
        //        WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName", WilayahID).ToList();
        //        LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => x.fld_WlyhID == WilayahIDList && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
        //        LadangIDList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        selectionrpt = 2;
        //        wilayahselection = WilayahIDList;
        //        ladangselection = LadangIDList;
        //    }
        //    else if (WilayahID != 0 && LadangID != 0)
        //    {
        //        //mywlyid = String.Join("", WilayahID); ;
        //        wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
        //        WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName", WilayahID).ToList();
        //        LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_ID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
        //        selectionrpt = 3;
        //        wilayahselection = WilayahIDList;
        //        ladangselection = LadangIDList;
        //    }
        //    var getAllWilayahID = db.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)).Select(s => s.fld_ID).Distinct().ToList();
        //    drpyear = timezone.gettimezone().Year - int.Parse(GetConfig.GetData("yeardisplay")) + 1;
        //    drprangeyear = timezone.gettimezone().Year;

        //    var yearlist2 = new List<SelectListItem>();
        //    for (var i = drpyear; i <= drprangeyear; i++)
        //    {
        //        if (i == timezone.gettimezone().Year)
        //        {
        //            yearlist2.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString(), Selected = true });
        //        }
        //        else
        //        {
        //            yearlist2.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
        //        }
        //    }

        //    ViewBag.MonthList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "monthlist" && x.fldDeleted == false), "fldOptConfValue", "fldOptConfDesc", month);
        //    ViewBag.GetAllLdg = LadangIDList;
        //    ViewBag.YearList = yearlist2;
        //    ViewBag.WilayahIDList = WilayahIDList2;
        //    ViewBag.LadangIDList = LadangIDList2;
        //    ViewBag.NeragaID = NegaraID;
        //    ViewBag.SyarikatID = SyarikatID;
        //    ViewBag.WilayahSelection = wilayahselection;
        //    ViewBag.LadangSelection = ladangselection;
        //    ViewBag.Year = yearlist;
        //    ViewBag.Month = MonthList;
        //    ViewBag.Selectionrpt = selectionrpt;
        //    ViewBag.IncLdg = incldg;
        //    ViewBag.UserID = getuserid;
        //    ViewBag.getAllWilayahID = getAllWilayahID;
        //    ViewBag.getflag = getflag;

        //    return View();
        //}

        //[AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3,Viewer")]
        //public ActionResult WorkerTransacDetail(int negaraid, int syarikatid, int wilayahid, int ladangid, int month, int year, int bill, int incldg)
        //{
        //    string monthstring = month.ToString();
        //    if (monthstring.Length == 1)
        //    {
        //        monthstring = "0" + monthstring;
        //    }
        //    //db3.SetCommandTimeout(120);
        //    var resultreport = db3.sp_RptTransPek(negaraid, syarikatid, wilayahid, ladangid, monthstring, year);
        //    var skbno = db.tbl_Sctran.Where(x => x.fld_NegaraID == negaraid && x.fld_SyarikatID == syarikatid && x.fld_WilayahID == wilayahid && x.fld_LadangID == ladangid && x.fld_Bulan == monthstring && x.fld_Tahun == year && x.fld_NoSkb != "").Select(s => s.fld_NoSkb.Trim()).Distinct().FirstOrDefault();
        //    ViewBag.Month = month;
        //    ViewBag.Year = year;
        //    ViewBag.NamaSyarikat = db2.tbl_Syarikat.Where(x => x.fld_SyarikatID == syarikatid && x.fld_NegaraID == negaraid).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
        //    ViewBag.NoSyarikat = db2.tbl_Syarikat.Where(x => x.fld_SyarikatID == syarikatid && x.fld_NegaraID == negaraid).Select(s => s.fld_NoSyarikat).FirstOrDefault();
        //    ViewBag.WilayahName = db2.tbl_Wilayah.Where(x => x.fld_ID == wilayahid && x.fld_SyarikatID == syarikatid).Select(s => s.fld_WlyhName).FirstOrDefault();
        //    ViewBag.LadangCode = db2.tbl_Ladang.Where(x => x.fld_ID == ladangid && x.fld_WlyhID == wilayahid).Select(s => s.fld_LdgCode).FirstOrDefault();
        //    ViewBag.LadangName = db2.tbl_Ladang.Where(x => x.fld_ID == ladangid && x.fld_WlyhID == wilayahid).Select(s => s.fld_LdgName).FirstOrDefault();
        //    ViewBag.Bill = bill;
        //    ViewBag.IncLdg = incldg;
        //    ViewBag.SkbNo = skbno;
        //    ViewBag.DataCount = db3.sp_RptTransPek(negaraid, syarikatid, wilayahid, ladangid, monthstring, year).Count();

        //    return View("WorkerTransacDetail", resultreport);
        //}

        //[AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3,Viewer")]
        //public ActionResult WorkerTransacSummary()
        //{
        //    int? getuserid = getidentity.ID(User.Identity.Name);
        //    int month = timezone.gettimezone().AddMonths(-1).Month;
        //    int year = timezone.gettimezone().Year;
        //    int[] wlyhid = new int[] { };
        //    int? NegaraID = 0;
        //    int? SyarikatID = 0;
        //    int? WilayahID = 0;
        //    int? LadangID = 0;
        //    //string mywlyid = "";
        //    int? selectionrpt = 0;
        //    int? wilayahselection = 0;
        //    int? ladangselection = 0;
        //    int incldg = 0;
        //    int getflag = 0;
        //    int drpyear = 0;
        //    int drprangeyear = 0;

        //    ViewBag.Report = "class = active";

        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
        //    List<SelectListItem> WilayahIDList = new List<SelectListItem>();
        //    List<SelectListItem> LadangIDList = new List<SelectListItem>();

        //    if (WilayahID == 0 && LadangID == 0)
        //    {
        //        wlyhid = getwilyah.GetWilayahID(SyarikatID);
        //        // mywlyid = String.Join("", wlyhid); ;
        //        WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)).OrderBy(o => o.fld_ID), "fld_ID", "fld_WlyhName").ToList();
        //        WilayahIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        var wlyhID = db2.tbl_Wilayah.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).Select(s => s.fld_ID).Take(1).FirstOrDefault();
        //        LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => x.fld_WlyhID == wlyhID && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
        //        LadangIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        selectionrpt = 1;
        //        wilayahselection = WilayahID;
        //        ladangselection = LadangID;
        //        incldg = 1;
        //        getflag = 1;
        //    }
        //    else if (WilayahID != 0 && LadangID == 0)
        //    {
        //        //mywlyid = String.Join("", WilayahID); ;
        //        wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
        //        WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
        //        LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
        //        LadangIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        selectionrpt = 2;
        //        wilayahselection = WilayahID;
        //        ladangselection = LadangID;
        //        incldg = 2;
        //        getflag = 1;

        //    }
        //    else if (WilayahID != 0 && LadangID != 0)
        //    {
        //        //mywlyid = String.Join("", WilayahID); ;
        //        wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
        //        WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
        //        LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_ID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
        //        selectionrpt = 3;
        //        wilayahselection = WilayahID;
        //        ladangselection = LadangID;
        //        incldg = 3;
        //        getflag = 1;
        //    }

        //    drpyear = timezone.gettimezone().Year - int.Parse(GetConfig.GetData("yeardisplay")) + 1;
        //    drprangeyear = timezone.gettimezone().Year;

        //    var yearlist = new List<SelectListItem>();
        //    for (var i = drpyear; i <= drprangeyear; i++)
        //    {
        //        if (i == timezone.gettimezone().Year)
        //        {
        //            yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString(), Selected = true });
        //        }
        //        else
        //        {
        //            yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
        //        }
        //    }
        //    ViewBag.MonthList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "monthlist" && x.fldDeleted == false), "fldOptConfValue", "fldOptConfDesc", month);

        //    ViewBag.GetAllLdg = LadangIDList;
        //    ViewBag.YearList = yearlist;
        //    ViewBag.WilayahIDList = WilayahIDList;
        //    ViewBag.LadangIDList = LadangIDList;
        //    ViewBag.NeragaID = NegaraID;
        //    ViewBag.SyarikatID = SyarikatID;
        //    ViewBag.WilayahSelection = wilayahselection;
        //    ViewBag.LadangSelection = ladangselection;
        //    ViewBag.Year = year;
        //    ViewBag.Month = month;
        //    ViewBag.Selectionrpt = selectionrpt;
        //    ViewBag.IncLdg = incldg;
        //    ViewBag.UserID = getuserid;
        //    ViewBag.getflag = getflag;

        //    return View();
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //[AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3,Viewer")]
        //public ActionResult WorkerTransacSummary(int yearlist, int MonthList, int WilayahIDList, int LadangIDList)
        //{
        //    int? getuserid = getidentity.ID(User.Identity.Name);
        //    int month = timezone.gettimezone().AddMonths(-1).Month;
        //    int[] wlyhid = new int[] { };
        //    int? NegaraID = 0;
        //    int? SyarikatID = 0;
        //    int? WilayahID = 0;
        //    int? LadangID = 0;
        //    //string mywlyid = "";
        //    int? selectionrpt = 0;
        //    int? wilayahselection = 0;
        //    int? ladangselection = 0;
        //    int incldg = 0;
        //    int getflag = 0;
        //    int drpyear = 0;
        //    int drprangeyear = 0;

        //    ViewBag.Report = "class = active";

        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
        //    List<SelectListItem> WilayahIDList2 = new List<SelectListItem>();
        //    List<SelectListItem> LadangIDList2 = new List<SelectListItem>();

        //    if (WilayahID == 0 && LadangID == 0)
        //    {
        //        wlyhid = getwilyah.GetWilayahID(SyarikatID);
        //        // mywlyid = String.Join("", wlyhid); ;
        //        WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)).OrderBy(o => o.fld_ID), "fld_ID", "fld_WlyhName").ToList();
        //        WilayahIDList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        var wlyhID = db2.tbl_Wilayah.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).Select(s => s.fld_ID).Take(1).FirstOrDefault();
        //        LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => x.fld_WlyhID == WilayahIDList && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
        //        LadangIDList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        selectionrpt = 1;
        //        wilayahselection = WilayahIDList;
        //        ladangselection = LadangIDList;
        //        incldg = 1;
        //        getflag = 2;
        //    }
        //    else if (WilayahID != 0 && LadangID == 0)
        //    {
        //        //mywlyid = String.Join("", WilayahID); ;
        //        wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
        //        WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
        //        LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
        //        LadangIDList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        selectionrpt = 2;
        //        wilayahselection = WilayahIDList;
        //        ladangselection = LadangIDList;
        //        incldg = 2;
        //        getflag = 2;

        //    }
        //    else if (WilayahID != 0 && LadangID != 0)
        //    {
        //        //mywlyid = String.Join("", WilayahID); ;
        //        wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
        //        WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
        //        LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_ID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
        //        selectionrpt = 3;
        //        wilayahselection = WilayahIDList;
        //        ladangselection = LadangIDList;
        //        incldg = 3;
        //        getflag = 2;
        //    }

        //    drpyear = timezone.gettimezone().Year - int.Parse(GetConfig.GetData("yeardisplay")) + 1;
        //    drprangeyear = timezone.gettimezone().Year;

        //    var yearlist2 = new List<SelectListItem>();
        //    for (var i = drpyear; i <= drprangeyear; i++)
        //    {
        //        if (i == timezone.gettimezone().Year)
        //        {
        //            yearlist2.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString(), Selected = true });
        //        }
        //        else
        //        {
        //            yearlist2.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
        //        }
        //    }

        //    ViewBag.MonthList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "monthlist" && x.fldDeleted == false), "fldOptConfValue", "fldOptConfDesc", month);

        //    string monthstring = MonthList.ToString();
        //    if (monthstring.Length == 1)
        //    {
        //        monthstring = "0" + monthstring;
        //    }
        //    //db3.SetCommandTimeout(120);
        //    ViewBag.SumDbt = db3.sp_RptTransPek(NegaraID, SyarikatID, WilayahIDList, LadangIDList, monthstring, yearlist).Select(s => s.fldDebit).Sum();
        //    ViewBag.SumKrdt = db3.sp_RptTransPek(NegaraID, SyarikatID, WilayahIDList, LadangIDList, monthstring, yearlist).Select(s => s.fldKredit).Sum();

        //    ViewBag.Month = MonthList;
        //    ViewBag.Year = yearlist;
        //    ViewBag.GetAllLdg = LadangIDList2;
        //    ViewBag.YearList = yearlist2;
        //    ViewBag.WilayahIDList = WilayahIDList2;
        //    ViewBag.LadangIDList = LadangIDList2;
        //    ViewBag.NeragaID = NegaraID;
        //    ViewBag.SyarikatID = SyarikatID;
        //    ViewBag.WilayahSelection = wilayahselection;
        //    ViewBag.LadangSelection = ladangselection;
        //    ViewBag.Selectionrpt = selectionrpt;
        //    ViewBag.IncLdg = incldg;
        //    ViewBag.UserID = getuserid;
        //    ViewBag.getflag = getflag;
        //    ViewBag.NamaSyarikat = db2.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
        //    ViewBag.NoSyarikat = db2.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();
        //    ViewBag.LadangCode = db2.tbl_Ladang.Where(x => x.fld_ID == LadangIDList && x.fld_WlyhID == WilayahIDList).Select(s => s.fld_LdgCode).FirstOrDefault();
        //    ViewBag.LadangName = db2.tbl_Ladang.Where(x => x.fld_ID == LadangIDList && x.fld_WlyhID == WilayahIDList).Select(s => s.fld_LdgName).FirstOrDefault();
        //    ViewBag.WilayahName = db2.tbl_Wilayah.Where(x => x.fld_ID == WilayahIDList && x.fld_SyarikatID == SyarikatID).Select(s => s.fld_WlyhName).FirstOrDefault();

        //    return View();
        //}

        //[AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3,Viewer")]
        //public ActionResult WorkerTransacSalary()
        //{
        //    int? getuserid = getidentity.ID(User.Identity.Name);
        //    int month = timezone.gettimezone().AddMonths(-1).Month;
        //    int year = timezone.gettimezone().Year;
        //    int[] wlyhid = new int[] { };
        //    int? NegaraID = 0;
        //    int? SyarikatID = 0;
        //    int? WilayahID = 0;
        //    int? LadangID = 0;
        //    //string mywlyid = "";
        //    int? selectionrpt = 0;
        //    int? wilayahselection = 0;
        //    int? ladangselection = 0;
        //    int incldg = 0;
        //    int getflag = 0;
        //    int drpyear = 0;
        //    int drprangeyear = 0;

        //    ViewBag.Report = "class = active";

        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
        //    List<SelectListItem> WilayahIDList = new List<SelectListItem>();
        //    List<SelectListItem> LadangIDList = new List<SelectListItem>();

        //    if (WilayahID == 0 && LadangID == 0)
        //    {
        //        wlyhid = getwilyah.GetWilayahID(SyarikatID);
        //        // mywlyid = String.Join("", wlyhid); ;
        //        WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)).OrderBy(o => o.fld_ID), "fld_ID", "fld_WlyhName").ToList();
        //        WilayahIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        var wlyhID = db2.tbl_Wilayah.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).Select(s => s.fld_ID).Take(1).FirstOrDefault();
        //        LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => x.fld_WlyhID == wlyhID && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
        //        LadangIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        selectionrpt = 1;
        //        wilayahselection = WilayahID;
        //        ladangselection = LadangID;
        //        incldg = 1;
        //        getflag = 1;
        //    }
        //    else if (WilayahID != 0 && LadangID == 0)
        //    {
        //        //mywlyid = String.Join("", WilayahID); ;
        //        wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
        //        WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
        //        LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
        //        LadangIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        selectionrpt = 2;
        //        wilayahselection = WilayahID;
        //        ladangselection = LadangID;
        //        incldg = 2;
        //        getflag = 1;

        //    }
        //    else if (WilayahID != 0 && LadangID != 0)
        //    {
        //        //mywlyid = String.Join("", WilayahID); ;
        //        wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
        //        WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
        //        LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_ID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
        //        selectionrpt = 3;
        //        wilayahselection = WilayahID;
        //        ladangselection = LadangID;
        //        incldg = 3;
        //        getflag = 1;
        //    }

        //    drpyear = timezone.gettimezone().Year - int.Parse(GetConfig.GetData("yeardisplay")) + 1;
        //    drprangeyear = timezone.gettimezone().Year;

        //    var yearlist = new List<SelectListItem>();
        //    for (var i = drpyear; i <= drprangeyear; i++)
        //    {
        //        if (i == timezone.gettimezone().Year)
        //        {
        //            yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString(), Selected = true });
        //        }
        //        else
        //        {
        //            yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
        //        }
        //    }
        //    ViewBag.MonthList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "monthlist" && x.fldDeleted == false), "fldOptConfValue", "fldOptConfDesc", month);

        //    ViewBag.GetAllLdg = LadangIDList;
        //    ViewBag.YearList = yearlist;
        //    ViewBag.WilayahIDList = WilayahIDList;
        //    ViewBag.LadangIDList = LadangIDList;
        //    ViewBag.NeragaID = NegaraID;
        //    ViewBag.SyarikatID = SyarikatID;
        //    ViewBag.WilayahSelection = wilayahselection;
        //    ViewBag.LadangSelection = ladangselection;
        //    ViewBag.Year = year;
        //    ViewBag.Month = month;
        //    ViewBag.Selectionrpt = selectionrpt;
        //    ViewBag.IncLdg = incldg;
        //    ViewBag.UserID = getuserid;
        //    ViewBag.getflag = getflag;

        //    return View();
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //[AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3,Viewer")]
        //public ActionResult WorkerTransacSalary(int yearlist, int MonthList, int WilayahIDList, int LadangIDList)
        //{
        //    int? getuserid = getidentity.ID(User.Identity.Name);
        //    int month = timezone.gettimezone().AddMonths(-1).Month;
        //    int[] wlyhid = new int[] { };
        //    int? NegaraID = 0;
        //    int? SyarikatID = 0;
        //    int? WilayahID = 0;
        //    int? LadangID = 0;
        //    //string mywlyid = "";
        //    int? selectionrpt = 0;
        //    int? wilayahselection = 0;
        //    int? ladangselection = 0;
        //    int incldg = 0;
        //    int getflag = 0;
        //    int drpyear = 0;
        //    int drprangeyear = 0;

        //    ViewBag.Report = "class = active";

        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
        //    List<SelectListItem> WilayahIDList2 = new List<SelectListItem>();
        //    List<SelectListItem> LadangIDList2 = new List<SelectListItem>();

        //    if (WilayahID == 0 && LadangID == 0)
        //    {
        //        wlyhid = getwilyah.GetWilayahID(SyarikatID);
        //        // mywlyid = String.Join("", wlyhid); ;
        //        WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)).OrderBy(o => o.fld_ID), "fld_ID", "fld_WlyhName").ToList();
        //        WilayahIDList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        var wlyhID = db2.tbl_Wilayah.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).Select(s => s.fld_ID).Take(1).FirstOrDefault();
        //        LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => x.fld_WlyhID == WilayahIDList && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
        //        LadangIDList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        selectionrpt = 1;
        //        wilayahselection = WilayahIDList;
        //        ladangselection = LadangIDList;
        //        incldg = 1;
        //        getflag = 2;
        //    }
        //    else if (WilayahID != 0 && LadangID == 0)
        //    {
        //        //mywlyid = String.Join("", WilayahID); ;
        //        wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
        //        WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
        //        LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
        //        LadangIDList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        selectionrpt = 2;
        //        wilayahselection = WilayahIDList;
        //        ladangselection = LadangIDList;
        //        incldg = 2;
        //        getflag = 2;

        //    }
        //    else if (WilayahID != 0 && LadangID != 0)
        //    {
        //        //mywlyid = String.Join("", WilayahID); ;
        //        wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
        //        WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
        //        LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_ID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
        //        selectionrpt = 3;
        //        wilayahselection = WilayahIDList;
        //        ladangselection = LadangIDList;
        //        incldg = 3;
        //        getflag = 2;
        //    }

        //    drpyear = timezone.gettimezone().Year - int.Parse(GetConfig.GetData("yeardisplay")) + 1;
        //    drprangeyear = timezone.gettimezone().Year;

        //    var yearlist2 = new List<SelectListItem>();
        //    for (var i = drpyear; i <= drprangeyear; i++)
        //    {
        //        if (i == timezone.gettimezone().Year)
        //        {
        //            yearlist2.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString(), Selected = true });
        //        }
        //        else
        //        {
        //            yearlist2.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
        //        }
        //    }

        //    ViewBag.MonthList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "monthlist" && x.fldDeleted == false), "fldOptConfValue", "fldOptConfDesc", month);

        //    string monthstring = MonthList.ToString();
        //    if (monthstring.Length == 1)
        //    {
        //        monthstring = "0" + monthstring;
        //    }

        //    ViewBag.Month = MonthList;
        //    ViewBag.Year = yearlist;
        //    ViewBag.GetAllLdg = LadangIDList2;
        //    ViewBag.YearList = yearlist2;
        //    ViewBag.WilayahIDList = WilayahIDList2;
        //    ViewBag.LadangIDList = LadangIDList2;
        //    ViewBag.NeragaID = NegaraID;
        //    ViewBag.SyarikatID = SyarikatID;
        //    ViewBag.WilayahSelection = wilayahselection;
        //    ViewBag.LadangSelection = ladangselection;
        //    ViewBag.Selectionrpt = selectionrpt;
        //    ViewBag.IncLdg = incldg;
        //    ViewBag.UserID = getuserid;
        //    ViewBag.getflag = getflag;
        //    ViewBag.NamaSyarikat = db2.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
        //    ViewBag.NoSyarikat = db2.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();
        //    ViewBag.LadangCode = db2.tbl_Ladang.Where(x => x.fld_ID == LadangIDList && x.fld_WlyhID == WilayahIDList).Select(s => s.fld_LdgCode).FirstOrDefault();
        //    ViewBag.LadangName = db2.tbl_Ladang.Where(x => x.fld_ID == LadangIDList && x.fld_WlyhID == WilayahIDList).Select(s => s.fld_LdgName).FirstOrDefault();
        //    ViewBag.WilayahName = db2.tbl_Wilayah.Where(x => x.fld_ID == WilayahIDList && x.fld_SyarikatID == SyarikatID).Select(s => s.fld_WlyhName).FirstOrDefault();

        //    return View();
        //}

        //[AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3,Viewer")]
        //public ActionResult WorkerMyeg()
        //{
        //    int[] wlyhid = new int[] { };
        //    //string mywlyid = "";
        //    int? NegaraID = 0;
        //    int? SyarikatID = 0;
        //    int? WilayahID = 0;
        //    int? LadangID = 0;
        //    int year = timezone.gettimezone().Year;
        //    int month = timezone.gettimezone().AddMonths(-1).Month;
        //    int? getuserid = getidentity.ID(User.Identity.Name);
        //    int? selectionrpt = 0;
        //    int? wilayahselection = 0;
        //    int? ladangselection = 0;
        //    int incldg = 0;

        //    ViewBag.Report = "class = active";

        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

        //    List<SelectListItem> WilayahIDList = new List<SelectListItem>();
        //    List<SelectListItem> LadangIDList = new List<SelectListItem>();

        //    if (WilayahID == 0 && LadangID == 0)
        //    {
        //        wlyhid = getwilyah.GetWilayahID(SyarikatID);
        //        // mywlyid = String.Join("", wlyhid); ;
        //        WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
        //        //WilayahIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
        //        //LadangIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        selectionrpt = 1;
        //        wilayahselection = WilayahID;
        //        ladangselection = LadangID;
        //        incldg = 1;
        //    }
        //    else if (WilayahID != 0 && LadangID == 0)
        //    {
        //        //mywlyid = String.Join("", WilayahID); ;
        //        wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
        //        WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
        //        LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
        //        //LadangIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        selectionrpt = 2;
        //        wilayahselection = WilayahID;
        //        ladangselection = LadangID;
        //        incldg = 2;

        //    }
        //    else if (WilayahID != 0 && LadangID != 0)
        //    {
        //        //mywlyid = String.Join("", WilayahID); ;
        //        wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
        //        WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
        //        LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_ID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
        //        selectionrpt = 3;
        //        wilayahselection = WilayahID;
        //        ladangselection = LadangID;
        //        incldg = 3;
        //    }

        //    List<SelectListItem> KerakyatanList = new List<SelectListItem>();
        //    KerakyatanList = new SelectList(db.tblOptionConfigsWeb.Where(x => x.fldOptConfFlag1 == "krytnlist" && x.fldDeleted == false), "fldOptConfValue", "fldOptConfDesc").ToList();
        //    KerakyatanList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "ALL" }));

        //    ViewBag.MonthList = new SelectList(db.tblOptionConfigsWeb.Where(x => x.fldOptConfFlag1 == "exprdmonthlist" && x.fldDeleted == false), "fldOptConfValue", "fldOptConfDesc", month);
        //    ViewBag.GetAllKrytan = db.tblOptionConfigsWeb.Where(x => x.fldOptConfFlag1 == "krytnlist" && x.fldDeleted == false).Select(s => s.fldOptConfValue).ToList();
        //    ViewBag.WilayahIDList = WilayahIDList;
        //    ViewBag.LadangIDList = LadangIDList;
        //    ViewBag.KerakyatanList = KerakyatanList;
        //    ViewBag.NeragaID = NegaraID;
        //    ViewBag.SyarikatID = SyarikatID;
        //    ViewBag.WilayahSelection = wilayahselection;
        //    ViewBag.LadangSelection = ladangselection;
        //    ViewBag.Year = year;
        //    ViewBag.Month = month;
        //    ViewBag.Selectionrpt = selectionrpt;
        //    ViewBag.IncLdg = incldg;
        //    ViewBag.UserID = getuserid;
        //    ViewBag.GetFlag = 1;

        //    return View();
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //[AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3,Viewer")]
        //public ActionResult WorkerMyeg(int PassportMonth, int MonthList, int WilayahIDList, int LadangIDList, string KerakyatanList)
        //{
        //    int[] wlyhid = new int[] { };
        //    //string mywlyid = "";
        //    int? NegaraID = 0;
        //    int? SyarikatID = 0;
        //    int? WilayahID = 0;
        //    int? LadangID = 0;
        //    int year = timezone.gettimezone().Year;
        //    int month = timezone.gettimezone().AddMonths(-1).Month;
        //    int? getuserid = getidentity.ID(User.Identity.Name);
        //    int? selectionrpt = 0;
        //    int? wilayahselection = 0;
        //    int? ladangselection = 0;
        //    int incldg = 0;

        //    ViewBag.Report = "class = active";

        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

        //    List<SelectListItem> WilayahIDList2 = new List<SelectListItem>();
        //    List<SelectListItem> LadangIDList2 = new List<SelectListItem>();

        //    if (WilayahID == 0 && LadangID == 0)
        //    {
        //        wlyhid = getwilyah.GetWilayahID(SyarikatID);
        //        // mywlyid = String.Join("", wlyhid); ;
        //        WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
        //        //WilayahIDList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => x.fld_WlyhID == WilayahIDList && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
        //        //LadangIDList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        selectionrpt = 1;
        //        wilayahselection = WilayahIDList;
        //        ladangselection = LadangIDList;
        //        incldg = 1;
        //    }
        //    else if (WilayahID != 0 && LadangID == 0)
        //    {
        //        //mywlyid = String.Join("", WilayahID); ;
        //        wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
        //        WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
        //        LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => x.fld_WlyhID == WilayahIDList && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
        //        //LadangIDList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        selectionrpt = 2;
        //        wilayahselection = WilayahIDList;
        //        ladangselection = LadangIDList;
        //        incldg = 2;

        //    }
        //    else if (WilayahID != 0 && LadangID != 0)
        //    {
        //        //mywlyid = String.Join("", WilayahID); ;
        //        wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
        //        WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
        //        LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => x.fld_WlyhID == WilayahIDList && x.fld_ID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
        //        selectionrpt = 3;
        //        wilayahselection = WilayahIDList;
        //        ladangselection = LadangIDList;
        //        incldg = 3;
        //    }

        //    List<SelectListItem> KerakyatanList2 = new List<SelectListItem>();
        //    KerakyatanList2 = new SelectList(db.tblOptionConfigsWeb.Where(x => x.fldOptConfFlag1 == "krytnlist" && x.fldDeleted == false), "fldOptConfValue", "fldOptConfDesc").ToList();
        //    KerakyatanList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "ALL" }));

        //    ViewBag.MonthList = new SelectList(db.tblOptionConfigsWeb.Where(x => x.fldOptConfFlag1 == "exprdmonthlist" && x.fldDeleted == false), "fldOptConfValue", "fldOptConfDesc", month);
        //    ViewBag.GetAllKrytan = db.tblOptionConfigsWeb.Where(x => x.fldOptConfFlag1 == "krytnlist" && x.fldDeleted == false).Select(s => s.fldOptConfValue).ToList();

        //    List<Models.vw_DetailPekerja> resultreport = new List<Models.vw_DetailPekerja>();
        //    ViewBag.NamaSyarikat = db2.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
        //    ViewBag.NoSyarikat = db2.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();

        //    ViewBag.WilayahIDList = WilayahIDList2;
        //    ViewBag.LadangIDList = LadangIDList2;
        //    ViewBag.KerakyatanList = KerakyatanList2;
        //    ViewBag.NeragaID = NegaraID;
        //    ViewBag.SyarikatID = SyarikatID;
        //    ViewBag.WilayahSelection = wilayahselection;
        //    ViewBag.LadangSelection = ladangselection;
        //    ViewBag.RakyatSelection = KerakyatanList;
        //    ViewBag.Year = year;
        //    ViewBag.Month = MonthList;
        //    ViewBag.Selectionrpt = selectionrpt;
        //    ViewBag.IncLdg = incldg;
        //    ViewBag.UserID = getuserid;
        //    ViewBag.GetFlag = 2;
        //    ViewBag.PassportMonth = PassportMonth;

        //    return View();
        //}

        //[AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3,Viewer")]
        //public ActionResult WorkerMyegDetail(int radio, int negaraid, int syarikatid, int month, int wilayahid, int ladangid, string kerakyatan, int incldg)
        //{
        //    ViewBag.Month = month;
        //    ViewBag.NamaSyarikat = db2.tbl_Syarikat.Where(x => x.fld_SyarikatID == syarikatid && x.fld_NegaraID == negaraid).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
        //    ViewBag.NoSyarikat = db2.tbl_Syarikat.Where(x => x.fld_SyarikatID == syarikatid && x.fld_NegaraID == negaraid).Select(s => s.fld_NoSyarikat).FirstOrDefault();
        //    ViewBag.WilayahName = db2.tbl_Wilayah.Where(x => x.fld_ID == wilayahid && x.fld_SyarikatID == syarikatid).Select(s => s.fld_WlyhName).FirstOrDefault();
        //    ViewBag.LadangCode = db2.tbl_Ladang.Where(x => x.fld_ID == ladangid && x.fld_WlyhID == wilayahid).Select(s => s.fld_LdgCode).FirstOrDefault();
        //    ViewBag.LadangName = db2.tbl_Ladang.Where(x => x.fld_ID == ladangid && x.fld_WlyhID == wilayahid).Select(s => s.fld_LdgName).FirstOrDefault();
        //    if (radio == 0)
        //    {
        //        if (kerakyatan == "ALL")
        //        {
        //            var resultreport = db.vw_DetailPekerja.Where(x => x.fld_BilBlnTmtPsprt != null && x.fld_WilayahID == wilayahid && x.fld_LadangID == ladangid && x.fld_Kdaktf == "0");
        //            return View("WorkerMyegDetail", resultreport);
        //        }
        //        else
        //        {
        //            var resultreport = db.vw_DetailPekerja.Where(x => x.fld_BilBlnTmtPsprt != null && x.fld_WilayahID == wilayahid && x.fld_LadangID == ladangid && x.fld_Kdrkyt == kerakyatan && x.fld_Kdaktf == "0");
        //            return View("WorkerMyegDetail", resultreport);
        //        }
        //    }
        //    else
        //    {
        //        if (month >= 0)
        //        {
        //            if (kerakyatan == "ALL")
        //            {
        //                var resultreport = db.vw_DetailPekerja.Where(x => x.fld_BilBlnTmtPsprt == month && x.fld_WilayahID == wilayahid && x.fld_LadangID == ladangid && x.fld_Kdaktf == "0");
        //                return View("WorkerMyegDetail", resultreport);
        //            }
        //            else
        //            {
        //                var resultreport = db.vw_DetailPekerja.Where(x => x.fld_BilBlnTmtPsprt == month && x.fld_WilayahID == wilayahid && x.fld_LadangID == ladangid && x.fld_Kdrkyt == kerakyatan && x.fld_Kdaktf == "0");
        //                return View("WorkerMyegDetail", resultreport);
        //            }
        //        }
        //        else
        //        {
        //            if (kerakyatan == "ALL")
        //            {
        //                var resultreport = db.vw_DetailPekerja.Where(x => x.fld_BilBlnTmtPsprt <= -1 && x.fld_WilayahID == wilayahid && x.fld_LadangID == ladangid && x.fld_Kdaktf == "0");
        //                return View("WorkerMyegDetail", resultreport);
        //            }
        //            else
        //            {
        //                var resultreport = db.vw_DetailPekerja.Where(x => x.fld_BilBlnTmtPsprt <= -1 && x.fld_WilayahID == wilayahid && x.fld_LadangID == ladangid && x.fld_Kdrkyt == kerakyatan && x.fld_Kdaktf == "0");
        //                return View("WorkerMyegDetail", resultreport);
        //            }
        //        }
        //    }
        //}

        //[AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3,Viewer")]
        //public ActionResult WorkerMyegSummary(int radio, int negaraid, int syarikatid, int month, int wilayahid, int ladangid, string kerakyatan, int incldg)
        //{
        //    ViewBag.RadioSelect = radio;
        //    ViewBag.Month = month;
        //    ViewBag.WilayahID = wilayahid;
        //    ViewBag.LadangID = ladangid;

        //    ViewBag.NamaSyarikat = db2.tbl_Syarikat.Where(x => x.fld_SyarikatID == syarikatid && x.fld_NegaraID == negaraid).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
        //    ViewBag.NoSyarikat = db2.tbl_Syarikat.Where(x => x.fld_SyarikatID == syarikatid && x.fld_NegaraID == negaraid).Select(s => s.fld_NoSyarikat).FirstOrDefault();
        //    ViewBag.WilayahName = db2.tbl_Wilayah.Where(x => x.fld_ID == wilayahid && x.fld_SyarikatID == syarikatid).Select(s => s.fld_WlyhName).FirstOrDefault();
        //    ViewBag.LadangCode = db2.tbl_Ladang.Where(x => x.fld_ID == ladangid && x.fld_WlyhID == wilayahid).Select(s => s.fld_LdgCode).FirstOrDefault();
        //    ViewBag.LadangName = db2.tbl_Ladang.Where(x => x.fld_ID == ladangid && x.fld_WlyhID == wilayahid).Select(s => s.fld_LdgName).FirstOrDefault();

        //    if (kerakyatan == "ALL")
        //    {
        //        var resultreport = db.tblOptionConfigsWeb.Where(x => x.fldOptConfFlag1 == "krytnlist" && x.fldOptConfFlag2 != null);
        //        return View("WorkerMyegSummary", resultreport);
        //    }
        //    else
        //    {
        //        var resultreport = db.tblOptionConfigsWeb.Where(x => x.fldOptConfFlag1 == "krytnlist" && x.fldOptConfFlag2 != null && x.fldOptConfValue == kerakyatan);
        //        return View("WorkerMyegSummary", resultreport);
        //    }
        //}

        //[AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3,Viewer")]
        //public ActionResult NoSkb()
        //{
        //    int? getuserid = getidentity.ID(User.Identity.Name);
        //    int month = timezone.gettimezone().AddMonths(-1).Month;
        //    int year = timezone.gettimezone().Year;
        //    int[] wlyhid = new int[] { };
        //    int? NegaraID = 0;
        //    int? SyarikatID = 0;
        //    int? WilayahID = 0;
        //    int? LadangID = 0;
        //    //string mywlyid = "";
        //    int? selectionrpt = 0;
        //    int? wilayahselection = 0;
        //    int? ladangselection = 0;
        //    int incldg = 0;
        //    int getflag = 0;
        //    int drpyear = 0;
        //    int drprangeyear = 0;

        //    ViewBag.Report = "class = active";

        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
        //    List<SelectListItem> WilayahIDList = new List<SelectListItem>();
        //    List<SelectListItem> LadangIDList = new List<SelectListItem>();

        //    if (WilayahID == 0 && LadangID == 0)
        //    {
        //        wlyhid = getwilyah.GetWilayahID(SyarikatID);
        //        // mywlyid = String.Join("", wlyhid); ;
        //        WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)).OrderBy(o => o.fld_ID), "fld_ID", "fld_WlyhName").ToList();
        //        WilayahIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        var wlyhID = db2.tbl_Wilayah.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).Select(s => s.fld_ID).Take(1).FirstOrDefault();
        //        LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => x.fld_WlyhID == wlyhID && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
        //        LadangIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        selectionrpt = 1;
        //        wilayahselection = WilayahID;
        //        ladangselection = LadangID;
        //        incldg = 1;
        //        getflag = 1;
        //    }
        //    else if (WilayahID != 0 && LadangID == 0)
        //    {
        //        //mywlyid = String.Join("", WilayahID); ;
        //        wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
        //        WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
        //        LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
        //        LadangIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        selectionrpt = 2;
        //        wilayahselection = WilayahID;
        //        ladangselection = LadangID;
        //        incldg = 2;
        //        getflag = 1;

        //    }
        //    else if (WilayahID != 0 && LadangID != 0)
        //    {
        //        //mywlyid = String.Join("", WilayahID); ;
        //        wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
        //        WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
        //        LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_ID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
        //        selectionrpt = 3;
        //        wilayahselection = WilayahID;
        //        ladangselection = LadangID;
        //        incldg = 3;
        //        getflag = 1;
        //    }

        //    drpyear = timezone.gettimezone().Year - int.Parse(GetConfig.GetData("yeardisplay")) + 1;
        //    drprangeyear = timezone.gettimezone().Year;

        //    var yearlist = new List<SelectListItem>();
        //    for (var i = drpyear; i <= drprangeyear; i++)
        //    {
        //        if (i == timezone.gettimezone().Year)
        //        {
        //            yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString(), Selected = true });
        //        }
        //        else
        //        {
        //            yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
        //        }
        //    }
        //    ViewBag.MonthList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "monthlist" && x.fldDeleted == false), "fldOptConfValue", "fldOptConfDesc", month);

        //    ViewBag.GetAllLdg = LadangIDList;
        //    ViewBag.YearList = yearlist;
        //    ViewBag.WilayahIDList = WilayahIDList;
        //    ViewBag.LadangIDList = LadangIDList;
        //    ViewBag.NeragaID = NegaraID;
        //    ViewBag.SyarikatID = SyarikatID;
        //    ViewBag.WilayahSelection = wilayahselection;
        //    ViewBag.LadangSelection = ladangselection;
        //    ViewBag.Year = year;
        //    ViewBag.Month = month;
        //    ViewBag.Selectionrpt = selectionrpt;
        //    ViewBag.IncLdg = incldg;
        //    ViewBag.UserID = getuserid;
        //    ViewBag.getflag = getflag;

        //    return View();
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //[AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3,Viewer")]
        //public ActionResult NoSkb(int yearlist, int MonthList, int WilayahIDList, int LadangIDList)
        //{
        //    int? getuserid = getidentity.ID(User.Identity.Name);
        //    int month = timezone.gettimezone().AddMonths(-1).Month;
        //    int year = timezone.gettimezone().Year;
        //    int[] wlyhid = new int[] { };
        //    int? NegaraID = 0;
        //    int? SyarikatID = 0;
        //    int? WilayahID = 0;
        //    int? LadangID = 0;
        //    //string mywlyid = "";
        //    int? selectionrpt = 0;
        //    int? wilayahselection = 0;
        //    int? ladangselection = 0;
        //    int incldg = 0;
        //    int getflag = 0;
        //    int drpyear = 0;
        //    int drprangeyear = 0;

        //    ViewBag.Report = "class = active";

        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
        //    List<SelectListItem> WilayahIDList2 = new List<SelectListItem>();
        //    List<SelectListItem> LadangIDList2 = new List<SelectListItem>();

        //    if (WilayahID == 0 && LadangID == 0)
        //    {
        //        wlyhid = getwilyah.GetWilayahID(SyarikatID);
        //        // mywlyid = String.Join("", wlyhid); ;
        //        WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)).OrderBy(o => o.fld_ID), "fld_ID", "fld_WlyhName").ToList();
        //        WilayahIDList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        //var wlyhID = db2.tbl_Wilayah.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).Select(s => s.fld_ID).Take(1).FirstOrDefault();
        //        LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => x.fld_WlyhID == WilayahIDList && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
        //        LadangIDList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        selectionrpt = 1;
        //        wilayahselection = WilayahIDList;
        //        ladangselection = LadangIDList;
        //        incldg = 1;
        //        getflag = 2;
        //    }
        //    else if (WilayahID != 0 && LadangID == 0)
        //    {
        //        //mywlyid = String.Join("", WilayahID); ;
        //        wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
        //        WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
        //        LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
        //        LadangIDList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        selectionrpt = 2;
        //        wilayahselection = WilayahIDList;
        //        ladangselection = LadangIDList;
        //        incldg = 2;
        //        getflag = 2;

        //    }
        //    else if (WilayahID != 0 && LadangID != 0)
        //    {
        //        //mywlyid = String.Join("", WilayahID); ;
        //        wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
        //        WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
        //        LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_ID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
        //        selectionrpt = 3;
        //        wilayahselection = WilayahIDList;
        //        ladangselection = LadangIDList;
        //        incldg = 3;
        //        getflag = 2;
        //    }

        //    drpyear = timezone.gettimezone().Year - int.Parse(GetConfig.GetData("yeardisplay")) + 1;
        //    drprangeyear = timezone.gettimezone().Year;

        //    var yearlist2 = new List<SelectListItem>();
        //    for (var i = drpyear; i <= drprangeyear; i++)
        //    {
        //        if (i == timezone.gettimezone().Year)
        //        {
        //            yearlist2.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString(), Selected = true });
        //        }
        //        else
        //        {
        //            yearlist2.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
        //        }
        //    }
        //    ViewBag.MonthList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "monthlist" && x.fldDeleted == false), "fldOptConfValue", "fldOptConfDesc", month);

        //    ViewBag.GetAllLdg = LadangIDList2;
        //    ViewBag.YearList = yearlist2;
        //    ViewBag.WilayahIDList = WilayahIDList2;
        //    ViewBag.LadangIDList = LadangIDList2;
        //    ViewBag.NeragaID = NegaraID;
        //    ViewBag.SyarikatID = SyarikatID;
        //    ViewBag.WilayahSelection = wilayahselection;
        //    ViewBag.LadangSelection = ladangselection;
        //    ViewBag.Year = year;
        //    ViewBag.Month = MonthList;
        //    ViewBag.Selectionrpt = selectionrpt;
        //    ViewBag.IncLdg = incldg;
        //    ViewBag.UserID = getuserid;
        //    ViewBag.getflag = getflag;
        //    ViewBag.NamaSyarikat = db2.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
        //    ViewBag.NoSyarikat = db2.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();

        //    return View();
        //}

        //[AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3,Viewer")]
        //public ActionResult NoSkbDetail(int negaraid, int syarikatid, int wilayahid, int ladangid, int month, int year, int bill, int incldg)
        //{
        //    string monthstring = month.ToString();
        //    if (monthstring.Length == 1)
        //    {
        //        monthstring = "0" + monthstring;
        //    }
        //    //db3.SetCommandTimeout(120);
        //    var resultreport = db3.sp_RptTransPek(negaraid, syarikatid, wilayahid, ladangid, monthstring, year).Where(x => x.fldLejar == "452");//db.vw_skb.Where(x => x.fld_Bulan == monthstring && x.fld_Tahun == year && x.fld_WilayahID == wilayahid && x.fld_LadangID == ladangid);
        //    ViewBag.Month = month;
        //    ViewBag.Year = year;
        //    ViewBag.NamaSyarikat = db2.tbl_Syarikat.Where(x => x.fld_SyarikatID == syarikatid && x.fld_NegaraID == negaraid).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
        //    ViewBag.NoSyarikat = db2.tbl_Syarikat.Where(x => x.fld_SyarikatID == syarikatid && x.fld_NegaraID == negaraid).Select(s => s.fld_NoSyarikat).FirstOrDefault();
        //    ViewBag.WilayahName = db2.tbl_Wilayah.Where(x => x.fld_ID == wilayahid && x.fld_SyarikatID == syarikatid).Select(s => s.fld_WlyhName).FirstOrDefault();
        //    ViewBag.LadangCode = db2.tbl_Ladang.Where(x => x.fld_ID == ladangid && x.fld_WlyhID == wilayahid).Select(s => s.fld_LdgCode).FirstOrDefault();
        //    ViewBag.LadangName = db2.tbl_Ladang.Where(x => x.fld_ID == ladangid && x.fld_WlyhID == wilayahid).Select(s => s.fld_LdgName).FirstOrDefault();
        //    ViewBag.Bill = bill;
        //    ViewBag.IncLdg = incldg;

        //    return View("NoSkbDetail", resultreport);
        //}

        //[AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3,Viewer")]
        //public ActionResult NewWorkerApp()
        //{
        //    int[] wlyhid = new int[] { };
        //    //string mywlyid = "";
        //    int? NegaraID = 0;
        //    int? SyarikatID = 0;
        //    int? WilayahID = 0;
        //    int? LadangID = 0;
        //    int year = timezone.gettimezone().Year;
        //    int month = timezone.gettimezone().Month;
        //    int drpyear = 0;
        //    int drprangeyear = 0;
        //    int? getuserid = getidentity.ID(User.Identity.Name);
        //    int? selectionrpt = 0;
        //    int? wilayahselection = 0;
        //    int? ladangselection = 0;
        //    int incldg = 0;
        //    int getflag = 0;


        //    ViewBag.Report = "class = active";

        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

        //    List<SelectListItem> WilayahIDList = new List<SelectListItem>();
        //    List<SelectListItem> LadangIDList = new List<SelectListItem>();

        //    if (WilayahID == 0 && LadangID == 0)
        //    {
        //        wlyhid = getwilyah.GetWilayahID(SyarikatID);
        //        // mywlyid = String.Join("", wlyhid); ;
        //        WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)).OrderBy(o => o.fld_ID), "fld_ID", "fld_WlyhName").ToList();
        //        WilayahIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
        //        LadangIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        selectionrpt = 1;
        //        wilayahselection = WilayahID;
        //        ladangselection = LadangID;
        //        incldg = 1;
        //        getflag = 1;
        //    }
        //    else if (WilayahID != 0 && LadangID == 0)
        //    {
        //        //mywlyid = String.Join("", WilayahID); ;
        //        wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
        //        WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
        //        LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
        //        LadangIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        selectionrpt = 2;
        //        wilayahselection = WilayahID;
        //        ladangselection = LadangID;
        //        incldg = 2;
        //        getflag = 1;

        //    }
        //    else if (WilayahID != 0 && LadangID != 0)
        //    {
        //        //mywlyid = String.Join("", WilayahID); ;
        //        wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
        //        WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
        //        LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_ID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
        //        selectionrpt = 3;
        //        wilayahselection = WilayahID;
        //        ladangselection = LadangID;
        //        incldg = 3;
        //        getflag = 1;
        //    }
        //    drpyear = timezone.gettimezone().Year - int.Parse(GetConfig.GetData("yeardisplay")) + 1;
        //    drprangeyear = timezone.gettimezone().Year;

        //    var yearlist = new List<SelectListItem>();
        //    for (var i = drpyear; i <= drprangeyear; i++)
        //    {
        //        if (i == timezone.gettimezone().Year)
        //        {
        //            yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString(), Selected = true });
        //        }
        //        else
        //        {
        //            yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
        //        }
        //    }

        //    ViewBag.MonthList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "monthlist" && x.fldDeleted == false), "fldOptConfValue", "fldOptConfDesc", month);
        //    ViewBag.StatusApprove = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "statusapproval2" && x.fldDeleted == false), "fldOptConfValue", "fldOptConfDesc").ToList();

        //    ViewBag.YearList = yearlist;
        //    ViewBag.WilayahIDList = WilayahIDList;
        //    ViewBag.LadangIDList = LadangIDList;
        //    ViewBag.NeragaID = NegaraID;
        //    ViewBag.SyarikatID = SyarikatID;
        //    ViewBag.WilayahSelection = wilayahselection;
        //    ViewBag.LadangSelection = ladangselection;
        //    ViewBag.Year = year;
        //    ViewBag.Month = month;
        //    ViewBag.Selectionrpt = selectionrpt;
        //    ViewBag.IncLdg = incldg;
        //    ViewBag.UserID = getuserid;
        //    ViewBag.getflag = getflag;


        //    return View();
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //[AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3,Viewer")]
        //public ActionResult NewWorkerApp(string StatusApprove, int MonthList, int yearlist, int WilayahIDList, int LadangIDList)
        //{
        //    int[] wlyhid = new int[] { };
        //    //string mywlyid = "";
        //    int? NegaraID = 0;
        //    int? SyarikatID = 0;
        //    int? WilayahID = 0;
        //    int? LadangID = 0;
        //    int year = timezone.gettimezone().Year;
        //    int month = timezone.gettimezone().Month;
        //    int drpyear = 0;
        //    int drprangeyear = 0;
        //    int? getuserid = getidentity.ID(User.Identity.Name);
        //    int? selectionrpt = 0;
        //    int? wilayahselection = 0;
        //    int? ladangselection = 0;
        //    int incldg = 0;
        //    int getflag = 0;


        //    ViewBag.Report = "class = active";

        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

        //    List<SelectListItem> WilayahIDList2 = new List<SelectListItem>();
        //    List<SelectListItem> LadangIDList2 = new List<SelectListItem>();

        //    if (WilayahID == 0 && LadangID == 0)
        //    {
        //        wlyhid = getwilyah.GetWilayahID(SyarikatID);
        //        // mywlyid = String.Join("", wlyhid); ;
        //        WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)).OrderBy(o => o.fld_ID), "fld_ID", "fld_WlyhName").ToList();
        //        WilayahIDList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
        //        LadangIDList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        selectionrpt = 1;
        //        wilayahselection = WilayahIDList;
        //        ladangselection = LadangIDList;
        //        incldg = 1;
        //        getflag = 2;
        //    }
        //    else if (WilayahID != 0 && LadangID == 0)
        //    {
        //        //mywlyid = String.Join("", WilayahID); ;
        //        wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
        //        WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
        //        LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
        //        LadangIDList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        selectionrpt = 2;
        //        wilayahselection = WilayahIDList;
        //        ladangselection = LadangIDList;
        //        incldg = 2;
        //        getflag = 2;

        //    }
        //    else if (WilayahID != 0 && LadangID != 0)
        //    {
        //        //mywlyid = String.Join("", WilayahID); ;
        //        wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
        //        WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
        //        LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_ID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
        //        selectionrpt = 3;
        //        wilayahselection = WilayahIDList;
        //        ladangselection = LadangIDList;
        //        incldg = 3;
        //        getflag = 2;
        //    }
        //    drpyear = timezone.gettimezone().Year - int.Parse(GetConfig.GetData("yeardisplay")) + 1;
        //    drprangeyear = timezone.gettimezone().Year;

        //    var yearlist2 = new List<SelectListItem>();
        //    for (var i = drpyear; i <= drprangeyear; i++)
        //    {
        //        if (i == timezone.gettimezone().Year)
        //        {
        //            yearlist2.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString(), Selected = true });
        //        }
        //        else
        //        {
        //            yearlist2.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
        //        }
        //    }

        //    ViewBag.MonthList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "monthlist" && x.fldDeleted == false), "fldOptConfValue", "fldOptConfDesc", month);
        //    ViewBag.StatusApprove = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "statusapproval2" && x.fldDeleted == false), "fldOptConfValue", "fldOptConfDesc").ToList();

        //    ViewBag.YearList = yearlist2;
        //    ViewBag.WilayahIDList = WilayahIDList2;
        //    ViewBag.LadangIDList = LadangIDList2;
        //    ViewBag.NeragaID = NegaraID;
        //    ViewBag.SyarikatID = SyarikatID;
        //    ViewBag.WilayahSelection = wilayahselection;
        //    ViewBag.LadangSelection = ladangselection;
        //    ViewBag.Year = year;
        //    ViewBag.Month = MonthList;
        //    ViewBag.Selectionrpt = selectionrpt;
        //    ViewBag.IncLdg = incldg;
        //    ViewBag.UserID = getuserid;
        //    ViewBag.getflag = getflag;
        //    ViewBag.Status = StatusApprove;
        //    return View();
        //}

        //[AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3,Viewer")]
        //public ActionResult NewWorkerAppDetail(int negaraid, int syarikatid, int wilayahid, int ladangid, int month, int year, int incldg, string statusapprove)
        //{
        //    ViewBag.IncLdg = incldg;
        //    //ViewBag.Status = statusapprove;
        //    ViewBag.Month = month;
        //    ViewBag.Year = year;
        //    ViewBag.NamaSyarikat = db2.tbl_Syarikat.Where(x => x.fld_SyarikatID == syarikatid && x.fld_NegaraID == negaraid).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
        //    ViewBag.NoSyarikat = db2.tbl_Syarikat.Where(x => x.fld_SyarikatID == syarikatid && x.fld_NegaraID == negaraid).Select(s => s.fld_NoSyarikat).FirstOrDefault();
        //    ViewBag.WilayahName = db2.tbl_Wilayah.Where(x => x.fld_ID == wilayahid && x.fld_SyarikatID == syarikatid).Select(s => s.fld_WlyhName).FirstOrDefault();
        //    ViewBag.LadangCode = db2.tbl_Ladang.Where(x => x.fld_ID == ladangid && x.fld_WlyhID == wilayahid).Select(s => s.fld_LdgCode).FirstOrDefault();
        //    ViewBag.LadangName = db2.tbl_Ladang.Where(x => x.fld_ID == ladangid && x.fld_WlyhID == wilayahid).Select(s => s.fld_LdgName).FirstOrDefault();
        //    int status = int.Parse(statusapprove);

        //    List<Models.tblPkjmastApp> resultreport = new List<Models.tblPkjmastApp>();

        //    if (status == 0)
        //    {
        //        resultreport = db.tblPkjmastApps.Where(x => x.fldNegaraID == negaraid && x.fldSyarikatID == syarikatid && x.fldWilayahID == wilayahid && x.fldLadangID == ladangid && x.fldDateTimeApprove.Value.Month == month && x.fldDateTimeApprove.Value.Year == year).ToList();
        //        ViewBag.DataCount = resultreport.Count();
        //        //return View("NewWorkerAppDetail", resultreport);
        //    }
        //    else if (status == 1)
        //    {
        //        resultreport = db.tblPkjmastApps.Where(x => x.fldNegaraID == negaraid && x.fldSyarikatID == syarikatid && x.fldWilayahID == wilayahid && x.fldLadangID == ladangid && x.fldDateTimeApprove.Value.Month == month && x.fldDateTimeApprove.Value.Year == year && x.fldStatus == 1).ToList();
        //        ViewBag.DataCount = resultreport.Count();
        //        //return View("NewWorkerAppDetail", resultreport);
        //    }
        //    else
        //    {
        //        resultreport = db.tblPkjmastApps.Where(x => x.fldNegaraID == negaraid && x.fldSyarikatID == syarikatid && x.fldWilayahID == wilayahid && x.fldLadangID == ladangid && x.fldDateTimeApprove.Value.Month == month && x.fldDateTimeApprove.Value.Year == year && x.fldStatus == 0).ToList();
        //        ViewBag.DataCount = resultreport.Count();

        //    }
        //    return View("NewWorkerAppDetail", resultreport);
        //}

        //[AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3,Viewer")]
        //public ActionResult WorkerReport()
        //{
        //    int[] wlyhid = new int[] { };
        //    //string mywlyid = "";
        //    int? NegaraID = 0;
        //    int? SyarikatID = 0;
        //    int? WilayahID = 0;
        //    int? LadangID = 0;
        //    int year = timezone.gettimezone().Year;
        //    int month = timezone.gettimezone().AddMonths(-1).Month;
        //    int drpyear = 0;
        //    int drprangeyear = 0;
        //    int? getuserid = getidentity.ID(User.Identity.Name);
        //    int? selectionrpt = 0;
        //    int? wilayahselection = 0;
        //    int? ladangselection = 0;
        //    int incldg = 0;
        //    int getflag = 0;


        //    ViewBag.Report = "class = active";

        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

        //    List<SelectListItem> WilayahIDList = new List<SelectListItem>();
        //    List<SelectListItem> LadangIDList = new List<SelectListItem>();

        //    if (WilayahID == 0 && LadangID == 0)
        //    {
        //        wlyhid = getwilyah.GetWilayahID(SyarikatID);
        //        // mywlyid = String.Join("", wlyhid); ;
        //        WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)).OrderBy(o => o.fld_ID), "fld_ID", "fld_WlyhName").ToList();
        //        var wlyhID = db2.tbl_Wilayah.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).Select(s => s.fld_ID).Take(1).FirstOrDefault();
        //        LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => x.fld_WlyhID == wlyhID && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
        //        var ldgID = db2.tbl_Ladang.Where(x => x.fld_WlyhID == wlyhID && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => s.fld_ID).Take(1).FirstOrDefault();
        //        //LadangIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        selectionrpt = 1;
        //        wilayahselection = wlyhID;
        //        ladangselection = ldgID;
        //        incldg = 1;
        //        getflag = 1;
        //    }
        //    else if (WilayahID != 0 && LadangID == 0)
        //    {
        //        //mywlyid = String.Join("", WilayahID); ;
        //        wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
        //        WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
        //        LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
        //        //LadangIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        selectionrpt = 2;
        //        wilayahselection = WilayahID;
        //        ladangselection = LadangID;
        //        incldg = 2;
        //        getflag = 1;

        //    }
        //    else if (WilayahID != 0 && LadangID != 0)
        //    {
        //        //mywlyid = String.Join("", WilayahID); ;
        //        wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
        //        WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
        //        LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_ID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
        //        selectionrpt = 3;
        //        wilayahselection = WilayahID;
        //        ladangselection = LadangID;
        //        incldg = 3;
        //        getflag = 1;
        //    }
        //    List<SelectListItem> NoPekerjaList = new List<SelectListItem>();
        //    var getpekerja = db.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == wilayahselection && x.fld_LadangID == ladangselection).Select(s => s.fld_Nopkj).ToArray();
        //    var getpekerja2 = getpekerja.Distinct().ToList();
        //    NoPekerjaList = new SelectList(db.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == wilayahselection && x.fld_LadangID == ladangselection && getpekerja2.Contains(x.fld_Nopkj.Trim())).Select(s => new SelectListItem { Value = s.fld_Nopkj.Trim(), Text = s.fld_Nopkj.Trim() + " - " + s.fld_Nama1.Trim() }), "Value", "Text").Distinct().ToList();
        //    //NoPekerjaList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //    //NoPekerjaList= db.vw_KerjaHarian.Where(x => x.fld_WilayahID == wlyhid && x.fld_LadangID == ldgid && x.fld_Tarikh.Month == mont && x.fld_Tarikh.Year == year).Select(s => s.fld_Nopkj.Trim()).Distinct().ToList();

        //    drpyear = timezone.gettimezone().Year - int.Parse(GetConfig.GetData("yeardisplay")) + 1;
        //    drprangeyear = timezone.gettimezone().Year;

        //    var yearlist = new List<SelectListItem>();
        //    for (var i = drpyear; i <= drprangeyear; i++)
        //    {
        //        if (i == timezone.gettimezone().Year)
        //        {
        //            yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString(), Selected = true });
        //        }
        //        else
        //        {
        //            yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
        //        }
        //    }

        //    ViewBag.MonthList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "monthlist" && x.fldDeleted == false), "fldOptConfValue", "fldOptConfDesc", month);

        //    ViewBag.YearList = yearlist;
        //    ViewBag.WilayahIDList = WilayahIDList;
        //    ViewBag.LadangIDList = LadangIDList;
        //    ViewBag.NeragaID = NegaraID;
        //    ViewBag.SyarikatID = SyarikatID;
        //    ViewBag.NoPekerjaList = NoPekerjaList;
        //    ViewBag.WilayahSelection = wilayahselection;
        //    ViewBag.LadangSelection = ladangselection;
        //    ViewBag.PekerjaSelection = 0;
        //    ViewBag.Year = year;
        //    ViewBag.Month = month;
        //    ViewBag.Selectionrpt = selectionrpt;
        //    ViewBag.IncLdg = incldg;
        //    ViewBag.UserID = getuserid;
        //    ViewBag.getflag = getflag;


        //    return View();
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //[AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3,Viewer")]
        //public ActionResult WorkerReport(int MonthList, int yearlist, int WilayahIDList, int LadangIDList, string NoPekerjaList)
        //{
        //    int[] wlyhid = new int[] { };
        //    //string mywlyid = "";
        //    int? NegaraID = 0;
        //    int? SyarikatID = 0;
        //    int? WilayahID = 0;
        //    int? LadangID = 0;
        //    int year = timezone.gettimezone().Year;
        //    int month = timezone.gettimezone().AddMonths(-1).Month;
        //    int drpyear = 0;
        //    int drprangeyear = 0;
        //    int? getuserid = getidentity.ID(User.Identity.Name);
        //    int? selectionrpt = 0;
        //    int? wilayahselection = 0;
        //    int? ladangselection = 0;
        //    int incldg = 0;
        //    int getflag = 0;


        //    ViewBag.Report = "class = active";

        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

        //    List<SelectListItem> WilayahIDList2 = new List<SelectListItem>();
        //    List<SelectListItem> LadangIDList2 = new List<SelectListItem>();

        //    if (WilayahID == 0 && LadangID == 0)
        //    {
        //        wlyhid = getwilyah.GetWilayahID(SyarikatID);
        //        // mywlyid = String.Join("", wlyhid); ;
        //        WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName", WilayahID).ToList();
        //        //WilayahIDList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        if (WilayahIDList == 0)
        //        {
        //            LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
        //        }
        //        else
        //        {
        //            LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => x.fld_WlyhID == WilayahIDList && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
        //        }
        //        //LadangIDList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        selectionrpt = 1;
        //        wilayahselection = WilayahIDList;
        //        ladangselection = LadangIDList;
        //        incldg = 1;
        //        getflag = 2;
        //    }
        //    else if (WilayahID != 0 && LadangID == 0)
        //    {
        //        //mywlyid = String.Join("", WilayahID); ;
        //        wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
        //        WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName", WilayahID).ToList();
        //        if (WilayahIDList == 0)
        //        {
        //            LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
        //        }
        //        else
        //        {
        //            LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => x.fld_WlyhID == WilayahIDList && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
        //        }
        //        //LadangIDList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        selectionrpt = 2;
        //        wilayahselection = WilayahIDList;
        //        ladangselection = LadangIDList;
        //        incldg = 2;
        //        getflag = 2;
        //    }
        //    else if (WilayahID != 0 && LadangID != 0)
        //    {
        //        //mywlyid = String.Join("", WilayahID); ;
        //        wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
        //        WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName", WilayahID).ToList();
        //        LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_ID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
        //        selectionrpt = 3;
        //        wilayahselection = WilayahIDList;
        //        ladangselection = LadangIDList;
        //        incldg = 3;
        //        getflag = 2;
        //    }
        //    List<SelectListItem> NoPekerjaList2 = new List<SelectListItem>();
        //    var getpekerja = db.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == wilayahselection && x.fld_LadangID == ladangselection).Select(s => s.fld_Nopkj).Distinct().ToArray();
        //    NoPekerjaList2 = new SelectList(db.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == wilayahselection && x.fld_LadangID == ladangselection && getpekerja.Contains(x.fld_Nopkj.Trim())).Select(s => new SelectListItem { Value = s.fld_Nopkj.Trim(), Text = s.fld_Nopkj.Trim() + " - " + s.fld_Nama1.Trim() }), "Value", "Text", NoPekerjaList.Trim()).Distinct().ToList();
        //    //NoPekerjaList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));

        //    drpyear = timezone.gettimezone().Year - int.Parse(GetConfig.GetData("yeardisplay")) + 1;
        //    drprangeyear = timezone.gettimezone().Year;

        //    var yearlist2 = new List<SelectListItem>();
        //    for (var i = drpyear; i <= drprangeyear; i++)
        //    {
        //        if (i == timezone.gettimezone().Year)
        //        {
        //            yearlist2.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString(), Selected = true });
        //        }
        //        else
        //        {
        //            yearlist2.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
        //        }
        //    }

        //    ViewBag.MonthList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "monthlist" && x.fldDeleted == false), "fldOptConfValue", "fldOptConfDesc", month);
        //    ViewBag.StatusApprove = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "statusapproval2" && x.fldDeleted == false), "fldOptConfValue", "fldOptConfDesc").ToList();

        //    ViewBag.YearList = yearlist2;
        //    ViewBag.WilayahIDList = WilayahIDList2;
        //    ViewBag.LadangIDList = LadangIDList2;
        //    ViewBag.NeragaID = NegaraID;
        //    ViewBag.SyarikatID = SyarikatID;
        //    ViewBag.NoPekerjaList = NoPekerjaList2;
        //    ViewBag.PekerjaSelection = NoPekerjaList;
        //    ViewBag.WilayahSelection = wilayahselection;
        //    ViewBag.LadangSelection = ladangselection;
        //    ViewBag.Year = year;
        //    ViewBag.Month = MonthList;
        //    ViewBag.Selectionrpt = selectionrpt;
        //    ViewBag.IncLdg = incldg;
        //    ViewBag.UserID = getuserid;
        //    ViewBag.getflag = getflag;
        //    List<sp_RptLapMakKer_Result> resultreport = new List<sp_RptLapMakKer_Result>();
        //    if (NoPekerjaList == "0")
        //    {
        //        //db3.SetCommandTimeout(120);
        //        resultreport = db3.sp_RptLapMakKer(NegaraID, SyarikatID, WilayahIDList, LadangIDList, MonthList, yearlist, "0").ToList();
        //        ViewBag.DataCount = resultreport.Count();
        //        ViewBag.TotalUnit = resultreport.Select(s => s.fld_Kti1).Sum();
        //        ViewBag.TotalTask = resultreport.Select(s => s.fld_Kti3).Sum();
        //        ViewBag.TotalKong = resultreport.Select(s => s.fld_Kong).Sum();
        //        ViewBag.TotalKdrot = resultreport.Select(s => s.fld_Kdrot).Sum();
        //        ViewBag.TotalOt = resultreport.Select(s => s.fld_Jamot).Sum();
        //        ViewBag.TotalKuantiti = resultreport.Select(s => s.fld_Qty).Sum();
        //        ViewBag.TotalAmt = resultreport.Select(s => s.fld_Amt).Sum();
        //        ViewBag.TotalJumlah = resultreport.Select(s => s.fld_Jumlah).Sum();
        //    }
        //    else
        //    {
        //        //db3.SetCommandTimeout(120);
        //        resultreport = db3.sp_RptLapMakKer(NegaraID, SyarikatID, WilayahIDList, LadangIDList, MonthList, yearlist, NoPekerjaList).ToList();
        //        ViewBag.DataCount = resultreport.Count();
        //        ViewBag.TotalUnit = resultreport.Select(s => s.fld_Kti1).Sum();
        //        ViewBag.TotalTask = resultreport.Select(s => s.fld_Kti3).Sum();
        //        ViewBag.TotalKong = resultreport.Select(s => s.fld_Kong).Sum();
        //        ViewBag.TotalKdrot = resultreport.Select(s => s.fld_Kdrot).Sum();
        //        ViewBag.TotalOt = resultreport.Select(s => s.fld_Jamot).Sum();
        //        ViewBag.TotalKuantiti = resultreport.Select(s => s.fld_Qty).Sum();
        //        ViewBag.TotalAmt = resultreport.Select(s => s.fld_Amt).Sum();
        //        ViewBag.TotalJumlah = resultreport.Select(s => s.fld_Jumlah).Sum();
        //    }
        //    return View();
        //}

        //[AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3,Viewer")]
        //public ActionResult WorkerReportDetail(int negaraid, int syarikatid, int wilayahid, int ladangid, int month, int year, int incldg, string nopkj)
        //{
        //    ViewBag.IncLdg = incldg;
        //    //ViewBag.Status = statusapprove;
        //    ViewBag.Month = month;
        //    ViewBag.Year = year;
        //    ViewBag.NoPekerja = nopkj;

        //    ViewBag.NamaSyarikat = db2.tbl_Syarikat.Where(x => x.fld_SyarikatID == syarikatid && x.fld_NegaraID == negaraid).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
        //    ViewBag.NoSyarikat = db2.tbl_Syarikat.Where(x => x.fld_SyarikatID == syarikatid && x.fld_NegaraID == negaraid).Select(s => s.fld_NoSyarikat).FirstOrDefault();
        //    ViewBag.WilayahName = db2.tbl_Wilayah.Where(x => x.fld_ID == wilayahid && x.fld_SyarikatID == syarikatid).Select(s => s.fld_WlyhName).FirstOrDefault();
        //    ViewBag.LadangCode = db2.tbl_Ladang.Where(x => x.fld_ID == ladangid && x.fld_WlyhID == wilayahid).Select(s => s.fld_LdgCode).FirstOrDefault();
        //    ViewBag.LadangName = db2.tbl_Ladang.Where(x => x.fld_ID == ladangid && x.fld_WlyhID == wilayahid).Select(s => s.fld_LdgName).FirstOrDefault();

        //    List<sp_RptLapMakKer_Result> resultreport = new List<sp_RptLapMakKer_Result>();
        //    //db3.SetCommandTimeout(120);
        //    resultreport = db3.sp_RptLapMakKer(negaraid, syarikatid, wilayahid, ladangid, month, year, nopkj).ToList();
        //    ViewBag.DataCount = resultreport.Count();
        //    ViewBag.Kump = resultreport.Select(s => s.fld_Kum).Take(1).FirstOrDefault();
        //    ViewBag.NamaPekerja = resultreport.Select(s => s.fld_Nama1).Take(1).FirstOrDefault();
        //    ViewBag.NegaraID = negaraid;
        //    ViewBag.SyarikatID = syarikatid;
        //    ViewBag.WilayahID = wilayahid;
        //    ViewBag.LadangID = ladangid;
        //    return View("WorkerReportDetail", resultreport);
        //}

        //public ActionResult WorkerReportSummary(int negaraid, int syarikatid, int wilayahid, int ladangid, int month, int year, int incldg, string nopkj, int bill)
        //{
        //    ViewBag.NamaSyarikat = db2.tbl_Syarikat.Where(x => x.fld_SyarikatID == syarikatid && x.fld_NegaraID == negaraid).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
        //    ViewBag.NoSyarikat = db2.tbl_Syarikat.Where(x => x.fld_SyarikatID == syarikatid && x.fld_NegaraID == negaraid).Select(s => s.fld_NoSyarikat).FirstOrDefault();
        //    ViewBag.WilayahName = db2.tbl_Wilayah.Where(x => x.fld_ID == wilayahid && x.fld_SyarikatID == syarikatid).Select(s => s.fld_WlyhName).FirstOrDefault();
        //    ViewBag.LadangCode = db2.tbl_Ladang.Where(x => x.fld_ID == ladangid && x.fld_WlyhID == wilayahid).Select(s => s.fld_LdgCode).FirstOrDefault();
        //    ViewBag.LadangName = db2.tbl_Ladang.Where(x => x.fld_ID == ladangid && x.fld_WlyhID == wilayahid).Select(s => s.fld_LdgName).FirstOrDefault();
        //    ViewBag.NoPekerja = nopkj;

        //    List<sp_RptLapMakKer_Result> resultreport = new List<sp_RptLapMakKer_Result>();
        //    //db3.SetCommandTimeout(120);
        //    resultreport = db3.sp_RptLapMakKer(negaraid, syarikatid, wilayahid, ladangid, month, year, nopkj).ToList();
        //    ViewBag.DataCount = resultreport.Count();
        //    ViewBag.Kump = resultreport.Select(s => s.fld_Kum).FirstOrDefault();
        //    ViewBag.NamaPekerja = resultreport.Select(s => s.fld_Nama1).FirstOrDefault();
        //    //ViewBag.TotalUnit = resultreport.Select(s => s.fld_Kti1).Sum();
        //    //ViewBag.TotalTask = resultreport.Select(s => s.fld_Kti3).Sum();
        //    //ViewBag.TotalKong = resultreport.Select(s => s.fld_Kong).Sum();
        //    //ViewBag.TotalKdrot = resultreport.Select(s => s.fld_Kdrot).Sum();
        //    //ViewBag.TotalOt = resultreport.Select(s => s.fld_Jamot).Sum();
        //    //ViewBag.TotalKuantiti = resultreport.Select(s => s.fld_Qty).Sum();
        //    //ViewBag.TotalAmt = resultreport.Select(s => s.fld_Amt).Sum();
        //    ViewBag.Bill = bill;
        //    ViewBag.TotalJumlah = resultreport.Select(s => s.fld_Jumlah).Sum();

        //    return View("WorkerReportSummary", resultreport);
        //}

        //[AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3,Viewer")]
        //public ActionResult Paysheet()
        //{
        //    int[] wlyhid = new int[] { };
        //    //string mywlyid = "";
        //    int? NegaraID = 0;
        //    int? SyarikatID = 0;
        //    int? WilayahID = 0;
        //    int? LadangID = 0;
        //    int year = timezone.gettimezone().Year;
        //    int month = timezone.gettimezone().AddMonths(-1).Month;
        //    int drpyear = 0;
        //    int drprangeyear = 0;
        //    int? getuserid = getidentity.ID(User.Identity.Name);
        //    int? selectionrpt = 0;
        //    int? wilayahselection = 0;
        //    int? ladangselection = 0;
        //    int incldg = 0;
        //    int getflag = 0;


        //    ViewBag.Report = "class = active";

        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

        //    List<SelectListItem> WilayahIDList = new List<SelectListItem>();
        //    List<SelectListItem> LadangIDList = new List<SelectListItem>();

        //    if (WilayahID == 0 && LadangID == 0)
        //    {
        //        wlyhid = getwilyah.GetWilayahID(SyarikatID);
        //        // mywlyid = String.Join("", wlyhid); ;
        //        WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)).OrderBy(o => o.fld_ID), "fld_ID", "fld_WlyhName").ToList();
        //        WilayahIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
        //        LadangIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        selectionrpt = 1;
        //        wilayahselection = WilayahID;
        //        ladangselection = LadangID;
        //        incldg = 1;
        //        getflag = 1;
        //    }
        //    else if (WilayahID != 0 && LadangID == 0)
        //    {
        //        //mywlyid = String.Join("", WilayahID); ;
        //        wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
        //        WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
        //        LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
        //        LadangIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        selectionrpt = 2;
        //        wilayahselection = WilayahID;
        //        ladangselection = LadangID;
        //        incldg = 2;
        //        getflag = 1;

        //    }
        //    else if (WilayahID != 0 && LadangID != 0)
        //    {
        //        //mywlyid = String.Join("", WilayahID); ;
        //        wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
        //        WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
        //        LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_ID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
        //        selectionrpt = 3;
        //        wilayahselection = WilayahID;
        //        ladangselection = LadangID;
        //        incldg = 3;
        //        getflag = 1;
        //    }
        //    drpyear = timezone.gettimezone().Year - int.Parse(GetConfig.GetData("yeardisplay")) + 1;
        //    drprangeyear = timezone.gettimezone().Year;

        //    var yearlist = new List<SelectListItem>();
        //    for (var i = drpyear; i <= drprangeyear; i++)
        //    {
        //        if (i == timezone.gettimezone().Year)
        //        {
        //            yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString(), Selected = true });
        //        }
        //        else
        //        {
        //            yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
        //        }
        //    }

        //    ViewBag.MonthList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "monthlist" && x.fldDeleted == false), "fldOptConfValue", "fldOptConfDesc", month);

        //    ViewBag.YearList = yearlist;
        //    ViewBag.WilayahIDList = WilayahIDList;
        //    ViewBag.LadangIDList = LadangIDList;
        //    ViewBag.NeragaID = NegaraID;
        //    ViewBag.SyarikatID = SyarikatID;
        //    ViewBag.WilayahSelection = wilayahselection;
        //    ViewBag.LadangSelection = ladangselection;
        //    ViewBag.Year = year;
        //    ViewBag.Month = month;
        //    ViewBag.Selectionrpt = selectionrpt;
        //    ViewBag.IncLdg = incldg;
        //    ViewBag.UserID = getuserid;
        //    ViewBag.getflag = getflag;

        //    return View();
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //[AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3,Viewer")]
        //public ActionResult Paysheet(int MonthList, int yearlist, int WilayahIDList, int LadangIDList)
        //{
        //    int[] wlyhid = new int[] { };
        //    //string mywlyid = "";
        //    int? NegaraID = 0;
        //    int? SyarikatID = 0;
        //    int? WilayahID = 0;
        //    int? LadangID = 0;
        //    int year = timezone.gettimezone().Year;
        //    int month = timezone.gettimezone().AddMonths(-1).Month;
        //    int drpyear = 0;
        //    int drprangeyear = 0;
        //    int? getuserid = getidentity.ID(User.Identity.Name);
        //    int? selectionrpt = 0;
        //    int? wilayahselection = 0;
        //    int? ladangselection = 0;
        //    int incldg = 0;
        //    int getflag = 0;


        //    ViewBag.Report = "class = active";

        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

        //    List<SelectListItem> WilayahIDList2 = new List<SelectListItem>();
        //    List<SelectListItem> LadangIDList2 = new List<SelectListItem>();

        //    if (WilayahID == 0 && LadangID == 0)
        //    {
        //        wlyhid = getwilyah.GetWilayahID(SyarikatID);
        //        // mywlyid = String.Join("", wlyhid); ;
        //        WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)).OrderBy(o => o.fld_ID), "fld_ID", "fld_WlyhName").ToList();
        //        WilayahIDList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => x.fld_WlyhID == WilayahIDList && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
        //        LadangIDList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        selectionrpt = 1;
        //        wilayahselection = WilayahIDList;
        //        ladangselection = LadangIDList;
        //        incldg = 1;
        //        getflag = 2;
        //    }
        //    else if (WilayahID != 0 && LadangID == 0)
        //    {
        //        //mywlyid = String.Join("", WilayahID); ;
        //        wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
        //        WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
        //        LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => x.fld_WlyhID == WilayahIDList && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
        //        LadangIDList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        selectionrpt = 2;
        //        wilayahselection = WilayahIDList;
        //        ladangselection = LadangIDList;
        //        incldg = 2;
        //        getflag = 2;

        //    }
        //    else if (WilayahID != 0 && LadangID != 0)
        //    {
        //        //mywlyid = String.Join("", WilayahID); ;
        //        wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
        //        WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
        //        LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => x.fld_WlyhID == WilayahIDList && x.fld_ID == LadangIDList && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
        //        selectionrpt = 3;
        //        wilayahselection = WilayahIDList;
        //        ladangselection = LadangIDList;
        //        incldg = 3;
        //        getflag = 2;
        //    }
        //    drpyear = timezone.gettimezone().Year - int.Parse(GetConfig.GetData("yeardisplay")) + 1;
        //    drprangeyear = timezone.gettimezone().Year;

        //    var yearlist2 = new List<SelectListItem>();
        //    for (var i = drpyear; i <= drprangeyear; i++)
        //    {
        //        if (i == timezone.gettimezone().Year)
        //        {
        //            yearlist2.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString(), Selected = true });
        //        }
        //        else
        //        {
        //            yearlist2.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
        //        }
        //    }

        //    ViewBag.MonthList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "monthlist" && x.fldDeleted == false), "fldOptConfValue", "fldOptConfDesc", month);

        //    ViewBag.YearList = yearlist2;
        //    ViewBag.WilayahIDList = WilayahIDList2;
        //    ViewBag.LadangIDList = LadangIDList2;
        //    ViewBag.NeragaID = NegaraID;
        //    ViewBag.SyarikatID = SyarikatID;
        //    ViewBag.WilayahSelection = wilayahselection;
        //    ViewBag.LadangSelection = ladangselection;
        //    ViewBag.Year = year;
        //    ViewBag.Month = MonthList;
        //    ViewBag.Selectionrpt = selectionrpt;
        //    ViewBag.IncLdg = incldg;
        //    ViewBag.UserID = getuserid;
        //    ViewBag.getflag = getflag;
        //    return View();
        //}

        //[AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3,Viewer")]
        //public ActionResult PaysheetDetail(int negaraid, int syarikatid, int wilayahid, int ladangid, int month, int year, int incldg)
        //{
        //    decimal? JumlahGajiKasar = 0;
        //    decimal? JumlahKwspMajikan = 0;
        //    decimal? JumlahSocsoMajikan = 0;
        //    decimal? JumlahPendapatanSumbangan = 0;
        //    decimal? JumlahKwspMajikanPekerja = 0;
        //    decimal? JumlahSocsoMajikanPekerja = 0;
        //    decimal? JUmlahPotongan = 0;
        //    decimal? JumlahBersih = 0;

        //    var resultreport = db.vw_GajiBulanan.Where(x => x.fld_NegaraID == negaraid && x.fld_SyarikatID == syarikatid && x.fld_WilayahID == wilayahid && x.fld_LadangID == ladangid && x.fld_Month == month && x.fld_Year == year);
        //    ViewBag.DataCount = resultreport.Count();
        //    if (ViewBag.DataCount >= 1)
        //    {
        //        JumlahGajiKasar = resultreport.Sum(s => s.fld_Gaji_Kasar);
        //        JumlahKwspMajikan = resultreport.Sum(s => s.fld_Epf_Mjk);
        //        JumlahSocsoMajikan = resultreport.Sum(s => s.fld_Socso_Mjk);
        //        JumlahPendapatanSumbangan = (JumlahGajiKasar) + (JumlahKwspMajikan + JumlahSocsoMajikan);
        //        JumlahKwspMajikanPekerja = (JumlahKwspMajikan) + (resultreport.Sum(s => s.fld_Epf_Pkj));
        //        JumlahSocsoMajikanPekerja = (JumlahSocsoMajikan) + (resultreport.Sum(s => s.fld_Socso_Pkj));
        //        JUmlahPotongan = JumlahKwspMajikanPekerja + JumlahSocsoMajikanPekerja;
        //        JumlahBersih = JumlahPendapatanSumbangan + JUmlahPotongan;
        //    }

        //    ViewBag.JumlahGajiKasar = GetTriager.GetTotalForMoney(JumlahGajiKasar);
        //    ViewBag.JumlahKwspMajikan = GetTriager.GetTotalForMoney(JumlahKwspMajikan);
        //    ViewBag.JumlahSocsoMajikan = GetTriager.GetTotalForMoney(JumlahSocsoMajikan);
        //    ViewBag.JumlahPendapatanSumbangan = GetTriager.GetTotalForMoney(JumlahPendapatanSumbangan);
        //    ViewBag.JumlahKwspMajikanPekerja = GetTriager.GetTotalForMoney(JumlahKwspMajikanPekerja);
        //    ViewBag.JumlahSocsoMajikanPekerja = GetTriager.GetTotalForMoney(JumlahSocsoMajikanPekerja);
        //    ViewBag.JUmlahPotongan = GetTriager.GetTotalForMoney(JUmlahPotongan);
        //    ViewBag.JumlahBersih = GetTriager.GetTotalForMoney(JumlahBersih);

        //    ViewBag.IncLdg = incldg;
        //    ViewBag.Month = month;
        //    ViewBag.Year = year;
        //    ViewBag.NamaSyarikat = db2.tbl_Syarikat.Where(x => x.fld_SyarikatID == syarikatid && x.fld_NegaraID == negaraid).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
        //    ViewBag.NoSyarikat = db2.tbl_Syarikat.Where(x => x.fld_SyarikatID == syarikatid && x.fld_NegaraID == negaraid).Select(s => s.fld_NoSyarikat).FirstOrDefault();
        //    ViewBag.WilayahName = db2.tbl_Wilayah.Where(x => x.fld_ID == wilayahid && x.fld_SyarikatID == syarikatid).Select(s => s.fld_WlyhName).FirstOrDefault();
        //    ViewBag.LadangCode = db2.tbl_Ladang.Where(x => x.fld_ID == ladangid && x.fld_WlyhID == wilayahid).Select(s => s.fld_LdgCode).FirstOrDefault();
        //    ViewBag.LadangName = db2.tbl_Ladang.Where(x => x.fld_ID == ladangid && x.fld_WlyhID == wilayahid).Select(s => s.fld_LdgName).FirstOrDefault();

        //    return View("PaysheetDetail", resultreport);
        //}

        //[AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3,Viewer")]
        //public ActionResult WorkerList()
        //{
        //    int[] wlyhid = new int[] { };
        //    //string mywlyid = "";
        //    int? NegaraID = 0;
        //    int? SyarikatID = 0;
        //    int? WilayahID = 0;
        //    int? LadangID = 0;
        //    int year = timezone.gettimezone().Year;
        //    int month = timezone.gettimezone().AddMonths(-1).Month;
        //    int? getuserid = getidentity.ID(User.Identity.Name);
        //    int? selectionrpt = 0;
        //    int? wilayahselection = 0;
        //    int? ladangselection = 0;
        //    int incldg = 0;
        //    string statusselection = " ";
        //    string pekerjaselection = " ";

        //    ViewBag.Report = "class = active";

        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

        //    List<SelectListItem> WilayahIDList = new List<SelectListItem>();
        //    List<SelectListItem> LadangIDList = new List<SelectListItem>();

        //    if (WilayahID == 0 && LadangID == 0)
        //    {
        //        wlyhid = getwilyah.GetWilayahID(SyarikatID);
        //        // mywlyid = String.Join("", wlyhid); ;
        //        WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
        //        WilayahIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
        //        LadangIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        selectionrpt = 1;
        //        wilayahselection = WilayahID;
        //        ladangselection = LadangID;
        //        incldg = 1;
        //    }
        //    else if (WilayahID != 0 && LadangID == 0)
        //    {
        //        //mywlyid = String.Join("", WilayahID); ;
        //        wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
        //        WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
        //        LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
        //        LadangIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        selectionrpt = 2;
        //        wilayahselection = WilayahID;
        //        ladangselection = LadangID;
        //        incldg = 2;

        //    }
        //    else if (WilayahID != 0 && LadangID != 0)
        //    {
        //        //mywlyid = String.Join("", WilayahID); ;
        //        wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
        //        WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
        //        LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_ID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
        //        selectionrpt = 3;
        //        wilayahselection = WilayahID;
        //        ladangselection = LadangID;
        //        incldg = 3;
        //    }

        //    List<SelectListItem> StatusList = new List<SelectListItem>();
        //    StatusList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "statusaktif" && x.fldDeleted == false), "fldOptConfValue", "fldOptConfDesc", 1).ToList();
        //    StatusList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //    statusselection = "0";

        //    List<SelectListItem> NoPekerjaList = new List<SelectListItem>();
        //    NoPekerjaList = new SelectList(db.vw_DetailPekerja.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).Select(s => new SelectListItem { Value = s.fld_Nopkj.Trim(), Text = s.fld_Nopkj.Trim() + " - " + s.fld_Nama1.Trim() }), "Value", "Text").Distinct().Take(50).ToList();
        //    NoPekerjaList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //    pekerjaselection = "0";
        //    //var getpekerja = db.vw_KerjaHarian.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == wilayahselection && x.fld_LadangID == ladangselection).Select(s => s.fld_Nopkj.Trim()).Distinct().ToArray();
        //    //NoPekerjaList = new SelectList(db.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == wilayahselection && x.fld_LadangID == ladangselection && getpekerja.Contains(x.fld_Nopkj.Trim())).Select(s => new SelectListItem { Value = s.fld_Nopkj.Trim(), Text = s.fld_Nopkj.Trim() + " - " + s.fld_Nama1.Trim() }), "Value", "Text").Distinct().ToList();
        //    //NoPekerjaList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));

        //    //ViewBag.MonthList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "exprdmonthlist" && x.fldDeleted == false), "fldOptConfValue", "fldOptConfDesc", month);
        //    //ViewBag.GetAllKrytan = db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "krytnlist" && x.fldDeleted == false).Select(s => s.fldOptConfValue).ToList();
        //    ViewBag.WilayahIDList = WilayahIDList;
        //    ViewBag.LadangIDList = LadangIDList;
        //    ViewBag.StatusList = StatusList;
        //    ViewBag.NoPekerjaList = NoPekerjaList;
        //    ViewBag.NeragaID = NegaraID;
        //    ViewBag.SyarikatID = SyarikatID;
        //    ViewBag.WilayahSelection = wilayahselection;
        //    ViewBag.LadangSelection = ladangselection;
        //    ViewBag.StatusSelection = statusselection;
        //    ViewBag.PekerjaSelection = pekerjaselection;
        //    ViewBag.Year = year;
        //    ViewBag.Month = month;
        //    ViewBag.Selectionrpt = selectionrpt;
        //    ViewBag.IncLdg = incldg;
        //    ViewBag.UserID = getuserid;
        //    ViewBag.GetFlag = 1;

        //    return View();
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //[AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3,Viewer")]
        //public ActionResult WorkerList(string StatusList, int WilayahIDList, int LadangIDList, string NoPekerjaList, string PekerjaSearch)
        //{
        //    int[] wlyhid = new int[] { };
        //    //string mywlyid = "";
        //    int? NegaraID = 0;
        //    int? SyarikatID = 0;
        //    int? WilayahID = 0;
        //    int? LadangID = 0;
        //    int year = timezone.gettimezone().Year;
        //    int month = timezone.gettimezone().AddMonths(-1).Month;
        //    int? getuserid = getidentity.ID(User.Identity.Name);
        //    int? selectionrpt = 0;
        //    int? wilayahselection = 0;
        //    int? ladangselection = 0;
        //    int incldg = 0;
        //    string statusselection = " ";
        //    string pekerjaselection = " ";

        //    ViewBag.Report = "class = active";

        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

        //    List<SelectListItem> WilayahIDList2 = new List<SelectListItem>();
        //    List<SelectListItem> LadangIDList2 = new List<SelectListItem>();

        //    if (WilayahID == 0 && LadangID == 0)
        //    {
        //        wlyhid = getwilyah.GetWilayahID(SyarikatID);
        //        // mywlyid = String.Join("", wlyhid); ;
        //        WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
        //        WilayahIDList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        if (WilayahIDList == 0)
        //        {
        //            LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
        //        }
        //        else
        //        {
        //            LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => x.fld_WlyhID == WilayahIDList && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
        //        }
        //        //LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
        //        LadangIDList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        selectionrpt = 1;
        //        wilayahselection = WilayahIDList;
        //        ladangselection = LadangIDList;
        //        incldg = 1;
        //    }
        //    else if (WilayahID != 0 && LadangID == 0)
        //    {
        //        //mywlyid = String.Join("", WilayahID); ;
        //        wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
        //        WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
        //        LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
        //        LadangIDList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        selectionrpt = 2;
        //        wilayahselection = WilayahIDList;
        //        ladangselection = LadangIDList;
        //        incldg = 2;

        //    }
        //    else if (WilayahID != 0 && LadangID != 0)
        //    {
        //        //mywlyid = String.Join("", WilayahID); ;
        //        wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
        //        WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
        //        LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_ID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
        //        selectionrpt = 3;
        //        wilayahselection = WilayahIDList;
        //        ladangselection = LadangIDList;
        //        incldg = 3;
        //    }

        //    List<SelectListItem> StatusList2 = new List<SelectListItem>();
        //    StatusList2 = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "statusaktif" && x.fldDeleted == false), "fldOptConfValue", "fldOptConfDesc").ToList();
        //    StatusList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //    statusselection = StatusList;

        //    List<SelectListItem> NoPekerjaList2 = new List<SelectListItem>();
        //    if (wilayahselection == 0)
        //    {
        //        NoPekerjaList2 = new SelectList(db.vw_DetailPekerja.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).Select(s => new SelectListItem { Value = s.fld_Nopkj.Trim(), Text = s.fld_Nopkj.Trim() + " - " + s.fld_Nama1.Trim() }), "Value", "Text").Distinct().Take(50).ToList();
        //        NoPekerjaList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        pekerjaselection = NoPekerjaList;
        //    }
        //    else if (wilayahselection != 0 && ladangselection == 0)
        //    {
        //        NoPekerjaList2 = new SelectList(db.vw_DetailPekerja.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == wilayahselection).Select(s => new SelectListItem { Value = s.fld_Nopkj.Trim(), Text = s.fld_Nopkj.Trim() + " - " + s.fld_Nama1.Trim() }), "Value", "Text").Distinct().Take(50).ToList();
        //        NoPekerjaList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        pekerjaselection = NoPekerjaList;
        //    }
        //    else
        //    {
        //        NoPekerjaList2 = new SelectList(db.vw_DetailPekerja.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == wilayahselection && x.fld_LadangID == ladangselection).Select(s => new SelectListItem { Value = s.fld_Nopkj.Trim(), Text = s.fld_Nopkj.Trim() + " - " + s.fld_Nama1.Trim() }), "Value", "Text").Distinct().Take(50).ToList();
        //        NoPekerjaList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        pekerjaselection = NoPekerjaList;
        //    }


        //    //List<SelectListItem> KerakyatanList2 = new List<SelectListItem>();
        //    //KerakyatanList2 = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "krytnlist" && x.fldDeleted == false), "fldOptConfValue", "fldOptConfDesc").ToList();
        //    //KerakyatanList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "ALL" }));

        //    //ViewBag.MonthList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "exprdmonthlist" && x.fldDeleted == false), "fldOptConfValue", "fldOptConfDesc", month);
        //    //ViewBag.GetAllKrytan = db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "krytnlist" && x.fldDeleted == false).Select(s => s.fldOptConfValue).ToList();

        //    //List<vw_DetailPekerja> resultreport = new List<vw_DetailPekerja>();
        //    ViewBag.NamaSyarikat = db2.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
        //    ViewBag.NoSyarikat = db2.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();

        //    ViewBag.WilayahIDList = WilayahIDList2;
        //    ViewBag.LadangIDList = LadangIDList2;
        //    ViewBag.StatusList = StatusList2;
        //    ViewBag.NoPekerjaList = NoPekerjaList2;
        //    ViewBag.NeragaID = NegaraID;
        //    ViewBag.SyarikatID = SyarikatID;
        //    ViewBag.WilayahSelection = wilayahselection;
        //    ViewBag.LadangSelection = ladangselection;
        //    ViewBag.StatusSelection = statusselection;
        //    ViewBag.PekerjaSelection = pekerjaselection;
        //    ViewBag.Search = PekerjaSearch;
        //    //ViewBag.RakyatSelection = KerakyatanList;
        //    ViewBag.Year = year;
        //    //ViewBag.Month = MonthList;
        //    ViewBag.Selectionrpt = selectionrpt;
        //    ViewBag.IncLdg = incldg;
        //    ViewBag.UserID = getuserid;
        //    ViewBag.GetFlag = 2;

        //    return View();
        //}

        //[AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3,Viewer")]
        //public ActionResult WorkerListDetail(int negaraid, int syarikatid, string statuslist, int wilayahid, int ladangid, string nopekerja, int incldg, string searchpekerja)
        //{
        //    //ViewBag.Month = month;
        //    string status = "";
        //    ViewBag.NamaSyarikat = db2.tbl_Syarikat.Where(x => x.fld_SyarikatID == syarikatid && x.fld_NegaraID == negaraid).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
        //    ViewBag.NoSyarikat = db2.tbl_Syarikat.Where(x => x.fld_SyarikatID == syarikatid && x.fld_NegaraID == negaraid).Select(s => s.fld_NoSyarikat).FirstOrDefault();
        //    ViewBag.WilayahName = db2.tbl_Wilayah.Where(x => x.fld_ID == wilayahid && x.fld_SyarikatID == syarikatid).Select(s => s.fld_WlyhName).FirstOrDefault();
        //    ViewBag.LadangCode = db2.tbl_Ladang.Where(x => x.fld_ID == ladangid && x.fld_WlyhID == wilayahid).Select(s => s.fld_LdgCode).FirstOrDefault();
        //    ViewBag.LadangName = db2.tbl_Ladang.Where(x => x.fld_ID == ladangid && x.fld_WlyhID == wilayahid).Select(s => s.fld_LdgName).FirstOrDefault();
        //    switch (statuslist)
        //    {
        //        case "0":
        //            status = " ";
        //            break;
        //        case "1":
        //            status = "0";
        //            break;
        //        case "2":
        //            status = "1";
        //            break;
        //    }
        //    if (searchpekerja == "")
        //    {
        //        if (statuslist == "0" && nopekerja == "0")
        //        {
        //            var resultreport = db.vw_DetailPekerja.Where(x => x.fld_NegaraID == negaraid && x.fld_SyarikatID == syarikatid && x.fld_WilayahID == wilayahid && x.fld_LadangID == ladangid);
        //            ViewBag.DataCount = resultreport.Count();
        //            return View("WorkerListDetail", resultreport);
        //        }
        //        else if (statuslist == "0" && nopekerja != "0")
        //        {
        //            var resultreport = db.vw_DetailPekerja.Where(x => x.fld_NegaraID == negaraid && x.fld_SyarikatID == syarikatid && x.fld_WilayahID == wilayahid && x.fld_LadangID == ladangid && x.fld_Nopkj == nopekerja);
        //            ViewBag.DataCount = resultreport.Count();
        //            return View("WorkerListDetail", resultreport);
        //        }
        //        else if (statuslist != "0" && nopekerja == "0")
        //        {
        //            var resultreport = db.vw_DetailPekerja.Where(x => x.fld_NegaraID == negaraid && x.fld_SyarikatID == syarikatid && x.fld_WilayahID == wilayahid && x.fld_LadangID == ladangid && x.fld_Kdaktf == status);
        //            ViewBag.DataCount = resultreport.Count();
        //            return View("WorkerListDetail", resultreport);
        //        }
        //        else
        //        {
        //            var resultreport = db.vw_DetailPekerja.Where(x => x.fld_NegaraID == negaraid && x.fld_SyarikatID == syarikatid && x.fld_WilayahID == wilayahid && x.fld_LadangID == ladangid && x.fld_Nopkj == nopekerja && x.fld_Kdaktf == status);
        //            ViewBag.DataCount = resultreport.Count();
        //            return View("WorkerListDetail", resultreport);
        //        }
        //    }
        //    else
        //    {
        //        if (statuslist == "0")
        //        {
        //            var resultreport = db.vw_DetailPekerja.Where(x => x.fld_NegaraID == negaraid && x.fld_SyarikatID == syarikatid && x.fld_WilayahID == wilayahid && x.fld_LadangID == ladangid);
        //            var result = resultreport.Where(x => x.fld_Nopkj.Contains(searchpekerja) || x.fld_Nama1.Contains(searchpekerja));
        //            ViewBag.DataCount = result.Count();
        //            return View("WorkerListDetail", result);
        //        }
        //        else
        //        {
        //            var resultreport = db.vw_DetailPekerja.Where(x => x.fld_NegaraID == negaraid && x.fld_SyarikatID == syarikatid && x.fld_WilayahID == wilayahid && x.fld_LadangID == ladangid && x.fld_Kdaktf == status);
        //            var result = resultreport.Where(x => x.fld_Nopkj.Contains(searchpekerja) || x.fld_Nama1.Contains(searchpekerja));
        //            ViewBag.DataCount = result.Count();
        //            return View("WorkerListDetail", result);
        //        }


        //        //            var resultreport = db.vw_DetailPekerja.Where(x => x.fld_NegaraID == negaraid && x.fld_SyarikatID == syarikatid && x.fld_WilayahID == wilayahid && x.fld_LadangID == ladangid);
        //        //var result = resultreport.Where(x =>x.fld_Nopkj.Contains(searchpekerja) || x.fld_Nama1.Contains(searchpekerja));
        //        //ViewBag.DataCount = result.Count();
        //        //return View("WorkerListDetail", result);
        //    }
        //}

        //[AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3,Viewer")]
        //public ActionResult PaysheetByLadang()
        //{
        //    int[] wlyhid = new int[] { };
        //    //string mywlyid = "";
        //    int? NegaraID = 0;
        //    int? SyarikatID = 0;
        //    int? WilayahID = 0;
        //    int? LadangID = 0;
        //    int year = timezone.gettimezone().Year;
        //    int month = timezone.gettimezone().AddMonths(-1).Month;
        //    int drpyear = 0;
        //    int drprangeyear = 0;
        //    int? getuserid = getidentity.ID(User.Identity.Name);
        //    int? selectionrpt = 0;
        //    int? wilayahselection = 0;
        //    int? ladangselection = 0;
        //    int incldg = 0;
        //    int getflag = 0;


        //    ViewBag.Report = "class = active";

        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

        //    List<SelectListItem> WilayahIDList = new List<SelectListItem>();
        //    List<SelectListItem> LadangIDList = new List<SelectListItem>();

        //    if (WilayahID == 0 && LadangID == 0)
        //    {
        //        wlyhid = getwilyah.GetWilayahID(SyarikatID);
        //        // mywlyid = String.Join("", wlyhid); ;
        //        WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)).OrderBy(o => o.fld_ID), "fld_ID", "fld_WlyhName").ToList();
        //        WilayahIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
        //        LadangIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        selectionrpt = 1;
        //        wilayahselection = WilayahID;
        //        ladangselection = LadangID;
        //        incldg = 1;
        //        getflag = 1;
        //    }
        //    else if (WilayahID != 0 && LadangID == 0)
        //    {
        //        //mywlyid = String.Join("", WilayahID); ;
        //        wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
        //        WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
        //        LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
        //        LadangIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        selectionrpt = 2;
        //        wilayahselection = WilayahID;
        //        ladangselection = LadangID;
        //        incldg = 2;
        //        getflag = 1;

        //    }
        //    else if (WilayahID != 0 && LadangID != 0)
        //    {
        //        //mywlyid = String.Join("", WilayahID); ;
        //        wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
        //        WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
        //        LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_ID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
        //        selectionrpt = 3;
        //        wilayahselection = WilayahID;
        //        ladangselection = LadangID;
        //        incldg = 3;
        //        getflag = 1;
        //    }
        //    drpyear = timezone.gettimezone().Year - int.Parse(GetConfig.GetData("yeardisplay")) + 1;
        //    drprangeyear = timezone.gettimezone().Year;

        //    var yearlist = new List<SelectListItem>();
        //    for (var i = drpyear; i <= drprangeyear; i++)
        //    {
        //        if (i == timezone.gettimezone().Year)
        //        {
        //            yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString(), Selected = true });
        //        }
        //        else
        //        {
        //            yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
        //        }
        //    }

        //    ViewBag.MonthList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "monthlist" && x.fldDeleted == false), "fldOptConfValue", "fldOptConfDesc", month);

        //    ViewBag.YearList = yearlist;
        //    ViewBag.WilayahIDList = WilayahIDList;
        //    ViewBag.LadangIDList = LadangIDList;
        //    ViewBag.NeragaID = NegaraID;
        //    ViewBag.SyarikatID = SyarikatID;
        //    ViewBag.WilayahSelection = wilayahselection;
        //    ViewBag.LadangSelection = ladangselection;
        //    ViewBag.Year = year;
        //    ViewBag.Month = month;
        //    ViewBag.Selectionrpt = selectionrpt;
        //    ViewBag.IncLdg = incldg;
        //    ViewBag.UserID = getuserid;
        //    ViewBag.getflag = getflag;

        //    return View();
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //[AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3,Viewer")]
        //public ActionResult PaysheetByLadang(int MonthList, int yearlist, int WilayahIDList, int LadangIDList)
        //{
        //    int[] wlyhid = new int[] { };
        //    //string mywlyid = "";
        //    int? NegaraID = 0;
        //    int? SyarikatID = 0;
        //    int? WilayahID = 0;
        //    int? LadangID = 0;
        //    int year = timezone.gettimezone().Year;
        //    int month = timezone.gettimezone().AddMonths(-1).Month;
        //    int drpyear = 0;
        //    int drprangeyear = 0;
        //    int? getuserid = getidentity.ID(User.Identity.Name);
        //    int? selectionrpt = 0;
        //    int? wilayahselection = 0;
        //    int? ladangselection = 0;
        //    int incldg = 0;
        //    int getflag = 0;


        //    ViewBag.Report = "class = active";

        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

        //    List<SelectListItem> WilayahIDList2 = new List<SelectListItem>();
        //    List<SelectListItem> LadangIDList2 = new List<SelectListItem>();

        //    if (WilayahID == 0 && LadangID == 0)
        //    {
        //        wlyhid = getwilyah.GetWilayahID(SyarikatID);
        //        // mywlyid = String.Join("", wlyhid); ;
        //        WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)).OrderBy(o => o.fld_ID), "fld_ID", "fld_WlyhName").ToList();
        //        WilayahIDList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => x.fld_WlyhID == WilayahIDList && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
        //        LadangIDList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        selectionrpt = 1;
        //        wilayahselection = WilayahIDList;
        //        ladangselection = LadangIDList;
        //        incldg = 1;
        //        getflag = 2;
        //    }
        //    else if (WilayahID != 0 && LadangID == 0)
        //    {
        //        //mywlyid = String.Join("", WilayahID); ;
        //        wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
        //        WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
        //        LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => x.fld_WlyhID == WilayahIDList && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
        //        LadangIDList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        selectionrpt = 2;
        //        wilayahselection = WilayahIDList;
        //        ladangselection = LadangIDList;
        //        incldg = 2;
        //        getflag = 2;

        //    }
        //    else if (WilayahID != 0 && LadangID != 0)
        //    {
        //        //mywlyid = String.Join("", WilayahID); ;
        //        wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
        //        WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
        //        LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => x.fld_WlyhID == WilayahIDList && x.fld_ID == LadangIDList && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
        //        selectionrpt = 3;
        //        wilayahselection = WilayahIDList;
        //        ladangselection = LadangIDList;
        //        incldg = 3;
        //        getflag = 2;
        //    }
        //    drpyear = timezone.gettimezone().Year - int.Parse(GetConfig.GetData("yeardisplay")) + 1;
        //    drprangeyear = timezone.gettimezone().Year;

        //    var yearlist2 = new List<SelectListItem>();
        //    for (var i = drpyear; i <= drprangeyear; i++)
        //    {
        //        if (i == timezone.gettimezone().Year)
        //        {
        //            yearlist2.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString(), Selected = true });
        //        }
        //        else
        //        {
        //            yearlist2.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
        //        }
        //    }

        //    ViewBag.MonthList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "monthlist" && x.fldDeleted == false), "fldOptConfValue", "fldOptConfDesc", month);
        //    ViewBag.NamaSyarikat = db2.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
        //    ViewBag.NoSyarikat = db2.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();
        //    ViewBag.YearList = yearlist2;
        //    ViewBag.WilayahIDList = WilayahIDList2;
        //    ViewBag.LadangIDList = LadangIDList2;
        //    ViewBag.NeragaID = NegaraID;
        //    ViewBag.SyarikatID = SyarikatID;
        //    ViewBag.WilayahSelection = wilayahselection;
        //    ViewBag.LadangSelection = ladangselection;
        //    ViewBag.Year = year;
        //    ViewBag.Month = MonthList;
        //    ViewBag.Selectionrpt = selectionrpt;
        //    ViewBag.IncLdg = incldg;
        //    ViewBag.UserID = getuserid;
        //    ViewBag.getflag = getflag;
        //    return View();
        //}

        //[AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3,Viewer")]
        //public ActionResult PaysheetByLadangDetail(int negaraid, int syarikatid, int wilayahid, int ladangid, int month, int year, int incldg, int bill)
        //{
        //    string monthstring = month.ToString();
        //    if (monthstring.Length == 1)
        //    {
        //        monthstring = "0" + monthstring;
        //    }

        //    string namaldg = db.tbl_Ladang.Where(x => x.fld_ID == ladangid).Select(s => s.fld_LdgName).FirstOrDefault();
        //    var result = db.tbl_Sctran.Where(x => x.fld_Bulan == monthstring && x.fld_Tahun == year && x.fld_WilayahID == wilayahid && x.fld_LadangID == ladangid);
        //    decimal? GajiKasar = result.Where(x => x.fld_KdCaj == "D").Select(s => s.fld_Amt).Sum();
        //    decimal? kwsp = result.Where(x => x.fld_Lejar == "401").Select(s => s.fld_Amt).Sum();
        //    decimal? kwspP = result.Where(x => x.fld_Akt == "2101").Select(s => s.fld_Amt).Sum();
        //    decimal? socso = result.Where(x => x.fld_Lejar == "402").Select(s => s.fld_Amt).Sum();
        //    decimal? socsoP = result.Where(x => x.fld_Akt == "2102").Select(s => s.fld_Amt).Sum();

        //    decimal? pendahuluanTunai = result.Where(x => x.fld_Lejar == "426").Select(s => s.fld_Amt).Sum();
        //    decimal? lain2 = result.Where(x => x.fld_Lejar == "440").Select(s => s.fld_Amt).Sum();
        //    decimal? Levi = result.Where(x => x.fld_Lejar == "051").Select(s => s.fld_Amt).Sum();
        //    decimal? elektrik = result.Where(x => x.fld_Lejar == "601" && x.fld_Akt == "2601").Select(s => s.fld_Amt).Sum();
        //    decimal? air = result.Where(x => x.fld_Lejar == "601" && x.fld_Akt == "2602").Select(s => s.fld_Amt).Sum();
        //    decimal? insurans = result.Where(x => x.fld_Lejar == "405").Select(s => s.fld_Amt).Sum();
        //    decimal? lain = GetTriager.GetTotalForMoney2(elektrik) + GetTriager.GetTotalForMoney2(air) + GetTriager.GetTotalForMoney2(pendahuluanTunai) + GetTriager.GetTotalForMoney2(lain2) + GetTriager.GetTotalForMoney2(Levi) + GetTriager.GetTotalForMoney2(insurans);

        //    decimal? GajiBersih = result.Where(x => x.fld_Lejar == "452").Select(s => s.fld_Amt).Sum();
        //    decimal? kwspsocsoP = GetTriager.GetTotalForMoney2(kwspP) + GetTriager.GetTotalForMoney2(socsoP);
        //    decimal? kwspsocsoM = GetTriager.GetTotalForMoney2(kwsp) + GetTriager.GetTotalForMoney2(socso) - GetTriager.GetTotalForMoney2(kwspsocsoP);

        //    DateTime? tarikh = result.Select(s => s.fld_Tarikh).Distinct().FirstOrDefault();
        //    string skb = result.Where(x => x.fld_Lejar == "452").Select(s => s.fld_NoSkb).Distinct().FirstOrDefault();

        //    ViewBag.NamaLadang = namaldg;
        //    ViewBag.GajiKasar = GajiKasar;
        //    ViewBag.KWSP = kwspP;
        //    ViewBag.SOCSO = socsoP;
        //    ViewBag.Lain = lain;
        //    ViewBag.GajiBersih = GajiBersih;
        //    ViewBag.Bill = bill;
        //    ViewBag.Date = tarikh;
        //    ViewBag.Skb = skb;
        //    ViewBag.KwspSocsoP = kwspsocsoP;
        //    ViewBag.KwspSocsoM = kwspsocsoM;


        //    var resultreport = db.tbl_SokPermhnWang.Where(x => x.fld_Month == month && x.fld_Year == year && x.fld_WilayahID == wilayahid && x.fld_LadangID == ladangid && x.fld_TerimaHQ_Status == 1);
        //    decimal? manual = resultreport.Select(x => x.fld_JumlahManual).FirstOrDefault();
        //    decimal? CheckrolPekerja = GetTriager.GetTotalForMoney2(kwspP) + GetTriager.GetTotalForMoney2(socsoP) + GetTriager.GetTotalForMoney2(lain) + GetTriager.GetTotalForMoney2(GajiBersih);

        //    ViewBag.CheckrolPekerja = CheckrolPekerja;
        //    ViewBag.DataCount = resultreport.Count();
        //    return View("PaysheetByLadangDetail", resultreport);
        //}

        //[AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3,Viewer")]
        //public ActionResult PaysheetByLadangSummary(int ldgid, int negaraid, int syarikatid, int wilayahid, int month, int year, int incldg)
        //{
        //    string monthstring = month.ToString();
        //    if (monthstring.Length == 1)
        //    {
        //        monthstring = "0" + monthstring;
        //    }
        //    var result = db.tbl_Sctran.Where(x => x.fld_Bulan == monthstring && x.fld_Tahun == year);
        //    var resultreport = db.tbl_SokPermhnWang.Where(x => x.fld_Month == month && x.fld_Year == year && x.fld_TerimaHQ_Status == 1);
        //    if (ldgid == 0)
        //    {
        //        result = db.tbl_Sctran.Where(x => x.fld_Bulan == monthstring && x.fld_Tahun == year && x.fld_WilayahID == wilayahid);
        //        resultreport = db.tbl_SokPermhnWang.Where(x => x.fld_Month == month && x.fld_Year == year && x.fld_WilayahID == wilayahid && x.fld_TerimaHQ_Status == 1);
        //    }
        //    else
        //    {
        //        result = db.tbl_Sctran.Where(x => x.fld_Bulan == monthstring && x.fld_Tahun == year && x.fld_WilayahID == wilayahid && x.fld_LadangID == ldgid);
        //        resultreport = db.tbl_SokPermhnWang.Where(x => x.fld_Month == month && x.fld_Year == year && x.fld_WilayahID == wilayahid && x.fld_LadangID == ldgid && x.fld_TerimaHQ_Status == 1);
        //    }
        //    //var result = db.tbl_Sctran.Where(x => x.fld_Bulan == monthstring && x.fld_Tahun == year && x.fld_WilayahID == wilayahid);
        //    decimal? GajiKasar = result.Where(x => x.fld_KdCaj == "D").Select(s => s.fld_Amt).Sum();
        //    decimal? kwsp = result.Where(x => x.fld_Lejar == "401").Select(s => s.fld_Amt).Sum();
        //    decimal? kwspP = result.Where(x => x.fld_Akt == "2101").Select(s => s.fld_Amt).Sum();
        //    decimal? socso = result.Where(x => x.fld_Lejar == "402").Select(s => s.fld_Amt).Sum();
        //    decimal? socsoP = result.Where(x => x.fld_Akt == "2102").Select(s => s.fld_Amt).Sum();

        //    decimal? pendahuluanTunai = result.Where(x => x.fld_Lejar == "426").Select(s => s.fld_Amt).Sum();
        //    decimal? lain2 = result.Where(x => x.fld_Lejar == "440").Select(s => s.fld_Amt).Sum();
        //    decimal? Levi = result.Where(x => x.fld_Lejar == "051").Select(s => s.fld_Amt).Sum();
        //    decimal? elektrik = result.Where(x => x.fld_Lejar == "601" && x.fld_Akt == "2601").Select(s => s.fld_Amt).Sum();
        //    decimal? air = result.Where(x => x.fld_Lejar == "601" && x.fld_Akt == "2602").Select(s => s.fld_Amt).Sum();
        //    decimal? Insurans = result.Where(x => x.fld_Lejar == "405").Select(s => s.fld_Amt).Sum();
        //    //decimal? lain = elektrik + air;
        //    decimal? lain = GetTriager.GetTotalForMoney2(elektrik) + GetTriager.GetTotalForMoney2(air) + GetTriager.GetTotalForMoney2(pendahuluanTunai) + GetTriager.GetTotalForMoney2(lain2) + GetTriager.GetTotalForMoney2(Levi) + GetTriager.GetTotalForMoney2(Insurans);
        //    decimal? GajiBersih = result.Where(x => x.fld_Lejar == "452").Select(s => s.fld_Amt).Sum();
        //    decimal? kwspsocsoP = GetTriager.GetTotalForMoney2(kwspP) + GetTriager.GetTotalForMoney2(socsoP);
        //    decimal? kwspsocsoM = GetTriager.GetTotalForMoney2(kwsp) + GetTriager.GetTotalForMoney2(socso) - GetTriager.GetTotalForMoney2(kwspsocsoP);

        //    ViewBag.GajiKasar = GajiKasar;
        //    ViewBag.KWSP = kwspP;
        //    ViewBag.SOCSO = socsoP;
        //    ViewBag.Lain = lain;
        //    ViewBag.GajiBersih = GajiBersih;
        //    ViewBag.KwspSocsoP = kwspsocsoP;
        //    ViewBag.KwspSocsoM = kwspsocsoM;

        //    //var resultreport = db.tbl_SokPermhnWang.Where(x => x.fld_Month == month && x.fld_Year == year && x.fld_WilayahID == wilayahid && x.fld_TerimaHQ_Status == 1);
        //    decimal? manual = resultreport.Select(x => x.fld_JumlahManual).Sum();
        //    decimal? CheckrolPekerja = GetTriager.GetTotalForMoney2(kwspP) + GetTriager.GetTotalForMoney2(socsoP) + GetTriager.GetTotalForMoney2(lain) + GetTriager.GetTotalForMoney2(GajiBersih);
        //    ViewBag.CheckrolPekerja = CheckrolPekerja;
        //    ViewBag.PDP = resultreport.Select(s => s.fld_JumlahPDP).Sum();
        //    ViewBag.TT = resultreport.Select(s => s.fld_JumlahTT).Sum();
        //    ViewBag.CIT = resultreport.Select(s => s.fld_JumlahCIT).Sum();
        //    ViewBag.DataCount = resultreport.Count();
        //    ViewBag.Manual = manual;
        //    return View("PaysheetByLadangSummary", resultreport.Take(1));
        //}

        //[AccessDeniedAuthorizeAttribute(Roles = "Resource")]
        //public ActionResult ListResumeWorker()
        //{
        //    ViewBag.Report = "class = active";
        //    ViewBag.KumpulanSyarikatList = new SelectList(db.vw_NeragaSumberDetail.Where(x => x.fldUserName == User.Identity.Name).OrderBy(o => o.fld_NamaKmplnSyrkt), "fldKmplnSyrktID", "fld_NamaKmplnSyrkt");
        //    var getSatuSyarikat = db.vw_NeragaSumberDetail.Where(x => x.fldUserName == User.Identity.Name).OrderBy(o => o.fld_NamaKmplnSyrkt).Take(1).Select(s => s.fldKmplnSyrktID).FirstOrDefault();
        //    ViewBag.SyarikatList = new SelectList(db.vw_NSWL.Where(x => x.fld_KmplnSyrktID == getSatuSyarikat && x.fld_Deleted_S == false).Select(s => new { s.fld_SyarikatID, s.fld_NamaSyarikat }).Distinct(), "fld_SyarikatID", "fld_NamaSyarikat");
        //    var getSatuSyarikat2 = db.vw_NSWL.Where(x => x.fld_KmplnSyrktID == getSatuSyarikat && x.fld_Deleted_S == false).Select(s => s.fld_SyarikatID).Take(1).Distinct().FirstOrDefault();
        //    ViewBag.BatchList = new SelectList(db.tblTKABatches.Where(x => x.fldKmplnSyrktID == getSatuSyarikat && x.fldSyrktID == getSatuSyarikat2), "fldID", "fldNoBatch").ToList();
        //    List<vw_TKADetail> TKADetail = new List<vw_TKADetail>();

        //    return View("ListResumeWorker", TKADetail);
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //[AccessDeniedAuthorizeAttribute(Roles = "Resource")]
        //public ActionResult ListResumeWorker(int BatchList, int KumpulanSyarikatList, int SyarikatList)
        //{
        //    ViewBag.Report = "class = active";
        //    int getuserid = getidentity.ID(User.Identity.Name);

        //    ViewBag.KumpulanSyarikatList = new SelectList(db.vw_NeragaSumberDetail.Where(x => x.fldUserName == User.Identity.Name).OrderBy(o => o.fld_NamaKmplnSyrkt), "fldKmplnSyrktID", "fld_NamaKmplnSyrkt", KumpulanSyarikatList);
        //    var getSatuSyarikat = db.vw_NeragaSumberDetail.Where(x => x.fldUserName == User.Identity.Name).OrderBy(o => o.fld_NamaKmplnSyrkt).Take(1).Select(s => s.fldKmplnSyrktID).FirstOrDefault();
        //    ViewBag.SyarikatList = new SelectList(db.vw_NSWL.Where(x => x.fld_KmplnSyrktID == getSatuSyarikat && x.fld_Deleted_S == false).Select(s => new { s.fld_SyarikatID, s.fld_NamaSyarikat }).Distinct(), "fld_SyarikatID", "fld_NamaSyarikat", SyarikatList);
        //    var getSatuSyarikat2 = db.vw_NSWL.Where(x => x.fld_KmplnSyrktID == getSatuSyarikat && x.fld_Deleted_S == false).Select(s => s.fld_SyarikatID).Take(1).Distinct().FirstOrDefault();
        //    ViewBag.BatchList = new SelectList(db.tblTKABatches.Where(x => x.fldKmplnSyrktID == getSatuSyarikat && x.fldSyrktID == getSatuSyarikat2), "fldID", "fldNoBatch", BatchList).ToList();
        //    ViewBag.NamaSyarikatTo = db.vw_NSWL.Where(x => x.fld_SyarikatID == SyarikatList).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
        //    ViewBag.NamaSyarikatFrom = db2.tblUsers.Where(x => x.fldUserID == getuserid).Select(s => s.fldUserFullName).FirstOrDefault();
        //    List<vw_TKADetail> TKADetail = new List<vw_TKADetail>();
        //    TKADetail = db.vw_TKADetail.Where(x => x.fldTKABatchID == BatchList).ToList();
        //    ViewBag.BatchName = TKADetail.Select(s => s.fldNoBatch).Distinct().FirstOrDefault();
        //    ViewBag.BatchDate = TKADetail.Select(s => s.fldDTCreated).Take(1).FirstOrDefault();

        //    return View("ListResumeWorker", TKADetail);
        //}

        //public ActionResult SysVersion()
        //{
        //    int? getuserid = getidentity.ID(User.Identity.Name);
        //    int month = timezone.gettimezone().AddMonths(-1).Month;
        //    int year = timezone.gettimezone().Year;
        //    int[] wlyhid = new int[] { };
        //    int? NegaraID = 0;
        //    int? SyarikatID = 0;
        //    int? WilayahID = 0;
        //    int? LadangID = 0;
        //    //string mywlyid = "";
        //    int? selectionrpt = 0;
        //    int? wilayahselection = 0;
        //    int? ladangselection = 0;
        //    int incldg = 0;
        //    int getflag = 0;
        //    int drpyear = 0;
        //    int drprangeyear = 0;

        //    ViewBag.Report = "class = active";

        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
        //    List<SelectListItem> WilayahIDList = new List<SelectListItem>();
        //    List<SelectListItem> LadangIDList = new List<SelectListItem>();

        //    if (WilayahID == 0 && LadangID == 0)
        //    {
        //        wlyhid = getwilyah.GetWilayahID(SyarikatID);
        //        // mywlyid = String.Join("", wlyhid); ;
        //        WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)).OrderBy(o => o.fld_ID), "fld_ID", "fld_WlyhName").ToList();
        //        WilayahIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        var wlyhID = db2.tbl_Wilayah.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).Select(s => s.fld_ID).Take(1).FirstOrDefault();
        //        LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => x.fld_WlyhID == wlyhID && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
        //        LadangIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        selectionrpt = 1;
        //        wilayahselection = WilayahID;
        //        ladangselection = LadangID;
        //        incldg = 1;
        //        getflag = 1;
        //    }
        //    else if (WilayahID != 0 && LadangID == 0)
        //    {
        //        //mywlyid = String.Join("", WilayahID); ;
        //        wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
        //        WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
        //        LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
        //        LadangIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        selectionrpt = 2;
        //        wilayahselection = WilayahID;
        //        ladangselection = LadangID;
        //        incldg = 2;
        //        getflag = 1;

        //    }
        //    else if (WilayahID != 0 && LadangID != 0)
        //    {
        //        //mywlyid = String.Join("", WilayahID); ;
        //        wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
        //        WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
        //        LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_ID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
        //        selectionrpt = 3;
        //        wilayahselection = WilayahID;
        //        ladangselection = LadangID;
        //        incldg = 3;
        //        getflag = 1;
        //    }

        //    drpyear = timezone.gettimezone().Year - int.Parse(GetConfig.GetData("yeardisplay")) + 1;
        //    drprangeyear = timezone.gettimezone().Year;

        //    var yearlist = new List<SelectListItem>();
        //    for (var i = drpyear; i <= drprangeyear; i++)
        //    {
        //        if (i == timezone.gettimezone().Year)
        //        {
        //            yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString(), Selected = true });
        //        }
        //        else
        //        {
        //            yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
        //        }
        //    }
        //    ViewBag.MonthList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "monthlist" && x.fldDeleted == false), "fldOptConfValue", "fldOptConfDesc", month);

        //    //ViewBag.GetAllLdg = LadangIDList;
        //    ViewBag.YearList = yearlist;
        //    ViewBag.WilayahIDList = WilayahIDList;
        //    ViewBag.LadangIDList = LadangIDList;
        //    ViewBag.NeragaID = NegaraID;
        //    ViewBag.SyarikatID = SyarikatID;
        //    ViewBag.WilayahSelection = wilayahselection;
        //    ViewBag.LadangSelection = ladangselection;
        //    ViewBag.Year = year;
        //    ViewBag.Month = month;
        //    //ViewBag.Selectionrpt = selectionrpt;
        //    //ViewBag.IncLdg = incldg;
        //    //ViewBag.UserID = getuserid;
        //    //ViewBag.getflag = getflag;

        //    return View();
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult SysVersion(int yearlist, int MonthList, int WilayahIDList, int LadangIDList)
        //{
        //    int? getuserid = getidentity.ID(User.Identity.Name);
        //    int month = timezone.gettimezone().AddMonths(-1).Month;
        //    int year = timezone.gettimezone().Year;
        //    int[] wlyhid = new int[] { };
        //    int? NegaraID = 0;
        //    int? SyarikatID = 0;
        //    int? WilayahID = 0;
        //    int? LadangID = 0;
        //    //string mywlyid = "";
        //    int? selectionrpt = 0;
        //    int? wilayahselection = 0;
        //    int? ladangselection = 0;
        //    int incldg = 0;
        //    int getflag = 0;
        //    int drpyear = 0;
        //    int drprangeyear = 0;

        //    ViewBag.Report = "class = active";

        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
        //    List<SelectListItem> WilayahIDList2 = new List<SelectListItem>();
        //    List<SelectListItem> LadangIDList2 = new List<SelectListItem>();

        //    if (WilayahID == 0 && LadangID == 0)
        //    {
        //        wlyhid = getwilyah.GetWilayahID(SyarikatID);
        //        // mywlyid = String.Join("", wlyhid); ;
        //        WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)).OrderBy(o => o.fld_ID), "fld_ID", "fld_WlyhName").ToList();
        //        WilayahIDList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        //var wlyhID = db2.tbl_Wilayah.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).Select(s => s.fld_ID).Take(1).FirstOrDefault();
        //        LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => x.fld_WlyhID == WilayahIDList && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
        //        LadangIDList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        selectionrpt = 1;
        //        wilayahselection = WilayahIDList;
        //        ladangselection = LadangIDList;
        //        incldg = 1;
        //        getflag = 2;
        //    }
        //    else if (WilayahID != 0 && LadangID == 0)
        //    {
        //        //mywlyid = String.Join("", WilayahID); ;
        //        wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
        //        WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
        //        LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
        //        LadangIDList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        selectionrpt = 2;
        //        wilayahselection = WilayahIDList;
        //        ladangselection = LadangIDList;
        //        incldg = 2;
        //        getflag = 2;

        //    }
        //    else if (WilayahID != 0 && LadangID != 0)
        //    {
        //        //mywlyid = String.Join("", WilayahID); ;
        //        wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
        //        WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
        //        LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_ID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
        //        selectionrpt = 3;
        //        wilayahselection = WilayahIDList;
        //        ladangselection = LadangIDList;
        //        incldg = 3;
        //        getflag = 2;
        //    }

        //    drpyear = timezone.gettimezone().Year - int.Parse(GetConfig.GetData("yeardisplay")) + 1;
        //    drprangeyear = timezone.gettimezone().Year;

        //    var yearlist2 = new List<SelectListItem>();
        //    for (var i = drpyear; i <= drprangeyear; i++)
        //    {
        //        if (i == timezone.gettimezone().Year)
        //        {
        //            yearlist2.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString(), Selected = true });
        //        }
        //        else
        //        {
        //            yearlist2.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
        //        }
        //    }
        //    ViewBag.MonthList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "monthlist" && x.fldDeleted == false), "fldOptConfValue", "fldOptConfDesc", month);

        //    //ViewBag.GetAllLdg = LadangIDList2;
        //    ViewBag.YearList = yearlist2;
        //    ViewBag.WilayahIDList = WilayahIDList2;
        //    ViewBag.LadangIDList = LadangIDList2;
        //    ViewBag.NeragaID = NegaraID;
        //    ViewBag.SyarikatID = SyarikatID;
        //    ViewBag.WilayahSelection = wilayahselection;
        //    ViewBag.LadangSelection = ladangselection;
        //    ViewBag.Year = yearlist;
        //    ViewBag.Month = MonthList;
        //    //ViewBag.Selectionrpt = selectionrpt;
        //    //ViewBag.IncLdg = incldg;
        //    //ViewBag.UserID = getuserid;
        //    //ViewBag.getflag = getflag;
        //    //ViewBag.NamaSyarikat = db2.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
        //    //ViewBag.NoSyarikat = db2.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();

        //    return View();
        //}

        //public ActionResult SysVersionDetail(int negaraid, int syarikatid, int wilayahid, int ladangid, int month, int year)
        //{
        //    var resultreport = db.tbl_Version.Where(x => x.fld_Month == month && x.fld_Year == year && x.fld_WilayahID == wilayahid && x.fld_SyarikatID == syarikatid && x.fld_NegaraID == negaraid);

        //    if (ladangid != 0)
        //    {
        //        resultreport = db.tbl_Version.Where(x => x.fld_Month == month && x.fld_Year == year && x.fld_WilayahID == wilayahid && x.fld_LadangID == ladangid && x.fld_SyarikatID == syarikatid && x.fld_NegaraID == negaraid);

        //    }

        //    ViewBag.DataCount = resultreport.Count();
        //    ViewBag.Month = month;
        //    ViewBag.Year = year;
        //    ViewBag.NamaSyarikat = db2.tbl_Syarikat.Where(x => x.fld_SyarikatID == syarikatid && x.fld_NegaraID == negaraid).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
        //    ViewBag.NoSyarikat = db2.tbl_Syarikat.Where(x => x.fld_SyarikatID == syarikatid && x.fld_NegaraID == negaraid).Select(s => s.fld_NoSyarikat).FirstOrDefault();
        //    ViewBag.WilayahName = db2.tbl_Wilayah.Where(x => x.fld_ID == wilayahid && x.fld_SyarikatID == syarikatid).Select(s => s.fld_WlyhName).FirstOrDefault();
        //    ViewBag.LadangCode = db2.tbl_Ladang.Where(x => x.fld_ID == ladangid && x.fld_WlyhID == wilayahid).Select(s => s.fld_LdgCode).FirstOrDefault();
        //    ViewBag.LadangName = db2.tbl_Ladang.Where(x => x.fld_ID == ladangid && x.fld_WlyhID == wilayahid).Select(s => s.fld_LdgName).FirstOrDefault();
        //    //ViewBag.Bill = bill;
        //    //ViewBag.IncLdg = incldg;

        //    return View("SysVersionDetail", resultreport);
        //}

        //[AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3,Resource,Viewer")]
        //public ActionResult DataFileUploaded()
        //{
        //    int? getuserid = getidentity.ID(User.Identity.Name);
        //    int month = timezone.gettimezone().AddMonths(-1).Month;
        //    int year = timezone.gettimezone().Year;
        //    int[] wlyhid = new int[] { };
        //    int? NegaraID = 0;
        //    int? SyarikatID = 0;
        //    int? WilayahID = 0;
        //    int? LadangID = 0;
        //    int? wilayahselection = 0;
        //    int? ladangselection = 0;
        //    int drpyear = 0;
        //    int drprangeyear = 0;
        //    int WilayahIDSelect = 0;

        //    CheckSharedFolder CheckSharedFolder = new CheckSharedFolder();
        //    IOrderedEnumerable<FileInfo> getFiles;
        //    FileInfo[] files = null;
        //    DirectoryInfo salesFTPDirectory = null;
        //    List<FileUploadedData> FileUploadedDatas = new List<FileUploadedData>();
        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
        //    string getpathDB = CheckSharedFolder.GetSourceTargetPath("filesourcepath", NegaraID, SyarikatID);
        //    string getpath = getpathDB;

        //    ViewBag.Report = "class = active";

        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
        //    List<SelectListItem> WilayahIDList = new List<SelectListItem>();
        //    List<SelectListItem> LadangIDList = new List<SelectListItem>();

        //    if (WilayahID == 0 && LadangID == 0)
        //    {
        //        wlyhid = getwilyah.GetWilayahID(SyarikatID);
        //        WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)).OrderBy(o => o.fld_ID), "fld_ID", "fld_WlyhName").ToList();
        //        WilayahIDSelect = int.Parse(WilayahIDList.Take(1).Select(s => s.Value).FirstOrDefault());
        //        LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => x.fld_WlyhID == WilayahIDSelect && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
        //        LadangIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        wilayahselection = WilayahID;
        //        ladangselection = LadangID;
        //    }
        //    else if (WilayahID != 0 && LadangID == 0)
        //    {
        //        wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
        //        WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
        //        LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
        //        wilayahselection = WilayahID;
        //        ladangselection = LadangID;

        //    }
        //    else if (WilayahID != 0 && LadangID != 0)
        //    {
        //        wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
        //        WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
        //        LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_ID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
        //        wilayahselection = WilayahID;
        //        ladangselection = LadangID;
        //    }

        //    drpyear = timezone.gettimezone().Year - int.Parse(GetConfig.GetData("yeardisplay")) + 1;
        //    drprangeyear = timezone.gettimezone().Year;

        //    var yearlist = new List<SelectListItem>();
        //    for (var i = drpyear; i <= drprangeyear; i++)
        //    {
        //        if (i == timezone.gettimezone().Year)
        //        {
        //            yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString(), Selected = true });
        //        }
        //        else
        //        {
        //            yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
        //        }
        //    }
        //    ViewBag.MonthList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "monthlist" && x.fldDeleted == false), "fldOptConfValue", "fldOptConfDesc", month);

        //    ViewBag.YearList = yearlist;
        //    ViewBag.WilayahIDList = WilayahIDList;
        //    ViewBag.LadangIDList = LadangIDList;
        //    ViewBag.NeragaID = NegaraID;
        //    ViewBag.SyarikatID = SyarikatID;
        //    ViewBag.WilayahSelection = wilayahselection;
        //    ViewBag.LadangSelection = ladangselection;
        //    ViewBag.Year = year;
        //    ViewBag.Month = month;

        //    salesFTPDirectory = new DirectoryInfo(getpath);
        //    files = salesFTPDirectory.GetFiles();
        //    getFiles = files.Where(f => f.Extension == ".zip").OrderBy(o => o.CreationTime);
        //    foreach (var getFile in getFiles)
        //    {
        //        FileUploadedDatas.Add(new FileUploadedData() { FileName = getFile.Name, DateTimeCreated = getFile.CreationTime, SizeFile = getFile.Length });
        //    }
        //    return View(FileUploadedDatas);
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //[AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3,Resource,Viewer")]
        //public ActionResult DataFileUploaded(int yearlist, int MonthList, int WilayahIDList, int LadangIDList)
        //{
        //    int? getuserid = getidentity.ID(User.Identity.Name);
        //    int month = timezone.gettimezone().AddMonths(-1).Month;
        //    int year = timezone.gettimezone().Year;
        //    int[] wlyhid = new int[] { };
        //    int? NegaraID = 0;
        //    int? SyarikatID = 0;
        //    int? WilayahID = 0;
        //    int? LadangID = 0;
        //    int? wilayahselection = 0;
        //    int? ladangselection = 0;
        //    int drpyear = 0;
        //    int drprangeyear = 0;
        //    string stringyear = "";
        //    string stringmonth = "";
        //    IOrderedEnumerable<FileInfo> getFiles;
        //    FileInfo[] files = null;
        //    DirectoryInfo salesFTPDirectory = null;
        //    List<FileUploadedData> FileUploadedDatas = new List<FileUploadedData>();
        //    List<FileUploadedDataName> FileUploadedDataNames = new List<FileUploadedDataName>();
        //    FileUploadedDataName FileData = new FileUploadedDataName();
        //    CheckSharedFolder CheckSharedFolder = new CheckSharedFolder();
        //    List<AuthModels.tbl_Ladang> getladangdetails = new List<AuthModels.tbl_Ladang>();
        //    stringyear = yearlist.ToString();
        //    stringyear = stringyear.Substring(2, 2);
        //    stringmonth = MonthList.ToString();
        //    stringmonth = (stringmonth.Length == 1 ? "0" + stringmonth : stringmonth);
        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
        //    string getpathDB = CheckSharedFolder.GetSourceTargetPath("filesourcepath", NegaraID, SyarikatID);
        //    string getpath = getpathDB + stringmonth + stringyear + "\\";

        //    if (!Directory.Exists(getpath))
        //    {
        //        getpath = getpathDB;
        //    }

        //    ViewBag.Report = "class = active";

        //    List<SelectListItem> WilayahIDList2 = new List<SelectListItem>();
        //    List<SelectListItem> LadangIDList2 = new List<SelectListItem>();

        //    if (WilayahID == 0 && LadangID == 0)
        //    {
        //        wlyhid = getwilyah.GetWilayahID(SyarikatID);
        //        WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)).OrderBy(o => o.fld_ID), "fld_ID", "fld_WlyhName").ToList();
        //        LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => x.fld_WlyhID == WilayahIDList && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
        //        LadangIDList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        wilayahselection = WilayahIDList;
        //        ladangselection = LadangIDList;
        //    }
        //    else if (WilayahID != 0 && LadangID == 0)
        //    {
        //        wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
        //        WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
        //        LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
        //        LadangIDList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        wilayahselection = WilayahIDList;
        //        ladangselection = LadangIDList;

        //    }
        //    else if (WilayahID != 0 && LadangID != 0)
        //    {
        //        wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
        //        WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
        //        LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_ID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
        //        wilayahselection = WilayahIDList;
        //        ladangselection = LadangIDList;
        //    }

        //    drpyear = timezone.gettimezone().Year - int.Parse(GetConfig.GetData("yeardisplay")) + 1;
        //    drprangeyear = timezone.gettimezone().Year;

        //    var yearlist2 = new List<SelectListItem>();
        //    for (var i = drpyear; i <= drprangeyear; i++)
        //    {
        //        if (i == timezone.gettimezone().Year)
        //        {
        //            yearlist2.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString(), Selected = true });
        //        }
        //        else
        //        {
        //            yearlist2.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
        //        }
        //    }
        //    ViewBag.MonthList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "monthlist" && x.fldDeleted == false), "fldOptConfValue", "fldOptConfDesc", month);

        //    ViewBag.YearList = yearlist2;
        //    ViewBag.WilayahIDList = WilayahIDList2;
        //    ViewBag.LadangIDList = LadangIDList2;
        //    ViewBag.NeragaID = NegaraID;
        //    ViewBag.SyarikatID = SyarikatID;
        //    ViewBag.WilayahSelection = wilayahselection;
        //    ViewBag.LadangSelection = ladangselection;
        //    ViewBag.Year = yearlist;
        //    ViewBag.Month = MonthList;
        //    getladangdetails = LadangIDList == 0 ?
        //    db2.tbl_Ladang.Where(x => x.fld_WlyhID == WilayahIDList && x.fld_Deleted == false).ToList()
        //    :
        //    db2.tbl_Ladang.Where(x => x.fld_WlyhID == WilayahIDList && x.fld_ID == LadangIDList && x.fld_Deleted == false).ToList();
        //    foreach (var getladangdetail in getladangdetails)
        //    {
        //        FileUploadedDataNames.Add(new FileUploadedDataName() { FileName = getladangdetail.fld_LdgCode + stringmonth + stringyear + ".zip", KodLadang = getladangdetail.fld_LdgCode, NamaLadang = getladangdetail.fld_LdgName });
        //    }
        //    salesFTPDirectory = new DirectoryInfo(getpath);
        //    files = salesFTPDirectory.GetFiles();
        //    getFiles = files.Where(f => f.Extension == ".zip" && FileUploadedDataNames.Select(s => s.FileName).Contains(f.Name)).OrderBy(o => o.Name);
        //    foreach (var getFile in getFiles)
        //    {
        //        FileData = FileUploadedDataNames.Where(x => x.KodLadang == getFile.Name.Substring(0, 3)).FirstOrDefault();
        //        FileUploadedDatas.Add(new FileUploadedData() { FileName = getFile.Name, DateTimeCreated = getFile.CreationTime, SizeFile = getFile.Length, fld_LdgCode = FileData.KodLadang, fld_LdgName = FileData.NamaLadang });
        //    }
        //    ViewBag.FolderSelected = stringmonth + stringyear;
        //    return View(FileUploadedDatas);
        //}

        //public FileResult Download(string file, string from)
        //{
        //    int? NegaraID = 0;
        //    int? SyarikatID = 0;
        //    int? WilayahID = 0;
        //    int? LadangID = 0;
        //    int? getuserid = getidentity.ID(User.Identity.Name);
        //    DownloadFiles.FileDownloads objs = new DownloadFiles.FileDownloads();
        //    CheckSharedFolder CheckSharedFolder = new CheckSharedFolder();
        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
        //    string getpathDB = CheckSharedFolder.GetSourceTargetPath("filesourcepath", NegaraID, SyarikatID);
        //    var filesCol = objs.GetFiles(getpathDB + from + @"\");
        //    var CurrentFileName = filesCol.Where(x => x.FileName == file).FirstOrDefault();

        //    //using (var memoryStream = new MemoryStream())
        //    //{
        //    //    using (var ziparchive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
        //    //    {
        //    //        for (int i = 0; i < CurrentFileName.Count; i++)
        //    //        {
        //    //            ziparchive.CreateEntryFromFile(CurrentFileName[i].FilePath, CurrentFileName[i].FileName);
        //    //            //  ziparchive.CreateEntryFromFile(Server.MapPath("~/images/img_Download.PNG"), "img_Download.PNG");
        //    //        }
        //    //    }

        //    //    return File(memoryStream.ToArray(), "application/zip", file);
        //    //}
        //    string contentType = string.Empty;
        //    contentType = "application/pdf";
        //    return File(CurrentFileName.FilePath, contentType, CurrentFileName.FileName);

        //}

        public JsonResult GetPekerja(int WilayahID, int LadangID)
        {
            int? NegaraID = 0;
            int? SyarikatID = 0;
            int? WilayahID2 = 0;
            int? LadangID2 = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID2, out LadangID2, getuserid, User.Identity.Name);

            List<SelectListItem> pekerjalist = new List<SelectListItem>();
            var getpekerja = db.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Select(s => s.fld_Nopkj.Trim()).Distinct().ToArray();
            pekerjalist = new SelectList(db.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && getpekerja.Contains(x.fld_Nopkj.Trim())).Select(s => new SelectListItem { Value = s.fld_Nopkj.Trim(), Text = s.fld_Nopkj.Trim() + " - " + s.fld_Nama.Trim() }), "Value", "Text").Distinct().ToList();

            return Json(pekerjalist);
        }

        public JsonResult GetPekerja2(int WilayahID, int LadangID)
        {
            int? NegaraID = 0;
            int? SyarikatID = 0;
            int? WilayahID2 = 0;
            int? LadangID2 = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID2, out LadangID2, getuserid, User.Identity.Name);

            List<SelectListItem> pekerjalist = new List<SelectListItem>();

            //pekerjalist = new SelectList(db.vw_DetailPekerja.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Select(s => new SelectListItem { Value = s.fld_Nopkj.Trim(), Text = s.fld_Nopkj.Trim() + " - " + s.fld_Nama1.Trim() }), "Value", "Text").Distinct().ToList();
            //pekerjalist.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));

            if (WilayahID == 0)
            {
                pekerjalist = new SelectList(db.vw_DetailPekerja.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).Select(s => new SelectListItem { Value = s.fld_Nopkj.Trim(), Text = s.fld_Nopkj.Trim() + " - " + s.fld_Nama1.Trim() }), "Value", "Text").Distinct().Take(50).ToList();
            }
            else if (WilayahID != 0 && LadangID == 0)
            {
                pekerjalist = new SelectList(db.vw_DetailPekerja.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID).Select(s => new SelectListItem { Value = s.fld_Nopkj.Trim(), Text = s.fld_Nopkj.Trim() + " - " + s.fld_Nama1.Trim() }), "Value", "Text").Distinct().Take(50).ToList();
            }
            else
            {
                pekerjalist = new SelectList(db.vw_DetailPekerja.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Select(s => new SelectListItem { Value = s.fld_Nopkj.Trim(), Text = s.fld_Nopkj.Trim() + " - " + s.fld_Nama1.Trim() }), "Value", "Text").Distinct().Take(50).ToList();
            }

            return Json(pekerjalist);
        }

        //[HttpPost]
        //public ActionResult ConvertPDF(string myHtml, string filename, string reportname)
        //{
        //    string linkfile = "";
        //    bool success = false;
        //    string msg = "";
        //    string status = "";
        //    string appname = Request.ApplicationPath;
        //    string domain = Request.Url.GetLeftPart(UriPartial.Authority);

        //    if (appname != "/")
        //    {
        //        domain = domain + appname;
        //    }

        //    linkfile = ConvertToPdf.DownloadAsPDF(myHtml, filename, User.Identity.Name, reportname, domain);

        //    if (linkfile != "")
        //    {
        //        success = true;
        //        status = "success";
        //    }
        //    else
        //    {
        //        success = false;
        //        msg = "Something wrong.";
        //        status = "danger";
        //    }

        //    return Json(new { success = success, id = linkfile, msg = msg, status = status });
        //}

        //[HttpPost]
        //public ActionResult ConvertPDF2(string myHtml, string filename, string reportname)
        //{
        //    bool success = false;
        //    string msg = "";
        //    string status = "";
        //    tblHtmlReport tblHtmlReport = new tblHtmlReport();

        //    tblHtmlReport.fldHtlmCode = myHtml;
        //    tblHtmlReport.fldFileName = filename;
        //    tblHtmlReport.fldReportName = reportname;

        //    db.tblHtmlReports.Add(tblHtmlReport);
        //    db.SaveChanges();

        //    success = true;
        //    status = "success";

        //    return Json(new { success = success, id = tblHtmlReport.fldID, msg = msg, status = status, link = Url.Action("GetPDF", "Report", null, "http") + "/" + tblHtmlReport.fldID });
        //}

        //public ActionResult GetPDF(int id)
        //{
        //    int? NegaraID = 0;
        //    int? SyarikatID = 0;
        //    int? WilayahID = 0;
        //    int? LadangID = 0;
        //    int? getuserid = getidentity.ID(User.Identity.Name);
        //    string width = "", height = "";
        //    string imagepath = Server.MapPath("~/Asset/Images/");

        //    var gethtml = db.tblHtmlReports.Find(id);
        //    var getsize = db.tblReportLists.Where(x => x.fldReportListAction == gethtml.fldReportName.ToString()).FirstOrDefault();
        //    if (getsize != null)
        //    {
        //        width = getsize.fldWidthReport.ToString();
        //        height = getsize.fldHeightReport.ToString();
        //    }
        //    else
        //    {
        //        var getsizesubreport = db.tblSubReportLists.Where(x => x.fldSubReportListAction == gethtml.fldReportName.ToString()).FirstOrDefault();
        //        width = getsizesubreport.fldSubWidthReport.ToString();
        //        height = getsizesubreport.fldSubHeightReport.ToString();
        //    }
        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
        //    var logosyarikat = db2.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_LogoName).FirstOrDefault();

        //    //Export HTML String as PDF.
        //    //Image logo = Image.GetInstance(imagepath + logosyarikat);
        //    //Image alignment
        //    //logo.ScaleToFit(50f, 50f);
        //    //logo.Alignment = Image.TEXTWRAP | Image.ALIGN_CENTER;
        //    //StringReader sr = new StringReader(gethtml.fldHtlmCode);
        //    Document pdfDoc = new Document(new Rectangle(int.Parse(width), int.Parse(height)), 50f, 50f, 50f, 50f);
        //    //HTMLWorker htmlparser = new HTMLWorker(pdfDoc);
        //    PdfWriter writer = PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
        //    pdfDoc.Open();
        //    //pdfDoc.Add(logo);
        //    using (TextReader sr = new StringReader(gethtml.fldHtlmCode))
        //    {
        //        using (var htmlWorker = new HTMLWorkerExtended(pdfDoc, imagepath + logosyarikat))
        //        {
        //            htmlWorker.Open();
        //            htmlWorker.Parse(sr);
        //        }
        //    }
        //    pdfDoc.Close();
        //    Response.ContentType = "application/pdf";
        //    Response.AddHeader("content-disposition", "attachment;filename=" + gethtml.fldFileName + ".pdf");
        //    Response.Cache.SetCacheability(HttpCacheability.NoCache);
        //    Response.Write(pdfDoc);
        //    Response.End();

        //    db.Entry(gethtml).State = EntityState.Deleted;
        //    db.SaveChanges();
        //    return View();
        //}

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
                    ladanglist = new SelectList(db5.vw_NSWL.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_Deleted_L == false).OrderBy(o => o.fld_NamaLadang).Select(s => new SelectListItem { Value = s.fld_LadangID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_NamaLadang }), "Value", "Text").ToList();
                }
                else
                {
                    ladanglist = new SelectList(db5.vw_NSWL.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_Deleted_L == false).OrderBy(o => o.fld_NamaLadang).Select(s => new SelectListItem { Value = s.fld_LadangID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_NamaLadang }), "Value", "Text").ToList();
                }
            }

            return Json(ladanglist);
        }

        public JsonResult GetSubReportList(int ReportList)
        {
            List<SelectListItem> getsubreportlist = new List<SelectListItem>();

            getsubreportlist = new SelectList(dbC.tblSubReportLists.Where(x => x.fldMainReportID == ReportList && x.fldDeleted == false).OrderBy(o => o.fldSubReportListName).Select(s => new SelectListItem { Value = s.fldSubReportListID.ToString(), Text = s.fldSubReportListName }), "Value", "Text").ToList();

            return Json(getsubreportlist);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
                db2.Dispose();
                //db3.Dispose();
                //db4.Dispose();
            }
            base.Dispose(disposing);
        }


        [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3,Viewer")]
        public ActionResult PenggajianDataReport()
        {
            int[] wlyhid = new int[] { };
            int? NegaraID = 0;
            int? SyarikatID = 0;
            int? WilayahID = 0;
            int? LadangID = 0;
            int year = timezone.gettimezone().Year;
            int month = timezone.gettimezone().AddMonths(-1).Month;
            int drpyear = 0;
            int drprangeyear = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            int? wilayahselection = 0;
            int? ladangselection = 0;

            ViewBag.Report = "class = active";

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            drpyear = timezone.gettimezone().Year - int.Parse(GetConfig.GetData("yeardisplay")) + 1;
            drprangeyear = timezone.gettimezone().Year;

            var yearlist = new List<SelectListItem>();
            for (var i = drpyear; i <= drprangeyear; i++)
            {
                if (i == timezone.gettimezone().Year)
                {
                    yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString(), Selected = true });
                }
                else
                {
                    yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
                }
            }

            List<SelectListItem> companyList = new List<SelectListItem>();
            List<SelectListItem> wilayahList = new List<SelectListItem>();
            List<SelectListItem> ladangList = new List<SelectListItem>();
            companyList = new SelectList(
               db.tbl_Syarikat
                   .Where(x => x.fld_NegaraID == NegaraID && x.fld_Deleted == false).OrderBy(o => o.fld_NamaSyarikat)
                   .Select(
                       s => new SelectListItem { Value = s.fld_NamaPndkSyarikat.ToString(), Text = s.fld_NamaSyarikat }), "Value", "Text").ToList();
            companyList.Insert(0, new SelectListItem { Text = GlobalResCorp.lblChoose, Value = "0" });
            wilayahList.Insert(0, new SelectListItem { Text = GlobalResCorp.lblChoose, Value = "0" });
            ladangList.Insert(0, new SelectListItem { Text = GlobalResCorp.lblChoose, Value = "0" });

            ViewBag.MonthList = new SelectList(dbC.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "monthlist" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false), "fldOptConfValue", "fldOptConfDesc", month);
            ViewBag.YearList = yearlist;
            ViewBag.CompCodeList = companyList;
            ViewBag.UserID = getuserid;
            ViewBag.WilayahList = wilayahList;
            ViewBag.LadangList = ladangList;

            return View();
        }

        public ViewResult _PenggajianDataReport(string CompCodeList,int? MonthList, int? YearList, string CompanyListList, int? WilayahList, int? LadangList)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? DivisionID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            DateTime? CurrentDT = timezone.gettimezone();
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            ViewBag.NamaSyarikat = db.tbl_Syarikat.Where(x => x.fld_NegaraID == NegaraID && x.fld_NamaPndkSyarikat == CompCodeList).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
            ViewBag.NomborSyarikat = db.tbl_Syarikat.Where(x => x.fld_NegaraID == NegaraID && x.fld_NamaPndkSyarikat == CompCodeList).Select(s => s.fld_NoSyarikat).FirstOrDefault(); 
            ViewBag.Tahun = YearList;
            var namapenuhsyarikat = GetConfig.GetSyarikatFullName(CompCodeList);
            if (namapenuhsyarikat == "") { namapenuhsyarikat = "-"; }
            var namasyarikat = GetConfig.GetSyarikatName(CompCodeList);
            var nosyarikat = GetConfig.GetSyarikatName(CompCodeList);
            ViewBag.costcentre = CompCodeList;
            ViewBag.namapenuhsyarikat = namapenuhsyarikat.ToUpper();
            List<ModelsDapper.sp_PenggajianData_Result> PenggajianDataResult = new List<ModelsDapper.sp_PenggajianData_Result>();

                string constr = ConfigurationManager.ConnectionStrings["MVC_SYSTEM_HQ_CONN"].ConnectionString;
                var con = new SqlConnection(constr);

                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("NegaraID", NegaraID);
                parameters.Add("SyarikatID", 1);
                parameters.Add("WilayahID", WilayahID);
                parameters.Add("LadangID", LadangID);
                parameters.Add("Company", 1);
                parameters.Add("Year", YearList);
                parameters.Add("UserID", getuserid);
                parameters.Add("CostCentre", CompCodeList);

                con.Open();
            Dapper.SqlMapper.Settings.CommandTimeout = 3600;
            PenggajianDataResult = SqlMapper.Query<ModelsDapper.sp_PenggajianData_Result>(con, "sp_PenggajianData", parameters).ToList();
            //PenggajianDataResult = SqlMapper.Query<ModelsDapper.sp_PenggajianData_Result>(con, "sp_PenggajianData", parameters, commandType: CommandType.StoredProcedure).ToList();
            con.Close();

            if (WilayahList != 0 && LadangList != 0 )
            {
                PenggajianDataResult = PenggajianDataResult.Where(x => x.fld_WilayahID == WilayahList && x.fld_LadangID == LadangList && x.cost_centre == CompCodeList).ToList();
            }
            else if (WilayahList != 0 && LadangList == 0)
            {
                PenggajianDataResult = PenggajianDataResult.Where(x => x.fld_WilayahID == WilayahList && x.cost_centre == CompCodeList).ToList();
            }
            else if (WilayahList == 0 && LadangList == 0)
            {
                PenggajianDataResult = PenggajianDataResult.Where(x => x.cost_centre == CompCodeList).ToList();
            }
            else
            {
                PenggajianDataResult = PenggajianDataResult.Where(x => x.cost_centre == CompCodeList).ToList() ;
            }
            return View(PenggajianDataResult);
        }

        public JsonResult GetWilayah(string SyarikatID)
        {
            List<SelectListItem> ladanglist = new List<SelectListItem>();
            List<SelectListItem> wilayahlist = new List<SelectListItem>();

            int? NegaraID = 0;
            int? SyarikatID1 = 0;
            int? WilayahID2 = 0;
            int? LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            int[] wlyhid = new int[] { };

            SyarikatID1 = db5.tbl_Syarikat.Where(x => x.fld_NamaPndkSyarikat == SyarikatID && x.fld_Deleted == false).Select(x=>x.fld_SyarikatID).FirstOrDefault();
            GetNSWL.GetData(out NegaraID, out SyarikatID1, out WilayahID2, out LadangID, getuserid, User.Identity.Name);
            {
                if (SyarikatID == "0")
                {
                    //wilayahlist = new SelectList(db5.vw_NSWL.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_Deleted_L == false).Distinct().OrderBy(o => o.fld_NamaWilayah).Select(s => new SelectListItem { Value = s.fld_WilayahID.ToString(), Text = s.fld_NamaWilayah }), "Value", "Text").ToList();
                }
                else
                {
                    wlyhid = getwilyah.GetWilayahID(SyarikatID1);
                    wilayahlist = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
                }
            }

            return Json(wilayahlist);
        }
        public JsonResult GetLadangWil(string SyarikatID,int WilayahID)
        {
            List<SelectListItem> ladanglist = new List<SelectListItem>();
            List<SelectListItem> wilayahlist = new List<SelectListItem>();

            int? NegaraID = 0;
            int? SyarikatID1 = 0;
            int? WilayahID2 = 0;
            int? LadangID1 = 0;
            //int? getuserid = getidentity.ID(User.Identity.Name);

            //GetNSWL.GetData(out NegaraID, out SyarikatID1, out WilayahID2, out LadangID1, getuserid, User.Identity.Name);

                if (SyarikatID == "0" )
                {
                    //wilayahlist = new SelectList(db5.vw_NSWL.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_Deleted_L == false).Distinct().OrderBy(o => o.fld_NamaWilayah).Select(s => new SelectListItem { Value = s.fld_WilayahID.ToString(), Text = s.fld_NamaWilayah }), "Value", "Text").ToList();
                }
                else
                {
                    ladanglist = new SelectList(db5.vw_NSWL.Where(x => x.fld_CostCentre == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_Deleted_L == false).Select(s => new SelectListItem { Value = s.fld_LadangID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_NamaLadang }), "Value", "Text").ToList();
                }
            return Json(ladanglist);
        }


        //public ActionResult TransactionListingReport()
        //{
        //    Connection Connection = new Connection();
        //    int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
        //    int? getuserid = getidentity.ID(User.Identity.Name);
        //    ViewBag.Report = "class = active";
        //    //string host, catalog, user, pass = "";
        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
        //    //Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
        //    //MVC_SYSTEM_ModelsEstate dbr = MVC_SYSTEM_ModelsEstate.ConnectToSqlServer(host, catalog, user, pass);
        //    int[] wlyhid = new int[] { };
        //    int drpyear = 0;
        //    int drprangeyear = 0;
        //    int month = timezone.gettimezone().Month;

        //    //List<SelectListItem> SelectionList = new List<SelectListItem>();
        //    //SelectionList = new SelectList(
        //    //    dbr.tbl_Pkjmast
        //    //        .Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
        //    //                    x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
        //    //        .OrderBy(o => o.fld_Nopkj)
        //    //        .Select(s => new SelectListItem { Value = s.fld_Nopkj, Text = s.fld_Nopkj + "-" + s.fld_Nama }),
        //    //    "Value", "Text").ToList();
        //    //SelectionList.Insert(0, (new SelectListItem { Text = "Semua", Value = "0" }));

        //    //ViewBag.SelectionList = SelectionList;
        //    List<SelectListItem> WilayahIDList = new List<SelectListItem>();
        //    List<SelectListItem> LadangIDList = new List<SelectListItem>();

        //    if (WilayahID == 0 && LadangID == 0)
        //    {
        //        wlyhid = getwilyah.GetWilayahID(SyarikatID);
        //        // mywlyid = String.Join("", wlyhid); ;
        //        WilayahIDList = new SelectList(db.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
        //        WilayahIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        LadangIDList = new SelectList(db.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
        //        LadangIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        //selectionrpt = 1;
        //        //wilayahselection = WilayahID;
        //        //ladangselection = LadangID;
        //        //incldg = 1;
        //    }
        //    else if (WilayahID != 0 && LadangID == 0)
        //    {
        //        //mywlyid = String.Join("", WilayahID); ;
        //        wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
        //        WilayahIDList = new SelectList(db.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
        //        LadangIDList = new SelectList(db.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
        //        LadangIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        //selectionrpt = 2;
        //        //wilayahselection = WilayahID;
        //        //ladangselection = LadangID;
        //        //incldg = 2;

        //    }
        //    else if (WilayahID != 0 && LadangID != 0)
        //    {
        //        //mywlyid = String.Join("", WilayahID); ;
        //        wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
        //        WilayahIDList = new SelectList(db.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
        //        LadangIDList = new SelectList(db.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_ID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
        //        //selectionrpt = 3;
        //        //wilayahselection = WilayahID;
        //        //ladangselection = LadangID;
        //        //incldg = 3;
        //    }

        //    drpyear = timezone.gettimezone().Year - int.Parse(GetConfig.GetData("yeardisplay")) + 1;
        //    drprangeyear = timezone.gettimezone().Year;

        //    var yearlist = new List<SelectListItem>();
        //    for (var i = drpyear; i <= drprangeyear; i++)
        //    {
        //        if (i == timezone.gettimezone().Year)
        //        {
        //            yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString(), Selected = true });
        //        }
        //        else
        //        {
        //            yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
        //        }
        //    }

        //    ViewBag.MonthList = new SelectList(
        //        db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "monthlist" && x.fldDeleted == false),
        //        "fldOptConfValue", "fldOptConfDesc");

        //    ViewBag.YearList = yearlist;
        //    ViewBag.WilayahIDList = WilayahIDList;
        //    ViewBag.LadangIDList = LadangIDList;
        //    return View();
        //}

        //public ViewResult _TransactionListingRptSearch(int? WilayahIDList, int? LadangIDList, int? MonthList, int? YearList)
        //{
        //    if (WilayahIDList == null && LadangIDList == null)
        //    {
        //        ViewBag.Message = "Sila Pilih Wilayah Dan Ladang ";
        //        return View();
        //    }
        //    else if (WilayahIDList == 0 && LadangIDList == 0)
        //    {
        //        ViewBag.Message = "Sila Pilih Wilayah Dan Ladang ";
        //        return View();
        //    }
        //    Connection Connection = new Connection();
        //    int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
        //    int? getuserid = getidentity.ID(User.Identity.Name);
        //    string host, catalog, user, pass = "";
        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
        //    Connection.GetConnection(out host, out catalog, out user, out pass, 2, SyarikatID.Value,
        //        NegaraID.Value);
        //    MVC_SYSTEM_ViewingModels dbview = MVC_SYSTEM_ViewingModels.ConnectToSqlServer(host, catalog, user, pass);
        //    MVC_SYSTEM_ViewingModels dbview2 = new MVC_SYSTEM_ViewingModels();
        //    MVC_SYSTEM_ModelsCorporate dbhq = new MVC_SYSTEM_ModelsCorporate();

        //    ViewBag.MonthList = MonthList;
        //    ViewBag.YearList = YearList;

        //    ViewBag.NamaSyarikat = dbhq.tbl_Syarikat
        //        .Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID)
        //        .Select(s => s.fld_NamaSyarikat)
        //        .FirstOrDefault();
        //    ViewBag.NoSyarikat = dbhq.tbl_Syarikat
        //        .Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID)
        //        .Select(s => s.fld_NoSyarikat)
        //        .FirstOrDefault();
        //    ViewBag.NegaraID = NegaraID;
        //    ViewBag.SyarikatID = SyarikatID;
        //    ViewBag.UserID = getuserid;
        //    ViewBag.UserName = User.Identity.Name;
        //    ViewBag.Date = DateTime.Now.ToShortDateString();
        //    ViewBag.IDPenyelia = getuserid;


        //    if (MonthList == null && YearList == null)
        //    {
        //        ViewBag.Message = "Sila Pilih Bulan Dan Tahun ";
        //        return View();
        //    }

        //    else
        //    {
        //        if (WilayahIDList == 2 && LadangIDList == 0)
        //        {
        //            LadangIDList = 92;
        //        }

        //        var TransactionListingList = dbview.vw_RptSctran
        //            .Where(x => x.fld_KodAktvt != "3803" && x.fld_KodAktvt != "3800" && x.fld_Month == MonthList &&
        //                        x.fld_Year == YearList && x.fld_NegaraID == NegaraID &&
        //                        x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahIDList &&
        //                        x.fld_LadangID == LadangIDList)
        //            .OrderBy(o => o.fld_Kategori);

        //        if (!TransactionListingList.Any())
        //        {
        //            ViewBag.Message = "Tiada Rekod";
        //            return View();

        //        }
        //        ViewBag.NamaPengurus = dbhq.tbl_Ladang
        //        .Where(x => x.fld_ID == LadangIDList)
        //        .Select(s => s.fld_Pengurus).Single();
        //        ViewBag.NamaPenyelia = dbhq.tblUsers
        //            .Where(x => x.fldUserID == getuserid)
        //            .Select(s => s.fldUserFullName).Single();
        //        return View(TransactionListingList);
        //    }
        //}

        //public ActionResult PaySlipReport()
        //{
        //    ViewBag.Report = "class = active";
        //    //Connection Connection = new Connection();
        //    int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
        //    int? getuserid = getidentity.ID(User.Identity.Name);
        //    //string host, catalog, user, pass = "";
        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
        //    //Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
        //    //MVC_SYSTEM_ModelsEstate dbr = MVC_SYSTEM_ModelsEstate.ConnectToSqlServer(host, catalog, user, pass);
        //    MVC_SYSTEM_ModelsCorporate dbhq = new MVC_SYSTEM_ModelsCorporate();
        //    int[] wlyhid = new int[] { };
        //    int drpyear = 0;
        //    int drprangeyear = 0;
        //    int month = timezone.gettimezone().Month;

        //    //List<SelectListItem> SelectionList = new List<SelectListItem>();
        //    //SelectionList = new SelectList(
        //    //    dbr.tbl_Pkjmast
        //    //        .Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
        //    //                    x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Kdaktf == "1")
        //    //        .OrderBy(o => o.fld_Nopkj)
        //    //        .Select(s => new SelectListItem { Value = s.fld_Nopkj, Text = s.fld_Nopkj + "-" + s.fld_Nama }),
        //    //    "Value", "Text").ToList();
        //    //SelectionList.Insert(0, (new SelectListItem { Text = "Semua", Value = "0" }));

        //    //ViewBag.SelectionList = SelectionList;

        //    List<SelectListItem> WilayahIDList = new List<SelectListItem>();
        //    List<SelectListItem> LadangIDList = new List<SelectListItem>();

        //    if (WilayahID == 0 && LadangID == 0)
        //    {
        //        wlyhid = getwilyah.GetWilayahID(SyarikatID);
        //        // mywlyid = String.Join("", wlyhid); ;
        //        WilayahIDList = new SelectList(db.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
        //        WilayahIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        LadangIDList = new SelectList(db.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
        //        LadangIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        //selectionrpt = 1;
        //        //wilayahselection = WilayahID;
        //        //ladangselection = LadangID;
        //        //incldg = 1;
        //    }
        //    else if (WilayahID != 0 && LadangID == 0)
        //    {
        //        //mywlyid = String.Join("", WilayahID); ;
        //        wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
        //        WilayahIDList = new SelectList(db.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
        //        LadangIDList = new SelectList(db.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
        //        LadangIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        //selectionrpt = 2;
        //        //wilayahselection = WilayahID;
        //        //ladangselection = LadangID;
        //        //incldg = 2;

        //    }
        //    else if (WilayahID != 0 && LadangID != 0)
        //    {
        //        //mywlyid = String.Join("", WilayahID); ;
        //        wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
        //        WilayahIDList = new SelectList(db.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
        //        LadangIDList = new SelectList(db.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_ID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
        //        //selectionrpt = 3;
        //        //wilayahselection = WilayahID;
        //        //ladangselection = LadangID;
        //        //incldg = 3;
        //    }

        //    drpyear = timezone.gettimezone().Year - int.Parse(GetConfig.GetData("yeardisplay")) + 1;
        //    drprangeyear = timezone.gettimezone().Year;

        //    var yearlist = new List<SelectListItem>();
        //    for (var i = drpyear; i <= drprangeyear; i++)
        //    {
        //        if (i == timezone.gettimezone().Year)
        //        {
        //            yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString(), Selected = true });
        //        }
        //        else
        //        {
        //            yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
        //        }
        //    }

        //    ViewBag.YearList = yearlist;

        //    //var statusList = new List<SelectListItem>();
        //    //statusList = new SelectList(
        //    //    dbhq.tblOptionConfigsWeb
        //    //        .Where(x => x.fldOptConfFlag1 == "statusaktif" && x.fldDeleted == false)
        //    //        .OrderBy(o => o.fldOptConfDesc)
        //    //        .Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }),
        //    //    "Value", "Text").ToList();

        //    var monthList = new SelectList(
        //        dbhq.tblOptionConfigsWeb.Where(x => x.fldOptConfFlag1 == "monthlist" && x.fldDeleted == false),
        //        "fldOptConfValue", "fldOptConfDesc", month);

        //    ViewBag.MonthList = monthList;
        //    ViewBag.WilayahIDList = WilayahIDList;
        //    ViewBag.LadangIDList = LadangIDList;
        //    //ViewBag.StatusList = statusList;

        //    return View();
        //}

        //public ActionResult _WorkerPaySlipRptAdvanceSearch()
        //{
        //    Connection Connection = new Connection();
        //    int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
        //    int? getuserid = getidentity.ID(User.Identity.Name);
        //    string host, catalog, user, pass = "";
        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
        //    Connection.GetConnection(out host, out catalog, out user, out pass, 2, SyarikatID.Value, NegaraID.Value);
        //    MVC_SYSTEM_ViewingModels dbview = MVC_SYSTEM_ViewingModels.ConnectToSqlServer(host, catalog, user, pass);
        //    MVC_SYSTEM_ModelsEstate dbr = MVC_SYSTEM_ModelsEstate.ConnectToSqlServer(host, catalog, user, pass);
        //    MVC_SYSTEM_ModelsCorporate dbhq = new MVC_SYSTEM_ModelsCorporate();

        //    var statusList = new SelectList(
        //        dbhq.tblOptionConfigsWeb
        //            .Where(x => x.fldOptConfFlag1 == "statusaktif" && x.fldDeleted == false)
        //            .OrderBy(o => o.fldOptConfDesc)
        //            .Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }),
        //        "Value", "Text").ToList();

        //    ViewBag.StatusList = statusList;

        //    var workCategoryList = new SelectList(
        //        dbhq.tblOptionConfigsWeb
        //            .Where(x => x.fldOptConfFlag1 == "designation" && x.fldDeleted == false)
        //            .OrderBy(o => o.fldOptConfDesc)
        //            .Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }),
        //        "Value", "Text").ToList();

        //    ViewBag.WorkCategoryList = workCategoryList;

        //    return View();
        //}

        //public ViewResult _WorkerPaySlipRptSearch(int? WilayahIDList, int? LadangIDList, int? MonthList, int? YearList)
        //{
        //    List<vw_PaySlipPekerja> PaySlipPekerja = new List<vw_PaySlipPekerja>();
        //    if (WilayahIDList == null && LadangIDList == null)
        //    {
        //        ViewBag.Message = "Sila Pilih Bulan, Tahun Dan Pekerja";
        //        return View(PaySlipPekerja);
        //    }
        //    else if (WilayahIDList == 0 && LadangIDList == 0)
        //    {
        //        ViewBag.Message = "Sila Pilih Bulan, Tahun Dan Pekerja";
        //        return View(PaySlipPekerja);
        //    }
        //    ViewBag.WilayahIDList = WilayahIDList;
        //    ViewBag.LadangIDList = LadangIDList;

        //    Connection Connection = new Connection();
        //    int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
        //    int? getuserid = getidentity.ID(User.Identity.Name);
        //    string host, catalog, user, pass = "";
        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
        //    Connection.GetConnection(out host, out catalog, out user, out pass, 2, SyarikatID.Value,
        //        NegaraID.Value);
        //    MVC_SYSTEM_ViewingModels dbview = MVC_SYSTEM_ViewingModels.ConnectToSqlServer(host, catalog, user, pass);
        //    MVC_SYSTEM_ModelsCorporate dbhq = new MVC_SYSTEM_ModelsCorporate();



        //    ViewBag.MonthList = MonthList;
        //    ViewBag.YearList = YearList;
        //    //ViewBag.WorkerList = SelectionList;
        //    ViewBag.NamaSyarikat = dbhq.tbl_Syarikat
        //        .Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID)
        //        .Select(s => s.fld_NamaSyarikat)
        //        .FirstOrDefault();
        //    ViewBag.NoSyarikat = dbhq.tbl_Syarikat
        //        .Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID)
        //        .Select(s => s.fld_NoSyarikat)
        //        .FirstOrDefault();
        //    ViewBag.NegaraID = NegaraID;
        //    ViewBag.SyarikatID = SyarikatID;
        //    ViewBag.UserID = getuserid;
        //    ViewBag.UserName = User.Identity.Name;
        //    ViewBag.Date = DateTime.Now.ToShortDateString();

        //    if (MonthList == null && YearList == null)
        //    {
        //        ViewBag.Message = "Sila Pilih Bulan, Tahun Dan Pekerja";
        //        return View(PaySlipPekerja);
        //    }
        //    else
        //    {
        //        IOrderedQueryable<ViewingModelsOPMS.vw_GajiPekerja> workerData;

        //        if (WilayahIDList == 2 && LadangIDList == 0)
        //        {
        //            LadangIDList = 92;
        //        }

        //        workerData = dbview.vw_GajiPekerja
        //            .Where(x => x.fld_Kdaktf == "1" && x.fld_NegaraID == NegaraID &&
        //                        x.fld_Year == YearList && x.fld_Month == MonthList &&
        //                        x.fld_SyarikatID == SyarikatID &&
        //                        x.fld_WilayahID == WilayahIDList && x.fld_LadangID == LadangIDList)
        //            .OrderBy(x => x.fld_Nama);

        //        foreach (var i in workerData)
        //        {

        //            List<ViewingModelsOPMS.vw_MaklumatInsentif> workerIncentiveRecordList = new List<ViewingModelsOPMS.vw_MaklumatInsentif>();

        //            List<FootNoteCustomModel> footNoteCustomModelList = new List<FootNoteCustomModel>();

        //            var workerMonthlySalary = dbview.tbl_GajiBulanan
        //                .SingleOrDefault(x => x.fld_Nopkj == i.fld_Nopkj && x.fld_Month == MonthList &&
        //                            x.fld_Year == YearList && x.fld_NegaraID == NegaraID &&
        //                            x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahIDList &&
        //                            x.fld_LadangID == LadangIDList);

        //            var workerIncentiveRecord = dbview.vw_MaklumatInsentif
        //                .Where(x => x.fld_Nopkj == i.fld_Nopkj && x.fld_Month == MonthList &&
        //                            x.fld_Year == YearList && x.fld_NegaraID == NegaraID &&
        //                            x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahIDList &&
        //                            x.fld_LadangID == LadangIDList && x.fld_Deleted == false);

        //            foreach (var insentifRecord in workerIncentiveRecord)
        //            {
        //                workerIncentiveRecordList.Add(insentifRecord);
        //            }

        //            List<KerjaPekerjaCustomModel> kerjaPekerjaCustomModelList = new List<KerjaPekerjaCustomModel>();

        //            var workerWorkRecordGroupBy = dbview.vw_KerjaPekerja
        //               .Where(x => x.fld_Nopkj == i.fld_Nopkj && x.fld_Tarikh.Value.Month == MonthList &&
        //                            x.fld_Tarikh.Value.Year == YearList)
        //                .GroupBy(x => new { x.fld_KodAktvt, x.fld_KodPkt, x.fld_Kdhdct })
        //                .OrderBy(o => o.Key.fld_KodAktvt)
        //                .ThenBy(t => t.Key.fld_KodPkt)
        //                .ThenBy(t2 => t2.Key.fld_Kdhdct)
        //                .Select(lg =>
        //                    new
        //                    {
        //                        fld_ID = lg.FirstOrDefault().fld_ID,
        //                        fld_Desc = lg.FirstOrDefault().fld_Desc,
        //                        fld_KodPkt = lg.FirstOrDefault().fld_KodPkt,
        //                        fld_JumlahHasil = lg.Sum(w => w.fld_JumlahHasil),
        //                        fld_Unit = lg.FirstOrDefault().fld_Unit,
        //                        fld_KadarByr = lg.FirstOrDefault().fld_KadarByr,
        //                        fld_Gandaan = lg.FirstOrDefault().fldOptConfFlag3,
        //                        fld_TotalAmount = lg.Sum(w => w.fld_Amount)
        //                    });

        //            foreach (var work in workerWorkRecordGroupBy)
        //            {
        //                KerjaPekerjaCustomModel kerjaPekerjaCustomModel = new KerjaPekerjaCustomModel();

        //                kerjaPekerjaCustomModel.fld_ID = work.fld_ID;
        //                kerjaPekerjaCustomModel.fld_Desc = work.fld_Desc;
        //                kerjaPekerjaCustomModel.fld_KodPkt = work.fld_KodPkt;
        //                kerjaPekerjaCustomModel.fld_JumlahHasil = work.fld_JumlahHasil;
        //                kerjaPekerjaCustomModel.fld_Unit = work.fld_Unit;
        //                kerjaPekerjaCustomModel.fld_KadarByr = work.fld_KadarByr;
        //                kerjaPekerjaCustomModel.fld_Gandaan = work.fld_Gandaan;
        //                kerjaPekerjaCustomModel.fld_TotalAmount = work.fld_TotalAmount;

        //                kerjaPekerjaCustomModelList.Add(kerjaPekerjaCustomModel);
        //            }

        //            List<OTPekerjaCustomModel> otPekerjaCustomModelList = new List<OTPekerjaCustomModel>();

        //            var workerOTRecordGroupBy = dbview.vw_OTPekerja
        //                .Where(x => x.fld_Nopkj == i.fld_Nopkj && x.fld_Tarikh.Value.Month == MonthList &&
        //                            x.fld_Tarikh.Value.Year == YearList)
        //                .GroupBy(x => x.fld_Kdhdct)
        //                .OrderBy(o => o.Key)
        //                .Select(lg =>
        //                    new
        //                    {
        //                        fld_ID = lg.FirstOrDefault().fld_ID,
        //                        fld_JumlahJamOT = lg.Sum(w => w.fld_JamOT),
        //                        fld_Desc = lg.FirstOrDefault().fldDesc,
        //                        fld_KadarByr = lg.FirstOrDefault().fld_Kadar,
        //                        fld_Gandaan = lg.FirstOrDefault().fldRate,
        //                        fld_TotalAmount = lg.Sum(w => w.fld_Jumlah)
        //                    });

        //            foreach (var ot in workerOTRecordGroupBy)
        //            {
        //                OTPekerjaCustomModel otPekerjaCustomModel = new OTPekerjaCustomModel();

        //                otPekerjaCustomModel.fld_ID = ot.fld_ID;
        //                otPekerjaCustomModel.fld_Desc = "Lebih Masa " + ot.fld_Desc;
        //                otPekerjaCustomModel.fld_JumlahJamOT = ot.fld_JumlahJamOT;
        //                otPekerjaCustomModel.fld_Unit = "JAM";
        //                otPekerjaCustomModel.fld_KadarByr = ot.fld_KadarByr;
        //                otPekerjaCustomModel.fld_Gandaan = ot.fld_Gandaan;
        //                otPekerjaCustomModel.fld_TotalAmount = ot.fld_TotalAmount;

        //                otPekerjaCustomModelList.Add(otPekerjaCustomModel);

        //                FootNoteCustomModel otFootNoteCustomModel = new FootNoteCustomModel();

        //                otFootNoteCustomModel.fld_Desc = "Lebih Masa " + ot.fld_Desc;
        //                otFootNoteCustomModel.fld_Bilangan = ot.fld_JumlahJamOT;

        //                footNoteCustomModelList.Add(otFootNoteCustomModel);
        //            }

        //            List<BonusPekerjaCustomModel> bonusPekerjaCustomModelList = new List<BonusPekerjaCustomModel>();

        //            var workerBonusRecordGroupBy = dbview.vw_BonusPekerja
        //                .Where(x => x.fld_Nopkj == i.fld_Nopkj && x.fld_Tarikh.Value.Month == MonthList &&
        //                            x.fld_Tarikh.Value.Year == YearList)
        //                .GroupBy(x => new { x.fld_KodPkt, x.fld_Bonus })
        //                .OrderBy(o => o.Key.fld_KodPkt)
        //                .ThenBy(t => t.Key.fld_Bonus)
        //                .Select(lg =>
        //                    new
        //                    {
        //                        fld_ID = lg.FirstOrDefault().fld_ID,
        //                        fld_Desc = lg.FirstOrDefault().fld_Desc,
        //                        fld_KodPkt = lg.FirstOrDefault().fld_KodPkt,
        //                        fld_BilanganHari = lg.Count(),
        //                        fld_Bonus = lg.FirstOrDefault().fld_Bonus,
        //                        fld_KadarByr = lg.FirstOrDefault().fld_Kadar,
        //                        fld_TotalAmount = lg.Sum(w => w.fld_Jumlah)
        //                    });

        //            foreach (var ot in workerBonusRecordGroupBy)
        //            {
        //                BonusPekerjaCustomModel bonusPekerjaCustomModel = new BonusPekerjaCustomModel();

        //                bonusPekerjaCustomModel.fld_ID = ot.fld_ID;
        //                bonusPekerjaCustomModel.fld_Desc = ot.fld_Desc;
        //                bonusPekerjaCustomModel.fld_BilanganHari = ot.fld_BilanganHari;
        //                bonusPekerjaCustomModel.fld_KodPkt = ot.fld_KodPkt;
        //                bonusPekerjaCustomModel.fld_Bonus = ot.fld_Bonus;
        //                bonusPekerjaCustomModel.fld_KadarByr = ot.fld_KadarByr;
        //                bonusPekerjaCustomModel.fld_TotalAmount = ot.fld_TotalAmount;

        //                bonusPekerjaCustomModelList.Add(bonusPekerjaCustomModel);
        //            }

        //            List<CutiPekerjaCustomModel> cutiPekerjaCustomModelList = new List<CutiPekerjaCustomModel>();

        //            var workerLeaveRecordGroupBy = dbview.vw_CutiPekerja
        //                .Where(x => x.fld_Nopkj == i.fld_Nopkj && x.fld_Tarikh.Value.Month == MonthList &&
        //                            x.fld_Tarikh.Value.Year == YearList)
        //                .GroupBy(x => new { x.fld_Kdhdct })
        //                .OrderBy(o => o.Key.fld_Kdhdct)
        //                .Select(lg =>
        //                    new
        //                    {
        //                        fld_ID = lg.FirstOrDefault().fld_ID,
        //                        fld_Desc = lg.FirstOrDefault().fldOptConfDesc,
        //                        fld_BilanganHari = lg.Count(),
        //                        fld_KadarByr = lg.FirstOrDefault().fld_Kadar,
        //                        fld_TotalAmount = lg.Sum(w => w.fld_Jumlah)
        //                    });

        //            foreach (var ot in workerLeaveRecordGroupBy)
        //            {
        //                CutiPekerjaCustomModel cutiPekerjaCustomModel = new CutiPekerjaCustomModel();

        //                cutiPekerjaCustomModel.fld_ID = ot.fld_ID;
        //                cutiPekerjaCustomModel.fld_Desc = ot.fld_Desc;
        //                cutiPekerjaCustomModel.fld_BilanganHari = ot.fld_BilanganHari;
        //                cutiPekerjaCustomModel.fld_KadarByr = ot.fld_KadarByr;
        //                cutiPekerjaCustomModel.fld_TotalAmount = ot.fld_TotalAmount;

        //                cutiPekerjaCustomModelList.Add(cutiPekerjaCustomModel);
        //            }

        //            var workerWorkingDay = dbview.vw_KehadiranPekerja
        //                .Where(x => x.fld_Nopkj == i.fld_Nopkj && x.fld_Tarikh.Value.Month == MonthList &&
        //                            x.fld_Tarikh.Value.Year == YearList)
        //                .GroupBy(x => new { x.fld_Kdhdct })
        //                .OrderBy(o => o.Key.fld_Kdhdct)
        //                .Select(lg =>
        //                    new
        //                    {
        //                        fld_Desc = lg.FirstOrDefault().fldOptConfDesc,
        //                        fld_Bilangan = lg.Count(),
        //                    });

        //            foreach (var workingDay in workerWorkingDay)
        //            {
        //                FootNoteCustomModel footNoteCustomModel = new FootNoteCustomModel();

        //                footNoteCustomModel.fld_Desc = workingDay.fld_Desc;
        //                footNoteCustomModel.fld_Bilangan = workingDay.fld_Bilangan;

        //                footNoteCustomModelList.Add(footNoteCustomModel);
        //            }

        //            var workerRainDay = dbview.vw_KehadiranPekerja
        //                .Count(x => x.fld_Nopkj == i.fld_Nopkj && x.fld_Tarikh.Value.Month == MonthList &&
        //                            x.fld_Tarikh.Value.Year == YearList && x.fld_Hujan == 1);

        //            if (workerRainDay != 0)
        //            {
        //                FootNoteCustomModel footNoteHariHujanCustomModel = new FootNoteCustomModel();

        //                footNoteHariHujanCustomModel.fld_Desc = "Jumlah Hari Hujan";
        //                footNoteHariHujanCustomModel.fld_Bilangan = workerRainDay;

        //                footNoteCustomModelList.Add(footNoteHariHujanCustomModel);
        //            }

        //            PaySlipPekerja.Add(
        //                new vw_PaySlipPekerja()
        //                {
        //                    Pkjmast = i,
        //                    GajiBulanan = workerMonthlySalary,
        //                    InsentifPekerja = workerIncentiveRecordList,
        //                    KerjaPekerja = kerjaPekerjaCustomModelList,
        //                    OTPekerja = otPekerjaCustomModelList,
        //                    BonusPekerja = bonusPekerjaCustomModelList,
        //                    CutiPekerja = cutiPekerjaCustomModelList,
        //                    FootNote = footNoteCustomModelList
        //                });
        //        }

        //        if (PaySlipPekerja.Count == 0)
        //        {
        //            ViewBag.Message = "Tiada Rekod";
        //        }

        //        return View(PaySlipPekerja);
        //    }
        //}





        public JsonResult GetList(int wlyh, int ldg, int RadioGroup, string StatusList)
        {
            Connection Connection = new Connection();
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, 2, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_ModelsEstate dbr = MVC_SYSTEM_ModelsEstate.ConnectToSqlServer(host, catalog, user, pass);
            List<SelectListItem> SelectionList = new List<SelectListItem>();
            string SelectionLabel = "";

            if (RadioGroup == 0)
            {
                if (String.IsNullOrEmpty(StatusList))
                {
                    //Individu Semua
                    SelectionLabel = "Pekerja";

                    SelectionList = new SelectList(
                        dbr.tbl_Pkjmast
                            .Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                        x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Kdaktf == "1")
                            .OrderBy(o => o.fld_Nopkj)
                            .Select(
                                s => new SelectListItem { Value = s.fld_Nopkj, Text = s.fld_Nopkj + "-" + s.fld_Nama }),
                        "Value", "Text").ToList();
                    SelectionList.Insert(0, (new SelectListItem { Text = "Semua", Value = "0" }));
                }

                else
                {
                    //Individu Semua
                    SelectionLabel = "Pekerja";
                    if (StatusList == "0")
                    {
                        SelectionList = new SelectList(dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == wlyh && x.fld_LadangID == ldg).OrderBy(o => o.fld_Nopkj).Select(s => new SelectListItem { Value = s.fld_Nopkj, Text = s.fld_Nopkj + "-" + s.fld_Nama }), "Value", "Text").ToList();
                        SelectionList.Insert(0, (new SelectListItem { Text = "Semua", Value = "0" }));
                    }
                    else
                    {
                        if (ldg == 0)
                        {
                            SelectionList = new SelectList(dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == wlyh && x.fld_Kdaktf == StatusList).OrderBy(o => o.fld_Nopkj).Select(s => new SelectListItem { Value = s.fld_Nopkj, Text = s.fld_Nopkj + "-" + s.fld_Nama }), "Value", "Text").ToList();
                            SelectionList.Insert(0, (new SelectListItem { Text = "Semua", Value = "0" }));
                        }
                        else
                        {
                            SelectionList = new SelectList(dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == wlyh && x.fld_LadangID == ldg && x.fld_Kdaktf == StatusList).OrderBy(o => o.fld_Nopkj).Select(s => new SelectListItem { Value = s.fld_Nopkj, Text = s.fld_Nopkj + "-" + s.fld_Nama }), "Value", "Text").ToList();
                            SelectionList.Insert(0, (new SelectListItem { Text = "Semua", Value = "0" }));
                        }

                    }
                }

            }
            else
            {
                //Group
                if (ldg == 0)
                {
                    SelectionLabel = "Kumpulan";
                    SelectionList = new SelectList(dbr.vw_KumpulanKerja.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == wlyh && x.fld_deleted == false).OrderBy(o => o.fld_KodKumpulan).Select(s => new SelectListItem { Value = s.fld_KodKumpulan, Text = s.fld_KodKumpulan + "-" + s.fld_Keterangan }), "Value", "Text").ToList();
                    SelectionList.Insert(0, (new SelectListItem { Text = "Semua", Value = "0" }));
                }
                else
                {
                    SelectionLabel = "Kumpulan";
                    SelectionList = new SelectList(dbr.vw_KumpulanKerja.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == wlyh && x.fld_LadangID == ldg && x.fld_deleted == false).OrderBy(o => o.fld_KodKumpulan).Select(s => new SelectListItem { Value = s.fld_KodKumpulan, Text = s.fld_KodKumpulan + "-" + s.fld_Keterangan }), "Value", "Text").ToList();
                    SelectionList.Insert(0, (new SelectListItem { Text = "Semua", Value = "0" }));
                }

            }
            return Json(new { SelectionList = SelectionList, SelectionLabel = SelectionLabel });
        }
        
        //[AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3,Viewer")]
        //public ActionResult MyegReport()
        //{
        //    int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
        //    int? getuserid = getidentity.ID(User.Identity.Name);
        //    string host, catalog, user, pass = "";
        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
        //    List<SelectListItem> wilayahList = new List<SelectListItem>();
        //    List<SelectListItem> ldgList = new List<SelectListItem>();
        //    List<SelectListItem> krytnlist = new List<SelectListItem>();
        //    wilayahList = new SelectList(db.tbl_Wilayah.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).OrderBy(o => o.fld_ID).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_WlyhName }), "Value", "Text").ToList();
        //    ldgList = new SelectList(db.tbl_Ladang.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).OrderBy(o => o.fld_ID).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + "-" + s.fld_LdgName }), "Value", "Text").ToList();
        //    krytnlist = new SelectList(db.tblOptionConfigsWeb.Where(x => x.fldOptConfFlag1 == "krytnlist" && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).OrderBy(o => o.fldOptConfID).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();
        //    krytnlist.Insert(0, (new SelectListItem { Text = GlobalResCorp.lblAll, Value = "0" }));


        //    ViewBag.Report = "class = active";
        //    ViewBag.MonthList = new SelectList(db.tblOptionConfigsWeb.Where(x => x.fldOptConfFlag1 == "exprdmonthlist" && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID), "fldOptConfValue", "fldOptConfDesc");
        //    ViewBag.KrytnList = krytnlist;
        //    ViewBag.WilayahList = wilayahList;
        //    ViewBag.LdgList = ldgList;

        //    return View();
        //}

        //public ActionResult _MyegReport(string RadioGroup, string MonthList, int? WilayahList, int? LdgList, string KrytnList, int page = 1, string sort = "fld_Nopkj", string sortdir = "ASC")
        //{
        //    int? getuserid = getidentity.ID(User.Identity.Name);
        //    int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
        //    List<CustMod_Myeg> InfoMyegList = new List<CustMod_Myeg>();

        //    if (MonthList == null && KrytnList == null)
        //    {
        //        ViewBag.Message = GlobalResCorp.msgChooseWork;
        //        return View();
        //    }

        //    DateTime todaydate = DateTime.Today;
        //    DateTime startdate = DateTime.Today.AddMonths(int.Parse(MonthList));


        //    if (RadioGroup != "0")
        //    {

        //        dbSP.SetCommandTimeout(120);
        //        var result = dbSP.sp_MyegDetail(NegaraID, SyarikatID, WilayahList, LdgList).ToList();


        //        return View(result);
        //    }
        //    else
        //    {
        //        dbSP.SetCommandTimeout(120);
        //        var result1 = dbSP.sp_MyegDetail(NegaraID, SyarikatID, WilayahList, LdgList).ToList();

        //        return View(result1);
        //    }

        //}

        //public ActionResult PaySheetReport()
        //{

        //    ViewBag.Report = "class = active";
        //    //Connection Connection = new Connection();
        //    int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
        //    int? getuserid = getidentity.ID(User.Identity.Name);
        //    //string host, catalog, user, pass = "";
        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
        //    //Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
        //    //MVC_SYSTEM_ModelsEstate dbr = MVC_SYSTEM_ModelsEstate.ConnectToSqlServer(host, catalog, user, pass);
        //    MVC_SYSTEM_ModelsCorporate dbhq = new MVC_SYSTEM_ModelsCorporate();
        //    int[] wlyhid = new int[] { };
        //    int drpyear = 0;
        //    int drprangeyear = 0;
        //    int month = timezone.gettimezone().Month;

        //    //List<SelectListItem> SelectionList = new List<SelectListItem>();
        //    //SelectionList = new SelectList(
        //    //    dbr.tbl_Pkjmast
        //    //        .Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
        //    //                    x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Kdaktf == "1")
        //    //        .OrderBy(o => o.fld_Nopkj)
        //    //        .Select(s => new SelectListItem { Value = s.fld_Nopkj, Text = s.fld_Nopkj + "-" + s.fld_Nama }),
        //    //    "Value", "Text").ToList();
        //    //SelectionList.Insert(0, (new SelectListItem { Text = "Semua", Value = "0" }));

        //    //ViewBag.SelectionList = SelectionList;
        //    List<SelectListItem> WilayahIDList = new List<SelectListItem>();
        //    List<SelectListItem> LadangIDList = new List<SelectListItem>();

        //    if (WilayahID == 0 && LadangID == 0)
        //    {
        //        wlyhid = getwilyah.GetWilayahID(SyarikatID);
        //        // mywlyid = String.Join("", wlyhid); ;
        //        WilayahIDList = new SelectList(db.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
        //        WilayahIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
        //        LadangIDList = new SelectList(db.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
        //        LadangIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));

        //    }
        //    else if (WilayahID != 0 && LadangID == 0)
        //    {
        //        //mywlyid = String.Join("", WilayahID); ;
        //        wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
        //        WilayahIDList = new SelectList(db.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
        //        LadangIDList = new SelectList(db.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
        //        LadangIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));

        //    }
        //    else if (WilayahID != 0 && LadangID != 0)
        //    {
        //        //mywlyid = String.Join("", WilayahID); ;
        //        wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
        //        WilayahIDList = new SelectList(db.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
        //        LadangIDList = new SelectList(db.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_ID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();

        //    }
        //    drpyear = timezone.gettimezone().Year - int.Parse(GetConfig.GetData("yeardisplay")) + 1;
        //    drprangeyear = timezone.gettimezone().Year;

        //    var yearlist = new List<SelectListItem>();
        //    for (var i = drpyear; i <= drprangeyear; i++)
        //    {
        //        if (i == timezone.gettimezone().Year)
        //        {
        //            yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString(), Selected = true });
        //        }
        //        else
        //        {
        //            yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
        //        }
        //    }

        //    ViewBag.YearList = yearlist;

        //    //var statusList = new List<SelectListItem>();
        //    //statusList = new SelectList(
        //    //    dbhq.tblOptionConfigsWeb
        //    //        .Where(x => x.fldOptConfFlag1 == "statusaktif" && x.fldDeleted == false)
        //    //        .OrderBy(o => o.fldOptConfDesc)
        //    //        .Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }),
        //    //    "Value", "Text").ToList();

        //    var monthList = new SelectList(
        //        dbhq.tblOptionConfigsWeb.Where(x => x.fldOptConfFlag1 == "monthlist" && x.fldDeleted == false),
        //        "fldOptConfValue", "fldOptConfDesc", month);

        //    ViewBag.MonthList = monthList;
        //    //ViewBag.StatusList = statusList;
        //    ViewBag.WilayahIDList = WilayahIDList;
        //    ViewBag.LadangIDList = LadangIDList;
        //    return View();
        //}

        //public ActionResult _WorkerPaySheetRptAdvanceSearch()
        //{
        //    Connection Connection = new Connection();
        //    int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
        //    int? getuserid = getidentity.ID(User.Identity.Name);
        //    string host, catalog, user, pass = "";
        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
        //    Connection.GetConnection(out host, out catalog, out user, out pass, 2, SyarikatID.Value, NegaraID.Value);
        //    MVC_SYSTEM_ViewingModels dbview = MVC_SYSTEM_ViewingModels.ConnectToSqlServer(host, catalog, user, pass);
        //    MVC_SYSTEM_ModelsEstate dbr = MVC_SYSTEM_ModelsEstate.ConnectToSqlServer(host, catalog, user, pass);
        //    MVC_SYSTEM_ModelsCorporate dbhq = new MVC_SYSTEM_ModelsCorporate();

        //    var statusList = new SelectList(
        //        dbhq.tblOptionConfigsWeb
        //            .Where(x => x.fldOptConfFlag1 == "statusaktif" && x.fldDeleted == false)
        //            .OrderBy(o => o.fldOptConfDesc)
        //            .Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }),
        //        "Value", "Text").ToList();

        //    ViewBag.StatusList = statusList;

        //    var workCategoryList = new SelectList(
        //        dbhq.tblOptionConfigsWeb
        //            .Where(x => x.fldOptConfFlag1 == "designation" && x.fldDeleted == false)
        //            .OrderBy(o => o.fldOptConfDesc)
        //            .Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }),
        //        "Value", "Text").ToList();

        //    ViewBag.WorkCategoryList = workCategoryList;

        //    return View();
        //}

        //public ViewResult _WorkerPaySheetRptSearch(int? WilayahIDList, int? LadangIDList, int? MonthList, int? YearList)
        //{
        //    List<vw_PaySheetPekerjaCustomModel> PaySheetPekerjaList = new List<vw_PaySheetPekerjaCustomModel>();
        //    if (WilayahIDList == null && LadangIDList == null)
        //    {
        //        ViewBag.Message = "Sila Pilih Bulan, Tahun Dan Pekerja";
        //        return View(PaySheetPekerjaList);
        //    }
        //    else if (WilayahIDList == 0 && LadangIDList == 0)
        //    {
        //        ViewBag.Message = "Sila Pilih Bulan, Tahun Dan Pekerja";
        //        return View(PaySheetPekerjaList);
        //    }
        //    ViewBag.WilayahIDList = WilayahIDList;
        //    ViewBag.LadangIDList = LadangIDList;



        //    Connection Connection = new Connection();
        //    int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
        //    int? getuserid = getidentity.ID(User.Identity.Name);
        //    string host, catalog, user, pass = "";
        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
        //    Connection.GetConnection(out host, out catalog, out user, out pass, 2, SyarikatID.Value,
        //        NegaraID.Value);
        //    MVC_SYSTEM_ViewingModels dbview = MVC_SYSTEM_ViewingModels.ConnectToSqlServer(host, catalog, user, pass);
        //    MVC_SYSTEM_ViewingModels dbview2 = new MVC_SYSTEM_ViewingModels();
        //    MVC_SYSTEM_ModelsCorporate dbhq = new MVC_SYSTEM_ModelsCorporate();

        //    //List<vw_PaySheetPekerjaCustomModel> PaySheetPekerjaList = new List<vw_PaySheetPekerjaCustomModel>();

        //    ViewBag.MonthList = MonthList;
        //    ViewBag.YearList = YearList;
        //    //ViewBag.WorkerList = SelectionList;
        //    ViewBag.NamaSyarikat = dbhq.tbl_Syarikat
        //        .Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID)
        //        .Select(s => s.fld_NamaSyarikat)
        //        .FirstOrDefault();
        //    ViewBag.NoSyarikat = dbhq.tbl_Syarikat
        //        .Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID)
        //        .Select(s => s.fld_NoSyarikat)
        //        .FirstOrDefault();
        //    ViewBag.NegaraID = NegaraID;
        //    ViewBag.SyarikatID = SyarikatID;
        //    ViewBag.UserID = getuserid;
        //    ViewBag.UserName = User.Identity.Name;
        //    ViewBag.Date = DateTime.Now.ToShortDateString();
        //    ViewBag.NamaPengurus = dbhq.tbl_Ladang
        //        .Where(x => x.fld_ID == LadangIDList)
        //        .Select(s => s.fld_Pengurus).Single();
        //    ViewBag.NamaPenyelia = dbhq.tblUsers
        //       .Where(x => x.fldUserID == getuserid)
        //       .Select(s => s.fldUserFullName).Single();
        //    ViewBag.IDPenyelia = getuserid;

        //    if (MonthList == null && YearList == null)
        //    {
        //        ViewBag.Message = "Sila Pilih Bulan, Tahun Dan Pekerja";
        //        return View(PaySheetPekerjaList);
        //    }

        //    else
        //    {
        //        IOrderedQueryable<ViewingModelsOPMS.vw_PaySheetPekerja> salaryData;

        //        if (WilayahIDList == 2 && LadangIDList == 0)
        //        {
        //            LadangIDList = 92;
        //        }

        //        salaryData = dbview.vw_PaySheetPekerja
        //            .Where(x => x.fld_NegaraID == NegaraID &&
        //                                    x.fld_Year == YearList && x.fld_Month == MonthList &&
        //                                    x.fld_SyarikatID == SyarikatID &&
        //                                    x.fld_WilayahID == WilayahIDList && x.fld_LadangID == LadangIDList)
        //                        .OrderBy(x => x.fld_Nama);

        //        foreach (var salary in salaryData)
        //        {
        //            PaySheetPekerjaList.Add(
        //                new vw_PaySheetPekerjaCustomModel()
        //                {
        //                    PaySheetPekerja = salary
        //                });
        //        }

        //        if (PaySheetPekerjaList.Count == 0)
        //        {
        //            ViewBag.Message = "Tiada Rekod";
        //        }

        //        return View(PaySheetPekerjaList);
        //    }
        //}
    }
}