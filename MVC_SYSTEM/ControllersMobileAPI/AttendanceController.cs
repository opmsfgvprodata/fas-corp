using MVC_SYSTEM.Class;
using MVC_SYSTEM.log;
using MVC_SYSTEM.ModelsEstate;
using MVC_SYSTEM.ModelsMobileAPI;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Script.Serialization;

namespace MVC_SYSTEM.ControllersMobileAPI
{
    public class AttendanceController : ApiController
    {
        [HttpPost]
        public List<AttendanceResultUpload> Post([FromBody] List<AttandanceData> AttandanceDataList)
        {
            Connection Connection = new Connection();
            errorlog geterror = new errorlog();
            CompanyIdentity CompanyIdentity = new CompanyIdentity();
            GetLadang GetLadang = new GetLadang();
            CheckrollFunction EstateFunction = new CheckrollFunction();
            GetIdentity getidentity = new GetIdentity();
            string LadangNegeriCode = "";
            string Msg = "";
            bool CutOfDateStatus = false;
            bool LeaveSelection = false;
            int ResultID = 0;
            List<tbl_Kerjahdr> tbl_Kerjahdrs = new List<tbl_Kerjahdr>();
            ChangeTimeZone timezone = new ChangeTimeZone();
            List<AttendanceResultUpload> AttandanceResultUploadList = new List<AttendanceResultUpload>();

            try
            {
                var jsonSerialiser = new JavaScriptSerializer();
                var json = jsonSerialiser.Serialize(AttandanceDataList);

                geterror.testlog(json, "Data Kehadiran");

                var GetCompanyIdentity = AttandanceDataList.Take(1).Select(s => new { s.fld_NegaraID, s.fld_SyarikatID, s.fld_WilayahID, s.fld_LadangID }).FirstOrDefault();
                CompanyIdentity.fld_NegaraID = GetCompanyIdentity.fld_NegaraID;
                CompanyIdentity.fld_SyarikatID = GetCompanyIdentity.fld_SyarikatID;
                CompanyIdentity.fld_WilayahID = GetCompanyIdentity.fld_WilayahID;
                CompanyIdentity.fld_LadangID = GetCompanyIdentity.fld_LadangID;

                MVC_SYSTEM_ModelsEstate dbEst = Connection.GetConnectionMobile(CompanyIdentity.fld_WilayahID, CompanyIdentity.fld_SyarikatID, CompanyIdentity.fld_NegaraID);

                var SelectionDateUpload = AttandanceDataList.Select(s => s.fld_Tarikh).OrderBy(o => o).Distinct().ToList();

                var GetWorkerAttandanceList = dbEst.tbl_Kerjahdr.Where(x => SelectionDateUpload.Contains(x.fld_Tarikh.Value) && x.fld_NegaraID == CompanyIdentity.fld_NegaraID && x.fld_SyarikatID == CompanyIdentity.fld_SyarikatID && x.fld_WilayahID == CompanyIdentity.fld_WilayahID && x.fld_LadangID == CompanyIdentity.fld_LadangID).ToList();

                foreach (var AttandanceData in AttandanceDataList)
                {
                    ResultID = ResultID + 1;
                    int? getuserid = getidentity.ID(AttandanceData.fld_CreatedBy);
                    var CheckData = GetWorkerAttandanceList.Where(x => x.fld_Nopkj == AttandanceData.fld_Nopkj && x.fld_Tarikh == AttandanceData.fld_Tarikh).FirstOrDefault();
                    var CheckData2 = tbl_Kerjahdrs.Where(x => x.fld_Nopkj == AttandanceData.fld_Nopkj && x.fld_Tarikh == AttandanceData.fld_Tarikh).FirstOrDefault();
                    if (CheckData == null && CheckData2 == null)
                    {
                        LadangNegeriCode = GetLadang.GetLadangNegeriCode(CompanyIdentity.fld_LadangID);
                        if (EstateFunction.GetCutiAmMgguMatchDate(LadangNegeriCode, CompanyIdentity.fld_NegaraID, CompanyIdentity.fld_SyarikatID, AttandanceData.fld_Tarikh, AttandanceData.fld_Kdhdct, out Msg))
                        {
                            CutOfDateStatus = EstateFunction.GetStatusCutProcess(dbEst, AttandanceData.fld_Tarikh, CompanyIdentity.fld_NegaraID, CompanyIdentity.fld_SyarikatID, CompanyIdentity.fld_WilayahID, CompanyIdentity.fld_LadangID);
                            if (!CutOfDateStatus)
                            {
                                LeaveSelection = EstateFunction.CheckLeaveType(AttandanceData.fld_Kdhdct, CompanyIdentity.fld_NegaraID, CompanyIdentity.fld_SyarikatID) ? true : false;
                                if (LeaveSelection)
                                {
                                    if (EstateFunction.LeaveCalBal(dbEst, AttandanceData.fld_Tarikh.Year, AttandanceData.fld_Nopkj, AttandanceData.fld_Kdhdct, CompanyIdentity.fld_NegaraID, CompanyIdentity.fld_SyarikatID, CompanyIdentity.fld_WilayahID, CompanyIdentity.fld_LadangID))
                                    {
                                        tbl_Kerjahdrs.Add(new tbl_Kerjahdr() { fld_Nopkj = AttandanceData.fld_Nopkj, fld_Kum = AttandanceData.fld_KodKumpulan, fld_Tarikh = AttandanceData.fld_Tarikh, fld_Kdhdct = AttandanceData.fld_Kdhdct, fld_Hujan = AttandanceData.fld_Hujan, fld_NegaraID = AttandanceData.fld_NegaraID, fld_SyarikatID = AttandanceData.fld_SyarikatID, fld_WilayahID = AttandanceData.fld_WilayahID, fld_LadangID = AttandanceData.fld_LadangID, fld_CreatedBy = getuserid, fld_CreatedDT = AttandanceData.fld_CreatedDT, fld_DataSource = AttandanceData.fld_DataSource });
                                        EstateFunction.LeaveDeduct(dbEst, AttandanceData.fld_Tarikh.Year, AttandanceData.fld_Nopkj, AttandanceData.fld_Kdhdct, CompanyIdentity.fld_NegaraID, CompanyIdentity.fld_SyarikatID, CompanyIdentity.fld_WilayahID, CompanyIdentity.fld_LadangID);
                                        AttandanceResultUploadList.Add(new AttendanceResultUpload() { fld_Nopkj = AttandanceData.fld_Nopkj, fld_Tarikh = AttandanceData.fld_Tarikh, fld_Status = 1, Msg = "Successful Uploaded", AttReturnID = ResultID });
                                    }
                                    else
                                    {
                                        tbl_Kerjahdrs.Add(new tbl_Kerjahdr() { fld_Nopkj = AttandanceData.fld_Nopkj, fld_Kum = AttandanceData.fld_KodKumpulan, fld_Tarikh = AttandanceData.fld_Tarikh, fld_Kdhdct = AttandanceData.fld_Kdhdct, fld_Hujan = AttandanceData.fld_Hujan, fld_NegaraID = AttandanceData.fld_NegaraID, fld_SyarikatID = AttandanceData.fld_SyarikatID, fld_WilayahID = AttandanceData.fld_WilayahID, fld_LadangID = AttandanceData.fld_LadangID, fld_CreatedBy = getuserid, fld_CreatedDT = AttandanceData.fld_CreatedDT, fld_DataSource = AttandanceData.fld_DataSource });
                                        AttandanceResultUploadList.Add(new AttendanceResultUpload() { fld_Nopkj = AttandanceData.fld_Nopkj, fld_Tarikh = AttandanceData.fld_Tarikh, fld_Status = 1, Msg = "Successful Uploaded", AttReturnID = ResultID });
                                    }
                                }
                                else
                                {
                                    tbl_Kerjahdrs.Add(new tbl_Kerjahdr() { fld_Nopkj = AttandanceData.fld_Nopkj, fld_Kum = AttandanceData.fld_KodKumpulan, fld_Tarikh = AttandanceData.fld_Tarikh, fld_Kdhdct = AttandanceData.fld_Kdhdct, fld_Hujan = AttandanceData.fld_Hujan, fld_NegaraID = AttandanceData.fld_NegaraID, fld_SyarikatID = AttandanceData.fld_SyarikatID, fld_WilayahID = AttandanceData.fld_WilayahID, fld_LadangID = AttandanceData.fld_LadangID, fld_CreatedBy = getuserid, fld_CreatedDT = AttandanceData.fld_CreatedDT, fld_DataSource = AttandanceData.fld_DataSource });
                                    AttandanceResultUploadList.Add(new AttendanceResultUpload() { fld_Nopkj = AttandanceData.fld_Nopkj, fld_Tarikh = AttandanceData.fld_Tarikh, fld_Status = 1, Msg = "Successful Uploaded", AttReturnID = ResultID });
                                }
                            }
                            else
                            {
                                AttandanceResultUploadList.Add(new AttendanceResultUpload() { fld_Nopkj = AttandanceData.fld_Nopkj, fld_Tarikh = AttandanceData.fld_Tarikh, fld_Status = 2, Msg = "Sorry this data cannot upload because transaction already closed", AttReturnID = ResultID });
                            }
                        }
                        else
                        {
                            AttandanceResultUploadList.Add(new AttendanceResultUpload() { fld_Nopkj = AttandanceData.fld_Nopkj, fld_Tarikh = AttandanceData.fld_Tarikh, fld_Status = 2, Msg = "Sorry this data cannot upload because date selection is not holiday date", AttReturnID = ResultID });
                        }
                    }
                    else
                    {
                        AttandanceResultUploadList.Add(new AttendanceResultUpload() { fld_Nopkj = AttandanceData.fld_Nopkj, fld_Tarikh = AttandanceData.fld_Tarikh, fld_Status = 2, Msg = "Sorry this data cannot upload because date already in system", AttReturnID = ResultID });
                    }
                }

                if (tbl_Kerjahdrs.Count > 0)
                {
                    dbEst.tbl_Kerjahdr.AddRange(tbl_Kerjahdrs);
                    dbEst.SaveChanges();
                }
            }
            catch (Exception exception)
            {
                ResultID = ResultID + 1;
                geterror.catcherro(exception.Message, exception.StackTrace, exception.Source, exception.TargetSite.ToString());
                AttandanceResultUploadList.Add(new AttendanceResultUpload() { fld_Nopkj = "-", fld_Tarikh = timezone.gettimezone(), fld_Status = 0, Msg = "Sorry this data cannot upload because some error occor", AttReturnID = ResultID });
            }

            return AttandanceResultUploadList;
        }
    }
}
