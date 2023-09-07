using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Web;
using System.Web.Mvc;
using MVC_SYSTEM.Class;
using MVC_SYSTEM.log;
using MVC_SYSTEM.ModelsCorporate;
using MVC_SYSTEM.ViewingModels;

namespace MVC_SYSTEM.Controllers
{
    public class IntegrationDataController : Controller
    {
        private MVC_SYSTEM_ModelsCorporate db = new MVC_SYSTEM_ModelsCorporate();
        private GetIdentity GetIdentity = new GetIdentity();
        private GetConfig GetConfig = new GetConfig();
        private GetNSWL GetNSWL = new GetNSWL();
        private Connection Connection = new Connection();
        private ChangeTimeZone timezone = new ChangeTimeZone();
        errorlog geterror = new errorlog();
        private GlobalFunction GlobalFunction = new GlobalFunction();
        GetWilayah getwilyah = new GetWilayah();

        // GET: Maintenance
        public ActionResult Index()
        {
            ViewBag.IntegrationDemo = "class = active";
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            List<SelectListItem> sublist = new List<SelectListItem>();
            ViewBag.IntegrationDemoSubList = sublist;
            ViewBag.IntegrationDemo = "class = active";
            //ViewBag.IntegrationDemoList = new SelectList(db.tblMenuLists.Where(x => x.fld_Flag == "integrationData" && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_Desc }), "Value", "Text").ToList();
            // edited by fitri 08-09-2020
            ViewBag.IntegrationDemoList = new SelectList(db.tblMenuLists.Where(x => x.fld_Flag == "integrationDemo" && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_Desc }), "Value", "Text").ToList();
            db.Dispose();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(string IntegrationDemoList, string IntegrationDemoSubList)
        {
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            if (IntegrationDemoSubList != null)
            {
                return RedirectToAction(IntegrationDemoSubList, "IntegrationData");
            }
            else
            {
                int integrationdemolist = int.Parse(IntegrationDemoList);
                var action = db.tblMenuLists.Where(x => x.fld_ID == integrationdemolist && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).Select(s => s.fld_Val).FirstOrDefault();
                db.Dispose();
                return RedirectToAction(action, "IntegrationData");
            }
        }

        public JsonResult GetSubList(int ListID)
        {
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            var findsub = db.tblMenuLists.Where(x => x.fld_ID == ListID).Select(s => s.fld_Sub).FirstOrDefault();
            List<SelectListItem> sublist = new List<SelectListItem>();
            if (findsub != null)
            {
                sublist = new SelectList(db.tblMenuLists.Where(x => x.fld_Flag == findsub && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).OrderBy(o => o.fld_ID).Select(s => new SelectListItem { Value = s.fld_Val, Text = s.fld_Desc }), "Value", "Text").ToList();
            }
            db.Dispose();
            return Json(sublist);
        }

        public ActionResult costCenter(string filter, int page = 1, string sort = "fld_CostCenter",
            string sortdir = "ASC")
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            ViewBag.IntegrationDemo = "class = active";

            return View();
        }

        public ActionResult _costCenter(string filter, int page = 1,
            string sort = "fld_ModifiedDt",
            string sortdir = "DESC")
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            int pageSize = int.Parse(GetConfig.GetData("paging"));
            var records = new PagedList<ModelsCorporate.tbl_SAPCCPUP>();
            int role = GetIdentity.RoleID(getuserid).Value;

            var ccData = db.tbl_SAPCCPUP
                .Where(x => x.fld_NegaraID == NegaraID &&
                            x.fld_SyarikatID == SyarikatID);

            if (!String.IsNullOrEmpty(filter))
            {
                records.Content = ccData
                    .Where(x => x.fld_CostCenter.ToUpper().Contains(filter.ToUpper()) ||
                                x.fld_CostCenterDesc.ToUpper().Contains(filter.ToUpper()))
                    .OrderBy(sort + " " + sortdir)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                records.TotalRecords = ccData
                    .Count(x => x.fld_CostCenter.ToUpper().Contains(filter.ToUpper()) ||
                                x.fld_CostCenterDesc.ToUpper().Contains(filter.ToUpper()));
            }

            else
            {
                records.Content = ccData.OrderBy(sort + " " + sortdir)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                records.TotalRecords = ccData
                    .Count();
            }

            records.CurrentPage = page;
            records.PageSize = pageSize;
            ViewBag.RoleID = role;
            ViewBag.pageSize = 1;

            return View(records);
        }

        public ActionResult GeneralLedger(string filter, int page = 1, string sort = "fld_ModifiedDt",
            string sortdir = "DESC")
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            ViewBag.IntegrationDemo = "class = active";

            return View();
        }

        public ActionResult _GeneralLedger(string filter, int page = 1,
            string sort = "fld_ModifiedDt",
            string sortdir = "DESC")
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            int pageSize = int.Parse(GetConfig.GetData("paging"));
            var records = new PagedList<ModelsCorporate.tbl_SAPGLPUP>();
            int role = GetIdentity.RoleID(getuserid).Value;

            var glData = db.tbl_SAPGLPUP
                .Where(x => x.fld_NegaraID == NegaraID &&
                            x.fld_SyarikatID == SyarikatID);

            if (!String.IsNullOrEmpty(filter))
            {
                records.Content = glData
                    .Where(x => x.fld_GLCode.ToUpper().Contains(filter.ToUpper()) ||
                                x.fld_GLDesc.ToUpper().Contains(filter.ToUpper()))
                    .OrderBy(sort + " " + sortdir)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                records.TotalRecords = glData
                    .Count(x => x.fld_GLCode.ToUpper().Contains(filter.ToUpper()) ||
                                x.fld_GLDesc.ToUpper().Contains(filter.ToUpper()));
            }

            else
            {
                records.Content = glData.OrderBy(sort + " " + sortdir)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                records.TotalRecords = glData
                    .Count();
            }

            records.CurrentPage = page;
            records.PageSize = pageSize;
            ViewBag.RoleID = role;
            ViewBag.pageSize = 1;

            return View(records);
        }

        public ActionResult Vendor(string filter, int page = 1, string sort = "fld_ModifiedDt",
            string sortdir = "DESC")
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            ViewBag.IntegrationDemo = "class = active";

            return View();
        }

        public ActionResult _Vendor(string filter, int page = 1,
            string sort = "fld_ModifiedDt",
            string sortdir = "DESC")
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            int pageSize = int.Parse(GetConfig.GetData("paging"));
            var records = new PagedList<ModelsCorporate.tbl_SAPVendorPUP>();
            int role = GetIdentity.RoleID(getuserid).Value;

            var vendorData = db.tbl_SAPVendorPUP
                .Where(x => x.fld_NegaraID == NegaraID &&
                            x.fld_SyarikatID == SyarikatID);

            if (!String.IsNullOrEmpty(filter))
            {
                records.Content = vendorData
                    .Where(x => x.fld_VendorCode.ToUpper().Contains(filter.ToUpper()) ||
                                x.fld_VendorName.ToUpper().Contains(filter.ToUpper()))
                    .OrderBy(sort + " " + sortdir)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                records.TotalRecords = vendorData
                    .Count(x => x.fld_VendorCode.ToUpper().Contains(filter.ToUpper()) ||
                                x.fld_VendorName.ToUpper().Contains(filter.ToUpper()));
            }

            else
            {
                records.Content = vendorData.OrderBy(sort + " " + sortdir)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                records.TotalRecords = vendorData
                    .Count();
            }

            records.CurrentPage = page;
            records.PageSize = pageSize;
            ViewBag.RoleID = role;
            ViewBag.pageSize = 1;

            return View(records);
        }

        public ActionResult Customer(string filter, int page = 1, string sort = "fld_ModifiedDt",
            string sortdir = "DESC")
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            ViewBag.IntegrationDemo = "class = active";

            return View();
        }

        public ActionResult _Customer(string filter, int page = 1,
            string sort = "fld_CustomerCode",
            string sortdir = "ASC")
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            int pageSize = int.Parse(GetConfig.GetData("paging"));
            var records = new PagedList<ModelsCorporate.tbl_SAPCustomerPUP>();
            int role = GetIdentity.RoleID(getuserid).Value;

            var customerData = db.tbl_SAPCustomerPUP
                .Where(x => x.fld_NegaraID == NegaraID &&
                            x.fld_SyarikatID == SyarikatID);

            if (!String.IsNullOrEmpty(filter))
            {
                records.Content = customerData
                    .Where(x => x.fld_CustomerCode.ToUpper().Contains(filter.ToUpper()) ||
                                x.fld_CustomerName.ToUpper().Contains(filter.ToUpper()))
                    .OrderBy(sort + " " + sortdir)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                records.TotalRecords = customerData
                    .Count(x => x.fld_CustomerCode.ToUpper().Contains(filter.ToUpper()) ||
                                x.fld_CustomerName.ToUpper().Contains(filter.ToUpper()));
            }

            else
            {
                records.Content = customerData.OrderBy(sort + " " + sortdir)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                records.TotalRecords = customerData
                    .Count();
            }

            records.CurrentPage = page;
            records.PageSize = pageSize;
            ViewBag.RoleID = role;
            ViewBag.pageSize = 1;

            return View(records);
        }

        public ActionResult PDCustom(string filter, int page = 1, string sort = "fld_ModifiedDt",
            string sortdir = "DESC")
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            ViewBag.IntegrationDemo = "class = active";

            return View();
        }

        public ActionResult _PDCustom(string filter, int page = 1,
            string sort = "fld_ModifiedDt",
            string sortdir = "DESC")
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            int pageSize = int.Parse(GetConfig.GetData("paging"));
            var records = new PagedList<ModelsCorporate.tbl_SAPCUSTOMPUP>();
            int role = GetIdentity.RoleID(getuserid).Value;

            var pdCustomData = db.tbl_SAPCUSTOMPUP
                .Where(x => x.fld_NegaraID == NegaraID &&
                            x.fld_SyarikatID == SyarikatID);

            if (!String.IsNullOrEmpty(filter))
            {
                records.Content = pdCustomData
                    .Where(x => x.fld_WilayahID.ToString().ToUpper().Contains(filter.ToUpper()))
                    .OrderBy(sort + " " + sortdir)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                records.TotalRecords = pdCustomData
                    .Count(x => x.fld_WilayahID.ToString().ToUpper().Contains(filter.ToUpper()));
            }

            else
            {
                records.Content = pdCustomData.OrderBy(sort + " " + sortdir)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                records.TotalRecords = pdCustomData
                    .Count();
            }

            records.CurrentPage = page;
            records.PageSize = pageSize;
            ViewBag.RoleID = role;
            ViewBag.pageSize = 1;

            return View(records);
        }

        public ActionResult PD(string filter, int page = 1, string sort = "fld_ModifiedDt",
            string sortdir = "DESC")
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            ViewBag.IntegrationDemo = "class = active";

            return View();
        }

        public ActionResult _PD(string filter, int page = 1,
            string sort = "fld_ModifiedDt",
            string sortdir = "DESC")
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            int pageSize = int.Parse(GetConfig.GetData("paging"));
            var records = new PagedList<ModelsCorporate.tbl_SAPPDPUP>();
            int role = GetIdentity.RoleID(getuserid).Value;

            var pdData = db.tbl_SAPPDPUP
                .Where(x => x.fld_NegaraID == NegaraID &&
                            x.fld_SyarikatID == SyarikatID);

            if (!String.IsNullOrEmpty(filter))
            {
                records.Content = pdData
                    .Where(x => x.fld_ProjectDefinition.ToUpper().Contains(filter.ToUpper()) ||
                                x.fld_ProjectDesc.ToUpper().Contains(filter.ToUpper()))
                    .OrderBy(sort + " " + sortdir)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                records.TotalRecords = pdData
                    .Count(x => x.fld_ProjectDefinition.ToUpper().Contains(filter.ToUpper()) ||
                                x.fld_ProjectDesc.ToUpper().Contains(filter.ToUpper()));
            }

            else
            {
                records.Content = pdData.OrderBy(sort + " " + sortdir)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                records.TotalRecords = pdData
                    .Count();
            }

            records.CurrentPage = page;
            records.PageSize = pageSize;
            ViewBag.RoleID = role;
            ViewBag.pageSize = 1;

            return View(records);
        }
    }
}