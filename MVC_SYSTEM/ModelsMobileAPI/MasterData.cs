using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MVC_SYSTEM.ModelsCustom;
using MVC_SYSTEM.ModelsMobileAPI.ModelsCustom;

namespace MVC_SYSTEM.ModelsMobileAPI
{
    public class MasterData
    {
        public LoginResult LoginResult { get; set; }
        public List<tbl_PkjMast> tbl_PkjMast { get; set; }
        public List<tbl_KumpulanPkj> tbl_KumpulanPkj { get; set; }
        public List<tbl_LeaveQouta> tbl_CutiPeruntukan { get; set; }
        public List<tbl_JenisKhdrn> tbl_JenisKhdrn { get; set; }
        public List<tbl_JenisPkt> tbl_JenisPkt { get; set; }
        public List<tbl_Pkt> tbl_Pkt { get; set; }
        public List<tbl_ActivityType> tbl_ActivityType { get; set; }
        public List<tbl_CCNN> tbl_CCNN { get; set; }
        public List<tbl_MapGLSAP> tbl_MapGL { get; set; }
        public List<tbl_AktvtKod> tbl_AktvtKod { get; set; }
        public List<tbl_Lajer> tbl_Lajer { get; set; }
        public List<tbl_PublicHolidayDate> tbl_PublicHolidayDate { get; set; }
        public List<tbl_PkjIncrementSalary> tbl_PkjIncrementSalary { get; set; }
    }
}