using MVC_SYSTEM.AuthModels;
using MVC_SYSTEM.Models;
using MVC_SYSTEM.ModelsCorporate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.Class
{
    public class GetNSWL
    {
        private MVC_SYSTEM_Auth db2 = new MVC_SYSTEM_Auth();
        GetIdentity getidentity = new GetIdentity();
        GetWilayah getwilyah = new GetWilayah();

        //new Model
        private MVC_SYSTEM_ModelsCorporate db = new MVC_SYSTEM_ModelsCorporate();

        public void GetData(out int ? NegaraID, out int ? SyarikatID, out int?  WilayahID, out int ? LadangID, int ? userid, string username)
        {
            NegaraID = 0;
            SyarikatID = 0;
            WilayahID = 0;
            LadangID = 0;

            if (getidentity.SuperPowerAdmin(username) || getidentity.SuperAdmin(username))
            {
                var getcountycompany = db2.tbl_SuperAdminSelection.Where(x => x.fld_SuperAdminID == userid).FirstOrDefault();
                NegaraID = getcountycompany.fld_NegaraID;
                SyarikatID = getcountycompany.fld_SyarikatID;
                WilayahID = 0;
                LadangID = 0;
            }
            else if (getidentity.Admin1(username))
            {
                var getcountycompany = db2.tblUsers.Where(x => x.fldUserID == userid).FirstOrDefault();
                NegaraID = getcountycompany.fldNegaraID;
                SyarikatID = getcountycompany.fldSyarikatID;
                WilayahID = 0;
                LadangID = 0;
            }
            else if (getidentity.Admin2(username))
            {
                var getcountycompany = db2.tblUsers.Where(x => x.fldUserID == userid).FirstOrDefault();
                NegaraID = getcountycompany.fldNegaraID;
                SyarikatID = getcountycompany.fldSyarikatID;
                WilayahID = getcountycompany.fldWilayahID;
                LadangID = 0;
            }
            else if (getidentity.SuperPowerUser(username))
            {
                var getcountycompany = db2.tblUsers.Where(x => x.fldUserID == userid).FirstOrDefault();
                NegaraID = getcountycompany.fldNegaraID;
                SyarikatID = getcountycompany.fldSyarikatID;
                WilayahID = getcountycompany.fldWilayahID;
                LadangID = getcountycompany.fldLadangID;
            }
            else if (getidentity.SuperUser(username) || getidentity.NormalUser(username))
            {
                var getcountycompany = db2.tblUsers.Where(x => x.fldUserID == userid).FirstOrDefault();
                NegaraID = getcountycompany.fldNegaraID;
                SyarikatID = getcountycompany.fldSyarikatID;
                WilayahID = getcountycompany.fldWilayahID;
                LadangID = getcountycompany.fldLadangID;
            }
        }

        public ModelsCorporate.vw_NSWL GetLadangDetail(int LadangID)
        {
            ModelsCorporate.vw_NSWL NSWL = new ModelsCorporate.vw_NSWL();

            NSWL = db.vw_NSWL.Where(x => x.fld_LadangID == LadangID).FirstOrDefault();

            db.Dispose();

            return NSWL;
        }

        public ModelsCorporate.vw_NSWL GetLadangDetail(string kdprmhnan, string kdldg)
        {
            ModelsCorporate.vw_NSWL NSWL = new ModelsCorporate.vw_NSWL();

            NSWL = db.vw_NSWL.Where(x => x.fld_LdgCode == kdldg && x.fld_RequestCode == kdprmhnan).FirstOrDefault();

            db.Dispose();

            return NSWL;
        }

        public ModelsCorporate.vw_NSWL GetLadangDetailByKodLadang(string KodLadang)
        {
            ModelsCorporate.vw_NSWL NSWL = new ModelsCorporate.vw_NSWL();

            NSWL = db.vw_NSWL.Where(x => x.fld_LdgCode == KodLadang).FirstOrDefault();

            db.Dispose();

            return NSWL;
        }

        public int? GetDivisionSelection(int? userid, int? NegaraID, int? SyarikatID, int? WilayahID, int? LadangID)
        {
            int? DivisionID = 0;

            var GetDivision = db.tbl_EstateDivisionSelection.Where(x => x.fld_UserID == userid && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Select(s => s.fld_DivisionID).FirstOrDefault();

            if (GetDivision > 0)
            {
                DivisionID = GetDivision;
            }
            else
            {
                DivisionID = 0;
            }

            return DivisionID;
        }

        public List<vw_NSWL_2> GetLadangDetailByRegion(string CompCode)
        {
            List<vw_NSWL_2> NSWL = new List<vw_NSWL_2>();

            NSWL = db.vw_NSWL_2.Where(x => x.fld_CostCentre == CompCode && x.fld_Deleted_L == false).ToList();

            db.Dispose();

            return NSWL;
        }

        //Baru
        public List<ModelsCorporate.vw_NSWL> GetAllLadangDetail()
        {
            var NSWL = db.vw_NSWL.ToList();

            db.Dispose();

            return NSWL;
        }
        //fatin added - 17/11/2023
        public void GetSyarikatRCMSDetail(string compcode, out string CorpID, out string ClientID, out string AccNo, out string InitialName)
        {
            var getsyarikat = db.tbl_Syarikat.Where(x => x.fld_NamaPndkSyarikat == compcode).FirstOrDefault();
            CorpID = getsyarikat.fld_CorporateID;
            ClientID = getsyarikat.fld_ClientBatchID;
            AccNo = getsyarikat.fld_AccountNo;
            InitialName = getsyarikat.fld_NamaPndkSyarikat;
        }
    }
}