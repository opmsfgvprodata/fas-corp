using MVC_SYSTEM.App_LocalResources;
using MVC_SYSTEM.Attributes;
using MVC_SYSTEM.Class;
using MVC_SYSTEM.log;
using MVC_SYSTEM.ModelsCorporate;
using MVC_SYSTEM.ModelsCustom;
using MVC_SYSTEM.ModelsEstate;
using MVC_SYSTEM.ViewingModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVC_SYSTEM.Controllers
{
    [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3")]
    public class EstateMaintenanceController : Controller
    {
        private MVC_SYSTEM_ModelsCorporate db = new MVC_SYSTEM_ModelsCorporate();
        private MVC_SYSTEM_ModelsEstate dbE = new MVC_SYSTEM_ModelsEstate();
        private GetIdentity GetIdentity = new GetIdentity();
        private GetConfig GetConfig = new GetConfig();
        private GetNSWL GetNSWL = new GetNSWL();
        private Connection Connection = new Connection();
        private ChangeTimeZone timezone = new ChangeTimeZone();
        errorlog geterror = new errorlog();
        private GlobalFunction GlobalFunction = new GlobalFunction();
        GetWilayah getwilyah = new GetWilayah();
        // GET: EstateMaintenance
        public ActionResult Index()
        {
            ViewBag.EstateMaintenance = "class = active";
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            List<SelectListItem> sublist = new List<SelectListItem>();
            ViewBag.MaintenanceSubList = sublist;
            ViewBag.MaintenanceList = new SelectList(db.tblMenuLists.Where(x => x.fld_Flag == "estatemaintenance" && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID)
                .Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_Desc }), "Value", "Text").ToList();
            db.Dispose();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(string MaintenanceList)
        {
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            int maintenancelist = int.Parse(MaintenanceList);
            var action = db.tblMenuLists.Where(x => x.fld_ID == maintenancelist && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).Select(s => s.fld_Val).FirstOrDefault();
            db.Dispose();
            return RedirectToAction(action, "EstateMaintenance");
        }

        public ActionResult EstateLevelTemporaryManagement()
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            ViewBag.EstateMaintenance = "class = active";

            List<SelectListItem> wilayahList = new List<SelectListItem>();
            wilayahList = new SelectList(
                db.tbl_Wilayah
                    .Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).OrderBy(o => o.fld_WlyhName)
                    .Select(
                        s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_WlyhName }), "Value", "Text").ToList();
            wilayahList.Insert(0, new SelectListItem { Text = GlobalResCorp.lblChoose, Value = "" });
            ViewBag.WilayahList = wilayahList;

            List<SelectListItem> ladangList = new List<SelectListItem>();
            ladangList.Insert(0, new SelectListItem { Text = GlobalResCorp.lblChoose, Value = "" });
            ViewBag.LadangList = ladangList;

            return View();
        }

        public ActionResult _EstateLevelTemporaryManagementList(int? WilayahList, int? LadangList, int page = 1, string sortdir = "ASC")
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            int pageSize = int.Parse(GetConfig.GetData("paging"));
            var records = new PagedList<tbl_PktPinjam>();
            int role = GetIdentity.RoleID(getuserid).Value;
            var message = "";
            
            List<tbl_PktPinjam> GetTransferPkt = new List<tbl_PktPinjam>();

            if (!String.IsNullOrEmpty(WilayahList.ToString()) && !String.IsNullOrEmpty(LadangList.ToString()))
            {
                Connection.GetConnection(out host, out catalog, out user, out pass, WilayahList, SyarikatID.Value, NegaraID.Value);
                MVC_SYSTEM_ModelsEstate estateConnection = MVC_SYSTEM_ModelsEstate.ConnectToSqlServer(host, catalog, user, pass);
                GetTransferPkt = estateConnection.tbl_PktPinjam.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahList && x.fld_LadangID == LadangList).ToList();
                message = GlobalResCorp.msgNoRecord;
            }

            else
            {
                message = "Please select to search the data";
            }

            records.Content = GetTransferPkt;
            records.TotalRecords = GetTransferPkt.Count();
            records.CurrentPage = page;
            records.PageSize = pageSize;
            ViewBag.RoleID = role;
            ViewBag.pageSize = 1;
            ViewBag.Message = message;

            return View(records);
        }

        public ActionResult _EstateLevelTemporaryManagementCreate()
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            ViewBag.EstateMaintenance = "class = active";

            var GetWilayah = db.tbl_Wilayah
                    .Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).OrderBy(o => o.fld_WlyhName).ToList();

            List<SelectListItem> wilayahList1 = new List<SelectListItem>();
            wilayahList1 = new SelectList(
                GetWilayah.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).OrderBy(o => o.fld_WlyhName)
                    .Select(
                        s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_WlyhName }), "Value", "Text").ToList();
            wilayahList1.Insert(0, new SelectListItem { Text = GlobalResCorp.lblChoose, Value = "" });
            ViewBag.WilayahList1 = wilayahList1;

            List<SelectListItem> ladangList1 = new List<SelectListItem>();
            ladangList1.Insert(0, new SelectListItem { Text = GlobalResCorp.lblChoose, Value = "" });
            ViewBag.LadangList1 = ladangList1;

            List<SelectListItem> JnisAktvt = new List<SelectListItem>();
            var GetJenisAktvtyDesc = db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "activityLevel" && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).Select(s => new { s.fldOptConfValue, s.fldOptConfDesc }).ToList();JnisAktvt = new SelectList(GetJenisAktvtyDesc.Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();
            JnisAktvt.Insert(0, (new SelectListItem { Text = GlobalResCorp.lblChoose, Value = "" }));
            ViewBag.JnisAktvt = JnisAktvt;

            List<SelectListItem> JnisPkt = new List<SelectListItem>();
            JnisPkt = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "jnspkt" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();
            JnisPkt.Insert(0, new SelectListItem { Text = GlobalResCorp.lblChoose, Value = "" });
            ViewBag.JnisPkt = JnisPkt;

            List<SelectListItem> PilihanPkt = new List<SelectListItem>();
            PilihanPkt.Insert(0, new SelectListItem { Text = GlobalResCorp.lblChoose, Value = "" });
            ViewBag.PilihanPkt = PilihanPkt;

            List<SelectListItem> wilayahList2 = new List<SelectListItem>();
            wilayahList2 = new SelectList(
                GetWilayah.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).OrderBy(o => o.fld_WlyhName)
                    .Select(
                        s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_WlyhName }), "Value", "Text").ToList();
            wilayahList2.Insert(0, new SelectListItem { Text = GlobalResCorp.lblChoose, Value = "" });
            ViewBag.wilayahList2 = wilayahList2;

            List<SelectListItem> ladangList2 = new List<SelectListItem>();
            ladangList2.Insert(0, new SelectListItem { Text = GlobalResCorp.lblChoose, Value = "" });
            ViewBag.ladangList2 = ladangList2;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult _EstateLevelTemporaryManagementCreate(CustMod_TransferPkt CustMod_TransferPkt)
        {
            if (CustMod_TransferPkt.ladangList1 != CustMod_TransferPkt.ladangList2)
            {
                int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
                int? getuserid = GetIdentity.ID(User.Identity.Name);
                DateTime? CurrentDT = timezone.gettimezone();
                string host, catalog, user, pass = "";
                GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
                Connection.GetConnection(out host, out catalog, out user, out pass, CustMod_TransferPkt.wilayahList1, SyarikatID.Value, NegaraID.Value);
                MVC_SYSTEM_ModelsEstate EstateConnFrom = MVC_SYSTEM_ModelsEstate.ConnectToSqlServer(host, catalog, user, pass);

                Connection.GetConnection(out host, out catalog, out user, out pass, CustMod_TransferPkt.wilayahList2, SyarikatID.Value, NegaraID.Value);
                MVC_SYSTEM_ModelsEstate EstateConnTo = MVC_SYSTEM_ModelsEstate.ConnectToSqlServer(host, catalog, user, pass);
                int NegaraIDAsl = 0;
                int SyarikatIDAsal = 0;
                int WilayahIDAsal = 0;
                int LadangIDAsal = 0;
                int DivisionIDAsal = 0;
                string KodPkt = "";
                string NamaPkt = "";
                string SAPCode = "";
                string JenisLot = "";
                tbl_PktPinjam tbl_PktPinjam = new tbl_PktPinjam();
                switch (CustMod_TransferPkt.JnisPkt)
                {
                    case 1:
                        var SelectPkt = EstateConnFrom.tbl_PktUtama.Where(x => x.fld_ID == CustMod_TransferPkt.PilihanPkt && x.fld_Deleted == false).FirstOrDefault();
                        NegaraIDAsl = SelectPkt.fld_NegaraID.Value;
                        SyarikatIDAsal = SelectPkt.fld_SyarikatID.Value;
                        WilayahIDAsal = SelectPkt.fld_WilayahID.Value;
                        LadangIDAsal = SelectPkt.fld_LadangID.Value;
                        DivisionIDAsal = SelectPkt.fld_DivisionID.Value;
                        KodPkt = SelectPkt.fld_PktUtama;
                        NamaPkt = SelectPkt.fld_NamaPktUtama;
                        SAPCode = SelectPkt.fld_IOcode;
                        JenisLot = SelectPkt.fld_JnsLot;
                        break;
                    case 2:
                        var SelectPkt2 = EstateConnFrom.tbl_SubPkt.Join(EstateConnFrom.tbl_PktUtama, j => new { j.fld_NegaraID, j.fld_SyarikatID, j.fld_WilayahID, j.fld_LadangID, fld_PktUtama = j.fld_KodPktUtama }, k => new { k.fld_NegaraID, k.fld_SyarikatID, k.fld_WilayahID, k.fld_LadangID, fld_PktUtama = k.fld_PktUtama }, (j, k) => new { j.fld_NegaraID, j.fld_SyarikatID, j.fld_WilayahID, j.fld_LadangID, k.fld_JnsLot, j.fld_Pkt, j.fld_NamaPkt, j.fld_Deleted, k.fld_IOcode, k.fld_DivisionID, j.fld_ID }).Where(x => x.fld_ID == CustMod_TransferPkt.PilihanPkt && x.fld_Deleted == false).FirstOrDefault();
                        NegaraIDAsl = SelectPkt2.fld_NegaraID.Value;
                        SyarikatIDAsal = SelectPkt2.fld_SyarikatID.Value;
                        WilayahIDAsal = SelectPkt2.fld_WilayahID.Value;
                        LadangIDAsal = SelectPkt2.fld_LadangID.Value;
                        DivisionIDAsal = SelectPkt2.fld_DivisionID.Value;
                        KodPkt = SelectPkt2.fld_Pkt;
                        NamaPkt = SelectPkt2.fld_NamaPkt;
                        SAPCode = SelectPkt2.fld_IOcode;
                        JenisLot = SelectPkt2.fld_JnsLot;
                        break;
                    case 3:
                        var SelectPkt3 = EstateConnFrom.tbl_Blok.Join(EstateConnFrom.tbl_PktUtama, j => new { j.fld_NegaraID, j.fld_SyarikatID, j.fld_WilayahID, j.fld_LadangID, fld_PktUtama = j.fld_KodPktutama }, k => new { k.fld_NegaraID, k.fld_SyarikatID, k.fld_WilayahID, k.fld_LadangID, fld_PktUtama = k.fld_PktUtama }, (j, k) => new { j.fld_NegaraID, j.fld_SyarikatID, j.fld_WilayahID, j.fld_LadangID, k.fld_JnsLot, j.fld_Blok, j.fld_NamaBlok, j.fld_Deleted, k.fld_IOcode, k.fld_DivisionID, j.fld_ID }).Where(x => x.fld_ID == CustMod_TransferPkt.PilihanPkt && x.fld_Deleted == false).FirstOrDefault();
                        NegaraIDAsl = SelectPkt3.fld_NegaraID.Value;
                        SyarikatIDAsal = SelectPkt3.fld_SyarikatID.Value;
                        WilayahIDAsal = SelectPkt3.fld_WilayahID.Value;
                        LadangIDAsal = SelectPkt3.fld_LadangID.Value;
                        DivisionIDAsal = SelectPkt3.fld_DivisionID.Value;
                        KodPkt = SelectPkt3.fld_Blok;
                        NamaPkt = SelectPkt3.fld_NamaBlok;
                        SAPCode = SelectPkt3.fld_IOcode;
                        JenisLot = SelectPkt3.fld_JnsLot;
                        break;
                }

                tbl_PktPinjam.fld_JenisPkt = CustMod_TransferPkt.JnisPkt;
                tbl_PktPinjam.fld_JnsLot = JenisLot;
                tbl_PktPinjam.fld_KodPkt = KodPkt + DivisionIDAsal + "P";
                tbl_PktPinjam.fld_NamaPkt = NamaPkt + " - (Pinjam)";
                tbl_PktPinjam.fld_DivisionID = 0;
                tbl_PktPinjam.fld_LadangID = CustMod_TransferPkt.ladangList2;
                tbl_PktPinjam.fld_WilayahID = CustMod_TransferPkt.wilayahList2;
                tbl_PktPinjam.fld_SyarikatID = SyarikatID;
                tbl_PktPinjam.fld_NegaraID = NegaraID;
                tbl_PktPinjam.fld_DivisionIDAsal = DivisionIDAsal;
                tbl_PktPinjam.fld_LadangIDAsal = LadangIDAsal;
                tbl_PktPinjam.fld_WilayahIDAsal = WilayahIDAsal;
                tbl_PktPinjam.fld_SyarikatIDAsal = SyarikatID;
                tbl_PktPinjam.fld_NegaraIDAsal = NegaraIDAsl;
                tbl_PktPinjam.fld_SAPCode = SAPCode;
                tbl_PktPinjam.fld_OriginPktID = CustMod_TransferPkt.PilihanPkt;
                tbl_PktPinjam.fld_EndDT = CustMod_TransferPkt.DateEnd;
                tbl_PktPinjam.fld_CreatedBy = getuserid;
                tbl_PktPinjam.fld_CreatedDT = CurrentDT;
                EstateConnTo.tbl_PktPinjam.Add(tbl_PktPinjam);
                EstateConnTo.SaveChanges();

                EstateConnFrom.Dispose();
                EstateConnTo.Dispose();

                string appname = Request.ApplicationPath;
                string domain = Request.Url.GetLeftPart(UriPartial.Authority);
                var lang = Request.RequestContext.RouteData.Values["lang"];

                if (appname != "/")
                {
                    domain = domain + appname;
                }

                return Json(new
                {
                    success = true,
                    msg = GlobalResCorp.msgAdd,
                    status = "success",
                    checkingdata = "0",
                    method = "4",
                    div = "SearchingData",
                    rootUrl = domain,
                    action = "_EstateLevelTemporaryManagementList",
                    controller = "EstateMaintenance"
                });
            }
            else
            {
                return Json(new
                {
                    success = false,
                    msg = "Estate From and Estate To Cannot be same",
                    status = "warning",
                    checkingdata = "0"
                });
            }
        }

        public ActionResult _EstateLevelTemporaryManagementDelete(int id, int wil)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, wil, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_ModelsEstate estateConnection = MVC_SYSTEM_ModelsEstate.ConnectToSqlServer(host, catalog, user, pass);

            var GetLevelDetail = estateConnection.tbl_PktPinjam.Find(id);

            return PartialView(GetLevelDetail);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult _EstateLevelTemporaryManagementDelete(tbl_PktPinjam tbl_PktPinjam)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, tbl_PktPinjam.fld_WilayahID, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_ModelsEstate estateConnection = MVC_SYSTEM_ModelsEstate.ConnectToSqlServer(host, catalog, user, pass);

            try
            {
                var GetLevelDetail = estateConnection.tbl_PktPinjam.Find(tbl_PktPinjam.fld_ID);
                estateConnection.tbl_PktPinjam.Remove(GetLevelDetail);
                estateConnection.SaveChanges();

                string appname = Request.ApplicationPath;
                string domain = Request.Url.GetLeftPart(UriPartial.Authority);
                var lang = Request.RequestContext.RouteData.Values["lang"];

                if (appname != "/")
                {
                    domain = domain + appname;
                }

                return Json(new
                {
                    success = true,
                    msg = "Data Deleted",
                    status = "success",
                    checkingdata = "0",
                    method = "1",
                    div = "SearchingData",
                    rootUrl = domain,
                    action = "_EstateLevelTemporaryManagementList",
                    controller = "EstateMaintenance"
                });
            }

            catch (Exception ex)
            {
                geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                return Json(new
                {
                    success = false,
                    msg = GlobalResCorp.msgError,
                    status = "danger",
                    checkingdata = "0"
                });
            }

            finally
            {
                db.Dispose();
            }
        }

        public JsonResult GetLadang(int WilayahID)
        {
            List<SelectListItem> ladanglist = new List<SelectListItem>();

            int? NegaraID = 0;
            int? SyarikatID = 0;
            int? WilayahID2 = 0;
            int? LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID2, out LadangID, getuserid, User.Identity.Name);

            if (getwilyah.GetAvailableWilayah(SyarikatID))
            {
                if (WilayahID == 0)
                {
                    ladanglist = new SelectList(db.vw_NSWL.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_Deleted_L == false).OrderBy(o => o.fld_NamaLadang).Select(s => new SelectListItem { Value = s.fld_LadangID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_NamaLadang }), "Value", "Text").ToList();
                }
                else
                {
                    ladanglist = new SelectList(db.vw_NSWL.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_Deleted_L == false).OrderBy(o => o.fld_NamaLadang).Select(s => new SelectListItem { Value = s.fld_LadangID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_NamaLadang }), "Value", "Text").ToList();
                }
            }

            return Json(ladanglist);
        }

        public JsonResult GetPkt(string JnisAktvt, byte JnsPkt, int WilayahList, int LadangList)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahList, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_ModelsEstate estateConnection = MVC_SYSTEM_ModelsEstate.ConnectToSqlServer(host, catalog, user, pass);
            List<SelectListItem> PilihPeringkat = new List<SelectListItem>();
            switch (JnsPkt)
            {
                case 1:
                    var SelectPkt = estateConnection.tbl_PktUtama.Where(x => x.fld_JnsLot == JnisAktvt && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahList && x.fld_LadangID == LadangList && x.fld_Deleted == false).ToList();
                    PilihPeringkat = new SelectList(SelectPkt.Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_PktUtama + " - " + s.fld_NamaPktUtama + " - (" + s.fld_IOcode + ")" }), "Value", "Text").ToList();
                    break;
                case 2:
                    var SelectPkt2 = estateConnection.tbl_SubPkt.Join(estateConnection.tbl_PktUtama, j => new { j.fld_NegaraID, j.fld_SyarikatID, j.fld_WilayahID, j.fld_LadangID, fld_PktUtama = j.fld_KodPktUtama }, k => new { k.fld_NegaraID, k.fld_SyarikatID, k.fld_WilayahID, k.fld_LadangID, fld_PktUtama = k.fld_PktUtama }, (j, k) => new { j.fld_NegaraID, j.fld_SyarikatID, j.fld_WilayahID, j.fld_LadangID, k.fld_JnsLot, j.fld_Pkt, j.fld_NamaPkt, j.fld_Deleted, k.fld_IOcode, k.fld_DivisionID, j.fld_ID }).Where(x => x.fld_JnsLot == JnisAktvt && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahList && x.fld_LadangID == LadangList && x.fld_Deleted == false).ToList();
                    PilihPeringkat = new SelectList(SelectPkt2.Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_Pkt + " - " + s.fld_NamaPkt + " - (" + s.fld_IOcode + ")" }), "Value", "Text").ToList();
                   break;
                case 3:
                    var SelectPkt3 = estateConnection.tbl_Blok.Join(estateConnection.tbl_PktUtama, j => new { j.fld_NegaraID, j.fld_SyarikatID, j.fld_WilayahID, j.fld_LadangID, fld_PktUtama = j.fld_KodPktutama }, k => new { k.fld_NegaraID, k.fld_SyarikatID, k.fld_WilayahID, k.fld_LadangID, fld_PktUtama = k.fld_PktUtama }, (j, k) => new { j.fld_NegaraID, j.fld_SyarikatID, j.fld_WilayahID, j.fld_LadangID, k.fld_JnsLot, j.fld_Blok, j.fld_NamaBlok, j.fld_Deleted, k.fld_IOcode, k.fld_DivisionID, j.fld_ID }).Where(x => x.fld_JnsLot == JnisAktvt && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahList && x.fld_LadangID == LadangList && x.fld_Deleted == false).ToList();
                    PilihPeringkat = new SelectList(SelectPkt3.Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_Blok + " - " + s.fld_NamaBlok + " - (" + s.fld_IOcode + ")" }), "Value", "Text").ToList();
                  break;
            }
            if (PilihPeringkat.Count > 0)
            {
                PilihPeringkat.Insert(0, (new SelectListItem { Text = GlobalResCorp.lblChoose, Value = "" }));
            }
            else
            {
                PilihPeringkat.Insert(0, (new SelectListItem { Text = "No Data", Value = "" }));
            }

            estateConnection.Dispose();

            return Json(new { PilihPeringkat });
        }
    }
}