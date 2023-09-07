using MVC_SYSTEM.ModelsMobileAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Results;
using MVC_SYSTEM.Class;
using MVC_SYSTEM.ViewingModels;
using MVC_SYSTEM.ModelsEstate;
using System.Web.Script.Serialization;
using MVC_SYSTEM.log;

namespace MVC_SYSTEM.ControllersMobileAPI
{
    public class PaySlipController : ApiController
    {
        [HttpPost]
        public JsonResult<vw_PaySlipPekerja> Post([FromBody] PaySlipRequestForm PaySlipRequestForm)
        {
            PaySlipData PaySlipData = new PaySlipData();
            GetNSWL GetNSWL = new GetNSWL();
            string host, catalog, user, pass = "";
            Connection Connection = new Connection();
            errorlog geterror = new errorlog();

            var GetNSWLDetail = GetNSWL.GetLadangDetailByKodLadang(PaySlipRequestForm.EstateCode);
            vw_PaySlipPekerja PaySlipPekerja = new vw_PaySlipPekerja();

            var jsonSerialiser = new JavaScriptSerializer();
            var json = jsonSerialiser.Serialize(PaySlipRequestForm);

            geterror.testlog(json, "Pay Slip");

            if (GetNSWLDetail != null)
            {
                Connection.GetConnection(out host, out catalog, out user, out pass, GetNSWLDetail.fld_WilayahID, GetNSWLDetail.fld_SyarikatID,
                GetNSWLDetail.fld_NegaraID);

                MVC_SYSTEM_Viewing dbview = MVC_SYSTEM_Viewing.ConnectToSqlServer(host, catalog, user, pass);
                MVC_SYSTEM_ModelsEstate db = Connection.GetConnectionMobile(GetNSWLDetail.fld_WilayahID, GetNSWLDetail.fld_SyarikatID, GetNSWLDetail.fld_NegaraID);

                var GetNoPkj = db.tbl_Pkjmast.Where(x => (x.fld_Nokp == PaySlipRequestForm.NoPkj || x.fld_Nopkj == PaySlipRequestForm.NoPkj) && x.fld_NegaraID == GetNSWLDetail.fld_NegaraID &&
                                    x.fld_SyarikatID == GetNSWLDetail.fld_SyarikatID &&
                                    x.fld_WilayahID == GetNSWLDetail.fld_WilayahID && x.fld_LadangID == GetNSWLDetail.fld_LadangID && x.fld_StatusApproved == 1).Select(s => s.fld_Nopkj).FirstOrDefault();

                var workerDataSingle = new ViewingModels.vw_GajiPekerja();
                
                    workerDataSingle = dbview.vw_GajiPekerja
                        .Where(x => (x.fld_Nopkj == GetNoPkj)&&
                                    x.fld_Year == PaySlipRequestForm.Year && x.fld_Month == PaySlipRequestForm.Month &&
                                    x.fld_NegaraID == GetNSWLDetail.fld_NegaraID &&
                                    x.fld_SyarikatID == GetNSWLDetail.fld_SyarikatID &&
                                    x.fld_WilayahID == GetNSWLDetail.fld_WilayahID && x.fld_LadangID == GetNSWLDetail.fld_LadangID)
                        .OrderBy(x => x.fld_Nama)
                        .SingleOrDefault();
                

                if (workerDataSingle != null)
                {
                    List<ViewingModels.vw_MaklumatInsentif> workerIncentiveRecordList = new List<ViewingModels.vw_MaklumatInsentif>();

                    List<FootNoteCustomModel> footNoteCustomModelList = new List<FootNoteCustomModel>();

                    var workerMonthlySalary = dbview.tbl_GajiBulanan
                        .SingleOrDefault(x => x.fld_Nopkj == GetNoPkj && x.fld_Month == PaySlipRequestForm.Month &&
                                              x.fld_Year == PaySlipRequestForm.Year && x.fld_NegaraID == GetNSWLDetail.fld_NegaraID &&
                                              x.fld_SyarikatID == GetNSWLDetail.fld_SyarikatID && x.fld_WilayahID == GetNSWLDetail.fld_WilayahID &&
                                              x.fld_LadangID == GetNSWLDetail.fld_LadangID);

                    List<CarumanTambahanCustomModel> carumanTambahanCustomModelList = new List<CarumanTambahanCustomModel>();

                    var workerAdditionalContribution = db.tbl_ByrCarumanTambahan
                        .Where(x => x.fld_GajiID == workerDataSingle.fld_ID && x.fld_Month == PaySlipRequestForm.Month &&
                                    x.fld_Year == PaySlipRequestForm.Year && x.fld_NegaraID == GetNSWLDetail.fld_NegaraID &&
                                    x.fld_SyarikatID == GetNSWLDetail.fld_SyarikatID && x.fld_WilayahID == GetNSWLDetail.fld_WilayahID &&
                                    x.fld_LadangID == GetNSWLDetail.fld_LadangID);

                    foreach (var caruman in workerAdditionalContribution)
                    {
                        CarumanTambahanCustomModel carumanTambahanCustomModel = new CarumanTambahanCustomModel();

                        carumanTambahanCustomModel.fld_ID = caruman.fld_ID;
                        carumanTambahanCustomModel.fld_KodCarumanTambahan = caruman.fld_KodSubCaruman;
                        carumanTambahanCustomModel.fld_CarumanMajikan = caruman.fld_CarumanMajikan;
                        carumanTambahanCustomModel.fld_CarumanPekerja = caruman.fld_CarumanPekerja;

                        carumanTambahanCustomModelList.Add(carumanTambahanCustomModel);
                    }

                    var workerIncentiveRecord = dbview.vw_MaklumatInsentif
                        .Where(x => x.fld_Nopkj == GetNoPkj && x.fld_Month == PaySlipRequestForm.Month &&
                                    x.fld_Year == PaySlipRequestForm.Year && x.fld_NegaraID == GetNSWLDetail.fld_NegaraID &&
                                    x.fld_SyarikatID == GetNSWLDetail.fld_SyarikatID && x.fld_WilayahID == GetNSWLDetail.fld_WilayahID &&
                                    x.fld_LadangID == GetNSWLDetail.fld_LadangID && x.fld_Deleted == false);

                    foreach (var insentifRecord in workerIncentiveRecord)
                    {
                        workerIncentiveRecordList.Add(insentifRecord);
                    }

                    List<KerjaPekerjaCustomModel> kerjaPekerjaCustomModelList = new List<KerjaPekerjaCustomModel>();

                    var workerWorkRecordGroupBy = dbview.vw_KerjaPekerja
                        .Where(x => x.fld_Nopkj == GetNoPkj && x.fld_Tarikh.Value.Month == PaySlipRequestForm.Month &&
                                    x.fld_Tarikh.Value.Year == PaySlipRequestForm.Year)
                        .GroupBy(x => new { x.fld_KodAktvt, x.fld_KodPkt, x.fld_Kdhdct })
                        .OrderBy(o => o.Key.fld_KodAktvt)
                        .ThenBy(t => t.Key.fld_KodPkt)
                        .ThenBy(t2 => t2.Key.fld_Kdhdct)
                        .Select(lg =>
                            new
                            {
                                fld_ID = lg.FirstOrDefault().fld_ID,
                                fld_Desc = lg.FirstOrDefault().fld_Desc,
                                fld_KodPkt = lg.FirstOrDefault().fld_KodPkt,
                                fld_JumlahHasil = lg.Sum(w => w.fld_JumlahHasil),
                                fld_Unit = lg.FirstOrDefault().fld_Unit,
                                fld_KadarByr = lg.FirstOrDefault().fld_KadarByr,
                                fld_Gandaan = lg.FirstOrDefault().fldOptConfFlag3,
                                fld_TotalAmount = lg.Sum(w => w.fld_Amount)
                            });

                    foreach (var work in workerWorkRecordGroupBy)
                    {
                        KerjaPekerjaCustomModel kerjaPekerjaCustomModel = new KerjaPekerjaCustomModel();

                        kerjaPekerjaCustomModel.fld_ID = work.fld_ID;
                        kerjaPekerjaCustomModel.fld_Desc = work.fld_Desc;
                        kerjaPekerjaCustomModel.fld_KodPkt = work.fld_KodPkt;
                        kerjaPekerjaCustomModel.fld_JumlahHasil = work.fld_JumlahHasil;
                        kerjaPekerjaCustomModel.fld_Unit = work.fld_Unit;
                        kerjaPekerjaCustomModel.fld_KadarByr = work.fld_KadarByr;
                        kerjaPekerjaCustomModel.fld_Gandaan = work.fld_Gandaan;
                        kerjaPekerjaCustomModel.fld_TotalAmount = work.fld_TotalAmount;

                        kerjaPekerjaCustomModelList.Add(kerjaPekerjaCustomModel);
                    }

                    List<OTPekerjaCustomModel> otPekerjaCustomModelList = new List<OTPekerjaCustomModel>();

                    var workerOTRecordGroupBy = dbview.vw_OTPekerja
                        .Where(x => x.fld_Nopkj == GetNoPkj && x.fld_Tarikh.Value.Month == PaySlipRequestForm.Month &&
                                    x.fld_Tarikh.Value.Year == PaySlipRequestForm.Year)
                        .GroupBy(x => x.fld_Kdhdct)
                        .OrderBy(o => o.Key)
                        .Select(lg =>
                            new
                            {
                                fld_ID = lg.FirstOrDefault().fld_ID,
                                fld_JumlahJamOT = lg.Sum(w => w.fld_JamOT),
                                fld_Desc = lg.FirstOrDefault().fldDesc,
                                fld_KadarByr = lg.FirstOrDefault().fld_Kadar,
                                fld_Gandaan = lg.FirstOrDefault().fldRate,
                                fld_TotalAmount = lg.Sum(w => w.fld_Jumlah)
                            });

                    foreach (var ot in workerOTRecordGroupBy)
                    {
                        OTPekerjaCustomModel otPekerjaCustomModel = new OTPekerjaCustomModel();

                        otPekerjaCustomModel.fld_ID = ot.fld_ID;
                        otPekerjaCustomModel.fld_Desc = "OT - " + ot.fld_Desc;
                        otPekerjaCustomModel.fld_JumlahJamOT = ot.fld_JumlahJamOT;
                        otPekerjaCustomModel.fld_Unit = "HOUR";
                        otPekerjaCustomModel.fld_KadarByr = ot.fld_KadarByr;
                        otPekerjaCustomModel.fld_Gandaan = ot.fld_Gandaan;
                        otPekerjaCustomModel.fld_TotalAmount = ot.fld_TotalAmount;

                        otPekerjaCustomModelList.Add(otPekerjaCustomModel);

                        FootNoteCustomModel otFootNoteCustomModel = new FootNoteCustomModel();

                        otFootNoteCustomModel.fld_Desc = "OT - " + ot.fld_Desc;
                        otFootNoteCustomModel.fld_Bilangan = ot.fld_JumlahJamOT;

                        footNoteCustomModelList.Add(otFootNoteCustomModel);
                    }

                    List<BonusPekerjaCustomModel> bonusPekerjaCustomModelList = new List<BonusPekerjaCustomModel>();

                    var workerBonusRecordGroupBy = dbview.vw_BonusPekerja
                        .Where(x => x.fld_Nopkj == GetNoPkj && x.fld_Tarikh.Value.Month == PaySlipRequestForm.Month &&
                                    x.fld_Tarikh.Value.Year == PaySlipRequestForm.Year)
                        .GroupBy(x => new { x.fld_KodPkt, x.fld_Bonus, x.fld_KodAktvt })
                        .OrderBy(o => o.Key.fld_KodPkt)
                        .ThenBy(t => t.Key.fld_Bonus)
                        .Select(lg =>
                            new
                            {
                                fld_ID = lg.FirstOrDefault().fld_ID,
                                fld_Desc = lg.FirstOrDefault().fld_Desc,
                                fld_KodPkt = lg.FirstOrDefault().fld_KodPkt,
                                fld_BilanganHari = lg.Count(),
                                fld_Bonus = lg.FirstOrDefault().fld_Bonus,
                                fld_KadarByr = lg.FirstOrDefault().fld_Kadar,
                                fld_TotalAmount = lg.Sum(w => w.fld_Jumlah)
                            });

                    foreach (var ot in workerBonusRecordGroupBy)
                    {
                        BonusPekerjaCustomModel bonusPekerjaCustomModel = new BonusPekerjaCustomModel();

                        bonusPekerjaCustomModel.fld_ID = ot.fld_ID;
                        bonusPekerjaCustomModel.fld_Desc = ot.fld_Desc;
                        bonusPekerjaCustomModel.fld_BilanganHari = ot.fld_BilanganHari;
                        bonusPekerjaCustomModel.fld_KodPkt = ot.fld_KodPkt;
                        bonusPekerjaCustomModel.fld_Bonus = ot.fld_Bonus;
                        bonusPekerjaCustomModel.fld_KadarByr = ot.fld_KadarByr;
                        bonusPekerjaCustomModel.fld_TotalAmount = ot.fld_TotalAmount;

                        bonusPekerjaCustomModelList.Add(bonusPekerjaCustomModel);
                    }

                    List<CutiPekerjaCustomModel> cutiPekerjaCustomModelList = new List<CutiPekerjaCustomModel>();

                    var workerLeaveRecordGroupBy = dbview.vw_CutiPekerja
                        .Where(x => x.fld_Nopkj == GetNoPkj && x.fld_Tarikh.Value.Month == PaySlipRequestForm.Month &&
                                    x.fld_Tarikh.Value.Year == PaySlipRequestForm.Year)
                        .GroupBy(x => new { x.fld_Kdhdct })
                        .OrderBy(o => o.Key.fld_Kdhdct)
                        .Select(lg =>
                            new
                            {
                                fld_ID = lg.FirstOrDefault().fld_ID,
                                fld_Desc = lg.FirstOrDefault().fldOptConfDesc,
                                fld_BilanganHari = lg.Count(),
                                fld_KadarByr = lg.FirstOrDefault().fld_Kadar,
                                fld_TotalAmount = lg.Sum(w => w.fld_Jumlah)
                            });

                    foreach (var ot in workerLeaveRecordGroupBy)
                    {
                        CutiPekerjaCustomModel cutiPekerjaCustomModel = new CutiPekerjaCustomModel();

                        cutiPekerjaCustomModel.fld_ID = ot.fld_ID;
                        cutiPekerjaCustomModel.fld_Desc = ot.fld_Desc;
                        cutiPekerjaCustomModel.fld_BilanganHari = ot.fld_BilanganHari;
                        cutiPekerjaCustomModel.fld_KadarByr = ot.fld_KadarByr;
                        cutiPekerjaCustomModel.fld_TotalAmount = ot.fld_TotalAmount;

                        cutiPekerjaCustomModelList.Add(cutiPekerjaCustomModel);
                    }


                    var workerWorkingDay = dbview.vw_KehadiranPekerja
                        .Where(x => x.fld_Nopkj == GetNoPkj && x.fld_Tarikh.Value.Month == PaySlipRequestForm.Month &&
                                    x.fld_Tarikh.Value.Year == PaySlipRequestForm.Year)
                        .GroupBy(x => new { x.fld_Kdhdct })
                        .OrderBy(o => o.Key.fld_Kdhdct)
                        .Select(lg =>
                            new
                            {
                                fld_Desc = lg.FirstOrDefault().fldOptConfDesc,
                                fld_Bilangan = lg.Count(),
                            });

                    foreach (var workingDay in workerWorkingDay)
                    {
                        FootNoteCustomModel footNoteCustomModel = new FootNoteCustomModel();

                        footNoteCustomModel.fld_Desc = workingDay.fld_Desc;
                        footNoteCustomModel.fld_Bilangan = workingDay.fld_Bilangan;

                        footNoteCustomModelList.Add(footNoteCustomModel);
                    }

                    var workerRainDay = dbview.vw_KehadiranPekerja
                        .Count(x => x.fld_Nopkj == GetNoPkj && x.fld_Tarikh.Value.Month == PaySlipRequestForm.Month &&
                                    x.fld_Tarikh.Value.Year == PaySlipRequestForm.Year && x.fld_Hujan == 1);

                    if (workerRainDay != 0)
                    {
                        FootNoteCustomModel footNoteHariHujanCustomModel = new FootNoteCustomModel();

                        footNoteHariHujanCustomModel.fld_Desc = "Total Raining Days";
                        footNoteHariHujanCustomModel.fld_Bilangan = workerRainDay;

                        footNoteCustomModelList.Add(footNoteHariHujanCustomModel);
                    }

                    PaySlipPekerja.Pkjmast = workerDataSingle;
                    PaySlipPekerja.GajiBulanan = workerMonthlySalary;
                    PaySlipPekerja.InsentifPekerja = workerIncentiveRecordList;
                    PaySlipPekerja.KerjaPekerja = kerjaPekerjaCustomModelList;
                    PaySlipPekerja.OTPekerja = otPekerjaCustomModelList;
                    PaySlipPekerja.BonusPekerja = bonusPekerjaCustomModelList;
                    PaySlipPekerja.CutiPekerja = cutiPekerjaCustomModelList;
                    PaySlipPekerja.FootNote = footNoteCustomModelList;
                    PaySlipPekerja.CarumanTambahan = carumanTambahanCustomModelList;
                }

            }
            return Json(PaySlipPekerja);
        }
    }
}
