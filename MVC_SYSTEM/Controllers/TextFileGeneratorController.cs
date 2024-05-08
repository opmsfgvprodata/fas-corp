using Dapper;
using Itenso.TimePeriod;
using iTextSharp.text.pdf.parser;
using MVC_SYSTEM.App_LocalResources;
using MVC_SYSTEM.Attributes;
using MVC_SYSTEM.Class;
using MVC_SYSTEM.log;
using MVC_SYSTEM.Models;
using MVC_SYSTEM.ModelsCorporate;
using MVC_SYSTEM.ModelsDapper;
using MVC_SYSTEM.ModelsSP;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Web;
using System.Web.Http.Results;
using System.Web.Mvc;
using static iTextSharp.text.pdf.AcroFields;

namespace MVC_SYSTEM.Controllers
{
    [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3")]
    public class TextFileGeneratorController : Controller
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
        // GET: TextFileGenerator
        public ActionResult Index()
        {
            ViewBag.MaybankFileGen = "class = active";
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            List<SelectListItem> sublist = new List<SelectListItem>();
            ViewBag.MenuSubList = sublist;
            ViewBag.MenuList = new SelectList(dbC.tblMenuLists.Where(x => x.fld_Flag == "textfilegen" && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).OrderBy(o => o.fld_ID).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_Desc }), "Value", "Text").ToList();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(string MenuList)
        {
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            int menulist = int.Parse(MenuList);
            var action = dbC.tblMenuLists.Where(x => x.fld_ID == menulist && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).OrderBy(o => o.fld_ID).Select(s => s.fld_Val).FirstOrDefault();

            return RedirectToAction(action, "TextFileGenerator");
        }

        public ActionResult TaxCP39()
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

            CompCodeList.Insert(0, (new SelectListItem { Text = "All", Value = "0" }));
            ViewBag.CompCodeList = CompCodeList;
            return View();
        }

        public ViewResult _TaxCP39(string CompCodeList, int? MonthList, int? YearList, string print)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            string NamaSyarikat = "";
            string ClientId = "";

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            List<sp_TaxCP39_Result> taxCP39 = new List<sp_TaxCP39_Result>();

            ViewBag.MonthList = MonthList;
            ViewBag.YearList = YearList;
            var CompCodeCopy = "FASSB";
            if (CompCodeList != "0" && CompCodeList != null)
            {
                CompCodeCopy = CompCodeList;
            }
            var syarikat = dbC.tbl_Syarikat.Where(x => x.fld_NamaPndkSyarikat == CompCodeCopy).FirstOrDefault();
            ViewBag.NamaSyarikat = syarikat.fld_NamaSyarikat;
            ViewBag.NamaPendekSyarikat = syarikat.fld_NamaPndkSyarikat;
            ViewBag.NoSyarikat = syarikat.fld_NoSyarikat;
            ViewBag.CorpID = syarikat.fld_CorporateID;
            ViewBag.AccNo = syarikat.fld_AccountNo;
            ClientId = syarikat.fld_ClientBatchID;
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

            ViewBag.NegaraID = NegaraID;
            ViewBag.SyarikatID = SyarikatID;
            ViewBag.UserID = getuserid;
            ViewBag.UserName = User.Identity.Name;
            ViewBag.Date = DateTime.Now.ToShortDateString();
            ViewBag.Time = DateTime.Now.ToShortTimeString();
            ViewBag.Print = print;

            ViewBag.Description = "Region " + NamaSyarikat + " - CP 39 for " + MonthList + "/" + YearList;

            string constr = ConfigurationManager.ConnectionStrings["MVC_SYSTEM_HQ_CONN"].ConnectionString;
            var con = new SqlConnection(constr);
            try
            {
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("NegaraID", NegaraID);
                parameters.Add("SyarikatID", SyarikatID);
                parameters.Add("Month", MonthList);
                parameters.Add("Year", YearList);
                parameters.Add("CompCode", CompCodeList);
                con.Open();
                taxCP39 = SqlMapper.Query<sp_TaxCP39_Result>(con, "sp_TaxCP39", parameters).ToList();
                con.Close();
            }
            catch (Exception ex)
            {
                throw;
            }

            ViewBag.RecordNo = taxCP39.Count();

            if (taxCP39.Count() == 0)
            {
                ViewBag.Message = GlobalResCorp.msgNoRecord;
            }
            return View(taxCP39);
        }

        public JsonResult TaxCP39Detail(int Month, int Year, string CompCode)
        {
            string msg = "";
            string statusmsg = "";
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);

            string stringyear = "";
            string stringmonth = "";
            stringyear = Year.ToString();
            stringmonth = Month.ToString();
            stringmonth = (stringmonth.Length == 1 ? "0" + stringmonth : stringmonth);
            decimal? TotalMTDAmt = 0;
            int TotalMTDRec = 0;
            decimal? TotalCP38Amt = 0;
            int TotalCP38Rec = 0;
            int CountData = 0;

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            List<sp_TaxCP39_Result> taxCP39 = new List<sp_TaxCP39_Result>();
            var CompCodeCopy = "FASSB";
            if (CompCode != "0")
            {
                CompCodeCopy = CompCode;
            }
            var SyarikatDetail = dbC.tbl_Syarikat.Where(x => x.fld_NamaPndkSyarikat == CompCodeCopy).FirstOrDefault();
            string filename = "Tax CP39 (" + SyarikatDetail.fld_NamaPndkSyarikat.ToUpper() + ") " + "" + stringmonth + stringyear + ".txt";
            string constr = ConfigurationManager.ConnectionStrings["MVC_SYSTEM_HQ_CONN"].ConnectionString;
            var con = new SqlConnection(constr);
            try
            {
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("NegaraID", NegaraID);
                parameters.Add("SyarikatID", SyarikatID);
                parameters.Add("Month", Month);
                parameters.Add("Year", Year);
                parameters.Add("CompCode", CompCode);
                con.Open();
                taxCP39 = SqlMapper.Query<sp_TaxCP39_Result>(con, "sp_TaxCP39", parameters).ToList();
                con.Close();
            }
            catch (Exception ex)
            {
                throw;
            }

            if (taxCP39.Count() != 0)
            {
                TotalMTDAmt = taxCP39.Sum(s => s.fld_CarumanPekerja);
                TotalMTDRec = taxCP39.Count();
                TotalCP38Amt = taxCP39.Sum(s => s.fld_CP38Amount);
                TotalCP38Rec = taxCP39.Where(x => x.fld_CP38Amount > 0).Count();
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
            return Json(new { msg, statusmsg, file = filename, TotalMTDAmt, TotalMTDRec, TotalCP38Amt, TotalCP38Rec });
        }

        [HttpPost]
        public ActionResult DownloadCP39TextFile(int Month, int Year, string CompCode)
        {
            string msg = "";
            string statusmsg = "";
            string link = "";
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            GetGenerateFile getGenerateFile = new GetGenerateFile();
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            var CompCodeCopy = "FASSB";
            if (CompCode != "0")
            {
                CompCodeCopy = CompCode;
            }
            var SyarikatDetail = dbC.tbl_Syarikat.Where(x => x.fld_NamaPndkSyarikat == CompCodeCopy).FirstOrDefault();
            List<sp_TaxCP39_Result> taxCP39 = new List<sp_TaxCP39_Result>();
            string constr = ConfigurationManager.ConnectionStrings["MVC_SYSTEM_HQ_CONN"].ConnectionString;
            var con = new SqlConnection(constr);
            try
            {
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("NegaraID", NegaraID);
                parameters.Add("SyarikatID", SyarikatID);
                parameters.Add("Month", Month);
                parameters.Add("Year", Year);
                parameters.Add("CompCode", CompCode);
                con.Open();
                taxCP39 = SqlMapper.Query<sp_TaxCP39_Result>(con, "sp_TaxCP39", parameters).ToList();
                con.Close();
            }
            catch (Exception ex)
            {
                throw;
            }

            var TotalMTDAmt = taxCP39.Sum(s => s.fld_CarumanPekerja) * 100;
            var TotalMTDRec = taxCP39.Count();
            var TotalCP38Amt = taxCP39.Sum(s => s.fld_CP38Amount) * 100;
            var TotalCP38Rec = taxCP39.Where(x => x.fld_CP38Amount > 0).Count();
            var TotalMTDAmtStr = TotalMTDAmt.ToString("0");
            var TotalMTDRecStr = TotalMTDRec.ToString("0");
            var TotalCP38AmtStr = TotalCP38Amt.ToString("0");
            var TotalCP38RecStr = TotalCP38Rec.ToString("0");
            string fileContent = "";

            #region Header
            fileContent = TextFileContent("H", 1, " ", true);
            fileContent += TextFileContent(SyarikatDetail.fld_EmployerTaxNo, 10, "0", true);
            fileContent += TextFileContent(SyarikatDetail.fld_EmployerTaxNo, 10, "0", true);
            fileContent += TextFileContent(Year.ToString(), 4, "0", true);
            fileContent += TextFileContent(Month.ToString(), 2, "0", true);
            fileContent += TextFileContent(TotalMTDAmtStr, 10, "0", true);
            fileContent += TextFileContent(TotalMTDRecStr, 5, "0", true);
            fileContent += TextFileContent(TotalCP38AmtStr, 10, "0", true);
            fileContent += TextFileContent(TotalCP38RecStr, 5, "0", true);
            fileContent += Environment.NewLine;
            #endregion Header

            #region Body
            foreach (var item in taxCP39)
            {
                fileContent += TextFileContent("D", 1, " ", true);
                fileContent += TextFileContent(item.fld_TaxNo, 10, "0", true);
                fileContent += TextFileContent(item.fld_WifeCode, 1, " ", true);
                fileContent += TextFileContent(item.fld_Nama, 60, " ", false);
                fileContent += TextFileContent("", 12, " ", true);
                fileContent += TextFileContent(item.fld_Nokp, 12, " ", true);
                fileContent += TextFileContent(item.fld_PassportNo, 12, " ", false);
                fileContent += TextFileContent(item.fld_CountryCode, 2, " ", true);
                fileContent += TextFileContent((item.fld_CarumanPekerja * 100).ToString("0"), 8, "0", true);
                fileContent += TextFileContent((item.fld_CP38Amount * 100).ToString("0"), 8, "0", true);
                fileContent += TextFileContent("", 10, " ", false);
                if (taxCP39.IndexOf(item) != taxCP39.Count - 1)
                {
                    fileContent += Environment.NewLine;
                }
            }
            #endregion Body
            var filename = SyarikatDetail.fld_EmployerTaxNo + TextFileContent(Month.ToString(), 2, "0", true) + "_" + TextFileContent(Year.ToString(), 4, "0", true) + ".txt";
            var filePath = getGenerateFile.CreateTextFile(filename, fileContent, "CP39");

            link = Url.Action("Download", "TextFileGenerator", new { filePath, filename });

            msg = GlobalResCorp.msgGenerateSuccess;
            statusmsg = "success";

            return Json(new { msg, statusmsg, link });
        }

        public string TextFileContent(string data, int length, string emptyReplceData, bool isLeft)
        {
            string result = "";

            if (data.Length > length)
            {
                result = data.Substring(0, length);
            }
            else
            {
                if (isLeft)
                {
                    result = data.PadLeft(length, Convert.ToChar(emptyReplceData));
                }
                else
                {
                    result = data.PadRight(length, Convert.ToChar(emptyReplceData));
                }
            }

            return result;
        }

        // ...

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
    }
}