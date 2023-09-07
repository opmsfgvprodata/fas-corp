using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using MVC_SYSTEM.Class;
using MVC_SYSTEM.log;
using MVC_SYSTEM.ModelsCorporate;
using MVC_SYSTEM.ModelsSAPPUP;
using MVC_SYSTEM.OPMStoSAP;
using MVC_SYSTEM.ViewingModels;

namespace MVC_SYSTEM.ControllersAPI
{
    public class OPMSPOSTTOSAPController : Controller
    {
        private MVC_SYSTEM_Models_SAPPUP db = new MVC_SYSTEM_Models_SAPPUP();
        private GetIdentity GetIdentity = new GetIdentity();
        private GetConfig GetConfig = new GetConfig();
        private GetNSWL GetNSWL = new GetNSWL();
        private Connection Connection = new Connection();
        private ChangeTimeZone timezone = new ChangeTimeZone();
        errorlog geterror = new errorlog();
        private GlobalFunction GlobalFunction = new GlobalFunction();
        GetWilayah getwilyah = new GetWilayah();

        // GET: OPMSPOSTTOSAP
        public ActionResult Index()
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            ViewBag.OPMSPOSTTOSAP = "class = active";

            //GL to GL
            //try
            //{
            //    BasicHttpBinding binding = new BasicHttpBinding();
            //    binding.Security.Mode = BasicHttpSecurityMode.TransportCredentialOnly;
            //    binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;
            //    NetworkCredential Cred = new NetworkCredential();

            //    EndpointAddress endpoint = new EndpointAddress(
            //        "http://feldadev.felhqr.myfelda:8000/sap/bc/srt/rfc/sap/zws_acc_doc_post/210/zws_acc_doc_post/zws_acc_doc_post");

            //    ZWS_ACC_DOC_POSTClient SAPPosting = new ZWS_ACC_DOC_POSTClient(binding, endpoint);

            //    BAPIACGL09 InputGL = new BAPIACGL09();
            //    BAPIACGL09 InputGL1 = new BAPIACGL09();
            //    BAPIACHE09 InputHeader = new BAPIACHE09();
            //    BAPIACCR09 InputAmountGL = new BAPIACCR09();
            //    BAPIACCR09 InputAmount1GL1 = new BAPIACCR09();
            //    BAPIRET2 Return = new BAPIRET2();

            //    ZFI_ACC_DOC_POST Request = new ZFI_ACC_DOC_POST();

            //    ZFI_ACC_DOC_POSTResponse Response = new ZFI_ACC_DOC_POSTResponse();

            //    Cred.UserName = "3000745";
            //    Cred.Password = "fikadev";
            //    SAPPosting.ClientCredentials.UserName.UserName = Cred.UserName;
            //    SAPPosting.ClientCredentials.UserName.Password = Cred.Password;
            //    SAPPosting.Open();

            //    InputGL.ITEMNO_ACC = "0000000001";
            //    InputGL.GL_ACCOUNT = "0053100070";
            //    InputGL.ITEM_TEXT = "CUBAAN GL 123";
            //    InputGL.COSTCENTER = "0941010010";

            //    InputGL1.ITEMNO_ACC = "0000000002";
            //    InputGL1.GL_ACCOUNT = "0025602010";
            //    InputGL1.ITEM_TEXT = "CUBAAN GL 123";

            //    InputHeader.HEADER_TXT = "123";
            //    //header.USERNAME = "3000185";
            //    InputHeader.USERNAME = "3000745";
            //    InputHeader.COMP_CODE = "9410";
            //    InputHeader.DOC_DATE = "2018-08-02";
            //    InputHeader.PSTNG_DATE = "2018-08-02";
            //    InputHeader.DOC_TYPE = "SA";
            //    InputHeader.REF_DOC_NO = "CHECKROLL2";

            //    InputAmountGL.ITEMNO_ACC = "0000000001";
            //    InputAmountGL.CURRENCY = "RM";
            //    InputAmountGL.AMT_DOCCUR = 100;

            //    InputAmount1GL1.ITEMNO_ACC = "0000000002";
            //    InputAmount1GL1.CURRENCY = "RM";
            //    InputAmount1GL1.AMT_DOCCUR = -100;

            //    Return.TYPE = null;
            //    Return.ID = null;
            //    Return.NUMBER = null;
            //    Return.LOG_NO = null;
            //    Return.LOG_MSG_NO = null;
            //    Return.MESSAGE_V1 = null;
            //    Return.MESSAGE_V2 = null;
            //    Return.MESSAGE_V3 = null;
            //    Return.MESSAGE_V4 = null;
            //    Return.PARAMETER = null;
            //    Return.ROW = 0;
            //    Return.FIELD = null;
            //    Return.SYSTEM = null;

            //    BAPIRET2[] ReturnArray = new BAPIRET2[] { Return };
            //    BAPIACCR09[] AmountArray = new BAPIACCR09[] { InputAmountGL, InputAmount1GL1 };
            //    BAPIACGL09[] GLArray = new BAPIACGL09[] { InputGL, InputGL1 };
            //    //BAPIACHE09[] HeaderArray = new BAPIACHE09[] { InputHeader };

            //    Request.ACCOUNTGL = GLArray;
            //    Request.RETURN = ReturnArray;
            //    Request.CURRENCYAMOUNT = AmountArray;
            //    Request.DOCUMENTHEADER = InputHeader;

            //    Response = SAPPosting.ZFI_ACC_DOC_POST(Request);
            //}

            //catch (Exception ex)
            //{

            //}

            //GL to GL
            //try
            //{
            //    BasicHttpBinding binding = new BasicHttpBinding();
            //    binding.Security.Mode = BasicHttpSecurityMode.TransportCredentialOnly;
            //    binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;
            //    NetworkCredential Cred = new NetworkCredential();

            //    EndpointAddress endpoint = new EndpointAddress(
            //        "http://feldadev.felhqr.myfelda:8000/sap/bc/srt/rfc/sap/zws_acc_doc_post/210/zws_acc_doc_post/zws_acc_doc_post");

            //    ZWS_ACC_DOC_POSTClient SAPPosting = new ZWS_ACC_DOC_POSTClient(binding, endpoint);

            //    BAPIACGL09 InputGL1 = new BAPIACGL09();
            //    BAPIACGL09 InputGL2 = new BAPIACGL09();
            //    BAPIACGL09 InputGL3 = new BAPIACGL09();
            //    BAPIACGL09 InputGL4 = new BAPIACGL09();
            //    BAPIACHE09 InputHeader = new BAPIACHE09();
            //    BAPIACCR09 InputAmountGL1 = new BAPIACCR09();
            //    BAPIACCR09 InputAmountGL2 = new BAPIACCR09();
            //    BAPIACCR09 InputAmountGL3 = new BAPIACCR09();
            //    BAPIACCR09 InputAmountGL4 = new BAPIACCR09();

            //    BAPIRET2 Return = new BAPIRET2();

            //    ZFI_ACC_DOC_POST Request = new ZFI_ACC_DOC_POST();

            //    ZFI_ACC_DOC_POSTResponse Response = new ZFI_ACC_DOC_POSTResponse();

            //    Cred.UserName = "3000745";
            //    Cred.Password = "fikadev";
            //    SAPPosting.ClientCredentials.UserName.UserName = Cred.UserName;
            //    SAPPosting.ClientCredentials.UserName.Password = Cred.Password;
            //    SAPPosting.Open();



            //    //InputGL1.ITEMNO_ACC = "0000000001";
            //    //InputGL1.GL_ACCOUNT = "0053100070";
            //    //InputGL1.ITEM_TEXT = "Harvesting & Collection (CHECKROLL)";
            //    //InputGL1.NETWORK = "000006000142";
            //    //InputGL1.ACTIVITY = "0304";

            //    //InputAmountGL1.ITEMNO_ACC = "0000000001";
            //    //InputAmountGL1.CURRENCY = "RM";
            //    //InputAmountGL1.AMT_DOCCUR = -500;

            //    //InputGL2.ITEMNO_ACC = "0000000002";
            //    //InputGL2.GL_ACCOUNT = "0053100100";
            //    //InputGL2.ITEM_TEXT = "Manuring (CHECKROLL)";
            //    //InputGL2.NETWORK = "000006000141";
            //    //InputGL2.ACTIVITY = "0622";

            //    //InputAmountGL2.ITEMNO_ACC = "0000000002";
            //    //InputAmountGL2.CURRENCY = "RM";
            //    //InputAmountGL2.AMT_DOCCUR = -500;

            //    InputGL3.ITEMNO_ACC = "0000000001";
            //    InputGL3.GL_ACCOUNT = "0053100100";
            //    //InputGL3.GL_ACCOUNT = "0053100110";
            //    //GL Problem
            //    InputGL3.ITEM_TEXT = "Manuring (CHECKROLL)";
            //    InputGL3.NETWORK = "000006000140";
            //    InputGL3.ACTIVITY = "1009";

            //    InputAmountGL3.ITEMNO_ACC = "0000000001";
            //    InputAmountGL3.CURRENCY = "RM";
            //    InputAmountGL3.AMT_DOCCUR = -2000;

            //    InputGL4.ITEMNO_ACC = "0000000002";
            //    //InputGL3.GL_ACCOUNT = "0018941200";
            //    InputGL4.GL_ACCOUNT = "0018941200";
            //    InputGL4.ITEM_TEXT = "GAJI";

            //    InputAmountGL4.ITEMNO_ACC = "0000000002";
            //    InputAmountGL4.CURRENCY = "RM";
            //    InputAmountGL4.AMT_DOCCUR = 2000;

            //    InputHeader.HEADER_TXT = "123";
            //    //header.USERNAME = "3000185";
            //    InputHeader.USERNAME = "3000745";
            //    InputHeader.COMP_CODE = "9410";
            //    InputHeader.DOC_DATE = "2018-08-30";
            //    InputHeader.PSTNG_DATE = "2018-08-30";
            //    InputHeader.DOC_TYPE = "SA";
            //    InputHeader.REF_DOC_NO = "CHECKROLL813s";

            //    Return.TYPE = null;
            //    Return.ID = null;
            //    Return.MESSAGE = null;
            //    Return.NUMBER = null;
            //    Return.LOG_NO = null;
            //    Return.LOG_MSG_NO = null;
            //    Return.MESSAGE_V1 = null;
            //    Return.MESSAGE_V2 = null;
            //    Return.MESSAGE_V3 = null;
            //    Return.MESSAGE_V4 = null;
            //    Return.PARAMETER = null;
            //    Return.ROW = 0;
            //    Return.FIELD = null;
            //    Return.SYSTEM = null;

            //    BAPIRET2[] ReturnArray = new BAPIRET2[] { Return };
            //    //BAPIACCR09[] AmountArray = new BAPIACCR09[] { InputAmount1GL3, InputAmountGL, InputAmount1GL1, InputAmount1GL2  };
            //    //BAPIACGL09[] GLArray = new BAPIACGL09[] { InputGL3, InputGL, InputGL1, InputGL2  };
            //    //BAPIACHE09[] HeaderArray = new BAPIACHE09[] { InputHeader };

            //    BAPIACCR09[] AmountArray = new BAPIACCR09[] { InputAmountGL3, InputAmountGL4 };
            //    BAPIACGL09[] GLArray = new BAPIACGL09[] { InputGL3, InputGL4 };

            //    Request.ACCOUNTGL = GLArray;
            //    Request.RETURN = ReturnArray;
            //    Request.CURRENCYAMOUNT = AmountArray;
            //    Request.DOCUMENTHEADER = InputHeader;

            //    Response = SAPPosting.ZFI_ACC_DOC_POST(Request);
            //}

            //catch (Exception ex)
            //{

            //}

            //GL to AR
            //try
            //{
            //    BasicHttpBinding binding = new BasicHttpBinding();
            //    binding.Security.Mode = BasicHttpSecurityMode.TransportCredentialOnly;
            //    binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;
            //    NetworkCredential Cred = new NetworkCredential();

            //    EndpointAddress endpoint = new EndpointAddress(
            //        "http://feldadev.felhqr.myfelda:8000/sap/bc/srt/rfc/sap/zws_acc_doc_post/210/zws_acc_doc_post/zws_acc_doc_post");

            //    ZWS_ACC_DOC_POSTClient SAPPosting = new ZWS_ACC_DOC_POSTClient(binding, endpoint);

            //    BAPIACGL09 InputGL = new BAPIACGL09();
            //    BAPIACAR09 InputAR = new BAPIACAR09();
            //    BAPIACHE09 InputHeader = new BAPIACHE09();
            //    BAPIACCR09 InputAmountGL = new BAPIACCR09();
            //    BAPIACCR09 InputAmount1GL1 = new BAPIACCR09();
            //    BAPIRET2 Return = new BAPIRET2();

            //    ZFI_ACC_DOC_POST Request = new ZFI_ACC_DOC_POST();

            //    ZFI_ACC_DOC_POSTResponse Response = new ZFI_ACC_DOC_POSTResponse();

            //    Cred.UserName = "3000745";
            //    Cred.Password = "fikadev";
            //    SAPPosting.ClientCredentials.UserName.UserName = Cred.UserName;
            //    SAPPosting.ClientCredentials.UserName.Password = Cred.Password;
            //    SAPPosting.Open();

            //    InputGL.ITEMNO_ACC = "0000000002";
            //    InputGL.GL_ACCOUNT = "0060000020";
            //    InputGL.ITEM_TEXT = "CUBAAN AR 123";
            //    InputGL.COSTCENTER = "0941010020";

            //    InputAR.ITEMNO_ACC = "0000000001";
            //    InputAR.CUSTOMER = "0000200060";
            //    InputAR.ITEM_TEXT = "CUBAAN AR 123";

            //    InputHeader.HEADER_TXT = "123";
            //    //header.USERNAME = "3000185";
            //    InputHeader.USERNAME = "3000745";
            //    InputHeader.COMP_CODE = "9410";
            //    InputHeader.DOC_DATE = "2018-08-16";
            //    InputHeader.PSTNG_DATE = "2018-08-16";
            //    InputHeader.DOC_TYPE = "DR";
            //    InputHeader.REF_DOC_NO = "CHECKROLL7";

            //    InputAmountGL.ITEMNO_ACC = "0000000001";
            //    InputAmountGL.CURRENCY = "RM";
            //    InputAmountGL.AMT_DOCCUR = 100;

            //    InputAmount1GL1.ITEMNO_ACC = "0000000002";
            //    InputAmount1GL1.CURRENCY = "RM";
            //    InputAmount1GL1.AMT_DOCCUR = -100;

            //    Return.TYPE = null;
            //    Return.ID = null;
            //    Return.NUMBER = null;
            //    Return.LOG_NO = null;
            //    Return.LOG_MSG_NO = null;
            //    Return.MESSAGE_V1 = null;
            //    Return.MESSAGE_V2 = null;
            //    Return.MESSAGE_V3 = null;
            //    Return.MESSAGE_V4 = null;
            //    Return.PARAMETER = null;
            //    Return.ROW = 0;
            //    Return.FIELD = null;
            //    Return.SYSTEM = null;

            //    BAPIRET2[] ReturnArray = new BAPIRET2[] { Return };
            //    BAPIACCR09[] AmountArray = new BAPIACCR09[] { InputAmountGL, InputAmount1GL1 };
            //    BAPIACGL09[] GLArray = new BAPIACGL09[] { InputGL };
            //    BAPIACAR09[] ARArray = new BAPIACAR09[] { InputAR };

            //    //BAPIACHE09[] HeaderArray = new BAPIACHE09[] { InputHeader };

            //    Request.ACCOUNTGL = GLArray;
            //    Request.ACCOUNTRECEIVABLE = ARArray;
            //    Request.RETURN = ReturnArray;
            //    Request.CURRENCYAMOUNT = AmountArray;
            //    Request.DOCUMENTHEADER = InputHeader;

            //    Response = SAPPosting.ZFI_ACC_DOC_POST(Request);
            //}

            //catch (Exception ex)
            //{

            //}

            //GL to AP
            try
            {
                BasicHttpBinding binding = new BasicHttpBinding();
                binding.Security.Mode = BasicHttpSecurityMode.TransportCredentialOnly;
                binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;
                NetworkCredential Cred = new NetworkCredential();

                EndpointAddress endpoint = new EndpointAddress(
                    "http://feldadev.felhqr.myfelda:8000/sap/bc/srt/rfc/sap/zws_acc_doc_post/210/zws_acc_doc_post/zws_acc_doc_post");

                ZWS_ACC_DOC_POSTClient SAPPosting = new ZWS_ACC_DOC_POSTClient(binding, endpoint);

                BAPIACGL09 InputGL = new BAPIACGL09();
                BAPIACAP09 InputAP = new BAPIACAP09();
                BAPIACHE09 InputHeader = new BAPIACHE09();
                BAPIACCR09 InputAmountGL = new BAPIACCR09();
                BAPIACCR09 InputAmount1GL1 = new BAPIACCR09();
                BAPIRET2 Return = new BAPIRET2();

                ZFI_ACC_DOC_POST Request = new ZFI_ACC_DOC_POST();

                ZFI_ACC_DOC_POSTResponse Response = new ZFI_ACC_DOC_POSTResponse();

                Cred.UserName = "3000745";
                Cred.Password = "fikadev";
                SAPPosting.ClientCredentials.UserName.UserName = Cred.UserName;
                SAPPosting.ClientCredentials.UserName.Password = Cred.Password;
                SAPPosting.Open();

                InputGL.ITEMNO_ACC = "0000000002";
                InputGL.GL_ACCOUNT = "0025990010";
                InputGL.ITEM_TEXT = "CUBAAN AP 123";

                InputAP.ITEMNO_ACC = "0000000001";
                InputAP.VENDOR_NO = "0100000076";
                InputAP.ITEM_TEXT = "CUBAAN AP 123";

                InputHeader.HEADER_TXT = "123";
                //header.USERNAME = "3000185";
                InputHeader.USERNAME = "3000745";
                InputHeader.COMP_CODE = "9410";
                InputHeader.DOC_DATE = "2018-08-16";
                InputHeader.PSTNG_DATE = "2018-08-16";
                InputHeader.DOC_TYPE = "KR";
                InputHeader.REF_DOC_NO = "CHECKROLLAA1";

                InputAmountGL.ITEMNO_ACC = "0000000001";
                InputAmountGL.CURRENCY = "RM";
                InputAmountGL.AMT_DOCCUR = 100;

                InputAmount1GL1.ITEMNO_ACC = "0000000002";
                InputAmount1GL1.CURRENCY = "RM";
                InputAmount1GL1.AMT_DOCCUR = -100;

                Return.TYPE = null;
                Return.ID = null;
                Return.NUMBER = null;
                Return.LOG_NO = null;
                Return.LOG_MSG_NO = null;
                Return.MESSAGE_V1 = null;
                Return.MESSAGE_V2 = null;
                Return.MESSAGE_V3 = null;
                Return.MESSAGE_V4 = null;
                Return.PARAMETER = null;
                Return.ROW = 0;
                Return.FIELD = null;
                Return.SYSTEM = null;

                BAPIRET2[] ReturnArray = new BAPIRET2[] { Return };
                BAPIACCR09[] AmountArray = new BAPIACCR09[] { InputAmountGL, InputAmount1GL1 };
                BAPIACGL09[] GLArray = new BAPIACGL09[] { InputGL };
                BAPIACAP09[] APArray = new BAPIACAP09[] { InputAP };

                //BAPIACHE09[] HeaderArray = new BAPIACHE09[] { InputHeader };

                Request.ACCOUNTGL = GLArray;
                Request.ACCOUNTPAYABLE = APArray;
                Request.RETURN = ReturnArray;
                Request.CURRENCYAMOUNT = AmountArray;
                Request.DOCUMENTHEADER = InputHeader;

                Response = SAPPosting.ZFI_ACC_DOC_POST(Request);
            }

            catch (Exception ex)
            {

            }


            return View();
        }

        //public ActionResult _PostToSAP(string filter, int page = 1,
        //    string sort = "fldOptConfFlag1",
        //    string sortdir = "ASC")
        //{
        //    int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
        //    int? getuserid = GetIdentity.ID(User.Identity.Name);
        //    string host, catalog, user, pass = "";
        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

        //    //int pageSize = int.Parse(GetConfig.GetData("paging"));
        //    //var records = new PagedList<ModelsCorporate.tblOptionConfigsWeb>();
        //    //int role = GetIdentity.RoleID(getuserid).Value;

        //    List<SelectListItem> ccList = new List<SelectListItem>();

        //    ccList = new SelectList(
        //        db.tbl_SAPCCPUP
        //            .Where(x => x.fld_NegaraID == NegaraID &&
        //                        x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false)
        //            .OrderBy(o => o.fld_CostCenter)
        //            .Select(
        //                s => new SelectListItem { Value = s.fld_CostCenter, Text = s.fld_CostCenterDesc }),
        //        "Value", "Text").ToList();
        //    ccList.Insert(0, new SelectListItem { Text = "Please Choose", Value = "" });

        //    ViewBag.CCList = ccList;

        //    List<SelectListItem> gLList = new List<SelectListItem>();

        //    gLList = new SelectList(
        //        db.tbl_SAPGLPUP
        //            .Where(x => x.fld_NegaraID == NegaraID &&
        //                        x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false)
        //            .OrderBy(o => o.fld_GLCode)
        //            .Select(
        //                s => new SelectListItem { Value = s.fld_GLCode, Text = s.fld_GLDesc }),
        //        "Value", "Text").ToList();
        //    gLList.Insert(0, new SelectListItem { Text = "Sila Pilih", Value = "" });

        //    ViewBag.GLList = gLList;

        //    //List<SelectListItem> gLList = new List<SelectListItem>();

        //    //gLList = new SelectList(
        //    //    db.tbl_SAPGLPUP
        //    //        .Where(x => x.fld_NegaraID == NegaraID &&
        //    //                    x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false)
        //    //        .OrderBy(o => o.fld_GLCode)
        //    //        .Select(
        //    //            s => new SelectListItem { Value = s.fld_GLCode, Text = s.fld_GLDesc }),
        //    //    "Value", "Text").ToList();
        //    //gLList.Insert(0, new SelectListItem { Text = "Sila Pilih", Value = "" });

        //    //ViewBag.GLList = gLList;

        //    return View();
        //}
    }
}