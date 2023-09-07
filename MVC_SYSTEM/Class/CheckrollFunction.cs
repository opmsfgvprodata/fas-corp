using MVC_SYSTEM.ModelsCorporate;
using MVC_SYSTEM.ModelsEstate;
using MVC_SYSTEM.ModelsMobileAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.Class
{
    public class CheckrollFunction
    {
        private MVC_SYSTEM_ModelsCorporate db = new MVC_SYSTEM_ModelsCorporate();
        private ChangeTimeZone timezone = new ChangeTimeZone();
        private GetConfig GetConfig = new GetConfig();

        public void SaveDataKerjaSAP(MVC_SYSTEM_ModelsEstate dbr, List<tbl_Kerja> DataKerja, List<WorkData> WorkDataList, int? NegaraID, int? SyarikatID, int? WilayahID, int? LadangID)
        {
            List<tbl_KerjaSAPData> KerjaSAPDatas = new List<tbl_KerjaSAPData>();

            foreach (var EachDataKerja in DataKerja)
            {
                var GetSAPData = WorkDataList.Where(x => x.fld_Nopkj == EachDataKerja.fld_Nopkj && x.fld_Tarikh == EachDataKerja.fld_Tarikh && x.fld_KodActivityOPMS == EachDataKerja.fld_KodAktvt && x.fld_JenisPktKod == EachDataKerja.fld_JnsPkt && x.fld_PktKod == EachDataKerja.fld_KodPkt).FirstOrDefault();
                KerjaSAPDatas.Add(new tbl_KerjaSAPData { fld_GLKod = GetSAPData.fld_KodGL, fld_IOKod = null, fld_KerjaID = EachDataKerja.fld_ID, fld_PaySheetID = null, fld_NegaraID = NegaraID, fld_SyarikatID = SyarikatID, fld_WilayahID = WilayahID, fld_LadangID = LadangID, fld_NNCC = GetSAPData.fld_CCNNCode, fld_KodAktivitiSAP = GetSAPData.fld_ActivitySAP });
            }

            dbr.tbl_KerjaSAPData.AddRange(KerjaSAPDatas);
            dbr.SaveChanges();

        }

        public bool LeaveCalBal(MVC_SYSTEM_ModelsEstate dbr, int year, string NoPkj, string KodKatCuti, int? NegaraID, int? SyarikatID, int? WilayahID, int? LadangID)
        {
            bool result = false;

            var getdata = dbr.tbl_CutiPeruntukan.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_NoPkj == NoPkj && x.fld_KodCuti == KodKatCuti).Select(s => s.fld_JumlahCuti - s.fld_JumlahCutiDiambil).FirstOrDefault();

            result = getdata > 0 ? true : false;

            return result;
        }

        public void LeaveDeduct(MVC_SYSTEM_ModelsEstate dbr, int year, string NoPkj, string KodKatCuti, int? NegaraID, int? SyarikatID, int? WilayahID, int? LadangID)
        {
            var getdata = dbr.tbl_CutiPeruntukan.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_NoPkj == NoPkj && x.fld_KodCuti == KodKatCuti).FirstOrDefault();
            getdata.fld_JumlahCutiDiambil = getdata.fld_JumlahCutiDiambil + 1;
            dbr.SaveChanges();
        }

        public bool GetStatusCutProcess(MVC_SYSTEM_ModelsEstate dbr, DateTime? DateSelected, int? NegaraID, int? SyarikatID, int? WilayahID, int? LadangID)
        {
            bool result = false;
            DateTime NowDate = timezone.gettimezone();
            DateTime FirstDayNowDate = new DateTime(NowDate.Year, NowDate.Month, 1);
            DateTime FirstDaySelectedDate = new DateTime(DateSelected.Value.Year, DateSelected.Value.Month, 1);
            int GetCutOfDateDay = int.Parse(GetConfig.GetWebConfigValue("haritrakhir", NegaraID, SyarikatID));
            DateTime CutOfDate = FirstDayNowDate.AddDays(GetCutOfDateDay);

            var getTtpUrsNiaga = dbr.tbl_TutupUrusNiaga.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Year == DateSelected.Value.Year && x.fld_Month == DateSelected.Value.Month).FirstOrDefault();

            if (FirstDayNowDate == FirstDaySelectedDate && getTtpUrsNiaga != null)
            {
                result = getTtpUrsNiaga.fld_StsTtpUrsNiaga == true ? true : false;
            }
            else if (FirstDayNowDate == FirstDaySelectedDate && getTtpUrsNiaga == null)
            {
                result = false;
            }
            else if (FirstDayNowDate > FirstDaySelectedDate && (getTtpUrsNiaga != null || getTtpUrsNiaga == null))
            {
                if (NowDate >= CutOfDate && (getTtpUrsNiaga == null || getTtpUrsNiaga != null))
                {
                    result = true;
                }
                else if (NowDate < CutOfDate && getTtpUrsNiaga != null)
                {
                    result = getTtpUrsNiaga.fld_StsTtpUrsNiaga == true ? true : false;
                }
                else if (NowDate < CutOfDate && getTtpUrsNiaga == null)
                {
                    result = false;
                }
            }

            return result;
        }

        public bool CheckLeaveType(string KodKatCuti, int? NegaraID, int? SyarikatID)
        {
            bool result = false;

            var getdata = db.tbl_CutiKategori.Where(x => x.fld_KodCuti == KodKatCuti && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).Count();

            result = getdata > 0 ? true : false;

            return result;
        }

        public bool GetCutiAmMgguMatchDate(string CodeNegeri, int? NegaraID, int? SyarikatID, DateTime SelectedDate, string CutiCode, out string Msg)
        {
            bool Result = true;
            Msg = "";
            if (CutiCode == "C01" || CutiCode == "H03")
            {
                string CodeNegeriConvrt = CodeNegeri;
                var CheckHoliday = db.tbl_CutiUmum.Where(x => x.fld_TarikhCuti == SelectedDate && x.fld_Negeri == CodeNegeriConvrt && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_IsSelected == true).FirstOrDefault();
                Result = CheckHoliday != null ? true : false;
                Msg = "Tarikh pilihan bukan cuti am";
            }
            //else if (CutiCode == "C07" || CutiCode == "H02")
            //{
            //    int getday = (int)SelectedDate.DayOfWeek;
            //    var CheckData = db.tbl_MingguNegeri.Where(x => x.fld_NegeriID == CodeNegeri && x.fld_JenisMinggu == getday && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).FirstOrDefault();
            //    Result = CheckData != null ? true : false;
            //    Msg = "Tarikh pilihan bukan hari minggu";
            //}
            else
            {
                string CodeNegeriConvrt = CodeNegeri;
                var CheckHoliday = db.tbl_CutiUmum.Where(x => x.fld_TarikhCuti == SelectedDate && x.fld_Negeri == CodeNegeriConvrt && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_IsSelected == true).FirstOrDefault();
                if (CheckHoliday != null)
                {
                    int getday = (int)SelectedDate.DayOfWeek;
                    var CheckData = db.tbl_MingguNegeri.Where(x => x.fld_NegeriID == int.Parse(CodeNegeri) && x.fld_JenisMinggu == getday && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).FirstOrDefault();
                    if (CheckHoliday == null && CheckData == null)
                    {
                        Result = true;
                    }
                    else
                    {
                        if (CheckHoliday != null)
                        {
                            Msg = "Tarikh pilihan adalah cuti am";
                            Result = false;
                        }
                        //else if (CheckData != null)
                        //{
                        //    Msg = "Tarikh pilihan adalah cuti mingguan";
                        //    Result = false;
                        //}
                        else
                        {
                            Result = true;
                        }


                    }
                }
                else
                {
                    Result = true;
                }
            }
            return Result;
        }
    }
}