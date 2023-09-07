using MVC_SYSTEM.Class;
using MVC_SYSTEM.Models;
using MVC_SYSTEM.ViewingModels;
using MVC_SYSTEM.log;
using MVC_SYSTEM.App_LocalResources;
using MVC_SYSTEM.Attributes;
using MVC_SYSTEM.AuthModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using MVC_SYSTEM.ModelsCorporate;

namespace MVC_SYSTEM.Controllers
{
    public class GeneralConfigsController : Controller
    {
        private MVC_SYSTEM_ModelsCorporate db = new MVC_SYSTEM_ModelsCorporate();
        private MVC_SYSTEM_Auth db2 = new MVC_SYSTEM_Auth();
        GetConfig GetConfig = new GetConfig();
        GetIdentity GetIdentity = new GetIdentity();
        GetNSWL GetNSWL = new GetNSWL();
        GetWilayah GetWilayah = new GetWilayah();
        GetLadang GetLadang = new GetLadang();
        errorlog geterror = new errorlog();
        // GET: GeneralConfigs
        [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1")]
        public ActionResult Index(string filter = "", int fldUserID = 0, int page = 1, string sort = "fldUserName", string sortdir = "DESC")
        {
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            int? getroleid = GetIdentity.getRoleID(getuserid);
            int?[] reportid = new int?[] { };
            ViewBag.GeneralConfig = "class = active";
            ViewBag.MaintenanceList = new SelectList(db.tblMaintenanceLists.Where(x => x.fldDeleted == false).OrderBy(o => o.fldID).Select(s => new SelectListItem { Value = s.fldID.ToString(), Text = s.fldName }), "Value", "Text").ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1")]
        public ActionResult Index(int MaintenanceList)
        {
            string action = "", controller = "";
            var getentry = db.tblMaintenanceLists.Find(MaintenanceList);

            action = getentry.fldAction;
            controller = getentry.fldController;

            return RedirectToAction(action, controller, new { id = MaintenanceList });
        }

        [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1")]
        public ActionResult GeneralConfig(string filter = "", int fldUserID = 0, int page = 1, string sort = "fldOptConfValue", string sortdir = "ASC", int id = 0)
        {

            MVC_SYSTEM_Viewing dbview = new MVC_SYSTEM_Viewing();
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            int? NegaraID = 0;
            int? SyarikatID = 0;
            int? WilayahID = 0;
            int? LadangID = 0;

            ViewBag.DataEntry = "class = active";
            ViewBag.Dropdown2 = "dropdown open active";
            ViewBag.filter = filter;
            int pageSize = int.Parse(GetConfig.GetData("paging"));
            var records = new PagedList<ViewingModels.tblOptionConfigsWeb>();
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            int flag = id;
            string type = "";
            switch (flag)
            {
                case 1:
                    type = "monthlist";
                    break;
                case 3:
                    type = "cuti";
                    break;
                case 4:
                    type = "jantina";
                    break;
                case 5:
                    type = "kdhPengiraan";
                    break;
                case 6:
                    type = "krytnlist";
                    break;
            }

            if (filter == "")
            {
                records.Content = dbview.tblOptionConfigsWeb.Where(x => x.fldOptConfFlag1 == type && x.fldDeleted == false)
                        .OrderBy(sort + " " + sortdir)
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();

                records.TotalRecords = dbview.tblOptionConfigsWeb.Where(x => x.fldOptConfFlag1 == type && x.fldDeleted == false).Count();
                records.CurrentPage = page;
                records.PageSize = pageSize;
            }
            else
            {
                records.Content = dbview.tblOptionConfigsWeb.Where(x => (x.fldOptConfValue.Contains(filter) || x.fldOptConfDesc.Contains(filter)) && (x.fldOptConfFlag1 == type && x.fldDeleted == false))
                        .OrderBy(sort + " " + sortdir)
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();

                records.TotalRecords = dbview.tblOptionConfigsWeb.Where(x => (x.fldOptConfValue.Contains(filter) || x.fldOptConfDesc.Contains(filter)) && (x.fldOptConfFlag1 == type && x.fldDeleted == false)).Count();
                records.CurrentPage = page;
                records.PageSize = pageSize;
            }
            ViewBag.Flag1 = type;
            return View(records);
        }

        public ActionResult ConfigInsert(string flag1)
        {
            ViewData["Flag1"] = flag1;
            return PartialView("ConfigInsert");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ConfigInsert(string flag1, ModelsCorporate.tblOptionConfigsWeb tblOptionConfigsWeb)
        {

            //if (ModelState.IsValid)
            //{
            try
            {
                var checkdata = db.tblOptionConfigsWebs.Where(x => x.fldOptConfValue == tblOptionConfigsWeb.fldOptConfValue && x.fldOptConfFlag1 == flag1).FirstOrDefault();
                if (checkdata == null)
                {
                    tblOptionConfigsWeb.fldOptConfFlag1 = flag1;
                    tblOptionConfigsWeb.fldDeleted = false;
                    db.tblOptionConfigsWebs.Add(tblOptionConfigsWeb);
                    db.SaveChanges();
                    var getid = db.tblOptionConfigsWebs.Where(w => w.fldOptConfID == tblOptionConfigsWeb.fldOptConfID).FirstOrDefault();
                    return Json(new { success = true, msg = "Data successfully added.", status = "success", checkingdata = "0", method = "1", getid = getid, data1 = tblOptionConfigsWeb.fldOptConfID, data2 = flag1, data3 = "" });
                }
                else
                {
                    return Json(new { success = true, msg = "Data already exist.", status = "warning", checkingdata = "1" });
                }

            }
            catch (Exception ex)
            {
                geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                return Json(new { success = true, msg = "Error occur please contact IT.", status = "danger", checkingdata = "1" });
            }
            //}
            //else
            //{
            //    return Json(new { success = true, msg = "Please check fill you inserted.", status = "warning", checkingdata = "1" });
            //}
        }

        public ActionResult ConfigUpdate(int? id)
        {

            if (id == null)
            {
                return RedirectToAction("GeneralConfig");
            }
            ModelsCorporate.tblOptionConfigsWeb tblOptionConfigsWeb = db.tblOptionConfigsWebs.Where(w => w.fldOptConfID == id && w.fldDeleted == false).FirstOrDefault();
            if (tblOptionConfigsWeb == null)
            {
                return RedirectToAction("GeneralConfig");
            }
            return PartialView("ConfigUpdate", tblOptionConfigsWeb);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ConfigUpdate(int id, Models.tblOptionConfigsWeb tblOptionConfigsWeb)
        {
                try
                {
                    var getdata = db.tblOptionConfigsWebs.Where(w => w.fldOptConfID == id && w.fldDeleted == false).FirstOrDefault();
                    getdata.fldOptConfDesc = tblOptionConfigsWeb.fldOptConfDesc;

                    db.Entry(getdata).State = EntityState.Modified;
                    db.SaveChanges();
                    var getid = id;
                    return Json(new { success = true, msg = "Data successfully edited.", status = "success", checkingdata = "0", method = "1", getid = getid, data1 = "", data2 = "" });
                }
                catch (Exception ex)
                {
                    geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                    return Json(new { success = true, msg = "Error occur please contact IT.", status = "danger", checkingdata = "1" });
                }
        }

        public ActionResult ConfigDelete(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("GeneralConfig");
            }
            ModelsCorporate.tblOptionConfigsWeb tblOptionConfigsWeb = db.tblOptionConfigsWebs.Where(w => w.fldOptConfID == id && w.fldDeleted == false).FirstOrDefault();
            if (tblOptionConfigsWeb == null)
            {
                return RedirectToAction("GeneralConfig");
            }
            return PartialView("ConfigDelete", tblOptionConfigsWeb);
        }

        [HttpPost, ActionName("ConfigDelete")]
        [ValidateAntiForgeryToken]
        public ActionResult ConfigDeleteConfirmed(int id)
        {
            try
            {
                ModelsCorporate.tblOptionConfigsWeb tblOptionConfigsWeb = db.tblOptionConfigsWebs.Find(id);
                if (tblOptionConfigsWeb == null)
                {
                    return Json(new { success = true, msg = "Data already deleted.", status = "success", checkingdata = "0", method = "1", getid = "", data1 = "", data2 = "" });
                }
                else
                {
                    db.tblOptionConfigsWebs.Remove(tblOptionConfigsWeb);
                    db.SaveChanges();
                    return Json(new { success = true, msg = "Data successfully deleted.", status = "success", checkingdata = "0", method = "1", getid = "", data1 = "", data2 = "" });
                }

            }
            catch (Exception ex)
            {
                geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                return Json(new { success = true, msg = "Error occur please contact IT.", status = "danger", checkingdata = "1" });
            }
        }

        [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3")]
        public ActionResult EstateDetail(int WilayahIDList = 0, int fldUserID = 0, int page = 1, string sort = "fld_WlyhID", string sortdir = "ASC")
        {
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            int? NegaraID = 0;
            int? SyarikatID = 0;
            int? WilayahID = 0;
            int? LadangID = 0;
            int[] wlyhid = new int[] { };
            //string mywlyid = "";
            //int? wilayahselection = 0;

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            List<SelectListItem> WilayahIDList2 = new List<SelectListItem>();
            //List<SelectListItem> LadangIDList = new List<SelectListItem>();
            if (WilayahID == 0)
            {
                wlyhid = GetWilayah.GetWilayahID(SyarikatID);
                //mywlyid = String.Join("", wlyhid); ;
                WilayahIDList2 = new SelectList(db.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)).OrderBy(o => o.fld_ID), "fld_ID", "fld_WlyhName").ToList();
                WilayahIDList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
            }
            else
            {
                //WilayahIDList = WilayahID;
                wlyhid = GetWilayah.GetWilayahID2(SyarikatID, WilayahID);
                //mywlyid = String.Join("", wlyhid); ;
                WilayahIDList2 = new SelectList(db.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)).OrderBy(o => o.fld_ID), "fld_ID", "fld_WlyhName").ToList();
                //WilayahIDList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
            }




            MVC_SYSTEM_Viewing dbview = new MVC_SYSTEM_Viewing();
            ViewBag.GeneralConfig = "class = active";
            int pageSize = int.Parse(GetConfig.GetData("paging"));
            //  var records = new PagedList<ViewingModels.tbl_Ladang>(); //comment by wani 12.4.2021
            var records = new PagedList<ViewingModels.vw_WilayahLadang>(); //add by wani 12.4.2021

            //add by wani 12.4.2021
            var vw_WilayahLadang = new List<vw_WilayahLadang>(); //wani add new model
            int id = 1;
            //end add by wani 12.4.2021

            if (WilayahIDList == 0)
            {
                //comment by wani 12.4.2021
                //records.Content = dbview.tbl_Ladang
                //       .OrderBy(sort + " " + sortdir)
                //       .Skip((page - 1) * pageSize)
                //       .Take(pageSize)
                //       .ToList();

                //records.TotalRecords = dbview.tbl_Ladang.Count();
                //records.CurrentPage = page;
                //records.PageSize = pageSize;
                //end comment by wani 12.4.2021

                //add by wani 12.4.2021
                var dataLadang = dbview.tbl_Ladang.ToList();

                foreach (var ldg in dataLadang)
                {
                    var wilayahData = db.tbl_Wilayah.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_ID == ldg.fld_WlyhID).ToList();
                    foreach (var wil in wilayahData)
                    {
                        vw_WilayahLadang.Add(new vw_WilayahLadang
                        {
                            fld_ID = id,
                            LdgCode = ldg.fld_LdgCode,
                            LdgName = ldg.fld_LdgName,
                            WlyhID = ldg.fld_WlyhID,
                            LdgEmail = ldg.fld_LdgEmail,
                            NoAcc = ldg.fld_NoAcc,
                            NoGL = ldg.fld_NoGL,
                            NoCIT = ldg.fld_NoCIT,
                            WilayahName = wil.fld_WlyhName
                        });
                        id++;
                    }
                    records.Content = vw_WilayahLadang.ToList();
                    records.TotalRecords = vw_WilayahLadang.Count();
                    records.CurrentPage = page;
                    records.PageSize = pageSize;
                }
                //end add by wani 12.4.2021
            }
            else
            {
                //add by wani 12.4.2021
                var dataLadang = dbview.tbl_Ladang.Where(x => x.fld_WlyhID == WilayahIDList).ToList();

                foreach (var ldg in dataLadang)
                {
                    var wilayahData = db.tbl_Wilayah.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_ID == ldg.fld_WlyhID).ToList();
                    foreach (var wil in wilayahData)
                    {
                        vw_WilayahLadang.Add(new vw_WilayahLadang
                        {
                            fld_ID = id,
                            LdgCode = ldg.fld_LdgCode,
                            LdgName = ldg.fld_LdgName,
                            WlyhID = ldg.fld_WlyhID,
                            LdgEmail = ldg.fld_LdgEmail,
                            NoAcc = ldg.fld_NoAcc,
                            NoGL = ldg.fld_NoGL,
                            NoCIT = ldg.fld_NoCIT,
                            WilayahName = wil.fld_WlyhName
                        });
                        id++;
                    }
                    records.Content = vw_WilayahLadang.ToList();
                    records.TotalRecords = vw_WilayahLadang.Count();
                    records.CurrentPage = page;
                    records.PageSize = pageSize;
                }
                //end add by wani 12.4.2021

                //comment by wani 12.4.2021
                //records.Content = dbview.tbl_Ladang.Where(x => x.fld_WlyhID == WilayahIDList)
                //       .OrderBy(sort + " " + sortdir)
                //       .Skip((page - 1) * pageSize)
                //       .Take(pageSize)
                //       .ToList();

                //records.TotalRecords = dbview.tbl_Ladang.Where(x => x.fld_WlyhID == WilayahIDList).Count();
                //records.CurrentPage = page;
                //records.PageSize = pageSize;
                //end comment by wani 12.4.2021
            }
            ViewBag.WilayahIDList = WilayahIDList2;
            return View(records);
        }


        public ActionResult EstateDetailUpdate(string id, int? wlyh)
        {

            if (id == null)
            {
                return RedirectToAction("EstateDetail");
            }
            ModelsCorporate.tbl_Ladang tbl_Ladang = db.tbl_Ladang.Where(w => w.fld_WlyhID == wlyh && w.fld_LdgCode == id).FirstOrDefault();
            if (tbl_Ladang == null)
            {
                return RedirectToAction("EstateDetail");
            }

            return PartialView("EstateDetailUpdate", tbl_Ladang);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // public ActionResult EstateDetailUpdate(string id, int wlyh, Models.tbl_Ladang tbl_Ladang) //comment by wani 26.1.2021
        public ActionResult EstateDetailUpdate(string id, int wlyh, ModelsCorporate.tbl_Ladang tbl_Ladang) // added by wani 26.1.2021
        {
            //----comment by wani 23.2.2021
            //if (ModelState.IsValid)
            //{
            //----closed comment by wani 23.2.2021
            try
            {
                    var getdata = db.tbl_Ladang.Where(w => w.fld_LdgCode == id && w.fld_WlyhID == wlyh).FirstOrDefault();

                    getdata.fld_LdgName = tbl_Ladang.fld_LdgName; // added by wani 26.1.2021
                    getdata.fld_LdgEmail = tbl_Ladang.fld_LdgEmail;
                    getdata.fld_NoAcc = tbl_Ladang.fld_NoAcc;
                    getdata.fld_NoGL = tbl_Ladang.fld_NoGL;
                    getdata.fld_NoCIT = tbl_Ladang.fld_NoCIT;

                    db.Entry(getdata).State = EntityState.Modified;
                    db.SaveChanges();
                    var getid = id;
                    return Json(new { success = true, msg = "Data successfully edited.", status = "success", checkingdata = "0", method = "1", getid = getid, data1 = "", data2 = "" });
                }
                catch (Exception ex)
                {
                    geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                    return Json(new { success = true, msg = "Error occur please contact IT.", status = "danger", checkingdata = "1" });
                }
            }
        //----comment by wani 23.2.2021
        //    else
        //    {
        //        return Json(new { success = true, msg = "Please check fill you inserted.", status = "warning", checkingdata = "1" });
        //    }
        //}
        //----closed comment by wani 23.2.2021

        public ActionResult EstateDetailInsert()
        {
            //int drpyear = 0;
            //int drprangeyear = 0;
            int? NegaraID = 0;
            int? SyarikatID = 0;
            int? WilayahID = 0;
            int? LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            int[] wlyhid = new int[] { };
            //string mywlyid = "";

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            List<SelectListItem> WilayahIDList = new List<SelectListItem>();
            List<SelectListItem> LadangIDList = new List<SelectListItem>();

            //-----added by wani 27.1.2021
            List<SelectListItem> NegaraIDList = new List<SelectListItem>();
            NegaraIDList = new SelectList(db.tbl_Negara.Where(x => x.fld_Deleted == false), "fld_NegaraID", "fld_NamaNegara").ToList();
            ViewBag.fld_NegaraID = NegaraIDList;

            //----------------------comment by wani 28.6.2021
            //List<SelectListItem> SyarikatIDList = new List<SelectListItem>();
            //SyarikatIDList = new SelectList(db.tbl_Syarikat.Where(x => x.fld_Deleted == false), "fld_SyarikatID", "fld_NamaSyarikat").ToList();
            //ViewBag.fld_SyarikatID = SyarikatIDList;
            //----------------------closed comment by wani 28.6.2021

            List<SelectListItem> NegeriIDList = new List<SelectListItem>();
            NegeriIDList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldDeleted == false && x.fldOptConfFlag1 == "negeri"), "fldOptConfValue", "fldOptConfFlag2").ToList();
            ViewBag.fld_KodNegeri = NegeriIDList;
            //-----closed added by wani 27.1.2021

            //----add by wani 1.4.2021
            List<SelectListItem> CostCentreList = new List<SelectListItem>();
            CostCentreList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldDeleted == false && x.fldOptConfFlag1 == "costcentre"), "fldOptConfValue", "fldOptConfFlag2").ToList();
            ViewBag.fld_CostCentre = CostCentreList;
            //-----closed add by wani 1.4.2021


            if (WilayahID == 0 && LadangID == 0)
            {
                wlyhid = GetWilayah.GetWilayahID(SyarikatID);
                //mywlyid = String.Join("", wlyhid); ;
                WilayahIDList = new SelectList(db.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)).OrderBy(o => o.fld_ID), "fld_ID", "fld_WlyhName").ToList();
            }
            else if (WilayahID != 0 && LadangID == 0)
            {
                //mywlyid = String.Join("", WilayahID); ;
                wlyhid = GetWilayah.GetWilayahID2(SyarikatID, WilayahID);
                WilayahIDList = new SelectList(db.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
            }
            else if (WilayahID != 0 && LadangID != 0)
            {
                //mywlyid = String.Join("", WilayahID); ;
                wlyhid = GetWilayah.GetWilayahID2(SyarikatID, WilayahID);
                WilayahIDList = new SelectList(db.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
            }

            ViewBag.fld_WlyhID = WilayahIDList;

            //-------------------get estate code 
            //-------------------added by wani 28.1.2021
            var getEstateRunningNo = db.tbl_Ladang
                   .Where(x => x.fld_NegaraID == NegaraID &&
                               x.fld_SyarikatID == SyarikatID)
                   .OrderByDescending(o => o.fld_LdgCode)
                   .Select(s => new { s.fld_LdgCode })
                   .FirstOrDefault();

            int estateTypeCodeLength = getEstateRunningNo.fld_LdgCode.Length;

            string estateCode = "";


            string generateNewLadangCode =
                (Convert.ToInt32(getEstateRunningNo.fld_LdgCode) + 1).ToString("0000");

            ViewBag.fld_LdgCode = generateNewLadangCode;
            //----------------------------------added by wani 28.1.2021


            return PartialView("EstateDetailInsert");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // public ActionResult EstateDetailInsert(ModelsCorporate.tbl_Ladang tbl_Ladang) //comment by wani 28.1.2021
        public ActionResult EstateDetailInsert(ModelsCorporate.tbl_Ladang tbl_Ladang, ModelsCorporate.tbl_Division tbl_Division) //added by wani 28.1.2021
        {
            //GetLadang GetLadang = new GetLadang();
            if (ModelState.IsValid)
            {
                try

                {

                    var checkdata = db.tbl_Ladang.Where(x => x.fld_WlyhID == tbl_Ladang.fld_WlyhID && x.fld_LdgCode == tbl_Ladang.fld_LdgCode).FirstOrDefault();
                    var checkdata1 = db.tbl_Ladang.Where(x => x.fld_LdgName == tbl_Ladang.fld_LdgName).FirstOrDefault(); // added by wani 28.1.2021

                    //-----comment by wani 19.2.2021
                    //if (tbl_Ladang.fld_LdgEmail == null)
                    //{

                    //    return Json(new { success = true, msg = "Please fill in email.", status = "warning", checkingdata = "1" });
                    //}
                    //if (tbl_Ladang.fld_NoAcc == null)
                    //{

                    //    return Json(new { success = true, msg = "Please fill in Acc no.", status = "warning", checkingdata = "1" });
                    //}
                    //if (tbl_Ladang.fld_NoGL == null)
                    //{

                    //    return Json(new { success = true, msg = "Please fill in GL no.", status = "warning", checkingdata = "1" });
                    //}
                    //if (tbl_Ladang.fld_NoCIT == null)
                    //{

                    //    return Json(new { success = true, msg = "Please fill in CIT no.", status = "warning", checkingdata = "1" });
                    //}
                    //----- closed comment by wani 19.2.2021

                    if (checkdata == null && checkdata1 == null) //edited by wani 19.2.2021
                    {
                        tbl_Ladang.fld_Deleted = false;
                        tbl_Ladang.fld_SyarikatID = 1; //add by wani 28.6.2021
                        db.tbl_Ladang.Add(tbl_Ladang);
                        db.SaveChanges();
                        var getid = db.tbl_Ladang.Where(w => w.fld_ID == tbl_Ladang.fld_ID).FirstOrDefault();

                        //----- added by wani 2.2.2021
                        int getladangid = db.tbl_Ladang.Where(w => w.fld_Deleted == false).OrderByDescending(o => o.fld_ID).Select(s => s.fld_ID).FirstOrDefault();

                        //var getsyarikatid = tbl_Ladang.fld_SyarikatID; //comment by wani 28.6.2021
                        // var getdivisionsap = db.tbl_Syarikat.Where(w => w.fld_SyarikatID == getsyarikatid).Select(s => s.fld_SAPComCode).FirstOrDefault(); //comment by wani 28.6.2021

                        //add by wani 28.6.2021
                        var getcostcentre = tbl_Ladang.fld_CostCentre;
                        var getdivisionsapcode = db.tblOptionConfigsWebs.Where(w => w.fldOptConfFlag1 == "costcentre" && w.fldOptConfValue == getcostcentre).Select(s => s.fldOptConfDesc).FirstOrDefault();
                        //closed add by wani 28.6.2021

                        tbl_Division.fld_DivisionName = "Division 1";
                        tbl_Division.fld_SyarikatID = 1; //add by wani 28.6.2021
                        tbl_Division.fld_JnsLot = "E";
                        tbl_Division.fld_Deleted = false;
                        tbl_Division.fld_DivisionSAPCode = getdivisionsapcode; //edit by wani 28.6.2021
                        tbl_Division.fld_WilayahID = tbl_Ladang.fld_WlyhID;
                        tbl_Division.fld_LadangID = getladangid;
                        db.tbl_Division.Add(tbl_Division);
                        db.SaveChanges();
                        var getid2 = db.tbl_Division.Where(w => w.fld_ID == tbl_Division.fld_ID).FirstOrDefault();
                        //------ closed added by wani 2.2.2021

                        return Json(new { success = true, msg = "Data successfully added.", status = "success", checkingdata = "0", method = "1", getid = getid, data1 = tbl_Ladang.fld_ID, data2 = tbl_Ladang.fld_ID, data3 = "" });
                    }
                    else
                    {
                        return Json(new { success = true, msg = "Nama Ladang/Ladang Code already exist.", status = "warning", checkingdata = "1" }); //edited by wani 19.2.2021
                    }

                }
                catch (Exception ex)
                {
                    geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                    return Json(new { success = true, msg = "Error occur please contact IT.", status = "danger", checkingdata = "1" });

                }


            }
            else
            {
                return Json(new { success = true, msg = "Please check fill you inserted.", status = "danger", checkingdata = "1" }); //edited by wani 19.2.2021
            }
        }

        public ActionResult EstateDetailDelete(string id, int? wlyh)
        {
            if (id == null)
            {
                return RedirectToAction("EstateDetail");
            }
            ModelsCorporate.tbl_Ladang tbl_Ladang = db.tbl_Ladang.Where(w => w.fld_WlyhID == wlyh && w.fld_LdgCode == id).FirstOrDefault();
            if (tbl_Ladang == null)
            {
                return RedirectToAction("EstateDetail");
            }
            return PartialView("EstateDetailDelete", tbl_Ladang);
        }

        [HttpPost, ActionName("EstateDetailDelete")]
        [ValidateAntiForgeryToken]
        public ActionResult EstateDetailDeleteConfirmed(string id, int? wlyh) //edited by wani 23.6.2021
        {
            try
            {
                ModelsCorporate.tbl_Ladang tbl_Ladang = db.tbl_Ladang.Where(w => w.fld_WlyhID == wlyh && w.fld_LdgCode == id).FirstOrDefault();
                if (tbl_Ladang == null)
                {
                    return Json(new { success = true, msg = "Data already deleted.", status = "success", checkingdata = "0", method = "1", getid = "", data1 = "", data2 = "" });
                }
                else
                {
                    var ladangid = db.tbl_Ladang.Where(x => x.fld_LdgCode == id).Select(o => o.fld_ID).FirstOrDefault(); //add by wani 22.6.2021
                    db.tbl_Ladang.Remove(tbl_Ladang);
                    db.SaveChanges();

                    //------added by wani 4.2.2021
                    ModelsCorporate.tbl_Division tbl_Division = db.tbl_Division.Where(w => w.fld_LadangID == ladangid && w.fld_WilayahID == wlyh).FirstOrDefault(); //edit by wani 22.6.2021
                    db.tbl_Division.Remove(tbl_Division);
                    db.SaveChanges();
                    //------closed added by wani 4.2.2021

                    return Json(new { success = true, msg = "Data successfully deleted.", status = "success", checkingdata = "0", method = "1", getid = "", data1 = "", data2 = "" });
                }

            }
            catch (Exception ex)
            {
                geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                return Json(new { success = true, msg = "Error occur please contact IT.", status = "danger", checkingdata = "1" });
            }

        }

        [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1")]
        public ActionResult CompanyDetail()
        {
            var record = db.tbl_Syarikat.Where(x => x.fld_NegaraID == 1).FirstOrDefault();
            return View(record);
        }

        public ActionResult CompanyDetailUpdate()
        {
            ModelsCorporate.tbl_Syarikat tbl_Syarikat = db.tbl_Syarikat.Where(x => x.fld_NegaraID == 1).FirstOrDefault();
            return PartialView("CompanyDetailUpdate", tbl_Syarikat);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        //public ActionResult CompanyDetailUpdate(Models.tbl_Syarikat tbl_Syarikat)  //comment by wani 19.3.2021
        public ActionResult CompanyDetailUpdate(ModelsCorporate.tbl_Syarikat tbl_Syarikat)  //add by wani 19.3.2021
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var getdata = db.tbl_Syarikat.Where(w => w.fld_NegaraID == 1).FirstOrDefault();

                    getdata.fld_NamaSyarikat = tbl_Syarikat.fld_NamaSyarikat;
                    getdata.fld_NamaPndkSyarikat = tbl_Syarikat.fld_NamaPndkSyarikat;
                    getdata.fld_NoSyarikat = tbl_Syarikat.fld_NoSyarikat;
                    getdata.fld_SyarikatEmail = tbl_Syarikat.fld_SyarikatEmail;

                    db.Entry(getdata).State = EntityState.Modified;
                    db.SaveChanges();
                    var getid = 0;
                    return Json(new { success = true, msg = "Data successfully edited.", status = "success", checkingdata = "0", method = "1", getid = getid, data1 = "", data2 = "" });
                }
                catch (Exception ex)
                {
                    geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                    return Json(new { success = true, msg = "Error occur please contact IT.", status = "danger", checkingdata = "1" });
                }
            }
            else
            {
                return Json(new { success = true, msg = "Please check fill you inserted.", status = "warning", checkingdata = "1" });
            }
        }

        [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1")]
        public ActionResult AdditionalEmail(string filter = "", int page = 1, string sort = "fldName", string sortdir = "ASC")
        {
            MVC_SYSTEM_Viewing dbview = new MVC_SYSTEM_Viewing();
            int pageSize = int.Parse(GetConfig.GetData("paging"));
            var records = new PagedList<ViewingModels.tblEmailList>();

            if (filter == "")
            {
                records.Content = dbview.tblEmailList.Where(x => x.fldDeleted == false)
                       .OrderBy(sort + " " + sortdir)
                       .Skip((page - 1) * pageSize)
                       .Take(pageSize)
                       .ToList();

                records.TotalRecords = dbview.tblEmailList.Where(x => x.fldDeleted == false).Count();
                records.CurrentPage = page;
                records.PageSize = pageSize;
            }
            else
            {
                int wlyhid = int.Parse(filter);
                records.Content = dbview.tblEmailList.Where(x => (x.fldName.Contains(filter) || x.fldEmail.Contains(filter)) && (x.fldDeleted == false))
                       .OrderBy(sort + " " + sortdir)
                       .Skip((page - 1) * pageSize)
                       .Take(pageSize)
                       .ToList();

                records.TotalRecords = dbview.tblEmailList.Where(x => (x.fldName.Contains(filter) || x.fldEmail.Contains(filter)) && (x.fldDeleted == false)).Count();
                records.CurrentPage = page;
                records.PageSize = pageSize;
            }
            return View(records);
        }

        public ActionResult AdditionalEmailInsert()
        {

            List<SelectListItem> Category = new List<SelectListItem>();
            Category.Insert(0, (new SelectListItem { Text = "BCC", Value = "BCC" }));
            Category.Insert(1, (new SelectListItem { Text = "CC", Value = "CC" }));
            //Fitri add 24-12-2020
            ViewBag.fldDepartment = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldDeleted == false && x.fldOptConfFlag1 == "department"), "fldOptConfValue", "fldOptConfValue");

            int negaraid = 0;

            ViewBag.fldNegaraID = new SelectList(db2.tbl_Negara.Where(x => x.fld_Deleted == false), "fld_NegaraID", "fld_NamaNegara");
            negaraid = db2.tbl_Negara.Where(x => x.fld_Deleted == false).Select(s => s.fld_NegaraID).Take(1).FirstOrDefault();
            ViewBag.fldSyarikatID = new SelectList(db.tbl_Syarikat.Where(x => x.fld_NegaraID == negaraid && x.fld_Deleted == false), "fld_SyarikatID", "fld_NamaSyarikat");
            ViewBag.fldCategory = Category;
            //ViewBag.fld_WlyhID = WilayahIDList;
            return PartialView("AdditionalEmailInsert");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AdditionalEmailInsert(ModelsCorporate.tblEmailList tblEmailList)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var checkdata = db.tblEmailLists.Where(x => x.fldName == tblEmailList.fldName).FirstOrDefault();
                    if (checkdata == null)
                    {
                        //string category = "";
                        //if (CategoryList == 0)
                        //{
                        //    category = "BCC";
                        //}
                        //else
                        //{
                        //    category = "CC";
                        //}
                        //tblEmailList.fldCategory = category;
                        tblEmailList.fldDeleted = false;
                        db.tblEmailLists.Add(tblEmailList);
                        db.SaveChanges();
                        var getid = db.tblEmailLists.Where(w => w.fldName == tblEmailList.fldName).FirstOrDefault();
                        return Json(new { success = true, msg = "Data successfully added.", status = "success", checkingdata = "0", method = "1", getid = getid, data1 = tblEmailList.fldName, data2 = tblEmailList.fldName, data3 = "" });
                    }
                    else
                    {
                        return Json(new { success = true, msg = "Data already exist.", status = "warning", checkingdata = "1" });
                    }

                }
                catch (Exception ex)
                {
                    geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                    return Json(new { success = true, msg = "Error occur please contact IT.", status = "danger", checkingdata = "1" });
                }
            }
            else
            {
                return Json(new { success = true, msg = "Please check fill you inserted.", status = "warning", checkingdata = "1" });
            }
        }

        public ActionResult AdditionalEmailUpdate(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("AdditionalEmail");
            }
            ModelsCorporate.tblEmailList tblEmailList = db.tblEmailLists.Find(id);
            if (tblEmailList == null)
            {
                return RedirectToAction("AdditionalEmail");
            }
            //Fitri add 24-12-2020
            ViewBag.fldDepartment = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldDeleted == false && x.fldOptConfFlag1 == "department"), "fldOptConfValue", "fldOptConfValue", tblEmailList.fldDepartment);
            return PartialView("AdditionalEmailUpdate", tblEmailList);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AdditionalEmailUpdate(int id, Models.tblEmailList tblEmailList)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var getdata = db.tblEmailLists.Find(id);
                    getdata.fldEmail = tblEmailList.fldEmail;
                    getdata.fldDepartment = tblEmailList.fldDepartment; //Fitri add 24-12-2020
                    db.Entry(getdata).State = EntityState.Modified;
                    db.SaveChanges();
                    var getid = id;
                    return Json(new { success = true, msg = "Data successfully edited.", status = "success", checkingdata = "0", method = "1", getid = getid, data1 = "", data2 = "" });
                }
                catch (Exception ex)
                {
                    geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                    return Json(new { success = true, msg = "Error occur please contact IT.", status = "danger", checkingdata = "1" });
                }
            }
            else
            {
                return Json(new { success = true, msg = "Please check fill you inserted.", status = "warning", checkingdata = "1" });
            }
        }

        public ActionResult AdditionalEmailDelete(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("AdditionalEmail");
            }
            ModelsCorporate.tblEmailList tblEmailList = db.tblEmailLists.Find(id);
            if (tblEmailList == null)
            {
                return RedirectToAction("AdditionalEmail");
            }
            return PartialView("AdditionalEmailDelete", tblEmailList);
        }

        [HttpPost, ActionName("AdditionalEmailDelete")]
        [ValidateAntiForgeryToken]
        public ActionResult AdditionalEmailDeleteConfirmed(int id)
        {
            try
            {
                ModelsCorporate.tblEmailList tblEmailList = db.tblEmailLists.Find(id);
                if (tblEmailList == null)
                {
                    return Json(new { success = true, msg = "Data already deleted.", status = "success", checkingdata = "0", method = "1", getid = "", data1 = "", data2 = "" });
                }
                else
                {
                    db.tblEmailLists.Remove(tblEmailList);
                    db.SaveChanges();
                    return Json(new { success = true, msg = "Data successfully deleted.", status = "success", checkingdata = "0", method = "1", getid = "", data1 = "", data2 = "" });
                }

            }
            catch (Exception ex)
            {
                geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                return Json(new { success = true, msg = "Error occur please contact IT.", status = "danger", checkingdata = "1" });
            }
        }

        [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1")]
        public ActionResult Supplier(int page = 1, string sort = "fld_KodPbkl", string sortdir = "ASC", int id = 0)
        {
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            int? NegaraID = 0;
            int? SyarikatID = 0;
            int? WilayahID = 0;
            int? LadangID = 0;
            int[] wlyhid = new int[] { };
            //string mywlyid = "";

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            List<SelectListItem> WilayahIDList = new List<SelectListItem>();
            List<SelectListItem> LadangIDList = new List<SelectListItem>();

            wlyhid = GetWilayah.GetWilayahID(SyarikatID);
            //mywlyid = String.Join("", wlyhid); ;
            WilayahIDList = new SelectList(db.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)).OrderBy(o => o.fld_ID), "fld_ID", "fld_WlyhName").ToList();
            WilayahIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
            var wlyhID = db.tbl_Wilayah.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).Select(s => s.fld_ID).Take(1).FirstOrDefault();
            LadangIDList = new SelectList(db.tbl_Ladang.Where(x => x.fld_WlyhID == wlyhID && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
            LadangIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));

            //if (WilayahID == 0 && LadangID == 0)
            //{
            //    wlyhid = GetWilayah.GetWilayahID(SyarikatID);
            //    //mywlyid = String.Join("", wlyhid); ;
            //    WilayahIDList = new SelectList(db.tbl_Wilayah.Where(x => mywlyid.Contains(x.fld_ID.ToString())).OrderBy(o => o.fld_ID), "fld_ID", "fld_WlyhName").ToList();
            //    WilayahIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
            //    var wlyhID = db.tbl_Wilayah.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).Select(s => s.fld_ID).Take(1).FirstOrDefault();
            //    LadangIDList = new SelectList(db.tbl_Ladang.Where(x => x.fld_WlyhID == wlyhID && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
            //    LadangIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
            //    //selectionrpt = 1;
            //    //wilayahselection = WilayahID;
            //    //ladangselection = LadangID;
            //    //incldg = 1;
            //    //getflag = 1;
            //}
            //else if (WilayahID != 0 && LadangID == 0)
            //{
            //    //mywlyid = String.Join("", WilayahID); ;
            //    WilayahIDList = new SelectList(db.tbl_Wilayah.Where(x => mywlyid.Contains(x.fld_ID.ToString())), "fld_ID", "fld_WlyhName").ToList();
            //    LadangIDList = new SelectList(db.tbl_Ladang.Where(x => mywlyid.Contains(x.fld_WlyhID.ToString()) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
            //    LadangIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
            //    //selectionrpt = 2;
            //    //wilayahselection = WilayahID;
            //    //ladangselection = LadangID;
            //    //incldg = 2;
            //    //getflag = 1;

            //}
            //else if (WilayahID != 0 && LadangID != 0)
            //{
            //    //mywlyid = String.Join("", WilayahID); ;
            //    WilayahIDList = new SelectList(db.tbl_Wilayah.Where(x => mywlyid.Contains(x.fld_ID.ToString())), "fld_ID", "fld_WlyhName").ToList();
            //    LadangIDList = new SelectList(db.tbl_Ladang.Where(x => mywlyid.Contains(x.fld_WlyhID.ToString()) && x.fld_ID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
            //    //selectionrpt = 3;
            //    //wilayahselection = WilayahID;
            //    //ladangselection = LadangID;
            //    //incldg = 3;
            //    //getflag = 1;
            //}

            MVC_SYSTEM_Viewing dbview = new MVC_SYSTEM_Viewing();
            ViewBag.GeneralConfig = "class = active";
            ViewBag.Dropdown2 = "dropdown open active";
            int pageSize = int.Parse(GetConfig.GetData("paging"));
            var records = new PagedList<ViewingModels.tbl_Pembekal>();
            if (GetIdentity.MySuperAdmin(User.Identity.Name))
            {
                records.Content = dbview.tbl_Pembekal.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_Deleted == false)
                    .OrderBy(sort + " " + sortdir)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                records.TotalRecords = dbview.tbl_Pembekal.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_Deleted == false).Count();
                records.CurrentPage = page;
                records.PageSize = pageSize;
            }

            ViewBag.WilayahIDList = WilayahIDList;
            ViewBag.LadangIDList = LadangIDList;
            return View(records);
        }

        public ActionResult SupplierDetail(string filter = "", int fldUserID = 0, int page = 1, string sort = "fld_KodPbkl", string sortdir = "ASC", int id = 0)
        {

            MVC_SYSTEM_Viewing dbview = new MVC_SYSTEM_Viewing();
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            int? NegaraID = 0;
            int? SyarikatID = 0;
            int? WilayahID = 0;
            int? LadangID = 0;
            ViewBag.User = "class = active";
            ViewBag.Dropdown2 = "dropdown open active";
            int pageSize = int.Parse(GetConfig.GetData("paging"));
            var records = new PagedList<ViewingModels.tbl_Pembekal>();
            ViewBag.filter = filter;
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            if (filter == "")
            {
                if (GetIdentity.MySuperAdmin(User.Identity.Name))
                {
                    records.Content = dbview.tbl_Pembekal.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_Deleted == false)
                        .OrderBy(sort + " " + sortdir)
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();

                    records.TotalRecords = dbview.tbl_Pembekal.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_Deleted == false).Count();
                    records.CurrentPage = page;
                    records.PageSize = pageSize;
                }
                else
                {
                    records.Content = dbview.tbl_Pembekal.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_Deleted == false)
                        .OrderBy(sort + " " + sortdir)
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();

                    records.TotalRecords = dbview.tbl_Pembekal.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_Deleted == false).Count();
                    records.CurrentPage = page;
                    records.PageSize = pageSize;
                }
            }
            else
            {
                if (GetIdentity.MySuperAdmin(User.Identity.Name))
                {
                    records.Content = dbview.tbl_Pembekal.Where(x => (x.fld_KodPbkl.Contains(filter) || x.fld_NamaPbkl.Contains(filter)) && x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_Deleted == false)
                    .OrderBy(sort + " " + sortdir)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                    records.TotalRecords = dbview.tbl_Pembekal.Where(x => (x.fld_KodPbkl.Contains(filter) || x.fld_NamaPbkl.Contains(filter)) && x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_Deleted == false).Count();
                    records.CurrentPage = page;
                    records.PageSize = pageSize;
                }
                else
                {
                    records.Content = dbview.tbl_Pembekal.Where(x => (x.fld_KodPbkl.Contains(filter) || x.fld_NamaPbkl.Contains(filter)) && x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_Deleted == false)
                    .OrderBy(sort + " " + sortdir)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                    records.TotalRecords = dbview.tbl_Pembekal.Where(x => (x.fld_KodPbkl.Contains(filter) || x.fld_NamaPbkl.Contains(filter)) && x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_Deleted == false).Count();
                    records.CurrentPage = page;
                    records.PageSize = pageSize;
                }
            }
            return View(records);
        }

        public ActionResult SupplierDetailInsert()
        {
            int negaraid = 0;

            ViewBag.fld_NegaraID = new SelectList(db2.tbl_Negara.Where(x => x.fld_Deleted == false), "fld_NegaraID", "fld_NamaNegara");
            negaraid = db2.tbl_Negara.Where(x => x.fld_Deleted == false).Select(s => s.fld_NegaraID).Take(1).FirstOrDefault();
            ViewBag.fld_SyarikatID = new SelectList(db.tbl_Syarikat.Where(x => x.fld_NegaraID == negaraid && x.fld_Deleted == false), "fld_SyarikatID", "fld_NamaSyarikat");

            return PartialView("SupplierDetailInsert");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SupplierDetailInsert(ModelsCorporate.tbl_Pembekal tbl_Pembekal)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var checkdata = db.tbl_Pembekal.Where(x => x.fld_KodPbkl == tbl_Pembekal.fld_KodPbkl).FirstOrDefault();
                    if (checkdata == null)
                    {
                        tbl_Pembekal.fld_NamaPbkl = tbl_Pembekal.fld_NamaPbkl.ToUpper();
                        tbl_Pembekal.fld_NegaraID = tbl_Pembekal.fld_NegaraID;
                        tbl_Pembekal.fld_SyarikatID = tbl_Pembekal.fld_SyarikatID;
                        tbl_Pembekal.fld_Deleted = false;
                        db.tbl_Pembekal.Add(tbl_Pembekal);
                        db.SaveChanges();
                        var getid = db.tbl_Pembekal.Where(w => w.fld_KodPbkl == tbl_Pembekal.fld_KodPbkl).FirstOrDefault();
                        return Json(new { success = true, msg = "Data successfully added.", status = "success", checkingdata = "0", method = "1", getid = getid, data1 = tbl_Pembekal.fld_KodPbkl, data2 = tbl_Pembekal.fld_KodPbkl, data3 = "" });
                    }
                    else
                    {
                        return Json(new { success = true, msg = "Data already exist.", status = "warning", checkingdata = "1" });
                    }

                }
                catch (Exception ex)
                {
                    geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                    return Json(new { success = true, msg = "Error occur please contact IT.", status = "danger", checkingdata = "1" });
                }
            }
            else
            {
                return Json(new { success = true, msg = "Please check fill you inserted.", status = "warning", checkingdata = "1" });
            }
        }

        public ActionResult SupplierDetailUpdate(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("SupplierDetail");
            }
            ModelsCorporate.tbl_Pembekal tbl_Pembekal = db.tbl_Pembekal.Find(id);
            if (tbl_Pembekal == null)
            {
                return RedirectToAction("SupplierDetail");
            }

            ViewBag.NegaraList = new SelectList(db2.tbl_Negara.Where(x => x.fld_Deleted == false && x.fld_NegaraID == tbl_Pembekal.fld_NegaraID), "fld_NegaraID", "fld_NamaNegara");
            ViewBag.SyarikatList = new SelectList(db.tbl_Syarikat.Where(x => x.fld_NegaraID == tbl_Pembekal.fld_NegaraID && x.fld_Deleted == false), "fld_SyarikatID", "fld_NamaSyarikat");
            return PartialView("SupplierDetailUpdate", tbl_Pembekal);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SupplierDetailUpdate(int id, Models.tbl_Pembekal tbl_Pembekal)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var getdata = db.tbl_Pembekal.Find(id);
                    //getdata.fldUserPassword = crypto.Encrypt(tblUser.fldUserPassword);
                    getdata.fld_NamaPbkl = tbl_Pembekal.fld_NamaPbkl;

                    db.Entry(getdata).State = EntityState.Modified;
                    db.SaveChanges();
                    var getid = id;
                    return Json(new { success = true, msg = "Data successfully edited.", status = "success", checkingdata = "0", method = "1", getid = getid, data1 = "", data2 = "" });
                }
                catch (Exception ex)
                {
                    geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                    return Json(new { success = true, msg = "Error occur please contact IT.", status = "danger", checkingdata = "1" });
                }
            }
            else
            {
                return Json(new { success = true, msg = "Please check fill you inserted.", status = "warning", checkingdata = "1" });
            }
        }

        public ActionResult SupplierDetailDelete(int? id)
        {
            string negara = "";
            string syarikat = "";

            if (id == null)
            {
                return RedirectToAction("SupplierDetail");
            }
            ModelsCorporate.tbl_Pembekal tbl_Pembekal = db.tbl_Pembekal.Find(id);
            if (tbl_Pembekal == null)
            {
                return RedirectToAction("SupplierDetail");
            }
            else
            {
                negara = db2.tbl_Negara.Where(x => x.fld_NegaraID == tbl_Pembekal.fld_NegaraID).Select(s => s.fld_NamaNegara).FirstOrDefault();
                syarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == tbl_Pembekal.fld_SyarikatID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
            }
            ViewBag.CountryName = negara;
            ViewBag.CompanyName = syarikat;
            return PartialView("SupplierDetailDelete", tbl_Pembekal);
        }

        [HttpPost, ActionName("SupplierDetailDelete")]
        [ValidateAntiForgeryToken]
        public ActionResult SupplierDetailDeleteConfirmed(int id)
        {
            try
            {
                ModelsCorporate.tbl_Pembekal tbl_Pembekal = db.tbl_Pembekal.Find(id);
                if (tbl_Pembekal == null)
                {
                    return Json(new { success = true, msg = "Data already deleted.", status = "success", checkingdata = "0", method = "1", getid = "", data1 = "", data2 = "" });
                }
                else
                {
                    db.tbl_Pembekal.Remove(tbl_Pembekal);
                    db.SaveChanges();
                    return Json(new { success = true, msg = "Data successfully deleted.", status = "success", checkingdata = "0", method = "1", getid = "", data1 = "", data2 = "" });
                }

            }
            catch (Exception ex)
            {
                geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                return Json(new { success = true, msg = "Error occur please contact IT.", status = "danger", checkingdata = "1" });
            }
        }

        [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1")]
        public ActionResult Region(string filter = "", int fldUserID = 0, int page = 1, string sort = "fld_WlyhName", string sortdir = "DESC")
        {
            MVC_SYSTEM_Viewing dbview = new MVC_SYSTEM_Viewing();
            ViewBag.GeneralConfig = "class = active";
            ViewBag.Dropdown2 = "dropdown open active";
            int pageSize = int.Parse(GetConfig.GetData("paging"));
            var records = new PagedList<ViewingModels.tbl_Wilayah>();
            ViewBag.filter = filter;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            int? NegaraID = 0;
            int? SyarikatID = 0;
            int? WilayahID = 0;
            int? LadangID = 0;

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            if (filter == "")
            {
                records.Content = dbview.tbl_Wilayah.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false)
                    .OrderBy(sort + " " + sortdir)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                records.TotalRecords = dbview.tbl_Wilayah.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).Count();
                records.CurrentPage = page;
                records.PageSize = pageSize;
            }
            else
            {
                records.Content = dbview.tbl_Wilayah.Where(x => (x.fld_WlyhName.Contains(filter) || x.fld_WlyhEmail.Contains(filter)) && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false)
                .OrderBy(sort + " " + sortdir)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

                records.TotalRecords = dbview.tbl_Wilayah.Where(x => (x.fld_WlyhName.Contains(filter) || x.fld_WlyhEmail.Contains(filter)) && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).Count();
                records.CurrentPage = page;
                records.PageSize = pageSize;
            }
            return View(records);
        }

        public ActionResult RegionInsert()
        {
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            int? NegaraID = 0;
            int? SyarikatID = 0;
            int? WilayahID = 0;
            int? LadangID = 0;
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            ViewBag.fld_SyarikatID = new SelectList(db.tbl_Syarikat.Where(x => x.fld_NegaraID == NegaraID && x.fld_Deleted == false), "fld_SyarikatID", "fld_NamaSyarikat").ToList();
            return PartialView("RegionInsert");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RegionInsert(ModelsCorporate.tbl_Wilayah tbl_Wilayah)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var checkdata = db.tbl_Wilayah.Where(x => x.fld_WlyhName == tbl_Wilayah.fld_WlyhName).FirstOrDefault();
                    if (checkdata == null)
                    {
                        tbl_Wilayah.fld_WlyhName = tbl_Wilayah.fld_WlyhName.ToUpper();
                        //tbl_Wilayah.fld_SyarikatID = 0;
                        //tbl_Wilayah.fldUserPassword = crypto.Encrypt(tbl_Wilayah.fldUserPassword);
                        tbl_Wilayah.fld_Deleted = false;
                        db.tbl_Wilayah.Add(tbl_Wilayah);
                        db.SaveChanges();
                        var getid = db.tbl_Wilayah.Where(w => w.fld_WlyhName == tbl_Wilayah.fld_WlyhName).FirstOrDefault();
                        return Json(new { success = true, msg = "Data successfully added.", status = "success", checkingdata = "0", method = "1", getid = getid, data1 = tbl_Wilayah.fld_WlyhName, data2 = tbl_Wilayah.fld_WlyhName, data3 = "" });
                    }
                    else
                    {
                        return Json(new { success = true, msg = "Data already exist.", status = "warning", checkingdata = "1" });
                    }

                }
                catch (Exception ex)
                {
                    geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                    return Json(new { success = true, msg = "Error occur please contact IT.", status = "danger", checkingdata = "1" });
                }
            }
            else
            {
                return Json(new { success = true, msg = "Please check fill you inserted.", status = "warning", checkingdata = "1" });
            }
        }

        public ActionResult RegionUpdate(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Region");
            }
            ModelsCorporate.tbl_Wilayah tbl_Wilayah = db.tbl_Wilayah.Find(id);
            if (tbl_Wilayah == null)
            {
                return RedirectToAction("Region");
            }
            return PartialView("RegionUpdate", tbl_Wilayah);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RegionUpdate(int id, Models.tbl_Wilayah tbl_Wilayah)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var getdata = db.tbl_Wilayah.Find(id);
                    getdata.fld_WlyhEmail = tbl_Wilayah.fld_WlyhEmail;
                    db.Entry(getdata).State = EntityState.Modified;
                    db.SaveChanges();
                    var getid = id;
                    return Json(new { success = true, msg = "Data successfully edited.", status = "success", checkingdata = "0", method = "1", getid = getid, data1 = "", data2 = "" });
                }
                catch (Exception ex)
                {
                    geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                    return Json(new { success = true, msg = "Error occur please contact IT.", status = "danger", checkingdata = "1" });
                }
            }
            else
            {
                return Json(new { success = true, msg = "Please check fill you inserted.", status = "warning", checkingdata = "1" });
            }
        }

        public ActionResult RegionDelete(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Region");
            }
            ModelsCorporate.tbl_Wilayah tbl_Wilayah = db.tbl_Wilayah.Find(id);
            if (tbl_Wilayah == null)
            {
                return RedirectToAction("Region");
            }
            return PartialView("RegionDelete", tbl_Wilayah);
        }

        [HttpPost, ActionName("RegionDelete")]
        [ValidateAntiForgeryToken]
        public ActionResult RegionDeleteConfirmed(int id)
        {
            try
            {
                ModelsCorporate.tbl_Wilayah tbl_Wilayah = db.tbl_Wilayah.Find(id);
                if (tbl_Wilayah == null)
                {
                    return Json(new { success = true, msg = "Data already deleted.", status = "success", checkingdata = "0", method = "1", getid = "", data1 = "", data2 = "" });
                }
                else
                {
                    db.tbl_Wilayah.Remove(tbl_Wilayah);
                    db.SaveChanges();
                    return Json(new { success = true, msg = "Data successfully deleted.", status = "success", checkingdata = "0", method = "1", getid = "", data1 = "", data2 = "" });
                }

            }
            catch (Exception ex)
            {
                geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                return Json(new { success = true, msg = "Error occur please contact IT.", status = "danger", checkingdata = "1" });
            }
        }
    }
}