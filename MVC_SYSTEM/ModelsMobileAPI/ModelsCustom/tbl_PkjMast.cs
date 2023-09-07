using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.ModelsMobileAPI.ModelsCustom
{
    public class tbl_PkjMast
    {
        [Key]
        public int fld_PkjMastID { get; set; }

        public string fld_Nopkj { get; set; }

        public string fld_Nama { get; set; }

        public int? fld_KumpulanID { get; set; }

        public string fld_KodKumpulan { get; set; }

        public int fld_NegaraID { get; set; }

        public int fld_SyarikatID { get; set; }

        public int fld_WilayahID { get; set; }

        public int fld_LadangID { get; set; }

        public int fld_DivisionID { get; set; }
    }
}