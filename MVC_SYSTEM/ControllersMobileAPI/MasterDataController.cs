using MVC_SYSTEM.log;
using MVC_SYSTEM.Models;
using MVC_SYSTEM.ModelsMobileAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Results;
using MVC_SYSTEM.Security;
using MVC_SYSTEM.Class;
using System.Web.Script.Serialization;
using System.Web.Http.Cors;

namespace MVC_SYSTEM.ControllersMobileAPI
{
    public class MasterDataController : ApiController
    {
        [HttpPost]
        public JsonResult<MasterData> Post([FromBody] MasterDataSyncForm MasterDataSyncForm)
        {
            MasterData MasterData = new MasterData();
            errorlog geterror = new errorlog();
            GetMasterData GetMasterData = new GetMasterData();
            LoginResult LoginResult = new LoginResult();
            int ID = 1;
            try
            {
                MVC_SYSTEM_Models db = new MVC_SYSTEM_Models();
                EncryptDecrypt EncryptDecrypt = new EncryptDecrypt();
                var jsonSerialiser = new JavaScriptSerializer();
                var json = jsonSerialiser.Serialize(MasterDataSyncForm);
                geterror.testlog(json, "Master Data");
                MasterData.tbl_KumpulanPkj = GetMasterData.tbl_KumpulanPkj(MasterDataSyncForm.fld_KmplnSyrktID.Value, MasterDataSyncForm.fld_NegaraID.Value, MasterDataSyncForm.fld_SyarikatID.Value, MasterDataSyncForm.fld_WilayahID.Value, MasterDataSyncForm.fld_LadangID.Value, MasterDataSyncForm.fld_DivisionID.Value);
                MasterData.tbl_PkjMast = GetMasterData.tbl_PkjMast(MasterDataSyncForm.fld_KmplnSyrktID.Value, MasterDataSyncForm.fld_NegaraID.Value, MasterDataSyncForm.fld_SyarikatID.Value, MasterDataSyncForm.fld_WilayahID.Value, MasterDataSyncForm.fld_LadangID.Value, MasterDataSyncForm.fld_DivisionID.Value);
                MasterData.tbl_CutiPeruntukan = GetMasterData.tbl_CutiPeruntukan(MasterDataSyncForm.fld_KmplnSyrktID.Value, MasterDataSyncForm.fld_NegaraID.Value, MasterDataSyncForm.fld_SyarikatID.Value, MasterDataSyncForm.fld_WilayahID.Value, MasterDataSyncForm.fld_LadangID.Value, MasterDataSyncForm.fld_DivisionID.Value);
                MasterData.tbl_JenisKhdrn = GetMasterData.tbl_JenisKhdrn(MasterDataSyncForm.fld_KmplnSyrktID.Value, MasterDataSyncForm.fld_NegaraID.Value, MasterDataSyncForm.fld_SyarikatID.Value, MasterDataSyncForm.fld_WilayahID.Value, MasterDataSyncForm.fld_LadangID.Value);
                MasterData.tbl_JenisPkt = GetMasterData.tbl_JenisPkt(MasterDataSyncForm.fld_KmplnSyrktID.Value, MasterDataSyncForm.fld_NegaraID.Value, MasterDataSyncForm.fld_SyarikatID.Value, MasterDataSyncForm.fld_WilayahID.Value, MasterDataSyncForm.fld_LadangID.Value);
                MasterData.tbl_Pkt = GetMasterData.tbl_Pkt(MasterDataSyncForm.fld_KmplnSyrktID.Value, MasterDataSyncForm.fld_NegaraID.Value, MasterDataSyncForm.fld_SyarikatID.Value, MasterDataSyncForm.fld_WilayahID.Value, MasterDataSyncForm.fld_LadangID.Value, MasterDataSyncForm.fld_DivisionID.Value);
                MasterData.tbl_Lajer = GetMasterData.tbl_Lajer(MasterDataSyncForm.fld_KmplnSyrktID.Value, MasterDataSyncForm.fld_NegaraID.Value, MasterDataSyncForm.fld_SyarikatID.Value, MasterDataSyncForm.fld_WilayahID.Value, MasterDataSyncForm.fld_LadangID.Value);
                MasterData.tbl_MapGL = GetMasterData.tbl_MapGL(MasterDataSyncForm.fld_KmplnSyrktID.Value, MasterDataSyncForm.fld_NegaraID.Value, MasterDataSyncForm.fld_SyarikatID.Value, MasterDataSyncForm.fld_WilayahID.Value, MasterDataSyncForm.fld_LadangID.Value);
                MasterData.tbl_AktvtKod = GetMasterData.tbl_AktvtKod(MasterDataSyncForm.fld_KmplnSyrktID.Value, MasterDataSyncForm.fld_NegaraID.Value, MasterDataSyncForm.fld_SyarikatID.Value, MasterDataSyncForm.fld_WilayahID.Value, MasterDataSyncForm.fld_LadangID.Value);
                MasterData.tbl_PublicHolidayDate = GetMasterData.tbl_PublicHolidayDate(MasterDataSyncForm.fld_KmplnSyrktID.Value, MasterDataSyncForm.fld_NegaraID.Value, MasterDataSyncForm.fld_SyarikatID.Value, MasterDataSyncForm.fld_WilayahID.Value, MasterDataSyncForm.fld_LadangID.Value);
                MasterData.tbl_CCNN = GetMasterData.tbl_CCNN(MasterDataSyncForm.fld_KmplnSyrktID.Value, MasterDataSyncForm.fld_NegaraID.Value, MasterDataSyncForm.fld_SyarikatID.Value, MasterDataSyncForm.fld_WilayahID.Value, MasterDataSyncForm.fld_LadangID.Value, MasterDataSyncForm.fld_DivisionID.Value);
                MasterData.tbl_ActivityType = GetMasterData.tbl_ActivityType(MasterDataSyncForm.fld_KmplnSyrktID.Value, MasterDataSyncForm.fld_NegaraID.Value, MasterDataSyncForm.fld_SyarikatID.Value, MasterDataSyncForm.fld_WilayahID.Value, MasterDataSyncForm.fld_LadangID.Value);
                MasterData.tbl_PkjIncrementSalary = GetMasterData.tbl_PkjIncrmntSalary(MasterDataSyncForm.fld_KmplnSyrktID.Value, MasterDataSyncForm.fld_NegaraID.Value, MasterDataSyncForm.fld_SyarikatID.Value, MasterDataSyncForm.fld_WilayahID.Value, MasterDataSyncForm.fld_LadangID.Value, MasterDataSyncForm.fld_DivisionID.Value);

            }
            catch (Exception ex)
            {
                geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
            }

            return Json(MasterData);
        }
    }
}
