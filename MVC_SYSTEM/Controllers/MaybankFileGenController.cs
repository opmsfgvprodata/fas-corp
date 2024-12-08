using MVC_SYSTEM.Attributes;
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
using Dapper;
using MVC_SYSTEM.ModelsDapper;
using System.Configuration;
using System.Data.SqlClient;
using static System.Net.Mime.MediaTypeNames;
using System.Data;
using System.Drawing;
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
            ViewBag.MenuList = new SelectList(dbC.tblMenuLists.Where(x => x.fld_Flag == "m2e" && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).OrderBy(o => o.fld_ID).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_Desc }), "Value", "Text").ToList();
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
                if (MenuSubList.Contains("|"))
                {
                    string[] split = MenuSubList.Split('|');
                    return RedirectToAction(split[1], split[0]);
                }
                else
                    return RedirectToAction(MenuSubList, "MaybankFileGen");
            }
            else
            {
                int menulist = int.Parse(MenuList);
                var action = dbC.tblMenuLists.Where(x => x.fld_ID == menulist && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).OrderBy(o => o.fld_ID).Select(s => s.fld_Val).FirstOrDefault();
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
        public ActionResult DownloadTextTax(int Month, int Year, string CompCode, string filter, string[] WorkerId, DateTime PaymentDate)
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
                List<ModelsCorporate.tblOptionConfigsWeb> CountryCodeDetail = new List<ModelsCorporate.tblOptionConfigsWeb>();

                List<sp_TaxCP39_Result> maybankrcmsList = new List<sp_TaxCP39_Result>();
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
                    maybankrcmsList = SqlMapper.Query<sp_TaxCP39_Result>(con, "sp_TaxCP39", parameters).ToList();

                    if (WorkerId == null)
                        WorkerId = new string[] { "0" };

                    if (WorkerId.Contains("0"))
                    {
                        maybankrcmsList = SqlMapper.Query<sp_TaxCP39_Result>(con, "sp_TaxCP39", parameters).ToList();
                    }
                    else
                    {
                        maybankrcmsList = SqlMapper.Query<sp_TaxCP39_Result>(con, "sp_TaxCP39", parameters).Where(x => WorkerId.Contains(x.fld_Nopkj)).ToList();
                    }
                    con.Close();
                }
                catch (Exception ex)
                {
                    throw;
                }


                //if (WorkerId == null)
                //    WorkerId = new string[] { "0" };

                //if (WorkerId.Contains("0"))
                //{
                //    maybankrcmsList = dbSP.sp_TaxCP39Prev(NegaraID.Value, SyarikatID.Value, Year, Month, getuserid, CompCode).ToList();
                //}
                //else
                //{
                //    maybankrcmsList = dbSP.sp_TaxCP39Prev(NegaraID.Value, SyarikatID.Value, Year, Month, getuserid, CompCode).Where(x => WorkerId.Contains(x.fld_NoPkj)).ToList();
                //}

                var SyarikatDetail = db.tbl_Syarikat.Where(x => x.fld_NamaPndkSyarikat == CompCode).FirstOrDefault();
                //CountryCodeDetail = dbC.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "krytnlistlhdn").ToList();

                filePath = GetGenerateFile.GenerateFileMaybankTax(maybankrcmsList, SyarikatDetail, stringmonth, stringyear, NegaraID, SyarikatID, CompCode, filter, PaymentDate, out filename);

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
        public ActionResult DownloadTextOthers(int Month, int Year, string CompCode, string filter, string[] WorkerId, DateTime PaymentDate, string Incentive)
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
                List<sp_MaybankRcmsOthers_Result> maybankrcmsList = new List<sp_MaybankRcmsOthers_Result>();

                if (WorkerId == null)
                    WorkerId = new string[] { "0" };

                if (WorkerId.Contains("0"))
                {
                    maybankrcmsList = dbSP.sp_MaybankRcmsOthers(NegaraID.Value, SyarikatID.Value, Year, Month, getuserid, CompCode, Incentive).ToList();
                }
                else
                {
                    maybankrcmsList = dbSP.sp_MaybankRcmsOthers(NegaraID.Value, SyarikatID.Value, Year, Month, getuserid, CompCode, Incentive).Where(x => WorkerId.Contains(x.fld_Nopkj)).ToList();
                }

                var SyarikatDetail = db.tbl_Syarikat.Where(x => x.fld_NamaPndkSyarikat == CompCode).FirstOrDefault();

                filePath = GetGenerateFile.GenerateFileMaybankOthers(maybankrcmsList, SyarikatDetail, stringmonth, stringyear, NegaraID, SyarikatID, CompCode, filter, PaymentDate, Incentive, out filename);

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
            return Json(new { msg, statusmsg, file = filename, salary = TotalGaji, totaldata = CountData, clientid = ClientIDText });
        }

        public JsonResult CheckGenDataDetailTax(int Month, int Year, string CompCode, /*int Wilayah,*/ string[] WorkerId)
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

            List<sp_TaxCP39_Result> maybankrcmsList = new List<sp_TaxCP39_Result>();

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
                //maybankrcmsList = SqlMapper.Query<sp_TaxCP39_Result>(con, "sp_TaxCP39", parameters).ToList();
                if (WorkerId == null)
                    WorkerId = new string[] { "0" };

                if (WorkerId.Contains("0"))
                {
                    maybankrcmsList = SqlMapper.Query<sp_TaxCP39_Result>(con, "sp_TaxCP39", parameters).ToList();
                }
                else
                {
                    maybankrcmsList = SqlMapper.Query<sp_TaxCP39_Result>(con, "sp_TaxCP39", parameters).Where(x => WorkerId.Contains(x.fld_Nopkj)).ToList();
                }

                con.Close();
            }
            catch (Exception ex)
            {
                throw;
            }

            //if (WorkerId == null)
            //    WorkerId = new string[] { "0" };

            //if (WorkerId.Contains("0"))
            //{
            //    maybankrcmsList = dbSP.sp_TaxCP39(NegaraID.Value, SyarikatID.Value, Year, Month, getuserid, CompCode).ToList();
            //}
            //else
            //{
            //    maybankrcmsList = dbSP.sp_TaxCP39(NegaraID.Value, SyarikatID.Value, Year, Month, getuserid, CompCode).Where(x => WorkerId.Contains(x.fld_Nopkj)).ToList();
            //}

            var SyarikatDetail = dbC.tbl_Syarikat.Where(x => x.fld_NamaPndkSyarikat == CompCode).FirstOrDefault();
            string filename = "M2E TAX(" + SyarikatDetail.fld_NamaPndkSyarikat.ToUpper() + ") " + "" + stringmonth + stringyear + ".txt";

            if (ClientID == null || ClientID == " ")
            {
                if (CompCode == "FASSB")
                {
                    ClientIDText = "FAS" + stringmonth + stringyear;
                }

                if (CompCode == "RNDSB")
                {
                    ClientIDText = "RND" + stringmonth + stringyear;
                }
            }
            else
            {
                ClientIDText = ClientID;
            }

            if (maybankrcmsList.Count() != 0)
            {
                TotalGaji = maybankrcmsList.Sum(s => s.fld_CarumanPekerja);
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
            return Json(new { msg, statusmsg, file = filename, salary = TotalGaji, totaldata = CountData, clientid = ClientIDText });
        }

        public JsonResult CheckGenDataDetailOthers(int Month, int Year, string CompCode, string Incentive, string[] WorkerId)
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

            List<sp_MaybankRcmsOthers_Result> maybankrcmsList = new List<sp_MaybankRcmsOthers_Result>();
            if (WorkerId == null)
                WorkerId = new string[] { "0" };

            if (WorkerId.Contains("0"))
            {
                maybankrcmsList = dbSP.sp_MaybankRcmsOthers(NegaraID.Value, SyarikatID.Value, Year, Month, getuserid, CompCode, Incentive).ToList();
            }
            else
            {
                maybankrcmsList = dbSP.sp_MaybankRcmsOthers(NegaraID.Value, SyarikatID.Value, Year, Month, getuserid, CompCode, Incentive).Where(x => WorkerId.Contains(x.fld_Nopkj)).ToList();
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
            return Json(new { msg, statusmsg, file = filename, salary = TotalGaji, totaldata = CountData, clientid = ClientIDText });
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

        public ActionResult RcmsGenTax()
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

        public ActionResult RcmsGenOthers()
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

            List<SelectListItem> IncentiveList = new List<SelectListItem>();
            IncentiveList = new SelectList(dbC.tbl_JenisInsentif.Where(x => x.fld_InclSecondPayslip == true && x.fld_JenisInsentif == "P" && x.fld_Deleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).OrderBy(o => o.fld_KodInsentif).Select(s => new SelectListItem { Value = s.fld_KodInsentif, Text = s.fld_Keterangan }), "Value", "Text").ToList();
            IncentiveList.Insert(0, (new SelectListItem { Text = "Please Select", Value = "0" }));
            ViewBag.IncentiveList = IncentiveList;

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

        public ViewResult _rcmstax(string CompCodeList, int? MonthList, int? YearList, string print, string filter)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            //string WilayahName = "";
            string NamaSyarikat = "";
            string ClientId = "";
            //string LdgCode = "";

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            List<sp_TaxCP39_Result> maybankrcmsList = new List<sp_TaxCP39_Result>();

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
                    ViewBag.clientid = "FAS" + MonthList + YearList;
                }

                if (CompCodeList == "RNDSB")
                {
                    ViewBag.clientid = "RND" + MonthList + YearList;
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
            ViewBag.NegaraID = NegaraID;
            ViewBag.SyarikatID = SyarikatID;
            ViewBag.UserID = getuserid;
            ViewBag.UserName = User.Identity.Name;
            ViewBag.Date = DateTime.Now.ToShortDateString();
            ViewBag.Time = DateTime.Now.ToShortTimeString();
            ViewBag.Print = print;
            ViewBag.Description = "Region " + NamaSyarikat + " - Salary payment for " + MonthList + "/" + YearList;
            if (MonthList == null || YearList == null || CompCodeList == "0")
            {
                ViewBag.Message = "Please select month, year, company and payment date";
                return View(maybankrcmsList);
            }
            else
            {
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
                    maybankrcmsList = SqlMapper.Query<sp_TaxCP39_Result>(con, "sp_TaxCP39", parameters).ToList();
                    con.Close();
                }
                catch (Exception ex)
                {
                    throw;
                }
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

        public ViewResult _rcmsOthers(string CompCodeList, int? MonthList, int? YearList, string IncentiveList, string print, string filter)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            //string WilayahName = "";
            string NamaSyarikat = "";
            string ClientId = "";
            //string LdgCode = "";

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            List<sp_MaybankRcmsOthers_Result> maybankrcmsList = new List<sp_MaybankRcmsOthers_Result>();

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
            ViewBag.Description = "Region " + NamaSyarikat + " - Payment for " + MonthList + "/" + YearList;
            if (MonthList == null || YearList == null || CompCodeList == "0" || IncentiveList == "0")
            {
                ViewBag.Message = "Please select month, year, company, incentive type and payment date";
                return View(maybankrcmsList);
            }
            else
            {
                dbSP.SetCommandTimeout(2400);
                maybankrcmsList = dbSP.sp_MaybankRcmsOthers(NegaraID, SyarikatID, YearList, MonthList, getuserid, CompCodeList, IncentiveList).ToList();

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

        public ActionResult RcmsGenIndividuOthers()
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

            List<SelectListItem> IncentiveList = new List<SelectListItem>();
            IncentiveList = new SelectList(dbC.tbl_JenisInsentif.Where(x => x.fld_InclSecondPayslip == true && x.fld_JenisInsentif == "P" && x.fld_Deleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).OrderBy(o => o.fld_KodInsentif).Select(s => new SelectListItem { Value = s.fld_KodInsentif, Text = s.fld_Keterangan }), "Value", "Text").ToList();
            IncentiveList.Insert(0, (new SelectListItem { Text = "Please Select", Value = "0" }));
            ViewBag.IncentiveList = IncentiveList;

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

        public ViewResult _rcmsIndividuOthers(string CompCodeList, int? MonthList, int? YearList, string IncentiveList, string print, string filter, string[] WorkerId)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            //string WilayahName = "";
            string NamaSyarikat = "";
            string ClientId = "";
            //string LdgCode = "";

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            List<sp_MaybankRcmsOthers_Result> maybankrcmsList = new List<sp_MaybankRcmsOthers_Result>();

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
            if (MonthList == null || YearList == null || CompCodeList == "0" || IncentiveList == "0")
            {
                ViewBag.Message = "Please select month, year, company, incentive type and payment date";
                return View(maybankrcmsList);
            }
            else
            {
                if (WorkerId.Contains("0"))
                {
                    dbSP.SetCommandTimeout(2400);
                    maybankrcmsList = dbSP.sp_MaybankRcmsOthers(NegaraID, SyarikatID, YearList, MonthList, getuserid, CompCodeList, IncentiveList).ToList();
                }
                else
                {
                    dbSP.SetCommandTimeout(2400);
                    maybankrcmsList = dbSP.sp_MaybankRcmsOthers(NegaraID, SyarikatID, YearList, MonthList, getuserid, CompCodeList, IncentiveList).Where(x => WorkerId.Contains(x.fld_Nopkj)).ToList();
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

        public JsonResult GetWorkerOthers(string CompCode, int Year, int Month, string Incentive)
        {
            List<SelectListItem> workerList = new List<SelectListItem>();
            var userDetail = getidentity.GetUserDetail(User.Identity.Name);
            var getuserid = userDetail.fldUserID;
            var maybankrcmsList = dbSP.sp_MaybankRcmsOthers(userDetail.fldNegaraID, userDetail.fldSyarikatID, Year, Month, getuserid, CompCode, Incentive).Select(s => new { s.fld_Nopkj, s.fld_Nama }).OrderBy(o => o.fld_Nama).ToList();

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

        public ActionResult SSCOnlinePaymentOthers()
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

            List<SelectListItem> IncentiveList = new List<SelectListItem>();
            IncentiveList = new SelectList(dbC.tbl_JenisInsentif.Where(x => x.fld_InclSecondPayslip == true && x.fld_JenisInsentif == "P" && x.fld_Deleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).OrderBy(o => o.fld_KodInsentif).Select(s => new SelectListItem { Value = s.fld_KodInsentif, Text = s.fld_Keterangan }), "Value", "Text").ToList();
            IncentiveList.Insert(0, (new SelectListItem { Text = "Please Select", Value = "0" }));
            ViewBag.IncentiveList = IncentiveList;

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

        public ViewResult _SSCOnlinePaymentOthers(string CompCodeList, int? MonthList, int? YearList, string IncentiveList)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            //string WilayahName = "";
            string NamaPendekSyarikat = "";
            //string LdgCode = "";

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            List<sp_MaybankRcmsOnlinePaymentRptOthers_Result> ssconlinepayment = new List<sp_MaybankRcmsOnlinePaymentRptOthers_Result>();

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
            if (MonthList == null || YearList == null || CompCodeList == "0" || IncentiveList == "0")
            {
                ViewBag.Message = "Please select month, year, company and incentive type";
                return View(ssconlinepayment);
            }
            else
            {
                dbSP.SetCommandTimeout(2400);
                ssconlinepayment = dbSP.sp_MaybankRcmsOnlinePaymentRptOthers(NegaraID, SyarikatID, YearList, MonthList, getuserid, CompCodeList, IncentiveList).ToList();

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

        public ActionResult RcmsZAP64Others()
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

            List<SelectListItem> IncentiveList = new List<SelectListItem>();
            IncentiveList = new SelectList(dbC.tbl_JenisInsentif.Where(x => x.fld_InclSecondPayslip == true && x.fld_JenisInsentif == "P" && x.fld_Deleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).OrderBy(o => o.fld_KodInsentif).Select(s => new SelectListItem { Value = s.fld_KodInsentif, Text = s.fld_Keterangan }), "Value", "Text").ToList();
            IncentiveList.Insert(0, (new SelectListItem { Text = "Please Select", Value = "0" }));
            ViewBag.IncentiveList = IncentiveList;

            ViewBag.UserID = getuserid;
            //dbC.Dispose();
            return View();
        }

        public ActionResult RcmsZAP64Tax()
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

            if (YearList == null && MonthList == null)
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

        public ViewResult _RcmsZAP64Others(string CompCodeList, int? MonthList, int? YearList, string print, string PaymentDate, string IncentiveList)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            //string WilayahName = "";
            string NamaSyarikat = "";
            //string LdgCode = "";

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            List<sp_MaybankRcmsZAP64Others_Result> maybankrcmsZAP64 = new List<sp_MaybankRcmsZAP64Others_Result>();


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

            if (YearList == null && MonthList == null)
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
            if (MonthList == null || YearList == null || CompCodeList == "0" || IncentiveList == "0")
            {
                ViewBag.Message = "Please select month, year, company, incentive type and payment date";
                return View(maybankrcmsZAP64);
            }
            else
            {
                dbSP.SetCommandTimeout(2400);
                maybankrcmsZAP64 = dbSP.sp_MaybankRcmsZAP64Others(NegaraID, SyarikatID, YearList, MonthList, getuserid, CompCodeList, IncentiveList).ToList();

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

        public ViewResult _RcmsZAP64Tax(string CompCodeList, int? MonthList, int? YearList, string print, string PaymentDate)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            //string WilayahName = "";
            string NamaSyarikat = "";
            //string LdgCode = "";

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            List<sp_MaybankRcmsZAP64Tax_Result> maybankrcmsZAP64 = new List<sp_MaybankRcmsZAP64Tax_Result>();


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

            if (YearList == null && MonthList == null)
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
                //dbSP.SetCommandTimeout(2400);
                //maybankrcmsZAP64 = dbSP.sp_MaybankRcmsZAP64(NegaraID, SyarikatID, YearList, MonthList, getuserid, CompCodeList).ToList();

                string constr = ConfigurationManager.ConnectionStrings["MVC_SYSTEM_HQ_CONN"].ConnectionString;
                var con = new SqlConnection(constr);
                try
                {
                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("NegaraID", NegaraID);
                    parameters.Add("SyarikatID", SyarikatID);
                    parameters.Add("Year", YearList);
                    parameters.Add("Month", MonthList);
                    parameters.Add("UserID", getuserid);
                    parameters.Add("CompCode", CompCodeList);
                    con.Open();
                    maybankrcmsZAP64 = SqlMapper.Query<sp_MaybankRcmsZAP64Tax_Result>(con, "sp_MaybankRcmsZAP64Tax", parameters).ToList();
                    con.Close();
                }
                catch (Exception ex)
                {
                    throw;
                }


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
            fileContent = GetGenerateFile.TextFileContent("H", 1, " ", true);
            fileContent += GetGenerateFile.TextFileContent(SyarikatDetail.fld_EmployerTaxNo, 10, "0", true);
            fileContent += GetGenerateFile.TextFileContent(SyarikatDetail.fld_EmployerTaxNo, 10, "0", true);
            fileContent += GetGenerateFile.TextFileContent(Year.ToString(), 4, "0", true);
            fileContent += GetGenerateFile.TextFileContent(Month.ToString(), 2, "0", true);
            fileContent += GetGenerateFile.TextFileContent(TotalMTDAmtStr, 10, "0", true);
            fileContent += GetGenerateFile.TextFileContent(TotalMTDRecStr, 5, "0", true);
            fileContent += GetGenerateFile.TextFileContent(TotalCP38AmtStr, 10, "0", true);
            fileContent += GetGenerateFile.TextFileContent(TotalCP38RecStr, 5, "0", true);
            fileContent += Environment.NewLine;
            #endregion Header

            #region Body
            foreach (var item in taxCP39)
            {
                fileContent += GetGenerateFile.TextFileContent("D", 1, " ", true);
                fileContent += GetGenerateFile.TextFileContent(item.fld_TaxNo, 10, "0", true);
                fileContent += GetGenerateFile.TextFileContent(item.fld_WifeCode, 1, " ", true);
                fileContent += GetGenerateFile.TextFileContent(item.fld_Nama, 60, " ", false);
                fileContent += GetGenerateFile.TextFileContent("", 12, " ", true);
                fileContent += GetGenerateFile.TextFileContent(item.fld_Nokp, 12, " ", true);
                fileContent += GetGenerateFile.TextFileContent(item.fld_PassportNo, 12, " ", false);
                fileContent += GetGenerateFile.TextFileContent(item.fld_CountryCode, 2, " ", true);
                fileContent += GetGenerateFile.TextFileContent((item.fld_CarumanPekerja * 100).ToString("0"), 8, "0", true);
                fileContent += GetGenerateFile.TextFileContent((item.fld_CP38Amount * 100).ToString("0"), 8, "0", true);
                fileContent += GetGenerateFile.TextFileContent("", 10, " ", false);
                if (taxCP39.IndexOf(item) != taxCP39.Count - 1)
                {
                    fileContent += Environment.NewLine;
                }
            }
            #endregion Body
            var filename = SyarikatDetail.fld_EmployerTaxNo + GetGenerateFile.TextFileContent(Month.ToString(), 2, "0", true) + "_" + GetGenerateFile.TextFileContent(Year.ToString(), 4, "0", true) + ".txt";
            var filePath = getGenerateFile.CreateTextFile(filename, fileContent, "CP39");

            link = Url.Action("Download", "MaybankFileGen", new { filePath, filename });

            msg = GlobalResCorp.msgGenerateSuccess;
            statusmsg = "success";

            return Json(new { msg, statusmsg, link });
        }

        public ActionResult TaxCP39Form()
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

            ViewBag.CompCodeList = CompCodeList;
            return View();
        }

        public FileStreamResult TaxCP39FormPdf(int Month, int Year, string CompCode)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string NamaSyarikat = "";

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            List<sp_TaxCP39_Result> taxCP39 = new List<sp_TaxCP39_Result>();

            var monthName = ((Constans.Month)Month).ToString().ToUpper();
            var SyarikatDetail = dbC.tbl_Syarikat.Where(x => x.fld_NamaPndkSyarikat == CompCode).FirstOrDefault(); 

            if (CompCode != null)
            {
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
            }

            if (taxCP39.Count() == 0)
            {
                Document pdfDoc = new Document(PageSize.A4, 10, 10, 10, 5);
                MemoryStream ms = new MemoryStream();
                MemoryStream output = new MemoryStream();
                PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, ms);
                Chunk chunk = new Chunk();
                Paragraph para = new Paragraph();
                pdfDoc.Open();
                PdfPTable table = new PdfPTable(1);
                table.WidthPercentage = 100;
                PdfPCell cell = new PdfPCell();
                chunk = new Chunk("No Data Found", FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK));
                cell = new PdfPCell(new Phrase(chunk));
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Border = 0;
                table.AddCell(cell);
                pdfDoc.Add(table);
                pdfWriter.CloseStream = false;
                pdfDoc.Close();

                ms.Close();

                byte[] file = ms.ToArray();
                output.Write(file, 0, file.Length);
                output.Position = 0;
                return new FileStreamResult(output, "application/pdf");
            }
            else
            {
                MemoryStream output = new MemoryStream();

                string cp39Form = GetConfig.PdfPathFile("CP39 FORM-1.pdf");

                // open the reader
                PdfReader reader = new PdfReader(cp39Form);
                iTextSharp.text.Rectangle size = reader.GetPageSizeWithRotation(1);
                Document document = new Document(size);

                // open the writer
                MemoryStream ms = new MemoryStream();
                //FileStream fs = new FileStream(newFile, FileMode.Create, FileAccess.Write);
                PdfWriter writer = PdfWriter.GetInstance(document, ms);
                document.Open();
                PdfContentByte cb = writer.DirectContent;

                // front page content
                BaseFont bf = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, BaseFont.EMBEDDED);
                cb.SetColorFill(BaseColor.BLACK);
                cb.SetFontAndSize(bf, 8);

                string text = "";

                cb.BeginText();
                text = monthName; //Month name
                cb.ShowTextAligned(0, text, 358, 506, 0);
                cb.EndText();

                cb.BeginText();
                text = Year.ToString(); //Year
                cb.ShowTextAligned(0, text, 450, 506, 0);
                cb.EndText();

                var noEmployerTax = SyarikatDetail.fld_EmployerTaxNo;
                char[] noEmployerTaxArr = noEmployerTax.ToCharArray();
                int arrCountNoEmployerTaxArr = noEmployerTaxArr.Count();
                float noEmployerXPosition = 0;

                for (int i = 0; i <= arrCountNoEmployerTaxArr; i++)
                {
                    try
                    {
                        text = noEmployerTaxArr[i].ToString();
                    }
                    catch (Exception ex)
                    {
                        text = "";
                    }
                    if (text != "")
                    {
                        switch (i)
                        {
                            case 0:
                                noEmployerXPosition = 100;
                                break;
                            case 1:
                                noEmployerXPosition = 113;
                                break;
                            case 2:
                                noEmployerXPosition = 126;
                                break;
                            case 3:
                                noEmployerXPosition = 140;
                                break;
                            case 4:
                                noEmployerXPosition = 152;
                                break;
                            case 5:
                                noEmployerXPosition = 167;
                                break;
                            case 6:
                                noEmployerXPosition = 180;
                                break;
                            case 7:
                                noEmployerXPosition = 192;
                                break;
                            case 8:
                                noEmployerXPosition = 205;
                                break;
                            case 9:
                                noEmployerXPosition = 230;
                                break;
                            case 10:
                                noEmployerXPosition = 243;
                                break;
                        }
                        cb.BeginText();
                        cb.ShowTextAligned(0, text, noEmployerXPosition, 451, 0);
                        cb.EndText();
                    }
                }

                var TotalMTDAmt = taxCP39.Sum(s => s.fld_CarumanPekerja);
                var TotalMTDRec = taxCP39.Count();
                var TotalCP38Amt = taxCP39.Sum(s => s.fld_CP38Amount);
                var TotalCP38Rec = taxCP39.Where(x => x.fld_CP38Amount > 0).Count();

                cb.BeginText();
                text = "RM   " + TotalMTDAmt.ToString("N"); //MTD Amt
                cb.ShowTextAligned(2, text, 413, 450, 0);
                cb.EndText();

                cb.BeginText();
                text = "RM   " + TotalCP38Amt.ToString("N"); //CP38
                cb.ShowTextAligned(2, text, 503, 450, 0);
                cb.EndText();

                cb.BeginText();
                text = TotalMTDRec.ToString(); //MTD Amt
                cb.ShowTextAligned(1, text, 373, 432, 0);
                cb.EndText();

                cb.BeginText();
                text = TotalCP38Rec.ToString(); //CP38
                cb.ShowTextAligned(1, text, 466, 432, 0);
                cb.EndText();

                var totalAmount = TotalMTDAmt + TotalCP38Amt;

                cb.BeginText();
                text = "RM   " + totalAmount.ToString("N"); //Total Amt
                cb.ShowTextAligned(1, text, 446, 409, 0);
                cb.EndText();

                cb.BeginText();
                text = SyarikatDetail.fld_NamaSyarikat;
                cb.ShowTextAligned(0, text, 98, 390, 0);
                cb.EndText();

                cb.BeginText();
                text = DateTime.Now.ToString("dd.MM.yyyy"); //Total Amt
                cb.ShowTextAligned(1, text, 446, 337, 0);
                cb.EndText();

                // front page content

                PdfImportedPage page = writer.GetImportedPage(reader, 1);
                cb.AddTemplate(page, 0, 0);

                document.Close();
                writer.Close();
                reader.Close();
                ms.Close();
                byte[] file = ms.ToArray();

                cp39Form = GetConfig.PdfPathFile("CP39 FORM-2.pdf");

                // open the reader

                size = new iTextSharp.text.Rectangle(792, 612);
                document = new Document(size);

                // open the writer
                ms = new MemoryStream();
                //FileStream fs = new FileStream(newFile, FileMode.Create, FileAccess.Write);
                writer = PdfWriter.GetInstance(document, ms);
                document.Open();
                cb = writer.DirectContent;

                // front page content


                var taxCP39Arr = taxCP39.ToArray();
                var totalPages = Convert.ToInt32(taxCP39Arr.Count() / 24);
                var getModulus = taxCP39Arr.Count() % 24;
                if (getModulus > 0)
                {
                    totalPages += 1;
                }
                var dataIndex = 0;

                for (int i = 0; i < totalPages; i++)
                {
                    document.NewPage();
                    cb = writer.DirectContent;

                    bf = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, BaseFont.EMBEDDED);
                    cb.SetColorFill(BaseColor.BLACK);
                    cb.SetFontAndSize(bf, 8);

                    PdfPTable mainTable = new PdfPTable(5);
                    Chunk chunk = new Chunk();
                    mainTable.WidthPercentage = 100;
                    float[] widths = new float[] { 0.5f, 0.6f, 1.5f, 1, 0.1f };
                    mainTable.SetWidths(widths);

                    PdfPCell mainCell = new PdfPCell();
                    chunk = new Chunk("No. Rujukan Majikan E", FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK));
                    mainCell = new PdfPCell(new Phrase(chunk));
                    mainCell.HorizontalAlignment = Element.ALIGN_LEFT;
                    mainCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    mainCell.Border = 0;
                    mainTable.AddCell(mainCell);

                    PdfPTable table = new PdfPTable(12);
                    chunk = new Chunk();
                    //table.WidthPercentage = 5;
                    widths = new float[] { 0.1f, 0.1f, 0.1f, 0.1f, 0.1f, 0.1f, 0.1f, 0.1f, 0.1f, 0.1f, 0.1f, 0.1f };
                    table.SetWidths(widths);

                    PdfPCell cell = new PdfPCell();

                    for (int y = 0; y <= arrCountNoEmployerTaxArr; y++)
                    {
                        try
                        {
                            text = noEmployerTaxArr[y].ToString();
                        }
                        catch (Exception ex)
                        {
                            text = "";
                        }
                        chunk = new Chunk(text, FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK));
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Border = iTextSharp.text.Rectangle.BOTTOM_BORDER | iTextSharp.text.Rectangle.LEFT_BORDER | iTextSharp.text.Rectangle.RIGHT_BORDER | iTextSharp.text.Rectangle.TOP_BORDER;
                        table.AddCell(cell);

                        if (y == 8)
                        {
                            chunk = new Chunk("-", FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK));
                            cell = new PdfPCell(new Phrase(chunk));
                            cell.HorizontalAlignment = Element.ALIGN_CENTER;
                            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                            cell.Border = 0;
                            table.AddCell(cell);
                        }
                    }
                    mainCell = new PdfPCell(table);
                    mainCell.HorizontalAlignment = Element.ALIGN_LEFT;
                    mainCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    mainCell.Border = 0;
                    mainTable.AddCell(mainCell);

                    chunk = new Chunk(" ", FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK));
                    mainCell = new PdfPCell(new Phrase(chunk));
                    mainCell.HorizontalAlignment = Element.ALIGN_LEFT;
                    mainCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    mainCell.Border = 0;
                    mainTable.AddCell(mainCell);

                    chunk = new Chunk("Muka Surat", FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK));
                    mainCell = new PdfPCell(new Phrase(chunk));
                    mainCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    mainCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    mainCell.Border = 0;
                    mainTable.AddCell(mainCell);

                    text = (i + 1).ToString();
                    text = GetGenerateFile.TextFileContent(text, 2, "0", true);
                    char[] pageNoArr = text.ToCharArray();

                    table = new PdfPTable(2);
                    chunk = new Chunk();
                    widths = new float[] { 0.1f, 0.1f };
                    table.SetWidths(widths);

                    chunk = new Chunk(pageNoArr[0].ToString(), FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK));
                    cell = new PdfPCell(new Phrase(chunk));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.Border = iTextSharp.text.Rectangle.BOTTOM_BORDER | iTextSharp.text.Rectangle.LEFT_BORDER | iTextSharp.text.Rectangle.RIGHT_BORDER | iTextSharp.text.Rectangle.TOP_BORDER;
                    table.AddCell(cell);

                    chunk = new Chunk(pageNoArr[1].ToString(), FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK));
                    cell = new PdfPCell(new Phrase(chunk));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.Border = iTextSharp.text.Rectangle.BOTTOM_BORDER | iTextSharp.text.Rectangle.LEFT_BORDER | iTextSharp.text.Rectangle.RIGHT_BORDER | iTextSharp.text.Rectangle.TOP_BORDER;
                    table.AddCell(cell);

                    mainCell = new PdfPCell(table);
                    mainCell.HorizontalAlignment = Element.ALIGN_LEFT;
                    mainCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    mainCell.Colspan = 5;
                    mainCell.Border = 0;
                    mainTable.AddCell(mainCell);

                    table = new PdfPTable(10);
                    table.SpacingBefore = 10;
                    chunk = new Chunk();
                    table.WidthPercentage = 106;

                    widths = new float[] { 0.5f, 1.5f, 3, 1, 1, 1, 1, 1, 1, 1 };

                    table.SetWidths(widths);

                    cell = new PdfPCell();
                    chunk = new Chunk("BIL.", FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK));
                    cell = new PdfPCell(new Phrase(chunk));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.Rowspan = 2;
                    cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    cell.Border = iTextSharp.text.Rectangle.BOTTOM_BORDER | iTextSharp.text.Rectangle.LEFT_BORDER | iTextSharp.text.Rectangle.RIGHT_BORDER | iTextSharp.text.Rectangle.TOP_BORDER;
                    table.AddCell(cell);

                    cell = new PdfPCell();
                    chunk = new Chunk("NO. RUJUKAN CUKAI \r\nPENDAPATAN", FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK));
                    cell = new PdfPCell(new Phrase(chunk));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.Rowspan = 2;
                    cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    cell.Border = iTextSharp.text.Rectangle.BOTTOM_BORDER | iTextSharp.text.Rectangle.LEFT_BORDER | iTextSharp.text.Rectangle.RIGHT_BORDER | iTextSharp.text.Rectangle.TOP_BORDER;
                    table.AddCell(cell);

                    cell = new PdfPCell();
                    chunk = new Chunk("NAMA PENUH PEKERJA \r\n(SEPERTI DI KAD PENGENALAN ATAU PASPORT)", FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK));
                    cell = new PdfPCell(new Phrase(chunk));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.Rowspan = 2;
                    cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    cell.Border = iTextSharp.text.Rectangle.BOTTOM_BORDER | iTextSharp.text.Rectangle.LEFT_BORDER | iTextSharp.text.Rectangle.RIGHT_BORDER | iTextSharp.text.Rectangle.TOP_BORDER;
                    table.AddCell(cell);

                    cell = new PdfPCell();
                    chunk = new Chunk("NO. K/P LAMA", FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK));
                    cell = new PdfPCell(new Phrase(chunk));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.Rowspan = 2;
                    cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    cell.Border = iTextSharp.text.Rectangle.BOTTOM_BORDER | iTextSharp.text.Rectangle.LEFT_BORDER | iTextSharp.text.Rectangle.RIGHT_BORDER | iTextSharp.text.Rectangle.TOP_BORDER;
                    table.AddCell(cell);

                    cell = new PdfPCell();
                    chunk = new Chunk("NO. K/P BARU", FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK));
                    cell = new PdfPCell(new Phrase(chunk));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.Rowspan = 2;
                    cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    cell.Border = iTextSharp.text.Rectangle.BOTTOM_BORDER | iTextSharp.text.Rectangle.LEFT_BORDER | iTextSharp.text.Rectangle.RIGHT_BORDER | iTextSharp.text.Rectangle.TOP_BORDER;
                    table.AddCell(cell);

                    cell = new PdfPCell();
                    chunk = new Chunk("NO. \r\nPEKERJA", FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK));
                    cell = new PdfPCell(new Phrase(chunk));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.Rowspan = 2;
                    cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    cell.Border = iTextSharp.text.Rectangle.BOTTOM_BORDER | iTextSharp.text.Rectangle.LEFT_BORDER | iTextSharp.text.Rectangle.RIGHT_BORDER | iTextSharp.text.Rectangle.TOP_BORDER;
                    table.AddCell(cell);

                    cell = new PdfPCell();
                    chunk = new Chunk("BAGI PEKERJA ASING", FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK));
                    cell = new PdfPCell(new Phrase(chunk));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.Colspan = 2;
                    cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    cell.Border = iTextSharp.text.Rectangle.BOTTOM_BORDER | iTextSharp.text.Rectangle.LEFT_BORDER | iTextSharp.text.Rectangle.RIGHT_BORDER | iTextSharp.text.Rectangle.TOP_BORDER;
                    table.AddCell(cell);

                    cell = new PdfPCell();
                    chunk = new Chunk("JUMLAH POTONGAN CUKAI", FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK));
                    cell = new PdfPCell(new Phrase(chunk));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.Colspan = 2;
                    cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    cell.Border = iTextSharp.text.Rectangle.BOTTOM_BORDER | iTextSharp.text.Rectangle.LEFT_BORDER | iTextSharp.text.Rectangle.RIGHT_BORDER | iTextSharp.text.Rectangle.TOP_BORDER;
                    table.AddCell(cell);

                    cell = new PdfPCell();
                    chunk = new Chunk("NO. \r\nPASPORT", FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK));
                    cell = new PdfPCell(new Phrase(chunk));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    cell.Border = iTextSharp.text.Rectangle.BOTTOM_BORDER | iTextSharp.text.Rectangle.LEFT_BORDER | iTextSharp.text.Rectangle.RIGHT_BORDER | iTextSharp.text.Rectangle.TOP_BORDER;
                    table.AddCell(cell);

                    cell = new PdfPCell();
                    chunk = new Chunk("KOD \r\nNEGARA", FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK));
                    cell = new PdfPCell(new Phrase(chunk));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    cell.Border = iTextSharp.text.Rectangle.BOTTOM_BORDER | iTextSharp.text.Rectangle.LEFT_BORDER | iTextSharp.text.Rectangle.RIGHT_BORDER | iTextSharp.text.Rectangle.TOP_BORDER;
                    table.AddCell(cell);

                    cell = new PdfPCell();
                    chunk = new Chunk("PCB (RM)", FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK));
                    cell = new PdfPCell(new Phrase(chunk));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    cell.Border = iTextSharp.text.Rectangle.BOTTOM_BORDER | iTextSharp.text.Rectangle.LEFT_BORDER | iTextSharp.text.Rectangle.RIGHT_BORDER | iTextSharp.text.Rectangle.TOP_BORDER;
                    table.AddCell(cell);

                    cell = new PdfPCell();
                    chunk = new Chunk("CP38 (RM)", FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK));
                    cell = new PdfPCell(new Phrase(chunk));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    cell.Border = iTextSharp.text.Rectangle.BOTTOM_BORDER | iTextSharp.text.Rectangle.LEFT_BORDER | iTextSharp.text.Rectangle.RIGHT_BORDER | iTextSharp.text.Rectangle.TOP_BORDER;
                    table.AddCell(cell);

                    decimal totalPCB = 0;
                    decimal totalCP38 = 0;
                    for (int pageDataIndex = 0; pageDataIndex < 24; pageDataIndex++)
                    {
                        string bil = "";
                        string refTaxNo = "";
                        string workerName = "";
                        string oldIC = "";
                        string newIC = "";
                        string workerNo = "";
                        string passportNo = "";
                        string countryCode = "";
                        string pCB = "";
                        string cP38 = "";
                        var taxCP39Data = new sp_TaxCP39_Result();
                        if (dataIndex < taxCP39Arr.Count())
                        {
                            taxCP39Data = taxCP39Arr[dataIndex];
                            bil = (pageDataIndex + 1).ToString();
                            refTaxNo = taxCP39Data.fld_TaxNo;
                            workerName = taxCP39Data.fld_Nama;
                            newIC = taxCP39Data.fld_Nokp;
                            workerNo = taxCP39Data.fld_Nopkj;
                            passportNo = taxCP39Data.fld_PassportNo;
                            countryCode = taxCP39Data.fld_CountryCode != "MY" ? taxCP39Data.fld_CountryCode : "";
                            pCB = taxCP39Data.fld_CarumanPekerja.ToString("N");
                            cP38 = taxCP39Data.fld_CP38Amount.ToString("N");
                            totalPCB = totalPCB + taxCP39Data.fld_CarumanPekerja;
                            totalCP38 = totalCP38 + taxCP39Data.fld_CP38Amount;
                        }

                        cell = new PdfPCell();
                        chunk = new Chunk(bil, FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK));
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.FixedHeight = 18;

                        cell.Border = iTextSharp.text.Rectangle.BOTTOM_BORDER | iTextSharp.text.Rectangle.LEFT_BORDER | iTextSharp.text.Rectangle.RIGHT_BORDER | iTextSharp.text.Rectangle.TOP_BORDER;
                        table.AddCell(cell);

                        cell = new PdfPCell();
                        chunk = new Chunk(refTaxNo, FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK));
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.FixedHeight = 18;
                        cell.Border = iTextSharp.text.Rectangle.BOTTOM_BORDER | iTextSharp.text.Rectangle.LEFT_BORDER | iTextSharp.text.Rectangle.RIGHT_BORDER | iTextSharp.text.Rectangle.TOP_BORDER;
                        table.AddCell(cell);

                        cell = new PdfPCell();
                        chunk = new Chunk(workerName, FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK));
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.FixedHeight = 18;
                        cell.Border = iTextSharp.text.Rectangle.BOTTOM_BORDER | iTextSharp.text.Rectangle.LEFT_BORDER | iTextSharp.text.Rectangle.RIGHT_BORDER | iTextSharp.text.Rectangle.TOP_BORDER;
                        table.AddCell(cell);

                        cell = new PdfPCell();
                        chunk = new Chunk(oldIC, FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK));
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.FixedHeight = 18;
                        cell.Border = iTextSharp.text.Rectangle.BOTTOM_BORDER | iTextSharp.text.Rectangle.LEFT_BORDER | iTextSharp.text.Rectangle.RIGHT_BORDER | iTextSharp.text.Rectangle.TOP_BORDER;
                        table.AddCell(cell);

                        cell = new PdfPCell();
                        chunk = new Chunk(newIC, FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK));
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.FixedHeight = 18;
                        cell.Border = iTextSharp.text.Rectangle.BOTTOM_BORDER | iTextSharp.text.Rectangle.LEFT_BORDER | iTextSharp.text.Rectangle.RIGHT_BORDER | iTextSharp.text.Rectangle.TOP_BORDER;
                        table.AddCell(cell);

                        cell = new PdfPCell();
                        chunk = new Chunk(workerNo, FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK));
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.FixedHeight = 18;
                        cell.Border = iTextSharp.text.Rectangle.BOTTOM_BORDER | iTextSharp.text.Rectangle.LEFT_BORDER | iTextSharp.text.Rectangle.RIGHT_BORDER | iTextSharp.text.Rectangle.TOP_BORDER;
                        table.AddCell(cell);

                        cell = new PdfPCell();
                        chunk = new Chunk(passportNo, FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK));
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.FixedHeight = 18;
                        cell.Border = iTextSharp.text.Rectangle.BOTTOM_BORDER | iTextSharp.text.Rectangle.LEFT_BORDER | iTextSharp.text.Rectangle.RIGHT_BORDER | iTextSharp.text.Rectangle.TOP_BORDER;
                        table.AddCell(cell);

                        cell = new PdfPCell();
                        chunk = new Chunk(countryCode, FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK));
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.FixedHeight = 18;
                        cell.Border = iTextSharp.text.Rectangle.BOTTOM_BORDER | iTextSharp.text.Rectangle.LEFT_BORDER | iTextSharp.text.Rectangle.RIGHT_BORDER | iTextSharp.text.Rectangle.TOP_BORDER;
                        table.AddCell(cell);

                        cell = new PdfPCell();
                        chunk = new Chunk(pCB, FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK));
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.FixedHeight = 18;
                        cell.Border = iTextSharp.text.Rectangle.BOTTOM_BORDER | iTextSharp.text.Rectangle.LEFT_BORDER | iTextSharp.text.Rectangle.RIGHT_BORDER | iTextSharp.text.Rectangle.TOP_BORDER;
                        table.AddCell(cell);

                        cell = new PdfPCell();
                        chunk = new Chunk(cP38, FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK));
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.FixedHeight = 18;
                        cell.Border = iTextSharp.text.Rectangle.BOTTOM_BORDER | iTextSharp.text.Rectangle.LEFT_BORDER | iTextSharp.text.Rectangle.RIGHT_BORDER | iTextSharp.text.Rectangle.TOP_BORDER;
                        table.AddCell(cell);
                        dataIndex++;
                    }
                    cell = new PdfPCell();
                    chunk = new Chunk("Borang CP39 boleh diperolehi di laman web : http://www.hasil.gov.my", FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK));
                    cell = new PdfPCell(new Phrase(chunk));
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.FixedHeight = 18;
                    cell.Border = 0;
                    cell.Colspan = 6;
                    table.AddCell(cell);

                    cell = new PdfPCell();
                    chunk = new Chunk("JUMLAH", FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK));
                    cell = new PdfPCell(new Phrase(chunk));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.FixedHeight = 18;
                    cell.Colspan = 2;
                    cell.Border = iTextSharp.text.Rectangle.BOTTOM_BORDER | iTextSharp.text.Rectangle.LEFT_BORDER | iTextSharp.text.Rectangle.RIGHT_BORDER | iTextSharp.text.Rectangle.TOP_BORDER;
                    table.AddCell(cell);

                    cell = new PdfPCell();
                    chunk = new Chunk(totalPCB.ToString("N"), FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK));
                    cell = new PdfPCell(new Phrase(chunk));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.FixedHeight = 18;
                    cell.Border = iTextSharp.text.Rectangle.BOTTOM_BORDER | iTextSharp.text.Rectangle.LEFT_BORDER | iTextSharp.text.Rectangle.RIGHT_BORDER | iTextSharp.text.Rectangle.TOP_BORDER;
                    table.AddCell(cell);

                    cell = new PdfPCell();
                    chunk = new Chunk(totalCP38.ToString("N"), FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK));
                    cell = new PdfPCell(new Phrase(chunk));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.FixedHeight = 18;
                    cell.Border = iTextSharp.text.Rectangle.BOTTOM_BORDER | iTextSharp.text.Rectangle.LEFT_BORDER | iTextSharp.text.Rectangle.RIGHT_BORDER | iTextSharp.text.Rectangle.TOP_BORDER;
                    table.AddCell(cell);

                    cell = new PdfPCell();
                    chunk = new Chunk(" ", FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK));
                    cell = new PdfPCell(new Phrase(chunk));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.FixedHeight = 18;
                    cell.Colspan = 6;
                    cell.Border = 0;
                    table.AddCell(cell);

                    cell = new PdfPCell();
                    chunk = new Chunk("JUMLAH BESAR", FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK));
                    cell = new PdfPCell(new Phrase(chunk));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.FixedHeight = 18;
                    cell.Colspan = 2;
                    cell.Border = iTextSharp.text.Rectangle.BOTTOM_BORDER | iTextSharp.text.Rectangle.LEFT_BORDER | iTextSharp.text.Rectangle.RIGHT_BORDER | iTextSharp.text.Rectangle.TOP_BORDER;
                    table.AddCell(cell);

                    cell = new PdfPCell();
                    chunk = new Chunk(totalAmount.ToString("N"), FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK));
                    cell = new PdfPCell(new Phrase(chunk));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.FixedHeight = 18;
                    cell.Colspan = 2;
                    cell.Border = iTextSharp.text.Rectangle.BOTTOM_BORDER | iTextSharp.text.Rectangle.LEFT_BORDER | iTextSharp.text.Rectangle.RIGHT_BORDER | iTextSharp.text.Rectangle.TOP_BORDER;
                    table.AddCell(cell);

                    mainCell = new PdfPCell(table);
                    mainCell.HorizontalAlignment = Element.ALIGN_LEFT;
                    mainCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    mainCell.Colspan = 5;
                    mainCell.Border = 0;
                    mainTable.AddCell(mainCell);

                    document.Add(mainTable);
                }

                // front page content

                document.Close();
                writer.Close();
                reader.Close();
                ms.Close();
                byte[] file2 = ms.ToArray();

                //I don't have a web server handy so I'm going to write my final MemoryStream to a byte array and then to disk
                byte[] bytes;


                //Create our final combined MemoryStream
                using (MemoryStream finalStream = new MemoryStream())
                {
                    //Create our copy object
                    PdfCopyFields copy = new PdfCopyFields(finalStream);

                    copy.AddDocument(new PdfReader(file));

                    copy.AddDocument(new PdfReader(file2));
                    copy.Close();

                    //Get the raw bytes to save to disk
                    bytes = finalStream.ToArray();
                }
                output.Write(bytes, 0, bytes.Length);
                output.Position = 0;
                return new FileStreamResult(output, "application/pdf");

            }
        }

        public ActionResult TaxCP8D()
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            int[] wlyhid = new int[] { };
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

            List<SelectListItem> CompCodeList = new List<SelectListItem>();
            CompCodeList = new SelectList(dbC.tbl_Syarikat.OrderBy(x => x.fld_NamaPndkSyarikat), "fld_NamaPndkSyarikat", "fld_NamaPndkSyarikat").ToList();
            ViewBag.CompCodeList = CompCodeList;
            return View();
        }

        public ViewResult _TaxCP8D(string CompCodeList, int? YearList, string print)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string NamaSyarikat = "";

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            var taxCP8D_Result = new List<TaxCP8D_Result>();
            var workerInfoList = new List<WorkerInfo>();
            var workerTaxCP8DList = new List<WorkerTaxCP8D>();
            var otherContributionList = new List<OtherContribution>();
            var specialIncentiveList = new List<SpecialIncentive>();
            var NSWL = GetNSWL.GetLadangDetailByRegion(CompCodeList);

            ViewBag.YearList = YearList;
            var syarikat = dbC.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID).FirstOrDefault();
            ViewBag.NamaSyarikat = syarikat.fld_NamaSyarikat;
            ViewBag.NamaPendekSyarikat = syarikat.fld_NamaPndkSyarikat;
            ViewBag.NoSyarikat = syarikat.fld_NoSyarikat;

            ViewBag.NegaraID = NegaraID;
            ViewBag.SyarikatID = SyarikatID;
            ViewBag.UserID = getuserid;
            ViewBag.UserName = User.Identity.Name;
            ViewBag.Date = DateTime.Now.ToShortDateString();
            ViewBag.Time = DateTime.Now.ToShortTimeString();
            ViewBag.Print = print;

            ViewBag.Description = "Region " + NamaSyarikat + " - CP 8D for " + YearList;

            if (CompCodeList != null)
            {
                foreach (var regionID in NSWL.Select(s => s.fld_WilayahID).Distinct().ToList())
                {
                    string constr = Connection.GetConnectionString(regionID, SyarikatID.Value, NegaraID.Value);
                    var con = new SqlConnection(constr);
                    try
                    {
                        DynamicParameters parameters = new DynamicParameters();
                        parameters.Add("Year", YearList);
                        con.Open();
                        var result = SqlMapper.QueryMultiple(con, "sp_TaxCP8D", parameters, commandType: CommandType.StoredProcedure);
                        workerInfoList.AddRange(result.Read<WorkerInfo>().ToList());
                        workerTaxCP8DList.AddRange(result.Read<WorkerTaxCP8D>().ToList());
                        otherContributionList.AddRange(result.Read<OtherContribution>().ToList());
                        specialIncentiveList.AddRange(result.Read<SpecialIncentive>().ToList());
                        con.Close();
                    }
                    catch (Exception ex)
                    {
                        throw;
                    }
                }
            }

            if (workerTaxCP8DList.Count() > 0)
            {
                foreach (var workerInfo in workerInfoList.Select(s => new { s.fld_NoPkjPermanent, s.fld_LadangID, s.fld_DivisionID }).Distinct().ToList())
                {
                    var workerTax = workerTaxCP8DList.Where(x => x.fld_NoPkjPermanent == workerInfo.fld_NoPkjPermanent).ToList();
                    if (workerTax.Count() > 0)
                    {
                        var workerNo = workerInfoList.Where(x => x.fld_NoPkjPermanent == workerInfo.fld_NoPkjPermanent).Select(s => s.fld_Nopkj).ToList();
                        var specialIncentive = specialIncentiveList.Where(x => workerNo.Contains(x.fld_Nopkj)).ToList();
                        var estateInfo = NSWL.Where(x => x.fld_LadangID == workerInfo.fld_LadangID).FirstOrDefault();
                        var workerInfo2 = workerInfoList.Where(x => x.fld_NoPkjPermanent == workerInfo.fld_NoPkjPermanent).FirstOrDefault();
                        var workerTaxInfo = workerTax.OrderByDescending(o => o.fld_Month).Take(1).FirstOrDefault();
                        taxCP8D_Result.Add(new TaxCP8D_Result
                        {
                            EstateName = estateInfo.fld_NamaLadang,
                            TINNo = workerTaxInfo.fld_TaxNo,
                            NoPkerja = workerInfo.fld_NoPkjPermanent,
                            NamaPkerja = workerInfo2.fld_Nama,
                            IDNo = workerInfo2.fld_Nokp,
                            PCB = workerTax.Sum(s => s.fld_PCB) + specialIncentive.Sum(s => s.fld_PCBCarumanPekerja),
                            CP38 = workerTax.Sum(s => s.fld_CP38),
                        });
                    }
                }
            }
            return View(taxCP8D_Result);
        }

        public JsonResult TaxCP8DDetail(string CompCode, int Year)
        {
            string msg = "";
            string statusmsg = "";
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);

            string stringyear = "";
            string stringmonth = "";
            stringyear = Year.ToString();
            stringmonth = (stringmonth.Length == 1 ? "0" + stringmonth : stringmonth);
            decimal? TotalMTDAmt = 0;
            int TotalMTDRec = 0;
            decimal? TotalCP38Amt = 0;
            int TotalCP38Rec = 0;

            var NSWL = GetNSWL.GetLadangDetailByRegion(CompCode);

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            List<sp_TaxCP39_Result> taxCP39 = new List<sp_TaxCP39_Result>();

            var SyarikatDetail = dbC.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID).FirstOrDefault();
            string filename = "Tax CP8D (" + SyarikatDetail.fld_NamaPndkSyarikat.ToUpper() + ") " + "" + stringmonth + stringyear + ".txt";

            try
            {
                var taxCP8D_Result = new List<TaxCP8D_Result>();
                var workerInfoList = new List<WorkerInfo>();
                var workerTaxCP8DList = new List<WorkerTaxCP8D>();
                var otherContributionList = new List<OtherContribution>();
                var specialIncentiveList = new List<SpecialIncentive>();
                foreach (var regionID in NSWL.Select(s => s.fld_WilayahID).Distinct().ToList())
                {
                    string constr = Connection.GetConnectionString(regionID, SyarikatID.Value, NegaraID.Value);
                    var con = new SqlConnection(constr);

                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("Year", Year);
                    con.Open();
                    var result = SqlMapper.QueryMultiple(con, "sp_TaxCP8D", parameters, commandType: CommandType.StoredProcedure);
                    workerInfoList.AddRange(result.Read<WorkerInfo>().ToList());
                    workerTaxCP8DList.AddRange(result.Read<WorkerTaxCP8D>().ToList());
                    otherContributionList.AddRange(result.Read<OtherContribution>().ToList());
                    specialIncentiveList.AddRange(result.Read<SpecialIncentive>().ToList());
                    con.Close();
                }

                if (workerTaxCP8DList.Count() > 0)
                {
                    foreach (var workerInfo in workerInfoList.Select(s => new { s.fld_NoPkjPermanent, s.fld_LadangID, s.fld_DivisionID }).Distinct().ToList())
                    {
                        var workerTax = workerTaxCP8DList.Where(x => x.fld_NoPkjPermanent == workerInfo.fld_NoPkjPermanent).ToList();
                        if (workerTax.Count() > 0)
                        {
                            var workerNo = workerInfoList.Where(x => x.fld_NoPkjPermanent == workerInfo.fld_NoPkjPermanent).Select(s => s.fld_Nopkj).ToList();
                            var specialIncentive = specialIncentiveList.Where(x => workerNo.Contains(x.fld_Nopkj)).ToList();
                            var estateInfo = NSWL.Where(x => x.fld_DivisionID == workerInfo.fld_DivisionID).FirstOrDefault();
                            var workerInfo2 = workerInfoList.Where(x => x.fld_NoPkjPermanent == workerInfo.fld_NoPkjPermanent).FirstOrDefault();
                            taxCP8D_Result.Add(new TaxCP8D_Result
                            {
                                IDNo = workerInfo2.fld_Nokp,
                                PCB = workerTax.Sum(s => s.fld_PCB) + specialIncentive.Sum(s => s.fld_PCBCarumanPekerja),
                                CP38 = workerTax.Sum(s => s.fld_CP38),
                            });
                        }

                    }
                }

                TotalMTDAmt = taxCP8D_Result.Sum(s => s.PCB);
                TotalMTDRec = taxCP8D_Result.Count();
                TotalCP38Amt = taxCP8D_Result.Sum(s => s.CP38);
                TotalCP38Rec = taxCP8D_Result.Where(x => x.CP38 > 0).Count();
            }
            catch (Exception ex)
            {
                throw;
            }

            if (TotalMTDAmt != 0)
            {
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
        public ActionResult DownloadCP8DTextFile(int Year, string CompCode)
        {
            string msg = "";
            string statusmsg = "";
            string link = "";
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            GetGenerateFile getGenerateFile = new GetGenerateFile();
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            var taxCP8D_Result = new List<TaxCP8D_Result>();
            var workerInfoList = new List<WorkerInfo>();
            var workerTaxCP8DList = new List<WorkerTaxCP8D>();
            var otherContributionList = new List<OtherContribution>();
            var specialIncentiveList = new List<SpecialIncentive>();
            var SyarikatDetail = dbC.tbl_Syarikat.Where(x => x.fld_NamaPndkSyarikat == CompCode).FirstOrDefault();
            try
            {
                var NSWL = GetNSWL.GetLadangDetailByRegion(CompCode);

                foreach (var regionID in NSWL.Select(s => s.fld_WilayahID).Distinct().ToList())
                {
                    string constr = Connection.GetConnectionString(regionID, SyarikatID.Value, NegaraID.Value);
                    var con = new SqlConnection(constr);
                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("Year", Year);
                    con.Open();
                    var result = SqlMapper.QueryMultiple(con, "sp_TaxCP8D", parameters, commandType: CommandType.StoredProcedure);
                    workerInfoList.AddRange(result.Read<WorkerInfo>().ToList());
                    workerTaxCP8DList.AddRange(result.Read<WorkerTaxCP8D>().ToList());
                    otherContributionList.AddRange(result.Read<OtherContribution>().ToList());
                    specialIncentiveList.AddRange(result.Read<SpecialIncentive>().ToList());
                    con.Close();
                }
                
                if (workerTaxCP8DList.Count() > 0)
                {
                    foreach (var workerInfo in workerInfoList.Select(s => new { s.fld_NoPkjPermanent, s.fld_LadangID, s.fld_DivisionID }).Distinct().ToList())
                    {
                        var workerTax = workerTaxCP8DList.Where(x => x.fld_NoPkjPermanent == workerInfo.fld_NoPkjPermanent).ToList();
                        if (workerTax.Count() > 0)
                        {
                            var workerNo = workerInfoList.Where(x => x.fld_NoPkjPermanent == workerInfo.fld_NoPkjPermanent).Select(s => s.fld_Nopkj).ToList();
                            var specialIncentive = specialIncentiveList.Where(x => workerNo.Contains(x.fld_Nopkj)).ToList();
                            var estateInfo = NSWL.Where(x => x.fld_DivisionID == workerInfo.fld_DivisionID).FirstOrDefault();
                            var workerInfo2 = workerInfoList.Where(x => x.fld_NoPkjPermanent == workerInfo.fld_NoPkjPermanent).FirstOrDefault();
                            var otherContribution = otherContributionList.Where(x => x.fld_NopkjPermanent == workerInfo.fld_NoPkjPermanent).ToList();
                            var workerTaxInfo = workerTax.OrderByDescending(o => o.fld_Month).Take(1).FirstOrDefault();
                            taxCP8D_Result.Add(new TaxCP8D_Result
                            {
                                NamaPkerja = workerInfo2.fld_Nama,
                                TINNo = workerTax.Select(s => s.fld_TaxNo).FirstOrDefault(),
                                NoPkerja = workerInfo.fld_NoPkjPermanent,
                                IDNo = workerInfo2.fld_Nokp,
                                KategoryPekerja = workerTaxInfo.fld_TaxMaritalStatus,
                                StatusPekerja = 2,
                                TarikhAkhirBekerja = workerTaxInfo.fld_Kdrkyt == "MA" ? workerTaxInfo.fld_Trlhr.Value.AddYears(60) : workerTaxInfo.fld_ContractExpiryDate ?? workerTaxInfo.fld_Trlhr.Value.AddYears(60),
                                MajikanTanggungCukai = 2,
                                BilanganAnak = workerTaxInfo.fld_ChildAbove18CertFull + workerTaxInfo.fld_ChildAbove18CertHalf + workerTaxInfo.fld_ChildAbove18HigherFull + workerTaxInfo.fld_ChildAbove18HigherHalf + workerTaxInfo.fld_ChildBelow18Full + workerTaxInfo.fld_ChildBelow18Half + workerTaxInfo.fld_DisabledChildFull + workerTaxInfo.fld_DisabledChildHalf + workerTaxInfo.fld_DisabledChildStudyFull + workerTaxInfo.fld_DisabledChildStudyHalf,
                                JumlahPelepasanAnak = (int)workerTax.OrderByDescending(o => o.fld_Month).Select(s => s.fld_PelepasanAnak).Take(1).FirstOrDefault(),
                                JumlahSaraanKasar = (int)workerTax.Sum(s => s.fld_SaraanKasar),
                                ManfaatBarangan = 0,
                                NilaiKediaman = 0,
                                ESOS = 0,
                                ElaunDikecualikan = 0,
                                JumlahTuntutanPelepasan = 0,
                                JumlahTututanZakat = 0,
                                KWSP = (int)workerTax.Sum(s => s.fld_KWSPPkj) + (int)specialIncentive.Sum(s => s.fld_KWSPPkj),
                                ZakatPotonganGaji = (int)workerTax.Sum(s => s.fld_Zakat),
                                PCB = workerTax.Sum(s => s.fld_PCB) + specialIncentive.Sum(s => s.fld_PCBCarumanPekerja),
                                CP38 = workerTax.Sum(s => s.fld_CP38),
                                InsuransPotonganGaji = 0,
                                PERKESO = (int)otherContribution.Sum(s => s.fld_CarumanPekerja)
                            });
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            string fileContent = "";

            #region Body
            foreach (var item in taxCP8D_Result)
            {
                fileContent += item.NamaPkerja + "|";
                fileContent += item.TINNo + "|";
                fileContent += item.IDNo + "|";
                fileContent += item.KategoryPekerja + "|";
                fileContent += item.StatusPekerja + "|";
                fileContent += item.TarikhAkhirBekerja.Value.ToString("dd-MM-yyyy") + "|";
                fileContent += item.MajikanTanggungCukai + "|";
                fileContent += item.BilanganAnak == 0 ? "|" : item.BilanganAnak + "|";
                fileContent += item.JumlahPelepasanAnak == 0 ? "|" : item.JumlahPelepasanAnak + "|";
                fileContent += item.JumlahSaraanKasar == 0 ? "|" : item.JumlahSaraanKasar + "|";
                fileContent += item.ManfaatBarangan == 0 ? "|" : item.ManfaatBarangan + "|";
                fileContent += item.NilaiKediaman == 0 ? "|" : item.NilaiKediaman + "|";
                fileContent += item.ESOS == 0 ? "|" : item.ESOS + "|";
                fileContent += item.ElaunDikecualikan == 0 ? "|" : item.ElaunDikecualikan + "|";
                fileContent += item.JumlahTuntutanPelepasan == 0 ? "|" : item.JumlahTuntutanPelepasan + "|";
                fileContent += item.JumlahTututanZakat == 0 ? "|" : item.JumlahTututanZakat + "|";
                fileContent += item.KWSP == 0 ? "|" : item.KWSP + "|";
                fileContent += item.ZakatPotonganGaji == 0 ? "|" : item.ZakatPotonganGaji + "|";
                fileContent += item.PCB == 0 ? "|" : item.PCB + "|";
                fileContent += item.CP38 == 0 ? "|" : item.CP38 + "|";
                fileContent += item.InsuransPotonganGaji == 0 ? "|" : item.InsuransPotonganGaji + "|";
                fileContent += item.PERKESO == 0 ? "|" : item.PERKESO.ToString();
                if (taxCP8D_Result.IndexOf(item) != taxCP8D_Result.Count - 1)
                {
                    fileContent += Environment.NewLine;
                }
            }
            #endregion Body
            var filename = "P" + SyarikatDetail.fld_EmployerTaxNo + "_" + Year + ".txt";
            var filePath = getGenerateFile.CreateTextFile(filename, fileContent, "CP39");

            link = Url.Action("Download", "MaybankFileGen", new { filePath, filename });

            msg = GlobalResCorp.msgGenerateSuccess;
            statusmsg = "success";

            return Json(new { msg, statusmsg, link });
        }

        public ActionResult RcmsInstructionLetter()
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
            ViewBag.CompCodeList = CompCodeList;
            return View();
        }

        public FileStreamResult _RcmsInstructionLetter(string CompCode, int? Month, int? Year, string PaymentDate)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            string NamaSyarikat = "";
            string ClientId = "";

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            List<sp_MaybankRcms_Result_v2> maybankRcms_Result = new List<sp_MaybankRcms_Result_v2>();

            ViewBag.MonthList = Month;
            ViewBag.YearList = Year;
            var CompCodeCopy = "FASSB";
            if (CompCode != "0" && CompCode != null)
            {
                CompCodeCopy = CompCode;
            }
            var syarikat = dbC.tbl_Syarikat.Where(x => x.fld_NamaPndkSyarikat == CompCodeCopy).FirstOrDefault();
            ViewBag.NamaSyarikat = syarikat.fld_NamaSyarikat;
            ViewBag.NamaPendekSyarikat = syarikat.fld_NamaPndkSyarikat;
            ViewBag.NoSyarikat = syarikat.fld_NoSyarikat;
            ViewBag.CorpID = syarikat.fld_CorporateID;
            ViewBag.AccNo = syarikat.fld_AccountNo;
            ClientId = syarikat.fld_ClientBatchID;
            string pdfForm = "";
            if (ClientId == null || ClientId == "")
            {
                if (CompCode == "FASSB")
                {
                    pdfForm = GetConfig.PdfPathFile("Instruction Letter Plain FAS.pdf");
                }

                if (CompCode == "RNDSB")
                {
                    pdfForm = GetConfig.PdfPathFile("Instruction Letter Plain RND.pdf");
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

            string constr = ConfigurationManager.ConnectionStrings["MVC_SYSTEM_HQ_CONN"].ConnectionString;
            var con = new SqlConnection(constr);
            try
            {
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("NegaraID", NegaraID);
                parameters.Add("SyarikatID", SyarikatID);
                parameters.Add("Year", Year);
                parameters.Add("Month", Month);
                parameters.Add("UserID", getuserid);
                parameters.Add("CompCode", CompCode);
                con.Open();
                maybankRcms_Result = SqlMapper.Query<sp_MaybankRcms_Result_v2>(con, "sp_MaybankRcms", parameters).ToList();
                con.Close();
            }
            catch (Exception ex)
            {
                throw;
            }

            if (maybankRcms_Result.Count() == 0 || PaymentDate == "")
            {
                Document pdfDoc = new Document(PageSize.A4, 10, 10, 10, 5);
                MemoryStream ms = new MemoryStream();
                MemoryStream output = new MemoryStream();
                PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, ms);
                Chunk chunk = new Chunk();
                Paragraph para = new Paragraph();
                pdfDoc.Open();
                PdfPTable table = new PdfPTable(1);
                table.WidthPercentage = 100;
                PdfPCell cell = new PdfPCell();
                chunk = new Chunk("No Data Found", FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK));
                cell = new PdfPCell(new Phrase(chunk));
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Border = 0;
                table.AddCell(cell);
                pdfDoc.Add(table);
                pdfWriter.CloseStream = false;
                pdfDoc.Close();

                ms.Close();

                byte[] file = ms.ToArray();
                output.Write(file, 0, file.Length);
                output.Position = 0;
                return new FileStreamResult(output, "application/pdf");
            }
            else
            {
                MemoryStream output = new MemoryStream();

                // open the reader
                PdfReader reader = new PdfReader(pdfForm);
                iTextSharp.text.Rectangle size = reader.GetPageSizeWithRotation(1);
                Document document = new Document(size);

                // open the writer
                MemoryStream ms = new MemoryStream();
                //FileStream fs = new FileStream(newFile, FileMode.Create, FileAccess.Write);
                PdfWriter writer = PdfWriter.GetInstance(document, ms);
                document.Open();
                PdfContentByte cb = writer.DirectContent;

                // front page content
                BaseFont bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.EMBEDDED);
                cb.SetColorFill(BaseColor.BLACK);
                cb.SetFontAndSize(bf, 10);

                string letterTitle = "M2E Payroll Payment Instruction Letter";
                string currentDate = DateTime.Now.ToString("dd.MM.yyyy");
                string paymentReference = "MONTHLY SALARY CREDITING";
                string paymentDescription = "SALARY " + ((Constans.MonthNumber)Month).GetEnumDescription().ToString().ToUpper() + "/" + Year.ToString();
                string clientCode = syarikat.fld_CorporateID.ToString();
                string originatorName = syarikat.fld_NamaSyarikat.ToString().ToUpper();
                string originatorAcc = syarikat.fld_AccountNo;
                string amount = maybankRcms_Result.Sum(s => s.fld_GajiBersih).Value.ToString("n");
                string headCount = maybankRcms_Result.Count().ToString();
                DateTime creditDate = DateTime.ParseExact(PaymentDate, "yyyy-MM-dd",
                                       System.Globalization.CultureInfo.InvariantCulture);
                string creditDateStr = creditDate.ToString("dd.MM.yyyy");
                string highestCredit = maybankRcms_Result.Max(s => s.fld_GajiBersih).Value.ToString("n");
                string lowestCredit = maybankRcms_Result.Min(s => s.fld_GajiBersih).Value.ToString("n");

                cb.BeginText();
                string text = letterTitle;
                cb.ShowTextAligned(1, text, 299, 700, 0);
                cb.EndText();

                cb.BeginText();
                text = currentDate;
                cb.ShowTextAligned(0, text, 101, 670, 0);
                cb.EndText();

                cb.BeginText();
                text = paymentReference;
                cb.ShowTextAligned(0, text, 222, 539, 0);
                cb.EndText();

                cb.BeginText();
                text = paymentDescription;
                cb.ShowTextAligned(0, text, 222, 524, 0);
                cb.EndText();

                cb.BeginText();
                text = clientCode;
                cb.ShowTextAligned(0, text, 222, 510, 0);
                cb.EndText();

                cb.BeginText();
                text = originatorName;
                cb.ShowTextAligned(0, text, 222, 495, 0);
                cb.EndText();

                cb.BeginText();
                text = originatorAcc;
                cb.ShowTextAligned(0, text, 222, 481, 0);
                cb.EndText();

                cb.BeginText();
                text = amount;
                cb.ShowTextAligned(1, text, 282, 424, 0);
                cb.EndText();

                cb.BeginText();
                text = amount;
                cb.ShowTextAligned(1, text, 282, 409, 0);
                cb.EndText();

                cb.BeginText();
                text = headCount;
                cb.ShowTextAligned(1, text, 402, 424, 0);
                cb.EndText();

                cb.BeginText();
                text = headCount;
                cb.ShowTextAligned(1, text, 402, 409, 0);
                cb.EndText();

                cb.BeginText();
                text = creditDateStr;
                cb.ShowTextAligned(0, text, 222, 355, 0);
                cb.EndText();

                cb.BeginText();
                text = highestCredit;
                cb.ShowTextAligned(0, text, 222, 340, 0);
                cb.EndText();

                cb.BeginText();
                text = lowestCredit;
                cb.ShowTextAligned(0, text, 222, 325, 0);
                cb.EndText();

                PdfImportedPage page = writer.GetImportedPage(reader, 1);
                cb.AddTemplate(page, 0, 0);

                document.Close();
                writer.Close();
                reader.Close();
                ms.Close();
                byte[] file = ms.ToArray();


                output.Write(file, 0, file.Length);
                output.Position = 0;
                return new FileStreamResult(output, "application/pdf");
            }
        }

        public ActionResult RcmsInstructionLetterOthers()
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

            List<SelectListItem> IncentiveList = new List<SelectListItem>();
            IncentiveList = new SelectList(dbC.tbl_JenisInsentif.Where(x => x.fld_InclSecondPayslip == true && x.fld_JenisInsentif == "P" && x.fld_Deleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).OrderBy(o => o.fld_KodInsentif).Select(s => new SelectListItem { Value = s.fld_KodInsentif, Text = s.fld_Keterangan }), "Value", "Text").ToList();
            ViewBag.IncentiveList = IncentiveList;

            ViewBag.CompCodeList = CompCodeList;
            return View();
        }

        public FileStreamResult _RcmsInstructionLetterOthers(string CompCode, int? Month, int? Year, string PaymentDate, string Incentive)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            string NamaSyarikat = "";
            string ClientId = "";

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            List<sp_MaybankRcmsOthers_Result_v2> maybankRcms_Result = new List<sp_MaybankRcmsOthers_Result_v2>();

            ViewBag.MonthList = Month;
            ViewBag.YearList = Year;
            var CompCodeCopy = "FASSB";
            if (CompCode != "0" && CompCode != null)
            {
                CompCodeCopy = CompCode;
            }
            var syarikat = dbC.tbl_Syarikat.Where(x => x.fld_NamaPndkSyarikat == CompCodeCopy).FirstOrDefault();
            ViewBag.NamaSyarikat = syarikat.fld_NamaSyarikat;
            ViewBag.NamaPendekSyarikat = syarikat.fld_NamaPndkSyarikat;
            ViewBag.NoSyarikat = syarikat.fld_NoSyarikat;
            ViewBag.CorpID = syarikat.fld_CorporateID;
            ViewBag.AccNo = syarikat.fld_AccountNo;
            ClientId = syarikat.fld_ClientBatchID;
            var tbl_JenisInsentif = dbC.tbl_JenisInsentif.Where(x => x.fld_InclSecondPayslip == true && x.fld_JenisInsentif == "P" && x.fld_Deleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_KodInsentif == Incentive).FirstOrDefault();
            string pdfForm = "";
            if (ClientId == null || ClientId == "")
            {
                if (CompCode == "FASSB")
                {
                    pdfForm = GetConfig.PdfPathFile("Instruction Letter Plain FAS.pdf");
                }

                if (CompCode == "RNDSB")
                {
                    pdfForm = GetConfig.PdfPathFile("Instruction Letter Plain RND.pdf");
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

            string constr = ConfigurationManager.ConnectionStrings["MVC_SYSTEM_HQ_CONN"].ConnectionString;
            var con = new SqlConnection(constr);
            try
            {
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("NegaraID", NegaraID);
                parameters.Add("SyarikatID", SyarikatID);
                parameters.Add("Year", Year);
                parameters.Add("Month", Month);
                parameters.Add("UserID", getuserid);
                parameters.Add("CompCode", CompCode);
                parameters.Add("Incentive", Incentive);
                con.Open();
                maybankRcms_Result = SqlMapper.Query<sp_MaybankRcmsOthers_Result_v2>(con, "sp_MaybankRcmsOthers", parameters).ToList();
                con.Close();
            }
            catch (Exception ex)
            {
                throw;
            }

            if (maybankRcms_Result.Count() == 0 || PaymentDate == "")
            {
                Document pdfDoc = new Document(PageSize.A4, 10, 10, 10, 5);
                MemoryStream ms = new MemoryStream();
                MemoryStream output = new MemoryStream();
                PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, ms);
                Chunk chunk = new Chunk();
                Paragraph para = new Paragraph();
                pdfDoc.Open();
                PdfPTable table = new PdfPTable(1);
                table.WidthPercentage = 100;
                PdfPCell cell = new PdfPCell();
                chunk = new Chunk("No Data Found", FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK));
                cell = new PdfPCell(new Phrase(chunk));
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Border = 0;
                table.AddCell(cell);
                pdfDoc.Add(table);
                pdfWriter.CloseStream = false;
                pdfDoc.Close();

                ms.Close();

                byte[] file = ms.ToArray();
                output.Write(file, 0, file.Length);
                output.Position = 0;
                return new FileStreamResult(output, "application/pdf");
            }
            else
            {
                MemoryStream output = new MemoryStream();

                // open the reader
                PdfReader reader = new PdfReader(pdfForm);
                iTextSharp.text.Rectangle size = reader.GetPageSizeWithRotation(1);
                Document document = new Document(size);

                // open the writer
                MemoryStream ms = new MemoryStream();
                //FileStream fs = new FileStream(newFile, FileMode.Create, FileAccess.Write);
                PdfWriter writer = PdfWriter.GetInstance(document, ms);
                document.Open();
                PdfContentByte cb = writer.DirectContent;

                // front page content
                BaseFont bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.EMBEDDED);
                cb.SetColorFill(BaseColor.BLACK);
                cb.SetFontAndSize(bf, 10);

                string letterTitle = "M2E " + tbl_JenisInsentif.fld_Keterangan + " Payment Instruction Letter";
                string currentDate = DateTime.Now.ToString("dd.MM.yyyy");
                string paymentReference = tbl_JenisInsentif.fld_Keterangan.ToUpper() + " CREDITING";
                string paymentDescription = tbl_JenisInsentif.fld_Keterangan.ToUpper() + " " + Year.ToString();
                string clientCode = syarikat.fld_CorporateID.ToString();
                string originatorName = syarikat.fld_NamaSyarikat.ToString().ToUpper();
                string originatorAcc = syarikat.fld_AccountNo;
                string amount = maybankRcms_Result.Sum(s => s.fld_GajiBersih).Value.ToString("n");
                string headCount = maybankRcms_Result.Count().ToString();
                DateTime creditDate = DateTime.ParseExact(PaymentDate, "yyyy-MM-dd",
                                       System.Globalization.CultureInfo.InvariantCulture);
                string creditDateStr = creditDate.ToString("dd.MM.yyyy");
                string highestCredit = maybankRcms_Result.Max(s => s.fld_GajiBersih).Value.ToString("n");
                string lowestCredit = maybankRcms_Result.Min(s => s.fld_GajiBersih).Value.ToString("n");

                cb.BeginText();
                string text = letterTitle;
                cb.ShowTextAligned(1, text, 299, 700, 0);
                cb.EndText();

                cb.BeginText();
                text = currentDate;
                cb.ShowTextAligned(0, text, 101, 670, 0);
                cb.EndText();

                cb.BeginText();
                text = paymentReference;
                cb.ShowTextAligned(0, text, 222, 539, 0);
                cb.EndText();

                cb.BeginText();
                text = paymentDescription;
                cb.ShowTextAligned(0, text, 222, 524, 0);
                cb.EndText();

                cb.BeginText();
                text = clientCode;
                cb.ShowTextAligned(0, text, 222, 510, 0);
                cb.EndText();

                cb.BeginText();
                text = originatorName;
                cb.ShowTextAligned(0, text, 222, 495, 0);
                cb.EndText();

                cb.BeginText();
                text = originatorAcc;
                cb.ShowTextAligned(0, text, 222, 481, 0);
                cb.EndText();

                cb.BeginText();
                text = amount;
                cb.ShowTextAligned(1, text, 282, 424, 0);
                cb.EndText();

                cb.BeginText();
                text = amount;
                cb.ShowTextAligned(1, text, 282, 409, 0);
                cb.EndText();

                cb.BeginText();
                text = headCount;
                cb.ShowTextAligned(1, text, 402, 424, 0);
                cb.EndText();

                cb.BeginText();
                text = headCount;
                cb.ShowTextAligned(1, text, 402, 409, 0);
                cb.EndText();

                cb.BeginText();
                text = creditDateStr;
                cb.ShowTextAligned(0, text, 222, 355, 0);
                cb.EndText();

                cb.BeginText();
                text = highestCredit;
                cb.ShowTextAligned(0, text, 222, 340, 0);
                cb.EndText();

                cb.BeginText();
                text = lowestCredit;
                cb.ShowTextAligned(0, text, 222, 325, 0);
                cb.EndText();

                PdfImportedPage page = writer.GetImportedPage(reader, 1);
                cb.AddTemplate(page, 0, 0);

                document.Close();
                writer.Close();
                reader.Close();
                ms.Close();
                byte[] file = ms.ToArray();


                output.Write(file, 0, file.Length);
                output.Position = 0;
                return new FileStreamResult(output, "application/pdf");
            }
        }

        //public ActionResult RcmsInstructionLetterOthers()
        //{

        //}

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