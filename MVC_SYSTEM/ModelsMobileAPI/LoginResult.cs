using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.ModelsMobileAPI
{
    public class LoginResult
    {
        public int LoginStatus { get; set; }

        public int fld_ProfileID { get; set; }

        public int? fld_KmplnSyrktID { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public int? fld_WilayahID { get; set; }

        public int? fld_LadangID { get; set; }

        public string fld_KodLadang { get; set; }

        public string fld_NamaLadang { get; set; }

        public string RemarkStatus { get; set; }
    }
}