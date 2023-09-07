using MVC_SYSTEM.ModelsMobileAPI.ModelsCustom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MVC_SYSTEM.ModelsEstate;
using MVC_SYSTEM.ModelsCorporate;
using MVC_SYSTEM.Security;

namespace MVC_SYSTEM.Class
{
    public class GetMasterData
    {
        public List<ModelsMobileAPI.ModelsCustom.tbl_Division> tbl_Division(int fld_KmplnSyrktID, int fldNegaraID, int fldSyarikatID, int fldWilayahID, int fldLadangID)
        {
            MVC_SYSTEM_ModelsCorporate db = new MVC_SYSTEM_ModelsCorporate();
            int ID = 1;
            List<ModelsMobileAPI.ModelsCustom.tbl_Division> tbl_DivisionList = new List<ModelsMobileAPI.ModelsCustom.tbl_Division>();

            var Divisions = db.vw_NSWL_2.Where(x => x.fld_NegaraID == fldNegaraID && x.fld_SyarikatID == fldSyarikatID && x.fld_WilayahID == fldWilayahID && x.fld_LadangID == fldLadangID && x.fld_Deleted_D == false).ToList();

            foreach (var Division in Divisions)
            {
                tbl_DivisionList.Add(new ModelsMobileAPI.ModelsCustom.tbl_Division() { fld_DivisionID = Division.fld_DivisionID, fld_DivisionName = Division.fld_DivisionName });

                ID++;
            }
            db.Dispose();
            return tbl_DivisionList;
        }

        public List<tbl_KumpulanPkj> tbl_KumpulanPkj(int fld_KmplnSyrktID, int fldNegaraID, int fldSyarikatID, int fldWilayahID, int fldLadangID, int fldDivisionID)
        {
            Connection Connection = new Connection();
            MVC_SYSTEM_ModelsEstate db = Connection.GetConnectionMobile(fldWilayahID, fldSyarikatID, fldNegaraID);
            int ID = 1;
            List<tbl_KumpulanPkj> tbl_KumpulanPkjList = new List<tbl_KumpulanPkj>();

            var KumpulanPkjs = db.tbl_KumpulanKerja.Where(x => x.fld_deleted == false && x.fld_NegaraID == fldNegaraID && x.fld_SyarikatID == fldSyarikatID && x.fld_WilayahID == fldWilayahID && x.fld_LadangID == fldLadangID && x.fld_DivisionID == fldDivisionID).ToList();

            foreach (var KumpulanPkj in KumpulanPkjs)
            {
                tbl_KumpulanPkjList.Add(new tbl_KumpulanPkj() { fld_KumpulanID = KumpulanPkj.fld_KumpulanID, fld_KodKumpulan = KumpulanPkj.fld_KodKumpulan.Trim(), fld_Keterangan = KumpulanPkj.fld_Keterangan.Trim(), fld_NegaraID = fldNegaraID, fld_SyarikatID = fldSyarikatID, fld_WilayahID = fldWilayahID, fld_LadangID = fldLadangID, fld_DivisionID = fldDivisionID });

                ID++;
            }

            db.Dispose();
            return tbl_KumpulanPkjList;
        }

        public List<tbl_PkjMast> tbl_PkjMast(int fld_KmplnSyrktID, int fldNegaraID, int fldSyarikatID, int fldWilayahID, int fldLadangID, int fldDivisionID)
        {
            Connection Connection = new Connection();
            MVC_SYSTEM_ModelsEstate db = Connection.GetConnectionMobile(fldWilayahID, fldSyarikatID, fldNegaraID);
            int ID = 1;
            List<tbl_PkjMast> tbl_PkjMastList = new List<tbl_PkjMast>();
            string KumpulanCode = "";

            var PkjMasts = db.tbl_Pkjmast.Where(x => x.fld_NegaraID == fldNegaraID && x.fld_SyarikatID == fldSyarikatID && x.fld_WilayahID == fldWilayahID && x.fld_LadangID == fldLadangID && x.fld_StatusApproved == 1 && x.fld_Kdaktf == "1" && x.fld_KumpulanID != null && x.fld_DivisionID == fldDivisionID).ToList();

            foreach (var PkjMast in PkjMasts)
            {
                KumpulanCode = db.tbl_KumpulanKerja.Where(x => x.fld_NegaraID == fldNegaraID && x.fld_SyarikatID == fldSyarikatID && x.fld_WilayahID == fldWilayahID && x.fld_LadangID == fldLadangID && x.fld_KumpulanID == PkjMast.fld_KumpulanID).Select(s => s.fld_KodKumpulan).FirstOrDefault();
                tbl_PkjMastList.Add(new tbl_PkjMast() { fld_PkjMastID = ID, fld_Nopkj = PkjMast.fld_Nopkj.Trim(), fld_Nama = PkjMast.fld_Nama.Trim(), fld_KumpulanID = PkjMast.fld_KumpulanID, fld_KodKumpulan = KumpulanCode, fld_NegaraID = fldNegaraID, fld_SyarikatID = fldSyarikatID, fld_WilayahID = fldWilayahID, fld_LadangID = fldLadangID, fld_DivisionID = fldDivisionID });

                ID++;
            }
            db.Dispose();
            return tbl_PkjMastList;
        }

        public List<tbl_LeaveQouta> tbl_CutiPeruntukan(int fld_KmplnSyrktID, int fldNegaraID, int fldSyarikatID, int fldWilayahID, int fldLadangID, int fldDivisionID)
        {
            Connection Connection = new Connection();
            ChangeTimeZone ChangeTimeZone = new ChangeTimeZone();
            MVC_SYSTEM_ModelsEstate db = Connection.GetConnectionMobile(fldWilayahID, fldSyarikatID, fldNegaraID);
            int ID = 1;
            int Year = 0;
            List<tbl_LeaveQouta> tbl_LeaveQoutaList = new List<tbl_LeaveQouta>();

            var PkjMasts = db.tbl_Pkjmast.Where(x => x.fld_NegaraID == fldNegaraID && x.fld_SyarikatID == fldSyarikatID && x.fld_WilayahID == fldWilayahID && x.fld_LadangID == fldLadangID && x.fld_StatusApproved == 1 && x.fld_Kdaktf == "1" && x.fld_KumpulanID != null && x.fld_DivisionID == fldDivisionID).Select(s => s.fld_Nopkj).ToList();

            Year = ChangeTimeZone.gettimezone().Year;
            var LeaveQoutas = db.tbl_CutiPeruntukan.Where(x => x.fld_NegaraID == fldNegaraID && x.fld_SyarikatID == fldSyarikatID && x.fld_WilayahID == fldWilayahID && x.fld_LadangID == fldLadangID && x.fld_Tahun == Year && PkjMasts.Contains(x.fld_NoPkj)).ToList();

            foreach (var LeaveQouta in LeaveQoutas)
            {
                tbl_LeaveQoutaList.Add(new tbl_LeaveQouta() { fld_CutiPeruntukanID = ID, fld_NoPkj = LeaveQouta.fld_NoPkj, fld_KodCuti = LeaveQouta.fld_KodCuti, fld_Tahun = LeaveQouta.fld_Tahun, fld_JumlahCuti = LeaveQouta.fld_JumlahCuti, fld_JumlahCutiDiambil = LeaveQouta.fld_JumlahCutiDiambil, fld_NegaraID = fldNegaraID, fld_SyarikatID = fldSyarikatID, fld_WilayahID = fldWilayahID, fld_LadangID = fldLadangID });

                ID++;
            }
            db.Dispose();
            return tbl_LeaveQoutaList;
        }

        public List<tbl_JenisKhdrn> tbl_JenisKhdrn(int fld_KmplnSyrktID, int fldNegaraID, int fldSyarikatID, int fldWilayahID, int fldLadangID)
        {
            MVC_SYSTEM_ModelsCorporate db = new MVC_SYSTEM_ModelsCorporate();
            int ID = 1;
            List<tbl_JenisKhdrn> tbl_JenisKhdrnList = new List<tbl_JenisKhdrn>();

            var JenisKhdrns = db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "cuti" && x.fldDeleted == false && x.fld_NegaraID == fldNegaraID && x.fld_SyarikatID == fldSyarikatID).ToList();

            foreach (var JenisKhdrn in JenisKhdrns)
            {
                tbl_JenisKhdrnList.Add(new tbl_JenisKhdrn() { fld_JenisKhdrnID = ID, fld_JenisKhdrnKod = JenisKhdrn.fldOptConfValue, fld_Keterangan = JenisKhdrn.fldOptConfDesc.Trim(), fld_StatusKhdran = JenisKhdrn.fldOptConfFlag2.Trim(), fld_RateMultiple = short.Parse(JenisKhdrn.fldOptConfFlag3 == null ? "0" : JenisKhdrn.fldOptConfFlag3) });

                ID++;
            }
            db.Dispose();
            return tbl_JenisKhdrnList;
        }

        public List<tbl_JenisPkt> tbl_JenisPkt(int fld_KmplnSyrktID, int fldNegaraID, int fldSyarikatID, int fldWilayahID, int fldLadangID)
        {
            MVC_SYSTEM_ModelsCorporate db = new MVC_SYSTEM_ModelsCorporate();
            int ID = 1;
            List<tbl_JenisPkt> tbl_JenisPktList = new List<tbl_JenisPkt>();

            var JenisPkts = db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "jnspkt" && x.fldDeleted == false && x.fld_NegaraID == fldNegaraID && x.fld_SyarikatID == fldSyarikatID).ToList();

            foreach (var JenisPkt in JenisPkts)
            {
                tbl_JenisPktList.Add(new tbl_JenisPkt() { fld_JenisPktID = ID, fld_JenisPktKod = byte.Parse(JenisPkt.fldOptConfValue), fld_JenisPktNama = JenisPkt.fldOptConfDesc.Trim() });

                ID++;
            }
            db.Dispose();
            return tbl_JenisPktList;
        }

        public List<tbl_Lajer> tbl_Lajer(int fld_KmplnSyrktID, int fldNegaraID, int fldSyarikatID, int fldWilayahID, int fldLadangID)
        {
            MVC_SYSTEM_ModelsCorporate db = new MVC_SYSTEM_ModelsCorporate();
            int ID = 1;
            List<tbl_Lajer> tbl_LajerList = new List<tbl_Lajer>();

            var Lajers = db.tbl_Lejar.Where(x => x.fld_NegaraID == fldNegaraID && x.fld_SyarikatID == fldSyarikatID && x.fld_Deleted == false).ToList();

            foreach (var Lajer in Lajers)
            {
                tbl_LajerList.Add(new tbl_Lajer() { fld_LajerID = ID, fld_LajerKod = Lajer.fld_KodLejar, fld_Keterangan = Lajer.fld_Desc.Trim() });

                ID++;
            }
            db.Dispose();
            return tbl_LajerList;
        }

        public List<tbl_Pkt> tbl_Pkt(int fld_KmplnSyrktID, int fldNegaraID, int fldSyarikatID, int fldWilayahID, int fldLadangID, int fldDivisionID)
        {
            ChangeTimeZone timezone = new ChangeTimeZone();
            Connection Connection = new Connection();
            MVC_SYSTEM_ModelsEstate db = Connection.GetConnectionMobile(fldWilayahID, fldSyarikatID, fldNegaraID);
            int ID = 1;
            List<tbl_Pkt> tbl_PktList = new List<tbl_Pkt>();

            var PktUtamas = db.tbl_PktUtama.Where(x => x.fld_NegaraID == fldNegaraID && x.fld_SyarikatID == fldSyarikatID && x.fld_WilayahID == fldWilayahID && x.fld_LadangID == fldLadangID && x.fld_DivisionID == fldDivisionID && x.fld_Deleted == false).ToList();

            foreach (var PktUtama in PktUtamas)
            {
                tbl_PktList.Add(new tbl_Pkt() { fld_PktID = ID, fld_JenisPktKod = 1, fld_PktKod = PktUtama.fld_PktUtama, fld_PktNama = PktUtama.fld_NamaPktUtama, fld_NegaraID = fldNegaraID, fld_SyarikatID = fldSyarikatID, fld_WilayahID = fldWilayahID, fld_LadangID = fldLadangID, fld_JnsLot = PktUtama.fld_JnsLot, fld_DivisionID = fldDivisionID });
                ID++;
            }

            var PktSubs = db.tbl_SubPkt.Where(x => x.fld_NegaraID == fldNegaraID && x.fld_SyarikatID == fldSyarikatID && x.fld_WilayahID == fldWilayahID && x.fld_LadangID == fldLadangID && x.fld_Deleted == false).ToList();
            if (PktSubs.Count() > 0)
            {
                ID++;
                foreach (var PktSub in PktSubs)
                {
                    var GetJnsLot = PktUtamas.Where(x => x.fld_PktUtama == PktSub.fld_KodPktUtama).Select(s => s.fld_JnsLot).FirstOrDefault();
                    tbl_PktList.Add(new tbl_Pkt() { fld_PktID = ID, fld_JenisPktKod = 2, fld_PktKod = PktSub.fld_Pkt, fld_PktNama = PktSub.fld_NamaPkt, fld_NegaraID = fldNegaraID, fld_SyarikatID = fldSyarikatID, fld_WilayahID = fldWilayahID, fld_LadangID = fldLadangID, fld_JnsLot = GetJnsLot, fld_DivisionID = fldDivisionID });
                    ID++;
                }
            }

            var PktBloks = db.tbl_Blok.Where(x => x.fld_NegaraID == fldNegaraID && x.fld_SyarikatID == fldSyarikatID && x.fld_WilayahID == fldWilayahID && x.fld_LadangID == fldLadangID && x.fld_Deleted == false).ToList();
            if (PktBloks.Count() > 0)
            {
                ID++;
                foreach (var PktBlok in PktBloks)
                {
                    var GetJnsLot = PktUtamas.Where(x => x.fld_PktUtama == PktBlok.fld_KodPktutama).Select(s => s.fld_JnsLot).FirstOrDefault();
                    tbl_PktList.Add(new tbl_Pkt() { fld_PktID = ID, fld_JenisPktKod = 3, fld_PktKod = PktBlok.fld_Blok, fld_PktNama = PktBlok.fld_NamaBlok, fld_NegaraID = fldNegaraID, fld_SyarikatID = fldSyarikatID, fld_WilayahID = fldWilayahID, fld_LadangID = fldLadangID, fld_JnsLot = GetJnsLot, fld_DivisionID = fldDivisionID });
                    ID++;
                }
            }
            db.Dispose();
            return tbl_PktList;
        }


        public List<tbl_MapGLSAP> tbl_MapGL(int fld_KmplnSyrktID, int fldNegaraID, int fldSyarikatID, int fldWilayahID, int fldLadangID)
        {
            MVC_SYSTEM_ModelsCorporate db = new MVC_SYSTEM_ModelsCorporate();
            int ID = 1;
            List<tbl_MapGLSAP> tbl_MapGLSAPList = new List<tbl_MapGLSAP>();

            var MapGLs = db.tbl_MapGL.Where(x => x.fld_Deleted == false && x.fld_NegaraID == fldNegaraID && x.fld_SyarikatID == fldSyarikatID).ToList();

            foreach (var MapGL in MapGLs)
            {
                tbl_MapGLSAPList.Add(new tbl_MapGLSAP() { fld_MapGLID = ID, fld_AktvtKod = MapGL.fld_KodAktvt.Trim(), fld_KodGL = MapGL.fld_KodGL.Trim() });
                ID++;
            }
            db.Dispose();
            return tbl_MapGLSAPList;
        }

        public List<tbl_AktvtKod> tbl_AktvtKod(int fld_KmplnSyrktID, int fldNegaraID, int fldSyarikatID, int fldWilayahID, int fldLadangID)
        {
            MVC_SYSTEM_ModelsCorporate db = new MVC_SYSTEM_ModelsCorporate();
            int ID = 1;
            List<tbl_AktvtKod> tbl_AktvtKodList = new List<tbl_AktvtKod>();
            
            var KodAtvts = db.tbl_UpahAktiviti.Where(x => x.fld_Deleted == false && x.fld_NegaraID == fldNegaraID && x.fld_SyarikatID == fldSyarikatID).ToList();

            foreach (var KodAtvt in KodAtvts)
            {
                tbl_AktvtKodList.Add(new tbl_AktvtKod() { fld_AktvtKodID = ID, fld_AktvtKod = KodAtvt.fld_KodAktvt, fld_Keterangan = KodAtvt.fld_Desc, fld_MaxProduktiviti = KodAtvt.fld_MaxProduktiviti, fld_Unit = KodAtvt.fld_Unit, fld_JenisAktvtKod = KodAtvt.fld_KodJenisAktvt, fld_FormatForm = KodAtvt.fld_DisabledFlag, fld_KdhMenuai = KodAtvt.fld_KdhByr, fld_Rate = KodAtvt.fld_Harga.Value });

                ID++;
            }

            db.Dispose();
            return tbl_AktvtKodList;
        }

        public List<tbl_Users> tbl_Users(int fld_KmplnSyrktID, int fldNegaraID, int fldSyarikatID, int fldWilayahID, int fldLadangID)
        {
            MVC_SYSTEM_ModelsCorporate db = new MVC_SYSTEM_ModelsCorporate();
            EncryptDecrypt EncryptDecrypt = new EncryptDecrypt();
            int ID = 1;
            string password = "";
            List<tbl_Users> tbl_UsersList = new List<tbl_Users>();

            var Users = db.tblUsers.Where(x => x.fld_KmplnSyrktID == fld_KmplnSyrktID && x.fldNegaraID == fldNegaraID && x.fldSyarikatID == fldSyarikatID && x.fldWilayahID == fldWilayahID && x.fldLadangID == fldLadangID && x.fldDeleted == false).ToList();

            foreach (var User in Users)
            {
                password = EncryptDecrypt.Decrypt(User.fldUserPassword);
                tbl_UsersList.Add(new tbl_Users() { fld_UserID = ID, fld_UserName = User.fldUserName.Trim(), fld_UserPassword = password, fld_RoleID = User.fldRoleID, fld_NegaraID = fldNegaraID, fld_SyarikatID = fldSyarikatID, fld_WilayahID = fldWilayahID, fld_LadangID = fldLadangID });

                ID++;
            }
            db.Dispose();
            return tbl_UsersList;
        }

        public List<tbl_PublicHolidayDate> tbl_PublicHolidayDate(int fld_KmplnSyrktID, int fldNegaraID, int fldSyarikatID, int fldWilayahID, int fldLadangID)
        {
            MVC_SYSTEM_ModelsCorporate db = new MVC_SYSTEM_ModelsCorporate();
            int ID = 1;
            List<tbl_PublicHolidayDate> tbl_PublicHolidayDateList = new List<tbl_PublicHolidayDate>();
            var GetPublicHolidays = db.vw_CutiUmumLdgDetails.Where(x => x.fld_NegaraID == fldNegaraID && x.fld_SyarikatID == fldSyarikatID && x.fld_WilayahID == fldWilayahID && x.fld_LadangID == fldLadangID && x.fld_CutiUmumDeleted == false && x.fld_Deleted == false).ToList();

            foreach (var GetPublicHoliday in GetPublicHolidays)
            {
                tbl_PublicHolidayDateList.Add(new tbl_PublicHolidayDate() { fld_PublicHolidayDateID = ID, fld_Date = GetPublicHoliday.fld_TarikhCuti.Value, fld_Desc = GetPublicHoliday.fld_KeteranganCuti, fld_NegaraID = fldNegaraID, fld_SyarikatID = fldSyarikatID, fld_WilayahID = fldWilayahID, fld_LadangID = fldLadangID });
                ID++;
            }
            db.Dispose();
            return tbl_PublicHolidayDateList;
        }

        public List<tbl_CCNN> tbl_CCNN(int fld_KmplnSyrktID, int fldNegaraID, int fldSyarikatID, int fldWilayahID, int fldLadangID, int fldDivisionID)
        {
            MVC_SYSTEM_ModelsCorporate db = new MVC_SYSTEM_ModelsCorporate();
            Connection Connection = new Connection();
            MVC_SYSTEM_ModelsEstate db2 = Connection.GetConnectionMobile(fldWilayahID, fldSyarikatID, fldNegaraID);
            int ID = 1;
            List<tbl_CCNN> tbl_CCNNList = new List<tbl_CCNN>();
            string Kod_PktUtama = "";

            var GetCCNNs = db.tbl_AktivitiNNCCMap.Where(x => x.fld_NegaraID == fldNegaraID && x.fld_SyarikatID == fldSyarikatID && x.fld_WIlayahID == fldWilayahID && x.fld_LadangID == fldLadangID && x.fld_Deleted == false).ToList();

            foreach(var GetCCNN in GetCCNNs)
            {
                if (GetCCNN.fld_JenisAktiviti == "E")
                {
                    var GetWBSCode = db.tbl_SAPPDPUP.Where(x => x.fld_NegaraID == fldNegaraID && x.fld_SyarikatID == fldSyarikatID && x.fld_WilayahID == fldWilayahID && x.fld_LadangID == fldLadangID && x.fld_NetworkNo == GetCCNN.fld_NNCC && x.fld_ActivityCode == GetCCNN.fld_KodAktivitiSAP).Select(s => s.fld_WBSCode).FirstOrDefault();
                    Kod_PktUtama = db2.tbl_PktUtama.Where(x => x.fld_NegaraID == fldNegaraID && x.fld_SyarikatID == fldSyarikatID && x.fld_WilayahID == fldWilayahID && x.fld_LadangID == fldLadangID && x.fld_DivisionID == fldDivisionID && x.fld_IOcode == GetWBSCode && x.fld_JnsLot == GetCCNN.fld_JenisAktiviti && x.fld_Deleted == false).Select(s => s.fld_PktUtama).FirstOrDefault();
                }
                else
                {
                    Kod_PktUtama = db2.tbl_PktUtama.Where(x => x.fld_NegaraID == fldNegaraID && x.fld_SyarikatID == fldSyarikatID && x.fld_WilayahID == fldWilayahID && x.fld_LadangID == fldLadangID && x.fld_DivisionID == fldDivisionID && x.fld_IOcode == GetCCNN.fld_NNCC && x.fld_JnsLot == GetCCNN.fld_JenisAktiviti && x.fld_Deleted == false).Select(s => s.fld_PktUtama).FirstOrDefault();
                }
                if (Kod_PktUtama != null)
                {
                    tbl_CCNNList.Add(new tbl_CCNN { fld_CCNNID = ID, fld_CCNNCode = GetCCNN.fld_NNCC, fld_ActivitySAP = GetCCNN.fld_KodAktivitiSAP, fld_ActivityTypeCode = GetCCNN.fld_JenisAktiviti, fld_FormFlag = short.Parse(GetCCNN.fld_Flag), fld_KodActivityOPMS = GetCCNN.fld_KodAktivitiOPMS, fld_NegaraID = fldNegaraID, fld_SyarikatID = fldSyarikatID, fld_WilayahID = fldWilayahID, fld_LadangID = fldLadangID, fld_PktKodUtama = Kod_PktUtama });
                    ID++;
                }
            }
            db.Dispose();
            db2.Dispose();
            return tbl_CCNNList;
        }

        public List<tbl_ActivityType> tbl_ActivityType(int fld_KmplnSyrktID, int fldNegaraID, int fldSyarikatID, int fldWilayahID, int fldLadangID)
        {
            MVC_SYSTEM_ModelsCorporate db = new MVC_SYSTEM_ModelsCorporate();
            int ID = 1;
            List<tbl_ActivityType> tbl_ActivityTypeList = new List<tbl_ActivityType>();

            var GetJenisAktvtyDesc = db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "activityLevel" && x.fldDeleted == false && x.fld_NegaraID == fldNegaraID && x.fld_SyarikatID == fldSyarikatID).Select(s => new { s.fldOptConfValue, s.fldOptConfDesc, s.fldOptConfFlag3 }).ToList();
            var GetJenisAktvtyEst = db.tbl_AktvtEstate.Where(x => x.fld_NegaraID == fldNegaraID && x.fld_SyarikatID == fldSyarikatID && x.fld_WilayahID == fldWilayahID && x.fld_LadangID == fldLadangID).Select(s => new { s.fld_KodJnsAktvt }).ToList();
            var GetJenisAktvtys = GetJenisAktvtyEst.Join(GetJenisAktvtyDesc, j => new { a = j.fld_KodJnsAktvt }, k => new { a = k.fldOptConfValue }, (j, k) => new { k.fldOptConfValue, k.fldOptConfDesc, k.fldOptConfFlag3 }).ToList();

            foreach (var GetJenisAktvty in GetJenisAktvtys)
            {
                tbl_ActivityTypeList.Add(new tbl_ActivityType { fld_ActivityTypeID = ID, fld_ActivityTypeCode = GetJenisAktvty.fldOptConfValue, fld_Desc = GetJenisAktvty.fldOptConfDesc, fld_FormFlag = short.Parse(GetJenisAktvty.fldOptConfFlag3), fld_NegaraID = fldNegaraID, fld_SyarikatID = fldSyarikatID, fld_WilayahID = fldWilayahID, fld_LadangID = fldLadangID });
                ID++;
            }
            db.Dispose();
            return tbl_ActivityTypeList;
        }

        public List<tbl_PkjIncrementSalary> tbl_PkjIncrmntSalary(int fld_KmplnSyrktID, int fldNegaraID, int fldSyarikatID, int fldWilayahID, int fldLadangID, int fldDivisionID)
        {
            List<tbl_PkjIncrementSalary> tbl_PkjIncrementSalaryList = new List<tbl_PkjIncrementSalary>();
            Connection Connection = new Connection();
            MVC_SYSTEM_ModelsEstate db = Connection.GetConnectionMobile(fldWilayahID, fldSyarikatID, fldNegaraID);
            int ID = 1;

            var PkjMasts = db.tbl_Pkjmast.Where(x => x.fld_NegaraID == fldNegaraID && x.fld_SyarikatID == fldSyarikatID && x.fld_WilayahID == fldWilayahID && x.fld_LadangID == fldLadangID && x.fld_StatusApproved == 1 && x.fld_Kdaktf == "1" && x.fld_KumpulanID != null && x.fld_DivisionID == fldDivisionID).Select(s => s.fld_Nopkj).ToList();

            var PkjIncrements = db.tbl_PkjIncrmntSalary.Where(x => x.fld_NegaraID == fldNegaraID && x.fld_SyarikatID == fldSyarikatID && x.fld_WilayahID == fldWilayahID && x.fld_LadangID == fldLadangID && x.fld_AppStatus == true && PkjMasts.Contains(x.fld_Nopkj)).ToList();

            foreach (var PkjIncrement in PkjIncrements)
            {
                tbl_PkjIncrementSalaryList.Add(new tbl_PkjIncrementSalary { fld_PkjSalaryID = ID, fld_IncrmntSalary = PkjIncrement.fld_IncrmntSalary.Value, fld_Nopkj = PkjIncrement.fld_Nopkj, fld_NegaraID = fldNegaraID, fld_SyarikatID = fldSyarikatID, fld_WilayahID = fldWilayahID, fld_LadangID = fldLadangID });
                ID++;
            }
            db.Dispose();
            return tbl_PkjIncrementSalaryList;
        }
    }
}