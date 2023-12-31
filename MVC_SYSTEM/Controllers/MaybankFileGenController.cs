﻿using MVC_SYSTEM.Attributes;
using MVC_SYSTEM.Class;
using MVC_SYSTEM.log;
using MVC_SYSTEM.ModelsCorporate;
using MVC_SYSTEM.Models;
using MVC_SYSTEM.App_LocalResources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MVC_SYSTEM.ViewingModels;
using MVC_SYSTEM.ModelsSP;
using iTextSharp.text.pdf;
using iTextSharp.text;
using System.Data.Entity;
using System.IO;
//using Itenso.TimePeriod;
//using System.Globalization;
//using System.Drawing;

namespace MVC_SYSTEM.Controllers
{
    [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3")]
    public class MaybankFileGenController : Controller
    {
        private MVC_SYSTEM_ModelsCorporate dbC = new MVC_SYSTEM_ModelsCorporate();
        private MVC_SYSTEM_Models db = new MVC_SYSTEM_Models();
        private GetIdentity getidentity = new GetIdentity();
        private GetTriager GetTriager = new GetTriager();
        private GetNSWL GetNSWL = new GetNSWL();
        private ChangeTimeZone timezone = new ChangeTimeZone();
        private errorlog geterror = new errorlog();
        private GetConfig GetConfig = new GetConfig();
        private GetIdentity GetIdentity = new GetIdentity();
        private GetWilayah getWilayah = new GetWilayah();
        private Connection Connection = new Connection();
        private MVC_SYSTEM_SP2_Models dbSP = new MVC_SYSTEM_SP2_Models();
        //ConvertToPdf ConvertToPdf = new ConvertToPdf();
        //private GetGenerateFile GetGenerateFile = new GetGenerateFile();
        // GET: MaybankFileGen
        public ActionResult Index()
        {
            ViewBag.MaybankFileGen = "class = active";
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            List<SelectListItem> sublist = new List<SelectListItem>();
            ViewBag.MenuSubList = sublist;
            ViewBag.MenuList = new SelectList(dbC.tblMenuLists.Where(x => x.fld_Flag == "m2e" && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_Desc }), "Value", "Text").ToList();
            db.Dispose();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(string MenuList, string MenuSubList)
        {
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            if (MenuSubList != null)
            {
                return RedirectToAction(MenuSubList, "MaybankFileGen");
            }
            else
            {
                int menulist = int.Parse(MenuList);
                var action = dbC.tblMenuLists.Where(x => x.fld_ID == menulist && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).Select(s => s.fld_Val).FirstOrDefault();
                db.Dispose();
                return RedirectToAction(action, "MaybankFileGen");
            }
        }

        public JsonResult GetSubList(int ListID)
        {
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            var findsub = dbC.tblMenuLists.Where(x => x.fld_ID == ListID).Select(s => s.fld_Sub).FirstOrDefault();
            List<SelectListItem> sublist = new List<SelectListItem>();
            if (findsub != null)
            {
                sublist = new SelectList(dbC.tblMenuLists.Where(x => x.fld_Flag == findsub && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).OrderBy(o => o.fld_ID).Select(s => new SelectListItem { Value = s.fld_Val, Text = s.fld_Desc }), "Value", "Text").ToList();
            }
            db.Dispose();
            return Json(sublist);
        }

        [HttpPost]
        public ActionResult DownloadText(int Month, int Year, string CompCode, string filter, string[] WorkerId, DateTime PaymentDate)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
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

            ViewBag.MaybankFileGen = "class = active";

            try
            {
                GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
                List<sp_MaybankRcms_Result> maybankrcmsList = new List<sp_MaybankRcms_Result>();

                if (WorkerId == null)
                    WorkerId = new string[] { "0" };

                if (WorkerId.Contains("0"))
                {
                    maybankrcmsList = dbSP.sp_MaybankRcms(NegaraID.Value, SyarikatID.Value, Year, Month, getuserid, CompCode).ToList();
                }
                else
                {
                    maybankrcmsList = dbSP.sp_MaybankRcms(NegaraID.Value, SyarikatID.Value, Year, Month, getuserid, CompCode).Where(x => WorkerId.Contains(x.fld_Nopkj)).ToList();
                }

                var SyarikatDetail = db.tbl_Syarikat.Where(x => x.fld_NamaPndkSyarikat == CompCode).FirstOrDefault();

                filePath = GetGenerateFile.GenerateFileMaybank(maybankrcmsList, SyarikatDetail, stringmonth, stringyear, NegaraID, SyarikatID, CompCode, filter, PaymentDate, out filename);

                link = Url.Action("Download", "MaybankFileGen", new { filePath, filename });

                //dbr.Dispose();

                msg = GlobalResCorp.msgGenerateSuccess;
                statusmsg = "success";
            }
            catch (Exception ex)
            {
                geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                msg = GlobalResCorp.msgGenerateFailed;
                statusmsg = "warning";
            }

            return Json(new { msg, statusmsg, link });
        }

        [HttpPost]
        public ActionResult DownloadTextIndividu(int Month, int Year, string CompCode, /*int Wilayah,*/ string filter, string[] WorkerId, DateTime PaymentDate)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
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

            ViewBag.MaybankFileGen = "class = active";

            try
            {
                GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
                List<sp_MaybankRcms_Result> maybankrcmsList = new List<sp_MaybankRcms_Result>();

                if (WorkerId == null)
                    WorkerId = new string[] { "0" };

                if (WorkerId.Contains("0"))
                {
                    maybankrcmsList = dbSP.sp_MaybankRcms(NegaraID.Value, SyarikatID.Value, Year, Month, getuserid, CompCode).ToList();
                }
                else
                {
                    maybankrcmsList = dbSP.sp_MaybankRcms(NegaraID.Value, SyarikatID.Value, Year, Month, getuserid, CompCode).Where(x => WorkerId.Contains(x.fld_Nopkj)).ToList();
                }

                //var WilayahDetail = db.tbl_Wilayah.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_ID == Wilayah).FirstOrDefault();
                var SyarikatDetail = db.tbl_Syarikat.Where(x => x.fld_NamaPndkSyarikat == CompCode).FirstOrDefault();

                filePath = GetGenerateFile.GenerateFileMaybank(maybankrcmsList, SyarikatDetail, stringmonth, stringyear, NegaraID, SyarikatID, /*Wilayah,*/ CompCode, filter, PaymentDate, out filename);

                link = Url.Action("Download", "MaybankFileGen", new { filePath, filename });

                //dbr.Dispose();

                msg = GlobalResCorp.msgGenerateSuccess;
                statusmsg = "success";
            }
            catch (Exception ex)
            {
                geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                msg = GlobalResCorp.msgGenerateFailed;
                statusmsg = "warning";
            }

            return Json(new { msg, statusmsg, link });
        }

        public JsonResult CheckGenDataDetail(int Month, int Year, string CompCode, /*int Wilayah,*/ string[] WorkerId)
        {
            string msg = "";
            string statusmsg = "";
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";

            string stringyear = "";
            string stringmonth = "";
            string CorpID = "";
            string ClientID = "";
            string ClientIDText = "";
            string AccNo = "";
            string InitialName = "";
            stringyear = Year.ToString();
            stringmonth = Month.ToString();
            stringmonth = (stringmonth.Length == 1 ? "0" + stringmonth : stringmonth);
            decimal? TotalGaji = 0;
            int CountData = 0;

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            GetNSWL.GetSyarikatRCMSDetail(CompCode, out CorpID, out ClientID, out AccNo, out InitialName);
            //Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);

            List<sp_MaybankRcms_Result> maybankrcmsList = new List<sp_MaybankRcms_Result>();
            if (WorkerId == null)
                WorkerId = new string[] { "0" };

            if (WorkerId.Contains("0"))
            {
                maybankrcmsList = dbSP.sp_MaybankRcms(NegaraID.Value, SyarikatID.Value, Year, Month, getuserid, CompCode).ToList();
            }
            else
            {
                maybankrcmsList = dbSP.sp_MaybankRcms(NegaraID.Value, SyarikatID.Value, Year, Month, getuserid, CompCode).Where(x => WorkerId.Contains(x.fld_Nopkj)).ToList();
            }
            //var WilayahDetail = dbC.tbl_Wilayah.Where(x => x.fld_ID == Wilayah).FirstOrDefault();
            var SyarikatDetail = dbC.tbl_Syarikat.Where(x => x.fld_NamaPndkSyarikat == CompCode).FirstOrDefault();
            string filename = "M2E LABOR (" + SyarikatDetail.fld_NamaPndkSyarikat.ToUpper() + ") " + "" + stringmonth + stringyear + ".txt";

            if (ClientID == null || ClientID == " ")
            {
                if (CompCode == "FASSB")
                {
                    ClientIDText = "FGVASB" + stringmonth + stringyear;
                }

                if (CompCode == "RNDSB")
                {
                    ClientIDText = "RNDSB" + stringmonth + stringyear;
                }               
            }
            else
            {
                ClientIDText = ClientID;
            }

            if (maybankrcmsList.Count() != 0)
            {
                TotalGaji = maybankrcmsList.Sum(s => s.fld_GajiBersih);
                CountData = maybankrcmsList.Count();
                msg = GlobalResCorp.msgDataFound;
                statusmsg = "success";
            }
            else
            {
                msg = GlobalResCorp.msgDataNotFound;
                statusmsg = "warning";
            }

            dbSP.Dispose();
            dbC.Dispose();
            return Json(new { msg, statusmsg, file = filename, salary = TotalGaji, totaldata = CountData, clientid = ClientIDText});
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

        public ActionResult RcmsGen()
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";

            DateTime Minus1month = timezone.gettimezone().AddMonths(-1);
            int year = Minus1month.Year;
            int month = Minus1month.Month;
            int drpyear = 0;
            int drprangeyear = 0;

            ViewBag.MaybankFileGen = "class = active";

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            drpyear = timezone.gettimezone().Year - int.Parse(GetConfig.GetData("yeardisplay")) + 1;
            drprangeyear = timezone.gettimezone().Year;

            var yearlist = new List<SelectListItem>();
            for (var i = drpyear; i <= drprangeyear; i++)
            {
                if (i == year)
                {
                    yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString(), Selected = true });
                }
                else
                {
                    yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
                }
            }

            ViewBag.YearList = yearlist;

            ViewBag.MonthList = new SelectList(dbC.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "monthlist" && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID), "fldOptConfValue", "fldOptConfDesc", month);

            List<SelectListItem> CompCodeList = new List<SelectListItem>();
        
            CompCodeList = new SelectList(dbC.tbl_Syarikat.OrderBy(x => x.fld_NamaPndkSyarikat), "fld_NamaPndkSyarikat", "fld_NamaPndkSyarikat").ToList();

            CompCodeList.Insert(0, (new SelectListItem { Text = "Please Select", Value = "0" }));
            ViewBag.CompCodeList = CompCodeList;

            //WilayahList.Insert(0, (new SelectListItem { Text = "Please Select", Value = "0" }));
            //ViewBag.WilayahList = WilayahList;


            ViewBag.UserID = getuserid;
            //dbC.Dispose();
            return View();
        }

        public ViewResult _rcms(string CompCodeList, int? MonthList, int? YearList, string print, string filter)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            //string WilayahName = "";
            string NamaSyarikat = "";
            string ClientId = "";
            //string LdgCode = "";

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            List<sp_MaybankRcms_Result> maybankrcmsList = new List<sp_MaybankRcms_Result>();

            ViewBag.MonthList = MonthList;
            ViewBag.YearList = YearList;
            ViewBag.NamaSyarikat = dbC.tbl_Syarikat
                .Where(x => x.fld_NegaraID == NegaraID && x.fld_NamaPndkSyarikat == CompCodeList)
                .Select(s => s.fld_NamaSyarikat)
                .FirstOrDefault();
            ViewBag.NamaPendekSyarikat = dbC.tbl_Syarikat
               .Where(x => x.fld_NegaraID == NegaraID && x.fld_NamaPndkSyarikat == CompCodeList)
               .Select(s => s.fld_NamaPndkSyarikat)
               .FirstOrDefault();
            ViewBag.NoSyarikat = dbC.tbl_Syarikat
                .Where(x => x.fld_NegaraID == NegaraID && x.fld_NamaPndkSyarikat == CompCodeList)
                .Select(s => s.fld_NoSyarikat)
                .FirstOrDefault();
            ViewBag.CorpID = dbC.tbl_Syarikat
                .Where(x => x.fld_NegaraID == NegaraID && x.fld_NamaPndkSyarikat == CompCodeList)
                .Select(s => s.fld_CorporateID)
                .FirstOrDefault();
            ClientId = dbC.tbl_Syarikat
                .Where(x => x.fld_NegaraID == NegaraID && x.fld_NamaPndkSyarikat == CompCodeList)
                .Select(s => s.fld_ClientBatchID)
                .FirstOrDefault();
            if (ClientId == null || ClientId == "")
            {
                if (CompCodeList == "FASSB")
                {
                    ViewBag.clientid = "FGVASB" + MonthList + YearList;
                }

                if (CompCodeList == "RNDSB")
                {
                    ViewBag.clientid = "RNDSB" + MonthList + YearList;
                }
            }
            else
            {
                ViewBag.clientid = ClientId;
            }

            ViewBag.AccNo = dbC.tbl_Syarikat
                .Where(x => x.fld_NegaraID == NegaraID && x.fld_NamaPndkSyarikat == CompCodeList)
                .Select(s => s.fld_AccountNo)
                .FirstOrDefault();
            //ViewBag.WilayahName = db.tbl_Wilayah
            //    .Where(x => x.fld_ID == WilayahList && x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID)
            //    .Select(s => s.fld_WlyhName)
            //    .FirstOrDefault();
            //var LdgDetail = db.tbl_Ladang
            //    .Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_ID == LadangID)
            //    .FirstOrDefault();
            //LdgName = db.tbl_Ladang
            //    .Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_ID == LadangID)
            //    .Select(s => s.fld_LdgName)
            //    .FirstOrDefault();
            //LdgCode = db.tbl_Ladang
            //    .Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_ID == LadangID)
            //    .Select(s => s.fld_LdgCode)
            //    .FirstOrDefault();
            //ViewBag.Ladang = LdgName.Trim();
            //ViewBag.LadangCode = LdgCode.Trim();
            //ViewBag.OriginatorId = LdgDetail.fld_OriginatorID;
            //ViewBag.OriginatorName = LdgDetail.fld_OriginatorName;
            //ViewBag.AccNo = LdgDetail.fld_NoAcc;
            ViewBag.NegaraID = NegaraID;
            ViewBag.SyarikatID = SyarikatID;
            ViewBag.UserID = getuserid;
            ViewBag.UserName = User.Identity.Name;
            ViewBag.Date = DateTime.Now.ToShortDateString();
            ViewBag.Time = DateTime.Now.ToShortTimeString();
            ViewBag.Print = print;
            //ViewBag.WilayahName = WilayahName;
            ViewBag.Description = "Region " + NamaSyarikat + " - Salary payment for " + MonthList + "/" + YearList;
            if (MonthList == null || YearList == null || CompCodeList == "0")
            {
                ViewBag.Message = "Please select month, year, company and payment date";
                return View(maybankrcmsList);
            }
            else
            {
                dbSP.SetCommandTimeout(2400);
                maybankrcmsList = dbSP.sp_MaybankRcms(NegaraID, SyarikatID, YearList, MonthList, getuserid, CompCodeList).ToList();

                var BankList = dbC.tbl_Bank
                    .Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_Deleted == false)
                    .ToList();

                ViewBag.RecordNo = maybankrcmsList.Count();

                if (maybankrcmsList.Count() == 0)
                {
                    ViewBag.Message = GlobalResCorp.msgNoRecord;
                }

                if (filter != "")
                {
                    ViewBag.filter = filter;
                }
                return View(maybankrcmsList);
            }
        }


        public ActionResult RcmsGenIndividu()
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";

            DateTime Minus1month = timezone.gettimezone().AddMonths(-1);
            int year = Minus1month.Year;
            int month = Minus1month.Month;
            int drpyear = 0;
            int drprangeyear = 0;

            ViewBag.MaybankFileGen = "class = active";

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            drpyear = timezone.gettimezone().Year - int.Parse(GetConfig.GetData("yeardisplay")) + 1;
            drprangeyear = timezone.gettimezone().Year;

            var yearlist = new List<SelectListItem>();
            for (var i = drpyear; i <= drprangeyear; i++)
            {
                if (i == year)
                {
                    yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString(), Selected = true });
                }
                else
                {
                    yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
                }
            }

            ViewBag.YearList = yearlist;

            ViewBag.MonthList = new SelectList(dbC.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "monthlist" && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID), "fldOptConfValue", "fldOptConfDesc", month);

            List<SelectListItem> CompCodeList = new List<SelectListItem>();
            CompCodeList = new SelectList(dbC.tbl_Syarikat.OrderBy(x => x.fld_NamaPndkSyarikat), "fld_NamaPndkSyarikat", "fld_NamaPndkSyarikat").ToList();

            CompCodeList.Insert(0, (new SelectListItem { Text = "Please Select", Value = "0" }));
            ViewBag.CompCodeList = CompCodeList;

            //List<SelectListItem> WilayahList = new List<SelectListItem>();
            //WilayahList = new SelectList(dbC.tbl_Wilayah.Where(x => x.fld_Deleted == false && x.fld_SyarikatID == SyarikatID)
            //    .OrderBy(o => o.fld_WlyhName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_WlyhName }), "Value", "Text").ToList();
            //WilayahList.Insert(0, (new SelectListItem { Text = "Sila Pilih", Value = "0" }));
            //ViewBag.WilayahList = WilayahList;

            ViewBag.UserID = getuserid;
            //dbC.Dispose();
            return View();
        }

        public ViewResult _rcmsIndividu(string CompCodeList, int? MonthList, int? YearList, string print, string filter, string[] WorkerId)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            //string WilayahName = "";
            string NamaSyarikat = "";
            string ClientId = "";
            //string LdgCode = "";

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            List<sp_MaybankRcms_Result> maybankrcmsList = new List<sp_MaybankRcms_Result>();

            ViewBag.MonthList = MonthList;
            ViewBag.YearList = YearList;
            ViewBag.NamaSyarikat = dbC.tbl_Syarikat
                .Where(x => x.fld_NegaraID == NegaraID && x.fld_NamaPndkSyarikat == CompCodeList)
                .Select(s => s.fld_NamaSyarikat)
                .FirstOrDefault();
            ViewBag.NamaPendekSyarikat = dbC.tbl_Syarikat
               .Where(x => x.fld_NegaraID == NegaraID && x.fld_NamaPndkSyarikat == CompCodeList)
               .Select(s => s.fld_NamaPndkSyarikat)
               .FirstOrDefault();
            ViewBag.NoSyarikat = dbC.tbl_Syarikat
                .Where(x => x.fld_NegaraID == NegaraID && x.fld_NamaPndkSyarikat == CompCodeList)
                .Select(s => s.fld_NoSyarikat)
                .FirstOrDefault();
            ViewBag.CorpID = dbC.tbl_Syarikat
                .Where(x => x.fld_NegaraID == NegaraID && x.fld_NamaPndkSyarikat == CompCodeList)
                .Select(s => s.fld_CorporateID)
                .FirstOrDefault();
            ClientId = dbC.tbl_Syarikat
                 .Where(x => x.fld_NegaraID == NegaraID && x.fld_NamaPndkSyarikat == CompCodeList)
                 .Select(s => s.fld_ClientBatchID)
                 .FirstOrDefault();
            if (ClientId == null || ClientId == "")
            {
                if (CompCodeList == "FASSB")
                {
                    ViewBag.clientid = "FGVASB" + MonthList + YearList;
                }

                if (CompCodeList == "RNDSB")
                {
                    ViewBag.clientid = "RNDSB" + MonthList + YearList;
                }
            }
            else
            {
                ViewBag.clientid = ClientId;
            }

            ViewBag.AccNo = dbC.tbl_Syarikat
                .Where(x => x.fld_NegaraID == NegaraID && x.fld_NamaPndkSyarikat == CompCodeList)
                .Select(s => s.fld_AccountNo)
                .FirstOrDefault();
            //ViewBag.WilayahName = db.tbl_Wilayah
            //    .Where(x => x.fld_ID == WilayahList && x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID)
            //    .Select(s => s.fld_WlyhName)
            //    .FirstOrDefault();
            ViewBag.NegaraID = NegaraID;
            ViewBag.SyarikatID = SyarikatID;
            ViewBag.UserID = getuserid;
            ViewBag.UserName = User.Identity.Name;
            ViewBag.Date = DateTime.Now.ToShortDateString();
            ViewBag.Time = DateTime.Now.ToShortTimeString();
            ViewBag.Print = print;
            //ViewBag.WilayahName = WilayahName;

            ViewBag.Description = "Region " + NamaSyarikat + " - Salary payment for " + MonthList + "/" + YearList;
            if (MonthList == null && YearList == null)
            {
                ViewBag.Message = "Please select month, year, company and payment date";
                return View(maybankrcmsList);
            }
            else
            {
                if (WorkerId.Contains("0"))
                {
                    dbSP.SetCommandTimeout(2400);
                    maybankrcmsList = dbSP.sp_MaybankRcms(NegaraID, SyarikatID, YearList, MonthList, getuserid, CompCodeList).ToList();
                }
                else
                {
                    dbSP.SetCommandTimeout(2400);
                    maybankrcmsList = dbSP.sp_MaybankRcms(NegaraID, SyarikatID, YearList, MonthList, getuserid, CompCodeList).Where(x => WorkerId.Contains(x.fld_Nopkj)).ToList();
                }

                var BankList = dbC.tbl_Bank
                    .Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_Deleted == false)
                    .ToList();

                ViewBag.RecordNo = maybankrcmsList.Count();

                if (maybankrcmsList.Count() == 0)
                {
                    ViewBag.Message = GlobalResCorp.msgNoRecord;
                }

                if (filter != "")
                {
                    ViewBag.filter = filter;
                }
                return View(maybankrcmsList);
            }
        }

        public JsonResult GetWorker(string CompCode, int Year, int Month)
        {
            List<SelectListItem> workerList = new List<SelectListItem>();
            var userDetail = getidentity.GetUserDetail(User.Identity.Name);
            var getuserid = userDetail.fldUserID;
            var maybankrcmsList = dbSP.sp_MaybankRcms(userDetail.fldNegaraID, userDetail.fldSyarikatID, Year, Month, getuserid, CompCode).Select(s => new { s.fld_Nopkj, s.fld_Nama }).OrderBy(o => o.fld_Nama).ToList();

            if (maybankrcmsList.Count() > 0)
            {
                workerList = new SelectList(maybankrcmsList.Select(s => new SelectListItem { Value = s.fld_Nopkj.ToString(), Text = s.fld_Nopkj + " - " + s.fld_Nama }), "Value", "Text").ToList();
            }

            return Json(workerList);
        }

        public ActionResult SSCOnlinePayment()
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";

            DateTime Minus1month = timezone.gettimezone().AddMonths(-1);
            int year = Minus1month.Year;
            int month = Minus1month.Month;
            int drpyear = 0;
            int drprangeyear = 0;

            ViewBag.MaybankFileGen = "class = active";

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            drpyear = timezone.gettimezone().Year - int.Parse(GetConfig.GetData("yeardisplay")) + 1;
            drprangeyear = timezone.gettimezone().Year;

            var yearlist = new List<SelectListItem>();
            for (var i = drpyear; i <= drprangeyear; i++)
            {
                if (i == year)
                {
                    yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString(), Selected = true });
                }
                else
                {
                    yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
                }
            }

            ViewBag.YearList = yearlist;

            ViewBag.MonthList = new SelectList(dbC.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "monthlist" && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID), "fldOptConfValue", "fldOptConfDesc", month);


            List<SelectListItem> CompCodeList = new List<SelectListItem>();

            CompCodeList = new SelectList(dbC.tbl_Syarikat.OrderBy(x => x.fld_NamaPndkSyarikat), "fld_NamaPndkSyarikat", "fld_NamaPndkSyarikat").ToList();

            CompCodeList.Insert(0, (new SelectListItem { Text = "Please Select", Value = "0" }));
            ViewBag.CompCodeList = CompCodeList;

            ViewBag.UserID = getuserid;
            //dbC.Dispose();
            return View();
        }

        public ViewResult _SSCOnlinePayment(string CompCodeList, int? MonthList, int? YearList)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            //string WilayahName = "";
            string NamaPendekSyarikat = "";
            //string LdgCode = "";

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            List<sp_MaybankRcmsOnlinePaymentRpt_Result> ssconlinepayment = new List<sp_MaybankRcmsOnlinePaymentRpt_Result>();

            ViewBag.MonthList = MonthList;
            ViewBag.YearList = YearList;
            ViewBag.NamaSyarikat = dbC.tbl_Syarikat
                .Where(x => x.fld_NegaraID == NegaraID && x.fld_NamaPndkSyarikat == CompCodeList)
                .Select(s => s.fld_NamaSyarikat)
                .FirstOrDefault();
            ViewBag.NamaPendekSyarikat = dbC.tbl_Syarikat
               .Where(x => x.fld_NegaraID == NegaraID && x.fld_NamaPndkSyarikat == CompCodeList)
               .Select(s => s.fld_NamaPndkSyarikat)
               .FirstOrDefault();
            ViewBag.NoSyarikat = dbC.tbl_Syarikat
                .Where(x => x.fld_NegaraID == NegaraID && x.fld_NamaPndkSyarikat == CompCodeList)
                .Select(s => s.fld_NoSyarikat)
                .FirstOrDefault();
            ViewBag.CorpID = dbC.tbl_Syarikat
                .Where(x => x.fld_NegaraID == NegaraID && x.fld_NamaPndkSyarikat == CompCodeList)
                .Select(s => s.fld_CorporateID)
                .FirstOrDefault();
            ViewBag.ClientID = dbC.tbl_Syarikat
                .Where(x => x.fld_NegaraID == NegaraID && x.fld_NamaPndkSyarikat == CompCodeList)
                .Select(s => s.fld_ClientBatchID)
                .FirstOrDefault();
            ViewBag.AccNo = dbC.tbl_Syarikat
                .Where(x => x.fld_NegaraID == NegaraID && x.fld_NamaPndkSyarikat == CompCodeList)
                .Select(s => s.fld_AccountNo)
                .FirstOrDefault();
            ViewBag.NegaraID = NegaraID;
            ViewBag.SyarikatID = SyarikatID;
            ViewBag.UserID = getuserid;
            ViewBag.UserName = User.Identity.Name;
            ViewBag.Date = DateTime.Now.ToShortDateString();
            ViewBag.Time = DateTime.Now.ToShortTimeString();
            //ViewBag.Print = print;
            //ViewBag.WilayahName = WilayahName;
            ViewBag.Description = "Region " + NamaPendekSyarikat + " - SSC Online Payment Report for " + MonthList + "/" + YearList;
            if (MonthList == null || YearList == null || CompCodeList == "0")
            {
                ViewBag.Message = "Please select month, year, and company";
                return View(ssconlinepayment);
            }
            else
            {
                dbSP.SetCommandTimeout(2400);
                ssconlinepayment = dbSP.sp_MaybankRcmsOnlinePaymentRpt(NegaraID, SyarikatID, YearList, MonthList, getuserid, CompCodeList).ToList();

                var BankList = dbC.tbl_Bank
                    .Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_Deleted == false)
                    .ToList();

                ViewBag.RecordNo = ssconlinepayment.Count();

                if (ssconlinepayment.Count() == 0)
                {
                    ViewBag.Message = GlobalResCorp.msgNoRecord;
                }

                return View(ssconlinepayment);
            }
        }

        public ActionResult RcmsZAP64()
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";

            DateTime Minus1month = timezone.gettimezone().AddMonths(-1);
            int year = Minus1month.Year;
            int month = Minus1month.Month;
            int drpyear = 0;
            int drprangeyear = 0;

            ViewBag.MaybankFileGen = "class = active";

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            drpyear = timezone.gettimezone().Year - int.Parse(GetConfig.GetData("yeardisplay")) + 1;
            drprangeyear = timezone.gettimezone().Year;

            var yearlist = new List<SelectListItem>();
            for (var i = drpyear; i <= drprangeyear; i++)
            {
                if (i == year)
                {
                    yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString(), Selected = true });
                }
                else
                {
                    yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
                }
            }

            ViewBag.YearList = yearlist;

            ViewBag.MonthList = new SelectList(dbC.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "monthlist" && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID), "fldOptConfValue", "fldOptConfDesc", month);

            List<SelectListItem> CompCodeList = new List<SelectListItem>();

            CompCodeList = new SelectList(dbC.tbl_Syarikat.OrderBy(x => x.fld_NamaPndkSyarikat), "fld_NamaPndkSyarikat", "fld_NamaPndkSyarikat").ToList();

            CompCodeList.Insert(0, (new SelectListItem { Text = "Please Select", Value = "0" }));
            ViewBag.CompCodeList = CompCodeList;

            ViewBag.UserID = getuserid;
            //dbC.Dispose();
            return View();
        }

        public ViewResult _RcmsZAP64(string CompCodeList, int? MonthList, int? YearList, string print, string PaymentDate)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            //string WilayahName = "";
            string NamaSyarikat = "";
            //string LdgCode = "";

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            List<sp_MaybankRcmsZAP64_Result> maybankrcmsZAP64 = new List<sp_MaybankRcmsZAP64_Result>();

            
            ViewBag.MonthList = MonthList;
            ViewBag.YearList = YearList;
            ViewBag.NamaSyarikat = dbC.tbl_Syarikat
                .Where(x => x.fld_NegaraID == NegaraID && x.fld_NamaPndkSyarikat == CompCodeList)
                .Select(s => s.fld_NamaSyarikat)
                .FirstOrDefault();
            ViewBag.NamaPendekSyarikat = dbC.tbl_Syarikat
               .Where(x => x.fld_NegaraID == NegaraID && x.fld_NamaPndkSyarikat == CompCodeList)
               .Select(s => s.fld_NamaPndkSyarikat)
               .FirstOrDefault();
            ViewBag.NoSyarikat = dbC.tbl_Syarikat
                .Where(x => x.fld_NegaraID == NegaraID && x.fld_NamaPndkSyarikat == CompCodeList)
                .Select(s => s.fld_NoSyarikat)
                .FirstOrDefault();
            ViewBag.CorpID = dbC.tbl_Syarikat
                .Where(x => x.fld_NegaraID == NegaraID && x.fld_NamaPndkSyarikat == CompCodeList)
                .Select(s => s.fld_CorporateID)
                .FirstOrDefault();
            ViewBag.ClientID = dbC.tbl_Syarikat
                .Where(x => x.fld_NegaraID == NegaraID && x.fld_NamaPndkSyarikat == CompCodeList)
                .Select(s => s.fld_ClientBatchID)
                .FirstOrDefault();
            ViewBag.AccNo = dbC.tbl_Syarikat
                .Where(x => x.fld_NegaraID == NegaraID && x.fld_NamaPndkSyarikat == CompCodeList)
                .Select(s => s.fld_AccountNo)
                .FirstOrDefault();
           
            ViewBag.NegaraID = NegaraID;
            ViewBag.SyarikatID = SyarikatID;
            ViewBag.UserID = getuserid;
            ViewBag.UserName = User.Identity.Name;
            ViewBag.Date = DateTime.Now.ToShortDateString();
            ViewBag.Time = DateTime.Now.ToShortTimeString();
            ViewBag.Print = print;

            if(YearList == null && MonthList == null)
            {
                ViewBag.DocDate = DateTime.Now.AddMonths(+1).AddDays(-DateTime.Now.Day).ToString("dd.MM.yyyy");
            }
            else
            {
                var lastday = DateTime.DaysInMonth(YearList.Value, MonthList.Value);
                ViewBag.DocDate = lastday + "." + MonthList + "." + YearList;
            }
            ViewBag.PostingDate = Convert.ToDateTime(PaymentDate).ToString("dd.MM.yyyy");
            ViewBag.Description = "Region " + NamaSyarikat + " - Maybank Rcms ZAP64 for " + MonthList + "/" + YearList;
            if (MonthList == null || YearList == null || CompCodeList == "0")
            {
                ViewBag.Message = "Please select month, year, company and payment date";
                return View(maybankrcmsZAP64);
            }
            else
            {
                dbSP.SetCommandTimeout(2400);
                maybankrcmsZAP64 = dbSP.sp_MaybankRcmsZAP64(NegaraID, SyarikatID, YearList, MonthList, getuserid, CompCodeList).ToList();

                var BankList = dbC.tbl_Bank
                    .Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_Deleted == false)
                    .ToList();

                ViewBag.RecordNo = maybankrcmsZAP64.Count();

                if (maybankrcmsZAP64.Count() == 0)
                {
                    ViewBag.Message = GlobalResCorp.msgNoRecord;
                }


                return View(maybankrcmsZAP64);
            }
        }

        [HttpPost]
        public ActionResult ConvertPDF2(string myHtml, string filename, string reportname)
        {
            bool success = false;
            string msg = "";
            string status = "";
            Models.tblHtmlReport tblHtmlReport = new Models.tblHtmlReport();

            tblHtmlReport.fldHtlmCode = myHtml;
            tblHtmlReport.fldFileName = filename;
            tblHtmlReport.fldReportName = reportname;

            db.tblHtmlReports.Add(tblHtmlReport);
            db.SaveChanges();

            success = true;
            status = "success";

            return Json(new { success = success, id = tblHtmlReport.fldID, msg = msg, status = status, link = Url.Action("GetPDF", "MaybankFileGen", null, "https") + "/" + tblHtmlReport.fldID });
        }

        public ActionResult GetPDF(int id)
        {
            int? NegaraID = 0;
            int? SyarikatID = 0;
            int? WilayahID = 0;
            int? LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string width = "1700", height = "1190";
            string imagepath = Server.MapPath("~/Asset/Images/");

            var gethtml = db.tblHtmlReports.Find(id);

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            var logosyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_LogoName).FirstOrDefault();


            Document pdfDoc = new Document(new iTextSharp.text.Rectangle(int.Parse(width), int.Parse(height)), 50f, 50f, 50f, 50f);

            PdfWriter writer = PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
            pdfDoc.Open();

            using (TextReader sr = new StringReader(gethtml.fldHtlmCode))
            {
                using (var htmlWorker = new HTMLWorkerExtended(pdfDoc, imagepath + logosyarikat))
                {
                    htmlWorker.Open();
                    htmlWorker.Parse(sr);
                }
            }
            pdfDoc.Close();
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "attachment;filename=" + gethtml.fldFileName + ".pdf");
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Write(pdfDoc);
            Response.End();

            db.Entry(gethtml).State = EntityState.Deleted;
            db.SaveChanges();
            return View();
        }

        [HttpPost]
        public ActionResult ConvertPDFRpt(string myHtml, string filename, string reportname)
        {
            bool success = false;
            string msg = "";
            string status = "";
            Models.tblHtmlReport tblHtmlReport = new Models.tblHtmlReport();

            tblHtmlReport.fldHtlmCode = myHtml;
            tblHtmlReport.fldFileName = filename;
            tblHtmlReport.fldReportName = reportname;

            db.tblHtmlReports.Add(tblHtmlReport);
            db.SaveChanges();

            success = true;
            status = "success";

            return Json(new { success = success, id = tblHtmlReport.fldID, msg = msg, status = status, link = Url.Action("GetPDFRpt", "MaybankFileGen", null, "https") + "/" + tblHtmlReport.fldID });
        }

        public ActionResult GetPDFRpt(int id)
        {
            int? NegaraID = 0;
            int? SyarikatID = 0;
            int? WilayahID = 0;
            int? LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string width = "816", height = "1056";
            //string width = "1000", height = "1190";
            string imagepath = Server.MapPath("~/Asset/Images/");

            var gethtml = db.tblHtmlReports.Find(id);

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            var logosyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_LogoName).FirstOrDefault();


            Document pdfDoc = new Document(new iTextSharp.text.Rectangle(int.Parse(width), int.Parse(height)), 50f, 50f, 50f, 50f);

            PdfWriter writer = PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
            pdfDoc.Open();

            using (TextReader sr = new StringReader(gethtml.fldHtlmCode))
            {
                using (var htmlWorker = new HTMLWorkerExtended(pdfDoc, imagepath + logosyarikat))
                {
                    htmlWorker.Open();
                    htmlWorker.Parse(sr);
                }
            }
            pdfDoc.Close();
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "attachment;filename=" + gethtml.fldFileName + ".pdf");
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Write(pdfDoc);
            Response.End();

            db.Entry(gethtml).State = EntityState.Deleted;
            db.SaveChanges();
            return View();
        }

        //public JsonResult GetWilayah(string SyarikatID)
        //{
        //    List<SelectListItem> wilayahlist = new List<SelectListItem>();
        //    //List<SelectListItem> ladanglist = new List<SelectListItem>();

        //    int? NegaraID = 0;
        //    int? SyarikatID2 = 0;
        //    int? WilayahID = 0;
        //    int? LadangID = 0;
        //    int? getuserid = GetIdentity.ID(User.Identity.Name);

        //    GetNSWL.GetData(out NegaraID, out SyarikatID2, out WilayahID, out LadangID, getuserid, User.Identity.Name);
        //    var syarikatCodeId = dbC.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fldOptConfValue == SyarikatID.ToString() && x.fld_NegaraID == NegaraID).Select(x => x.fld_SyarikatID).FirstOrDefault();
        //    int SyarikatCode = Convert.ToInt16(syarikatCodeId);

        //    if (getWilayah.GetAvailableWilayah(SyarikatCode))
        //    {
        //        if (WilayahID == 0)
        //        {
        //            //dapatkan ladang filter by costcenter
        //            var listladang2 = db.tbl_Ladang.Where(x => x.fld_CostCentre == SyarikatID && x.fld_SyarikatID == SyarikatCode && x.fld_Deleted == false).Select(x => x.fld_WlyhID).ToList();
        //            var listwilayah1 = db.tbl_Wilayah.Where(x => x.fld_Deleted == false && listladang2.Contains(x.fld_ID)).ToList();
        //            wilayahlist = new SelectList(listwilayah1.OrderBy(o => o.fld_WlyhName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_WlyhName }), "Value", "Text").ToList();
        //            wilayahlist.Insert(0, (new SelectListItem { Text = @GlobalResCorp.lblChoose, Value = "0" }));
        //            //ladanglist.Insert(0, (new SelectListItem { Text = @GlobalResCorp.lblChoose, Value = "0" }));
        //        }
        //        else
        //        {
        //            wilayahlist = new SelectList(db.tbl_Wilayah.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID2 && x.fld_ID == WilayahID && x.fld_Deleted == false).OrderBy(o => o.fld_WlyhName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_WlyhName }), "Value", "Text").ToList();
        //            wilayahlist.Insert(0, (new SelectListItem { Text = @GlobalResCorp.lblChoose, Value = "0" }));
        //            //ladanglist.Insert(0, (new SelectListItem { Text = @GlobalResCorp.lblChoose, Value = "0" }));
        //        }
        //    }

        //    return Json(wilayahlist);
        //}

    }
}