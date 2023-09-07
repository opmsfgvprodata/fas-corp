using MVC_SYSTEM.App_LocalResources;
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
using MVC_SYSTEM.CorpNewModels;

namespace MVC_SYSTEM.Controllers
{
    public class Approval2Controller : Controller
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
        private MVC_SYSTEM_CorpNewModels dbNM = new MVC_SYSTEM_CorpNewModels();

        [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3,Viewer")]
        public ActionResult ApprovalSalaryRequest()
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

            ViewBag.Approval = "class = active";

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

            List<tbl_SalaryRequest> salaryrequest = new List<tbl_SalaryRequest>();
            salaryrequest = dbNM.tbl_SalaryRequest.Where(x => x.fld_Year == year && x.fld_Month == month).ToList();

            ViewBag.MonthList = new SelectList(dbC.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "monthlist" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false), "fldOptConfValue", "fldOptConfDesc", month);
            ViewBag.YearList = yearlist;
            ViewBag.WilayahIDList = WilayahIDList;
            ViewBag.LadangIDList = LadangIDList;

            ViewBag.UserID = getuserid;
            ViewBag.GetFlag = 1;

            return View(salaryrequest);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3,Viewer")]
        public ActionResult ApprovalSalaryRequest(int? MonthList, int YearList, int WilayahIDList, int LadangIDList)
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
            
            ViewBag.Approval = "class = active";

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            List<SelectListItem> WilayahIDList2 = new List<SelectListItem>();
            List<SelectListItem> LadangIDList2 = new List<SelectListItem>();
            List<tbl_SalaryRequest> salaryrequest = new List<tbl_SalaryRequest>();

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
            if (WilayahIDList == 0 && LadangIDList == 0)
            {
                salaryrequest = dbNM.tbl_SalaryRequest.Where(x => x.fld_Year == YearList && x.fld_Month == MonthList).ToList();
            }
            else if (WilayahIDList != 0 && LadangIDList == 0)
            {
                salaryrequest = dbNM.tbl_SalaryRequest.Where(x => x.fld_Year == YearList && x.fld_Month == MonthList && x.fld_WilayahID == wilayahselection).ToList();
            }
            else if (WilayahIDList != 0 && LadangIDList != 0)
            {
                salaryrequest = dbNM.tbl_SalaryRequest.Where(x => x.fld_Year == YearList && x.fld_Month == MonthList && x.fld_WilayahID == wilayahselection && x.fld_LadangID == ladangselection).ToList();
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

            ViewBag.MonthList = new SelectList(dbC.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "monthlist" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false), "fldOptConfValue", "fldOptConfDesc", MonthList);
            ViewBag.YearList = yearlist;
            ViewBag.WilayahIDList = WilayahIDList2;
            ViewBag.LadangIDList = LadangIDList2;

            ViewBag.UserID = getuserid;
            ViewBag.GetFlag = 1;

            return View(salaryrequest);
        }

        public JsonResult Reviewal(int id, string status)
        {
            SendEmailNotification SendEmailNotification = new SendEmailNotification();
            int? getuserid = getidentity.ID(User.Identity.Name);
            var dt = timezone.gettimezone();
            var getdata = dbNM.tbl_SalaryRequest.Find(id);
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            if (status == "reject")
            {
                getdata.fld_ReviewStatus = false;
                getdata.fld_ReviewDT = dt;
                getdata.fld_ReviewBy = getuserid;
            }
            else
            {
                getdata.fld_ReviewStatus = true;
                getdata.fld_ReviewDT = dt;
                getdata.fld_ReviewBy = getuserid;

                GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
                var GetEstate = db.vw_NSWL.Where(x => x.fld_NegaraID == getdata.fld_NegaraID && x.fld_SyarikatID == getdata.fld_SyarikatID && x.fld_WilayahID == getdata.fld_WilayahID && x.fld_LadangID == getdata.fld_LadangID).Select(s => new { s.fld_NamaWilayah, s.fld_LdgCode, s.fld_NamaLadang, s.fld_WlyhEmail, s.fld_LdgEmail, s.fld_SyarikatEmail, s.fld_NegaraID, s.fld_SyarikatID, s.fld_WilayahID, s.fld_LadangID, s.fld_CostCentre }).FirstOrDefault();
                string Subject = "Kelulusan Permohonan Gaji";
                string Message = "";
                string Department = "";
                string[] cc = new string[] { };
                List<string> cclist = new List<string>();
                string[] bcc = new string[] { };
                List<string> bcclist = new List<string>();


                Message = "<html>";
                Message += "<body>";
                Message += "<p>Assalamualaikum,</p>";
                Message += "<p>Mohon pihak HQ meluluskan permohonan gaji. Keterangan seperti dibawah:-</p>";
                Message += "<table border=\"1\">";
                Message += "<thead>";
                Message += "<tr>";
                Message += "<th>Nama Wilayah</th><th>Kod Ladang</th><th>Nama Ladang</th><th>Bulan</th><th>Tahun</th><th>Jumlah Keseluruhan (RM)</th>";
                Message += "</tr>";
                Message += "</thead>";
                Message += "<tbody>";
                Message += "<tr>";
                Message += "<td align=\"center\">" + GetEstate.fld_NamaWilayah + "</td><td align=\"center\">" + GetEstate.fld_LdgCode + "</td><td align=\"center\">" + GetEstate.fld_NamaLadang + "</td><td align=\"center\">" + getdata.fld_Month + "</td><td align=\"center\">" + getdata.fld_Year + "</td><td align=\"center\">" + getdata.fld_TotalAmount + "</td>";
                Message += "</tr>";
                Message += "</tbody>";
                Message += "</table>";
                Message += "<p>Terima Kasih.</p>";
                Message += "</body>";
                Message += "</html>";

                cclist.Add(GetEstate.fld_SyarikatEmail);
                cclist.Add(GetEstate.fld_LdgEmail);
                cc = cclist.ToArray();
                if (GetEstate.fld_CostCentre == "FASSB")
                {
                    Department = "HQ_FINANCE_APPROVAL_FASSB";
                }
                else
                {
                    Department = "HQ_FINANCE_APPROVAL_RNDSB";
                }
                var ToEmail = db.tblEmailLists.Where(x => x.fldNegaraID == GetEstate.fld_NegaraID && x.fldSyarikatID == GetEstate.fld_SyarikatID && x.fldDepartment == Department && x.fldCategory == "TO" && x.fldDeleted == false).Select(s => new { s.fldEmail, s.fldName }).FirstOrDefault();
                SendEmailNotification.SendEmail(Subject, Message, ToEmail.fldEmail, cc, bcc);
            }
            dbNM.Entry(getdata).State = EntityState.Modified;
            dbNM.SaveChanges();
            return Json(new { });
        }

        public JsonResult Approval(int id, string status)
        {
            SendEmailNotification SendEmailNotification = new SendEmailNotification();
            int? getuserid = getidentity.ID(User.Identity.Name);
            var dt = timezone.gettimezone();
            var getdata = dbNM.tbl_SalaryRequest.Find(id);
            if (status == "reject")
            {
               
                getdata.fld_ApproveStatus = false;
                getdata.fld_ApproveDT = dt;
                getdata.fld_ApproveBy = getuserid;
            }
            else
            {
                getdata.fld_ApproveStatus = true;
                getdata.fld_ApproveDT = dt;
                getdata.fld_ApproveBy = getuserid;
                var GetEstate = db.vw_NSWL.Where(x => x.fld_NegaraID == getdata.fld_NegaraID && x.fld_SyarikatID == getdata.fld_SyarikatID && x.fld_WilayahID == getdata.fld_WilayahID && x.fld_LadangID == getdata.fld_LadangID).Select(s => new { s.fld_NamaWilayah, s.fld_LdgCode, s.fld_NamaLadang, s.fld_WlyhEmail, s.fld_LdgEmail, s.fld_SyarikatEmail, s.fld_NegaraID, s.fld_SyarikatID, s.fld_WilayahID, s.fld_LadangID, s.fld_CostCentre }).FirstOrDefault();
                string Subject = "Kelulusan Permohonan Gaji";
                string Message = "";
                string Department = "";
                string[] cc = new string[] { };
                List<string> cclist = new List<string>();
                string[] bcc = new string[] { };
                List<string> bcclist = new List<string>();


                Message = "<html>";
                Message += "<body>";
                Message += "<p>Assalamualaikum,</p>";
                Message += "<p>Pihak HQ telah meluluskan permohonan gaji. Keterangan seperti dibawah:-</p>";
                Message += "<table border=\"1\">";
                Message += "<thead>";
                Message += "<tr>";
                Message += "<th>Nama Wilayah</th><th>Kod Ladang</th><th>Nama Ladang</th><th>Bulan</th><th>Tahun</th><th>Jumlah Keseluruhan (RM)</th>";
                Message += "</tr>";
                Message += "</thead>";
                Message += "<tbody>";
                Message += "<tr>";
                Message += "<td align=\"center\">" + GetEstate.fld_NamaWilayah + "</td><td align=\"center\">" + GetEstate.fld_LdgCode + "</td><td align=\"center\">" + GetEstate.fld_NamaLadang + "</td><td align=\"center\">" + getdata.fld_Month + "</td><td align=\"center\">" + getdata.fld_Year + "</td><td align=\"center\">" + getdata.fld_TotalAmount + "</td>";
                Message += "</tr>";
                Message += "</tbody>";
                Message += "</table>";
                Message += "<p>Terima Kasih.</p>";
                Message += "</body>";
                Message += "</html>";

                cclist.Add(GetEstate.fld_SyarikatEmail);
                cclist.Add(GetEstate.fld_LdgEmail);
                cc = cclist.ToArray();
                if (GetEstate.fld_CostCentre == "FASSB")
                {
                    Department = "HQ_FINANCE_APPROVAL_FASSB";
                }
                else
                {
                    Department = "HQ_FINANCE_APPROVAL_RNDSB";
                }
                var ToEmail = db.tblEmailLists.Where(x => x.fldNegaraID == GetEstate.fld_NegaraID && x.fldSyarikatID == GetEstate.fld_SyarikatID && x.fldDepartment == Department && x.fldCategory == "TO" && x.fldDeleted == false).Select(s => new { s.fldEmail, s.fldName }).FirstOrDefault();
                SendEmailNotification.SendEmail(Subject, Message, GetEstate.fld_LdgEmail, cc, bcc);

            }
            dbNM.Entry(getdata).State = EntityState.Modified;
            dbNM.SaveChanges();
            return Json(new { });
        }

        public JsonResult ReviewAll(int wil, int year, int month)
        {
            SendEmailNotification SendEmailNotification = new SendEmailNotification();
            int? getuserid = getidentity.ID(User.Identity.Name);
            var dt = timezone.gettimezone();
            var GetEstateList = db.vw_NSWL.Where(x => x.fld_WilayahID == wil && x.fld_Deleted_L == false).ToList();
            var getdatas = dbNM.tbl_SalaryRequest.Where(x => x.fld_WilayahID == wil && x.fld_Month == month && x.fld_Year == year).ToList();
            foreach (var GetEstate in GetEstateList)
            {
                var getdata = getdatas.Where(x => x.fld_LadangID == GetEstate.fld_LadangID).FirstOrDefault();
                if (getdata != null)
                {
                    getdata.fld_ReviewStatus = true;
                    getdata.fld_ReviewDT = dt;
                    getdata.fld_ReviewBy = getuserid;
                    dbNM.Entry(getdata).State = EntityState.Modified;
                    dbNM.SaveChanges();
                    string Subject = "Kelulusan Permohonan Gaji";
                    string Message = "";
                    string Department = "";
                    string[] cc = new string[] { };
                    List<string> cclist = new List<string>();
                    string[] bcc = new string[] { };
                    List<string> bcclist = new List<string>();

                    Message = "<html>";
                    Message += "<body>";
                    Message += "<p>Assalamualaikum,</p>";
                    Message += "<p>Mohon pihak HQ meluluskan permohonan gaji. Keterangan seperti dibawah:-</p>";
                    Message += "<table border=\"1\">";
                    Message += "<thead>";
                    Message += "<tr>";
                    Message += "<th>Nama Wilayah</th><th>Kod Ladang</th><th>Nama Ladang</th><th>Bulan</th><th>Tahun</th><th>Jumlah Keseluruhan (RM)</th>";
                    Message += "</tr>";
                    Message += "</thead>";
                    Message += "<tbody>";
                    Message += "<tr>";
                    Message += "<td align=\"center\">" + GetEstate.fld_NamaWilayah + "</td><td align=\"center\">" + GetEstate.fld_LdgCode + "</td><td align=\"center\">" + GetEstate.fld_NamaLadang + "</td><td align=\"center\">" + getdata.fld_Month + "</td><td align=\"center\">" + getdata.fld_Year + "</td><td align=\"center\">" + getdata.fld_TotalAmount + "</td>";
                    Message += "</tr>";
                    Message += "</tbody>";
                    Message += "</table>";
                    Message += "<p>Terima Kasih.</p>";
                    Message += "</body>";
                    Message += "</html>";

                    cclist.Add(GetEstate.fld_SyarikatEmail);
                    cclist.Add(GetEstate.fld_LdgEmail);
                    cc = cclist.ToArray();
                    if (GetEstate.fld_CostCentre == "FASSB")
                    {
                        Department = "HQ_FINANCE_APPROVAL_FASSB";
                    }
                    else
                    {
                        Department = "HQ_FINANCE_APPROVAL_RNDSB";
                    }
                    var ToEmail = db.tblEmailLists.Where(x => x.fldNegaraID == GetEstate.fld_NegaraID && x.fldSyarikatID == GetEstate.fld_SyarikatID && x.fldDepartment == Department && x.fldCategory == "TO" && x.fldDeleted == false).Select(s => new { s.fldEmail, s.fldName }).FirstOrDefault();
                    SendEmailNotification.SendEmail(Subject, Message, GetEstate.fld_LdgEmail, cc, bcc);
                }
            }
            //var getdata = dbNM.tbl_SalaryRequest.Find(id);

            return Json(new { });
        }

        public JsonResult ApprovalAll(int wil, int year, int month)
        {
            SendEmailNotification SendEmailNotification = new SendEmailNotification();
            int? getuserid = getidentity.ID(User.Identity.Name);
            var dt = timezone.gettimezone();
            var GetEstateList = db.vw_NSWL.Where(x => x.fld_WilayahID == wil && x.fld_Deleted_L == false).ToList();
            var getdatas = dbNM.tbl_SalaryRequest.Where(x => x.fld_WilayahID == wil && x.fld_Month == month && x.fld_Year == year).ToList();
            foreach (var GetEstate in GetEstateList)
            {
                var getdata = getdatas.Where(x=>x.fld_LadangID == GetEstate.fld_LadangID && x.fld_ReviewStatus==true).FirstOrDefault();
                if(getdata != null)
                {
                    getdata.fld_ApproveStatus = true;
                    getdata.fld_ApproveDT = dt;
                    getdata.fld_ApproveBy = getuserid;
                    dbNM.Entry(getdata).State = EntityState.Modified;
                    dbNM.SaveChanges();
                    string Subject = "Kelulusan Permohonan Gaji";
                    string Message = "";
                    string Department = "";
                    string[] cc = new string[] { };
                    List<string> cclist = new List<string>();
                    string[] bcc = new string[] { };
                    List<string> bcclist = new List<string>();
                    
                    Message = "<html>";
                    Message += "<body>";
                    Message += "<p>Assalamualaikum,</p>";
                    Message += "<p>Pihak HQ telah meluluskan permohonan gaji. Keterangan seperti dibawah:-</p>";
                    Message += "<table border=\"1\">";
                    Message += "<thead>";
                    Message += "<tr>";
                    Message += "<th>Nama Wilayah</th><th>Kod Ladang</th><th>Nama Ladang</th><th>Bulan</th><th>Tahun</th><th>Jumlah Keseluruhan (RM)</th>";
                    Message += "</tr>";
                    Message += "</thead>";
                    Message += "<tbody>";
                    Message += "<tr>";
                    Message += "<td align=\"center\">" + GetEstate.fld_NamaWilayah + "</td><td align=\"center\">" + GetEstate.fld_LdgCode + "</td><td align=\"center\">" + GetEstate.fld_NamaLadang + "</td><td align=\"center\">" + getdata.fld_Month + "</td><td align=\"center\">" + getdata.fld_Year + "</td><td align=\"center\">" + getdata.fld_TotalAmount + "</td>";
                    Message += "</tr>";
                    Message += "</tbody>";
                    Message += "</table>";
                    Message += "<p>Terima Kasih.</p>";
                    Message += "</body>";
                    Message += "</html>";

                    cclist.Add(GetEstate.fld_SyarikatEmail);
                    cclist.Add(GetEstate.fld_LdgEmail);
                    cc = cclist.ToArray();
                    if (GetEstate.fld_CostCentre == "FASSB")
                    {
                        Department = "HQ_FINANCE_APPROVAL_FASSB";
                    }
                    else
                    {
                        Department = "HQ_FINANCE_APPROVAL_RNDSB";
                    }
                    var ToEmail = db.tblEmailLists.Where(x => x.fldNegaraID == GetEstate.fld_NegaraID && x.fldSyarikatID == GetEstate.fld_SyarikatID && x.fldDepartment == Department && x.fldCategory == "TO" && x.fldDeleted == false).Select(s => new { s.fldEmail, s.fldName }).FirstOrDefault();
                    SendEmailNotification.SendEmail(Subject, Message, GetEstate.fld_LdgEmail, cc, bcc);
                }
            }
            //var getdata = dbNM.tbl_SalaryRequest.Find(id);

            return Json(new { });
        }
    }
}