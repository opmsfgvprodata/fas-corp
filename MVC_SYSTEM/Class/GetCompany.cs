using MVC_SYSTEM.AuthModels;
using MVC_SYSTEM.Models;
using MVC_SYSTEM.ModelsCorporate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.Class
{
    public class GetCompany
    {
        private MVC_SYSTEM_Auth db = new MVC_SYSTEM_Auth();
        private MVC_SYSTEM_Models db2 = new MVC_SYSTEM_Models();
        private MVC_SYSTEM_ModelsCorporate dbCorp = new MVC_SYSTEM_ModelsCorporate();
        public int[] GetWilayahID(int? SyarikatID)
        {
            IEnumerable<int> enumerable = db.tbl_Wilayah.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).OrderBy(o => o.fld_ID).Select(s => s.fld_ID).ToArray();
            int[] wlyhid = enumerable.ToArray();

            return wlyhid;
        }

        //add by faeza
        public int? GetWilayahID3(int LadangID)
        {
            int? WilayahID;

            WilayahID = db.tbl_Ladang.Where(x => x.fld_ID == LadangID).Select(s => s.fld_WlyhID).FirstOrDefault();

            return WilayahID;
        }

        public int[] GetWilayahID2(int? SyarikatID, int? WilayahID)
        {
            IEnumerable<int> enumerable = db.tbl_Wilayah.Where(x => x.fld_ID == WilayahID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).Select(s => s.fld_ID).ToArray();
            int[] wlyhid = enumerable.ToArray();

            return wlyhid;
        }

        public int?[] GetWilayahIDForApplicationSupport(int? NegaraID, int? SyarikatID, int? WilayahID, int? month, int? year)
        {
            IEnumerable<int?> enumerable = db2.vw_PermohonanWang.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_Month == month && x.fld_Year == year).Select(s => s.fld_WilayahID).Distinct().ToArray();
            int?[] wlyhid = enumerable.ToArray();

            return wlyhid;
        }

        public int?[] GetWilayahIDForApplicationSupport2(int? NegaraID, int? SyarikatID, int? WilayahID, int? month, int? year)
        {
            IEnumerable<int?> enumerable = dbCorp.vw_PermohonanKewangan.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_Month == month && x.fld_Year == year).Select(s => s.fld_WilayahID).Distinct().ToArray();
            int?[] wlyhid = enumerable.ToArray();

            return wlyhid;
        }

        public int?[] GetWilayahIDForApplicationSupport2(int? NegaraID, int? SyarikatID, int? month, int? year)
        {
            IEnumerable<int?> enumerable = db2.tbl_SokPermhnWang.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Month == month && x.fld_Year == year).Select(s => s.fld_WilayahID).Distinct().ToArray();
            int?[] wlyhid = enumerable.ToArray();

            return wlyhid;
        }

        public string GetWilayahName(int wlyhid)
        {
            string name;

            name = db.tbl_Wilayah.Where(x => x.fld_ID == wlyhid).Select(s => s.fld_WlyhName).FirstOrDefault();

            return name;
        }

        public string GetWilayahName2(int wlyhid)
        {
            string name = "";

            if (wlyhid != 0)
            {
                name = db.tbl_Wilayah.Where(x => x.fld_ID == wlyhid).Select(s => s.fld_WlyhName).FirstOrDefault();
            }
            else
            {
                name = "HQ";
            }

            return name;
        }

        public bool GetAvailableCompany(int? NegaraID)
        {
            bool result = false;

            var getcompany = db.tbl_Syarikat.Where(x => x.fld_Deleted == false && x.fld_NegaraID==1).Count();

            if (getcompany > 0)
            {
                result = true;
            }

            return result;
        }
    }
}