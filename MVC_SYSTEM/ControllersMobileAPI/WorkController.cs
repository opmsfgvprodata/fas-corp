using MVC_SYSTEM.Class;
using MVC_SYSTEM.log;
using MVC_SYSTEM.ModelsEstate;
using MVC_SYSTEM.ModelsMobileAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Script.Serialization;

namespace MVC_SYSTEM.ControllersMobileAPI
{
    public class WorkController : ApiController
    {
        [HttpPost]
        public List<WorkResultUpload> Post([FromBody] List<WorkData> WorkDataList)
        {
            List<WorkResultUpload> WorkResultUploadList = new List<WorkResultUpload>();
            Connection Connection = new Connection();
            errorlog geterror = new errorlog();
            CompanyIdentity CompanyIdentity = new CompanyIdentity();
            GetLadang GetLadang = new GetLadang();
            CheckrollFunction EstateFunction = new CheckrollFunction();
            GetIdentity getidentity = new GetIdentity();
            GetConfig GetConfig = new GetConfig();
            List<tbl_Kerja> tbl_Kerjas = new List<tbl_Kerja>();
            string keteranganhdr, statushdr;
            short KadarByrn = 0;
            ChangeTimeZone timezone = new ChangeTimeZone();
            bool CutOfDateStatus = false;
            int ResultID = 0;

            try
            {
                var jsonSerialiser = new JavaScriptSerializer();
                var json = jsonSerialiser.Serialize(WorkDataList);

                geterror.testlog(json, "Data Kerja");

                var GetCompanyIdentity = WorkDataList.Take(1).Select(s => new { s.fld_NegaraID, s.fld_SyarikatID, s.fld_WilayahID, s.fld_LadangID }).FirstOrDefault();
                CompanyIdentity.fld_NegaraID = GetCompanyIdentity.fld_NegaraID;
                CompanyIdentity.fld_SyarikatID = GetCompanyIdentity.fld_SyarikatID;
                CompanyIdentity.fld_WilayahID = GetCompanyIdentity.fld_WilayahID;
                CompanyIdentity.fld_LadangID = GetCompanyIdentity.fld_LadangID;

                MVC_SYSTEM_ModelsEstate dbEst = Connection.GetConnectionMobile(CompanyIdentity.fld_WilayahID, CompanyIdentity.fld_SyarikatID, CompanyIdentity.fld_NegaraID);

                var GetDateList = WorkDataList.Select(s => s.fld_Tarikh).ToArray();
                var GetAttListFromDate = dbEst.tbl_Kerjahdr.Where(x => x.fld_NegaraID == CompanyIdentity.fld_NegaraID && x.fld_SyarikatID == CompanyIdentity.fld_SyarikatID && x.fld_WilayahID == CompanyIdentity.fld_WilayahID && x.fld_LadangID == CompanyIdentity.fld_LadangID && GetDateList.Contains(x.fld_Tarikh.Value)).ToList();
                var GetWrkListFromDate = dbEst.tbl_Kerja.Where(x => x.fld_NegaraID == CompanyIdentity.fld_NegaraID && x.fld_SyarikatID == CompanyIdentity.fld_SyarikatID && x.fld_WilayahID == CompanyIdentity.fld_WilayahID && x.fld_LadangID == CompanyIdentity.fld_LadangID && GetDateList.Contains(x.fld_Tarikh.Value)).ToList();

                foreach (var WorkData in WorkDataList)
                {
                    ResultID = ResultID + 1;
                    CutOfDateStatus = EstateFunction.GetStatusCutProcess(dbEst, WorkData.fld_Tarikh, CompanyIdentity.fld_NegaraID, CompanyIdentity.fld_SyarikatID, CompanyIdentity.fld_WilayahID, CompanyIdentity.fld_LadangID);
                    if (!CutOfDateStatus)
                    {
                        var CheckAtt = GetAttListFromDate.Where(x => x.fld_Nopkj == WorkData.fld_Nopkj && x.fld_Tarikh == WorkData.fld_Tarikh).FirstOrDefault();
                        if (CheckAtt != null)
                        {
                            GetConfig.GetCutiDesc(CheckAtt.fld_Kdhdct, "cuti", out keteranganhdr, out statushdr, out KadarByrn, CompanyIdentity.fld_NegaraID, CompanyIdentity.fld_SyarikatID);
                            if (statushdr == "hadirkerja")
                            {
                                var CheckWork = GetWrkListFromDate.Where(x => x.fld_Nopkj == WorkData.fld_Nopkj && x.fld_Tarikh == WorkData.fld_Tarikh).ToList();
                                if (CheckWork.Count() == 0)
                                {
                                    int? getuserid = getidentity.ID(WorkData.fld_CreatedBy);
                                    tbl_Kerjas.Add(new tbl_Kerja() { fld_Nopkj = WorkData.fld_Nopkj, fld_Tarikh = WorkData.fld_Tarikh, fld_JnsPkt = WorkData.fld_JenisPktKod, fld_KodPkt = WorkData.fld_PktKod, fld_JnisAktvt = WorkData.fld_ActivityTypeCode, fld_KodAktvt = WorkData.fld_KodActivityOPMS, fld_JumlahHasil = WorkData.fld_Hasil, fld_Bonus = WorkData.fld_Bonus, fld_KadarByr = WorkData.fld_Rate, fld_Amount = WorkData.fld_Amount, fld_JamOT = WorkData.fld_OT, fld_BrtGth = 0, fld_PerBrshGth = 0, fld_KdhMenuai = "-", fld_KodGL = WorkData.fld_LajerKod, fld_Kong = 0, fld_Kum = WorkData.fld_KodKumpulan, fld_DataSource = WorkData.fld_DataSource, fld_CreatedDT = WorkData.fld_CreatedDT, fld_CreatedBy = getuserid, fld_NegaraID = WorkData.fld_NegaraID, fld_SyarikatID = WorkData.fld_SyarikatID, fld_WilayahID = WorkData.fld_WilayahID, fld_LadangID = WorkData.fld_LadangID, fld_Unit = WorkData.fld_Unit, fld_HrgaKwsnSkar = 0, fld_KodKwsnSkar = "OR", fld_OverallAmount = WorkData.fld_Amount, fld_Quality = WorkData.fld_Quality, fld_ApprovalKwsnSkarDT = null, fld_ApprovalKwsnSkarLainBy = 0, fld_DailyIncentive = 0 });
                                    WorkResultUploadList.Add(new WorkResultUpload() { fld_Nopkj = WorkData.fld_Nopkj, fld_Tarikh = WorkData.fld_Tarikh, fld_Status = 1, Msg = "Successful Uploaded", WorkReturnID = ResultID });
                                }
                                else
                                {
                                    WorkResultUploadList.Add(new WorkResultUpload() { fld_Nopkj = WorkData.fld_Nopkj, fld_Tarikh = WorkData.fld_Tarikh, fld_Status = 2, Msg = "Sorry this data cannot upload because work data already exist please refer on OPMS Web System", WorkReturnID = ResultID });
                                }
                            }
                            else
                            {
                                WorkResultUploadList.Add(new WorkResultUpload() { fld_Nopkj = WorkData.fld_Nopkj, fld_Tarikh = WorkData.fld_Tarikh, fld_Status = 2, Msg = "Sorry this data cannot upload because data attandance in OPMS Web System is absent", WorkReturnID = ResultID });
                            }
                        }
                        else
                        {
                            WorkResultUploadList.Add(new WorkResultUpload() { fld_Nopkj = WorkData.fld_Nopkj, fld_Tarikh = WorkData.fld_Tarikh, fld_Status = 2, Msg = "Sorry this data cannot upload because no data attandance", WorkReturnID = ResultID });
                        }
                    }
                    else
                    {
                        WorkResultUploadList.Add(new WorkResultUpload() { fld_Nopkj = WorkData.fld_Nopkj, fld_Tarikh = WorkData.fld_Tarikh, fld_Status = 2, Msg = "Sorry this data cannot upload because transaction already closed", WorkReturnID = ResultID });
                    }
                }
                if (tbl_Kerjas.Count > 0)
                {
                    dbEst.tbl_Kerja.AddRange(tbl_Kerjas);
                    dbEst.SaveChanges();
                    EstateFunction.SaveDataKerjaSAP(dbEst, tbl_Kerjas, WorkDataList, CompanyIdentity.fld_NegaraID, CompanyIdentity.fld_SyarikatID, CompanyIdentity.fld_WilayahID, CompanyIdentity.fld_LadangID);
                }
            }
            catch (Exception exception)
            {
                ResultID = ResultID + 1;
                geterror.catcherro(exception.Message, exception.StackTrace, exception.Source, exception.TargetSite.ToString());
                WorkResultUploadList.Add(new WorkResultUpload() { fld_Nopkj = "-", fld_Tarikh = timezone.gettimezone(), fld_Status = 0, Msg = "Sorry this data cannot upload because some error occor", WorkReturnID = ResultID });
            }

            return WorkResultUploadList;
        }
    }
}
